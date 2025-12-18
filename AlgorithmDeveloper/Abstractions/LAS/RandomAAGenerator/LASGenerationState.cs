namespace AlgorithmDeveloper.Abstractions.LAS.RandomAAGenerator
{
    /// <summary>
    /// Состояния конечного автомата для генерации ЛСА
    /// </summary>
    public enum LASGenerationState
    {
        /// <summary>
        /// Начальное состояние - генерация стартовой вершины Yн
        /// </summary>
        Start,

        /// <summary>
        /// После стартовой вершины - можем генерировать любые элементы
        /// </summary>
        AfterStart,

        /// <summary>
        /// После операторной вершины - можем генерировать операторные, условные, переходы или завершить
        /// </summary>
        AfterOperator,

        /// <summary>
        /// После условной вершины - обязательно нужно генерировать LBS
        /// </summary>
        AfterConditional,

        /// <summary>
        /// В левой ветви условной вершины (LBS)
        /// </summary>
        InLeftBranch,

        /// <summary>
        /// В правой ветви условной вершины (RBS)
        /// </summary>
        InRightBranch,

        /// <summary>
        /// После точки перехода - можем генерировать любые элементы кроме переходов
        /// </summary>
        AfterJumpPoint,

        /// <summary>
        /// Можем завершить алгоритм - генерация конечной вершины Yк
        /// </summary>
        CanFinish,

        /// <summary>
        /// Алгоритм завершен
        /// </summary>
        Finished
    }
}