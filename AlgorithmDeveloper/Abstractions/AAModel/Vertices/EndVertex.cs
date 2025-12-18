using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Geometry;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices
{
    /// <summary>
    /// Конечная вершина - "Yк".
    /// </summary>
    public sealed class EndVertex : BDVertex
    {
        public EndVertex()
        {
            ID = "Yк";
            UpdateGeometry();
        }

        public override string Description => "\n ------- Конец алгоритма ------- ";

        public override IBDVertex? GetNext(AbstractAutomata? model = null) => null;

        protected override void BuildGeometry()
        {
            var inputPoints = new List<Point>
            {
                new Point(Center.X, Center.Y - IFigure.HalfHeight - IFigure.Indent) // [top]
            };
            _gridGeometry = new GridGeometry
            (
                FigureShape.Ellipse,
                inputPoints,
                Enumerable.Empty<Point>()
            );
        }
    }
}