using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Runtime.CompilerServices;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.TransitionSystem;
using AlgorithmDeveloper.Abstractions.TransitionSystem.MAS;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph.Layout;
using AlgorithmDeveloper.Abstractions.AAModel.Utils;
using AlgorithmDeveloper.Abstractions.LAS;

namespace AlgorithmDeveloper.Abstractions.AAModel
{
    /// <summary>
    /// Внутренняя модель алгоритма (блок-схемы).
    /// Хранит вершины и предоставляет API для их добавления, настройки и связи.
    /// </summary>
    public class AbstractAutomata : IEquatable<AbstractAutomata>
    {
        #region Поля

        private readonly List<IBDVertex> _vertices = new();
        private readonly Dictionary<string, List<IBDVertex>> _byId = new(StringComparer.Ordinal);
        private readonly Dictionary<int, JumpPoint> _jumpPoints = new();
        private readonly Dictionary<int, List<IGraphFigure>> _byLayer = new();
        private List<TransitionFormula> _transitionFormulas = new();
        private MatrixAlgorithmSchema? _mas = null;

        /// <summary>
        /// Логическая схема (ЛСА), описывающая эту модель алгоритма.
        /// </summary>
        public string LAS { get; set; } = string.Empty;
        /// <summary>
        /// Информация об алгоритме.
        /// </summary>
        public string Information => BuildAlgorithmInfo(false);
        /// <summary>
        /// Информация о циклах алгоритма.
        /// </summary>
        public string CyclesInfo
        {
            get
            {
                var cycles = FindCycles(false);
                if (cycles.Count == 0)
                    return "Циклы не обнаружены.";

                var lines = new List<string> 
                {
                    $"Найденные циклы: [{cycles.Count}]" 
                };

                lines.Add("{");
                foreach (var c in cycles)
                {
                    lines.Add($"    Условия: \"{c.bin}\", ");
                    lines.Add($"            ∞: {c.cycle}");
                }
                lines.Add("}");
                return string.Join(Environment.NewLine, lines);
            }
        }
        /// <summary>
        /// Информация о ходах работы алгоритма.
        /// </summary>
        public string RunsInfo
        {
            get
            {
                var runs = FindRuns(false);
                var lines = new List<string>
                {
                    $"Результаты работы алгоритма для всех возможных комбинаций значений условных вершин: [{runs.Count}]"
                };
                lines.Add("{");
                foreach (var r in runs)
                {
                    lines.Add($"    Условия: \"{r.bin}\", ");
                    lines.Add($"            ход работы алгоритма: {r.path}");
                }
                lines.Add("}");
                return string.Join(Environment.NewLine, lines);
            }
        }
        /// <summary>
        /// Информация о переходах алгоритма и его матричная схема.
        /// </summary>
        public string TransitionsAndMASInfo 
        {
            get
            {
                var lines = new List<string>();
                lines.Add("");
                lines.Add($"► Формулы перехода: [{TransitionFormulas.Count}]");
                lines.Add("{");
                foreach (var t in TransitionFormulas)
                    lines.Add($"    {t.HumanReadableExpression}");
                lines.Add("}");
                lines.Add("◄");
                lines.Add("");

                lines.Add("");
                lines.Add($"► Матричная схема алгоритма:");
                lines.Add("{");
                lines.Add("");
                lines.Add(_mas?.ToString() ?? "[пустая]");
                lines.Add("}");
                lines.Add("◄");
                lines.Add("");

                return string.Join(Environment.NewLine, lines);
            }
        }
        /// <summary>
        /// Начальная вершина алгоритма - Yн.
        /// </summary>
        public StartVertex? Start { get; private set; }
        /// <summary>
        /// Конечная вершина алгоритма - Yк.
        /// </summary>
        public EndVertex? End { get; private set; }
        /// <summary>
        /// ReadOnly-коллекция вершин алгоритма.
        /// </summary>
        public IReadOnlyList<IBDVertex> Vertices => _vertices;
        /// <summary>
        /// ReadOnly-коллекция формул перехода - система формул перехода (СФП).
        /// </summary>
        public IReadOnlyList<TransitionFormula> TransitionFormulas => _transitionFormulas;
        /// <summary>
        /// Матричная схема алгоритма (МСА).
        /// </summary>
        public MatrixAlgorithmSchema? MAS => _mas;

