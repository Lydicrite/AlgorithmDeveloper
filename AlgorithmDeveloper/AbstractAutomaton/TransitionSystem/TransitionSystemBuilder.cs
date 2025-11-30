using AlgorithmDeveloper.AlgoDev.Model.Vertices;
using AlgorithmDeveloper.AlgorithmModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlgorithmDeveloper.AlgorithmModel.TransitionSystem
{
    /// <summary>
    /// Режим построения формул перехода.
    /// </summary>
    public enum TransitionBuildMode
    {
        /// <summary>
        /// Legacy — использовать старую стратегию, найденную в учебниках.
        /// <br></br>Строить формулы для всех стартовых вершин при любой комбинации условий, возможны парадоксы).
        /// </summary>
        Legacy,
        /// <summary>
        /// NoParadox — строить формулы только для стартовых вершин, достижимых из Yн при текущей комбинации условий (исключает парадоксы).
        /// </summary>
        NoParadox
    }

    /// <summary>
    /// Генератор формул перехода для модели алгоритма.
    /// Выполняет последовательные «запуски» модели для всех комбинаций значений условных вершин,
    /// собирая для каждой исходной вершины (Yн, Yi) условия перехода к достижимым целевым вершинам (Yi, Yк) по пути.
    /// </summary>
    public static class TransitionSystemBuilder
    {
        /// <summary>
        /// Строит формулы перехода для всех допустимых стартовых вершин (Start, Operator).
        /// Для каждой вершины формирует набор направлений к достижимым целевым вершинам (Operator, End)
        /// с соответствующими условиями, полученными путём симуляции для всех комбинаций значений условных вершин.
        /// </summary>
        public static List<TransitionFormula> BuildAll(AbstractAutomaton model, TransitionBuildMode mode = TransitionBuildMode.NoParadox)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Стартовые вершины: Yн и все операторные Yi
            var starts = model.Vertices
                .Where(v => v is StartVertex || v is OperatorVertex)
                .ToList();

            // Предсоздаём формулы для каждой стартовой вершины
            var formulasByStart = new Dictionary<IBDVertex, TransitionFormula>();
            foreach (var s in starts)
                formulasByStart[s] = new TransitionFormula(s);

            // Все условные вершины, сгруппированные по ID
            var conditionalGroups = model.Vertices.OfType<ConditionalVertex>()
                .GroupBy(c => c.ID)
                .OrderBy(g => g.Key, StringComparer.Ordinal)
                .ToList();

            int n = conditionalGroups.Count;
            int total = n == 0 ? 1 : (1 << n);

            // Перебор всех комбинаций значений условных вершин
            for (int mask = 0; mask < total; mask++)
            {
                // Установка значений .Value для условных вершин по текущей комбинации
                for (int i = 0; i < n; i++)
                {
                    bool val = ((mask >> i) & 1) == 1;
                    foreach (var cv in conditionalGroups[i])
                    {
                        model.SetConditionalValue(cv, val);
                    }
                }

                // В режиме NoParadox собираем множество стартов, достижимых из Yн при текущей комбинации
                HashSet<IBDVertex>? reachableStarts = null;
                if (mode == TransitionBuildMode.NoParadox)
                    reachableStarts = CollectReachableStarts(model);

                // Для каждой стартовой вершины запускаем симуляцию и собираем термы
                foreach (var start in starts)
                {
                    if (reachableStarts != null && !reachableStarts.Contains(start))
                        continue; // пропускаем недостижимые старты, чтобы исключить парадоксы

                    var encountered = new Dictionary<ConditionalVertex, bool>();
                    IBDVertex? current = start;
                    int maxSteps = Math.Max(1, model.Vertices.Count * 4); // защита от бесконечных циклов
                    int steps = 0;
                    bool advancedFromStart = false; // не фиксируем стартовую вершину как целевую на шаге 0

                    while (current != null && steps++ < maxSteps)
                    {
                        // На самом первом шаге просто сделать переход вперёд от YStart
                        if (!advancedFromStart && ReferenceEquals(current, start))
                        {
                            current = current.GetNext(model);
                            advancedFromStart = true;
                            continue;
                        }

                        if (current is ConditionalVertex cv)
                        {
                            // Добавляем в набор только те условные вершины, что реально встретились по пути
                            if (!cv.Value.HasValue)
                                break; // комбинация должна задавать все условия; если нет — прерываем
                            encountered[cv] = cv.Value.Value;
                            current = cv.GetNext(model);
                            continue;
                        }

                        // Целевые вершины: операторные Yi и конечная Yк
                        if (current is OperatorVertex || current is EndVertex)
                        {
                            var yFrom = start;
                            var yTo = current; // OperatorVertex или EndVertex

                            // Дедупликация: терм определяется парой (YTo, набор встреченных условий до достижения YTo)
                            var fingerprint = Fingerprint((IDictionary<ConditionalVertex, bool>)encountered);
                            var formula = formulasByStart[yFrom];

                            bool exists = formula.Conditions.Any(c => ReferenceEquals(c.YTo, yTo) && Fingerprint(c.XValues) == fingerprint);
                            if (!exists)
                            {
                                var term = new TransitionCondition(yFrom, yTo, encountered);
                                formula.AddCondition(term);
                            }

                            // Фиксируем только первую встреченную целевую вершину для текущих условий
                            break;
                        }

                        // Прочие вершины (например, JumpPoint или другие типы) — просто двигаемся дальше по Next
                        current = current.GetNext(model);
                    }
                }
            }

            // Сброс значений условных вершин в неопределённое состояние
            model.ResetConditions();

            var sortedFormulas = formulasByStart.Values
                .Select(f => new TransitionFormula(
                    f.YStart,
                    f.Conditions.OrderBy(c => c.YTo.ID ?? string.Empty, StringComparer.Ordinal)
                ))
                .OrderBy(f => f.YStart is StartVertex ? 0 : 1)
                .ThenBy(f => f.YStart.ID, StringComparer.Ordinal)
                .ToList();

            return sortedFormulas;
        }

        /// <summary>
        /// Собирает множество стартовых вершин (Yн и все операторные Yi), достижимых из Yн
        /// при текущей комбинации значений условных вершин (используются .Value у ConditionalVertex).
        /// </summary>
        private static HashSet<IBDVertex> CollectReachableStarts(AbstractAutomaton model)
        {
            var reachable = new HashSet<IBDVertex>();
            var startVertex = model.Vertices.OfType<StartVertex>().FirstOrDefault();
            if (startVertex == null)
                return reachable;
            reachable.Add(startVertex);

            IBDVertex? current = startVertex;
            int maxSteps = Math.Max(1, model.Vertices.Count * 8); // чуть больше лимит, чтобы захватить возможные циклы
            int steps = 0;
            bool advancedFromStart = false;
            var visited = new HashSet<IBDVertex>();

            while (current != null && steps++ < maxSteps)
            {
                if (!advancedFromStart && ReferenceEquals(current, startVertex))
                {
                    current = current.GetNext(model);
                    advancedFromStart = true;
                    continue;
                }

                if (!visited.Add(current))
                    break; // зацикливание — прекращаем обход

                if (current is OperatorVertex)
                    reachable.Add(current);

                if (current is EndVertex)
                    break; // достигли Yк — дальше нет смысла

                current = current.GetNext(model);
            }

            return reachable;
        }

        private static string Fingerprint(IDictionary<ConditionalVertex, bool> xvals)
        {
            if (xvals == null || xvals.Count == 0) return string.Empty;
            var parts = xvals
                .GroupBy(kv => kv.Key.ID)
                .OrderBy(g => g.Key, StringComparer.Ordinal)
                .Select(g => $"{g.Key}:{(g.First().Value ? 1 : 0)}");
            return string.Join(";", parts);
        }

        private static string Fingerprint(IReadOnlyDictionary<ConditionalVertex, bool> xvals)
        {
            if (xvals == null || xvals.Count == 0) return string.Empty;
            var parts = xvals
                .GroupBy(kv => kv.Key.ID)
                .OrderBy(g => g.Key, StringComparer.Ordinal)
                .Select(g => $"{g.Key}:{(g.First().Value ? 1 : 0)}");
            return string.Join(";", parts);
        }
    }
}