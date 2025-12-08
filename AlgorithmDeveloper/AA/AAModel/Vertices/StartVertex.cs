using AlgorithmDeveloper.AAModel;
using AlgorithmDeveloper.AAModel.Model.Vertices.Vizualization;

namespace AlgorithmDeveloper.AlgoDev.Model.Vertices
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
                new Point(Center.X, Center.Y + IFigure.HalfHeight + IFigure.Indent) // [3]
            };
            _gridGeometry = new GridGeometry(
                FigureShape.Ellipse,
                Enumerable.Empty<Point>(),
                outputPoints
            );
        }
    }
}