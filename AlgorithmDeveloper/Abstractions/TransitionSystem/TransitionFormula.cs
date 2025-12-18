using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmDeveloper.Abstractions.TransitionSystem
{
    /// <summary>
    /// Формула перехода для вершины Y[i]: Y[i] → (A[i][1] Y[1] ˅ ... ˅ A[i][m] Y[m]).
    /// </summary>
    public sealed class TransitionFormula
    {
        private readonly List<TransitionCondition> _conditions = new();

        /// <summary>
        /// Вершина, для которой составляется формула перехода (источник).
        /// Должна быть не условной, не конечной и не точкой перехода.
        /// </summary>
        public IBDVertex YStart { get; }
        /// <summary>
        /// Набор логических условий вида A[i][k]. Каждое условие содержит YFrom = YStart,
        /// и конкретный YTo.
        /// </summary>
        public IReadOnlyList<TransitionCondition> Conditions => _conditions;
        /// <summary>
        /// Человекочитаемое строковое представление формулы.
        /// </summary>
        public string HumanReadableExpression => Expression(OutputView.HumanReadable);
        /// <summary>
        /// Cтроковое представление формулы в виде логического выражения.
        /// </summary>
        public string LogicalExpression => Expression(OutputView.LogicalExpression);

        public TransitionFormula(IBDVertex yStart, IEnumerable<TransitionCondition>? conditions = null)
        {
            if (yStart is ConditionalVertex || yStart is EndVertex || yStart is JumpPoint)
                throw new ArgumentException("YStart должен быть не условной вершиной, не конечной и не точкой перехода.", nameof(yStart));

            YStart = yStart ?? throw new ArgumentNullException(nameof(yStart));
            if (conditions != null)
            {
                foreach (var c in conditions)
                    AddCondition(c);
            }
        }

        /// <summary>
        /// Добавляет условие в формулу. Проверяет согласованность источника.
        /// </summary>
        public void AddCondition(TransitionCondition condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (!ReferenceEquals(condition.YFrom, YStart))
                throw new ArgumentException("Условие должно иметь тот же YFrom, что и YStart формулы.", nameof(condition));
            _conditions.Add(condition);
        }

        /// <summary>
        /// Вычисляет строковое представление формулы перехода.
        /// </summary>
        public string Expression(OutputView view)
        {
            if (_conditions.Count == 0)
            {
                // Нет направлений — пустая формула
                return string.Empty;
            }

            if (view == OutputView.LogicalExpression)
            {
                // Собираем чисто логическую формулу без указаний YTo внутри термов.
                // Пустой терм интерпретируем как "1" (безусловный переход).
                var terms = _conditions.Select(c =>
                {
                    var expr = c.LogicalExpression;
                    return string.IsNullOrEmpty(expr) ? "1" : $"({expr})";
                });
                return string.Join(" ˅ ", terms);
            }
            else // HumanReadable
            {
                // В человекочитаемом виде показываем направление к YTo.
                var terms = _conditions.Select(c =>
                {
                    var cond = c.HumanReadableExpression;
                    var yTo = c.YTo.ID ?? string.Empty;
                    if (string.IsNullOrEmpty(cond))
                        return yTo; // терм без условий: просто целевая вершина
                    return $"({cond} {yTo})";
                }).Where(s => !string.IsNullOrEmpty(s));

                var right = string.Join(" ˅ ", terms);
                return $"{YStart.ID} → {right}";
            }
        }
    }
}