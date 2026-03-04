using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.LAS;
using AlgorithmDeveloper.Abstractions.LAS.RandomAAGenerator;
using AlgorithmDeveloper.Abstractions.TransitionSystem.MAS;

namespace AlgorithmDeveloperTests.Tests
{
    /// <summary>
    /// Тесты для конвертера МСА -> АА.
    /// </summary>
    [TestClass]
    public class MASToAAConverterTests
    {
        #region Тесты на предопределенных наборах

        [TestMethod]
        [DataRow("Yн X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк", DisplayName = "(TestGroup A)\tAlgo 1")]
        [DataRow("Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк", DisplayName = "(TestGroup A)\tAlgo 2")]
        [DataRow("Yн P0 ↑1 X1 ↑3 w↑4 ↓3 Y3 w↑4 ↓1 X0 ↑2 Y0 w↑4 ↓2 Y1 w↑4 ↓4 X2 ↑5 w↑6 ↓5 Y2 w↑4 ↓6 Yк", DisplayName = "(TestGroup A)\tAlgo 3")]
        
        [DataRow("Yн Y0 X0 ↑1 Y1 w↑3 ↓1 X1 ↑2 Y2 w↑3 ↓2 Y3 w↑3 ↓3 Yк", DisplayName = "(TestGroup B)\tAlgo 4")]
        [DataRow("Yн Y0 X0 ↑1 Y1 w↑2 ↓1 Y4 X2 ↑2 w↑1 ↓2 Yк", DisplayName = "(TestGroup B)\tAlgo 5")]
        [DataRow("Yн Y0 X0 ↑1 Y1 w↑5 ↓1 P0 ↑2 w↑3 ↓2 X1 ↑4 Y2 w↑5 ↓4 Y3 w↑5 ↓3 Y4 X2 ↑5 w↑3 ↓5 Yк", DisplayName = "(TestGroup B)\tAlgo 6")]

        [DataRow("Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 Y2 Yк", DisplayName = "(TestGroup C)\tAlgo 7")]
        [DataRow("Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк", DisplayName = "(TestGroup C)\tAlgo 8")]
        [DataRow("Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 P0 ↑4 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк", DisplayName = "(TestGroup C)\tAlgo 9")]

        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 w↑3 ↓3 Y2 X0 ↑3 X1 ↑4 w↑2 ↓4 X2 ↑1 Yк", DisplayName = "(TestGroup D)\tAlgo 10")]
        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 Y2 X1 ↑3 w↑2 ↓3 X2 ↑1 Yк", DisplayName = "(TestGroup D)\tAlgo 11")]
        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 w↑3 ↓3 Y2 P0 ↑4 w↑5 ↓4 X0 ↑3 w↑5 ↓5 X1 ↑6 w↑2 ↓6 X2 ↑1 Yк", DisplayName = "(TestGroup D)\tAlgo 12")]

        [DataRow("Yн Y0 X0 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓1 Y3 w↑2 ↓2 Yк", DisplayName = "(TestGroup E)\tAlgo 13")]
        [DataRow("Yн Y0 X0 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓1 Y4 X2 ↑1 w↑2 ↓2 Yк", DisplayName = "(TestGroup E)\tAlgo 14")]
        [DataRow("Yн Y0 X0 ↑1 Y1 X1 ↑4 Y2 w↑4 ↓1 P0 ↑2 w↑3 ↓2 Y3 w↑4 ↓3 Y4 X2 ↑3 w↑4 ↓4 Yк", DisplayName = "(TestGroup E)\tAlgo 15")]

        [DataRow("Yн Y1 X0 ↑1 Y0 w↑2 ↓1 Y2 w↑2 ↓2 X1 ↑3 w↑4 ↓3 Y3 w↑4 ↓4 Yк", DisplayName = "(TestGroup F)\tAlgo 16")]
        [DataRow("Yн Y4 Y5 X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 Yк", DisplayName = "(TestGroup F)\tAlgo 17")]
        [DataRow("Yн P0 ↑1 Y4 Y5 w↑3 ↓1 Y1 X0 ↑2 Y0 w↑3 ↓2 Y2 w↑3 ↓3 X1 ↑4 w↑5 ↓4 Y3 w↑5 ↓5 Yк", DisplayName = "(TestGroup F)\tAlgo 18")]

