using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmDeveloper.AlgorithmModel;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;
using System.Text;

namespace AlgorithmDeveloper.AlgorithmModel.Model
{
    public sealed class CommonSubgraph
    {
        public IReadOnlyDictionary<IBDVertex, IBDVertex> MappingAtoB { get; }
        public AlgoModel SubgraphFromA { get; }
        public AlgoModel SubgraphFromB { get; }

        public CommonSubgraph(Dictionary<IBDVertex, IBDVertex> mapping, AlgoModel subA, AlgoModel subB)
        {
            MappingAtoB = mapping;
            SubgraphFromA = subA;
            SubgraphFromB = subB;
        }
    }

    public static class AlgoMcsUtils
    {
        public static string BuildCommonSubgraphsReport(AlgoModel aa1, AlgoModel aa2, bool strictLabels = false, int resultLimit = 5)
        {
            var results = FindMaximumCommonSubgraphs(aa1, aa2, strictLabels, resultLimit);
            var sb = new StringBuilder();
            sb.AppendLine($"Общие подграфы для алгоритмов \"{aa1.InitialLAS}\" и \"{aa2.InitialLAS}\":");
            sb.AppendLine("{");
            if (results.Count == 0)
            {
                sb.AppendLine("    [не найдено]");
            }
            else
            {
                for (int i = 0; i < results.Count; i++)
                {
                    var r = results[i];
                    var listA = FormatSignificantVerticesList(r.SubgraphFromA);
                    var listB = FormatSignificantVerticesList(r.SubgraphFromB);
                    sb.AppendLine($"    Подграф #{i + 1}:");
                    sb.AppendLine($"        A: [{listA}]");
                    sb.AppendLine($"        B: [{listB}]");
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string FormatSignificantVerticesList(AlgoModel model)
        {
            var ids = GetSignificantIdsInTraversalOrder(model);
            return string.Join(", ", ids);
        }

        private static List<string> GetSignificantIdsInTraversalOrder(AlgoModel model)
        {
            var order = new List<IBDVertex>();
            var visited = new HashSet<IBDVertex>();

            var start = model.Vertices.OfType<StartVertex>().FirstOrDefault();
            var queue = new Queue<IBDVertex>();

            if (start != null)
            {
                queue.Enqueue(start);
            }
            else
            {
                var roots = model.Vertices
                    .Where(v => v is IGraphFigure && v is not JumpPoint)
                    .Cast<IBDVertex>()
                    .Where(v => (v as IGraphElement)?.InDegree == 0)
                    .ToList();
                foreach (var r in roots)
                    queue.Enqueue(r);
            }

            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                if (!visited.Add(v))
                    continue;

                if (v is IGraphFigure && v is not JumpPoint)
                    order.Add(v);

                foreach (var n in GetChildrenLogical(v))
                    if (!visited.Contains(n))
                        queue.Enqueue(n);
            }

            foreach (var v in model.Vertices.Where(v => v is IGraphFigure && v is not JumpPoint).Cast<IBDVertex>())
                if (!visited.Contains(v))
                    order.Add(v);

            return order
                .Select(v => v.ID ?? string.Empty)
                .ToList();
        }

        public static List<CommonSubgraph> FindMaximumCommonSubgraphs(AlgoModel a, AlgoModel b, bool strictLabels = false, int resultLimit = 5)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            a.UpdateAllGraphFigures();
            b.UpdateAllGraphFigures();

            var nodesA = a.Vertices.OfType<IGraphFigure>().Cast<IBDVertex>().Where(IsSignificant).ToList();
            var nodesB = b.Vertices.OfType<IGraphFigure>().Cast<IBDVertex>().Where(IsSignificant).ToList();

            var bestMappings = new List<Dictionary<IBDVertex, IBDVertex>>();
            int bestSize = 0;

            var initialPairs = new List<(IBDVertex, IBDVertex)>();
            foreach (var va in nodesA)
                foreach (var vb in nodesB)
                    if (LabelCompatible(va, vb, strictLabels))
                        initialPairs.Add((va, vb));

            initialPairs = initialPairs
                .OrderByDescending(p => HeuristicScore(p.Item1, p.Item2))
                .ToList();

            foreach (var seed in initialPairs)
            {
                var mapping = new Dictionary<IBDVertex, IBDVertex>();
                var usedA = new HashSet<IBDVertex>();
                var usedB = new HashSet<IBDVertex>();
                ExtendMapping(seed.Item1, seed.Item2, a, b, strictLabels, mapping, usedA, usedB, ref bestMappings, ref bestSize);
            }

            var unique = new List<Dictionary<IBDVertex, IBDVertex>>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var m in bestMappings)
            {
                var key = MappingKey(m);
                if (seen.Add(key))
                    unique.Add(m);
            }

            var results = new List<CommonSubgraph>();
            foreach (var m in unique.Take(Math.Max(1, resultLimit)))
            {
                var subA = BuildSubgraph(a, new HashSet<IBDVertex>(m.Keys));
                var subB = BuildSubgraph(b, new HashSet<IBDVertex>(m.Values));
                results.Add(new CommonSubgraph(m, subA, subB));
            }

            return results;
        }



