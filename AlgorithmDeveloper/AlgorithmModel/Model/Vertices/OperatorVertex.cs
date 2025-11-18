using AlgorithmDeveloper.AlgorithmModel;
using AlgorithmDeveloper.AlgorithmModel.Model.Vertices.Vizualization;

namespace AlgorithmDeveloper.AlgoDev.Model.Vertices
{
    /// <summary>
    /// Операторная вершина блок-схемы - "Yi".
    /// </summary>
    public sealed class OperatorVertex : BDVertex
    {
        public int Index { get; }

        public OperatorVertex(int index)
        {
            Index = index;
            ID = $"Y{index}";
            UpdateGeometry();
        }

        public override string Description => $"\nПройдена операторная вершина {Index}: \"{ID}\"";

        public override IBDVertex? GetNext(AlgoModel? model = null) => Next;

        protected override void BuildGeometry()
        {
            var inputPoints = new List<Point>
            {
                new Point(Center.X, Center.Y - IFigure.HalfHeight - IFigure.Indent) // [1]
            };

            var outputPoints = new List<Point>
            {
                new Point(Center.X, Center.Y + IFigure.HalfHeight + IFigure.Indent) // [3]
            };

            _gridGeometry = new GridGeometry(
                FigureShape.Rectangle,
                inputPoints,
                outputPoints
            );
        }
    }
}