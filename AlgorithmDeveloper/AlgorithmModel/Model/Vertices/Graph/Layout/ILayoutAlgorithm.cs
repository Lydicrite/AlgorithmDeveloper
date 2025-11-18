using System;
using System.Collections.Generic;
using AlgorithmDeveloper.AlgorithmModel;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;

namespace AlgorithmDeveloper.AlgoDev.Model.Vertices.Graph.Layout
{
    /// <summary>
    /// Интерфейс для алгоритмов компоновки графов, которые упорядочивают вершины <see cref="AlgoModel"/>.
    /// </summary>
    public interface ILayoutAlgorithm
    {
        /// <summary>
        /// Располагает вершины модели в пространстве.
        /// </summary>
        /// <param name="model">Модель алгоритма.</param>
        void Arrange(AlgoModel model);
    }
}