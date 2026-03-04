using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.AAModel.Utils;

namespace AlgorithmDeveloper.Abstractions.Combining
{
    /// <summary>
    /// Объединяет несколько абстрактных автоматов (АА) в один путем слияния структурно идентичных частей
    /// и введения условных ветвлений в местах различий.
    /// Эта методика минимизирует количество вершин в результирующем автомате.
    /// </summary>
    public static class StructuralMerger
    {
        /// <summary>
        /// Объединяет массив моделей АА в одну общую модель.
        /// </summary>
        /// <param name="models">Массив моделей для объединения.</param>
        /// <returns>Объединенная модель <see cref="AbstractAutomata"/>.</returns>
        /// <exception cref="ArgumentException">Если массив моделей пуст или одна из моделей не имеет начальной вершины.</exception>
        public static AbstractAutomata Combine(params AbstractAutomata[] models)
        {
            if (models == null || models.Length == 0)
                throw new ArgumentException("Не предоставлены модели для объединения.");
            
            // 1. Подготовка входных данных
            var initialInputs = new List<ModelCursor>();
            for (int i = 0; i < models.Length; i++)
            {
                var m = models[i];
                if (m.Start == null)
                    throw new ArgumentException($"Модель {i} не имеет начальной вершины.");
                
                initialInputs.Add(new ModelCursor(i, m.Start));
            }

            // Упорядочиваем по схожести (как в MASCombiner)
            var sortedInputs = OrderInputsBySimilarity(initialInputs, models);

            // Переназначаем индексы моделей используя коды Грея для сохранения локальности
            var inputs = new List<ModelCursor>();
            for (int k = 0; k < sortedInputs.Count; k++)
            {
                int grayCode = GetGrayCode(k);
                inputs.Add(new ModelCursor(grayCode, sortedInputs[k].CurrentNode));
            }

            // 2. Построение графа
            var resultAA = new AbstractAutomata();

            // Определяем начальный индекс для новых P-вершин, чтобы избежать коллизий
            int maxPIndex = -1;
            foreach (var m in models)
            {
                foreach (var v in m.Vertices.OfType<ConditionalVertex>())
                {
                    // Проверяем, является ли вершина P-вершиной (стандартный префикс "P" или "p")
                    if (string.Equals(v.Prefix, "P", StringComparison.OrdinalIgnoreCase))
                    {
                        if (v.Index > maxPIndex)
                            maxPIndex = v.Index;
                    }
                }
            }

            // Вычисляем размер пространства моделей для маски (степень двойки)
            int q = models.Length;
            int n = (int)Math.Ceiling(Math.Log2(q));
            if (n == 0 && q > 0) n = 1;
            int modelSpaceSize = 1 << n;

            var context = new MergeContext(resultAA, modelSpaceSize, maxPIndex + 1);

            // Создание начальной вершины
            var newStart = new StartVertex();
            resultAA.AddVertex(newStart);
            
            // Привязка курсоров к началу
            var nextCursors = new List<ModelCursor>();
            foreach(var cursor in inputs)
            {
                var next = GetEffectiveNext(cursor.CurrentNode.GetNext());
                if (next != null)
                {
                    nextCursors.Add(new ModelCursor(cursor.ModelIndex, next));
                }
            }

            if (nextCursors.Count > 0)
            {
                var mergedNext = MergeRecursive(nextCursors, context);
                resultAA.LinkNext(newStart, mergedNext);
            }

            // Вставка точек перехода (JumpPoints) для поддержки корректной генерации ЛСА
            InsertJumpPoints(resultAA, context);

            // 3. Завершение
            resultAA.Update();

            return resultAA;
        }