        public IReadOnlyDictionary<int, List<IGraphFigure>> ByLayer => _byLayer;

        /// <summary>
        /// Свойство, задающее порядок привязки значений для условных вершин (по ID, Ordinal).
        /// </summary>
        public IComparer<ConditionalVertex> ConditionalsBindingComparer { get; set; } = Comparer<ConditionalVertex>.Create((a, b) =>
        {
            var aId = a?.ID ?? string.Empty;
            var bId = b?.ID ?? string.Empty;
            return StringComparer.Ordinal.Compare(aId, bId);
        });

        #endregion



        #region Клонирование

        /// <summary>
        /// Создает глубокую копию модели автомата.
        /// Копирует все вершины и восстанавливает связи между ними.
        /// </summary>
        public AbstractAutomata Clone()
        {
            var clone = new AbstractAutomata();
            clone.LAS = this.LAS;
            
            // 1. Создаем копии всех вершин и сохраняем маппинг старая -> новая
            var vertexMap = new Dictionary<IBDVertex, IBDVertex>(ReferenceEqualityComparer.Instance);

            foreach (var originalVertex in this.Vertices)
            {
                IBDVertex newVertex = CloneVertex(originalVertex);
                clone.AddVertex(newVertex);
                vertexMap[originalVertex] = newVertex;
            }

            // 2. Восстанавливаем связи
            foreach (var originalVertex in this.Vertices)
            {
                var newVertex = vertexMap[originalVertex];

                // Копируем Next
                if (originalVertex.Next != null && vertexMap.TryGetValue(originalVertex.Next, out var nextCopy))
                {
                    newVertex.Next = nextCopy;
                }

                // Для условных вершин копируем LBS и RBS
                if (originalVertex is ConditionalVertex originalCond && newVertex is ConditionalVertex newCond)
                {
                    if (originalCond.LBS != null && vertexMap.TryGetValue(originalCond.LBS, out var lbsCopy))
                        newCond.LBS = lbsCopy;
                    
                    if (originalCond.RBS != null && vertexMap.TryGetValue(originalCond.RBS, out var rbsCopy))
                        newCond.RBS = rbsCopy;
                    
                    newCond.Value = originalCond.Value;
                }
            }

            // 3. Обновляем модель (строим формулы и т.д.)
            clone.Update();
            return clone;
        }

        private static IBDVertex CloneVertex(IBDVertex original)
        {
            return original switch
            {
                StartVertex => new StartVertex(),
                EndVertex => new EndVertex(),
                OperatorVertex op => new OperatorVertex(op.Index) { ID = op.ID },
                ConditionalVertex cond => new ConditionalVertex(cond.Prefix, cond.Index) { ID = cond.ID },
                JumpPoint jp => new JumpPoint(jp.JumpIndex) { ID = jp.ID },
                _ => throw new NotSupportedException($"Unknown vertex type: {original.GetType().Name}")
            };
        }

        #endregion



        public bool CheckCorrectness(out string message)
        {
            message = string.Empty;
            ParsingAggregateException? ex = null;
            bool correct = LASParser.TryParse(LAS, out ex);

            if (ex != null)
               message = ex.Message;

            return correct;
        }

        public void Update(bool UseJumpPoints = false)
        {
            // Сортировка вершин согласно универсальному сортировщику
            var order = new VertexTraversalOrder(Start, _vertices);
            var comparer = new VertexComparer(order);
            _vertices.Sort(comparer);

            // Исключаем переопределение компаратора для условных вершин, чтобы порядок в RunsInfo
            // всегда зависел только от ID (алфавитный порядок), а не от топологии графа.
            // Это критически важно для корректной работы Equals() при сравнении моделей с разной структурой, но одинаковой логикой.
            // ConditionalsBindingComparer = Comparer<ConditionalVertex>.Create((a, b) => comparer.Compare(a, b));

            _transitionFormulas = TransitionSystemBuilder.BuildAll(this, TransitionBuildMode.NoParadox);
            _mas = MatrixAlgorithmSchema.FromFormulas(_transitionFormulas);
            LAS = LAS == string.Empty ? AAToLASConverter.Convert(this, out _, false) : LAS;
            UpdateAllGraphFigures(UseJumpPoints);
        }

