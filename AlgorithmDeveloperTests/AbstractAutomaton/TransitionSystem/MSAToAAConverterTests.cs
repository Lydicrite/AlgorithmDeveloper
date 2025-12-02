using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgorithmDeveloper.AAModel;
using AlgorithmDeveloper.AAModel.TransitionSystem.Converters;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using LogicalExpressions.Parsing;
using LogicalExpressions.Core;

namespace AlgorithmDeveloperTests.TransitionSystem
{
    [TestClass]
    public class MSAToAAConverterTests
    {
        private static Type ParserType => typeof(AlgorithmDeveloper.AAModel.AbstractAutomaton).Assembly.GetType("AlgorithmDeveloper.AAModel.LAS.LASParser", throwOnError: true)!;
        
        private AlgorithmDeveloper.AAModel.AbstractAutomaton Parse(string las)
        {
            var method = ParserType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
            if (method == null) throw new Exception("LASParser.Parse method not found");
            try
            {
                return (AlgorithmDeveloper.AAModel.AbstractAutomaton)method.Invoke(null, new object[] { las })!;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }



        [TestMethod]
        [DataRow("Yн X1 ↑1 Y1 w↑1 ↓1 Yк", DisplayName = "(TestGroup A) - Simple Loop")]
        [DataRow("Yн Y1 Yк", DisplayName = "(TestGroup A) - Linear")]
        [DataRow("Yн X1 ↑1 Y1 w↑2 ↓1 Y2 w↑2 ↓2 Yк", DisplayName = "(TestGroup A) - Branching")]
        [DataRow("Yн X1 ↑1 X2 ↑2 Y1 w↑3 ↓1 Y2 w↑3 ↓2 Y3 w↑3 ↓3 Yк", DisplayName = "(TestGroup A) - Nested Conditions")]
        [DataRow("Yн X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк", DisplayName = "(TestGroup A) - Complex")]

        [DataRow("Yн P0 ↑1 X1 ↑3 w↑4 ↓3 Y3 w↑4 ↓1 X0 ↑2 Y0 w↑4 ↓2 Y1 w↑4 ↓4 X2 ↑5 w↑6 ↓5 Y2 w↑4 ↓6 Yк", DisplayName = "(TestGroup B) - AA_0")]
        [DataRow("Yн Y0 X0 ↑1 Y1 w↑5 ↓1 P0 ↑2 w↑3 ↓2 X1 ↑4 Y2 w↑5 ↓4 Y3 w↑5 ↓3 Y4 X2 ↑5 w↑3 ↓5 Yк", DisplayName = "(TestGroup B) - AA_1")]
        [DataRow("Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 P0 ↑4 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк", DisplayName = "(TestGroup B) - AA_2")]
        [DataRow("Yн w↑1 ↓1 Y0 w↑2 ↓2 Y1 w↑3 ↓3 Y2 P0 ↑4 w↑5 ↓4 X0 ↑3 w↑5 ↓5 X1 ↑6 w↑2 ↓6 X2 ↑1 Yк", DisplayName = "(TestGroup B) - AA_3")]
        [DataRow("Yн Y0 X0 ↑1 Y1 X1 ↑4 Y2 w↑4 ↓1 P0 ↑2 w↑3 ↓2 Y3 w↑4 ↓3 Y4 X2 ↑3 w↑4 ↓4 Yк", DisplayName = "(TestGroup B) - AA_4")]
        [DataRow("Yн P0 ↑1 Y4 Y5 w↑3 ↓1 Y1 X0 ↑2 Y0 w↑3 ↓2 Y2 w↑3 ↓3 X1 ↑4 w↑5 ↓4 Y3 w↑5 ↓5 Yк", DisplayName = "(TestGroup B) - AA_5")]
        [DataRow("Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 P0 ↑3 w↑4 ↓3 Y2 X1 ↑5 w↑3 ↓4 Y3 X2 ↑4 w↑5 ↓5 Yк", DisplayName = "(TestGroup B) - AA_6")]
        [DataRow("Yн Y1 X0 ↑1 Y0 w↑2 ↓1 P0 ↑2 Y3 w↑2 ↓2 X1 ↑3 w↑4 ↓3 Y2 w↑4 ↓4 Yк", DisplayName = "(TestGroup B) - AA_7")]
        [DataRow("Yн P0 ↑1 Y1 w↑2 ↓1 Y3 w↑3 ↓2 Y2 X1 ↑2 w↑3 ↓3 X0 ↑4 Y5 w↑5 ↓4 Y4 w↑5 ↓5 Yк", DisplayName = "(TestGroup B) - AA_8")]
        [DataRow("Yн Y0 X0 ↑1 Y2 w↑9 ( ↓1 P0 ↑2 Y3 w↑5 ) ( ↓2 Y1 w↑4 ) ( ↓9 P1 ↑4 w↑5 ) ( ↓4 X1 ↑6 w↑3 ) ( ↓5 X2 ↑3 w↑6 ) ( ↓3 Y4 w↑6 ) ↓6 Yк", DisplayName = "(TestGroup B) - AA_9")]
        [DataRow("Yн P0 ↑1 Y0 w↑1 ↓1 X1 ↑2 w↑4 ↓2 P1 ↑3 Y3 w↑4 ↓3 Y1 w↑4 ↓4 P2 ↑5 X2 ↑7 w↑8 ↓5 X0 ↑6 w↑8 ↓6 Y2 w↑8 ↓7 Y4 w↑8 ↓8 Yк", DisplayName = "(TestGroup B) - AA_10")]
        [DataRow("Yн X1 ↑1 Y1 w↑1 ↓1 Y2 P2 ↑2 X3 ↑3 w↑1 ↓3 P4 ↑4 w↑1 ↓4 Y3 Y4 w↑2 ↓2 Yк", DisplayName = "(TestGroup B) - AA_11")]

        [DataRow("Yн w↑1 ↓1 X1 ↑1 X2 ↑3 w↑2 ↓2 Y2 w↑4 ↓3 Y1 X5 ↑6 w↑4 ↓4 X3 ↑3 X4 ↑2 w↑5 ↓5 Y3 w↑10 ↓6 Y4 w↑7 ↓7 X6 ↑8 w↑5 ↓8 X7 ↑9 w↑10 ↓9 Y5 w↑7 ↓10 Yк", DisplayName = "(TestGroup C) - AA_0")]
        [DataRow("Yн w↑0 ↓0 X0 ↑0 w↑1 ↓1 X1 ↑1 w↑2 ↓2 X2 ↑4 X3 ↑3 Y1 w↑0 ↓3 Y0 w↑2 ↓4 X4 ↑5 P0 ↑7 P2 ↑9 w↑0 ↓5 X5 ↑6 w↑8 ↓6 P3 ↑6 w↑8 ↓7 P1 ↑8 w↑9 ↓8 Y3 P4 ↑6 w↑10 ↓9 Y2 P5 ↑10 w↑2 ↓10 Yк", DisplayName = "(TestGroup C) - AA_1")]
        [DataRow("Yн Y0 X0 ↑0 X1 ↑1 w↑0 ↓0 Y1 w↑6 ↓1 X2 ↑2 w↑1 ↓2 Y2 X3 ↑2 Y3 X4 ↑3 w↑1 ↓3 Y4 X5 ↑2 X6 ↑4 Y6 X8 ↑5 w↑3 ↓4 Y5 X7 ↑5 w↑0 ↓5 Y7 X9 ↑6 w↑1 ↓6 Yк", DisplayName = "(TestGroup C) - AA_2")]
        [DataRow("Yн P1 ↑1 Y1 w↑1 ↓1 P2 ↑2 Y2 P2 ↑3 Y3 w↑3 ↓3 P3 ↑4 w↑1 ↓4 Y4 Y5 w↑2 ↓2 P4 ↑5 X5 ↑6 Y6 w↑6 ↓6 Y7 w↑5 ↓5 Y8 X1 ↑7 w↑1 ↓7 Yк", DisplayName = "(TestGroup C) - AA_3")]
        public void E2E_Conversion_Test(string las)
        {
            // 1. Parse
            AlgorithmDeveloper.AAModel.AbstractAutomaton aa1;
            try 
            {
                aa1 = Parse(las);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Parsing failed: {ex.Message}");
                return;
            }

            // 2. Convert
            // Note: We use Legacy mode for verification because the reconstructed graph might 
            // introduce structural changes that affect "Reachability" from Yн in NoParadox mode,
            // potentially hiding some paths in the generated MAS. 
            // Legacy mode ensures all local paths are captured, allowing for structural equivalence check.
            
            var mas1 = BuildLegacyMAS(aa1);
            Assert.IsNotNull(mas1, "MAS should not be null");
            
            AlgorithmDeveloper.AAModel.AbstractAutomaton aa2 = MSAToAAConverter.Convert(mas1);
            
            // 3. Compare
            var mas2 = BuildLegacyMAS(aa2);
            Assert.IsNotNull(mas2, "Reconstructed MAS should not be null");

            // Compare Headers (Vertices)
            CollectionAssert.AreEqual((System.Collections.ICollection)mas1.Headers, (System.Collections.ICollection)mas2.Headers, "MAS Headers should match");

            // Compare Cells Logically
            foreach (var row1 in mas1.Rows)
            {
                string rowHeader = row1.Header;
                var row2 = mas2[rowHeader];

                for (int i = 0; i < mas1.Headers.Count; i++)
                {
                    // mas1.Headers[i] is the target vertex ID for column i
                    string cell1 = row1.Transitions[i];
                    string cell2 = row2.Transitions[i];

                    Assert.IsTrue(AreLogicallyEquivalent(cell1, cell2), 
                        $"Logic mismatch at Row '{rowHeader}', Col '{mas1.Headers[i]}'.\nExpected: {cell1}\nActual: {cell2}");
                }
            }
        }

        private AlgorithmDeveloper.AAModel.TransitionSystem.MAS.MatrixAlgorithmSchema BuildLegacyMAS(AlgorithmDeveloper.AAModel.AbstractAutomaton aa)
        {
            var formulas = AlgorithmDeveloper.AAModel.TransitionSystem.TransitionSystemBuilder.BuildAll(
                aa, 
                AlgorithmDeveloper.AAModel.TransitionSystem.TransitionBuildMode.Legacy
            );
            return AlgorithmDeveloper.AAModel.TransitionSystem.MAS.MatrixAlgorithmSchema.FromFormulas(formulas);
        }

        private bool AreLogicallyEquivalent(string expr1, string expr2)
        {
            if (string.IsNullOrWhiteSpace(expr1)) expr1 = "0"; // Empty cell means no transition (False)
            if (string.IsNullOrWhiteSpace(expr2)) expr2 = "0";

            expr1 = Sanitize(expr1);
            expr2 = Sanitize(expr2);

            try
            {
                var node1 = ExpressionParser.Parse(expr1);
                var node2 = ExpressionParser.Parse(expr2);
                
                var le1 = new LogicalExpression(node1);
                var le2 = new LogicalExpression(node2);

                return le1.EquivalentTo(le2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error comparing '{expr1}' and '{expr2}': {ex.Message}");
                return false;
            }
        }

        private string Sanitize(string expr)
        {
            return expr.Replace("˄", "&").Replace("˅", "|").Replace("¬", "!");
        }
    }
}
