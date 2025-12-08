using System;
using System.Collections.Generic;
using AlgorithmDeveloper.AAModel;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;

namespace AlgorithmDeveloper.AlgoDev.Model.Vertices.Graph.Layout
{
    /// <summary>
    /// Интерфейс для алгоритмов компоновки графов, которые упорядочивают вершины <see cref="AbstractAutomaton"/>.
    /// </summary>
    public interface ILayoutAlgorithm
    {
        /// <summary>
        /// Располагает вершины модели в пространстве.
        /// </summary>
        /// <param name="model">Модель алгоритма.</param>
        void Arrange(AbstractAutomata model);
    }
}