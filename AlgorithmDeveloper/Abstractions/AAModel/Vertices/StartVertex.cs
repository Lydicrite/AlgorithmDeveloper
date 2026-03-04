using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Geometry;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices
{
    /// <summary>
    /// Начальная вершина - "Yн".
    /// </summary>
    public sealed class StartVertex : BDVertex
    {
        public StartVertex()
        {
            ID = "Yн";
            UpdateGeometry();
        }

        public override string Description => "\n ------- Начало алгоритма ------- ";

        public override IBDVertex? GetNext(AbstractAutomata? model = null) => Next;

        protected override void BuildGeometry()
        {
            var outputPoints = new List<Point>
            {
                new Point(Center.X, Center.Y + IFigure.HalfHeight + IFigure.Indent) // [bottom]
            };

            _gridGeometry = new GridGeometry
            (
                FigureShape.Ellipse,
                Enumerable.Empty<Point>(),
                outputPoints
            );
        }
    }
}