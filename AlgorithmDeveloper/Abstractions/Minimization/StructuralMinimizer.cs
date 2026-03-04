using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmDeveloper.Abstractions.Minimization
{
    /// <summary>
    /// Инструмент для структурной минимизации моделей AbstractAutomata.
    /// Удаляет недостижимые вершины, спрямляет цепочки переходов, устраняет избыточные условия
    /// и объединяет эквивалентные условные вершины.
    /// </summary>
    public static class StructuralMinimizer
    {
        /// <summary>
        /// Выполняет структурную минимизацию модели автомата.
        /// Создает и возвращает новую минимизированную модель, не изменяя исходную.
        /// </summary>
        /// <param name="sourceModel">Исходная модель для минимизации.</param>
        /// <returns>Новая минимизированная модель.</returns>
        public static AbstractAutomata Minimize(AbstractAutomata sourceModel)
        {
            if (sourceModel == null) throw new ArgumentNullException(nameof(sourceModel));
            
            // Работаем с клоном
            var model = sourceModel.Clone();
            
            if (model.Start == null) return model;

            bool changed = true;
            while (changed)
            {
                changed = false;
                
                // 1. Удаление недостижимых вершин
                changed |= RemoveUnreachableVertices(model);

                // 2. Спрямление JumpPoint (точек перехода)
                changed |= BypassJumpPoints(model);

                // 3. Устранение избыточных проверок (LBS == RBS)
                changed |= RemoveRedundantChecks(model);

                // 4. Объединение эквивалентных условных вершин
                changed |= MergeEquivalentConditionals(model);

                // 5. Оптимизация последовательных проверок одного условия
                changed |= OptimizeSequentialConditions(model);

                // 6. Нормализация конечных вершин (слияние всех Yк в одну)
                changed |= NormalizeEndVertices(model);
            }

            // Финальная очистка и обновление
            RemoveUnreachableVertices(model);
            model.LAS = string.Empty; // Сбрасываем кэшированную строку ЛСА, чтобы она перегенерировалась
            model.Update();
            return model;
        }

        /// <summary>
        /// Удаляет логически недостижимые вершины и оптимизирует условные вершины на основе корреляции переменных.
        /// Если вершина достижима только при условии X=True, она заменяется на свою True-ветку.
        /// </summary>
        public static bool RemoveLogicallyUnreachable(AbstractAutomata model)
        {
            if (model.Start == null) return false;

            // 1. Собираем все условные вершины для управления симуляцией
            // Используем ту же логику группировки, что и в AbstractAutomata
            var conditionals = model.Vertices
                .OfType<ConditionalVertex>()
                .DistinctBy(v => v.ID)
                .OrderBy(v => v, model.ConditionalsBindingComparer)
                .ToList();

            int n = conditionals.Count;
            // Ограничение на количество переменных для избежания зависания на больших графах
            if (n > 18) return false; 

            int total = 1 << n;
            var reachable = new HashSet<IBDVertex>();
            var alwaysTrue = new HashSet<IBDVertex>();  // Вершины, которые всегда True когда достижимы
            var alwaysFalse = new HashSet<IBDVertex>(); // Вершины, которые всегда False когда достижимы
            var visitedVertices = new HashSet<IBDVertex>();  // Вершины, которые были достигнуты хотя бы раз

            // Инициализируем множества предположением "всегда", будем вычеркивать
            foreach (var c in conditionals)
            {
                alwaysTrue.Add(c);
                alwaysFalse.Add(c);
            }

            // 2. Симуляция всех вариантов
            var chars = new char[n];
            int maxSteps = Math.Max(1, model.Vertices.Count * 4);
            var takenLBS = new HashSet<ConditionalVertex>();
            var takenRBS = new HashSet<ConditionalVertex>();

            for (int mask = 0; mask < total; mask++)
            {
                // Формируем бинарную строку
                for (int i = 0; i < n; i++)
                    chars[i] = ((mask >> i) & 1) == 1 ? '1' : '0';
                
                string bin = new string(chars);
                model.SetConditionsFromBinary(bin);

                IBDVertex? current = model.Start;
                int steps = 0;
                var visitedInRun = new HashSet<IBDVertex>();

                while (current != null && steps++ < maxSteps)
                {
                    reachable.Add(current);
                    if (current is ConditionalVertex cv)
                    {
                        visitedVertices.Add(cv);
                        if (cv.Value == true) 
                        {
                            alwaysFalse.Remove(cv);
                            takenRBS.Add(cv);
                        }
                        else 
                        {
                            alwaysTrue.Remove(cv);
                            takenLBS.Add(cv);
                        }
                    }

                    if (!visitedInRun.Add(current)) break; // Цикл

                    current = current.GetNext(model);
                }
            }
            model.ResetConditions();

            bool changed = false;

            // 2.5. Prune untaken branches (Dead Code Elimination inside Conditionals)
            foreach (var cv in visitedVertices.OfType<ConditionalVertex>())
            {
                bool lbsTaken = takenLBS.Contains(cv);
                bool rbsTaken = takenRBS.Contains(cv);

                if (lbsTaken && !rbsTaken)
                {
                    // Only False branch taken. Redirect True branch to False branch.
                    if (cv.RBS != cv.LBS)
                    {
                        cv.RBS = cv.LBS;
                        changed = true;
                    }
                }
                else if (!lbsTaken && rbsTaken)
                {
                    if (cv.LBS != cv.RBS)
                    {
                        cv.LBS = cv.RBS;
                        changed = true;
                    }
                }
            }

            // 3. Оптимизация условных вершин ("Constant Propagation")
            // Если вершина была посещена, но всегда только с одним значением - заменяем её
            var parentMap = BuildParentMap(model);
            var toRemove = new List<IBDVertex>();


            // Проходим по ВСЕМ условным вершинам модели (включая дубликаты)
            foreach (var cv in model.Vertices.OfType<ConditionalVertex>().ToList())
            {
                // Если вершина вообще не посещалась, она будет удалена на этапе очистки недостижимых
                if (!visitedVertices.Contains(cv)) continue;

                bool isAlwaysTrue = alwaysTrue.Contains(cv);
                bool isAlwaysFalse = alwaysFalse.Contains(cv);

                IBDVertex? target = null;
                if (isAlwaysTrue) target = cv.RBS;
                else if (isAlwaysFalse) target = cv.LBS;

                if (target != null)
                {
                    // Заменяем cv на target
                    if (parentMap.TryGetValue(cv, out var parents))
                    {
                        foreach (var p in parents)
                        {
                            Redirect(p, cv, target);
                        }
                    }
                    toRemove.Add(cv);
                    changed = true;
                }
            }

            foreach (var v in toRemove)
                model.RemoveVertex(v);

            // 4. Удаление логически недостижимых вершин
            // (Тех, которые не попали в reachable)
            var allVertices = model.Vertices.ToList();
            foreach (var v in allVertices)
            {
                if (!reachable.Contains(v))
                {
                    model.RemoveVertex(v);
                    changed = true;
                }
            }

            return changed;
        }

        /// <summary>
        /// Удаляет вершины, недостижимые из Yн (Start).
        /// </summary>
        public static bool RemoveUnreachableVertices(AbstractAutomata model)
        {
            if (model.Start == null) return false;

            var reachable = new HashSet<IBDVertex>();
            var queue = new Queue<IBDVertex>();
            
            reachable.Add(model.Start);
            queue.Enqueue(model.Start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                // Собираем всех прямых потомков
                var neighbors = GetDirectNeighbors(current);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor != null && reachable.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            // Удаляем все вершины, которых нет в reachable
            var allVertices = model.Vertices.ToList(); // Копия списка
            bool removedAny = false;

            foreach (var v in allVertices)
            {
                if (!reachable.Contains(v))
                {
                    model.RemoveVertex(v);
                    removedAny = true;
                }
            }

            return removedAny;
        }

        /// <summary>
        /// Исключает JumpPoint из цепочек переходов: A -> JP1 -> JP2 -> B превращается в A -> JP2 -> B.
        /// Не удаляет JumpPoint, если он ведет к оператору (A -> JP -> Op), так как это необходимо для ЛСА.
        /// </summary>
        public static bool BypassJumpPoints(AbstractAutomata model)
        {
            bool changed = false;
            var vertices = model.Vertices.ToList();

            foreach (var v in vertices)
            {
                if (v is ConditionalVertex cv)
                {
                    // For Conditional, both LBS and RBS (usually) need to point to JumpPoints or End.
                    // We only compress chains of JumpPoints.
                    var newLbs = SkipJumpPointChains(cv.LBS);
                    var newRbs = SkipJumpPointChains(cv.RBS);

                    if (cv.LBS != newLbs)
                    {
                        cv.LBS = newLbs;
                        changed = true;
                    }
                    if (cv.RBS != newRbs)
                    {
                        cv.RBS = newRbs;
                        changed = true;
                    }
                }
                else
                {
                    var newNext = SkipJumpPointChains(v.Next);
                    if (v.Next != newNext)
                    {
                        v.Next = newNext;
                        changed = true;
                    }
                }
            }
            return changed;
        }

        /// <summary>
        /// Сжимает цепочки JumpPoint (JP1 -> JP2 -> ... -> JPn -> Target) до последнего JumpPoint (JPn -> Target).
        /// Если цепочка не содержит JumpPoint, возвращает исходную вершину.
        /// Если цепочка заканчивается не JumpPoint, возвращает последний JumpPoint.
        /// </summary>
        private static IBDVertex? SkipJumpPointChains(IBDVertex? start)
        {
            if (start is not JumpPoint) return start;

            var current = start;
            var visited = new HashSet<IBDVertex>();
            IBDVertex? lastJumpPoint = null;

            while (current is JumpPoint jp)
            {
                if (!visited.Add(current)) return lastJumpPoint; // Цикл
                lastJumpPoint = jp;
                current = jp.Next;
            }
            
            // current is NOT JumpPoint.
            // We want to return the JumpPoint that points to 'current'.
            // That is 'lastJumpPoint'.
            return lastJumpPoint;
        }

        /// <summary>
        /// Пропускает цепочку JumpPoint, но останавливается на последнем JumpPoint перед целевой вершиной.
        /// Это необходимо для ветвей LBS, которые обязаны указывать на метку (JumpPoint).
        /// </summary>
        private static IBDVertex? SkipToLastJumpPoint(IBDVertex? start)
        {
            return SkipJumpPointChains(start);
        }

        /// <summary>
        /// Пропускает цепочку JumpPoint и возвращает первую значимую вершину (или null).
        /// </summary>
        private static IBDVertex? SkipJumpPoints(IBDVertex? start)
        {
            var current = start;
            var visited = new HashSet<IBDVertex>(); // Защита от циклов из одних JumpPoint

            while (current is JumpPoint jp)
            {
                if (!visited.Add(current)) return null; // Цикл в JumpPoints -> обрыв
                current = jp.Next;
            }
            return current;
        }

        /// <summary>
        /// Устраняет условия, где LBS == RBS.
        /// A -> X(True: B, False: B) -> B  ==>  A -> B
        /// </summary>
        public static bool RemoveRedundantChecks(AbstractAutomata model)
        {
            bool changed = false;
            var vertices = model.Vertices.ToList();
            
            // Находим кандидатов на удаление
            var redundant = vertices.OfType<ConditionalVertex>()
                .Where(cv => cv.LBS == cv.RBS && cv.LBS != null)
                .ToList();

            if (redundant.Count == 0) return false;

            // Строим карту родителей для переброски ссылок
            var parentMap = BuildParentMap(model);

            foreach (var cv in redundant)
            {
                var target = cv.LBS; // == cv.RBS
                if (parentMap.TryGetValue(cv, out var parents))
                {
                    foreach (var parent in parents)
                    {
                        Redirect(parent, cv, target);
                        changed = true;
                    }
                }
                // Саму вершину удалим на этапе очистки недостижимых
            }

            return changed;
        }

        /// <summary>
        /// Объединяет эквивалентные условные вершины.
        /// Две вершины X с одинаковым ID эквивалентны, если ведут в одни и те же вершины по True и False.
        /// </summary>
        private static bool MergeEquivalentConditionals(AbstractAutomata model)
        {
            bool globalChanged = false;
            
            // Группируем по ID (например, "X1", "P2")
            var groups = model.Vertices.OfType<ConditionalVertex>()
                .GroupBy(v => v.ID)
                .Where(g => g.Count() > 1)
                .ToList();

            var parentMap = BuildParentMap(model);

            foreach (var group in groups)
            {
                var list = group.ToList();
                // Ищем дубликаты
                // Простой квадратичный перебор в группе (группы обычно маленькие)
                var mergedInGroup = new HashSet<ConditionalVertex>();

                for (int i = 0; i < list.Count; i++)
                {
                    var v1 = list[i];
                    if (mergedInGroup.Contains(v1)) continue;

                    for (int j = i + 1; j < list.Count; j++)
                    {
                        var v2 = list[j];
                        if (mergedInGroup.Contains(v2)) continue;

                        // Проверка структурной эквивалентности
                        if (v1.LBS == v2.LBS && v1.RBS == v2.RBS && v1.ID == v2.ID)
                        {
                            // v2 эквивалентна v1. Перенаправляем всех родителей v2 на v1.
                            if (parentMap.TryGetValue(v2, out var parents))
                            {
                                foreach (var p in parents)
                                {
                                    Redirect(p, v2, v1);
                                    globalChanged = true;
                                }
                            }
                            mergedInGroup.Add(v2);
                        }
                    }
                }
            }

            return globalChanged;
        }

        /// <summary>
        /// Оптимизирует последовательные проверки одного и того же условия без промежуточных операторов.
        /// Если X1 --(True)--> ... -> X1, то вторая проверка заменяется на переход по ветке True второй вершины.
        /// </summary>
        private static bool OptimizeSequentialConditions(AbstractAutomata model)
        {
            bool changed = false;
            var conditionals = model.Vertices.OfType<ConditionalVertex>().ToList();

            foreach (var cv in conditionals)
            {
                // Проверяем ветку True (RBS)
                var nextTrue = SkipJumpPoints(cv.RBS);
                if (nextTrue is ConditionalVertex nextCvTrue && nextCvTrue.ID == cv.ID)
                {
                    // X1=True -> ... -> X1. Значит второй X1 тоже True.
                    // Перенаправляем cv.RBS на nextCvTrue.RBS
                    if (cv.RBS != nextCvTrue.RBS)
                    {
                        cv.RBS = nextCvTrue.RBS;
                        changed = true;
                    }
                }

                // Проверяем ветку False (LBS)
                var nextFalse = SkipJumpPoints(cv.LBS);
                if (nextFalse is ConditionalVertex nextCvFalse && nextCvFalse.ID == cv.ID)
                {
                    // X1=False -> ... -> X1. Значит второй X1 тоже False.
                    // Перенаправляем cv.LBS на nextCvFalse.LBS
                    if (cv.LBS != nextCvFalse.LBS)
                    {
                        cv.LBS = nextCvFalse.LBS;
                        changed = true;
                    }
                }
            }
            return changed;
        }

        /// <summary>
        /// Объединяет все экземпляры конечной вершины (EndVertex) в один.
        /// </summary>
        private static bool NormalizeEndVertices(AbstractAutomata model)
        {
            var endVertices = model.Vertices.OfType<EndVertex>().ToList();
            if (endVertices.Count <= 1) return false;

            // Оставляем первую вершину как основную
            var primaryEnd = endVertices[0];
            var verticesToRemove = new HashSet<IBDVertex>();
            
            for (int i = 1; i < endVertices.Count; i++)
            {
                verticesToRemove.Add(endVertices[i]);
            }

            // Перенаправляем все ссылки на primaryEnd
            var parentMap = BuildParentMap(model);
            bool changed = false;

            foreach (var oldEnd in verticesToRemove)
            {
                if (parentMap.TryGetValue(oldEnd, out var parents))
                {
                    foreach (var parent in parents)
                    {
                        Redirect(parent, oldEnd, primaryEnd);
                        changed = true;
                    }
                }
                // Удаляем дубликат
                model.RemoveVertex(oldEnd);
                changed = true;
            }

            return changed;
        }





        #region Вспомогательные методы

        private static IEnumerable<IBDVertex?> GetDirectNeighbors(IBDVertex v)
        {
            if (v is ConditionalVertex cv)
            {
                yield return cv.LBS;
                yield return cv.RBS;
            }
            else
            {
                yield return v.Next;
            }
        }

        private static Dictionary<IBDVertex, List<IBDVertex>> BuildParentMap(AbstractAutomata model)
        {
            var map = new Dictionary<IBDVertex, List<IBDVertex>>();
            foreach (var v in model.Vertices)
            {
                foreach (var child in GetDirectNeighbors(v))
                {
                    if (child == null) continue;
                    if (!map.TryGetValue(child, out var list))
                    {
                        list = new List<IBDVertex>();
                        map[child] = list;
                    }
                    list.Add(v);
                }
            }
            return map;
        }

        private static void Redirect(IBDVertex parent, IBDVertex oldTarget, IBDVertex? newTarget)
        {
            if (parent is ConditionalVertex cv)
            {
                if (cv.LBS == oldTarget) cv.LBS = newTarget;
                if (cv.RBS == oldTarget) cv.RBS = newTarget;
            }
            else
            {
                if (parent.Next == oldTarget) parent.Next = newTarget;
            }
        }

        #endregion
    }
}