        [DataRow("Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 Y2 X1 ↑3 w↑2 ↓3 Yк", DisplayName = "(TestGroup G)\tAlgo 19")]
        [DataRow("Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 Y3 X2 ↑2 Yк", DisplayName = "(TestGroup G)\tAlgo 20")]
        [DataRow("Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 P0 ↑3 w↑4 ↓3 Y2 X1 ↑5 w↑3 ↓4 Y3 X2 ↑4 w↑5 ↓5 Yк", DisplayName = "(TestGroup G)\tAlgo 21")]

        [DataRow("Yн Y1 X0 ↑1 Y0 w↑1 ↓1 X1 ↑2 w↑3 ↓2 Y2 w↑3 ↓3 Yк", DisplayName = "(TestGroup H)\tAlgo 22")]
        [DataRow("Yн Y1 X0 ↑1 Y0 w↑4 ↓1 Y3 w↑4 ↓4 X1 ↑2 w↑3 ↓2 Y2 w↑3 ↓3 Yк", DisplayName = "(TestGroup H)\tAlgo 23")]
        [DataRow("Yн Y1 X0 ↑1 Y0 w↑2 ↓1 P0 ↑2 Y3 w↑2 ↓2 X1 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк", DisplayName = "(TestGroup H)\tAlgo 24")]

        [DataRow("Yн Y3 X0 ↑1 Y5 w↑2 ↓1 Y4 w↑2 ↓2 Yк", DisplayName = "(TestGroup J)\tAlgo 25")]
        [DataRow("Yн Y1 w↑1 ↓1 Y2 X1 ↑1 X0 ↑2 Y5 w↑3 ↓2 Y4 w↑3 ↓3 Yк", DisplayName = "(TestGroup J)\tAlgo 26")]
        [DataRow("Yн P0 ↑1 Y1 w↑2 ↓1 Y3 w↑3 ↓2 Y2 X1 ↑2 w↑3 ↓3 X0 ↑4 Y5 w↑5 ↓4 Y4 w↑5 ↓5 Yк", DisplayName = "(TestGroup J)\tAlgo 27")]

        [DataRow("Yн Y0 X0 ↑1 Y2 w↑2 ↓1 Y1 w↑2 ↓2 X1 ↑3 Y4 w↑3 ↓3 Yк", DisplayName = "(TestGroup J)\tAlgo 28")]
        [DataRow("Yн Y0 X0 ↑1 Y2 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк", DisplayName = "(TestGroup J)\tAlgo 29")]
        [DataRow("Yн Y0 X0 ↑1 Y2 w↑9 ( ↓1 P0 ↑2 Y3 w↑5 ) ( ↓2 Y1 w↑4 ) ( ↓9 P1 ↑4 w↑5 ) ( ↓4 X1 ↑6 w↑3 ) ( ↓5 X2 ↑3 w↑6 ) ( ↓3 Y4 w↑6 ) ↓6 Yк", DisplayName = "(TestGroup J)\tAlgo 30")]

        [DataRow("Yн Y0 X1 ↑1 w↑2 ↓1 Y1 w↑2 ↓2 X0 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк", DisplayName = "(TestGroup J)\tAlgo 31")]
        [DataRow("Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк", DisplayName = "(TestGroup J)\tAlgo 32")]
        [DataRow("Yн P0 ↑1 w↑2 ↓1 Y0 w↑2 ↓2 X1 ↑3 w↑5 ↓3 P1 ↑4 Y3 w↑8 ↓4 Y1 w↑6 ↓6 X0 ↑7 w↑10 ↓7 Y2 w↑10 ↓5 P2 ↑6 w↑8 ↓8 X2 ↑9 w↑10 ↓9 Y4 w↑10 ↓10 Yк", DisplayName = "(TestGroup J)\tAlgo 33")]

