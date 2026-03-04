using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.LAS;
using AlgorithmDeveloper.Abstractions.TransitionSystem.MAS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlgorithmDeveloperTests.Tests
{
    [TestClass]
    public class ReproductionTests
    {
        [TestMethod]
        public void Reproduce_RandomLAS_Failure()
        {
            // LAS from the failure log
            string las = "Yн Y1 X1 ↑1 P2 ↑2 Y2 Y3 w↑2 ↓2 P3 ↑3 X4 ↑4 Y4 w↑4 ↓4 Y5 w↑3 ↓3 Y6 Y7 Y8 w↑1 ↓1 X5 ↑5 Y9 Y10 Y11 X6 ↑6 Y12 Y13 w↑6 ↓6 Y14 w↑5 ↓5 P7 ↑7 Y15 Y16 w↑7 ↓7 P8 ↑8 w↑1 ↓8 X9 ↑9 P10 ↑10 Y17 w↑10 ↓10 P10 ↑11 Y18 w↑11 ↓11 X5 ↑12 Y19 w↑12 ↓12 Y20 w↑9 ↓9 Y21 Yк";

            Console.WriteLine($"Original LAS: {las}");

            // 1. Parse LAS -> Model 1
            bool ok = LASParser.TryParse(las, out var model1, out List<ParsingError> err);
            Assert.IsTrue(ok, $"Ошибка парсинга исходной ЛСА: {string.Join(", ", err.Select(e => e.Message))}");
            Assert.IsNotNull(model1);

            // 2. Model 1 -> MAS String
            var masString = model1.MAS!.ToString();
            Console.WriteLine($"Generated MAS:\n{masString}");

            // 3. MAS String -> Model 2
            var model2 = MASToAAConverter.Convert(masString);
            Console.WriteLine($"Restored LAS: {model2.LAS}");

            Console.WriteLine("Vertices in Model 2:");
            foreach (var v in model2.Vertices)
            {
                Console.WriteLine(v.ToString());
            }

            bool ok2 = model2.AreAllVerticesReachable(out var unreachable);
            Console.WriteLine($"AreAllVerticesReachable: {ok2}");
            if (!ok2) 
            {
                Console.WriteLine($"Unreachable in Model 2 object: {string.Join(", ", unreachable)}");
            }
            
            // Debug Model 2 Runs
             var jp15 = model2.Vertices.OfType<JumpPoint>().FirstOrDefault(j => j.JumpIndex == 15);
             Console.WriteLine($"JP15 in Model 2: {jp15?.ID ?? "null"}");

             Console.WriteLine("Model 2 Runs Check:");
             var runs2 = model2.FindRuns(true);
             bool found15 = false;
             foreach(var run in runs2)
             {
                  var path = run.Item2;
                  // Print first few paths to debug format
                  // Console.WriteLine($"Run: {path}"); 
                  
                  if (path.Contains("↓15"))
                  {
                      found15 = true;
                      Console.WriteLine($"[M2 REACHED] {run.Item1}: {path}");
                      break; // Found one is enough
                  }
             }
             if (!found15) 
             {
                 Console.WriteLine("[M2 MISSED] ↓15 never reached!");
                 Console.WriteLine("Printing first 5 paths:");
                 foreach(var run in runs2.Take(5))
                     Console.WriteLine(run.Item2);
             }

            // Debug Model 3 (Parsed from LAS)
            try
            {
                var model3 = LASParser.Parse(model2.LAS);
                bool ok3 = model3.AreAllVerticesReachable(out var un3);
                Console.WriteLine($"Model3 (Parsed) Reachable: {ok3}");
                if (!ok3) 
                {
                    Console.WriteLine($"Unreachable in Model 3: {string.Join(", ", un3)}");
                    
                    // Debug runs
                    var runs = model3.FindRuns(true);
                    Console.WriteLine("Model 3 Runs:");
                     foreach(var run in runs)
                     {
                         var path = run.Item2;
                         if (path.Contains("↓15"))
                             Console.WriteLine($"[REACHED] {run.Item1}: {path}");
                     }
                    
                    var jp15_3 = model3.Vertices.OfType<JumpPoint>().FirstOrDefault(j => j.JumpIndex == 15);
                    if (jp15_3 != null)
                    {
                         Console.WriteLine("JP15 found in Model 3.");
                         // Check incoming
                         foreach(var v in model3.Vertices)
                         {
                             if (v.Next == jp15_3) Console.WriteLine($"M3: {v} -> JP15 (Next)");
                             if (v is ConditionalVertex cv)
                             {
                                 if (cv.LBS == jp15_3) Console.WriteLine($"M3: {cv} -> JP15 (LBS)");
                                 if (cv.RBS == jp15_3) Console.WriteLine($"M3: {cv} -> JP15 (RBS)");
                             }
                         }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"LASParser failed: {ex.Message}");
            }

            bool correct = model2.CheckCorrectness(out string msg);
            Console.WriteLine($"CheckCorrectness: {correct}");
            if (!correct)
            {
                Console.WriteLine($"Errors: {msg}");
            }

            Assert.IsTrue(correct, $"Модель 2 имеет некорректную ЛСА: {msg}");
            Assert.IsTrue(model1.Equals(model2), "Models are not equivalent");
        }
    }
}
