using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AlgorithmDeveloper.AlgorithmModel;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;
using AlgorithmDeveloper.AlgorithmModel.Model.Vertices.Vizualization;

namespace AlgorithmDeveloper.AlgoDev.Model.Vertices.Graph.Layout
{
    /// <summary>
    /// Стратегия компоновки.
    /// </summary>
    public enum LayeringStrategy { ShortestPath, LongestPath }

    /// <summary>
    /// Алгоритм компоновки, реализующий компоновку Сугиямы с барицентрическим упорядочением.
    /// <br></br> - Назначает слои по длине пути от начала, пропуская точки перехода.
    /// <br></br> - Упорядочивает вершины на каждом слое, используя барицентрическую эвристику (проходы сверху вниз и снизу вверх).
    /// <br></br> - Вычисляет координаты центра для визуализации.
    /// </summary>
    public class SugiyamaLayoutAlgorithm : ILayoutAlgorithm
    {
        private readonly LayeringStrategy _strategy;
        private readonly bool _normalizeComponents;

        public SugiyamaLayoutAlgorithm(LayeringStrategy strategy = LayeringStrategy.ShortestPath, bool normalizeComponents = false)
        {
            _strategy = strategy;
            _normalizeComponents = normalizeComponents;
        }

        public void Arrange(AlgoModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var allVertices = model.Vertices.Where(v => v is IGraphFigure && v is not JumpPoint).Cast<IBDVertex>().ToList();
            if (allVertices.Count == 0)
                return;

            // Получает вершину, пропуская точки перехода.
            IBDVertex? SkipJump(IBDVertex? x)
            {
                while (x is JumpPoint jp)
                    x = jp.GetNext(model);
                return x;
            }

            // Получает дочерние вершины для данной, пропуская точки перехода.
            IEnumerable<IBDVertex> ChildrenNoJump(IBDVertex v)
            {
                if (v is ConditionalVertex cv)
                {
                    var l = SkipJump(cv.LBS);
                    var r = SkipJump(cv.RBS);
                    if (l != null) yield return l;
                    if (r != null) yield return r;
                }
                else
                {
                    var n = SkipJump(v.Next);
                    if (n != null) yield return n;
                }
            }

            // Получает родительские вершины для данной, пропуская точки перехода.
            IEnumerable<IBDVertex> ParentsNoJump(IBDVertex w)
            {
                foreach (var u in allVertices)
                {
                    foreach (var c in ChildrenNoJump(u))
                    {
                        if (ReferenceEquals(c, w))
                        {
                            yield return u;
                            break;
                        }
                    }
                }
            }



            // Компоновка по стратегии с нормализацией каждого компонента.
            var layer = new Dictionary<IBDVertex, int>();
            foreach (var v in allVertices)
            {
                if (v is IGraphElement ge)
                    ge.Layer = 0; // default
            }

            // Обнаружение циклов DAG (с пропуском точек перехода).
            bool DirectedHasCycle()
            {
                var color = new Dictionary<IBDVertex, int>(); // 0 = white, 1 = gray, 2 = black
                foreach (var v in allVertices) color[v] = 0;
                foreach (var v in allVertices)
                {
                    if (color[v] != 0) continue;
                    var stack = new Stack<(IBDVertex node, IEnumerator<IBDVertex> it)>();
                    color[v] = 1;
                    stack.Push((v, ChildrenNoJump(v).GetEnumerator()));
                    while (stack.Count > 0)
                    {
                        var (u, it) = stack.Peek();
                        if (it.MoveNext())
                        {
                            var w = it.Current;
                            if (!color.ContainsKey(w)) color[w] = 0;
                            if (color[w] == 0)
                            {
                                color[w] = 1;
                                stack.Push((w, ChildrenNoJump(w).GetEnumerator()));
                            }
                            else if (color[w] == 1)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            color[u] = 2;
                            stack.Pop();
                        }
                    }
                }
                return false;
            }

            void AssignShortestFromSeeds(IEnumerable<IBDVertex> seeds)
            {
                var q = new Queue<IBDVertex>();
                foreach (var s in seeds)
                {
                    if (!layer.ContainsKey(s))
                    {
                        layer[s] = 0;
                        q.Enqueue(s);
                    }
                }
                while (q.Count > 0)
                {
                    var v = q.Dequeue();
                    int lv = layer[v];
                    foreach (var w in ChildrenNoJump(v))
                    {
                        if (!layer.ContainsKey(w))
                        {
                            layer[w] = lv + 1;
                            q.Enqueue(w);
                        }
                        else if (lv + 1 < layer[w])
                        {
                            layer[w] = lv + 1;
                            q.Enqueue(w);
                        }
                    }
                }
            }

            void AssignLongestFromStart()
            {
                if (model.Start == null) return;
                layer[model.Start] = 0;
                int n = Math.Max(1, allVertices.Count);
                for (int iter = 0; iter < n; iter++)
                {
                    bool changed = false;
                    foreach (var v in allVertices)
                    {
                        int lv = layer.TryGetValue(v, out var lvPrev) ? lvPrev : 0;
                        foreach (var w in ChildrenNoJump(v))
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
            }

            // Выбираем стратегию компоновки
            if (_strategy == LayeringStrategy.LongestPath && DirectedHasCycle())
            {
                if (model.Start != null) AssignShortestFromSeeds(new[] { model.Start });
            }
            else
            {
                if (_strategy == LayeringStrategy.ShortestPath)
                {
                    if (model.Start != null) AssignShortestFromSeeds(new[] { model.Start });
                }
                else
                {
                    AssignLongestFromStart();
                }
            }

            // Дополнительно: нормализация и заполнение для каждого "отключенного" компонента
            if (_normalizeComponents)
            {
                // Ненаправленная смежность для обнаружения компонентов
                var und = new Dictionary<IBDVertex, List<IBDVertex>>();
                foreach (var v in allVertices) und[v] = new List<IBDVertex>();
                foreach (var v in allVertices)
                {
                    foreach (var w in ChildrenNoJump(v))
                    {
                        if (!und[v].Contains(w)) und[v].Add(w);
                        if (!und.ContainsKey(w)) und[w] = new List<IBDVertex>();
                        if (!und[w].Contains(v)) und[w].Add(v);
                    }
                }

                var visited = new HashSet<IBDVertex>();
                foreach (var v in allVertices)
                {
                    if (!visited.Add(v)) continue;
                    var comp = new List<IBDVertex>();
                    var s = new Stack<IBDVertex>();
                    s.Push(v);
                    while (s.Count > 0)
                    {
                        var u = s.Pop();
                        comp.Add(u);
                        foreach (var w in und[u])
                        {
                            if (visited.Add(w)) s.Push(w);
                        }
                    }

                    // Начальный локальный BFS для вершин, которые еще не разделены по слоям
                    var notAssigned = comp.Where(x => !layer.ContainsKey(x)).ToList();
                    if (notAssigned.Count > 0)
                    {
                        var sources = comp.Where(x => !ParentsNoJump(x).Any(p => comp.Contains(p))).ToList();
                        if (sources.Count == 0) sources = new List<IBDVertex> { comp[0] };
                        AssignShortestFromSeeds(sources);
                    }

                    // Сдвиг слоёв компонентов так, чтобы они начинались с 0
                    int minL = comp.Select(x => layer.TryGetValue(x, out var lv) ? lv : 0).DefaultIfEmpty(0).Min();
                    int shift = -minL;
                    foreach (var u in comp)
                    {
                        int cur = layer.TryGetValue(u, out var lv) ? lv : 0;
                        layer[u] = cur + shift;
                    }
                }
            }

            foreach (var v in allVertices)
            {
                if (v is IGraphElement ge)
                    ge.Layer = layer.TryGetValue(v, out int lv) ? lv : 0;
            }

            if (model.End != null && model.End is IGraphElement geEnd)
            {
                int maxOthers = allVertices
                    .Where(v => !ReferenceEquals(v, model.End))
                    .Select(v => layer.TryGetValue(v, out var lv) ? lv : 0)
                    .DefaultIfEmpty(0)
                    .Max();
                geEnd.Layer = maxOthers + 1;
            }

            var layers = allVertices
                .GroupBy(v => ((IGraphElement)v).Layer)
                .OrderBy(g => g.Key)
                .Select(g => g.ToList())
                .ToList();

            if (layers.Count == 0)
                return;

            

            double BarycenterFrom(Dictionary<IBDVertex, int> adjIndex, Func<IBDVertex, IEnumerable<IBDVertex>> getAdj, IBDVertex v, int defaultIdx)
            {
                var idxs = getAdj(v)
                    .Where(adjIndex.ContainsKey)
                    .Select(a => adjIndex[a])
                    .ToList();
                return idxs.Count == 0 ? defaultIdx : idxs.Average();
            }

            int passes = Math.Max(2, Math.Min(8, layers.Count * 2));
            for (int iter = 0; iter < passes; iter++)
            {
                for (int i = 1; i < layers.Count; i++)
                {
                    var upper = layers[i - 1];
                    var cur = layers[i];
                    var upperIndex = new Dictionary<IBDVertex, int>();
                    for (int k = 0; k < upper.Count; k++) upperIndex[upper[k]] = k;
                    var curIndex = new Dictionary<IBDVertex, int>();
                    for (int k = 0; k < cur.Count; k++) curIndex[cur[k]] = k;

                    cur.Sort((a, b) =>
                    {
                        double ba = BarycenterFrom(upperIndex, ParentsNoJump, a, curIndex[a]);
                        double bb = BarycenterFrom(upperIndex, ParentsNoJump, b, curIndex[b]);
                        int cmp = ba.CompareTo(bb);
                        if (cmp != 0) return cmp;
                        return curIndex[a].CompareTo(curIndex[b]);
                    });
                }

                for (int i = layers.Count - 2; i >= 0; i--)
                {
                    var lower = layers[i + 1];
                    var cur = layers[i];
                    var lowerIndex = new Dictionary<IBDVertex, int>();
                    for (int k = 0; k < lower.Count; k++) lowerIndex[lower[k]] = k;
                    var curIndex = new Dictionary<IBDVertex, int>();
                    for (int k = 0; k < cur.Count; k++) curIndex[cur[k]] = k;

                    cur.Sort((a, b) =>
                    {
                        double ba = BarycenterFrom(lowerIndex, ChildrenNoJump, a, curIndex[a]);
                        double bb = BarycenterFrom(lowerIndex, ChildrenNoJump, b, curIndex[b]);
                        int cmp = ba.CompareTo(bb);
                        if (cmp != 0) return cmp;
                        return curIndex[a].CompareTo(curIndex[b]);
                    });
                }
            }

            int horizontalSpacing = 150;
            int verticalSpacing = 100;
            int startX = 100;
            int startY = 100;

            for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
            {
                var verticesOnLayer = layers[layerIndex];
                int y = startY + layerIndex * verticalSpacing;

                int totalWidth = (verticesOnLayer.Count - 1) * horizontalSpacing;
                int offsetX = -totalWidth / 2;

                for (int i = 0; i < verticesOnLayer.Count; i++)
                {
                    var vertex = verticesOnLayer[i];
                    if (vertex is IGraphElement ge)
                        ge.IndexOnLayer = i;
                    var figure = (IFigure)vertex;
                    int x = startX + offsetX + i * horizontalSpacing + 200;
                    figure.Center = new Point(x, y);
                }
            }

            // Дополнительная пост-обработка раскладки:
            // 1) обеспечиваем ориентацию LBS строго слева и RBS строго справа на следующем слое;
            // 2) устраняем горизонтальные коллизии и рецентрируем слои;
            // 3) центрируем Yн и Yк на оси центра и выравниваем их по одной вертикали.
            int layerCenterX = startX + 200;

            void ReindexLayer(List<IBDVertex> list)
            {
                list.Sort((a, b) => ((IFigure)a).Center.X.CompareTo(((IFigure)b).Center.X));
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] is IGraphElement ge)
                        ge.IndexOnLayer = i;
                }
            }

