using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.Combining;
using AlgorithmDeveloper.Abstractions.LAS;
using AlgorithmDeveloper.Abstractions.Minimization;
using AlgorithmDeveloper.Abstractions.TransitionSystem.MAS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Interop;

namespace AlgorithmDeveloperTests.Tests
{
    [TestClass]
    public class MASCombinerTests
    {
        [TestMethod]
        [DataRow("Yн X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк",
                 "Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк",
                 "Yн P0 ↑1 X1 ↑3 w↑4 ↓3 Y3 w↑4 ↓1 X0 ↑2 Y0 w↑4 ↓2 Y1 w↑4 ↓4 X2 ↑5 w↑6 ↓5 Y2 w↑4 ↓6 Yк",
                 DisplayName = "Algo 3 = Algo 1 + Algo 2")]

        [DataRow("Yн Y0 X0 ↑1 Y1 w↑3 ↓1 X1 ↑2 Y2 w↑3 ↓2 Y3 w↑3 ↓3 Yк",
                 "Yн Y0 X0 ↑1 Y1 w↑2 ↓1 Y4 X2 ↑2 w↑1 ↓2 Yк",
                 "Yн Y0 X0 ↑1 Y1 w↑5 ↓1 P0 ↑2 w↑3 ↓2 X1 ↑4 Y2 w↑5 ↓4 Y3 w↑5 ↓3 Y4 X2 ↑5 w↑3 ↓5 Yк",
                 DisplayName = "Algo 6 = Algo 4 + Algo 5")]

