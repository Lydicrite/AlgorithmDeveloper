using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgorithmDeveloper.AlgorithmModel.LAS.RandomLASGenerator;
using AlgorithmDeveloper.AlgorithmModel.LAS;
using AlgorithmDeveloper.AlgorithmModel;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AlgorithmDeveloper.AlgorithmModel.LAS.RandomLASGenerator.Tests
{
    [TestClass]
    [assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 8)]
    public class RandomLASGeneratorTests
    {
        private LASValidator CreateValidator() => new LASValidator();

        private static string DescribeLAS(string las)
        {
            var tokens = las.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var operatorCount = tokens.Count(t => t.StartsWith("Y") && t != "Yн" && t != "Yк");
            var conditionalCount = tokens.Count(t => t.StartsWith("X") || t.StartsWith("P"));
            return $"ЛСА: {las}" +
                $"\n\tТокенов: {tokens.Length}" +
                $"\n\tОператорных вершин: {operatorCount}" +
                $"\n\tУсловных вершин: {conditionalCount}";
        }

        private static LASGenerationOptions BuildOptions
        (
            int? minTokens = null,
            int? operatorVertices = null,
            int? conditionalVertices = null,
            int? seed = null,
            int? maxAttempts = null
        )
        {
            var options = new LASGenerationOptions
            {
                MinTokenCount = minTokens,
                OperatorVertexCount = operatorVertices,
                ConditionalVertexCount = conditionalVertices,
                Seed = seed
            };

            if (maxAttempts.HasValue)
                options.MaxAttempts = maxAttempts.Value;

            return options;
        }

        private static bool SafeGenerate(LASGenerationOptions options, out string las, out string? genError)
        {
            try
            {
                las = RandomLASGenerator.Generate(options);
                genError = null;
                return true;
            }
            catch (Exception ex)
            {
                las = string.Empty;
                genError = ex.Message;
                return false;
            }
        }





        [TestMethod]
        [DataRow(10, 10, 10000)]
        [DataRow(10, 25, 10000)]
        public void BasicGeneration_ProducesValidLAS(int lasCount, int minTokens, int maxAttempts)
        {
            var val = CreateValidator();

            var options = BuildOptions(minTokens: minTokens, maxAttempts: maxAttempts);

            for (int i = 0; i < lasCount; i++)
            {
                Assert.IsTrue(SafeGenerate(options, out var las, out var genErr),
                    $"Генератор не смог создать ЛСА.\nОшибка: {genErr}\nОпции: MinTokens = {minTokens}, MaxAttempts = {maxAttempts}");

                Assert.IsFalse(string.IsNullOrWhiteSpace(las), "Сгенерирована пустая строка ЛСА.");

                Assert.IsTrue(val.TryValidate(las, out var model, out var parseErr),
                    $"ЛСА не проходит парсинг.\n{DescribeLAS(las)}\nОшибки парсера: {parseErr}");

                Assert.IsNotNull(model, "Парсер вернул пустую модель.");

                Console.Write($"\n\n\n\n\n[Тест {i + 1} / {lasCount}] Итерация корректна.\n{DescribeLAS(las)}\n\n{model.Information}\n");
            }
        }

        [TestMethod ]
        [DataRow(10, 10, 10000)]
        [DataRow(10, 20, 10000)]
        public void MinTokenCount_IsRespected(int lasCount, int minTokens, int maxAttempts)
        {
            var val = CreateValidator();
            var options = BuildOptions(minTokens: minTokens, maxAttempts: maxAttempts);

            for (int i = 0; i < lasCount; i++)
            {
                Assert.IsTrue(SafeGenerate(options, out var las, out var genErr),
                    $"Генерация не удалась. Ошибка: {genErr}\nОпции: MinTokens = {minTokens}, MaxAttempts = {maxAttempts}");

                var tokens = las.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Assert.IsGreaterThanOrEqualTo(minTokens,
                    tokens.Length, $"Недостаточно токенов ({tokens.Length} < {minTokens}).\n{DescribeLAS(las)}");

                Assert.IsTrue(val.TryValidate(las, out var model, out var parseErr),
                    $"ЛСА не проходит парсинг.\n{DescribeLAS(las)}\nОшибки парсера: {parseErr}");

                Assert.IsNotNull(model, "Парсер вернул пустую модель.");

                Console.Write($"\n\n\n\n\n[Тест {i + 1} / {lasCount}] Итерация корректна.\n{DescribeLAS(las)}\n\n{model.Information}\n");
            }
        }

        [TestMethod]
        [DataRow(5, 10, 3, 10000)]
        [DataRow(5, 15, 5, 10000)]
        public void OperatorVertexCount_IsExact(int lasCount, int minTokens, int operatorCount, int maxAttempts)
        {
            var val = CreateValidator();
            var options = BuildOptions(minTokens: minTokens, operatorVertices: operatorCount, maxAttempts: maxAttempts);

            for (int i = 0; i < lasCount; i++)
            {
                Assert.IsTrue(SafeGenerate(options, out var las, out var genErr),
                    $"Генерация не удалась. Ошибка: {genErr}\nОпции: MinTokens = {minTokens}, Operators = {operatorCount}, MaxAttempts = {maxAttempts}");

                var tokens = las.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var actual = tokens.Count(t => t.StartsWith("Y") && t != "Yн" && t != "Yк");
                Assert.AreEqual(operatorCount, actual,
                    $"Неверное количество операторных вершин ({actual} != {operatorCount}).\n{DescribeLAS(las)}");

                Assert.IsTrue(val.TryValidate(las, out var model, out var parseErr),
                    $"ЛСА не проходит парсинг.\n{DescribeLAS(las)}\nОшибки парсера: {parseErr}");

                Assert.IsNotNull(model, "Парсер вернул пустую модель.");

                Console.Write($"\n\n\n\n\n[Тест {i + 1} / {lasCount}] Итерация корректна.\n{DescribeLAS(las)}\n\n{model.Information}\n");
            }
        }

        [TestMethod]
        [DataRow(5, 8, 2, 10000)]
        [DataRow(5, 10, 4, 10000)]
        public void ConditionalVertexCount_IsExact(int lasCount, int minTokens, int conditionalCount, int maxAttempts)
        {
            var val = CreateValidator();
            var options = BuildOptions(minTokens: minTokens, conditionalVertices: conditionalCount, maxAttempts: maxAttempts);

            for (int i = 0; i < lasCount; i++)
            {
                Assert.IsTrue(SafeGenerate(options, out var las, out var genErr),
                    $"Генерация не удалась. Ошибка: {genErr}\nОпции: MinTokens = {minTokens}, Conditionals = {conditionalCount}, MaxAttempts = {maxAttempts}");

                var tokens = las.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var actual = tokens.Count(t => t.StartsWith("X") || t.StartsWith("P"));
                Assert.AreEqual(conditionalCount, actual,
                    $"Неверное количество условных вершин ({actual} != {conditionalCount}).\n{DescribeLAS(las)}");

                Assert.IsTrue(val.TryValidate(las, out var model, out var parseErr),
                   $"ЛСА не проходит парсинг.\n{DescribeLAS(las)}\nОшибки парсера: {parseErr}");

                Assert.IsNotNull(model, "Парсер вернул пустую модель.");

                Console.Write($"\n\n\n\n\n[Тест {i + 1} / {lasCount}] Итерация корректна.\n{DescribeLAS(las)}\n\n{model.Information}\n");
            }
        }

        [TestMethod]
        [DataRow(10, 12345, 10000)]
        [DataRow(10, 42, 10000)]
        public void DeterministicGeneration_WithSameSeed_IsEqual(int minTokens, int seed, int maxAttempts)
        {
            var options = BuildOptions(minTokens: minTokens, seed: seed, maxAttempts: maxAttempts);

            Assert.IsTrue(SafeGenerate(options, out var las1, out var err1),
                $"Первая генерация не удалась. Ошибка: {err1}\nОпции: MinTokens={minTokens}, Seed = {seed}, MaxAttempts = {maxAttempts}");

            Assert.IsTrue(SafeGenerate(options, out var las2, out var err2),
                $"Вторая генерация не удалась. Ошибка: {err2}\nОпции: MinTokens={minTokens}, Seed = {seed}, MaxAttempts = {maxAttempts}");

            Assert.AreEqual(las1, las2,
                $"Генерация с одинаковым seed даёт разные результаты.\nЛСА1: {DescribeLAS(las1)}\nЛСА2: {DescribeLAS(las2)}");
        }

        [TestMethod]
        [DataRow(10, 5,  10000, true)]
        [DataRow(10, 8,  10000, true)]
        [DataRow(10, 12, 10000, true)]
        [DataRow(10, 20, 10000, true)]
        [DataRow(10, 30, 1000000, true)]
        [DataRow(10, 40, 1000000, true)]
        // [DataRow(10, 60, 1000000, true)]

        [DataRow(1000, 5,  1000000, false)]
        [DataRow(1000, 8,  1000000, false)]
        [DataRow(1000, 12, 1000000, false)]
        [DataRow(1000, 20, 1000000, false)]
        [DataRow(1000, 30,  1000000, false)]
        [DataRow(1000, 40,  1000000, false)]
        // [DataRow(1000, 60, 1000000, false)]
        public void E2E_RandomLASGenerator_CorrectParseInModel_CorrectLASFromModelGeneration(int lasCount, int minTokens, int maxAttempts, bool logs)
        {
            var val = CreateValidator();
            var options = BuildOptions(minTokens: minTokens, maxAttempts: maxAttempts);

            for (int i = 0; i < lasCount; i++)
            {
                Assert.IsTrue(SafeGenerate(options, out var las, out var genErr),
                    $"Генерация не удалась. Ошибка: {genErr}\nОпции: MinTokens = {minTokens}, MaxAttempts = {maxAttempts}");

                Assert.IsTrue(val.TryValidate(las, out var m1, out var errors1), $"ЛСА не парсится.\n{DescribeLAS(las)}\nОшибки парсера: {errors1}");
                Assert.IsNotNull(m1, "Парсер вернул пустую модель для ЛСА 1.");

                var las2 = LASGenerator.Generate(m1!, out var log, enableLogging: true);

                if (!string.Equals(las, las2, StringComparison.Ordinal))
                {
                    var ok2 = val.TryValidate(las2, out var m2, out var errors2);
                    Assert.IsNotNull(m2, "Парсер вернул пустую модель для ЛСА 2." +
                        $"\nЛСА1: {DescribeLAS(las)}" +
                        $"\n\n{m1.Information}\n\n" +
                        $"\nЛСА2: {DescribeLAS(las2)}");

                    Assert.IsTrue(ok2,
                        $"Сгенерированная из модели ЛСА не парсится.\n{DescribeLAS(las2)}\nОшибки парсера: {errors2}\nЛог генератора:\n{log}");

                    Assert.IsTrue(m1!.Equals(m2!), 
                        $"Модели не эквивалентны после обратной генерации." +
                        $"\nЛСА1: {DescribeLAS(las)}" +
                        $"\n\n{m1.Information}\n\n" +
                        $"\nЛСА2: {DescribeLAS(las2)}" +
                        $"\n\n{m2.Information}\n\n" +
                        $"\nЛог генератора:\n{log}");

                    if (logs)
                        Console.Write
                        (
                            $"\n\n\n\n\n[Тест {i + 1} / {lasCount}] Итерация корректна - ЛСА 1 и ЛСА 2 различны." +
                            $"\nЛСА1: {DescribeLAS(las)}" +
                            $"\n\n{m1.Information}\n\n" +
                            $"\nЛСА2: {DescribeLAS(las2)}" +
                            $"\n\n{m2.Information}\n\n"
                        );
                }
                else
                {
                    if (logs)
                        Console.Write
                        (
                            $"\n\n\n\n\n[Тест {i + 1} / {lasCount}] Итерация корректна - ЛСА совпадают." +
                            $"\nЛСА: {DescribeLAS(las)}" +
                            $"\n\n{m1.Information}\n\n"
                        );
                }
            }
        }

        [TestMethod]
        [DataRow(10000, 5,  1000000,  true)]
        [DataRow(10000, 8,  1000000, true)]
        [DataRow(10000, 10, 1000000, true)]
        [DataRow(10000, 14, 1000000, true)]
        [DataRow(10000, 20,  1000000,  true)]
        [DataRow(10000, 30,  1000000, true)]
        [DataRow(10000, 40, 1000000, true)]
        public void Statistical_GenerationMetrics_Table(int lasCount, int minTokens, int maxAttempts, bool logs)
        {
            var val = CreateValidator();
            var options = BuildOptions(minTokens: minTokens, maxAttempts: maxAttempts);

            int total = 0;
            long tokensTotal = 0;
            long operatorsTotal = 0;
            long conditionalsTotal = 0;
            long transitionFormulasTotal = 0;
            int modelsWithCycles = 0;
            int cyclesTotal = 0;
            var cyclicModels = new Dictionary<int, (string, string)>();
            double sumLogTokens = 0.0;

            for (int i = 0; i < lasCount; i++)
            {
                Assert.IsTrue(SafeGenerate(options, out var las, out var genErr),
                    $"Генерация не удалась. Ошибка: {genErr}\nОпции: MinTokens = {minTokens}, MaxAttempts = {maxAttempts}");

                Assert.IsTrue(val.TryValidate(las, out var model, out var parseErr),
                    $"ЛСА не парсится.\n{DescribeLAS(las)}\nОшибки парсера: {parseErr}");
                Assert.IsNotNull(model, "Парсер вернул пустую модель.");

                var tokens = las.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int tokenCount = tokens.Length;
                int opCount = tokens.Count(t => t.StartsWith("Y") && t != "Yн" && t != "Yк");
                int condCount = tokens.Count(t => t.StartsWith("X") || t.StartsWith("P"));

                tokensTotal += tokenCount;
                operatorsTotal += opCount;
                conditionalsTotal += condCount;
                transitionFormulasTotal += model!.TransitionFormulas.Count;
                sumLogTokens += Math.Log(Math.Max(1, tokenCount));

                var cycles = model!.FindCycles(false);
                if (cycles.Count > 0)
                {
                    modelsWithCycles++;
                    cyclesTotal += cycles.Count;
                    cyclicModels.Add(i + 1, (las, model!.Information));
                }

                total++;
            }

            double meanTokensArithmetic = total > 0 ? tokensTotal / (double)total : 0.0;
            double meanTokensGeometric = total > 0 ? Math.Exp(sumLogTokens / total) : 0.0;

            string ovFreq = operatorsTotal > 0
                ? $"1 OV на {(tokensTotal / (double)operatorsTotal):F4} токен"
                : "OV отсутствуют";
            string ovPercent = tokensTotal > 0
                ? $"{(100.0 * operatorsTotal / tokensTotal):F4}%"
                : "0%";

            string cvFreq = conditionalsTotal > 0
                ? $"1 CV на {(tokensTotal / (double)conditionalsTotal):F4} токен"
                : "CV отсутствуют";
            string cvPercent = tokensTotal > 0
                ? $"{(100.0 * conditionalsTotal / tokensTotal):F4}%"
                : "0%";

            string cyclesSummary = total > 0
                ? $"{modelsWithCycles} из {total} ({(100.0 * modelsWithCycles / total):F4}%)"
                : "0 из 0 (0%)";

            string avgCyclesPerModel = total > 0 ? (cyclesTotal / (double)total).ToString("F4") : "0.0000";
            string avgCyclesPerCyclicModel = modelsWithCycles > 0 ? (cyclesTotal / (double)modelsWithCycles).ToString("F4") : "0.0000";
            string avgTransitionFormulas = total > 0 ? (transitionFormulasTotal / (double)total).ToString("F4") : "0.0000";
            string ovToCvRatio = conditionalsTotal > 0 ? (operatorsTotal / (double)conditionalsTotal).ToString("F4") : "∞";

            var sb = new StringBuilder();
            sb.AppendLine($"\n\n►► Статистики генерации ЛСА ◄◄");
            sb.AppendLine($"Параметры: lasCount={lasCount}, minTokens={minTokens}, maxAttempts={maxAttempts}, logs = {logs}");
            sb.AppendLine("------------------------------------------------------------------");
            sb.AppendLine("Метрика                                   | Значение");
            sb.AppendLine("------------------------------------------------------------------");
            sb.AppendLine($"Моделей с циклами                         | {cyclesSummary}");
            sb.AppendLine($"Среднее циклов на модель                  | {avgCyclesPerModel}");
            sb.AppendLine($"Среднее циклов на цикличную модель        | {avgCyclesPerCyclicModel}");
            sb.AppendLine($"Операторные вершины: частота              | {ovFreq}");
            sb.AppendLine($"Операторные вершины: % токенов            | {ovPercent}");
            sb.AppendLine($"Условные вершины: частота                 | {cvFreq}");
            sb.AppendLine($"Условные вершины: % токенов               | {cvPercent}");
            sb.AppendLine($"OV/CV                                     | {ovToCvRatio}");
            sb.AppendLine($"Среднее число формул перехода             | {avgTransitionFormulas}");
            sb.AppendLine($"Среднее число токенов (арифм.)            | {meanTokensArithmetic:F4}");
            sb.AppendLine($"Среднее число токенов (геом.)             | {meanTokensGeometric:F4}");
            sb.AppendLine($"Строка: minTokens/ср.арифм/ср.геом        | {minTokens} / {meanTokensArithmetic:F4} / {meanTokensGeometric:F4}");
            sb.AppendLine("------------------------------------------------------------------");

            sb.AppendLine("\nИнтерпретация:");
            sb.AppendLine("- Арифметическое среднее чувствительно к редким длинным ЛСА (выбросам).");
            sb.AppendLine("- Геометрическое среднее устойчивее и лучше отражает 'типичный' размер.");
            sb.AppendLine("- Для оценки плотности вершин полезны обе метрики вместе.");

            if (logs && cyclicModels.Count > 0)
            {
                sb.AppendLine($"Индексы моделей с циклами: [{string.Join(", ", cyclicModels.Keys)}]\nМодели с циклами: \n");
                foreach (var (index, (las, info)) in cyclicModels)
                {
                    sb.AppendLine($"\n\n\n\n\n--- Модель #{index} ---");
                    sb.AppendLine(DescribeLAS(las));
                    sb.AppendLine($"\n{info}");
                    sb.AppendLine();
                }
            }

            Console.Write(sb.ToString());
        }
    }
}
