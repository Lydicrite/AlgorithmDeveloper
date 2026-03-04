using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;
using AlgorithmDeveloper.Abstractions.LAS;
using AlgorithmDeveloper.Abstractions.TransitionSystem.MAS;

namespace AlgorithmDeveloperTests.Tests
{
    [TestClass]
    public class DiagnosticsTests
    {
        [TestMethod]
        [DataRow("Yн Y1 P1 ↑1 Y2 w↑1 ↓1 X2 ↑2 Y3 P3 ↑3 Y4 w↑3 ↓3 X4 ↑4 w↑1 ↓4 P5 ↑5 w↑1 ↓5 Y5 w↑2 ↓2 Yк", DisplayName = "(TestGroup X)\tAlgo 89")]
        public void RunSpecificTests(string las)
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

            Console.WriteLine($"Original LAS: {las}");
            Console.WriteLine($"Generated LAS: {model2.LAS}");
            Console.WriteLine($"Generated MAS:\n{masString}");

            // 4. Check Equality
            if (!model1.Equals(model2))
            {
                Assert.Fail("Модели не эквивалентны.");
            }
            Assert.IsTrue(correct, $"Модель 2 имеет некорректную ЛСА: {msg}");
        }
    }
}
