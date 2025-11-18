using AlgorithmDeveloper.AlgorithmModel;
using AlgorithmDeveloper.AlgorithmModel.Model.Vertices.Vizualization;

namespace AlgorithmDeveloper.AlgoDev.Model.Vertices
{
    /// <summary>
    /// Интерфейс, содержащий общие и базовые свойства для всех элементов ЛСА.
    /// </summary>
    public interface IBDVertex
    {
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
        IBDVertex? GetNext(AlgoModel? model = null);
    }

    /// <summary>
    /// Класс, предоставляющий возможности реализации для общих и базовых свойств всех элементов ЛСА.
    /// </summary>
    public abstract class BDVertex : IBDVertex, IGraphFigure
    {
        public string? ID { get; protected set; }
        public abstract string Description { get; }
        public IBDVertex? Next { get; set; }
        public abstract IBDVertex? GetNext(AlgoModel? model = null);





        #region Собственные методы

        /*
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not IBDVertex other) return false;

            return Equivalent(this, other, new HashSet<(IBDVertex, IBDVertex)>());
        }

        private static bool Equivalent(IBDVertex? a, IBDVertex? b, HashSet<(IBDVertex, IBDVertex)> visited)
        {
            if (a is null || b is null) return a is null && b is null;
            if (ReferenceEquals(a, b)) return true;

            var pair = (a, b);
            if (visited.Contains(pair)) return true; // уже сравнивали эту пару в рамках рекурсивной проверки
            visited.Add(pair);

            // Сравнение по типам согласно правилам эквивалентности
            switch (a)
            {
                case StartVertex when b is StartVertex:
                    return true;

                case EndVertex when b is EndVertex:
                    return true;

                case OperatorVertex ovA when b is OperatorVertex ovB:
                    return string.Equals(ovA.ID, ovB.ID, StringComparison.Ordinal);

                case ConditionalVertex cvA when b is ConditionalVertex cvB:
                    if (!string.Equals(cvA.ID, cvB.ID, StringComparison.Ordinal))
                        return false;
                    return Equivalent(cvA.LBS, cvB.LBS, visited) && Equivalent(cvA.RBS, cvB.RBS, visited);

                case JumpPoint jpA when b is JumpPoint jpB:
                    // Next() эквивалентны
                    if (!Equivalent(jpA.GetNext(null), jpB.GetNext(null), visited))
                        return false;
                    // Родители: одинаковое количество и попарная эквивалентность (как мультимножество)
                    var pa = (jpA as BDVertex).Parents;
                    var pb = (jpB as BDVertex).Parents;
                    if (pa.Count != pb.Count) return false;
                    var used = new bool[pb.Count];
                    foreach (var p in pa)
                    {
                        bool matched = false;
                        for (int j = 0; j < pb.Count; j++)
                        {
                            if (used[j]) continue;
                            if (Equivalent(p, pb[j], visited))
                            {
                                used[j] = true;
                                matched = true;
                                break;
                            }
                        }
                        if (!matched) return false;
                    }
                    return true;

                default:
                    // Типы различаются — не эквивалентны
                    return false;
            }
        }

        public override int GetHashCode()
        {
            // Хеш-код согласован с правилами Equals, но избегает глубокой рекурсии.
            // Коллизии допустимы, главное — равные объекты имеют равные хеши.
            int FragmentFor(IBDVertex? v)
            {
                if (v is null) return 0;
                return v switch
                {
                    StartVertex => 101,
                    EndVertex => 102,
                    OperatorVertex ov => HashCode.Combine(201, ov.ID ?? string.Empty),
                    ConditionalVertex cv => HashCode.Combine(202, cv.ID ?? string.Empty),
                    JumpPoint jp => HashCode.Combine(203, (jp as BDVertex).Parents.Count, jp.GetNext(null)?.GetType().Name ?? ""),
                    _ => 999
                };
            }

            return this switch
            {
                StartVertex => 1,
                EndVertex => 2,
                OperatorVertex ov => HashCode.Combine(3, ov.ID ?? string.Empty),
                ConditionalVertex cv => HashCode.Combine(4, cv.ID ?? string.Empty, FragmentFor(cv.LBS), FragmentFor(cv.RBS)),
                JumpPoint jp => HashCode.Combine(5, (jp as BDVertex).Parents.Count, jp.GetNext(null)?.GetType().Name ?? ""),
                _ => ID?.GetHashCode() ?? 0
            };
        }
        */

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

        public string? SubgraphId { get => _subgraphId; set => _subgraphId = value; }
        public int Layer { get => _layer; set => _layer = value; }
        public int IndexOnLayer { get => _layerIndex; set => _layerIndex = value; }
        public double Weight { get => _weight; set => _weight = value; }
        public int InDegree => _inDegree;
        public int OutDegree => _outDegree;
        public IReadOnlyList<IBDVertex> Parents => _parents;
        public IReadOnlyList<IBDVertex> Children => _children;
        public bool IsDummy { get => _isDummy; set => _isDummy = value; }

        public virtual void UpdateGraphMetrics(AlgoModel model)
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
