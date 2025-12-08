using AlgorithmDeveloper.AAModel;
using AlgorithmDeveloper.AAModel.Model.Vertices.Vizualization;

namespace AlgorithmDeveloper.AlgoDev.Model.Vertices
{
    public interface IGraphElement
    {
        /// <summary>
        /// Строковый идентификатор подграфа, к которому принадлежит элемент.
        /// </summary>
        string? SubgraphId { get; set; }
        /// <summary>
        /// Слой графа, на котором расположен элемент.
        /// </summary>
        int Layer { get; set; }
        /// <summary>
        /// Индекс элемента на слое (порядок расположения среди других элементов слоя).
        /// </summary>
        int IndexOnLayer { get; set; }
        /// <summary>
        /// Вес элемента графа (используется в алгоритмах компоновки).
        /// </summary>
        double Weight { get; set; }
        int InDegree { get; }
        int OutDegree { get; }

        /// <summary>
        /// Коллекция родителей (входящих связей) данного элемента графа.
        /// </summary>
        IReadOnlyList<IBDVertex> Parents { get; }
        /// <summary>
        /// Коллекция детей (исходящих связей) данного элемента графа.
        /// </summary>
        IReadOnlyList<IBDVertex> Children { get; }

        bool IsDummy { get; set; }

        void UpdateGraphMetrics(AbstractAutomata model);
    }

    public interface IGraphFigure : IFigure, IGraphElement { }
}