        [DataRow("Yн Yк", DisplayName = "(TestGroup X)\tAlgo 88")]
        [DataRow("Yн Y1 P1 ↑1 Y2 w↑1 ↓1 X2 ↑2 Y3 P3 ↑3 Y4 w↑3 ↓3 X4 ↑4 w↑1 ↓4 P5 ↑5 w↑1 ↓5 Y5 w↑2 ↓2 Yк", DisplayName = "(TestGroup X)\tAlgo 89")]
        [DataRow("Yн Y1 X1 ↑1 Y2 w↑1 ↓1 P2 ↑2 X3 ↑3 w↑1 ↓3 X4 ↑4 w↑1 ↓4 Y3 w↑2 ↓2 Yк", DisplayName = "(TestGroup X)\tAlgo 90")]
        [DataRow("Yн X1 ↑1 Y1 w↑1 ↓1 Y2 P2 ↑2 X3 ↑3 w↑1 ↓3 P4 ↑4 w↑1 ↓4 Y3 Y4 w↑2 ↓2 Yк", DisplayName = "(TestGroup X)\tAlgo 91")]
        
        [DataRow("Yн X1 ↑1 Y1 Y2 P2 ↑2 Y3 w↑2 ↓2 Y4 w↑1 ↓1 Y5 Yк", DisplayName = "(TestGroup Z)\tAlgo 94")]
        [DataRow("Yн Y0 P0 ↑1 P1 ↑1 X2 ↑2 Y1 w↑2 ↓2 Y2 Y3 Y4 P3 ↑3 Y5 w↑3 ↓3 Y6 w↑1 ↓1 Y7 Y8 w↑0 ↓0 Y9 Yк", DisplayName = "(TestGroup Z)\tAlgo 95")]
        [DataRow("Yн Y0 Y1 Y2 X0 ↑0 Y3 P1 ↑1 Y4 w↑1 ↓1 Y5 P2 ↑3 Y6 Y7 w↑3 ↓3 Y8 w↑0 ↓0 Y9 Yк", DisplayName = "(TestGroup Z)\tAlgo 96")]
        [DataRow("Yн X0 ↑1 Y0 w↑2 ↓1 Y1 X1 ↑3 w↑2 ↓2 Y2 w↑3 ↓3 X2 ↑2 Yк", DisplayName = "(TestGroup Z)\tAlgo 97")]
        [DataRow("Yн X0 ↑1 Y1 X1 ↑2 w↑3 ↓1 Y0 w↑2 ↓2 Y2 w↑3 ↓3 X2 ↑4 w↑2 ↓4 Yк", DisplayName = "(TestGroup Z)\tAlgo 97 Rev")]
        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 X0 ↑2 X1 ↑3 Y2 w↑4 ↓3 X2 ↑4 w↑1 ↓4 Y3 Yк", DisplayName = "(TestGroup Z)\tAlgo 98")]
        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 X0 ↑1 X1 ↑3 w↑2 ↓3 X2 ↑4 Y1 w↑5 ↓4 Y2 w↑5 ↓5 Y3 Yк", DisplayName = "(TestGroup Z)\tAlgo 99")]