        /// <summary>
        /// Добавляет вершину в модель и регистрирует её в соответствующих индексах.
        /// </summary>
        public T AddVertex<T>(T vertex) where T : IBDVertex
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            _vertices.Add(vertex);
            if (!string.IsNullOrEmpty(vertex.ID))
            {
                if (!_byId.TryGetValue(vertex.ID!, out var list))
                {
                    list = new List<IBDVertex>();
                    _byId[vertex.ID!] = list;
                }
                list.Add(vertex);
            }

            switch (vertex)
            {
                case StartVertex s:
                    Start = s;
                    break;
                case EndVertex e:
                    End = e;
                    break;
                case JumpPoint jp:
                    _jumpPoints[jp.JumpIndex] = jp;
                    break;
            }

            return vertex;
        }

        /// <summary>
        /// Удаляет вершину из модели.
        /// </summary>
        public bool RemoveVertex(IBDVertex vertex)
        {
            if (vertex == null) return false;
            
            bool removed = _vertices.Remove(vertex);
            if (removed && !string.IsNullOrEmpty(vertex.ID))
            {
                if (_byId.TryGetValue(vertex.ID!, out var list))
                {
                    list.Remove(vertex);
                    if (list.Count == 0)
                        _byId.Remove(vertex.ID!);
                }
            }
            return removed;
        }

        /// <summary>
        /// Гарантированно возвращает точку перехода по индексу, создавая её при необходимости.
        /// </summary>
        public JumpPoint EnsureJumpPoint(int index)
        {
            if (!_jumpPoints.TryGetValue(index, out var jp))
            {
                jp = AddVertex(new JumpPoint(index));
            }
            return jp;
        }

        /// <summary>
        /// Пытается найти вершину по строковому идентификатору.
        /// </summary>
        public bool TryGetById(string id, out IBDVertex? vertex)
        {
            if (id == null)
            {
                vertex = null;
                return false;
            }
            if (_byId.TryGetValue(id, out var list) && list.Count > 0)
            {
                vertex = list[0];
                return true;
            }
            vertex = null;
            return false;
        }

        /// <summary>
        /// Устанавливает последующую вершину для указанной.
        /// </summary>
        public void LinkNext(IBDVertex from, IBDVertex to)
        {
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (to == null) throw new ArgumentNullException(nameof(to));
            from.Next = to;
        }

