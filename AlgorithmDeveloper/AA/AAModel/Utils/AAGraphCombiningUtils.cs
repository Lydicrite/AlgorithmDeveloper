using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;
using System.Text;
using AlgorithmDeveloper.AbstractAutomaton.AAModel.Utils;

namespace AlgorithmDeveloper.AAModel.Model.Utils
{
    public sealed class CommonSubgraph
    {
        public IReadOnlyDictionary<IBDVertex, IBDVertex> MappingAtoB { get; }
        public AbstractAutomata SubgraphFromA { get; }
        public AbstractAutomata SubgraphFromB { get; }

        public CommonSubgraph(Dictionary<IBDVertex, IBDVertex> mapping, AbstractAutomata subA, AbstractAutomata subB)
        {
            MappingAtoB = mapping;
            SubgraphFromA = subA;
            SubgraphFromB = subB;
        }
    }

    public static class AAGraphCombiningUtils
    {
        public static string BuildCommonSubgraphsReport(AbstractAutomata aa1, AbstractAutomata aa2, bool strictLabels = false, int resultLimit = 5)
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
                    sb.AppendLine($"        АА 1: [{listA}]");
                    sb.AppendLine($"        АА 2: [{listB}]");
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string FormatSignificantVerticesList(AbstractAutomata model)
        {
            var ids = GetSignificantIdsInTraversalOrder(model);
            return string.Join(", ", ids);
        }

        private static List<string> GetSignificantIdsInTraversalOrder(AbstractAutomata model)
        {
            // Use VertexSorter to get sorted vertices
            var order = new VertexTraversalOrder(model.Start, model.Vertices);
            var comparer = new VertexComparer(order);

            var sorted = model.Vertices
                .Where(v => v is IGraphFigure && v is not JumpPoint)
                .OrderBy(v => v, comparer)
                .ToList();

            return sorted
                .Select(v => v.ID ?? string.Empty)
                .ToList();
        }

        public static List<CommonSubgraph> FindMaximumCommonSubgraphs(AbstractAutomata a, AbstractAutomata b, bool strictLabels = false, int resultLimit = 5)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            a.UpdateAllGraphFigures();
            b.UpdateAllGraphFigures();

            var seqA = EnumerateNextChains(a);
            var seqB = EnumerateNextChains(b);

            var commonKeys = seqA.Keys.Intersect(seqB.Keys, StringComparer.Ordinal).ToList();
            var maximalKeys = FilterToMaximalKeys(commonKeys);
            var sortedKeys = SortKeysByAppearance(maximalKeys, a, b);

            var candidates = new List<(string key, List<IBDVertex> listA, List<IBDVertex> listB)>();
            foreach (var key in sortedKeys)
            {
                var baseA = seqA[key];
                var baseB = seqB[key];
                var (extA, extB) = ExpandWithMatchingBranchHeads(a, b, baseA, baseB);
                candidates.Add((key, extA, extB));
            }

            var filtered = FilterCandidatesBySetSuperset(candidates);

            var results = new List<CommonSubgraph>();
            foreach (var cand in filtered.Take(Math.Max(1, resultLimit)))
            {
                var mapping = new Dictionary<IBDVertex, IBDVertex>();
                var byIdB = cand.listB.ToDictionary(v => v.ID ?? string.Empty, v => v, StringComparer.Ordinal);
                foreach (var va in cand.listA)
                {
                    var id = va.ID ?? string.Empty;
                    if (byIdB.TryGetValue(id, out var vb))
                        mapping[va] = vb;
                }

                var subA = BuildSubgraph(a, new HashSet<IBDVertex>(cand.listA));
                var subB = BuildSubgraph(b, new HashSet<IBDVertex>(cand.listB));
                results.Add(new CommonSubgraph(mapping, subA, subB));
            }

            return results;
        }