        private static void ExtendMapping
        (
            IBDVertex aNode,
            IBDVertex bNode,
            AlgoModel a,
            AlgoModel b,
            bool strictLabels,
            Dictionary<IBDVertex, IBDVertex> mapping,
            HashSet<IBDVertex> usedA,
            HashSet<IBDVertex> usedB,
            ref List<Dictionary<IBDVertex, IBDVertex>> bestMappings,
            ref int bestSize
        )
        {
            if (!LabelCompatible(aNode, bNode, strictLabels))
                return;

            if (usedA.Contains(aNode) || usedB.Contains(bNode))
                return;

            if (!Feasible(aNode, bNode, mapping))
                return;

            mapping[aNode] = bNode;
            usedA.Add(aNode);
            usedB.Add(bNode);

            if (mapping.Count > bestSize)
            {
                bestSize = mapping.Count;
                bestMappings.Clear();
                bestMappings.Add(new Dictionary<IBDVertex, IBDVertex>(mapping));
            }
            else if (mapping.Count == bestSize)
            {
                bestMappings.Add(new Dictionary<IBDVertex, IBDVertex>(mapping));
            }

            var frontierA = new HashSet<IBDVertex>();
            var frontierB = new HashSet<IBDVertex>();
            foreach (var kv in mapping)
            {
                foreach (var ch in GetChildrenLogical(kv.Key))
                    if (!usedA.Contains(ch))
                        frontierA.Add(ch);
                foreach (var ch in GetParentsLogical(kv.Key))
                    if (!usedA.Contains(ch))
                        frontierA.Add(ch);

                foreach (var ch in GetChildrenLogical(kv.Value))
                    if (!usedB.Contains(ch))
                        frontierB.Add(ch);
                foreach (var ch in GetParentsLogical(kv.Value))
                    if (!usedB.Contains(ch))
                        frontierB.Add(ch);
            }

            var candidates = new List<(IBDVertex, IBDVertex)>();
            foreach (var fa in frontierA)
            {
                if (!IsSignificant(fa)) continue;
                foreach (var fb in frontierB)
                {
                    if (!IsSignificant(fb)) continue;
                    if (LabelCompatible(fa, fb, strictLabels) && Feasible(fa, fb, mapping))
                        candidates.Add((fa, fb));
                }
            }

            candidates = candidates
                .OrderByDescending(p => HeuristicScore(p.Item1, p.Item2))
                .ToList();

            bool progressed = false;
            foreach (var c in candidates)
            {
                progressed = true;
                ExtendMapping(c.Item1, c.Item2, a, b, strictLabels, mapping, usedA, usedB, ref bestMappings, ref bestSize);
            }

            if (!progressed)
            {
                var remA = a.Vertices.Count - usedA.Count;
                var remB = b.Vertices.Count - usedB.Count;
                var bound = mapping.Count + Math.Min(remA, remB);
                if (bound < bestSize)
                {
                }
            }

            usedA.Remove(aNode);
            usedB.Remove(bNode);
            mapping.Remove(aNode);
        }