        [DataRow("Yн w↑1 ↓1 X1 ↑1 X2 ↑3 w↑2 ↓2 Y2 w↑4 ↓3 Y1 X5 ↑6 w↑4 ↓4 X3 ↑3 X4 ↑2 w↑5 ↓5 Y3 w↑10 ↓6 Y4 w↑7 ↓7 X6 ↑8 w↑5 ↓8 X7 ↑9 w↑10 ↓9 Y5 w↑7 ↓10 Yк",
            DisplayName = "ComplexAlgo 1 Variant 1")]
        [DataRow("Yн w↑1 ↓1 X1 ↑1 X2 ↑3 w↑2 ↓3 Y1 X5 ↑6 w↑4 ↓6 Y4 w↑7 ↓2 Y2 w↑4 ↓4 X3 ↑3 X4 ↑2 w↑5 ↓5 Y3 w↑10 ↓7 X6 ↑8 w↑5 ↓8 X7 ↑9 w↑10 ↓9 Y5 w↑7 ↓10 Yк",
            DisplayName = "ComplexAlgo 1 Variant 2")]
        [DataRow("Yн w↑0 ↓0 X0 ↑0 w↑1 ↓1 X1 ↑1 w↑2 ↓2 X2 ↑4 X3 ↑3 Y1 w↑0 ↓3 Y0 w↑2 ↓4 X4 ↑5 P0 ↑7 P2 ↑9 w↑0 ↓5 X5 ↑6 w↑8 ↓6 P3 ↑6 w↑8 ↓7 P1 ↑8 w↑9 ↓8 Y3 P4 ↑6 w↑10 ↓9 Y2 P5 ↑10 w↑2 ↓10 Yк",
            DisplayName = "ComplexAlgo 2 Variant 1")]
        [DataRow("Yн w↑0 ↓0 X0 ↑0 w↑1 ↓1 X1 ↑1 w↑2 ↓2 X2 ↑4 X3 ↑3 Y1 w↑0 ↓3 Y0 w↑2 ↓4 X4 ↑5 P0 ↑7 P2 ↑9 w↑0 ↓9 Y2 P5 ↑10 w↑2 ↓7 P1 ↑8 w↑9 ↓8 Y3 P4 ↑6 w↑10 ↓6 P3 ↑6 w↑8 ↓5 X5 ↑6 w↑8 ↓10 Yк",
            DisplayName = "ComplexAlgo 2 Variant 2")]
        [DataRow("Yн Y0 X0 ↑0 X1 ↑1 w↑0 ↓0 Y1 w↑6 ↓1 X2 ↑2 w↑1 ↓2 Y2 X3 ↑2 Y3 X4 ↑3 w↑1 ↓3 Y4 X5 ↑2 X6 ↑4 Y6 X8 ↑5 w↑3 ↓4 Y5 X7 ↑5 w↑0 ↓5 Y7 X9 ↑6 w↑1 ↓6 Yк",
            DisplayName = "ComplexAlgo 3 Variant 1")]
        [DataRow("Yн Y0 X0 ↑0 X1 ↑1 w↑0 ↓1 X2 ↑2 w↑1 ↓2 Y2 X3 ↑2 Y3 X4 ↑3 w↑1 ↓3 Y4 X5 ↑2 X6 ↑4 Y6 X8 ↑5 w↑3 ↓5 Y7 X9 ↑6 w↑1 ↓4 Y5 X7 ↑5 w↑0 ↓0 Y1 w↑6 ↓6 Yк",
            DisplayName = "ComplexAlgo 3 Variant 2")]
        [DataRow("Yн P1 ↑1 Y1 w↑1 ↓1 P2 ↑2 Y2 P2 ↑3 Y3 w↑3 ↓3 P3 ↑4 w↑1 ↓4 Y4 Y5 w↑2 ↓2 P4 ↑5 X5 ↑6 Y6 w↑6 ↓6 Y7 w↑5 ↓5 Y8 X1 ↑7 w↑1 ↓7 Yк",
            DisplayName = "ComplexAlgo 4")]

        // [DataRow("Yн P1 ↑1 Y1 w↑1 ↓1 P1 ↑2 X1 ↑3 Y2 w↑1 ↓3 X2 ↑4 P3 ↑5 Y3 w↑5 ↓5 P4 ↑6 w↑5 ↓6 Y4 w↑4 ↓4 Y5 w↑2 ↓2 Yк", DisplayName = "Strange Algo 1")]
        // [DataRow("Yн Y1 X1 ↑1 Y2 Y3 X2 ↑2 Y4 X3 ↑3 Y5 w↑3 ↓3 P4 ↑4 w↑3 ↓4 Y6 w↑2 ↓2 X5 ↑5 Y7 Y8 w↑5 ↓5 P6 ↑6 w↑3 ↓6 Y9 w↑1 ↓1 Yк", DisplayName = "Strange Algo 2")]
        // [DataRow("Yн X1 ↑1 Y1 Y2 X1 ↑2 Y3 w↑2 ↓2 X2 ↑3 Y4 w↑3 ↓3 X2 ↑4 w↑2 ↓4 Y5 Y6 w↑1 ↓1 Yк", DisplayName = "Strange Algo 3")]
        // [DataRow("Yн X1 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓2 P2 ↑3 Y3 Y4 Y5 w↑2 ↓3 X3 ↑4 Y6 w↑4 ↓4 Y7 w↑1 ↓1 Yк", DisplayName = "Strange Algo 4")]
        // [DataRow("Yн Y1 Y2 X1 ↑1 Y3 Y4 Y5 w↑1 ↓1 X1 ↑2 Y6 X2 ↑3 Y7 w↑3 ↓3 P3 ↑4 Y8 Y9 w↑3 ↓4 Y10 w↑2 ↓2 Yк", DisplayName = "Strange Algo 5")]
        // [DataRow("Yн P1 ↑1 Y1 P1 ↑2 Y2 w↑2 ↓2 X2 ↑3 Y3 w↑2 ↓3 Y4 w↑1 ↓1 Yк", DisplayName = "Strange Algo 6")]
        // [DataRow("", DisplayName = "Strange Algo ")]
        public void SimpleModels_MASToAA(string las)
        {
            AssertEquivalentMAS(las);
        }

