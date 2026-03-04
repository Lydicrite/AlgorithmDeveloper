using System.Drawing;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Geometry;
using AlgorithmDeveloper.UI.Elements.Controls.Viewports;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph.Layout
{
    public class OrthogonalEdgeRouter
    {
        private const int CYCLE_LANE_OFFSET = 40; // Отступ для канала обратной связи

        // Кэш фигур для поиска коллизий
        private List<IFigure> _allFigures = new();
        
        // Кэш проложенных вертикальных сегментов (для предотвращения пересечений)
        private List<Obstacle> _routedSegments = new();
        private HashSet<Point> _usedOutPorts = new();

        private struct Obstacle
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        public List<VisualEdge> BuildRoutes(AbstractAutomata model, bool useJumpPoints = false)
        {
            var edges = new List<VisualEdge>();
            if (model == null || model.Vertices.Count == 0) return edges;

            _allFigures = model.Vertices.OfType<IFigure>().ToList();
            _routedSegments.Clear();
            _usedOutPorts.Clear();

            // 1. Сбор всех связей
            var rawLinks = CollectLinks(model, useJumpPoints);

            // 2. Группировка по целевой вершине (Target)
            var byTarget = rawLinks.GroupBy(e => e.Target).ToDictionary(g => g.Key, g => g.ToList());

            // 3. Вычисление границ графа (глобальных)
            int minX = int.MaxValue;
            foreach (var v in _allFigures)
            {
                minX = Math.Min(minX, v.Center.X - IFigure.HalfWidth);
            }
            int globalCycleLaneLeftX = minX - CYCLE_LANE_OFFSET;

            // 4. Предварительная маршрутизация всех обратных связей (Back Edges)
            // Сортируем их по вертикальному расстоянию (Span), чтобы сначала прокладывать
            // короткие (внутренние) циклы, а затем длинные (внешние).
            // Это позволяет внешним циклам огибать внутренние.
            
            // Группируем обратные связи по цели (Target), чтобы объединять их перед входом в точку перехода
            var backEdgesByTarget = new Dictionary<IBDVertex, List<VisualEdge>>();
            var allBackEdges = new List<VisualEdge>();
            
            foreach (var target in byTarget.Keys)
            {
                var incoming = byTarget[target];
                var backs = incoming.Where(e => !IsDirect(e.Source, target)).ToList();
                if (backs.Any())
                {
                    backEdgesByTarget[target] = backs;
                    allBackEdges.AddRange(backs); // Для общей статистики или отрисовки, если нужно
                }
            }

            // Обрабатываем каждую группу обратных связей
            foreach (var kvp in backEdgesByTarget)
            {
                var target = kvp.Key;
                var currentEdges = kvp.Value;

                // Сортируем внутри группы по удаленности источника (снизу вверх)
                currentEdges.Sort((a, b) => 
                {
                    var aS = (IFigure)a.Source;
                    var bS = (IFigure)b.Source;
                    return bS.Center.Y.CompareTo(aS.Center.Y);
                });

                if (currentEdges.Count > 1 && target is JumpPoint)
                {
                    // Если несколько обратных связей идут в одну точку перехода, объединяем их
                    RouteMergedBackEdges(target, currentEdges);
                }
                else
                {
                    // Иначе маршрутизируем по отдельности
                    foreach (var edge in currentEdges)
                    {
                        RouteBackEdge(edge);
                    }
                }
            }

            // 5. Маршрутизация прямых связей и сборка
            // Сортируем цели сверху вниз (по слоям), чтобы маршрутизация шла последовательно
            var sortedTargets = byTarget.Keys
                .OfType<IGraphElement>()
                .OrderBy(ge => ge.Layer)
                .ThenBy(ge => ge.IndexOnLayer)
                .Cast<IBDVertex>()
                .ToList();

            // Добавляем те, что не попали в сортировку (например, JumpPoint, если они не IGraphElement)
            foreach (var t in byTarget.Keys)
                if (!sortedTargets.Contains(t)) sortedTargets.Add(t);

            foreach (var target in sortedTargets)
            {
                var incoming = byTarget[target];
                
                var directEdges = incoming.Where(e => IsDirect(e.Source, target)).ToList();
                // Обратные связи уже маршрутизированы выше

                // 5.1. Маршрутизация прямых связей
                if (directEdges.Any())
                {
                    if (directEdges.Count == 1)
                        RouteSingleDirect(directEdges[0]);
                    else
                        RouteDirectGroup(target, directEdges);
                }

                edges.AddRange(incoming);
            }

            _allFigures.Clear();
            _routedSegments.Clear();
            _usedOutPorts.Clear();
            return edges;
        }

        private List<VisualEdge> CollectLinks(AbstractAutomata model, bool useJumpPoints)
        {
            var list = new List<VisualEdge>();

            IBDVertex? ResolveNext(IBDVertex? next)
            {
                if (next == null) return null;
                if (useJumpPoints) return next;
                
                var current = next;
                int safety = 0;
                while (current is JumpPoint jp)
                {
                    current = jp.GetNext(model);
                    if (++safety > 1000) break;
                }
                return current;
            }

            foreach (var v in model.Vertices)
            {
                if (!useJumpPoints && v is JumpPoint) continue;

                if (v is ConditionalVertex cv)
                {
                    var l = ResolveNext(cv.LBS);
                    if (l != null) list.Add(new VisualEdge(cv, l, EdgeType.FalseBranch));
                    
                    var r = ResolveNext(cv.RBS);
                    if (r != null) list.Add(new VisualEdge(cv, r, EdgeType.TrueBranch));
                }
                else
                {
                    var n = ResolveNext(v.Next);
                    if (n != null) list.Add(new VisualEdge(v, n, EdgeType.Direct));
                }
            }
            return list;
        }

        private bool IsDirect(IBDVertex source, IBDVertex target)
        {
            if (source is IGraphElement sGe && target is IGraphElement tGe)
            {
                if (sGe.Layer < tGe.Layer) return true;
                if (sGe.Layer == tGe.Layer)
                {
                    var sFig = (IFigure)source;
                    var tFig = (IFigure)target;
                    return tFig.Center.Y > sFig.Center.Y;
                }
            }
            return false;
        }

        // --- Routing Logic ---

        private void RouteSingleDirect(VisualEdge edge)
        {
            var sFig = (IFigure)edge.Source;
            var tFig = (IFigure)edge.Target;
            
            // Вход в Target всегда сверху (или выбираем лучший)
            Point targetIn = tFig.InputLinkPoints.OrderBy(p => p.Y).FirstOrDefault();
            if (targetIn == Point.Empty)
                targetIn = new Point(tFig.Center.X, tFig.Center.Y - IFigure.HalfHeight - IFigure.Indent);
            
            // Определяем точку выхода
            Point start = GetBestOutputPort(edge.Source, edge.Target, edge.Type);

            bool isLeft = start.X < sFig.Center.X - 5;
            bool isRight = start.X > sFig.Center.X + 5;
            bool isSideExit = isLeft || isRight;

            edge.Points.Add(start);

            // Пытаемся построить L-образный маршрут (1 поворот)
            // Условие: для выхода сбоку X координата цели должна быть "по пути"
            bool simpleL = false;

            if (isSideExit)
            {
                if (isLeft && targetIn.X <= start.X) simpleL = true; // Выход влево и цель левее
                if (isRight && targetIn.X >= start.X) simpleL = true;  // Выход вправо и цель правее
                
                // Проверка на вертикальное перекрытие (чтобы горизонтальный сегмент не пересек Target)
                if (start.Y >= targetIn.Y) simpleL = false; 
            }
            else
            {
                // Выход снизу: L-форма невозможна, нужен Z (Vertical -> Horizontal -> Vertical) = 2 поворота
                // Или если X совпадает, то просто Vertical.
                if (Math.Abs(start.X - targetIn.X) < 2)
                {
                    edge.Points.Add(targetIn);
                    return;
                }
            }

            if (simpleL)
            {
                // 1 Turn: Horizontal -> Vertical
                // (Start) -> (Target.X, Start.Y) -> (Target.X, Target.Y)
                var corner = new Point(targetIn.X, start.Y);
                edge.Points.Add(corner);
                edge.Points.Add(targetIn);
            }
            else
            {
                // Fallback: Z-shape or "Bus" style logic
                // Out -> Vertical (to mid) -> Horizontal -> Vertical
                // Для боковых выходов сначала нужно сделать небольшой отступ (PreTurn)
                Point current = start;
                
                if (isSideExit)
                {
                    int offset = isLeft ? -10 : 10;
                    current = new Point(start.X + offset, start.Y);
                    edge.Points.Add(current);
                }

                // Уровень горизонтального пролета
                // Располагаем шину ближе к целевой вершине (Late Merging), чтобы вертикальные линии были длиннее
                int sBottom = sFig.Center.Y + IFigure.HalfHeight;
                int tTop = tFig.Center.Y - IFigure.HalfHeight;
                
                int midY = targetIn.Y - 20;

                // Если места мало, откатываемся к середине
                if (midY < sBottom + 10) 
                    midY = (sBottom + tTop) / 2;

                // Гарантируем минимальный отступ
                if (midY < sBottom + 10) midY = sBottom + 10;
                if (midY > tTop - 10) midY = tTop - 10;

                // Vertical to Mid
                var p1 = new Point(current.X, midY);
                edge.Points.Add(p1);

                // Horizontal to Target X
                var p2 = new Point(targetIn.X, midY);
                edge.Points.Add(p2);

                // Vertical to Target
                edge.Points.Add(targetIn);
            }
        }

        private void RouteDirectGroup(IBDVertex target, List<VisualEdge> group)
        {
            var tFig = (IFigure)target;
            Point targetIn = tFig.InputLinkPoints.OrderBy(p => p.Y).FirstOrDefault();
            if (targetIn == Point.Empty)
                targetIn = new Point(tFig.Center.X, tFig.Center.Y - IFigure.HalfHeight - IFigure.Indent);

            // Improvement: "Late Merging" strategy.
            // Вместо того чтобы объединять линии посередине (middle of the gap),
            // мы спускаем их максимально низко к целевой вершине перед объединением.
            // Это создает более длинные вертикальные участки и выглядит чище.
            
            // Определяем уровень "шины" непосредственно над целевой вершиной.
            // Отступаем достаточно, чтобы стрелка и поворот выглядели нормально.
            int busY = targetIn.Y - 20; 

            // Если "шина" оказывается выше, чем выходы из источников (например, очень близкие слои),
            // то придется использовать старую логику (середина), чтобы не идти вверх.
            int lowestSourceY = int.MinValue;
            foreach (var edge in group)
            {
                var sFig = (IFigure)edge.Source;
                int y = sFig.Center.Y + IFigure.HalfHeight + IFigure.Indent;
                if (y > lowestSourceY) lowestSourceY = y;
            }
            
            // Если места мало, берем середину, иначе прижимаем к низу
            if (busY < lowestSourceY + 10)
                busY = (lowestSourceY + targetIn.Y) / 2;

            // Защита от слипания
            if (busY < lowestSourceY + 5) busY = lowestSourceY + 5;
            if (busY > targetIn.Y - 10) busY = targetIn.Y - 10;

            // Словарь занятых X-координат на шине, чтобы не рисовать линии поверх друг друга
            var busConnectorsX = new HashSet<int>();

            foreach (var edge in group)
            {
                var sFig = (IFigure)edge.Source;
                Point start = GetBestOutputPort(edge.Source, edge.Target, edge.Type);
                Point? preTurn = null;
                
                bool isLeft = start.X < sFig.Center.X - 5;
                bool isRight = start.X > sFig.Center.X + 5;

                // Определяем точку старта
                if (isLeft)
                {
                    preTurn = new Point(start.X - 10, start.Y);
                }
                else if (isRight)
                {
                    preTurn = new Point(start.X + 10, start.Y);
                }

                edge.Points.Add(start);

                // --- Логика "Smart Vertical Pass" ---
                // Если X выхода совпадает с X входа (или очень близок), и это не боковой выход (или боковой с preTurn, который совпал),
                // то пытаемся провести прямую линию вниз, минуя общую шину, если это не перекрывает вход.
                
                int effectiveStartX = preTurn.HasValue ? preTurn.Value.X : start.X;
                bool isVerticallyAligned = Math.Abs(effectiveStartX - targetIn.X) < 2;

                if (isVerticallyAligned)
                {
                    // Прямой спуск
                    if (preTurn.HasValue) edge.Points.Add(preTurn.Value);
                    
                    // Сразу в цель
                    edge.Points.Add(targetIn);
                }
                else
                {
                    // Стандартный маршрут через шину (Bus)
                    if (preTurn.HasValue)
                    {
                        edge.Points.Add(preTurn.Value);
                        busConnectorsX.Add(preTurn.Value.X);
                        edge.Points.Add(new Point(preTurn.Value.X, busY));
                    }
                    else
                    {
                        busConnectorsX.Add(start.X);
                        edge.Points.Add(new Point(start.X, busY));
                    }
                }
            }

            // Добавляем сегменты самой шины для тех, кто не пошел напрямую
            // Находим крайние точки на шине
            // ВАЖНО: нужно соединить все точки входа на шину с точкой спуска к цели (targetIn.X)
            
            // Фильтруем ребра, которые не пошли напрямую (у которых последняя точка имеет Y == busY)
            var edgesOnBus = group.Where(e => e.Points.Last().Y == busY).ToList();

            if (edgesOnBus.Any())
            {
                foreach (var edge in edgesOnBus)
                {
                    // От текущей точки на шине до точки спуска (targetIn.X, busY)
                    edge.Points.Add(new Point(targetIn.X, busY));
                    // Вниз к цели
                    edge.Points.Add(targetIn);
                }
            }
        }

        private void RouteMergedBackEdges(IBDVertex target, List<VisualEdge> edges)
        {
            if (edges.Count == 0) return;

            var tFig = (IFigure)target;
            int targetTopY = tFig.Center.Y - IFigure.HalfHeight - IFigure.Indent;
            
            // Определяем общую полосу возврата (Lane)
            // Берем самую "широкую" полосу (самую далекую от центра), чтобы охватить все
            int commonLaneX = 0;
            bool laneInitialized = false;

            // Сначала маршрутизируем каждый выход до полосы возврата
            // И определяем общую полосу
            var edgeExitPoints = new Dictionary<VisualEdge, Point>();
            var edgeEntryToLaneY = new Dictionary<VisualEdge, int>();

            foreach (var edge in edges)
            {
                var sFig = (IFigure)edge.Source;
                Point start = GetBestOutputPort(edge.Source, edge.Target, edge.Type);
                
                bool isLeft = start.X < sFig.Center.X - 5;
                bool isRight = start.X > sFig.Center.X + 5;
                bool isSideExit = isLeft || isRight;
                
                Point current;
                if (isLeft) current = new Point(start.X - 10, start.Y);
                else if (isRight) current = new Point(start.X + 10, start.Y);
                else current = new Point(start.X, start.Y + 10);

                edge.Points.Add(start);
                edge.Points.Add(current);
                
                edgeExitPoints[edge] = current;
                
                // Рассчитываем LaneX для этого ребра
                int laneTraversalY = current.Y;
                int localLaneX;
                
                // Предпочитаем сторону, где больше места или где находится большинство выходов
                // Для простоты, если есть выход справа, идем справа, иначе слева?
                // Или для каждого индивидуально, а потом берем экстремум.
                
                // В текущей реализации RouteBackEdge выбирает сторону based on exit side.
                // Для JumpPoint (круг) лучше объединить все с одной стороны, если возможно.
                // Но если выходы с разных сторон (напр. True и False ветви), то придется использовать две полосы?
                // Задача просит объединить. Попробуем найти единую полосу.
                // Обычно JumpPoint находится по центру.
                
                // Простая эвристика: если большинство выходов справа -> общая полоса справа.
                bool goRight = isRight; // Пока индивидуально
                
                if (goRight)
                    localLaneX = CalculateSafeLaneRight(laneTraversalY, tFig.Center.Y - IFigure.HalfHeight, Math.Max(current.X, tFig.Center.X));
                else
                    localLaneX = CalculateSafeLaneLeft(laneTraversalY, tFig.Center.Y - IFigure.HalfHeight, Math.Min(current.X, tFig.Center.X));

                // Обновляем общую полосу (самую внешнюю)
                if (!laneInitialized)
                {
                    commonLaneX = localLaneX;
                    laneInitialized = true;
                }
                else
                {
                    // Если текущая common справа, и новая справа -> берем макс
                    if (commonLaneX > tFig.Center.X && localLaneX > tFig.Center.X)
                        commonLaneX = Math.Max(commonLaneX, localLaneX);
                    // Если текущая common слева, и новая слева -> берем мин
                    else if (commonLaneX < tFig.Center.X && localLaneX < tFig.Center.X)
                        commonLaneX = Math.Min(commonLaneX, localLaneX);
                    // Если стороны разные... проблема. Пока оставим как есть (разные полосы не объединятся).
                    // Для решения проблемы 1 (лишняя стрелочка) достаточно, чтобы ребра с одной стороны объединились.
                }
                
                // Проверка на спуск (Dip)
                if (isSideExit)
                {
                    bool pathBlocked = IsHorizontalSegmentBlocked(current.Y, current.X, localLaneX);
                    if (pathBlocked)
                    {
                        laneTraversalY = sFig.Center.Y + IFigure.HalfHeight + IFigure.Indent + 10;
                        edge.Points.Add(new Point(current.X, laneTraversalY));
                    }
                }
                edgeEntryToLaneY[edge] = laneTraversalY;
            }

            // Теперь соединяем все с commonLaneX (если стороны совпадают)
            // И ведем к JumpPoint
            
            // Вычисляем уровень возврата (aboveTargetY)
            int vSpacing = AlgorithmDeveloper.UI.Elements.Controls.Viewports.VisualizationSettings.VerticalSpacing;
            int aboveTargetY = targetTopY - 20;
            int midY = tFig.Center.Y - (int)(vSpacing * 0.5);
            if (aboveTargetY < midY) aboveTargetY = midY;

            // Группируем по стороне (Left/Right относительно Target)
            var leftEdges = edges.Where(e => edgeExitPoints[e].X < tFig.Center.X).ToList();
            var rightEdges = edges.Where(e => edgeExitPoints[e].X >= tFig.Center.X).ToList();

            void RouteGroupToLane(List<VisualEdge> group, int laneX)
            {
                if (group.Count == 0) return;
                
                // Регистрируем сегмент общей шины
                int minLaneY = aboveTargetY;
                int maxLaneY = aboveTargetY;

                foreach (var edge in group)
                {
                    int y = edgeEntryToLaneY[edge];
                    edge.Points.Add(new Point(laneX, y)); // Доходим до шины
                    minLaneY = Math.Min(minLaneY, y);
                    maxLaneY = Math.Max(maxLaneY, y);
                }

                // Рисуем сегменты вниз/вверх по шине к точке aboveTargetY
                foreach (var edge in group)
                {
                    edge.Points.Add(new Point(laneX, aboveTargetY));
                }

                // Регистрируем препятствие
                _routedSegments.Add(new Obstacle
                {
                    Left = laneX - 10,
                    Right = laneX + 10,
                    Top = minLaneY,
                    Bottom = maxLaneY
                });
                
                // От шины к центру над Target
                // Но у нас может быть две группы (слева и справа).
                // Они должны встретиться в точке (tFig.Center.X, aboveTargetY)
            }

            // Вычисляем общие полосы для левой и правой групп
            int leftLaneX = int.MaxValue; // Ближе к -беск
            if (leftEdges.Any())
            {
                // Берем самый левый localLaneX
                // (упрощение: пересчитываем или берем из предрасчета, если бы сохранили)
                // Для надежности пересчитаем safe lane для всей группы сразу?
                // Просто берем мин из индивидуальных
                leftLaneX = tFig.Center.X - CYCLE_LANE_OFFSET;
                foreach(var e in leftEdges)
                {
                     // Грубая оценка, можно улучшить
                     int y = edgeEntryToLaneY[e];
                     int lx = CalculateSafeLaneLeft(y, targetTopY, Math.Min(edgeExitPoints[e].X, tFig.Center.X));
                     leftLaneX = Math.Min(leftLaneX, lx);
                }
                RouteGroupToLane(leftEdges, leftLaneX);
            }

            int rightLaneX = int.MinValue;
            if (rightEdges.Any())
            {
                rightLaneX = tFig.Center.X + CYCLE_LANE_OFFSET;
                foreach(var e in rightEdges)
                {
                     int y = edgeEntryToLaneY[e];
                     int lx = CalculateSafeLaneRight(y, targetTopY, Math.Max(edgeExitPoints[e].X, tFig.Center.X));
                     rightLaneX = Math.Max(rightLaneX, lx);
                }
                RouteGroupToLane(rightEdges, rightLaneX);
            }

            // Финальный спуск в Target
            // Соединяем точки (LeftLaneX, aboveTargetY) и (RightLaneX, aboveTargetY) с (TargetX, aboveTargetY)
            // И затем в TargetIn
            
            var mergePoint = new Point(tFig.Center.X, aboveTargetY);
            var targetIn = new Point(tFig.Center.X, targetTopY);

            foreach (var edge in edges)
            {
                // Последняя точка сейчас (LaneX, aboveTargetY)
                // Добавляем mergePoint и targetIn
                // Проверка дубликатов точек
                var last = edge.Points.Last();
                if (last != mergePoint) edge.Points.Add(mergePoint);
                edge.Points.Add(targetIn);
            }
        }

        private void RouteBackEdge(VisualEdge edge)
        {
            var sFig = (IFigure)edge.Source;
            var tFig = (IFigure)edge.Target;

            // Выход
            Point start = GetBestOutputPort(edge.Source, edge.Target, edge.Type);
            
            bool isLeft = start.X < sFig.Center.X - 5;
            bool isRight = start.X > sFig.Center.X + 5;
            bool isSideExit = isLeft || isRight;
            
            Point current;

            if (isLeft)
            {
                current = new Point(start.X - 10, start.Y);
            }
            else if (isRight)
            {
                current = new Point(start.X + 10, start.Y);
            }
            else
            {
                current = new Point(start.X, start.Y + 10);
            }

            edge.Points.Add(start);
            edge.Points.Add(current);

            int laneTraversalY = current.Y;


            // Рассчитываем локальную полосу возврата (LaneX)
            // Пытаемся найти безопасную X координату слева от участвующих вершин
            // Используем laneTraversalY как нижнюю границу поиска препятствий
            
            int localLaneX;
            if (isRight)
            {
                // Для выхода справа ищем полосу справа
                localLaneX = CalculateSafeLaneRight(laneTraversalY, tFig.Center.Y - IFigure.HalfHeight, Math.Max(current.X, tFig.Center.X));
            }
            else
            {
                // Для выхода слева или снизу ищем полосу слева
                localLaneX = CalculateSafeLaneLeft(laneTraversalY, tFig.Center.Y - IFigure.HalfHeight, Math.Min(current.X, tFig.Center.X));
            }

            // Проверяем, нужно ли делать спуск (Dip)
            // Спуск нужен, если горизонтальный путь от current.X до localLaneX блокирован
            if (isSideExit)
            {
                bool pathBlocked = IsHorizontalSegmentBlocked(current.Y, current.X, localLaneX);
                if (pathBlocked)
                {
                    laneTraversalY = sFig.Center.Y + IFigure.HalfHeight + IFigure.Indent + 10;
                    edge.Points.Add(new Point(current.X, laneTraversalY));
                    
                    // Пересчитываем LaneX с учетом новой глубины (возможно, препятствия ниже другие)
                    // Но обычно LaneX определяется глобальным "контуром" препятствий, так что старый LaneX тоже валиден.
                    // Для надежности можно пересчитать, но пока оставим найденный LaneX (он обычно самый безопасный).
                }
            }

            var toLane = new Point(localLaneX, laneTraversalY);
            edge.Points.Add(toLane);

            int targetTopY = tFig.Center.Y - IFigure.HalfHeight - IFigure.Indent;
            
            // Вычисляем уровень возврата (aboveTargetY)
            // Пытаемся выровнять его с уровнем "шины" для прямых связей (Late Merging),
            // чтобы стрелки не поднимались слишком высоко без необходимости.
            // Стандартный отступ шины: 20 пикселей от точки входа.
            int aboveTargetY = targetTopY - 20;

            // Однако нужно проверить, не перекрывает ли это предыдущий слой.
            // Грубая проверка: (Center - 100) + HalfHeight + margin.
            // Но проще использовать безопасный диапазон, аналогичный RouteDirectGroup.
            // Если мы слишком близко к предыдущему слою, можно опустить, но обычно targetTopY - 20
            // находится ниже середины интервала, так что это безопасно.
            // На всякий случай ограничимся снизу (не слишком близко к targetTopY) и сверху (не выше середины).
            
            // Используем стандартный шаг назад
            int vSpacing = AlgorithmDeveloper.UI.Elements.Controls.Viewports.VisualizationSettings.VerticalSpacing;

            int midY = tFig.Center.Y - (int)(vSpacing * 0.5);
            if (aboveTargetY < midY) aboveTargetY = midY; // Если вдруг шина выше середины, берем середину

            var upLane = new Point(localLaneX, aboveTargetY);
            edge.Points.Add(upLane);

            // Регистрируем вертикальный сегмент как препятствие для последующих связей
            _routedSegments.Add(new Obstacle
            {
                Left = localLaneX - 10,
                Right = localLaneX + 10,
                Top = Math.Min(laneTraversalY, aboveTargetY),
                Bottom = Math.Max(laneTraversalY, aboveTargetY)
            });

            var toTargetX = new Point(tFig.Center.X, aboveTargetY);
            edge.Points.Add(toTargetX);

            var targetIn = new Point(tFig.Center.X, targetTopY);
            edge.Points.Add(targetIn);
        }

        private bool IsHorizontalSegmentBlocked(int y, int x1, int x2)
        {
            int left = Math.Min(x1, x2);
            int right = Math.Max(x1, x2);
            
            // Проверка с небольшим запасом по высоте, так как линия имеет толщину
            int yTop = y - 5;
            int yBottom = y + 5;

            foreach (var fig in _allFigures)
            {
                int figTop = fig.Center.Y - IFigure.HalfHeight;
                int figBottom = fig.Center.Y + IFigure.HalfHeight;

                // Если фигура пересекает нашу линию по Y
                if (figBottom >= yTop && figTop <= yBottom)
                {
                    int figLeft = fig.Center.X - IFigure.HalfWidth;
                    int figRight = fig.Center.X + IFigure.HalfWidth;

                    // Если фигура пересекает нашу линию по X (строго внутри интервала)
                    // Мы допускаем касание границ, но не проход сквозь
                    if (figRight > left && figLeft < right)
                    {
                        return true;
                    }
                }
            }

            foreach (var seg in _routedSegments)
            {
                if (seg.Bottom >= yTop && seg.Top <= yBottom)
                {
                    if (seg.Right > left && seg.Left < right)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int CalculateSafeLaneLeft(int yBottom, int yTop, int referenceX)
        {
            // Ищем препятствия в диапазоне Y [yTop, yBottom]
            // Нам нужно найти ближайший X слева, который свободен для прохода.
            
            // 1. Собираем все препятствия в диапазоне Y
            var obstacles = new List<Obstacle>();

            foreach (var f in _allFigures)
            {
                if (f.Center.Y + IFigure.HalfHeight >= yTop && f.Center.Y - IFigure.HalfHeight <= yBottom && f.Center.X - IFigure.HalfWidth < referenceX)
                {
                    obstacles.Add(new Obstacle
                    {
                        Left = f.Center.X - IFigure.HalfWidth,
                        Right = f.Center.X + IFigure.HalfWidth,
                        Top = f.Center.Y - IFigure.HalfHeight,
                        Bottom = f.Center.Y + IFigure.HalfHeight
                    });
                }
            }

            foreach (var seg in _routedSegments)
            {
                if (seg.Bottom >= yTop && seg.Top <= yBottom && seg.Right < referenceX)
                {
                    obstacles.Add(seg);
                }
            }

            obstacles.Sort((a, b) => b.Right.CompareTo(a.Right)); // сортируем справа налево

            if (obstacles.Count == 0)
                return referenceX - CYCLE_LANE_OFFSET;

            // 2. Ищем первый зазор
            // Начинаем поиск от referenceX и идем влево
            int currentX = referenceX - CYCLE_LANE_OFFSET;
            
            bool found = false;
            while (!found)
            {
                found = true;
                foreach (var obs in obstacles)
                {
                    // Проверяем конфликт с currentX (с учетом ширины линии/маршрута)
                    // Считаем, что полоса занимает [currentX-5, currentX+5]
                    // Или просто точку. Добавим запас CYCLE_LANE_OFFSET/2
                    int margin = 20; 

                    if (currentX >= obs.Left - margin && currentX <= obs.Right + margin)
                    {
                        // Конфликт. Сдвигаем currentX влево за препятствие
                        currentX = obs.Left - CYCLE_LANE_OFFSET;
                        found = false; 
                        break; // Перезапускаем проверку с новым X
                    }
                }
                
                // Защита от бесконечного цикла (если вышли далеко за пределы графа)
                if (currentX < -10000) break;
            }

            return currentX;
        }

        private int CalculateSafeLaneRight(int yBottom, int yTop, int referenceX)
        {
            // Ищем препятствия в диапазоне Y [yTop, yBottom]
            // Нам нужно найти ближайший X справа, который свободен для прохода.
            
            var obstacles = new List<Obstacle>();

            foreach (var f in _allFigures)
            {
                if (f.Center.Y + IFigure.HalfHeight >= yTop && f.Center.Y - IFigure.HalfHeight <= yBottom && f.Center.X + IFigure.HalfWidth > referenceX)
                {
                    obstacles.Add(new Obstacle
                    {
                        Left = f.Center.X - IFigure.HalfWidth,
                        Right = f.Center.X + IFigure.HalfWidth,
                        Top = f.Center.Y - IFigure.HalfHeight,
                        Bottom = f.Center.Y + IFigure.HalfHeight
                    });
                }
            }

            foreach (var seg in _routedSegments)
            {
                if (seg.Bottom >= yTop && seg.Top <= yBottom && seg.Left > referenceX)
                {
                    obstacles.Add(seg);
                }
            }

            obstacles.Sort((a, b) => a.Left.CompareTo(b.Left)); // сортируем слева направо

            if (obstacles.Count == 0)
                return referenceX + CYCLE_LANE_OFFSET;

            int currentX = referenceX + CYCLE_LANE_OFFSET;
            
            bool found = false;
            while (!found)
            {
                found = true;
                foreach (var obs in obstacles)
                {
                    int margin = 20;

                    if (currentX >= obs.Left - margin && currentX <= obs.Right + margin)
                    {
                        // Конфликт. Сдвигаем currentX вправо за препятствие
                        currentX = obs.Right + CYCLE_LANE_OFFSET;
                        found = false;
                        break;
                    }
                }
                if (currentX > 100000) break;
            }

            return currentX;
        }

        private Point GetBestOutputPort(IBDVertex source, IBDVertex target, EdgeType type)
        {
            var sFig = (IFigure)source;
            var tFig = (IFigure)target;
            
            // Candidates
            var candidates = sFig.OutputLinkPoints.Where(p => !_usedOutPorts.Contains(p)).ToList();
            
            // Filter by type
            if (source is ConditionalVertex)
            {
                if (type == EdgeType.FalseBranch) // Left or Bottom
                    candidates = candidates.Where(p => p.X <= sFig.Center.X + 1).ToList(); 
                else if (type == EdgeType.TrueBranch) // Right or Bottom
                    candidates = candidates.Where(p => p.X >= sFig.Center.X - 1).ToList();
            }
            
            // Target Input (assume first or closest)
            var targetInput = tFig.InputLinkPoints.OrderBy(p => p.Y).FirstOrDefault();
            if (targetInput == Point.Empty)
            {
                // Fallback
                targetInput = new Point(tFig.Center.X, tFig.Center.Y - IFigure.HalfHeight - IFigure.Indent);
            }
            
            // Find best
            Point best = Point.Empty;
            double minCost = double.MaxValue;
            
            // Если целевая вершина - точка перехода (JumpPoint), и она находится строго снизу от условной вершины,
            // даем огромный бонус выходу снизу, чтобы избежать выхода влево/вправо и петли.
            bool isJumpPointTarget = tFig is JumpPoint || (target is JumpPoint);
            
            foreach (var p in candidates)
            {
                double cost = Math.Abs(p.X - targetInput.X) + Math.Abs(p.Y - targetInput.Y); // Manhattan
                
                // Bonus for vertical alignment
                if (Math.Abs(p.X - targetInput.X) < 2) 
                {
                    cost -= 1000;
                    if (isJumpPointTarget && p.Y < targetInput.Y) cost -= 5000; // Extra bonus for JumpPoint below
                }
                
                // Penalty for intersections (Raycast)
                // We check if vertical path from p to targetInput.Y is blocked
                if (IsVerticalPathBlocked(p.X, p.Y, targetInput.Y)) cost += 5000;
                
                if (cost < minCost)
                {
                    minCost = cost;
                    best = p;
                }
            }
            
            if (best == Point.Empty && candidates.Count > 0) best = candidates[0]; // Fallback
            if (best == Point.Empty) best = new Point(sFig.Center.X, sFig.Center.Y + IFigure.HalfHeight + IFigure.Indent); // Total fallback
            
            _usedOutPorts.Add(best);
            return best;
        }

        private bool IsVerticalPathBlocked(int x, int y1, int y2)
        {
            int top = Math.Min(y1, y2);
            int bottom = Math.Max(y1, y2);
            int margin = 5;

            foreach (var fig in _allFigures)
            {
                int fTop = fig.Center.Y - IFigure.HalfHeight;
                int fBottom = fig.Center.Y + IFigure.HalfHeight;
                int fLeft = fig.Center.X - IFigure.HalfWidth;
                int fRight = fig.Center.X + IFigure.HalfWidth;

                if (x >= fLeft - margin && x <= fRight + margin)
                {
                    // Check Y overlap
                    if (Math.Max(top, fTop) < Math.Min(bottom, fBottom))
                        return true;
                }
            }
            return false;
        }
    }
}