        private static bool LabelCompatible(IBDVertex a, IBDVertex b, bool strict)
        {
            if (a is StartVertex && b is StartVertex) return true;
            if (a is EndVertex && b is EndVertex) return true;
            if (a is OperatorVertex ao && b is OperatorVertex bo)
                return !strict || string.Equals(ao.ID ?? string.Empty, bo.ID ?? string.Empty, StringComparison.Ordinal);
            if (a is ConditionalVertex ac && b is ConditionalVertex bc)
            {
                if (!string.Equals(ac.Prefix ?? string.Empty, bc.Prefix ?? string.Empty, StringComparison.Ordinal))
                    return false;
                return !strict || string.Equals(ac.ID ?? string.Empty, bc.ID ?? string.Empty, StringComparison.Ordinal);
            }
            if (a is JumpPoint || b is JumpPoint)
                return false;
            return false;
        }

        private static bool Feasible(IBDVertex aNode, IBDVertex bNode, Dictionary<IBDVertex, IBDVertex> mapping)
        {
            foreach (var pa in GetParentsLogical(aNode))
            {
                if (mapping.TryGetValue(pa, out var pb))
                {
                    if (!GetChildrenLogical(pb).Contains(bNode))
                        return false;
                }
            }

            foreach (var ca in GetChildrenLogical(aNode))
            {
                if (mapping.TryGetValue(ca, out var cb))
                {
                    if (!GetChildrenLogical(bNode).Contains(cb))
                        return false;
                }
            }

            return true;
        }

        private static int HeuristicScore(IBDVertex a, IBDVertex b)
        {
            int da = DegreeScore(a);
            int db = DegreeScore(b);
            int dt = Math.Min(da, db);
            int typeBonus = TypeOrder(a) == TypeOrder(b) ? 1 : 0;
            return dt * 2 + typeBonus;
        }

        private static int DegreeScore(IBDVertex v)
        {
            if (v is IGraphElement ge)
                return ge.InDegree + ge.OutDegree;
            return 0;
        }

        private static int TypeOrder(IBDVertex v)
        {
            if (v is StartVertex) return 5;
            if (v is EndVertex) return 5;
            if (v is OperatorVertex) return 4;
            if (v is ConditionalVertex c)
                return string.Equals(c.Prefix ?? string.Empty, "X", StringComparison.Ordinal) ? 3 : 2;
            if (v is JumpPoint) return 1;
            return 0;
        }

        private static IEnumerable<IBDVertex> GetChildren(IBDVertex v)
        {
            if (v is ConditionalVertex cv)
            {
                if (cv.LBS != null) yield return cv.LBS;
                if (cv.RBS != null) yield return cv.RBS;
                yield break;
            }
            if (v.Next != null) yield return v.Next;
        }

        private static IEnumerable<IBDVertex> GetParents(IBDVertex v)
        {
            if (v is IGraphFigure gf)
            {
                return gf.Parents;
            }
            return Enumerable.Empty<IBDVertex>();
        }

        private static bool IsSignificant(IBDVertex v) => v is not JumpPoint;

        private static IBDVertex? SkipJump(IBDVertex? x)
        {
            while (x is JumpPoint jp)
                x = jp.GetNext(null);
            return x;
        }

        private static IEnumerable<IBDVertex> GetChildrenLogical(IBDVertex v)
        {
            if (v is ConditionalVertex cv)
            {
                var l = SkipJump(cv.LBS);
                var r = SkipJump(cv.RBS);
                if (l != null && IsSignificant(l)) yield return l;
                if (r != null && IsSignificant(r)) yield return r;
            }
            else
            {
                var n = SkipJump(v.Next);
                if (n != null && IsSignificant(n)) yield return n;
            }
        }

