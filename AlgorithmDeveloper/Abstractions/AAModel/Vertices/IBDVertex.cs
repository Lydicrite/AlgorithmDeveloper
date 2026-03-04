using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Geometry;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices
{
    /// <summary>
    /// Интерфейс, содержащий общие и базовые свойства для всех элементов ЛСА.
    /// </summary>
    public interface IBDVertex
    {
        /// <summary>
        /// Уникальный идентификатор экземпляра вершины.
        /// </summary>
        Guid Uid { get; }
        /// <summary>
        /// Строковый идентификатор элемента.
        /// </summary>
        string? ID { get; }
        /// <summary>
        /// Возвращает подробное описание элемента.
        /// </summary>
        /// <returns></returns>
        string Description { get; }
        /// <summary>
        /// Потомок этого элемента (возможно null).
        /// </summary>
        IBDVertex? Next { get; set; }
        /// <summary>
        /// Получает потомка для этого элемента.
        /// Если указан <paramref name="model"/>, элемент может найти потомка через родительский алгоритм,
        /// а не только через собственное свойство <see cref="Next"/>.
        /// </summary>
        /// <param name="model">Необязательная модель алгоритма, позволяющая найти следующий элемент.</param>
        /// <returns>Потомок этого элемента (возможно null).</returns>
        IBDVertex? GetNext(AbstractAutomata? model = null);
    }



    /// <summary>
    /// Класс, предоставляющий возможности реализации для общих и базовых свойств всех элементов ЛСА.
    /// </summary>
    public abstract class BDVertex : IBDVertex, IGraphFigure
    {
        public Guid Uid { get; } = Guid.NewGuid();

        public string? ID { get; set; }
        public abstract string Description { get; }
        public IBDVertex? Next { get; set; }
        public abstract IBDVertex? GetNext(AbstractAutomata? model = null);





        #region Собственные методы

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not IBDVertex other) return false;
            return Uid.Equals(other.Uid);
        }

        public override int GetHashCode()
        {
            return Uid.GetHashCode();
        }

        #endregion





        #region Реализация части IGraphFigure - IFigure

        private Point _center = new Point(0, 0);
        protected GridGeometry _gridGeometry = new GridGeometry(FigureShape.Rectangle, Enumerable.Empty<Point>(), Enumerable.Empty<Point>());

        public Point Center
        {
            get => _center;
            set
            {
                _center = value;
                UpdateGeometry();
            }
        }
        public GridGeometry GridGeometry => _gridGeometry;
        public IEnumerable<Point> InputLinkPoints => _gridGeometry.InputLinkPoints;
        public IEnumerable<Point> OutputLinkPoints => _gridGeometry.OutputLinkPoints;

        protected void UpdateGeometry() => BuildGeometry();
        protected abstract void BuildGeometry();

        #endregion





        #region Реализация части IGraphFigure - IGraphElement

        private readonly List<IBDVertex> _parents = new();
        private readonly List<IBDVertex> _children = new();
        private int _layer;
        private int _layerIndex;
        private string? _subgraphId;
        private double _weight = 1.0;
        private bool _isDummy;
        private int _inDegree;
        private int _outDegree;
        private bool _isActive;

        public string? SubgraphId { get => _subgraphId; set => _subgraphId = value; }
        public int Layer { get => _layer; set => _layer = value; }
        public int IndexOnLayer { get => _layerIndex; set => _layerIndex = value; }
        public double Weight { get => _weight; set => _weight = value; }
        public int InDegree => _inDegree;
        public int OutDegree => _outDegree;
        public IReadOnlyList<IBDVertex> Parents => _parents;
        public IReadOnlyList<IBDVertex> Children => _children;
        public bool IsDummy { get => _isDummy; set => _isDummy = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; }

        public virtual void UpdateGraphMetrics(AbstractAutomata model)
        {
            _children.Clear();
            switch (this)
            {
                case ConditionalVertex cv:
                    if (cv.LBS != null) _children.Add(cv.LBS);
                    if (cv.RBS != null) _children.Add(cv.RBS);
                    break;
                default:
                    if (Next != null) _children.Add(Next);
                    break;
            }
            _outDegree = _children.Count;

            _parents.Clear();
            _inDegree = 0;
            foreach (var v in model.Vertices)
            {
                if (v is ConditionalVertex c2)
                {
                    if (c2.LBS == this || c2.RBS == this)
                    {
                        _parents.Add(c2);
                        _inDegree++;
                    }
                }
                else if (v.Next == this)
                {
                    _parents.Add(v);
                    _inDegree++;
                }
            }
        }

        #endregion

    }
}