        /// <summary>
        /// Устанавливает ветви для условной вершины.
        /// </summary>
        public void SetConditionalBranches(ConditionalVertex vertex, IBDVertex? leftBranch, IBDVertex? rightBranch)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));
            vertex.LBS = leftBranch;
            vertex.RBS = rightBranch;
        }

        /// <summary>
        /// Возвращает следующую вершину для указанной, используя логику самой вершины.
        /// </summary>
        public IBDVertex? GetNext(IBDVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));
            return vertex.GetNext(this);
        }

        /// <summary>
        /// Полная очистка модели.
        /// </summary>
        public void Clear()
        {
            _vertices.Clear();
            _byId.Clear();
            _jumpPoints.Clear();
            _transitionFormulas.Clear();
            Start = null;
            End = null;
            _byLayer.Clear();
        }





        #region Обновление графовых метрик и агрегатов

        /// <summary>
        /// Массово обновляет метрики графа для всех вершин и пересобирает агрегаты слоёв/компонент.
        /// </summary>
        public void UpdateAllGraphFigures(bool UseJumpPoints = false)
        {
            // Присвоение компонент связности
            AssignComponents(UseJumpPoints);

            // Обновление индивидуальных метрик (исключая JumpPoint, если флаг не установлен)
            foreach (var fig in _vertices.OfType<IGraphFigure>().Where(f => UseJumpPoints || f is not JumpPoint))
                fig.UpdateGraphMetrics(this);

            // Пересборка агрегатов по слоям (исключая JumpPoint, если флаг не установлен)
            _byLayer.Clear();
            foreach (var fig in _vertices.OfType<IGraphFigure>().Where(f => UseJumpPoints || f is not JumpPoint))
            {
                if (!_byLayer.TryGetValue(fig.Layer, out var list))
                    _byLayer[fig.Layer] = list = new List<IGraphFigure>();
                list.Add(fig);
            }

            // Сортировка по IndexOnLayer, который должен быть установлен алгоритмом компоновки
            foreach (var kv in _byLayer)
            {
                kv.Value.Sort((a, b) => a.IndexOnLayer.CompareTo(b.IndexOnLayer));
            }
        }

        /// <summary>
        /// Делегирует компоновку внешнему алгоритму и пересобирает агрегаты.
        /// </summary>
        public void ApplyLayout(ILayoutAlgorithm layout, bool UseJumpPoints = false)
        {
            if (layout == null) throw new ArgumentNullException(nameof(layout));
            layout.Arrange(this, UseJumpPoints);
            UpdateAllGraphFigures(UseJumpPoints);
        }

        private static IEnumerable<IBDVertex> NextLayerNeighbors(IBDVertex v, bool UseJumpPoints = false)
        {
            IBDVertex? Skip(IBDVertex? x)
            {
                if (UseJumpPoints) return x;
                while (x is JumpPoint jp)
                    x = jp.GetNext(null);
                return x;
            }

            if (v is ConditionalVertex cv)
            {
                var l = Skip(cv.LBS);
                var r = Skip(cv.RBS);
                if (l != null) yield return l;
                if (r != null) yield return r;
            }
            else
            {
                var n = Skip(v.Next);
                if (n != null) yield return n;
            }
        }

        /// <summary>
        /// Присваивает идентификаторы компонент связности (SubgraphId) всем фигурам графа.
        /// </summary>
        private void AssignComponents(bool UseJumpPoints = false)
        {
            var figures = _vertices.Where(v => v is IGraphFigure && (UseJumpPoints || v is not JumpPoint)).Cast<IBDVertex>().ToList();
            var visited = new HashSet<IBDVertex>();
            int component = 0;

            foreach (var v in figures)
            {
                if (visited.Contains(v)) 
                    continue;

                string id = $"C{component++}";
                var stack = new Stack<IBDVertex>();
                stack.Push(v);
                while (stack.Count > 0)
                {
                    var cur = stack.Pop();
                    if (!visited.Add(cur)) 
                        continue;

                    if (cur is IGraphElement ge)
                        ge.SubgraphId = id;

                    foreach (var n in NextLayerNeighbors(cur, UseJumpPoints))
                        if (!visited.Contains(n)) 
                            stack.Push(n);
                }
            }
        }

        /// <summary>
        /// Назначает слой (Layer) для каждой фигуры по расстоянию от Start.
        /// </summary>
        private void AssignLayersFromStart(bool UseJumpPoints = false)
        {
            var allVertices = _vertices.Where(v => v is IGraphFigure && (UseJumpPoints || v is not JumpPoint)).Cast<IBDVertex>().ToList();
            foreach (var v in allVertices)
            {
                if (v is IGraphElement ge)
                    ge.Layer = 0; // по умолчанию
            }

            if (Start == null) return;

            var layer = new Dictionary<IBDVertex, int>();
            layer[Start] = 0;

            // Longest-path релаксация вдоль направленных рёбер
            int n = Math.Max(1, allVertices.Count);
            for (int iter = 0; iter < n; iter++)
            {
                bool changed = false;
                foreach (var v in allVertices)
                {
                    int lv = layer.TryGetValue(v, out var lvPrev) ? lvPrev : 0;
                    foreach (var w in NextLayerNeighbors(v, UseJumpPoints))
                    {
                        int newLayer = lv + 1;
                        if (!layer.TryGetValue(w, out var cur) || newLayer > cur)
                        {
                            layer[w] = newLayer;
                            changed = true;
                        }
                    }
                }
                if (!changed) break;
            }

            foreach (var v in allVertices)
            {
                if (v is IGraphElement ge)
                    ge.Layer = layer.TryGetValue(v, out int lv) ? lv : 0;
            }

            // Гарантируем, что конечная вершина имеет максимальный слой
            if (End != null && End is IGraphElement geEnd)
            {
                int maxL = layer.Values.DefaultIfEmpty(0).Max();
                geEnd.Layer = maxL;
            }
        }

        #endregion





        #region Методы настройки

        /// <summary>
        /// Сбрасывает значения условных вершин в неопределённое состояние.
        /// </summary>
        public void ResetConditions()
        {
            foreach (var v in _vertices.OfType<ConditionalVertex>())
                v.Value = null;
        }

        /// <summary>
        /// Устанавливает для условной вершины её <see cref="ConditionalVertex.Value"/>.
        /// </summary>
        /// <param name="xIndex">Индекс условной вершины.</param>
        /// <param name="value">Значение, которое примет <see cref="ConditionalVertex.Value"/></param>
        public void SetConditionalValue(string xID, bool? value)
        {
            var conds = Vertices.OfType<ConditionalVertex>().Where(x => x.ID == xID);
            foreach (var cond in conds)
                cond.Value = value;
        }

        /// <summary>
        /// Устанавливает для условной вершины её <see cref="ConditionalVertex.Value"/>.
        /// </summary>
        /// <param name="xIndex">Индекс условной вершины.</param>
        /// <param name="value">Значение, которое примет <see cref="ConditionalVertex.Value"/></param>
        public void SetConditionalValue(ConditionalVertex vRef, bool? value)
        {
            if (vRef != null && Vertices.Contains(vRef))
            {
                // Обновляем все копии этой вершины
                SetConditionalValue(vRef.ID ?? string.Empty, value);
            }
        }

        /// <summary>
        /// Устанавливает значения условных вершин на основе бинарной строки.
        /// Для дубликатов (вершин с одинаковым ID) устанавливается одно и то же значение.
        /// </summary>
        /// <param name="binaryValues">Строка, состоящая из n символов '0' и '1', где n равно количеству уникальных ID условных вершин.</param>
        public void SetConditionsFromBinary(string binaryValues)
        {
            // Группируем по ID, чтобы обрабатывать дубликаты как одну логическую переменную
            var conditionalGroups = Vertices
                .OfType<ConditionalVertex>()
                .GroupBy(v => v.ID)
                .OrderBy(g => g.First(), ConditionalsBindingComparer)
                .ToList();

            if (binaryValues == "[любой исход]" || (conditionalGroups.Count == 0))
                return;
        
            if (!Regex.IsMatch(binaryValues, @"^[01]+$"))
                throw new ArgumentException("Строка должна содержать только 0 и 1");
        
            if (binaryValues.Length != conditionalGroups.Count)
                throw new ArgumentException($"Ожидается {conditionalGroups.Count} символов (по числу уникальных условий), получено {binaryValues.Length}");
        
            for (int i = 0; i < conditionalGroups.Count; i++)
            {
                bool val = binaryValues[i] == '1';
                foreach (var v in conditionalGroups[i])
                {
                    v.Value = val;
                }
            }
        }

        #endregion





        #region Информация об алгоритме

        /// <summary>
        /// Симулирует ход работы алгоритма при текущих значениях условных вершин.
        /// Возвращает строку пути и (если найдено) строку цикла, начинающуюся и заканчивающуюся одной и той же вершиной.
        /// Цикл определяется первой вершиной, встреченной повторно.
        /// </summary>
        public (string Path, string? Cycle) SimulateRunForCurrentConditions(bool showPoints = false)
        {
            var tokens = new List<string>();
            var visited = new Dictionary<IBDVertex, int>();
            var start = Start;
            if (start == null)
                return ("[ошибка: отсутствует начальная вершина Yн]", null);

            tokens.Add(start.ID ?? "Yн");
            visited[start] = 0;

            IBDVertex? current = start;
            string? cycleStr = null;
            int maxSteps = Math.Max(1, Vertices.Count * 4);
            int steps = 0;

            while (current != null && steps++ < maxSteps)
            {
                var next = current.GetNext(this);
                if (next == null) break;

                if (showPoints || (!(next is JumpPoint) && !(next is ConditionalVertex)))
                    tokens.Add(next.ID ?? "?");

                if (visited.TryGetValue(next, out int idx))
                {
                    var cycleTokens = tokens.GetRange(idx, tokens.Count - idx);
                    cycleStr = string.Join(" → ", cycleTokens);
                    break;
                }

                visited[next] = tokens.Count - 1;

                if (next is EndVertex)
                    break;

                current = next;
            }

            var pathStr = string.Join(" → ", tokens);
            return (pathStr, cycleStr);
        }

        /// <summary>
        /// Возвращает строку хода работы алгоритма для заданной бинарной строки значений условных вершин.
        /// </summary>
        public string BuildRunPathForBinary(string binaryValues, bool showPoints = false)
        {
            SetConditionsFromBinary(binaryValues);
            var (path, _) = SimulateRunForCurrentConditions(showPoints);
            return path;
        }

        private IReadOnlyList<ConditionalVertex> GetConditionalsOrdered()
        {
            return Vertices
                .OfType<ConditionalVertex>()
                .DistinctBy(v => v.ID)
                .OrderBy(v => v, ConditionalsBindingComparer)
                .ToList();
        }

        private string BuildBinaryForMask(int mask)
        {
            var conds = GetConditionalsOrdered();
            if (conds.Count == 0) return "[любой исход]";
            var chars = new char[conds.Count];
            for (int i = 0; i < conds.Count; i++)
                chars[i] = ((mask >> i) & 1) == 1 ? '1' : '0';
            return new string(chars);
        }



        /// <summary>
        /// Находит все циклы для всех возможных комбинаций условий.
        /// </summary>
        public List<(string bin, string cycle)> FindCycles(bool showPoints = false)
        {
            var conds = GetConditionalsOrdered();
            int n = conds.Count;
            int total = n == 0 ? 1 : (1 << n);
            var cycles = new List<(string bin, string cycle)>();

            for (int mask = 0; mask < total; mask++)
            {
                string bin = BuildBinaryForMask(mask);
                SetConditionsFromBinary(bin);
                var (_, cycle) = SimulateRunForCurrentConditions(true);
                if (!string.IsNullOrEmpty(cycle))
                    cycles.Add((bin, cycle));
                ResetConditions();
            }

            return cycles;
        }

        /// <summary>
        /// Находит все запуски (ходы работы) для всех возможных комбинаций условий.
        /// </summary>
        public List<(string bin, string path)> FindRuns(bool showPoints = false)
        {
            var conds = GetConditionalsOrdered();
            int n = conds.Count;
            int total = n == 0 ? 1 : (1 << n);
            var runs = new List<(string bin, string path)>();

            for (int mask = 0; mask < total; mask++)
            {
                string bin = BuildBinaryForMask(mask);
                SetConditionsFromBinary(bin);
                var (path, _) = SimulateRunForCurrentConditions(showPoints);
                runs.Add((bin, path));
                ResetConditions();
            }

            return runs;
        }

        /// <summary>
        /// Проверяет достижимость всех вершин: каждая вершина должна встречаться хотя бы в одном ходе работы алгоритма
        /// для некоторого набора условий.
        /// </summary>
        public bool AreAllVerticesReachable(out List<string> unreachableVertexIds, bool showPoints = false)
        {
            unreachableVertexIds = new List<string>();

            // Собираем все ходы работы алгоритма для возможных комбинаций условий
            // Для проверки достижимости нужно видеть все вершины, включая точки перехода
            var runs = FindRuns(true);
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var (_, path) in runs)
            {
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                // Путь формируется как "Yн → X1 → ... → Yк"; выделяем идентификаторы вершин
                var parts = path.Split('→');
                foreach (var raw in parts)
                {
                    var id = raw.Trim();
                    if (id.Length == 0)
                        continue;
                    // Игнорируем возможные диагностические маркеры
                    if (id.StartsWith("[ошибка", StringComparison.Ordinal))
                        continue;
                    seen.Add(id);
                }
            }

            // Любая вершина модели должна появиться хотя бы в одном пути
            foreach (var v in Vertices)
            {
                var id = v.ID?.Trim();
                if (string.IsNullOrEmpty(id))
                    continue; // Безымянные вершины опускаем
                if (!seen.Contains(id))
                    unreachableVertexIds.Add(id);
            }

            return unreachableVertexIds.Count == 0;
        }

        /// <summary>
        /// Формирует полный текстовый отчёт по алгоритму: найденные циклы и ходы работы для всех комбинаций условий.
        /// </summary>
        public string BuildAlgorithmInfo(bool showPoints = false)
        {
            var lines = new List<string>();
            var lsa = LAS;

            // Заголовок
            lines.Add($"►► Информация об алгоритме \"{lsa}\" ◄◄");

            // Циклы
            var cyclesInfo = CyclesInfo;
            if (!string.IsNullOrWhiteSpace(cyclesInfo))
            {
                lines.Add("");
                lines.Add(cyclesInfo);
                lines.Add("◄");
                lines.Add("");
            }

            // Ходы работы
            var runsInfo = RunsInfo;
            if (!string.IsNullOrWhiteSpace(runsInfo))
            {
                lines.Add("");
                lines.Add(runsInfo);
                lines.Add("◄");
                lines.Add("");
            }

            // Формулы переходов и МСА
            var tfMasInfo = TransitionsAndMASInfo;
            if (!string.IsNullOrWhiteSpace(tfMasInfo))
            {
                lines.Add(tfMasInfo);
            }

            // Конец
            lines.Add($"►► Вывод информации об алгоритме \"{lsa}\" завершён ◄◄");

            return string.Join(Environment.NewLine, lines);
        }

        #endregion





        #region Эквивалентность

        public bool Equals(AbstractAutomata? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            // 1) Сравнение ходов работы алгоритма на основе текстового представления RunsInfo
            // Мы не проверяем совпадение множеств вершин (Vertices), так как функциональная эквивалентность
            // определяется поведением (выходными данными) при одинаковых входных данных.
            // Если вершина недостижима и не влияет на ход выполнения, алгоритмы всё равно считаются эквивалентными.
            
            // Начальная и конечная вершины: наличие должно совпадать
            if ((Start is null) != (other.Start is null)) return false;
            if ((End is null) != (other.End is null)) return false;

            var runsInfo1 = RunsInfo;
            var runsInfo2 = other.RunsInfo;
            if (!string.Equals(runsInfo1, runsInfo2, StringComparison.Ordinal))
                return false;

            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is AbstractAutomata other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hc = new HashCode();

            // Наборы операторных и условных вершин (по ID)
            var opIds = Vertices.OfType<OperatorVertex>().Select(v => v.ID ?? string.Empty).OrderBy(x => x, StringComparer.Ordinal).ToArray();
            hc.Add(opIds.Length);
            for (int i = 0; i < opIds.Length; i++) hc.Add(opIds[i]);

            var condIds = Vertices.OfType<ConditionalVertex>().Select(v => v.ID ?? string.Empty).Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray();
            hc.Add(condIds.Length);
            for (int i = 0; i < condIds.Length; i++) hc.Add(condIds[i]);

            // Наличие начальной и конечной вершин
            hc.Add(Start is not null);
            hc.Add(End is not null);

            // Ходы работы алгоритма (текстовое представление)
            var runsInfo = RunsInfo;
            hc.Add(runsInfo);
            return hc.ToHashCode();
        }

        #endregion
    }
}