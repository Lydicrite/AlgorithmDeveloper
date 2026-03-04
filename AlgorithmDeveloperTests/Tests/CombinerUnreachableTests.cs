using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.Combining;
using AlgorithmDeveloper.Abstractions.LAS;
using AlgorithmDeveloper.Abstractions.TransitionSystem.MAS;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlgorithmDeveloperTests.Tests
{
    [TestClass]
    public class CombinerUnreachableTests
    {
        [TestMethod]
        public void ReproduceUnreachable()
        {
            string input1 = "Yн Y4 X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 P0 ↑3 w↑4 ↓3 Y2 X1 ↑5 w↑3 ↓4 Y3 X2 ↑4 w↑5 ↓5 Yк";
            string input2 = "Yн Y0 w↑1 ↓1 Y1 X0 ↑2 w↑1 ↓2 P0 ↑4 X1 ↑3 Y3 w↑4 ↓3 Y4 w↑4 ↓4 Y2 Yк";

            var model1 = LASParser.Parse(input1);
            var model2 = LASParser.Parse(input2);

            try
            {
                var combinedMas = MASCombiner.Combine(model1, model2);
                Console.WriteLine($"MASCombiner LAS: {combinedMas.LAS}");
                
                if (!combinedMas.CheckCorrectness(out var msg))
                {
                    Console.WriteLine($"MASCombiner correctness check failed: {msg}");
                    Assert.Fail(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MASCombiner exception: {ex.Message}");
                throw;
            }
        }
    }
}