        private static void ExtendMapping
        (
            IBDVertex aNode,
            IBDVertex bNode,
            AbstractAutomata a,
            AbstractAutomata b,
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

        private static Dictionary<string, List<IBDVertex>> EnumerateNextChains(AbstractAutomata model)
        {
            var significant = model.Vertices
                .Where(v => v is IGraphFigure && v is not JumpPoint)
                .Cast<IBDVertex>()
                .ToList();

            var dict = new Dictionary<string, List<IBDVertex>>(StringComparer.Ordinal);

            foreach (var start in significant)
            {
                var chain = BuildMaxNextChain(start);
                if (chain.Count == 0) continue;

                for (int i = 0; i < chain.Count; i++)
                {
                    for (int j = i; j < chain.Count; j++)
                    {
                        var sub = chain.GetRange(i, j - i + 1);
                        var key = string.Join("|", sub.Select(v => v.ID ?? string.Empty));
                        if (!dict.ContainsKey(key))
                            dict[key] = sub;
                    }
                }
            }

            return dict;
        }

        private static List<IBDVertex> BuildMaxNextChain(IBDVertex start)
        {
            var chain = new List<IBDVertex>();
            var visited = new HashSet<IBDVertex>();
            var cur = start;

            while (cur != null && !(cur is JumpPoint))
            {
                if (!visited.Add(cur))
                    break;

                chain.Add(cur);

                if (cur is ConditionalVertex)
                    break;

                cur = SkipJump(cur.Next);
            }

            return chain;
        }

        private static List<string> FilterToMaximalKeys(IEnumerable<string> keys)
        {
            var list = keys.Distinct(StringComparer.Ordinal).ToList();
            var items = list
                .Select(k => new { Key = k, Tokens = k.Split('|', StringSplitOptions.RemoveEmptyEntries) })
                .OrderByDescending(x => x.Tokens.Length)
                .ToList();

            var selected = new List<(string Key, string[] Tokens)>();
            foreach (var cand in items)
            {
                bool covered = selected.Any(big => IsSubarray(cand.Tokens, big.Tokens));
                if (!covered)
                    selected.Add((cand.Key, cand.Tokens));
            }

            return selected.Select(x => x.Key).ToList();
        }

        private static bool IsSubarray(string[] small, string[] big)
        {
            if (small.Length >= big.Length) return false;
            for (int i = 0; i <= big.Length - small.Length; i++)
            {
                bool eq = true;
                for (int j = 0; j < small.Length; j++)
                {
                    if (!string.Equals(small[j], big[i + j], StringComparison.Ordinal))
                    {
                        eq = false;
                        break;
                    }
                }
                if (eq) return true;
            }
            return false;
        }

        private static List<string> SortKeysByAppearance(IEnumerable<string> keys, AbstractAutomata a, AbstractAutomata b)
        {
            var orderA = GetSignificantIdsInTraversalOrder(a);
            var orderB = GetSignificantIdsInTraversalOrder(b);

            int FirstIndex(List<string> order, string[] seq)
            {
                for (int i = 0; i < order.Count; i++)
                {
                    if (string.Equals(order[i], seq[0], StringComparison.Ordinal))
                        return i;
                }
                return int.MaxValue;
            }

            return keys
                .Select(k => new { Key = k, Tokens = k.Split('|', StringSplitOptions.RemoveEmptyEntries) })
                .OrderBy(x => FirstIndex(orderA, x.Tokens))
                .ThenBy(x => FirstIndex(orderB, x.Tokens))
                .ThenBy(x => x.Tokens.Length)
                .Select(x => x.Key)
                .ToList();
        }

        private static (List<IBDVertex> aList, List<IBDVertex> bList) ExpandWithMatchingBranchHeads(AbstractAutomata a, AbstractAutomata b, List<IBDVertex> baseA, List<IBDVertex> baseB)
        {
            var setA = new Dictionary<string, IBDVertex>(StringComparer.Ordinal);
            var setB = new Dictionary<string, IBDVertex>(StringComparer.Ordinal);
            foreach (var v in baseA)
            {
                var id = v.ID ?? string.Empty;
                if (!setA.ContainsKey(id)) setA[id] = v;
            }
            foreach (var v in baseB)
            {
                var id = v.ID ?? string.Empty;
                if (!setB.ContainsKey(id)) setB[id] = v;
            }

            int n = Math.Min(baseA.Count, baseB.Count);
            for (int i = 0; i < n; i++)
            {
                if (baseA[i] is ConditionalVertex ca && baseB[i] is ConditionalVertex cb)
                {
                    if (!string.Equals(ca.Prefix, cb.Prefix, StringComparison.Ordinal))
                        continue;
                    if (!string.Equals(ca.ID ?? string.Empty, cb.ID ?? string.Empty, StringComparison.Ordinal))
                        continue;

                    var la = SkipJump(ca.LBS);
                    var lb = SkipJump(cb.LBS);
                    if (la != null && lb != null && IsSignificant(la) && IsSignificant(lb))
                    {
                        var lid = la.ID ?? string.Empty;
                        var lidB = lb.ID ?? string.Empty;
                        if (string.Equals(lid, lidB, StringComparison.Ordinal))
                        {
                            if (!setA.ContainsKey(lid)) setA[lid] = la;
                            if (!setB.ContainsKey(lid)) setB[lid] = lb;
                        }
                    }

                    var ra = SkipJump(ca.RBS);
                    var rb = SkipJump(cb.RBS);
                    if (ra != null && rb != null && IsSignificant(ra) && IsSignificant(rb))
                    {
                        var rid = ra.ID ?? string.Empty;
                        var ridB = rb.ID ?? string.Empty;
                        if (string.Equals(rid, ridB, StringComparison.Ordinal))
                        {
                            if (!setA.ContainsKey(rid)) setA[rid] = ra;
                            if (!setB.ContainsKey(rid)) setB[rid] = rb;
                        }
                    }
                }
            }

            var listA = baseA.ToList();
            var listB = baseB.ToList();
            foreach (var kv in setA)
                if (!listA.Contains(kv.Value)) listA.Add(kv.Value);
            foreach (var kv in setB)
                if (!listB.Contains(kv.Value)) listB.Add(kv.Value);

            return (listA, listB);
        }

        private static List<(string key, List<IBDVertex> listA, List<IBDVertex> listB)> FilterCandidatesBySetSuperset(List<(string key, List<IBDVertex> listA, List<IBDVertex> listB)> candidates)
        {
            var idA = candidates.Select(c => new HashSet<string>(c.listA.Select(v => v.ID ?? string.Empty), StringComparer.Ordinal)).ToList();
            var idB = candidates.Select(c => new HashSet<string>(c.listB.Select(v => v.ID ?? string.Empty), StringComparer.Ordinal)).ToList();

            bool IsSubset(HashSet<string> small, HashSet<string> big)
            {
                if (small.Count >= big.Count) return false;
                foreach (var s in small) if (!big.Contains(s)) return false;
                return true;
            }

            var keep = new bool[candidates.Count];
            for (int i = 0; i < keep.Length; i++) keep[i] = true;

            for (int i = 0; i < candidates.Count; i++)
            {
                if (!keep[i]) continue;
                for (int j = 0; j < candidates.Count; j++)
                {
                    if (i == j || !keep[j]) continue;
                    if (IsSubset(idA[j], idA[i]) && IsSubset(idB[j], idB[i]))
                    {
                        keep[j] = false;
                    }
                }
            }

            var result = new List<(string key, List<IBDVertex> listA, List<IBDVertex> listB)>();
            for (int i = 0; i < candidates.Count; i++)
                if (keep[i]) result.Add(candidates[i]);
            return result;
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

        private static AbstractAutomata BuildSubgraph(AbstractAutomata original, HashSet<IBDVertex> nodes)
        {
            var model = new AbstractAutomata();
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

        private static IBDVertex? BuildJumpChainFrom(AbstractAutomata model, IBDVertex startCopy, IBDVertex? origNext, Dictionary<IBDVertex, IBDVertex> map)
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
