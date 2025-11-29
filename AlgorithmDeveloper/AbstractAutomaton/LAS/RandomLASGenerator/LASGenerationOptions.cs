using System;

namespace AlgorithmDeveloper.AlgorithmModel.LAS.RandomLASGenerator
{
    /// <summary>
    /// Параметры генерации случайных ЛСА
    /// </summary>
    public class LASGenerationOptions
    {
        /// <summary>
        /// Минимальное количество токенов в генерируемой ЛСА
        /// </summary>
        public int? MinTokenCount { get; set; }

        /// <summary>
        /// Максимальное количество токенов в генерируемой ЛСА
        /// </summary>
        public int? MaxTokenCount { get; set; }

        /// <summary>
        /// Точное количество операторных вершин
        /// </summary>
        public int? OperatorVertexCount { get; set; }

        /// <summary>
        /// Точное количество условных вершин
        /// </summary>
        public int? ConditionalVertexCount { get; set; }

        /// <summary>
        /// Минимальное количество операторных вершин
        /// </summary>
        public int? MinOperatorVertexCount { get; set; }

        /// <summary>
        /// Максимальное количество операторных вершин
        /// </summary>
        public int? MaxOperatorVertexCount { get; set; }

        /// <summary>
        /// Минимальное количество условных вершин
        /// </summary>
        public int? MinConditionalVertexCount { get; set; }

        /// <summary>
        /// Максимальное количество условных вершин
        /// </summary>
        public int? MaxConditionalVertexCount { get; set; }

        /// <summary>
        /// Seed для генератора случайных чисел
        /// </summary>
        public int? Seed { get; set; }

        /// <summary>
        /// Максимальное количество попыток генерации
        /// </summary>
        public int MaxAttempts { get; set; } = 1000;

        /// <summary>
        /// Вероятность генерации операторной вершины (0.0 - 1.0)
        /// </summary>
        public double OperatorVertexProbability { get; set; } = 0.35;

        /// <summary>
        /// Вероятность генерации условной вершины (0.0 - 1.0)
        /// </summary>
        public double ConditionalVertexProbability { get; set; } = 0.55;

        /// <summary>
        /// Вероятность генерации перехода (0.0 - 1.0)
        /// </summary>
        public double JumpProbability { get; set; } = 0.3;

        /// <summary>
        /// Включить генерацию циклов (обратные переходы на ранее определенные точки ↓j)
        /// </summary>
        public bool EnableCycles { get; set; } = true;

        /// <summary>
        /// Вероятность генерации обратного перехода (цикла) относительно других действий (0.0 - 1.0)
        /// </summary>
        public double CycleJumpProbability { get; set; } = 0.15;

        /// <summary>
        /// Максимальное количество обратных переходов, которое разрешено в одной ЛСА (null = без ограничения)
        /// </summary>
        public int? MaxCycleJumpCount { get; set; } = 3;

        /// <summary>
        /// Минимальное количество определённых точек ↓j, после которого допускаются обратные переходы
        /// </summary>
        public int MinClosedJumpPointsForCycle { get; set; } = 1;

        /// <summary>
        /// Смещение выбора цели цикла к более недавним точкам (0.0 = равномерно, 1.0 = почти всегда самая новая)
        /// </summary>
        public double CycleRecencyBias { get; set; } = 0.7;

        /// <summary>
        /// Вероятность завершения алгоритма (0.0 - 1.0)
        /// </summary>
        public double FinishProbability { get; set; } = 0.1;

        /// <summary>
        /// Создает опции по умолчанию
        /// </summary>
        public static LASGenerationOptions Default => new LASGenerationOptions();

        /// <summary>
        /// Создает опции для генерации с минимальным количеством токенов
        /// </summary>
        public static LASGenerationOptions WithMinTokenCount(int minTokenCount, int? seed = null)
        {
            return new LASGenerationOptions
            {
                MinTokenCount = minTokenCount,
                Seed = seed
            };
        }

        /// <summary>
        /// Создает опции для генерации с указанным количеством операторных вершин
        /// </summary>
        public static LASGenerationOptions WithOperatorVertexCount(int operatorVertexCount, int? seed = null)
        {
            return new LASGenerationOptions
            {
                OperatorVertexCount = operatorVertexCount,
                Seed = seed
            };
        }

        /// <summary>
        /// Создает опции для генерации с указанным количеством условных вершин
        /// </summary>
        public static LASGenerationOptions WithConditionalVertexCount(int conditionalVertexCount, int? seed = null)
        {
            return new LASGenerationOptions
            {
                ConditionalVertexCount = conditionalVertexCount,
                Seed = seed
            };
        }
    }
}