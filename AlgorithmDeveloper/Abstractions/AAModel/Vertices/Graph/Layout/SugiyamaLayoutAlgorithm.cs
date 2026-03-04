using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph.Layout
{
    public enum LayeringStrategy { ShortestPath, LongestPath }

    public class SugiyamaLayoutAlgorithm : ILayoutAlgorithm
    {
        private readonly LayeringStrategy _strategy;
        private readonly bool _normalizeComponents;
        private readonly int _hSpacing;
        private readonly int _vSpacing;
        private readonly int _startX;
        private readonly int _startY;

        public SugiyamaLayoutAlgorithm(LayeringStrategy strategy = LayeringStrategy.ShortestPath, bool normalizeComponents = false)
        {
            _strategy = strategy;
            _normalizeComponents = normalizeComponents;
            _hSpacing = 150;
            _vSpacing = 100;
            _startX = 100;
            _startY = 100;
        }

        public SugiyamaLayoutAlgorithm(LayeringStrategy strategy, bool normalizeComponents, int horizontalSpacing, int verticalSpacing, int startX, int startY)
        {
            _strategy = strategy;
            _normalizeComponents = normalizeComponents;
            _hSpacing = Math.Max(40, horizontalSpacing);
            _vSpacing = Math.Max(40, verticalSpacing);
            _startX = startX;
            _startY = startY;
        }

        private class DummyVertex : BDVertex
        {
            public DummyVertex()
            {
                IsDummy = true;
                BuildGeometry();
            }

            public override string Description => "Dummy";
            public override IBDVertex? GetNext(AbstractAutomata? model = null) => Next;

            protected override void BuildGeometry()
            {
                _gridGeometry = new GridGeometry(FigureShape.Rectangle, Enumerable.Empty<Point>(), Enumerable.Empty<Point>());
            }
        }

        public void Arrange(AbstractAutomata model, bool UseJumpPoints = false)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var allVertices = model.Vertices.Where(v => v is IGraphFigure && (UseJumpPoints || v is not JumpPoint)).Cast<IBDVertex>().ToList();
            if (allVertices.Count == 0)
                return;

            IBDVertex? SkipJump(IBDVertex? x)
            {
                if (UseJumpPoints) return x;
                int safety = 0;
                while (x is JumpPoint jp)
                {
                    x = jp.GetNext(model);
                    if (++safety > 1000) break; // Protect against cyclic JumpPoints
                }
                return x;
            }

            // --- 1. Helper Functions for Graph Traversal ---

            IEnumerable<IBDVertex> ChildrenNoJump(IBDVertex v)
            {
                if (v is ConditionalVertex cv)
                {
                    var l = SkipJump(cv.LBS);
                    var r = SkipJump(cv.RBS);
                    // Order matters for symmetry: Left then Right
                    if (l != null) yield return l;
                    if (r != null) yield return r;
                }
                else
                {
                    var n = SkipJump(v.Next);
                    if (n != null) yield return n;
                }
            }

            // --- 2. Layering (Assign Y-Coordinates) ---
            
            // Detect back-edges to break cycles for layering
            var backEdges = new HashSet<(IBDVertex From, IBDVertex To)>();
            var visitedStates = new Dictionary<IBDVertex, int>(); // 0=White, 1=Gray, 2=Black

            void DetectBackEdges(IBDVertex u)
            {
                visitedStates[u] = 1; // Gray
                foreach (var v in ChildrenNoJump(u))
                {
                    if (!visitedStates.ContainsKey(v) || visitedStates[v] == 0)
                    {
                        DetectBackEdges(v);
                    }
                    else if (visitedStates[v] == 1)
                    {
                        // Cycle detected: u -> v is a back-edge
                        backEdges.Add((u, v));
                    }
                }
                visitedStates[u] = 2; // Black
            }

            if (model.Start != null)
                DetectBackEdges(model.Start);
            foreach (var v in allVertices)
            {
                if (!visitedStates.ContainsKey(v)) DetectBackEdges(v);
            }

            var layer = new Dictionary<IBDVertex, int>();
            foreach (var v in allVertices)
            {
                if (v is IGraphElement ge)
                    ge.Layer = 0; // default
            }

            void AssignLayers()
            {
                // Initialize seeds (roots)
                var q = new Queue<IBDVertex>();
                var degrees = new Dictionary<IBDVertex, int>();
                
                // Calculate in-degrees considering only forward edges
                foreach (var v in allVertices) degrees[v] = 0;
                foreach (var u in allVertices)
                {
                    foreach (var v in ChildrenNoJump(u))
                    {
                        if (backEdges.Contains((u, v))) continue;
                        degrees[v]++;
                    }
                }

                // Enqueue sources
                foreach (var v in allVertices)
                {
                    if (degrees[v] == 0)
                    {
                        layer[v] = 0;
                        q.Enqueue(v);
                    }
                }
                
                // If no sources (pure cycle), pick Start or first
                if (q.Count == 0 && allVertices.Count > 0)
                {
                    var start = model.Start ?? allVertices[0];
                    layer[start] = 0;
                    q.Enqueue(start);
                }

                while (q.Count > 0)
                {
                    var u = q.Dequeue();
                    int uLayer = layer.ContainsKey(u) ? layer[u] : 0;

                    foreach (var v in ChildrenNoJump(u))
                    {
                        if (backEdges.Contains((u, v))) continue;

                        // Longest path logic: layer[v] = max(layer[v], layer[u] + 1)
                        int newLayer = uLayer + 1;
                        if (!layer.ContainsKey(v) || newLayer > layer[v])
                        {
                            layer[v] = newLayer;
                        }

                        degrees[v]--;
                        if (degrees[v] == 0)
                        {
                            q.Enqueue(v);
                        }
                    }
                }

                // Handle remaining nodes in cycles (if any weren't reached)
                // Just BFS them to assign some layer
                foreach (var v in allVertices)
                {
                    if (!layer.ContainsKey(v))
                    {
                        // Assign based on parents if possible
                         int maxP = 0;
                         foreach (var p in allVertices) 
                            if (ChildrenNoJump(p).Contains(v) && layer.ContainsKey(p))
                                maxP = Math.Max(maxP, layer[p]);
                         layer[v] = maxP + 1;
                    }
                }
            }

            AssignLayers();

            // Apply layers to GraphElements
            foreach (var v in allVertices)
            {
                if (v is IGraphElement ge)
                    ge.Layer = layer.TryGetValue(v, out int lv) ? lv : 0;
            }

            // Ensure End vertex is at the bottom
            if (model.End != null && model.End is IGraphElement geEnd)
            {
                int maxLayer = allVertices.Max(v => layer.TryGetValue(v, out int l) ? l : 0);
                geEnd.Layer = maxLayer + 1;
            }

            // --- 3. Tree-Based Layout (Assign X-Coordinates) ---
            
            // 3.1. Build Spanning Tree
            // We assign each node a "Tree Parent". If a node has multiple parents (merge),
            // it is assigned to the one that is "closest" or "most logical" to maintain flow.
            // For symmetry, we prioritize Conditional parents claiming their branches.
            
            var treeChildren = new Dictionary<IBDVertex, List<IBDVertex>>();
            var treeParent = new Dictionary<IBDVertex, IBDVertex>();
            var visited = new HashSet<IBDVertex>();

            foreach (var v in allVertices) treeChildren[v] = new List<IBDVertex>();

            // Queue for BFS tree construction to ensure top-down growth
            var buildQueue = new Queue<IBDVertex>();
            if (model.Start != null)
            {
                buildQueue.Enqueue(model.Start);
                visited.Add(model.Start);
            }
            else
            {
                // Fallback for fragments
                foreach (var v in allVertices.OrderBy(v => ((IGraphElement)v).Layer))
                {
                    if (visited.Add(v)) buildQueue.Enqueue(v);
                }
            }
            
            // Iterate layers to pick up disconnected components
            var sortedByLayer = allVertices.OrderBy(v => ((IGraphElement)v).Layer).ToList();

            foreach (var rootCandidate in sortedByLayer)
            {
                if (!visited.Contains(rootCandidate))
                {
                    visited.Add(rootCandidate);
                    buildQueue.Enqueue(rootCandidate);
                }

                while (buildQueue.Count > 0)
                {
                    var u = buildQueue.Dequeue();
                    int uLayer = ((IGraphElement)u).Layer;

                    // Get children strictly from the next layers (ignore back-edges)
                    // We preserve the order of children (LBS then RBS) for layout symmetry
                    // But for optimization, we can swap LBS/RBS if it helps reduce edge crossing or length
                    // Here we can reorder children based on their relative position in the next layer?
                    // For now, let's stick to logical order: LBS (Left/False) then RBS (Right/True).
                    // This is handled by ChildrenNoJump yielding LBS then RBS.
                    
                    // Note: If we want to allow swapping, we should check which child is "heavier" or
                    // based on barycenter. But standard Sugiyama does crossing reduction later.
                    // Since we do tree-based layout, the order in 'treeChildren' determines left-to-right order.
                    
                    foreach (var v in ChildrenNoJump(u))
                    {
                        int vLayer = ((IGraphElement)v).Layer;
                        if (vLayer <= uLayer) continue; // Ignore back-edges and intra-layer edges for tree structure

                        if (!visited.Contains(v))
                        {
                            visited.Add(v);
                            treeChildren[u].Add(v);
                            treeParent[v] = u;
                            buildQueue.Enqueue(v);
                        }
                    }
                    
                    // Optimization: Swap branches if LBS is actually "heavier" or should be on the right?
                    // Or if we want strictly False=Left, True=Right.
                    // Currently ChildrenNoJump yields LBS(False), then RBS(True).
                    // So treeChildren[u] has [LBS, RBS].
                    // The layout engine places them left-to-right.
                    // So LBS is on the Left, RBS is on the Right. This matches the convention "False=Left/Bottom, True=Right/Bottom".
                    // If we want to support dynamic swapping, we need to change this order and update EdgeType logic accordingly?
                    // But EdgeType is fixed in the model (LBS is always FalseBranch).
                    // So we must ensure LBS is visually on the left or bottom-left.
                    // The current order [LBS, RBS] ensures LBS is placed to the left of RBS.
                    // So NO swapping should be done if we want to maintain False=Left standard.
                }
            }

            // 3.2. Insert Dummy Vertices for Long Edges
            // This prevents long vertical edges from intersecting with nodes on intermediate layers.
            var dummyVertices = new List<IBDVertex>();
            
            // We iterate a copy of vertices because we might modify treeChildren
            foreach (var u in allVertices.ToList())
            {
                var children = ChildrenNoJump(u).ToList();
                int uLayer = ((IGraphElement)u).Layer;

                foreach (var v in children)
                {
                    int vLayer = ((IGraphElement)v).Layer;
                    // Only forward edges
                    if (vLayer <= uLayer + 1) continue;

                    // It's a long edge (Layer diff > 1)
                    // We need dummies on layers: uLayer+1, ..., vLayer-1
                    
                    IBDVertex? prev = u;
                    IBDVertex? firstDummy = null;
                    IBDVertex? lastDummy = null;

                    for (int l = uLayer + 1; l < vLayer; l++)
                    {
                        var dummy = new DummyVertex
                        {
                            Layer = l,
                            IndexOnLayer = -1, // Will be set later if needed, but not critical
                            Center = Point.Empty // Will be set by AssignX
                        };
                        dummyVertices.Add(dummy);
                        treeChildren[dummy] = new List<IBDVertex>(); // Initialize children list for dummy
                        
                        if (firstDummy == null) firstDummy = dummy;
                        
                        // Link previous dummy to current
                        if (prev is DummyVertex dp)
                        {
                            treeChildren[dp].Add(dummy);
                            treeParent[dummy] = dp;
                        }
                        
                        prev = dummy;
                        lastDummy = dummy;
                    }

                    // Integrate first dummy into tree
                    if (firstDummy != null)
                    {
                        // Check if v was a direct tree child of u
                        if (treeChildren[u].Contains(v))
                        {
                            // Replace v with firstDummy
                            int idx = treeChildren[u].IndexOf(v);
                            treeChildren[u][idx] = firstDummy;
                            treeParent[firstDummy] = u;

                            // Link lastDummy to v
                            if (lastDummy != null)
                            {
                                treeChildren[lastDummy].Add(v);
                                treeParent[v] = lastDummy;
                            }
                        }
                        else
                        {
                            // v is not a tree child (cross edge).
                            // We still attach the dummy chain to u to reserve space under u.
                            // But the chain ends at lastDummy (dangling).
                            treeParent[firstDummy] = u;
                            
                            // Where to insert?
                            // If u is Conditional:
                            // Left Branch (False) -> Insert at 0
                            // Right Branch (True) -> Insert at end
                            bool isFalseBranch = false;
                            if (u is ConditionalVertex cv && cv.LBS == v) isFalseBranch = true;
                            // Note: We use original v comparison. If JumpPoints used, v is resolved.
                            // cv.LBS might point to JumpPoint, but v is the resolved target.
                            // So better logic: if v is the "Left" child.
                            
                            // Using the order in ChildrenNoJump: Left is first.
                            // But here we iterate children list.
                            // If u is Conditional, children[0] is Left, children[1] is Right (if exists).
                            
                            if (u is ConditionalVertex && children.Count > 0 && children[0] == v)
                            {
                                // Insert at beginning
                                treeChildren[u].Insert(0, firstDummy);
                            }
                            else
                            {
                                // Insert at end
                                treeChildren[u].Add(firstDummy);
                            }
                        }
                    }
                }
            }
            
            allVertices.AddRange(dummyVertices);

            // 3.3. Calculate Subtree Widths
            // Width(u) = Sum(Width(children)) + Spacing
            var nodeWidths = new Dictionary<IBDVertex, float>();

            float GetSubtreeWidth(IBDVertex u)
            {
                if (nodeWidths.TryGetValue(u, out float w)) return w;

                var children = treeChildren[u];
                if (children.Count == 0)
                {
                    // Base width for a single node
                    w = _hSpacing; 
                }
                else
                {
                    float childrenWidth = 0;
                    foreach (var child in children)
                    {
                        childrenWidth += GetSubtreeWidth(child);
                    }
                    // Add spacing between children
                    childrenWidth += (children.Count - 1) * (_hSpacing * 0.5f); // Slightly tighter spacing between branches
                    
                    // The node itself needs space, but usually the subtree is wider
                    w = Math.Max(_hSpacing, childrenWidth);
                }

                nodeWidths[u] = w;
                return w;
            }

            // Calculate widths for all roots (nodes with no tree parent)
            var roots = allVertices.Where(v => !treeParent.ContainsKey(v)).ToList();
            foreach (var root in roots) GetSubtreeWidth(root);

            // 3.3. Assign X Coordinates Recursively
            void AssignX(IBDVertex u, float xCenter)
            {
                // Set position
                var fig = (IFigure)u;
                int y = _startY + ((IGraphElement)u).Layer * _vSpacing;
                fig.Center = new Point((int)xCenter, y);

                var children = treeChildren[u];
                if (children.Count == 0) return;

                // Calculate total width of children to center them
                float totalChildrenWidth = 0;
                foreach (var child in children)
                {
                    totalChildrenWidth += nodeWidths[child];
                }
                totalChildrenWidth += (children.Count - 1) * (_hSpacing * 0.5f);

                // Start position (leftmost edge of the children block)
                float currentX = xCenter - totalChildrenWidth / 2f;

                foreach (var child in children)
                {
                    float childW = nodeWidths[child];
                    // Center of the child is: CurrentStart + ChildWidth/2
                    AssignX(child, currentX + childW / 2f);
                    // Move start for next child
                    currentX += childW + (_hSpacing * 0.5f);
                }
            }

            // Layout each disconnected component (root)
            // We arrange roots horizontally
            float currentRootX = _startX + 200;
            foreach (var root in roots)
            {
                float w = nodeWidths[root];
                AssignX(root, currentRootX + w / 2f);
                currentRootX += w + _hSpacing;
            }

            // --- 3.4. Balancing (Centering Merge Nodes) ---
            // Try to center nodes that have multiple parents (merge nodes)
            // relative to their parents, shifting their entire subtree if possible.
            
            // Re-build parent map for graph (to identify merge nodes)
            var graphParents = new Dictionary<IBDVertex, List<IBDVertex>>();
            foreach(var v in allVertices) graphParents[v] = new List<IBDVertex>();
            foreach(var u in allVertices)
            {
                 foreach(var v in ChildrenNoJump(u))
                 {
                     if (((IGraphElement)v).Layer > ((IGraphElement)u).Layer) // Only forward edges
                        graphParents[v].Add(u);
                 }
            }

            // Build descendants map (Subtree lookup)
            var treeDescendants = new Dictionary<IBDVertex, HashSet<IBDVertex>>();
            void BuildDescendants(IBDVertex u)
            {
                if (treeDescendants.ContainsKey(u)) return;
                var set = new HashSet<IBDVertex>();
                set.Add(u);
                foreach(var child in treeChildren[u])
                {
                    BuildDescendants(child);
                    set.UnionWith(treeDescendants[child]);
                }
                treeDescendants[u] = set;
            }
            foreach(var root in roots) BuildDescendants(root);

            // Layer-wise processing (Top-Down)
            int balancingMaxLayer = 0;
            if (allVertices.Count > 0) balancingMaxLayer = allVertices.Max(v => ((IGraphElement)v).Layer);
            
            for (int l = 1; l <= balancingMaxLayer; l++)
            {
                var layerNodes = allVertices.Where(v => ((IGraphElement)v).Layer == l).ToList();
                // Sort by X
                layerNodes.Sort((a, b) => ((IFigure)a).Center.X.CompareTo(((IFigure)b).Center.X));
                
                foreach (var v in layerNodes)
                {
                    var parents = graphParents[v];
                    if (parents.Count < 2) continue; // Not a merge node
                    
                    float avgParentX = (float)parents.Average(p => ((IFigure)p).Center.X);
                    int currentX = ((IFigure)v).Center.X;
                    int shift = (int)(avgParentX - currentX);
                    
                    if (Math.Abs(shift) < 5) continue;

                    if (!treeDescendants.ContainsKey(v)) continue;
                    var subtree = treeDescendants[v];

                    // Check limits
                    var subtreeByLayer = subtree.GroupBy(n => ((IGraphElement)n).Layer);
                    
                    int allowedShift = shift;
                    
                    if (shift < 0) // Move Left
                    {
                         int maxMove = int.MaxValue;
                         foreach(var grp in subtreeByLayer)
                         {
                             int layerIdx = grp.Key;
                             int minX = grp.Min(n => ((IFigure)n).Center.X);
                             
                             // Find nearest left neighbor in this layer that is NOT in subtree
                             var layerAll = allVertices.Where(n => ((IGraphElement)n).Layer == layerIdx);
                             int nearestLeftX = int.MinValue;
                             
                             foreach(var n in layerAll)
                             {
                                 if (subtree.Contains(n)) continue;
                                 int nx = ((IFigure)n).Center.X;
                                 if (nx < minX && nx > nearestLeftX) nearestLeftX = nx;
                             }
                             
                             if (nearestLeftX > int.MinValue)
                             {
                                 int space = (minX - nearestLeftX) - _hSpacing;
                                 if (space < maxMove) maxMove = space;
                             }
                         }
                         if (maxMove < 0) maxMove = 0;
                         if (-allowedShift > maxMove) allowedShift = -maxMove;
                    }
                    else // Move Right
                    {
                         int maxMove = int.MaxValue;
                         foreach(var grp in subtreeByLayer)
                         {
                             int layerIdx = grp.Key;
                             int maxX = grp.Max(n => ((IFigure)n).Center.X);
                             
                             var layerAll = allVertices.Where(n => ((IGraphElement)n).Layer == layerIdx);
                             int nearestRightX = int.MaxValue;
                             
                             foreach(var n in layerAll)
                             {
                                 if (subtree.Contains(n)) continue;
                                 int nx = ((IFigure)n).Center.X;
                                 if (nx > maxX && nx < nearestRightX) nearestRightX = nx;
                             }
                             
                             if (nearestRightX < int.MaxValue)
                             {
                                 int space = (nearestRightX - maxX) - _hSpacing;
                                 if (space < maxMove) maxMove = space;
                             }
                         }
                         if (maxMove < 0) maxMove = 0;
                         if (allowedShift > maxMove) allowedShift = maxMove;
                    }
                    
                    if (Math.Abs(allowedShift) > 0)
                    {
                        foreach(var n in subtree)
                        {
                            if (n is DummyVertex) continue; // Skip moving dummies, they are placeholders
                            var fig = (IFigure)n;
                            fig.Center = new Point(fig.Center.X + allowedShift, fig.Center.Y);
                        }
                    }
                }
            }

            // 3.5. Update IndexOnLayer for correct drawing order/debugging
            var layers = allVertices.Where(v => v is not DummyVertex).GroupBy(v => ((IGraphElement)v).Layer);
            foreach (var grp in layers)
            {
                var sorted = grp.OrderBy(v => ((IFigure)v).Center.X).ToList();
                for (int i = 0; i < sorted.Count; i++)
                {
                    if (sorted[i] is IGraphElement ge) ge.IndexOnLayer = i;
                }
            }
        }
    }
}
