using System;
using System.Collections.Generic;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph.Layout
{
    public interface ILayoutAlgorithm
    {
        void Arrange(AbstractAutomata model, bool UseJumpPoints = false);
    }
}