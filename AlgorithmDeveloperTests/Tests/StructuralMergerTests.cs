using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Utils;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.Combining;
using AlgorithmDeveloper.Abstractions.LAS;
using AlgorithmDeveloper.Abstractions.Minimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AlgorithmDeveloperTests.Tests
{
    [TestClass]
    public class StructuralMergerTests
    {
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
                foreach(var v in model.Vertices)
                {
                    Console.WriteLine($" - {v.GetType().Name} ID='{v.ID}' Next='{v.Next?.ID}'");
                    if (v is AlgorithmDeveloper.Abstractions.AAModel.Vertices.JumpPoint jp)
                        Console.WriteLine($"   JumpIndex: {jp.JumpIndex}");
                }
                Assert.Fail($"Модель некорректна: {msg}");
            }
        }

        [TestMethod]
        public void Combine_IdenticalModels_ReturnsSameStructure()
        {
            // AA1: Start -> Y1 -> Y2 -> End
            string las = "Yн Y1 Y2 Yк";
            var m1 = ParseLAS(las);
            var m2 = ParseLAS(las);

            var result = StructuralMerger.Combine(m1, m2);

            Console.WriteLine("ЛСА созданной объединённой модели: " + result.LAS);
            Console.WriteLine(result.BuildAlgorithmInfo());

            // Expect: Start -> Y1 -> Y2 -> End
            // No conditionals.
            // Vertices: Start, Y1, Y2, End + JumpPoint(End). Count = 5.
            
            Assert.HasCount(5, result.Vertices);
            Assert.IsTrue(result.Vertices.Any(v => v is StartVertex));
            Assert.IsTrue(result.Vertices.Any(v => v is EndVertex));
            Assert.IsTrue(result.Vertices.Any(v => v.ID == "Y1"));
            Assert.IsTrue(result.Vertices.Any(v => v.ID == "Y2"));
            
            // Проверка корректности
            CheckCorrectness(result);
            
            // Проверка эквивалентности (хотя бы по структуре с одной из исходных, т.к. они идентичны)
            Assert.IsTrue(result.Equals(m1), "Результат должен быть эквивалентен исходной модели");
        }

        [TestMethod]
        public void Combine_SimpleDivergence_AddsConditional()
        {
            // AA1: Y1
            // AA2: Y2
            var m1 = ParseLAS("Yн Y1 Yк");
            var m2 = ParseLAS("Yн Y2 Yк");

            var result = StructuralMerger.Combine(m1, m2);
            Console.WriteLine("ЛСА созданной объединённой модели: " + result.LAS);

            // Expect: Start -> P0? -> (Y1 | Y2) -> End
            // Check if we have a conditional starting with P
            
            var cond = result.Vertices.OfType<ConditionalVertex>().FirstOrDefault(v => v.ID.StartsWith("P"));
            Assert.IsNotNull(cond, "Should have a P-conditional vertex");
            
            // Verify execution
            result.SetConditionalValue(cond, false);
            var (path0, _) = result.SimulateRunForCurrentConditions();
            Assert.Contains("Y1", path0, "P=0 should lead to Y1");
            Assert.DoesNotContain("Y2", path0, "P=0 should NOT lead to Y2");

            result.SetConditionalValue(cond, true);
            var (path1, _) = result.SimulateRunForCurrentConditions();
            Assert.Contains("Y2", path1, "P=1 should lead to Y2");
            Assert.DoesNotContain("Y1", path1, "P=1 should NOT lead to Y1");
            
            CheckCorrectness(result);
        }

        [TestMethod]
        public void Combine_SuffixMerging_MergesCommonTail()
        {
            // AA1: Y1 -> Y3
            // AA2: Y2 -> Y3
            var m1 = ParseLAS("Yн Y1 Y3 Yк");
            var m2 = ParseLAS("Yн Y2 Y3 Yк");

            var result = StructuralMerger.Combine(m1, m2);
            Console.WriteLine("ЛСА созданной объединённой модели: " + result.LAS);

            // There should be exactly one Y3
            var y3Count = result.Vertices.Count(v => v.ID == "Y3");
            Assert.AreEqual(1, y3Count, "Should have exactly one Y3 (merged)");
            
            CheckCorrectness(result);
        }

        [TestMethod]
        public void Combine_ThreeModels_WithGrouping()
        {
            var m1 = ParseLAS("Yн Y1 Yк");
            var m2 = ParseLAS("Yн Y2 Yк");
            var m3 = ParseLAS("Yн Y1 Yк");

            var result = StructuralMerger.Combine(m1, m2, m3);
            Console.WriteLine("Combined LAS: " + result.LAS);

            // Should reuse Y1 for model 0 and 2.
            var y1Count = result.Vertices.Count(v => v.ID == "Y1");
            Assert.AreEqual(1, y1Count, "Should have merged Y1 for models 0 and 2");
            
            CheckCorrectness(result);
        }

        [TestMethod]
        public void Combine_MergePoint_InsertsJumpPoint()
        {
            // M1: Y1 -> Y3
            // M2: Y2 -> Y3
            // Y3 - это точка слияния (In-Degree = 2).
            
            var m1 = ParseLAS("Yн Y1 Y3 Yк");
            var m2 = ParseLAS("Yн Y2 Y3 Yк");

            var result = StructuralMerger.Combine(m1, m2);
            Console.WriteLine("Combined LAS: " + result.LAS);

            // Находим Y3
            var y3 = result.Vertices.FirstOrDefault(v => v.ID == "Y3");
            Assert.IsNotNull(y3, "Y3 должна существовать");

            var predecessors = result.Vertices.Where(v => v.Next == y3 || (v is ConditionalVertex cv && (cv.LBS == y3 || cv.RBS == y3))).ToList();
            
            // С использованием InsertJumpPoints должен быть РОВНО ОДИН предшественник, и это должен быть JumpPoint.
            Assert.HasCount(1, predecessors, "Y3 должна иметь ровно одного предшественника (JumpPoint)");
            Assert.IsInstanceOfType(predecessors[0], typeof(JumpPoint), "Предшественник Y3 должен быть JumpPoint");
            
            CheckCorrectness(result);
        }

        [TestMethod]
        public void Combine_Loop_InsertsJumpPoint()
        {
            // M1: Цикл Y1 -> Y2 -> Y1
            // Вручную строим граф
            var m1 = new AbstractAutomata();
            var start = new StartVertex();
            var y1 = new OperatorVertex(1);
            var y2 = new OperatorVertex(2);
            var jp = new JumpPoint(1);
            var end = new EndVertex();

            m1.AddVertex(start);
            m1.AddVertex(y1);
            m1.AddVertex(y2);
            m1.AddVertex(jp);
            m1.AddVertex(end);

            m1.LinkNext(start, jp);
            m1.LinkNext(jp, y1);
            m1.LinkNext(y1, y2);
            m1.LinkNext(y2, jp);
            
            m1.Update();
            
            var result = StructuralMerger.Combine(m1);
            Console.WriteLine("Combined LAS: " + result.LAS);

            var resY1 = result.Vertices.FirstOrDefault(v => v.ID == "Y1");
            Assert.IsNotNull(resY1, "Y1 должна существовать");
            
            var preds = result.Vertices.Where(v => v.Next == resY1 || (v is ConditionalVertex cv && (cv.LBS == resY1 || cv.RBS == resY1))).ToList();
            
            Assert.HasCount(1, preds, "Y1 должна иметь ровно одного предшественника (JumpPoint)");
            Assert.IsInstanceOfType(preds[0], typeof(JumpPoint));
            
            // CheckCorrectness(result); // Исключаем проверку, так как бесконечный цикл делает Yк недостижимой
        }

        [TestMethod]
        public void Combine_ConditionalFalseBranch_InsertsJumpPoint()
        {
            var m1 = ParseLAS("Yн X1 ↑1 Y1 ↓1 Y2 Yк");
            
            var result = StructuralMerger.Combine(m1);
            Console.WriteLine("Combined LAS: " + result.LAS);
            
            var y2 = result.Vertices.First(v => v.ID == "Y2");
            var preds = result.Vertices.Where(v => v.Next == y2 || (v is ConditionalVertex cv && (cv.LBS == y2 || cv.RBS == y2))).ToList();
            
            Assert.HasCount(1, preds, "Y2 (цель LBS) должна иметь ровно одного предшественника (JumpPoint)");
            Assert.IsInstanceOfType(preds[0], typeof(JumpPoint));
            
            CheckCorrectness(result);
        }
        
        [TestMethod]
        [DataRow(
            "Yн Y0 X1 ↑1 w↑2 ↓1 Y1 w↑2 ↓2 X0 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк",
            "Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк",
            DisplayName = "TestGroup 1"
        )]
        public void BuildCommonSubgraphsReport(string lasA, string lasB)
        {
            var aa1 = ParseLAS(lasA);
            var aa2 = ParseLAS(lasB);

            var report = AAGraphCombiningUtils.BuildCommonSubgraphsReport(aa1!, aa2!, strictLabels: false, resultLimit: 5);
            Console.WriteLine(report);

            Assert.IsFalse(string.IsNullOrWhiteSpace(report), "Пустой отчёт по общим подграфам");
            Assert.IsTrue(report.Contains(aa1!.LAS, StringComparison.Ordinal), "Отчёт не содержит ЛСА A");
            Assert.IsTrue(report.Contains(aa2!.LAS, StringComparison.Ordinal), "Отчёт не содержит ЛСА B");
        }





        [TestMethod]
        [DataRow("Yн P0 ↑1 X1 ↑3 w↑4 ↓3 Y3 w↑4 ↓1 X0 ↑2 Y0 w↑4 ↓2 Y1 w↑4 ↓4 X2 ↑5 w↑6 ↓5 Y2 w↑4 ↓6 Yк",
                 "Yн X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк",
                 "Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк",
                 DisplayName = "Algo 3 = Algo 1 + Algo 2")]

        [DataRow("Yн Y0 X0 ↑1 Y1 w↑5 ↓1 P0 ↑2 w↑3 ↓2 X1 ↑4 Y2 w↑5 ↓4 Y3 w↑5 ↓3 Y4 X2 ↑5 w↑3 ↓5 Yк",
                 "Yн Y0 X0 ↑1 Y1 w↑3 ↓1 X1 ↑2 Y2 w↑3 ↓2 Y3 w↑3 ↓3 Yк",
                 "Yн Y0 X0 ↑1 Y1 w↑2 ↓1 Y4 X2 ↑2 w↑1 ↓2 Yк",
                 DisplayName = "Algo 6 = Algo 4 + Algo 5")]

        [DataRow("Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 P0 ↑4 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк",
                 "Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 Y2 Yк",
                 "Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк",
                 DisplayName = "Algo 9 = Algo 7 + Algo 8")]

        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 w↑3 ↓3 Y2 P0 ↑4 w↑5 ↓4 X0 ↑3 w↑5 ↓5 X1 ↑6 w↑2 ↓6 X2 ↑1 Yк",
                 "Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 w↑3 ↓3 Y2 X0 ↑3 X1 ↑4 w↑2 ↓4 X2 ↑1 Yк",
                 "Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 Y2 X1 ↑3 w↑2 ↓3 X2 ↑1 Yк",
                 DisplayName = "Algo 12 = Algo 10 + Algo 11")]

        [DataRow("Yн Y0 X0 ↑1 Y1 X1 ↑4 Y2 w↑4 ↓1 P0 ↑2 w↑3 ↓2 Y3 w↑4 ↓3 Y4 X2 ↑3 w↑4 ↓4 Yк",
                 "Yн Y0 X0 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓1 Y3 w↑2 ↓2 Yк",
                 "Yн Y0 X0 ↑1 Y1 X1 ↑2 Y2 w↑2 ↓1 Y4 X2 ↑1 w↑2 ↓2 Yк",
                 DisplayName = "Algo 15 = Algo 13 + Algo 14")]

        [DataRow("Yн P0 ↑1 Y4 Y5 w↑3 ↓1 Y1 X0 ↑2 Y0 w↑3 ↓2 Y2 w↑3 ↓3 X1 ↑4 w↑5 ↓4 Y3 w↑5 ↓5 Yк",
                 "Yн Y1 X0 ↑1 Y0 w↑2 ↓1 Y2 w↑2 ↓2 X1 ↑3 w↑4 ↓3 Y3 w↑4 ↓4 Yк",
                 "Yн Y4 Y5 X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 Yк",
                 DisplayName = "Algo 18 = Algo 16 + Algo 17")]

        [DataRow("Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 P0 ↑3 w↑4 ↓3 Y2 X1 ↑5 w↑3 ↓4 Y3 X2 ↑4 w↑5 ↓5 Yк",
                 "Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 Y2 X1 ↑3 w↑2 ↓3 Yк",
                 "Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 Y3 X2 ↑2 Yк",
                 DisplayName = "Algo 21 = Algo 19 + Algo 20")]

        [DataRow("Yн Y1 X0 ↑1 Y0 w↑2 ↓1 P0 ↑2 Y3 w↑2 ↓2 X1 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк",
                 "Yн Y1 X0 ↑1 Y0 w↑1 ↓1 X1 ↑2 w↑3 ↓2 Y2 w↑3 ↓3 Yк",
                 "Yн Y1 X0 ↑1 Y0 w↑4 ↓1 Y3 w↑4 ↓4 X1 ↑2 w↑3 ↓2 Y2 w↑3 ↓3 Yк",
                 DisplayName = "Algo 24 = Algo 22 + Algo 23")]

        [DataRow("Yн P0 ↑1 Y1 w↑2 ↓1 Y3 w↑3 ↓2 Y2 X1 ↑2 w↑3 ↓3 X0 ↑4 Y5 w↑5 ↓4 Y4 w↑5 ↓5 Yк",
                 "Yн Y3 X0 ↑1 Y5 w↑2 ↓1 Y4 w↑2 ↓2 Yк",
                 "Yн Y1 w↑1 ↓1 Y2 X1 ↑1 X0 ↑2 Y5 w↑3 ↓2 Y4 w↑3 ↓3 Yк",
                 DisplayName = "Algo 27 = Algo 25 + Algo 26")]

        [DataRow("Yн Y0 X0 ↑1 Y2 w↑9 ↓1 P0 ↑2 Y3 w↑5 ↓2 Y1 w↑4 ↓9 P1 ↑4 w↑5 ↓4 X1 ↑6 w↑3 ↓5 X2 ↑3 w↑6 ↓3 Y4 w↑6 ↓6 Yк",
                 "Yн Y0 X0 ↑1 Y2 w↑2 ↓1 Y1 w↑2 ↓2 X1 ↑3 Y4 w↑3 ↓3 Yк",
                 "Yн Y0 X0 ↑1 Y2 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк",
                 DisplayName = "Algo 30 = Algo 28 + Algo 29")]

        [DataRow("Yн P0 ↑1 w↑2 ↓1 Y0 w↑2 ↓2 X1 ↑3 w↑5 ↓3 P1 ↑4 Y3 w↑8 ↓4 Y1 w↑6 ↓6 X0 ↑7 w↑10 ↓7 Y2 w↑10 ↓5 P2 ↑6 w↑8 ↓8 X2 ↑9 w↑10 ↓9 Y4 w↑10 ↓10 Yк",
                 "Yн Y0 X1 ↑1 w↑2 ↓1 Y1 w↑2 ↓2 X0 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк",
                 "Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y4 w↑4 ↓4 Yк",
                 DisplayName = "Algo 33 = Algo 31 + Algo 32")]

        /*
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
        */

        public void Combine_Models_CheckEquivalence(string expectedLas, params string[] inputLasModels)
        {
            var expected = ParseLAS(expectedLas);
            var models = inputLasModels.Select(las => ParseLAS(las)).ToArray();

            var result = StructuralMerger.Combine(models);
            Console.WriteLine($"ЛСА ожидаемой объединённой модели: {expectedLas}");
            Console.WriteLine($"ЛСА созданной объединённой модели: {result.LAS}");

            // Нетестовый код {
            var minimizedRes = StructuralMinimizer.Minimize(result.Clone());
            Console.WriteLine($"ЛСА \"минимизированной\" модели:     {minimizedRes.LAS}");
            if (!minimizedRes.Equals(expected))
                Console.WriteLine($"Минимизированная модель не эквивалентна исходной.");
            if (!minimizedRes.CheckCorrectness(out string msg))
                Console.WriteLine($"Минимизированная модель некорректна: {msg}");
            // } Нетестовый код


            // Проверка на корректность объединённой модели
            CheckCorrectness(result);
            
            // Проверка на эквивалентность с ожидаемой моделью
            Assert.IsTrue(result.Equals(expected), $"Результирующая модель должна быть эквивалентна ожидаемой." +
                $"\nМСА ожидаемой:\n{expected.MAS}\n" +
                $"\nМСА созданной:\n{result.MAS}");
            
            // Дополнительно: Проверка, что P-вершины имеют правильный формат (без _int)
            foreach (var v in result.Vertices.OfType<ConditionalVertex>())
            {
                if (v.ID.StartsWith("P"))
                {
                     // Проверка, что ID соответствует формату P{int}
                     bool formatOk = System.Text.RegularExpressions.Regex.IsMatch(v.ID, @"^P\d+$");
                     Assert.IsTrue(formatOk, $"ID условной вершины {v.ID} не соответствует формату P{{int}}");
                }
            }
        }
    }
}