            void ResolveCollisionsOnLayer(List<IBDVertex> list)
            {
                if (list.Count <= 1) return;
                list.Sort((a, b) => ((IFigure)a).Center.X.CompareTo(((IFigure)b).Center.X));
                float minDist = horizontalSpacing * 0.8f;
                for (int i = 1; i < list.Count; i++)
                {
                    var prevF = (IFigure)list[i - 1];
                    var curF = (IFigure)list[i];
                    float dx = curF.Center.X - prevF.Center.X;
                    if (dx < minDist)
                    {
                        int shift = (int)Math.Round(minDist - dx);
                        curF.Center = new Point(curF.Center.X + shift, curF.Center.Y);
                    }
                }

                // Рецентрируем слой вокруг глобального центра
                float avgX = (float)list.Average(v => ((IFigure)v).Center.X);
                int shiftAll = (int)Math.Round(layerCenterX - avgX);
                foreach (var v in list)
                {
                    var fig = (IFigure)v;
                    fig.Center = new Point(fig.Center.X + shiftAll, fig.Center.Y);
                }
                ReindexLayer(list);
            }

            // Ориентируем детей условных вершин на следующем слое
            for (int i = 0; i < layers.Count - 1; i++)
            {
                var parents = layers[i];
                var next = layers[i + 1];

                foreach (var v in parents)
                {
                    if (v is ConditionalVertex cv)
                    {
                        var parentFig = (IFigure)v;
                        int parentLayer = ((IGraphElement)v).Layer;

                        var l = SkipJump(cv.LBS);
                        var r = SkipJump(cv.RBS);

                        // Важно: если родитель на слое ПОСЛЕ дочерней — не трогаем ребёнка.
                        if (l is IGraphElement lge && lge.Layer > parentLayer && lge.Layer == parentLayer + 1)
                        {
                            var lf = (IFigure)l;
                            lf.Center = new Point(parentFig.Center.X - horizontalSpacing, lf.Center.Y);
                        }
                        if (r is IGraphElement rge && rge.Layer > parentLayer && rge.Layer == parentLayer + 1)
                        {
                            var rf = (IFigure)r;
                            rf.Center = new Point(parentFig.Center.X + horizontalSpacing, rf.Center.Y);
                        }
                    }
                }

                ResolveCollisionsOnLayer(next);
            }

            // Центрируем Yн и Yк на одной вертикали
            if (model.Start != null)
            {
                var fs = (IFigure)model.Start;
                fs.Center = new Point(layerCenterX, fs.Center.Y);
                var startLayerList = layers.FirstOrDefault(l => l.Contains(model.Start));
                if (startLayerList != null) ReindexLayer(startLayerList);
            }
            if (model.End != null)
            {
                var fe = (IFigure)model.End;
                fe.Center = new Point(layerCenterX, fe.Center.Y);
                var endLayerList = layers.FirstOrDefault(l => l.Contains(model.End));
                if (endLayerList != null) ReindexLayer(endLayerList);
            }
        }
    }
}