        private static List<ModelCursor> OrderInputsBySimilarity(List<ModelCursor> inputs, AbstractAutomata[] models)
        {
            var result = new List<ModelCursor>();
            var remaining = new List<ModelCursor>(inputs);

            // Начинаем с первой
            var current = remaining[0];
            result.Add(current);
            remaining.RemoveAt(0);

            while (remaining.Count > 0)
            {
                ModelCursor? bestNext = null;
                int maxSim = -1;

                foreach (var cand in remaining)
                {
                    int sim = CalculateSimilarity(models[current.ModelIndex], models[cand.ModelIndex]);
                    if (sim > maxSim)
                    {
                        maxSim = sim;
                        bestNext = cand;
                    }
                }

                if (bestNext != null)
                {
                    result.Add(bestNext);
                    remaining.Remove(bestNext);
                    current = bestNext;
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        private static int CalculateSimilarity(AbstractAutomata a, AbstractAutomata b)
        {
            // Используем существующий метод в AbstractAutomata, который сравнивает через MAS
            // Если MAS не построен, строим временно (или используем существующий, если есть)
            // Для производительности лучше бы сравнивать графы, но используем логику MASCombiner как эталон.
            
            // Убедимся, что MAS есть. Если нет - это может быть дорого.
            // Но StructuralMerger обычно вызывается для уже готовых моделей.
            if (a.MAS == null) a.Update();
            if (b.MAS == null) b.Update();

            if (a.MAS != null && b.MAS != null)
            {
                return a.MAS.CalculateSimilarity(b.MAS);
            }
            return 0;
        }

        private static int GetGrayCode(int i)
        {
            return i ^ (i >> 1);
        }

        /// <summary>
        /// Рекурсивно объединяет пути из нескольких моделей.
        /// </summary>
        /// <param name="cursors">Список курсоров, указывающих на текущие вершины в объединяемых моделях.</param>
        /// <param name="context">Контекст объединения.</param>
        /// <returns>Вершина в результирующей модели, соответствующая объединенному состоянию.</returns>
        private static IBDVertex MergeRecursive(List<ModelCursor> cursors, MergeContext context)
        {
            if (cursors.Count == 0)
                throw new InvalidOperationException("Невозможно объединить пустой список курсоров.");

            // DEBUG LOGGING
            // Console.WriteLine($"MergeRecursive: {cursors.Count} cursors. Keys: {string.Join(", ", cursors.Select(c => GetNodeSignature(c.CurrentNode)))}");

            int currentMask = context.GetMask(cursors);
            string key = context.GetStateKey(cursors);

            // 1. Проверка кэша и дослияние (Augment)
            if (context.TryGetVertex(key, out var existing, out int cachedMask))
            {
                // Если текущая маска уже покрыта кэшированной (цикл или повторный вход), возвращаем существующую.
                if ((cachedMask & currentMask) == currentMask)
                {
                    return existing;
                }

                // Иначе: Мы пришли в эту же структурную точку с НОВЫМИ моделями (асинхронное слияние).
                // Нужно "дослить" (Augment) существующую вершину, объединив её текущие пути с новыми.
                
                // Обновляем маску (теперь вершина обслуживает и новые модели)
                context.UpdateVertex(key, existing, cachedMask | currentMask);

                if (existing is EndVertex)
                {
                    // EndVertex не имеет продолжения, просто возвращаем.
                    return existing;
                }
                
                // Подготавливаем курсоры для рекурсивного слияния потомков.
                // 1. Курсоры от существующих моделей (из Result графа)
                var combinedCursorsNext = new List<ModelCursor>();
                var combinedCursorsLBS = new List<ModelCursor>();
                var combinedCursorsRBS = new List<ModelCursor>();

                for (int i = 0; i < context.ModelCount; i++)
                {
                    if (((cachedMask >> i) & 1) != 0)
                    {
                        // Эта модель уже была здесь. Её путь идет через existing.Next (или LBS/RBS).
                        // Мы берем existing.Next как "входной" узел для дальнейшего слияния.
                        // Важно: мы фактически перестраиваем (или переиспользуем через кэш) будущее этой вершины.
                        if (existing is ConditionalVertex ecv)
                        {
                            if (ecv.LBS != null) combinedCursorsLBS.Add(new ModelCursor(i, ecv.LBS));
                            if (ecv.RBS != null) combinedCursorsRBS.Add(new ModelCursor(i, ecv.RBS));
                        }
                        else
                        {
                            if (existing.Next != null) combinedCursorsNext.Add(new ModelCursor(i, existing.Next));
                        }
                    }
                }

                // 2. Курсоры от новых моделей (из Input графа)
                foreach (var c in cursors)
                {
                    if (c.CurrentNode is ConditionalVertex cv)
                    {
                        var l = GetEffectiveNext(cv.LBS);
                        var r = GetEffectiveNext(cv.RBS);
                        if (l != null) combinedCursorsLBS.Add(new ModelCursor(c.ModelIndex, l));
                        if (r != null) combinedCursorsRBS.Add(new ModelCursor(c.ModelIndex, r));
                    }
                    else
                    {
                        var n = GetEffectiveNext(c.CurrentNode.GetNext());
                        if (n != null) combinedCursorsNext.Add(new ModelCursor(c.ModelIndex, n));
                    }
                }

                // 3. Рекурсивное слияние и обновление связей
                if (existing is ConditionalVertex excv)
                {
                    if (combinedCursorsLBS.Count > 0)
                        excv.LBS = MergeRecursive(combinedCursorsLBS, context);
                    if (combinedCursorsRBS.Count > 0)
                        excv.RBS = MergeRecursive(combinedCursorsRBS, context);
                }
                else
                {
                    if (combinedCursorsNext.Count > 0)
                        existing.Next = MergeRecursive(combinedCursorsNext, context);
                }

                return existing;
            }

            // 2. Анализ курсоров на однородность
            var groups = cursors
                .GroupBy(c => GetNodeSignature(c.CurrentNode))
                .ToList();

            // 3. Логика объединения
            IBDVertex resultVertex;

            if (groups.Count == 1)
            {
                // Все курсоры указывают на "совместимые" узлы. Объединяем их в один.
                var sample = groups[0].First().CurrentNode;
                
                if (sample is EndVertex)
                {
                    resultVertex = context.GetOrCreateEnd();
                }
                else
                {
                    resultVertex = CreateMergedVertex(sample);
                    context.ResultModel.AddVertex(resultVertex);
                }

                // Регистрируем в кэше ДО рекурсии для обработки циклов
                context.RegisterVertex(key, resultVertex, currentMask);

                if (resultVertex is ConditionalVertex cv)
                {
                    // Объединение левых ветвей (LBS)
                    var lbsCursors = new List<ModelCursor>();
                    var rbsCursors = new List<ModelCursor>();
                    
                    foreach (var c in cursors)
                    {
                        if (c.CurrentNode is ConditionalVertex srcCv)
                        {
                            var l = GetEffectiveNext(srcCv.LBS);
                            var r = GetEffectiveNext(srcCv.RBS);
                            if (l != null) lbsCursors.Add(new ModelCursor(c.ModelIndex, l));
                            if (r != null) rbsCursors.Add(new ModelCursor(c.ModelIndex, r));
                        }
                    }

                    if (lbsCursors.Count > 0)
                        cv.LBS = MergeRecursive(lbsCursors, context);
                    
                    if (rbsCursors.Count > 0)
                        cv.RBS = MergeRecursive(rbsCursors, context);
                }
                else if (resultVertex is EndVertex)
                {
                    // Нечего связывать
                }
                else
                {
                    // Операторная вершина, Start (здесь не должно быть) и т.д.
                    // Связывание Next
                    var nextCursors = new List<ModelCursor>();
                    foreach(var c in cursors)
                    {
                        var n = GetEffectiveNext(c.CurrentNode.GetNext()); // Используем GetNext() дженерик или свойство
                        if (n != null)
                            nextCursors.Add(new ModelCursor(c.ModelIndex, n));
                    }

                    if (nextCursors.Count > 0)
                    {
                        resultVertex.Next = MergeRecursive(nextCursors, context);
                    }
                }
            }
            else
            {
                // Расхождение! Нужно ввести селектор (условие).
                int bitIndex = FindBestDiscriminatorBit(cursors, groups);
                
                // Создание вершины-селектора
                // Используем стандартный префикс "P" и уникальный индекс
                var selector = new ConditionalVertex("P", context.NextSwitchId());
                
                resultVertex = selector;
                context.RegisterVertex(key, resultVertex, currentMask);
                context.ResultModel.AddVertex(resultVertex);

                // Разделение курсоров
                var lowCursors = new List<ModelCursor>(); // Бит = 0
                var highCursors = new List<ModelCursor>(); // Бит = 1

                foreach (var c in cursors)
                {
                    if (((c.ModelIndex >> bitIndex) & 1) == 0)
                        lowCursors.Add(c);
                    else
                        highCursors.Add(c);
                }

                if (lowCursors.Count > 0)
                    selector.LBS = MergeRecursive(lowCursors, context);
                
                if (highCursors.Count > 0)
                    selector.RBS = MergeRecursive(highCursors, context);
                    
                // Оптимизация после создания: Если LBS == RBS, заменяем селектор на LBS
                if (selector.LBS == selector.RBS && selector.LBS != null)
                {
                    // Обновление кэша
                    context.UpdateVertex(key, selector.LBS, currentMask);
                    // Удаление селектора из модели
                    context.ResultModel.RemoveVertex(selector);
                    return selector.LBS;
                }
            }

            return resultVertex;
        }

        /// <summary>
        /// Находит лучший бит для различения групп моделей при расхождении путей.
        /// </summary>
        private static int FindBestDiscriminatorBit(List<ModelCursor> cursors, List<IGrouping<string, ModelCursor>> groups)
        {
            var indices = cursors.Select(c => c.ModelIndex).ToList();
            if (indices.Count <= 1) return 0;

            int bestBit = 0;
            int maxScore = -1;
            int bestBalance = int.MaxValue; 

            for (int b = 0; b < 16; b++)
            {
                int count0 = 0;
                int count1 = 0;
                
                foreach (var idx in indices)
                {
                    if (((idx >> b) & 1) == 0) count0++;
                    else count1++;
                }

                if (count0 == 0 || count1 == 0) continue; 

                // Вычисление оценки: Сколько групп сохраняются ВМЕСТЕ?
                int preservedGroups = 0;
                foreach (var g in groups)
                {
                    bool has0 = false;
                    bool has1 = false;
                    foreach (var c in g)
                    {
                        if (((c.ModelIndex >> b) & 1) == 0) has0 = true;
                        else has1 = true;
                    }

                    if (has0 && has1) { /* разделение */ }
                    else preservedGroups++;
                }

                if (preservedGroups > maxScore)
                {
                    maxScore = preservedGroups;
                    bestBit = b;
                    bestBalance = Math.Abs(count0 - count1);
                }
                else if (preservedGroups == maxScore)
                {
                    int balance = Math.Abs(count0 - count1);
                    if (balance < bestBalance)
                    {
                        bestBalance = balance;
                        bestBit = b;
                    }
                }
            }
            
            return bestBit;
        }

        /// <summary>
        /// Создает копию вершины для объединенной модели.
        /// </summary>
        private static IBDVertex CreateMergedVertex(IBDVertex template)
        {
            if (template is OperatorVertex op)
            {
                var newOp = new OperatorVertex(op.Index);
                // Важно: копируем ID, так как он может отличаться от стандартного "Yi" (например, "Xi")
                newOp.ID = op.ID;
                return newOp;
            }
            if (template is ConditionalVertex cv)
            {
                var newCv = new ConditionalVertex(cv.Prefix, cv.Index);
                newCv.ID = cv.ID;
                return newCv;
            }
            if (template is StartVertex) return new StartVertex();
            
            throw new NotSupportedException($"Неизвестный тип вершины: {template.GetType().Name}");
        }

        /// <summary>
        /// Получает уникальную сигнатуру узла для сравнения.
        /// </summary>
        private static string GetNodeSignature(IBDVertex v)
        {
            // Для целей сопоставления.
            if (v is OperatorVertex op) return $"Op:{op.ID}";
            if (v is ConditionalVertex cv) return $"Cond:{cv.ID}:{cv.Prefix}:{cv.Index}"; // Строгое совпадение по ID
            if (v is StartVertex) return "Start";
            if (v is EndVertex) return "End";
            return v.GetType().Name;
        }

        /// <summary>
        /// Получает следующую эффективную вершину, пропуская точки перехода (JumpPoints).
        /// </summary>
        private static IBDVertex? GetEffectiveNext(IBDVertex? v)
        {
            if (v == null) return null;
            // Пропуск JumpPoints
            var curr = v;
            while (curr is JumpPoint jp)
            {
                curr = jp.GetNext(null); // Предполагаем, что JumpPoint.Next заполнен или доступен.
                // JumpPoint.GetNext() возвращает Next.
                if (curr == null) return null;
            }
            return curr;
        }

        /// <summary>
        /// Вставляет точки перехода (JumpPoints) в модель для соответствия синтаксису ЛСА.
        /// </summary>
        private static void InsertJumpPoints(AbstractAutomata model, MergeContext context)
        {
            // 1. Сбор входящих ребер
            // Карта: Цель -> Список<(Источник, Тип ребра)>
            // Тип ребра: 0 = Next/RBS, 1 = LBS
            var incoming = new Dictionary<IBDVertex, List<(IBDVertex Source, int Type)>>();

            foreach (var v in model.Vertices)
            {
                if (v is JumpPoint jp)
                {
                    // Do nothing
                }
                incoming[v] = new List<(IBDVertex, int)>();
            }

            foreach (var v in model.Vertices.ToList())
            {
                if (v.Next != null)
                {
                    if (!incoming.ContainsKey(v.Next)) incoming[v.Next] = new List<(IBDVertex, int)>();
                    incoming[v.Next].Add((v, 0));
                }

                if (v is ConditionalVertex cv)
                {
                    if (cv.LBS != null)
                    {
                        if (!incoming.ContainsKey(cv.LBS)) incoming[cv.LBS] = new List<(IBDVertex, int)>();
                        incoming[cv.LBS].Add((cv, 1));
                    }
                    if (cv.RBS != null)
                    {
                        if (!incoming.ContainsKey(cv.RBS)) incoming[cv.RBS] = new List<(IBDVertex, int)>();
                        incoming[cv.RBS].Add((cv, 0));
                    }
                }
            }

            // 2. Вставка JumpPoint
            // Используем Обратный Post-Order обход (DFS) для нумерации (Топологическая сортировка).
            // 1. Глобально: Глубокие узлы (Child) получают БОЛЬШИЕ ID, чем поверхностные (Parent). 
            //    (Child > Parent). Это критично для корректной работы оптимизаций конвертера ЛСА.
            // 2. Локально: RBS обрабатывается перед LBS, поэтому RBS-цели получают меньшие индексы в PostOrder списке,
            //    но при OrderByDescending они получат БОЛЬШИЕ ID.
            //    Это обеспечивает RBS > LBS, что необходимо для корректного переключения ветвей.
            var postOrder = GetPostOrder(model);
            var orderMap = postOrder.Select((v, i) => (v, i)).ToDictionary(x => x.v, x => x.i);
            
            var targets = incoming.Keys.OrderByDescending(v => 
            {
                if (v is EndVertex) return int.MinValue; // EndVertex (MinValue) -> Last in Descending -> Max ID.
                return orderMap.TryGetValue(v, out int idx) ? idx : -1;
            }).ToList();
            
            foreach (var target in targets)
            {
                if (target is JumpPoint) continue;
                if (target is StartVertex) continue; 

                var edges = incoming[target];
                if (edges.Count == 0) continue;

                // Условия вставки JumpPoint:
                // 1. In-Degree > 1 (точка слияния путей)
                // 2. Входящее ребро типа LBS (требование синтаксиса ЛСА для ↑j)
                // 3. Целевая вершина - EndVertex (для корректности парсинга и завершения всех путей)
                
                bool needsJumpPoint = edges.Count > 1 || edges.Any(e => e.Type == 1) || target is EndVertex;

                if (needsJumpPoint)
                {
                    int jpIndex = context.NextJumpId();
                    var jp = model.EnsureJumpPoint(jpIndex);
                    
                    model.LinkNext(jp, target);
                    
                    foreach (var (source, type) in edges)
                    {
                        if (source is ConditionalVertex cv)
                        {
                            if (type == 1) // LBS
                            {
                                if (ReferenceEquals(cv.LBS, target)) cv.LBS = jp;
                            }
                            else // RBS
                            {
                                if (ReferenceEquals(cv.RBS, target)) cv.RBS = jp;
                            }
                        }
                        else
                        {
                            if (ReferenceEquals(source.Next, target)) source.Next = jp;
                        }
                    }
                }
            }
        }





        private static List<IBDVertex> GetPostOrder(AbstractAutomata model)
        {
            var result = new List<IBDVertex>();
            var visited = new HashSet<IBDVertex>();
            var stack = new Stack<IBDVertex>();
            
            if (model.Start != null)
                Visit(model.Start, visited, result);
                
            // Добавляем недостижимые вершины (если есть)
            foreach (var v in model.Vertices)
            {
                if (!visited.Contains(v))
                    Visit(v, visited, result);
            }
            
            return result;

            void Visit(IBDVertex u, HashSet<IBDVertex> vis, List<IBDVertex> res)
            {
                if (!vis.Add(u)) return;

                if (u is ConditionalVertex cv)
                {
                    // Посещаем RBS, затем LBS (Порядок важен для Reverse Post-Order нумерации).
                    if (cv.RBS != null) Visit(cv.RBS, vis, res);
                    if (cv.LBS != null) Visit(cv.LBS, vis, res);
                }
                else
                {
                    if (u.Next != null) Visit(u.Next, vis, res);
                }
                
                res.Add(u);
            }
        }

        #region Внутренние классы

        /// <summary>
        /// Курсор, отслеживающий текущее положение в конкретной модели при обходе.
        /// </summary>
        private class ModelCursor
        {
            public int ModelIndex { get; }
            public IBDVertex CurrentNode { get; }
            public ModelCursor(int idx, IBDVertex node) { ModelIndex = idx; CurrentNode = node; }
        }

        /// <summary>
        /// Контекст процесса объединения, хранящий состояние и кэши.
        /// </summary>
        private class MergeContext
        {
            public AbstractAutomata ResultModel { get; }
            public int ModelCount { get; }
            // Кэш хранит (Вершина, Маска моделей). Маска показывает, какие модели уже прошли через эту вершину.
            private Dictionary<string, (IBDVertex V, int Mask)> _cache = new();
            private EndVertex? _sharedEnd = null;
            private int _switchCounter = 0;
            private int _jumpCounter = 1;

            public MergeContext(AbstractAutomata res, int count, int startSwitchId = 0)
            {
                ResultModel = res;
                ModelCount = count;
                _switchCounter = startSwitchId - 1; // Так как NextSwitchId делает ++ перед возвратом
            }

            public int NextSwitchId() => ++_switchCounter;
            public int NextJumpId() => _jumpCounter++;
            
            public string GetStateKey(List<ModelCursor> cursors)
            {
                // Используем отсортированные сигнатуры для идентификации структурного состояния.
                // Мы игнорируем ModelIndex, чтобы позволить объединение непересекающихся путей, которые сходятся к одной структуре.
                var signatures = cursors
                    .Select(c => GetNodeSignature(c.CurrentNode))
                    .Distinct() // Если несколько моделей находятся в одном узле, это один структурный элемент
                    .OrderBy(s => s);
                return string.Join("|", signatures);
            }

            public int GetMask(List<ModelCursor> cursors)
            {
                int mask = 0;
                foreach (var c in cursors)
                    mask |= (1 << c.ModelIndex);
                return mask;
            }

            public bool TryGetVertex(string key, out IBDVertex v, out int mask)
            {
                if (_cache.TryGetValue(key, out var entry))
                {
                    v = entry.V;
                    mask = entry.Mask;
                    return true;
                }
                v = null!;
                mask = 0;
                return false;
            }

            public void RegisterVertex(string key, IBDVertex v, int mask)
            {
                _cache[key] = (v, mask);
                if (v is EndVertex ev)
                {
                    if (_sharedEnd == null) _sharedEnd = ev;
                }
            }
            
            public void UpdateVertex(string key, IBDVertex v, int mask)
            {
                _cache[key] = (v, mask);
            }

            public EndVertex GetOrCreateEnd()
            {
                if (_sharedEnd == null)
                {
                    _sharedEnd = new EndVertex();
                    ResultModel.AddVertex(_sharedEnd);
                }
                return _sharedEnd;
            }
        }

        #endregion
    }
}