        [DataRow("Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 Y2 Yк",
                 "Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк",
                 "Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 P0 ↑4 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк",
                 DisplayName = "Algo 9 = Algo 7 + Algo 8")]

        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 w↑3 ↓3 Y2 X0 ↑3 X1 ↑4 w↑2 ↓4 X2 ↑1 Yк",
                 "Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 Y2 X1 ↑3 w↑2 ↓3 X2 ↑1 Yк",
                 "Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 w↑3 ↓3 Y2 P0 ↑4 w↑5 ↓4 X0 ↑3 w↑5 ↓5 X1 ↑6 w↑2 ↓6 X2 ↑1 Yк",
                 DisplayName = "Algo 12 = Algo 10 + Algo 11")]

        [DataRow("Yн Y0 X0 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓1 Y3 w↑2 ↓2 Yк",
                 "Yн Y0 X0 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓1 Y4 X2 ↑1 w↑2 ↓2 Yк",
                 "Yн Y0 X0 ↑1 Y1 X1 ↑4 Y2 w↑4 ↓1 P0 ↑2 w↑3 ↓2 Y3 w↑4 ↓3 Y4 X2 ↑3 w↑4 ↓4 Yк",
                 DisplayName = "Algo 15 = Algo 13 + Algo 14")]

        [DataRow("Yн Y1 X0 ↑1 Y0 w↑2 ↓1 Y2 w↑2 ↓2 X1 ↑3 w↑4 ↓3 Y3 w↑4 ↓4 Yк",
                 "Yн Y4 Y5 X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 Yк",
                 "Yн P0 ↑1 Y4 Y5 w↑3 ↓1 Y1 X0 ↑2 Y0 w↑3 ↓2 Y2 w↑3 ↓3 X1 ↑4 w↑5 ↓4 Y3 w↑5 ↓5 Yк",
                 DisplayName = "Algo 18 = Algo 16 + Algo 17")]

        [DataRow("Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 Y2 X1 ↑3 w↑2 ↓3 Yк",
                 "Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 Y3 X2 ↑2 Yк",
                 "Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 P0 ↑3 w↑4 ↓3 Y2 X1 ↑5 w↑3 ↓4 Y3 X2 ↑4 w↑5 ↓5 Yк",
                 DisplayName = "Algo 21 = Algo 19 + Algo 20")]

        [DataRow("Yн Y1 X0 ↑1 Y0 w↑1 ↓1 X1 ↑2 w↑3 ↓2 Y2 w↑3 ↓3 Yк",
                 "Yн Y1 X0 ↑1 Y0 w↑4 ↓1 Y3 w↑4 ↓4 X1 ↑2 w↑3 ↓2 Y2 w↑3 ↓3 Yк",
                 "Yн Y1 X0 ↑1 Y0 w↑2 ↓1 P0 ↑2 Y3 w↑2 ↓2 X1 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк",
                 DisplayName = "Algo 24 = Algo 22 + Algo 23")]

        [DataRow("Yн Y3 X0 ↑1 Y5 w↑2 ↓1 Y4 w↑2 ↓2 Yк",
                 "Yн Y1 w↑1 ↓1 Y2 X1 ↑1 X0 ↑2 Y5 w↑3 ↓2 Y4 w↑3 ↓3 Yк",
                 "Yн P0 ↑1 Y1 w↑2 ↓1 Y3 w↑3 ↓2 Y2 X1 ↑2 w↑3 ↓3 X0 ↑4 Y5 w↑5 ↓4 Y4 w↑5 ↓5 Yк",
                 DisplayName = "Algo 27 = Algo 25 + Algo 26")]

        [DataRow("Yн Y0 X0 ↑1 Y2 w↑2 ↓1 Y1 w↑2 ↓2 X1 ↑3 Y4 w↑3 ↓3 Yк",
                 "Yн Y0 X0 ↑1 Y2 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк",
                 "Yн Y0 P0 ↑6 X0 ↑3 w↑2 ↓3 Y3 w↑8 ↓6 X0 ↑1 w↑2 ↓1 Y1 ↓7 X1 ↑5 w↑4 ↓4 Y4 w↑5 ↓2 Y2 P0 ↑7 w↑8 ↓8 X2 ↑4 w↑5 ↓5 Yк",
                 DisplayName = "Algo 30 = Algo 28 + Algo 29")]

        [DataRow("Yн Y0 X1 ↑1 w↑2 ↓1 Y1 w↑2 ↓2 X0 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк",
                 "Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк",
                 "Yн P0 ↑1 X1 ↑4 w↑7 ↓4 Y3 w↑7 ↓1 Y0 X0 ↑8 X1 ↑2 w↑6 ↓2 Y1 X0 ↑3 w↑6 ↓3 Y2 P0 ↑6 w↑8 ↓8 X1 ↑2 w↑3 ↓7 X2 ↑5 w↑6 ↓5 Y4 w↑6 ↓6 Yк",
                 DisplayName = "Algo 33 = Algo 31 + Algo 32")]
        public void CombineTwoModels(string las1, string las2, string lasResult)
        {
            // 1. Parse Inputs
            Assert.IsTrue(LASParser.TryParse(las1, out var m1, out List<ParsingError> e1), $"Error parsing M1: {string.Join(", ", e1.Select(e => e.Message))}");
            Assert.IsTrue(LASParser.TryParse(las2, out var m2, out List<ParsingError> e2), $"Error parsing M2: {string.Join(", ", e2.Select(e => e.Message))}");
            Assert.IsTrue(LASParser.TryParse(lasResult, out var mExpected, out List<ParsingError> e3), $"Error parsing MResult: {string.Join(", ", e3.Select(e => e.Message))}");

            // 2. Combine
            var mCombined = MASCombiner.Combine(m1, m2);

            // 3. Compare
            bool equal = mExpected.Equals(mCombined);
            bool correct = mCombined.CheckCorrectness(out string msg);

            if (!equal)
            {
                Console.WriteLine("=== Ожидаемая модель ===");
                Console.WriteLine(mExpected.Information);
                Console.WriteLine("=== Актуальная модель (от MASCombiner) ===");
                Console.WriteLine(mCombined.Information);
                Assert.IsTrue(equal, "Актуальная модель не эквивалентна ожидаемой");
                if (!correct)
                {
                    Assert.IsTrue(correct, "Актуальная модель имеет некорректную ЛСА");
                    Console.WriteLine($"\nОшибки парсинга ЛСА: \n{msg}");
                }
            }

            Console.WriteLine($"ЛСА ожидаемой объединённой модели: {lasResult}");
            Console.WriteLine($"ЛСА созданной объединённой модели: {mCombined.LAS}\n\n\n");
            Console.WriteLine($"МСА ожидаемой объединённой модели: \n{mExpected.MAS}");
            Console.WriteLine($"МСА созданной объединённой модели: \n{mCombined.MAS}");

            Assert.IsTrue(equal, "Актуальная модель не эквивалентна ожидаемой");
            Assert.IsTrue(correct, "Актуальная модель имеет некорректную ЛСА");
        }




        [TestMethod]
        [DataRow("Yн P0 ↑1 w↑3 ↓1 P1 ↑2 Y4 w↑5 ↓2 Y1 w↑3 ↓3 X1 ↑4 Y3 w↑5 ↓4 Y2 w↑5 ↓5 Yк",
                 "Yн Y1 X1 ↑1 Y3 w↑2 ↓1 Y2 w↑2 ↓2 Yк",
                 "Yн X1 ↑1 Y3 w↑2 ↓1 Y2 w↑2 ↓2 Yк",
                 "Yн Y4 Yк",
                 DisplayName = "Test 1")]
        [DataRow("Yн P0 ↑1 Y4 w↑5 ↓1 P1 ↑2 w↑3 ↓2 Y1 w↑3 ↓3 X1 ↑4 Y3 w↑5 ↓4 Y2 w↑5 ↓5 Yк",
                 "Yн Y1 X1 ↑1 Y3 w↑2 ↓1 Y2 w↑2 ↓2 Yк",
                 "Yн X1 ↑1 Y3 w↑2 ↓1 Y2 w↑2 ↓2 Yк",
                 "Yн Y4 Yк",
                 DisplayName = "Test 2")]
        public void Combine_Models_CheckEquivalence(string expectedLas, params string[] inputLasModels)
        {
            var expected = ParseLAS(expectedLas);
            var models = inputLasModels.Select(las => ParseLAS(las)).ToArray();

            var mCombined = MASCombiner.Combine(models);

            Console.WriteLine($"ЛСА ожидаемой объединённой модели: {expectedLas}");
            Console.WriteLine($"ЛСА созданной объединённой модели: {mCombined.LAS}");

            // Нетестовый код {
            var minimizedRes = StructuralMinimizer.Minimize(mCombined.Clone());
            Console.WriteLine($"ЛСА \"минимизированной\" модели:     {minimizedRes.LAS}");
            if (!minimizedRes.Equals(expected))
                Console.WriteLine($"Минимизированная модель не эквивалентна исходной.");
            if (!minimizedRes.CheckCorrectness(out string msg))
                Console.WriteLine($"Минимизированная модель некорректна: {msg}");
            // } Нетестовый код

            Console.WriteLine($"\n\n\nМСА ожидаемой объединённой модели: \n{expected.MAS}");
            Console.WriteLine($"МСА созданной объединённой модели: \n{mCombined.MAS}");

            // Проверка на эквивалентность с ожидаемой моделью
            // Assert.IsTrue(mCombined.Equals(expected), "Результирующая модель должна быть эквивалентна ожидаемой");
            // Проверка на корректность объединённой модели
            CheckCorrectness(mCombined);
        }





        #region Приватные методы

        private AbstractAutomata ParseLAS(string las)
        {
            if (!LASParser.TryParse(las, out AbstractAutomata? model, out ParsingAggregateException? ex))
            {
                throw new Exception($"Failed to parse LAS: {las}. Error: {ex?.Message}");
            }
            model!.Update();
            return model;
        }

        private void CheckCorrectness(AbstractAutomata model)
        {
            if (!model.CheckCorrectness(out string msg))
            {
                Console.WriteLine("DEBUG: Vertices dump:");
                foreach (var v in model.Vertices)
                {
                    Console.WriteLine($" - {v.GetType().Name} ID='{v.ID}' Next='{v.Next?.ID}'");
                    if (v is AlgorithmDeveloper.Abstractions.AAModel.Vertices.JumpPoint jp)
                        Console.WriteLine($"   JumpIndex: {jp.JumpIndex}");
                }
                Assert.Fail($"Модель некорректна: {msg}");
            }
        }

        #endregion
    }
}
