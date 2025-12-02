using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmDeveloper.AAModel;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;

namespace AlgorithmDeveloper.AAModel.TransitionSystem
{
    /// <summary>
    /// Логическое условие A[i][j] для перехода из вершины YFrom в вершину YTo.
    /// Содержит набор значений условных вершин, которые должны быть пройдены по пути.
    /// </summary>
    public sealed class TransitionCondition
    {
        private readonly Dictionary<ConditionalVertex, bool> _xValues;

        /// <summary>
        /// Вершина-источник (не условная, не конечная и не точка перехода).
        /// Допустимы: <see cref="StartVertex"/>, <see cref="OperatorVertex"/>.
        /// </summary>
        public IBDVertex YFrom { get; }
        /// <summary>
        /// Целевая вершина (не условная и не точка перехода). Допустимы: <see cref="OperatorVertex"/>, <see cref="EndVertex"/>.
        /// </summary>
        public IBDVertex YTo { get; }
        /// <summary>
        /// Набор условий: ключ — условная вершина, значение — требуемое булево значение.
        /// Может быть пустым, если переход не зависит от условий (тривиален).
        /// </summary>
        public IReadOnlyDictionary<ConditionalVertex, bool> XValues => _xValues;
        /// <summary>
        /// Человекочитаемое строковое представление условия.
        /// </summary>
        public string HumanReadableExpression => Expression(OutputView.HumanReadable);
        /// <summary>
        /// Cтроковое представление условия в виде логического выражения.
        /// </summary>
        public string LogicalExpression => Expression(OutputView.LogicalExpression);

        public TransitionCondition(IBDVertex yFrom, IBDVertex yTo, IDictionary<ConditionalVertex, bool>? xValues = null)
        {
            if (yFrom is ConditionalVertex || yFrom is EndVertex || yFrom is JumpPoint)
                throw new ArgumentException("YFrom должен быть не условной вершиной, не конечной и не точкой перехода.", nameof(yFrom));
            if (yTo is ConditionalVertex || yTo is JumpPoint)
                throw new ArgumentException("YTo должен быть не условной вершиной и не точкой перехода.", nameof(yTo));

            YFrom = yFrom ?? throw new ArgumentNullException(nameof(yFrom));
            YTo = yTo ?? throw new ArgumentNullException(nameof(yTo));
            _xValues = xValues != null
                ? new Dictionary<ConditionalVertex, bool>(xValues)
                : new Dictionary<ConditionalVertex, bool>();
        }

        /// <summary>
        /// Возвращает строковое представление логического условия A[i][j].
        /// Если набор условий пустой — возвращает string.Empty.
        /// </summary>
        public string Expression(OutputView view)
        {
            if (_xValues.Count == 0)
            {
                // По спецификации для отдельного логического условия пустой набор => пустая строка
                return string.Empty;
            }

            var ordered = _xValues
                .OrderBy(kv => kv.Key.ID, StringComparer.Ordinal)
                .Select(kv => kv.Value ? kv.Key.ID! : $"¬{kv.Key.ID}");

            return view switch
            {
                OutputView.LogicalExpression => string.Join(" ˄ ", ordered),
                OutputView.HumanReadable   => string.Join(" ", ordered),
                _ => string.Join(" ˄ ", ordered)
            };
        }
    }
}