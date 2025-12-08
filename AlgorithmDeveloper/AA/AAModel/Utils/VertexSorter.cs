using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;

namespace AlgorithmDeveloper.AbstractAutomaton.AAModel.Utils
{
    /// <summary>
    /// Класс, определяющий порядок обхода вершин для сортировки.
    /// </summary>
    public class VertexTraversalOrder
    {
        private readonly Dictionary<string, int> _orderMap = new Dictionary<string, int>(StringComparer.Ordinal);

        /// <summary>
        /// Создает новый экземпляр порядка обхода.
        /// </summary>
        /// <param name="startNode">Начальная вершина алгоритма.</param>
        /// <param name="allNodes">Все вершины алгоритма.</param>
        public VertexTraversalOrder(IBDVertex? startNode, IEnumerable<IBDVertex> allNodes)
        {
            BuildOrder(startNode, allNodes);
        }

        private void BuildOrder(IBDVertex? startNode, IEnumerable<IBDVertex> allNodes)
        {
            int index = 0;
            var visited = new HashSet<IBDVertex>();
            var queue = new Queue<IBDVertex>();

            // 1. Yн (StartVertex) - всегда первый
            // Если startNode передан и он StartVertex - начинаем с него.
            // Если просто передан startNode - тоже начинаем с него.
            if (startNode != null)
            {
                AddToOrder(startNode, ref index, visited, queue);
            }

            // 2. Траверс (BFS) с учетом приоритета LBS перед RBS
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current is ConditionalVertex cv)
                {
                    // "первой" (следующей в коллекции) будем считать ту вершину, что является .LBS
                    if (cv.LBS != null) AddToOrder(cv.LBS, ref index, visited, queue);
                    // "второй" - ту, что .RBS
                    if (cv.RBS != null) AddToOrder(cv.RBS, ref index, visited, queue);
                }
                else
                {
                    // Для обычных вершин - просто Next
                    // Учтем, что Next может быть null (например EndVertex, или обрыв)
                    var next = current.Next;
                    if (next != null) AddToOrder(next, ref index, visited, queue);
                }
            }

            // 3. Остальные вершины (недостижимые), кроме Yк
            // Сортируем их по ID, чтобы порядок был детерминированным
            if (allNodes != null)
            {
                var remaining = allNodes
                    .Where(v => !visited.Contains(v) && !(v is EndVertex))
                    .OrderBy(v => v.ID, StringComparer.Ordinal);

                foreach (var v in remaining)
                {
                    AddToOrder(v, ref index, visited, queue);
                }

                // 4. Yк (EndVertex) - всегда последний
                var endNodes = allNodes.OfType<EndVertex>();
                foreach (var end in endNodes)
                {
                    _orderMap[end.ID ?? string.Empty] = int.MaxValue;
                }
            }
        }

        private void AddToOrder(IBDVertex v, ref int index, HashSet<IBDVertex> visited, Queue<IBDVertex> queue)
        {
            // EndVertex пропускаем здесь, он будет добавлен в конце с MaxValue
            if (v is EndVertex) return;

            // JumpPoint - точки перехода, они тоже участвуют в порядке? 
            // По логике да, они вершины графа.
            
            if (visited.Add(v))
            {
                _orderMap[v.ID ?? string.Empty] = index++;
                queue.Enqueue(v);
            }
        }

        public int GetOrder(string? id)
        {
            return _orderMap.TryGetValue(id ?? string.Empty, out int order) ? order : int.MaxValue - 1;
        }

        public int GetOrder(IBDVertex? v)
        {
            return GetOrder(v?.ID);
        }
    }



    /// <summary>
    /// Универсальный компаратор для вершин и их ID.
    /// </summary>
    public class VertexComparer : IComparer<IBDVertex>, IComparer<string>
    {
        private readonly VertexTraversalOrder _order;

        public VertexComparer(VertexTraversalOrder order)
        {
            _order = order ?? throw new ArgumentNullException(nameof(order));
        }

        public int Compare(IBDVertex? x, IBDVertex? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int ox = _order.GetOrder(x);
            int oy = _order.GetOrder(y);

            int cmp = ox.CompareTo(oy);
            if (cmp != 0) return cmp;

            return StringComparer.Ordinal.Compare(x.ID, y.ID);
        }

        public int Compare(string? x, string? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int ox = _order.GetOrder(x);
            int oy = _order.GetOrder(y);

            int cmp = ox.CompareTo(oy);
            if (cmp != 0) return cmp;

            return StringComparer.Ordinal.Compare(x, y);
        }
    }
}