        private static IEnumerable<IBDVertex> GetParentsLogical(IBDVertex v)
        {
            var queue = new Queue<IBDVertex>();
            var visited = new HashSet<IBDVertex>();
            foreach (var p in GetParents(v))
                queue.Enqueue(p);
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                if (!visited.Add(cur)) continue;
                if (cur is JumpPoint)
                {
                    foreach (var pp in GetParents(cur))
                        queue.Enqueue(pp);
                }
                else if (IsSignificant(cur))
                {
                    yield return cur;
                }
            }
        }

        private static string MappingKey(Dictionary<IBDVertex, IBDVertex> mapping)
        {
            var pairs = mapping
                .OrderBy(kv => kv.Key.ID ?? string.Empty, StringComparer.Ordinal)
                .Select(kv => (kv.Key.ID ?? string.Empty) + "=" + (kv.Value.ID ?? string.Empty));
            return string.Join(";", pairs);
        }

        private static AlgoModel BuildSubgraph(AlgoModel original, HashSet<IBDVertex> nodes)
        {
            var model = new AlgoModel();
            var map = new Dictionary<IBDVertex, IBDVertex>();

            foreach (var v in original.Vertices)
            {
                if (!nodes.Contains(v))
                    continue;

                IBDVertex copy;
                if (v is StartVertex)
                {
                    copy = new StartVertex();
                }
                else if (v is EndVertex)
                {
                    copy = new EndVertex();
                }
                else if (v is OperatorVertex ov)
                {
                    copy = new OperatorVertex(ov.Index);
                }
                else if (v is ConditionalVertex cv)
                {
                    copy = new ConditionalVertex(cv.Prefix, cv.Index);
                }
                else if (v is JumpPoint jp)
                {
                    copy = new JumpPoint(jp.JumpIndex);
                }
                else
                {
                    continue;
                }

                model.AddVertex(copy);
                map[v] = copy;
            }

            foreach (var v in nodes)
            {
                if (!map.TryGetValue(v, out var cv))
                    continue;

                if (v is ConditionalVertex cOrig && cv is ConditionalVertex cCopy)
                {
                    var lFirst = BuildJumpChainFrom(model, cCopy, cOrig.LBS, map);
                    var rFirst = BuildJumpChainFrom(model, cCopy, cOrig.RBS, map);
                    model.SetConditionalBranches(cCopy, lFirst, rFirst);
                }
                else
                {
                    var first = BuildJumpChainFrom(model, cv, v.Next, map);
                    if (first != null)
                        model.LinkNext(cv, first);
                }
            }

            model.UpdateAllGraphFigures();
            return model;
        }

        private static IBDVertex? BuildJumpChainFrom(AlgoModel model, IBDVertex startCopy, IBDVertex? origNext, Dictionary<IBDVertex, IBDVertex> map)
        {
            IBDVertex? current = origNext;
            IBDVertex? prev = startCopy;
            IBDVertex? first = null;

            while (current is JumpPoint jp)
            {
                if (!map.TryGetValue(jp, out var jpCopy))
                {
                    jpCopy = new JumpPoint(jp.JumpIndex);
                    model.AddVertex(jpCopy);
                    map[jp] = jpCopy;
                }

                if (first == null) first = jpCopy;

                if (prev is not ConditionalVertex)
                    model.LinkNext(prev, jpCopy);

                prev = jpCopy;
                current = jp.GetNext(null);
            }

            if (current != null && map.TryGetValue(current, out var targetCopy))
            {
                if (prev is not ConditionalVertex)
                    model.LinkNext(prev, targetCopy);
                if (first == null) first = targetCopy;
            }

            return first;
        }
    }
}