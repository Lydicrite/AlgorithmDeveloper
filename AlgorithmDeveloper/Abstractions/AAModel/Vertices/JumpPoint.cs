using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Geometry;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices
{
    /// <summary>
    /// Представляет точку перехода ЛСА - "↓i".
    /// </summary>
    public sealed class JumpPoint : BDVertex
    {
        /// <summary>
        /// Индекс этой точки перехода.
        /// </summary>
        public int JumpIndex { get; }

        public JumpPoint(int index)
        {
            JumpIndex = index;
            ID = $"↓{index}";
            UpdateGeometry();
        }

        public override string Description =>
            $"\nПройдена точка перехода {JumpIndex}: \"{ID}\"";

        public override IBDVertex? GetNext(AbstractAutomata? model = null) => Next;

        protected override void BuildGeometry()
        {
            // Маленький круг: берём уменьшенные полурадиусы и выравниваем их
            int a = (int)Math.Round(IFigure.HalfWidth * 0.5);
            int b = (int)Math.Round(IFigure.HalfHeight * 0.5);
            int r = Math.Min(a, b);

            var inputPoints = new List<Point>
            {
                new Point(Center.X, Center.Y - r - 3) // [top]
            };

            var outputPoints = new List<Point>
            {
                new Point(Center.X, Center.Y + r + 3) // [bottom]
            };

            _gridGeometry = new GridGeometry
            (
                FigureShape.Circle,
                inputPoints,
                outputPoints
            );
        }
    }
}
