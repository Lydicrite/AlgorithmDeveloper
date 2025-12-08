using System;

namespace AlgorithmDeveloper.AAModel.LAS.RandomLASGenerator
{
    /// <summary>
    /// Интерфейс для генератора случайных ЛСА (Логических Схем Алгоритмов)
    /// </summary>
    public interface IRandomLASGenerator
    {
        /// <summary>
        /// Генерирует случайную ЛСА с минимальным количеством токенов
        /// </summary>
        /// <param name="minTokenCount">Минимальное количество токенов</param>
        /// <param name="seed">Seed для генератора случайных чисел (опционально)</param>
        /// <returns>Строка с корректной ЛСА</returns>
        string GenerateByMinTokenCount(int minTokenCount, int? seed = null);

        /// <summary>
        /// Генерирует случайную ЛСА с указанным количеством операторных вершин
        /// </summary>
        /// <param name="operatorVertexCount">Количество операторных вершин</param>
        /// <param name="seed">Seed для генератора случайных чисел (опционально)</param>
        /// <returns>Строка с корректной ЛСА</returns>
        string GenerateByOperatorVertexCount(int operatorVertexCount, int? seed = null);

        /// <summary>
        /// Генерирует случайную ЛСА с указанным количеством условных вершин
        /// </summary>
        /// <param name="conditionalVertexCount">Количество условных вершин</param>
        /// <param name="seed">Seed для генератора случайных чисел (опционально)</param>
        /// <returns>Строка с корректной ЛСА</returns>
        string GenerateByConditionalVertexCount(int conditionalVertexCount, int? seed = null);

        /// <summary>
        /// Генерирует случайную ЛСА с настраиваемыми параметрами
        /// </summary>
        /// <param name="options">Параметры генерации</param>
        /// <returns>Строка с корректной ЛСА</returns>
        string Generate(LASGenerationOptions options);
    }
}