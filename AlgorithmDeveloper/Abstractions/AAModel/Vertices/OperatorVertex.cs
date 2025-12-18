using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Geometry;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices
{
    /// <summary>
    /// Операторная вершина - "Yi".
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

        public override IBDVertex? GetNext(AbstractAutomata? model = null) => Next;

        protected override void BuildGeometry()
        {
            var inputPoints = new List<Point>
            {
                new Point(Center.X, Center.Y - IFigure.HalfHeight - IFigure.Indent) // [top]
            };

            var outputPoints = new List<Point>
            {
                new Point(Center.X, Center.Y + IFigure.HalfHeight + IFigure.Indent) // [bottom]
            };

            _gridGeometry = new GridGeometry
            (
                FigureShape.Rectangle,
                inputPoints,
                outputPoints
            );
        }
    }
}