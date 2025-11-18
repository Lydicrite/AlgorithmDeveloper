namespace AlgorithmDeveloper.AlgorithmModel.Model.Vertices.Vizualization
{
    /// <summary>
    /// Перечисление форм для геометрии вершин.
    /// </summary>
    public enum FigureShape
    {
        /// <summary>
        /// Эллипс (используется для Yн и Yк).
        /// </summary>
        Ellipse,
        /// <summary>
        /// Ромбовидный параллелограмм (используется для Xi).
        /// </summary>
        Diamond,
        /// <summary>
        /// Прямоугольник (используется для Yi).
        /// </summary>
        Rectangle,
        /// <summary>
        /// Круг (используется для ↓i).
        /// </summary>
        Circle,
    }



    /// <summary>
    /// Класс, представляющий геометрию сетки для вершин блок-схемы.
    /// </summary>
    public class GridGeometry
    {
        /// <summary>
        /// Форма вершины.
        /// </summary>
        public FigureShape Shape { get; }

        /// <summary>
        /// Входные точки связи.
        /// </summary>
        public IEnumerable<Point> InputLinkPoints { get; }

        /// <summary>
        /// Выходные точки связи.
        /// </summary>
        public IEnumerable<Point> OutputLinkPoints { get; }

        public GridGeometry(FigureShape shape, IEnumerable<Point> inputLinkPoints, IEnumerable<Point> outputLinkPoints)
        {
            Shape = shape;
            InputLinkPoints = inputLinkPoints ?? throw new ArgumentNullException(nameof(inputLinkPoints));
            OutputLinkPoints = outputLinkPoints ?? throw new ArgumentNullException(nameof(outputLinkPoints));
        }
    }



    /// <summary>
    /// Интерфейс для геометрии вершин блок-схемы.
    /// </summary>
    public interface IFigure
    {
        /// <summary>
        /// Точка центра вершины.
        /// </summary>
        Point Center { get; set; }
        /// <summary>
        /// Полуширина вершины (одинаковая для всех вершин).
        /// </summary>
        static int HalfWidth { get; } = 50;
        /// <summary>
        /// Полувысота вершины (одинаковая для всех вершин).
        /// </summary>
        static int HalfHeight { get; } = 25;
        /// <summary>
        /// Отступ для точек связи (одинаковый для всех вершин).
        /// </summary>
        static int Indent { get; } = 5;
        /// <summary>
        /// Геометрия сетки, содержащая форму и точки связи.
        /// </summary>
        GridGeometry GridGeometry { get; }
        /// <summary>
        /// Входные точки связи.
        /// </summary>
        IEnumerable<Point> InputLinkPoints { get; }
        /// <summary>
        /// Выходные точки связи.
        /// </summary>
        IEnumerable<Point> OutputLinkPoints { get; }
    }
}