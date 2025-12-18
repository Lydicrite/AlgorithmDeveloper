using System;

namespace AlgorithmDeveloper.Abstractions.TransitionSystem
{
    /// <summary>
    /// Вид, в котором будет выводиться в строку логическое условие и формула перехода.
    /// </summary>
    public enum OutputView
    {
        /// <summary>
        /// Формальный логический вид (без упоминания целевых вершин для формулы):
        /// A: "X0 ˄ ¬X1 ˄ ¬X2"; F: "(X0 ˄ ¬X1) ˅ (X0 ˄ X1)".
        /// </summary>
        LogicalExpression,
        /// <summary>
        /// Понятный человеку вид:
        /// A: "X0 ¬X1 ¬X2"; F: "Y0 → (X0 ¬X1 Y1) ˅ (X0 X1 Y2)".
        /// </summary>
        HumanReadable,
    }
}