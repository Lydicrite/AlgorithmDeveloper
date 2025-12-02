using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgorithmDeveloper.AAModel;
using System;
using System.Linq;
using System.Reflection;
using AlgorithmDeveloper.AAModel.Model.Utils;

namespace AlgorithmDeveloper.AAModel.Model.Tests
{
    [TestClass()]
    public class AAGraphCombiningUtilsTests
    {
        private static Type ParserType => typeof(AbstractAutomaton).Assembly.GetType("AlgorithmDeveloper.AAModel.LAS.LASParser", throwOnError: true)!;

        private static bool TryParseLAS(string las, out AbstractAutomaton? model, out string errorsText)
        {
            model = null;
            errorsText = string.Empty;
            var methods = ParserType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.Name == "TryParse");

            var exType = typeof(AbstractAutomaton).Assembly.GetType("AlgorithmDeveloper.AAModel.LAS.ParsingAggregateException", throwOnError: true)!;
            var target = methods.First(m =>
            {
                var p = m.GetParameters();
                return p.Length == 3 && p[0].ParameterType == typeof(string)
                       && p[1].IsOut && p[1].ParameterType == typeof(AbstractAutomaton).MakeByRefType()
                       && p[2].IsOut && p[2].ParameterType == exType.MakeByRefType();
            });

            object?[] args = new object?[] { las, null, null };
            var ok = (bool)target.Invoke(null, args)!;
            model = (AbstractAutomaton?)args[1];
            var ex = args[2];
            if (ex != null)
            {
                var msgProp = ex.GetType().GetProperty("Message")?.GetValue(ex)?.ToString();
                errorsText = msgProp ?? string.Empty;
            }
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