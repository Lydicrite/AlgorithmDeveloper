using AlgorithmDeveloper.AlgorithmModel;
using AlgorithmDeveloper.AlgorithmModel.Model.Vertices.Vizualization;

namespace AlgorithmDeveloper.AlgoDev.Model.Vertices
{
    /// <summary>
    /// Условная вершина блок-схемы - "Xi" или "Pi".
    /// </summary>
    public sealed class ConditionalVertex : BDVertex
    {
        public int Index { get; }
        public string Prefix { get; }
        public bool? Value { get; set; }
        public IBDVertex? LBS { get; set; }
        public IBDVertex? RBS { get; set; }

        public ConditionalVertex(string prefix, int index)
        {
            Prefix = prefix;
            Index = index;
            ID = $"{Prefix}{index}";
            UpdateGeometry();
        }

        public override string Description
        {
            get
            {
                string condValue = Value.HasValue ? (Value.Value ? "1" : "0") : "не установлено";
                return $"\nПройдена условная вершина {Index}: \"{ID}\"\n\tЗначение условия: {condValue}";
            }
        }

        public override IBDVertex? GetNext(AbstractAutomaton? model = null) => Value.HasValue ? (Value.Value ? RBS : LBS) : null;

        protected override void BuildGeometry()
        {
            var inputPoints = new List<Point>
            {
                new Point(Center.X, Center.Y - IFigure.HalfHeight - IFigure.Indent) // [top]
            };

            var outputPoints = new List<Point>
            {
                new Point(Center.X - IFigure.HalfWidth - IFigure.Indent, Center.Y), // [left]
                new Point(Center.X + IFigure.HalfWidth + IFigure.Indent, Center.Y)  // [right]
            };

            _gridGeometry = new GridGeometry(
                FigureShape.Diamond,
                inputPoints,
                outputPoints
            );
        }
    }
}
