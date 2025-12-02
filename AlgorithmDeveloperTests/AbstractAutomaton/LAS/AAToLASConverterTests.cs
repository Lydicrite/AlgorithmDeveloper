using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgorithmDeveloper.AAModel;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace AlgorithmDeveloper.AAModel.LAS.Tests
{
    [TestClass]
    public class AAToLASConverterTests
    {
        private static Type ParserType => typeof(AbstractAutomaton).Assembly.GetType("AlgorithmDeveloper.AAModel.LAS.LASParser", throwOnError: true)!;

        #region Тесты

        [TestMethod]
        [DataRow("Yн X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк",
            DisplayName = "(TestGroup A)\tAlgo 1")]
        [DataRow("Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк",
            DisplayName = "(TestGroup A)\tAlgo 2")]
        [DataRow("Yн P0 ↑1 X1 ↑3 w↑4 ↓3 Y3 w↑4 ↓1 X0 ↑2 Y0 w↑4 ↓2 Y1 w↑4 ↓4 X2 ↑5 w↑6 ↓5 Y2 w↑4 ↓6 Yк",
            DisplayName = "(TestGroup A)\tAlgo 3 = Algo 1 + Algo 2\t(Макс)")]


        [DataRow("Yн Y0 X0 ↑1 Y1 w↑3 ↓1 X1 ↑2 Y2 w↑3 ↓2 Y3 w↑3 ↓3 Yк",
            DisplayName = "(TestGroup B)\tAlgo 4")]
        [DataRow("Yн Y0 X0 ↑1 Y1 w↑2 ↓1 Y4 X2 ↑2 w↑1 ↓2 Yк",
            DisplayName = "(TestGroup B)\tAlgo 5")]
        [DataRow("Yн Y0 X0 ↑1 Y1 w↑5 ↓1 P0 ↑2 w↑3 ↓2 X1 ↑4 Y2 w↑5 ↓4 Y3 w↑5 ↓3 Y4 X2 ↑5 w↑3 ↓5 Yк",
            DisplayName = "(TestGroup B)\tAlgo 6 = Algo 4 + Algo 5\t(Ваня Г.)")]


        [DataRow("Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 Y2 Yк",
            DisplayName = "(TestGroup C)\tAlgo 7")]
        [DataRow("Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк",
            DisplayName = "(TestGroup C)\tAlgo 8")]
        [DataRow("Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 P0 ↑4 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк",
            DisplayName = "(TestGroup C)\tAlgo 9 = Algo 7 + Algo 8\t(Андрей)")]


        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 w↑3 ↓3 Y2 X0 ↑3 X1 ↑4 w↑2 ↓4 X2 ↑1 Yк",
            DisplayName = "(TestGroup D)\tAlgo 10")]
        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 Y2 X1 ↑3 w↑2 ↓3 X2 ↑1 Yк",
            DisplayName = "(TestGroup D)\tAlgo 11")]
        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 w↑3 ↓3 Y2 P0 ↑4 w↑5 ↓4 X0 ↑3 w↑5 ↓5 X1 ↑6 w↑2 ↓6 X2 ↑1 Yк",
            DisplayName = "(TestGroup D)\tAlgo 12 = Algo 10 + Algo 11\t(Ваня Ш.)")]


        [DataRow("Yн Y0 X0 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓1 Y3 w↑2 ↓2 Yк",
            DisplayName = "(TestGroup E)\tAlgo 13")]
        [DataRow("Yн Y0 X0 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓1 Y4 X2 ↑1 w↑2 ↓2 Yк",
            DisplayName = "(TestGroup E)\tAlgo 14")]
        [DataRow("Yн Y0 X0 ↑1 Y1 X1 ↑4 Y2 w↑4 ↓1 P0 ↑2 w↑3 ↓2 Y3 w↑4 ↓3 Y4 X2 ↑3 w↑4 ↓4 Yк",
            DisplayName = "(TestGroup E)\tAlgo 15 = Algo 13 + Algo 14\t(Ваня Л.)")]


        [DataRow("Yн Y1 X0 ↑1 Y0 w↑2 ↓1 Y2 w↑2 ↓2 X1 ↑3 w↑4 ↓3 Y3 w↑4 ↓4 Yк",
            DisplayName = "(TestGroup F)\tAlgo 16")]
        [DataRow("Yн Y4 Y5 X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 Yк",
            DisplayName = "(TestGroup F)\tAlgo 17")]
        [DataRow("Yн P0 ↑1 Y4 Y5 w↑3 ↓1 Y1 X0 ↑2 Y0 w↑3 ↓2 Y2 w↑3 ↓3 X1 ↑4 w↑5 ↓4 Y3 w↑5 ↓5 Yк",
            DisplayName = "(TestGroup F)\tAlgo 18 = Algo 16 + Algo 17\t(Саня Ш.)")]


        [DataRow("Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 Y2 X1 ↑3 w↑2 ↓3 Yк",
            DisplayName = "(TestGroup G)\tAlgo 19")]
        [DataRow("Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 Y3 X2 ↑2 Yк",
            DisplayName = "(TestGroup G)\tAlgo 20")]
        [DataRow("Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 P0 ↑3 w↑4 ↓3 Y2 X1 ↑5 w↑3 ↓4 Y3 X2 ↑4 w↑5 ↓5 Yк",
            DisplayName = "(TestGroup G)\tAlgo 21 = Algo 19 + Algo 20\t(Илья)")]


        [DataRow("Yн Y1 X0 ↑1 Y0 w↑1 ↓1 X1 ↑2 w↑3 ↓2 Y2 w↑3 ↓3 Yк",
            DisplayName = "(TestGroup H)\tAlgo 22")]
        [DataRow("Yн Y1 X0 ↑1 Y0 w↑4 ↓1 Y3 w↑4 ↓4 X1 ↑2 w↑3 ↓2 Y2 w↑3 ↓3 Yк",
            DisplayName = "(TestGroup H)\tAlgo 23")]
        [DataRow("Yн Y1 X0 ↑1 Y0 w↑2 ↓1 P0 ↑2 Y3 w↑2 ↓2 X1 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк",
            DisplayName = "(TestGroup H)\tAlgo 24 = Algo 22 + Algo 23\t(Саня С.)")]


        [DataRow("Yн Y3 X0 ↑1 Y5 w↑2 ↓1 Y4 w↑2 ↓2 Yк",
            DisplayName = "(TestGroup J)\tAlgo 25")]
        [DataRow("Yн Y1 w↑1 ↓1 Y2 X1 ↑1 X0 ↑2 Y5 w↑3 ↓2 Y4 w↑3 ↓3 Yк",
            DisplayName = "(TestGroup J)\tAlgo 26")]
        [DataRow("Yн P0 ↑1 Y1 w↑2 ↓1 Y3 w↑3 ↓2 Y2 X1 ↑2 w↑3 ↓3 X0 ↑4 Y5 w↑5 ↓4 Y4 w↑5 ↓5 Yк",
            DisplayName = "(TestGroup J)\tAlgo 27 = Algo 25 + Algo 26\t(Артемий)")]


        [DataRow("Yн Y0 X0 ↑1 Y2 w↑2 ↓1 Y1 w↑2 ↓2 X1 ↑3 Y4 w↑3 ↓3 Yк",
            DisplayName = "(TestGroup J)\tAlgo 28")]
        [DataRow("Yн Y0 X0 ↑1 Y2 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк",
            DisplayName = "(TestGroup J)\tAlgo 29")]
        [DataRow("Yн Y0 X0 ↑1 Y2 w↑9 ( ↓1 P0 ↑2 Y3 w↑5 ) ( ↓2 Y1 w↑4 ) ( ↓9 P1 ↑4 w↑5 ) ( ↓4 X1 ↑6 w↑3 ) ( ↓5 X2 ↑3 w↑6 ) ( ↓3 Y4 w↑6 ) ↓6 Yк",
            DisplayName = "(TestGroup J)\tAlgo 30 = Algo 28 + Algo 29\t(Стёпа)")]


        [DataRow("Yн Y0 X1 ↑1 w↑2 ↓1 Y1 w↑2 ↓2 X0 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк",
            DisplayName = "(TestGroup J)\tAlgo 31")]
        [DataRow("Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк",
            DisplayName = "(TestGroup J)\tAlgo 32")]
        [DataRow("Yн P0 ↑1 Y0 w↑1 ↓1 X1 ↑2 w↑4 ↓2 P1 ↑3 Y3 w↑4 ↓3 Y1 w↑4 ↓4 P2 ↑5 X2 ↑7 w↑8 ↓5 X0 ↑6 w↑8 ↓6 Y2 w↑8 ↓7 Y4 w↑8 ↓8 Yк",
            DisplayName = "(TestGroup J)\tAlgo 33 = Algo 31 + Algo 32\t(Миша)")]



        [DataRow("Yн Yк",
            DisplayName = "(TestGroup X)\tAlgo 88")]
        [DataRow("Yн Y1 P1 ↑1 Y2 w↑1 ↓1 X2 ↑2 Y3 P3 ↑3 Y4 w↑3 ↓3 X4 ↑4 w↑1 ↓4 P5 ↑5 w↑1 ↓5 Y5 w↑2 ↓2 Yк",
            DisplayName = "(TestGroup X)\tAlgo 89")]
        [DataRow("Yн Y1 X1 ↑1 Y2 w↑1 ↓1 P2 ↑2 X3 ↑3 w↑1 ↓3 X4 ↑4 w↑1 ↓4 Y3 w↑2 ↓2 Yк",
            DisplayName = "(TestGroup X)\tAlgo 90")]
        [DataRow("Yн X1 ↑1 Y1 w↑1 ↓1 Y2 P2 ↑2 X3 ↑3 w↑1 ↓3 P4 ↑4 w↑1 ↓4 Y3 Y4 w↑2 ↓2 Yк",
            DisplayName = "(TestGroup X)\tAlgo 91")]


        [DataRow("Yн X1 ↑1 X2 ↑2 w↑2 ↓2 P3 ↑3 w↑2 ↓3 Y1 w↑1 ↓1 Yк",
            DisplayName = "(TestGroup Y)\tAlgo 92")]
        [DataRow("Yн X1 ↑1 X2 ↑2 Y10 w↑2 ↓2 P3 ↑3 w↑2 ↓3 Y1 w↑1 ↓1 Yк",
            DisplayName = "(TestGroup Y)\tAlgo 92 - RBS-ветвь X2 содержит Y10")]
            [DataRow("Yн X1 ↑1 X2 ↑2 Y10 Y11 w↑2 ↓2 P3 ↑3 w↑2 ↓3 Y1 w↑1 ↓1 Yк",
            DisplayName = "(TestGroup Y)\tAlgo 92 - RBS-ветвь X2 содержит Y10 и Y11")]
        [DataRow("Yн Y1 Y2 X1 ↑1 X2 ↑2 w↑2 ↓2 P3 ↑3 w↑2 ↓3 Y3 w↑1 ↓1 Yк",
            DisplayName = "(TestGroup Y)\tAlgo 93")]
        [DataRow("Yн Y1 Y2 X1 ↑1 X2 ↑2 Y10 w↑2 ↓2 P3 ↑3 w↑2 ↓3 Y3 w↑1 ↓1 Yк",
            DisplayName = "(TestGroup Y)\tAlgo 93 - RBS-ветвь X2 содержит Y10")]
        [DataRow("Yн Y1 Y2 X1 ↑1 X2 ↑2 Y10 Y11 w↑2 ↓2 P3 ↑3 w↑2 ↓3 Y3 w↑1 ↓1 Yк",
            DisplayName = "(TestGroup Y)\tAlgo 93 - RBS-ветвь X2 содержит Y10 и Y11")]

        [DataRow("Yн X1 ↑1 Y1 Y2 P2 ↑2 Y3 w↑2 ↓2 Y4 w↑1 ↓1 Y5 Yк",
            DisplayName = "(TestGroup Z)\tAlgo 94")]
        [DataRow("Yн Y0 P0 ↑1 P1 ↑1 X2 ↑2 Y1 w↑2 ↓2 Y2 Y3 Y4 P3 ↑3 Y5 w↑3 ↓3 Y6 w↑1 ↓1 Y7 Y8 w↑0 ↓0 Y9 Yк",
            DisplayName = "(TestGroup Z)\tAlgo 95")]
        [DataRow("Yн Y0 Y1 Y2 X0 ↑0 Y3 P1 ↑1 Y4 w↑1 ↓1 Y5 P2 ↑3 Y6 Y7 w↑3 ↓3 Y8 w↑0 ↓0 Y9 Yк",
            DisplayName = "(TestGroup Z)\tAlgo 96")]
        [DataRow("Yн X0 ↑1 Y0 w↑2 ↓1 Y1 X1 ↑3 w↑2 ↓2 Y2 w↑3 ↓3 X2 ↑2 Yк",
            DisplayName = "(TestGroup Z)\tAlgo 97")]
        [DataRow("Yн X0 ↑1 Y1 X1 ↑2 w↑3 ↓1 Y0 w↑2 ↓2 Y2 w↑3 ↓3 X2 ↑4 w↑2 ↓4 Yк",
            DisplayName = "(TestGroup Z)\tAlgo 97 Rev")]
        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 X0 ↑2 X1 ↑3 Y2 w↑4 ↓3 X2 ↑4 w↑1 ↓4 Y3 Yк",
            DisplayName = "(TestGroup Z)\tAlgo 98")]
        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 X0 ↑1 X1 ↑3 w↑2 ↓3 X2 ↑4 Y1 w↑5 ↓4 Y2 w↑5 ↓5 Y3 Yк",
            DisplayName = "(TestGroup Z)\tAlgo 99")]
        public void SimpleModels(string las)
        {
            AssertEquivalentLAS(las);
        }

        [TestMethod]
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
        public void ComplexModels(string las)
        {
            AssertEquivalentLAS(las);
        }

        #endregion





        #region Вспомогательные методы

        private static AbstractAutomaton ParseLAS(string las)
        {
            var parse = ParserType.GetMethod("Parse", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!;
            return (AbstractAutomaton)parse.Invoke(null, new object[] { las })!;
        }

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

        private static void AssertEquivalentLAS(string las1)
        {
            // Парсим ЛСА 1
            if (!TryParseLAS(las1, out var model1, out var errors1))
                Assert.Inconclusive
                 (
                     $"Исходная ЛСА (ЛСА 1) не проходит парсинг." +
                     $"\nЛСА 1: {las1}" +
                     $"\nОшибки:\n{errors1}"
                 );

            // Генерируем ЛСА 2 из модели 1
            var las2 = AAToLASConverter.Convert(model1!, out var lasLog, enableLogging: true);

            // Если ЛСА совпадают, то модели точно эквивалентны
            if (string.Equals(las1, las2, StringComparison.Ordinal))
            {
                // Логи теста
                Console.WriteLine
                (
                    $"Исходная и сгенерированная ЛСА эквивалентны.\nЛСА: {las1}" +
                    $"\n\nИнформация об алгоритме:\n{model1!.Information}"
                );
                return;
            }

            bool errorsWhileParseLAS2 = false;
            // Иначе парсим ЛСА 2 и сравниваем модели
            if (!TryParseLAS(las2, out var model2, out var errors2))
            {
                errorsWhileParseLAS2 = true;
                Assert.Fail
                    (
                        $"Сгенерированная ЛСА (ЛСА 2) не проходит парсинг." +
                        $"\nЛСА 2: {las2}" +
                        $"\nОшибки:\n{errors2}"
                    );
            }

            // Логи теста
            Console.WriteLine
            (
                $"\nЛСА 1: {las1}" +
                $"\nЛСА 2: {las2}" +
                $"\n\nИнформация об алгоритме 1:\n{model1!.Information}" +
                $"\n\nИнформация об алгоритме 2:\n{model2!.Information}"
            );

            bool equivalent = model1!.Equals(model2!);
            if (!equivalent || errorsWhileParseLAS2)
            {
                Console.WriteLine($"\n\n=== ЛОГ ГЕНЕРАТОРА ЛСА ===");
                Console.WriteLine(lasLog);
                Console.WriteLine($"=== КОНЕЦ ЛОГА ===");
            }
            Assert.IsTrue(equivalent, "ЛСА не совпадают и модели не эквивалентны.");
        }

        #endregion
    }
}