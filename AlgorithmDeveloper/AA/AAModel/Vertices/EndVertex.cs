using AlgorithmDeveloper.AAModel;
using AlgorithmDeveloper.AAModel.Model.Vertices.Vizualization;

namespace AlgorithmDeveloper.AlgoDev.Model.Vertices
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
                new Point(Center.X, Center.Y - IFigure.HalfHeight - IFigure.Indent) // [1]
            };
            _gridGeometry = new GridGeometry(
                FigureShape.Ellipse,
                inputPoints,
                Enumerable.Empty<Point>()
            );
        }
    }
}