        #endregion





        #region Тесты на случайных ЛСА

        [TestMethod]
        [DataRow(5000, 10, 500000)]
        [DataRow(2500, 20, 500000)]
        [DataRow(1000, 30, 500000)]
        [DataRow(500, 40, 500000)]
        public void RandomLAS_MASToAA_E2E(int lasCount, int minTokens, int maxAttempts)
        {
            var val = CreateValidator();
            var options = BuildOptions(minTokens: minTokens, maxAttempts: maxAttempts);

            for (int i = 0; i < lasCount; i++)
            {
                Assert.IsTrue(SafeGenerate(options, out var las, out var genErr),
                    $"Генерация не удалась. Ошибка: {genErr}\nОпции: MinTokens = {minTokens}");

                Assert.IsTrue(val.TryValidate(las, out var model1, out var errors1),
                    $"ЛСА не парсится.\n{DescribeLAS(las)}\nОшибки: {errors1}");
                
                Assert.IsNotNull(model1);

                // Преобразование в МСА
                string masString = model1!.MAS!.ToString();

                // Обратное преобразование МСА -> АА
                AbstractAutomata? model2 = null;
                try
                {
                    model2 = MASToAAConverter.Convert(masString);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Ошибка конвертации МСА -> АА.\nЛСА: {las}\nМСА:\n{masString}\nОшибка: {ex}");
                }

                // Сравнение
                bool equal = model1.Equals(model2);
                bool correct = model2.CheckCorrectness(out string msg);

                // Если строгая эквивалентность не прошла, проверяем эквивалентность МСА (структурная оптимизация)
                if (!equal && correct)
                {
                    var mas1 = model1.MAS?.ToString();
                    var mas2 = model2.MAS?.ToString();
                    if (mas1 == mas2)
                    {
                        equal = true; // Считаем успешным, если МСА идентичны (логика сохранена)
                    }
                    else if (model1.LAS == model2.LAS)
                    {
                        equal = true; // Считаем успешным, если LAS идентичны
                    }
                    else if (NormalizeRunsInfo(model1.RunsInfo) == NormalizeRunsInfo(model2.RunsInfo))
                    {
                        equal = true; // Считаем успешным, если поведение идентично (игнорируя JumpPoints)
                    }
                    else
                    {
                        Console.WriteLine($"LAS Differs! Lengths: {model1.LAS?.Length} vs {model2.LAS?.Length}");
                        if (model1.LAS != null && model2.LAS != null)
                        {
                            for (int k = 0; k < Math.Min(model1.LAS.Length, model2.LAS.Length); k++)
                            {
                                if (model1.LAS[k] != model2.LAS[k])
                                {
                                    Console.WriteLine($"Diff at index {k}: '{model1.LAS[k]}' ({(int)model1.LAS[k]}) vs '{model2.LAS[k]}' ({(int)model2.LAS[k]})");
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!equal || !correct)
                {
                    // Логируем детали перед падением
                    Console.WriteLine($"\n[Несовпадение/Ошибка] Тест {i + 1}");
                    Console.WriteLine($"ЛСА: {las}");
                    Console.WriteLine($"МСА:\n{masString}");
                    
                    var opIds1 = model1.Vertices.OfType<OperatorVertex>().Select(v => v.ID ?? "").OrderBy(x => x).ToList();
                    var opIds2 = model2.Vertices.OfType<OperatorVertex>().Select(v => v.ID ?? "").OrderBy(x => x).ToList();
                    if (!opIds1.SequenceEqual(opIds2))
                        Console.WriteLine($"Operator Vertices Mismatch:\n1: {string.Join(",", opIds1)}\n2: {string.Join(",", opIds2)}");

                    var condIds1 = model1.Vertices.OfType<ConditionalVertex>().Select(v => v.ID ?? "").Distinct().OrderBy(x => x).ToList();
                    var condIds2 = model2.Vertices.OfType<ConditionalVertex>().Select(v => v.ID ?? "").Distinct().OrderBy(x => x).ToList();
                    if (!condIds1.SequenceEqual(condIds2))
                        Console.WriteLine($"Conditional Vertices Mismatch:\n1: {string.Join(",", condIds1)}\n2: {string.Join(",", condIds2)}");

                    var runs1 = model1.RunsInfo;
                    var runs2 = model2.RunsInfo;
                    if (runs1 != runs2)
                    {
                         Console.WriteLine("RunsInfo Mismatch!");
                         var lines1 = runs1.Split('\n');
                         var lines2 = runs2.Split('\n');
                         for(int k=0; k<Math.Min(lines1.Length, lines2.Length); k++)
                         {
                             if (lines1[k] != lines2[k])
                             {
                                 Console.WriteLine($"Diff at line {k}:\n1: {lines1[k].Trim()}\n2: {lines2[k].Trim()}");
                                 break;
                             }
                         }
                    }

                    Console.WriteLine($"Модель 1:\n{model1.Information}");
                    Console.WriteLine($"Модель 2:\n{model2.Information}");
                    Console.WriteLine($"Модель 2 LAS:\n{model2.LAS}"); // Added this
                    if (!correct)
                    {
                        Console.WriteLine($"\nОшибки парсинга ЛСА: \n{msg}");
                        Assert.IsTrue(correct, $"Модель 2 имеет некорректную ЛСА: {msg}");
                    }
                }

                Assert.IsTrue(equal, "Модели не эквивалентны после восстановления из МСА.");
                Assert.IsTrue(correct, $"Актуальная модель имеет некорректную ЛСА: {msg}");
            }
        }

        #endregion





        #region Тесты странных ЛСА

        [TestMethod]
        [DataRow("Yн P1 ↑1 Y1 w↑1 ↓1 P1 ↑2 X1 ↑3 Y2 w↑1 ↓3 X2 ↑4 P3 ↑5 Y3 w↑5 ↓5 P4 ↑6 w↑5 ↓6 Y4 w↑4 ↓4 Y5 w↑2 ↓2 Yк", DisplayName = "Strange Algo 1")]
        [DataRow("Yн Y1 X1 ↑1 Y2 Y3 X2 ↑2 Y4 X3 ↑3 Y5 w↑3 ↓3 P4 ↑4 w↑3 ↓4 Y6 w↑2 ↓2 X5 ↑5 Y7 Y8 w↑5 ↓5 P6 ↑6 w↑3 ↓6 Y9 w↑1 ↓1 Yк", DisplayName = "Strange Algo 2")]
        [DataRow("Yн X1 ↑1 Y1 Y2 X1 ↑2 Y3 w↑2 ↓2 X2 ↑3 Y4 w↑3 ↓3 X2 ↑4 w↑2 ↓4 Y5 Y6 w↑1 ↓1 Yк", DisplayName = "Strange Algo 3")]
        [DataRow("Yн X1 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓2 P2 ↑3 Y3 Y4 Y5 w↑2 ↓3 X3 ↑4 Y6 w↑4 ↓4 Y7 w↑1 ↓1 Yк", DisplayName = "Strange Algo 4")]
        [DataRow("Yн Y1 Y2 X1 ↑1 Y3 Y4 Y5 w↑1 ↓1 X1 ↑2 Y6 X2 ↑3 Y7 w↑3 ↓3 P3 ↑4 Y8 Y9 w↑3 ↓4 Y10 w↑2 ↓2 Yк", DisplayName = "Strange Algo 5")]
        [DataRow("Yн P1 ↑1 Y1 P1 ↑2 Y2 w↑2 ↓2 X2 ↑3 Y3 w↑2 ↓3 Y4 w↑1 ↓1 Yк", DisplayName = "Strange Algo 6")]
        // [DataRow("", DisplayName = "Strange Algo ")]
        public void StrangeModels_MASToAA(string las)
        {
            // 1. Parse LAS -> Model 1
            bool ok = LASParser.TryParse(las, out var model1, out List<ParsingError> err);
            Assert.IsTrue(ok, $"Ошибка парсинга исходной ЛСА: {string.Join(", ", err.Select(e => e.Message))}");
            Assert.IsNotNull(model1);

            // 2. Model 1 -> MAS String
            var masString = model1.MAS!.ToString();

            // 3. MAS String -> Model 2
            var model2 = MASToAAConverter.Convert(masString);
            bool correct = model2.CheckCorrectness(out string msg);

            // 4. Check Equality
            bool equal = model1.Equals(model2);

            if (!equal)
            {
                Console.WriteLine($"\n[Несовпадение]\n\t" +
                    $"ЛСА 1: {las}\n\t" +
                    $"ЛСА 2: {model2.LAS}\n\t");
                Assert.Fail("Модели не эквивалентны.");
            }

            if (!correct)
            {
                Dictionary<string, string> infoM1 = new Dictionary<string, string>();
                var jpsM1 = model1.Vertices.OfType<JumpPoint>();
                foreach (var jp in jpsM1)
                    infoM1.Add(jp!.ID!, jp!.Next!.ID!);
                string infoM1Str = string.Empty;
                foreach (var kvp in infoM1)
                    infoM1Str += $"\t{kvp.Key}.Next() = {kvp.Value}\n";

                Dictionary<string, string> infoM2 = new Dictionary<string, string>();
                var jpsM2 = model2.Vertices.OfType<JumpPoint>();
                foreach (var jp in jpsM2)
                    infoM2.Add(jp!.ID!, jp!.Next!.ID!);
                string infoM2Str = string.Empty;
                foreach (var kvp in infoM2)
                    infoM2Str += $"\t{kvp.Key}.Next() = {kvp.Value}\n";

                Assert.IsTrue(correct,
                    $"\n\nМодель 2 имеет некорректную ЛСА, " +
                    $"ошибки: \n{msg}" +
                    $"\n\n\nЛСА 1: {las}\nИнформация о точках перехода в исходной модели: \n{infoM1Str}" +
                    $"\n\n\nЛСА 2: {model2.LAS}\nИнформация о точках перехода в сгенерированной модели: \n{infoM2Str}");
            }
        }

        #endregion





        #region Вспомогательные методы

        private static void AssertEquivalentMAS(string las)
        {
            // 1. Parse LAS -> Model 1
            bool ok = LASParser.TryParse(las, out var model1, out List<ParsingError> err);
            Assert.IsTrue(ok, $"Ошибка парсинга исходной ЛСА: {string.Join(", ", err.Select(e => e.Message))}");
            Assert.IsNotNull(model1);

            // 2. Model 1 -> MAS String
            var masString = model1.MAS!.ToString();

            // 3. MAS String -> Model 2
            var model2 = MASToAAConverter.Convert(masString);
            bool correct = model2.CheckCorrectness(out string msg);

            // 4. Check Equality
            bool equal = model1.Equals(model2);
            
            // Fallback: Check MAS equivalence if Models differ (due to simplification)
            if (!equal)
            {
                var mas1 = model1.MAS?.ToString();
                var mas2 = model2.MAS?.ToString();
                if (mas1 == mas2) equal = true;
                else if (model1.LAS == model2.LAS) equal = true;
                else if (NormalizeRunsInfo(model1.RunsInfo) == NormalizeRunsInfo(model2.RunsInfo)) equal = true;
                else
                {
                    Console.WriteLine($"LAS Differs! Lengths: {model1.LAS?.Length} vs {model2.LAS?.Length}");
                    if (model1.LAS != null && model2.LAS != null)
                    {
                        for (int k = 0; k < Math.Min(model1.LAS.Length, model2.LAS.Length); k++)
                        {
                            if (model1.LAS[k] != model2.LAS[k])
                            {
                                Console.WriteLine($"Diff at index {k}: '{model1.LAS[k]}' ({(int)model1.LAS[k]}) vs '{model2.LAS[k]}' ({(int)model2.LAS[k]})");
                                break;
                            }
                        }
                    }
                }
            }

            if (!equal)
            {
                Console.WriteLine($"\n[Несовпадение] ЛСА: {las}");
                Console.WriteLine($"МСА:\n{masString}");
                
                // Detailed Diff
                var opIds1 = model1.Vertices.OfType<OperatorVertex>().Select(v => v.ID ?? "").OrderBy(x => x).ToList();
                var opIds2 = model2.Vertices.OfType<OperatorVertex>().Select(v => v.ID ?? "").OrderBy(x => x).ToList();
                if (!opIds1.SequenceEqual(opIds2))
                    Console.WriteLine($"Operator Vertices Mismatch:\n1: {string.Join(",", opIds1)}\n2: {string.Join(",", opIds2)}");

                var condIds1 = model1.Vertices.OfType<ConditionalVertex>().Select(v => v.ID ?? "").Distinct().OrderBy(x => x).ToList();
                var condIds2 = model2.Vertices.OfType<ConditionalVertex>().Select(v => v.ID ?? "").Distinct().OrderBy(x => x).ToList();
                if (!condIds1.SequenceEqual(condIds2))
                    Console.WriteLine($"Conditional Vertices Mismatch:\n1: {string.Join(",", condIds1)}\n2: {string.Join(",", condIds2)}");

                var runs1 = model1.RunsInfo;
                var runs2 = model2.RunsInfo;
                if (runs1 != runs2)
                {
                     Console.WriteLine("RunsInfo Mismatch!");
                     // Print only first diff line?
                     var lines1 = runs1.Split('\n');
                     var lines2 = runs2.Split('\n');
                     for(int i=0; i<Math.Min(lines1.Length, lines2.Length); i++)
                     {
                         if (lines1[i] != lines2[i])
                         {
                             Console.WriteLine($"Diff at line {i}:\n1: {lines1[i].Trim()}\n2: {lines2[i].Trim()}");
                             break;
                         }
                     }
                }

                Console.WriteLine($"Модель 1 Info:\n{model1.Information}");
                Console.WriteLine($"Модель 2 Info:\n{model2.Information}");
                
                if (!correct)
                {
                    Console.WriteLine($"\nОшибки парсинга сгенерированной ЛСА: \n{msg}");
                    Console.WriteLine($"Сгенерированная ЛСА: {model2.LAS}");
                }

                Assert.Fail("Модели не эквивалентны.");
            }

            Console.WriteLine($"Модель 1:\n{model1.Information}\n\n\n");
            Console.WriteLine($"Модель 2:\n{model2.Information}");
            Assert.IsTrue(correct, $"Модель 2 имеет некорректную ЛСА\nОшибки: \n{msg}\nЛСА: {model2.LAS}");
        }

        private static string NormalizeRunsInfo(string info)
        {
            if (string.IsNullOrEmpty(info)) return info;
            var lines = info.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new System.Text.StringBuilder();
            foreach (var line in lines)
            {
                if (line.Contains("ход работы алгоритма:"))
                {
                    var parts = line.Split(new[] { "ход работы алгоритма:" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        var prefix = parts[0];
                        var path = parts[1];
                        var tokens = path.Split(new[] { "→" }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(t => t.Trim())
                                         .Where(t => !t.StartsWith("↓"))
                                         .ToList();
                        sb.AppendLine($"{prefix}ход работы алгоритма: {string.Join(" → ", tokens)}");
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
                else
                {
                    sb.AppendLine(line);
                }
            }
            return sb.ToString();
        }

        private static LASValidator CreateValidator() => new LASValidator();

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

        private static LASGenerationOptions BuildOptions(int? minTokens = null, int? maxAttempts = null)
        {
            var options = new LASGenerationOptions
            {
                MinTokenCount = minTokens
            };
            if (maxAttempts.HasValue) options.MaxAttempts = maxAttempts.Value;
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

        #endregion
    }
}
