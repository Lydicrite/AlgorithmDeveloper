using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.LAS;
using AlgorithmDeveloper.Abstractions.Minimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmDeveloperTests.Tests
{
    [TestClass]
    public class StructuralMinimizerExtendedTests
    {
        [TestMethod]
        public void TestNormalizeEndVertices_MergesMultipleEnds()
        {
            // Create graph with two EndVertices
            var model = new AbstractAutomata();
            var start = model.AddVertex(new StartVertex());
            var end1 = model.AddVertex(new EndVertex());
            var end2 = model.AddVertex(new EndVertex());
            
            // Start -> X1 (True->End1, False->End2)
            var x1 = model.AddVertex(new ConditionalVertex("X", 1));
            model.LinkNext(start, x1);
            x1.RBS = end1;
            x1.LBS = end2;
            
            model.Update();
            
            var minimized = StructuralMinimizer.Minimize(model);
            
            Assert.AreEqual(1, minimized.Vertices.OfType<EndVertex>().Count(), "Should have exactly one EndVertex");
            
            // Check logic in minimized model
            // IMPORTANT: In the minimized model, if X1's RBS and LBS point to the same vertex (the merged EndVertex),
            // it becomes a redundant check and might be removed by RemoveRedundantChecks!
            // If X1 is removed, Start will point directly to End.
            
            if (minimized.Vertices.Any(v => v.ID == "X1"))
            {
                var minX1 = minimized.Vertices.OfType<ConditionalVertex>().First(v => v.ID == "X1");
                Assert.AreEqual(minX1.RBS, minX1.LBS, "Both branches should point to the same EndVertex instance");
            }
            else
            {
                // If X1 was optimized away, Start should point to End
                var minEnd = minimized.Vertices.OfType<EndVertex>().First();
                Assert.AreEqual(minEnd, minimized.Start.Next, "Redundant X1 removed, Start should point to End");
            }
        }

        [TestMethod]
        public void TestOptimizeSequentialConditions_Simple()
        {
            // Start -> X1 --True--> X1 --True--> Y1
            //             --False--> Y2
            // Expected: Start -> X1 --True--> Y1 (skips second X1)
            
            var model = new AbstractAutomata();
            var start = model.AddVertex(new StartVertex());
            var end = model.AddVertex(new EndVertex());
            
            var y1 = model.AddVertex(new OperatorVertex(1));
            model.LinkNext(y1, end);
            
            var y2 = model.AddVertex(new OperatorVertex(2));
            model.LinkNext(y2, end);
            
            var x1_first = model.AddVertex(new ConditionalVertex("X", 1));
            var x1_second = model.AddVertex(new ConditionalVertex("X", 1));
            
            model.LinkNext(start, x1_first);
            
            // First X1 setup
            x1_first.RBS = x1_second; // True branch goes to second X1
            x1_first.LBS = y2;
            
            // Second X1 setup
            x1_second.RBS = y1;
            x1_second.LBS = y2;
            
            model.Update();
            
            var minimized = StructuralMinimizer.Minimize(model);
            
            // Verify in minimized
            var minX1 = minimized.Vertices.OfType<ConditionalVertex>().First(v => v.ID == "X1"); // Should be only one reachable if optimized
            var minY1 = minimized.Vertices.OfType<OperatorVertex>().First(v => v.ID == "Y1");
            
            Assert.AreEqual(minY1, minX1.RBS, "True branch of first X1 should point directly to Y1");
            // Count conditionals
            Assert.AreEqual(1, minimized.Vertices.OfType<ConditionalVertex>().Count(v => v.ID == "X1"), "Second X1 should be removed as unreachable");
        }

        [TestMethod]
        public void TestOptimizeSequentialConditions_WithJumpPoints()
        {
            // Start -> X1 --True--> JP1 -> X1 ...
            var model = new AbstractAutomata();
            var start = model.AddVertex(new StartVertex());
            var end = model.AddVertex(new EndVertex());
            var jp1 = model.EnsureJumpPoint(1);
            
            var y1 = model.AddVertex(new OperatorVertex(1));
            model.LinkNext(y1, end);
            var y2 = model.AddVertex(new OperatorVertex(2));
            model.LinkNext(y2, end);
            
            var x1_first = model.AddVertex(new ConditionalVertex("X", 1));
            var x1_second = model.AddVertex(new ConditionalVertex("X", 1));
            
            model.LinkNext(start, x1_first);
            x1_first.RBS = jp1;
            model.LinkNext(jp1, x1_second);
            x1_first.LBS = y2;
            
            x1_second.RBS = y1;
            x1_second.LBS = y2;
            
            model.Update();
            
            var minimized = StructuralMinimizer.Minimize(model);
            
            var minX1 = minimized.Vertices.OfType<ConditionalVertex>().First(v => v.ID == "X1");
            var minY1 = minimized.Vertices.OfType<OperatorVertex>().First(v => v.ID == "Y1");
            
            Assert.AreEqual(minY1, minX1.RBS, "Should skip JP and second X1");
        }
    }
}
