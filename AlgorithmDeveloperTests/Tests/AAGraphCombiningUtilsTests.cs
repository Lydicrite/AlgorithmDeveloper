using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Utils;
using AlgorithmDeveloper.Abstractions.LAS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace AlgorithmDeveloperTests.Tests
{
    [TestClass()]
    public class AAGraphCombiningUtilsTests
    {
        private static bool TryParseLAS(string las, out AbstractAutomata? model, out string errorsText)
        {
            model = null;
            errorsText = string.Empty;

            bool ok = LASParser.TryParse(las, out model, out ParsingAggregateException? ex);
            errorsText = ex?.Message ?? string.Empty;

            return ok;
        }

       
        [TestMethod]
        [DataRow(
            "Yн Y0 X1 ↑1 w↑2 ↓1 Y1 w↑2 ↓2 X0 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк",
            "Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк",
            DisplayName = "TestGroup 1"
        )]
        public void BuildCommonSubgraphsReport(string lasA, string lasB)
        {
            if (!TryParseLAS(lasA, out var aa1, out var errA))
                Assert.Inconclusive($"ЛСА A не проходит парсинг.\nЛСА A: {lasA}\nОшибки:\n{errA}");
            if (!TryParseLAS(lasB, out var aa2, out var errB))
                Assert.Inconclusive($"ЛСА B не проходит парсинг.\nЛСА B: {lasB}\nОшибки:\n{errB}");

            var report = AAGraphCombiningUtils.BuildCommonSubgraphsReport(aa1!, aa2!, strictLabels: false, resultLimit: 5);
            Console.WriteLine(report);

            Assert.IsFalse(string.IsNullOrWhiteSpace(report), "Пустой отчёт по общим подграфам");
            Assert.IsTrue(report.Contains(aa1!.InitialLAS, StringComparison.Ordinal), "Отчёт не содержит ЛСА A");
            Assert.IsTrue(report.Contains(aa2!.InitialLAS, StringComparison.Ordinal), "Отчёт не содержит ЛСА B");
        }
    }
}