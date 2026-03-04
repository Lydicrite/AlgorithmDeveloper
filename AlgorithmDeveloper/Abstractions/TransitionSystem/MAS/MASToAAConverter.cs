using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.TransitionSystem;
using LogicalExpressions;
using LogicalExpressions.Core;
using LogicalExpressions.Parsing;
using LogicalExpressions.Core.Nodes;
using AlgorithmDeveloper.Abstractions.LAS;
using AlgorithmDeveloper.Abstractions.Minimization;

namespace AlgorithmDeveloper.Abstractions.TransitionSystem.MAS
{
    public static class MASToAAConverter
    {
        private class ConversionContext
        {
            // Кэш для дедупликации условных вершин: (VarName, LBS, RBS) -> Vertex
            public Dictionary<(string, IBDVertex?, IBDVertex?), IBDVertex> NodeCache = new();
            public int JumpPointCounter = 1;
        }

        /// <summary>
        /// Преобразует строковое представление МСА в модель <see cref="AbstractAutomata"/>.
        /// </summary>
        /// <param name="masString">Строковое представление МСА (таблица).</param>
        /// <param name="optimize">Флаг, включающий оптимизацию графа (удаление избыточных условий).</param>
        /// <returns>Восстановленная модель алгоритма.</returns>
        public static AbstractAutomata Convert(string masString, bool optimize = false)
        {
            if (string.IsNullOrWhiteSpace(masString))
                throw new ArgumentException("Входная строка МСА пуста.", nameof(masString));

            var model = new AbstractAutomata();
            var parsedData = ParseMASTable(masString);

            // Добавляем идентификаторы целевых вершин, которые могут отсутствовать в списке источников (например, Yк)
            foreach (var row in parsedData.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (!string.IsNullOrWhiteSpace(cell.TargetId))
                    {
                        parsedData.AllVertexIds.Add(cell.TargetId);
                    }
                }
            }

            // 1. Создаем все Y-вершины (Yн, Yi, Yк)
            var vertexMap = new Dictionary<string, IBDVertex>();

            foreach (var id in parsedData.AllVertexIds)
            {
                IBDVertex v;
                if (id == "Yн")
                    v = new StartVertex();
                else if (id == "Yк")
                    v = new EndVertex();
                else if (id.StartsWith("Y"))
                {
                    if (int.TryParse(id.Substring(1), out int idx))
                        v = new OperatorVertex(idx);
                    else
                        throw new FormatException($"Некорректный идентификатор операторной вершины: {id}");
                }
                else
                {
                    throw new FormatException($"Неизвестный тип вершины в заголовке МСА: {id}");
                }

                model.AddVertex(v);
                vertexMap[id] = v;
            }

            var ctx = new ConversionContext();

            // 2. Строим переходы для каждой строки (деревья условий)
            foreach (var row in parsedData.Rows)
            {
                var sourceId = row.SourceId;
                if (!vertexMap.TryGetValue(sourceId, out var sourceVertex))
                    continue;

                var transitions = new List<(string Formula, IBDVertex Target)>();
                foreach (var cell in row.Cells)
                {
                    if (string.IsNullOrWhiteSpace(cell.Formula)) continue;
                    if (vertexMap.TryGetValue(cell.TargetId, out var target))
                    {
                        transitions.Add((cell.Formula, target));
                    }
                }

                if (transitions.Count == 0) continue;

                BuildConditionTree(model, sourceVertex, transitions, ctx);
            }

            // 3. Удаление избыточных условных вершин (когда LBS == RBS)
            // Включаем только если запрошена оптимизация, так как это может изменить структуру исходной ЛСА.
            if (optimize)
            {
                RemoveRedundantConditions(model);
            }

            // 4. Постобработка: вставка JumpPoint для соответствия структуре ЛСА
            InsertJumpPoints(model, ctx);

            // 5. Повторная оптимизация после вставки JumpPoints (для устранения избыточности, возникшей из-за JPs)
            if (optimize)
            {
                RemoveRedundantConditions(model);
            }

            // 6. Удаление недостижимых вершин
            // В процессе конвертации могут возникать структурно связные, но логически недостижимые участки
            // (например, из-за корреляции условий). Удаляем их, чтобы модель была корректной.
            StructuralMinimizer.RemoveLogicallyUnreachable(model);
            
            // После удаления логически недостижимых могут появиться избыточные JumpPoint или условия
            StructuralMinimizer.RemoveRedundantChecks(model);
            StructuralMinimizer.BypassJumpPoints(model);

            StructuralMinimizer.RemoveUnreachableVertices(model); // На всякий случай запускаем структурную очистку тоже

            model.Update();
            
            // Генерируем валидную строку ЛСА для модели
            try
            {
                model.LAS = AAToLASConverter.Convert(model, out _, false);
                model.LAS = FixLAS(model.LAS, model);
            }
            catch
            {
                // Если генерация не удалась (например, граф некорректен), оставляем пустым
                // Это позволит тестам упасть с понятной ошибкой в CheckCorrectness
            }

            return model;
        }

        private static string FixLAS(string las, AbstractAutomata model)
        {
            if (string.IsNullOrWhiteSpace(las)) return las;

            // Паттерн: точка перехода ↓N, за которой сразу следует другая точка перехода ↓M.
            // Это происходит, когда тело точки перехода пустое.
            // Используем Lookahead (?=\s+↓), чтобы обрабатывать цепочки ↓N ↓M ↓K корректно.
            var pattern = @"(↓(\d+))(?=\s+(↓(\d+)))";
            
            var fixedLas = Regex.Replace(las, pattern, match =>
            {
                if (int.TryParse(match.Groups[2].Value, out int jpIdx))
                {
                    var jp = model.EnsureJumpPoint(jpIdx);
                    if (jp.Next is EndVertex)
                    {
                        return match.Groups[1].Value + " Yк";
                    }
                    else
                    {
                         // Если не конец, значит это цепочка ↓N ↓M, и они ведут в одну точку.
                         // Чтобы удовлетворить парсер, вставляем безусловный переход на следующую точку.
                         return match.Groups[1].Value + " w↑" + match.Groups[4].Value;
                    }
                }
                return match.Value;
            });

            // Удаление дубликатов Yк, следующих за безусловным переходом (w↑j Yк -> w↑j)
            // Это может возникнуть, если конвертер ошибочно добавил Yк после завершающего перехода,
            // а Yк уже существует в другом месте (например, внутри ↓j).
            fixedLas = Regex.Replace(fixedLas, @"(w↑\d+)\s+Yк", "$1");

            return fixedLas;
        }

        private static void RemoveRedundantConditions(AbstractAutomata model)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                
                // Строим карту входящих ребер
                var incoming = new Dictionary<IBDVertex, List<IBDVertex>>();
                foreach (var v in model.Vertices)
                {
                    var nexts = new List<IBDVertex>();
                    if (v.Next != null) nexts.Add(v.Next);
                    if (v is ConditionalVertex cv)
                    {
                        if (cv.LBS != null) nexts.Add(cv.LBS);
                        if (cv.RBS != null) nexts.Add(cv.RBS);
                    }
                    foreach (var n in nexts)
                    {
                        if (!incoming.ContainsKey(n)) incoming[n] = new List<IBDVertex>();
                        incoming[n].Add(v);
                    }
                }

                var toRemove = new List<IBDVertex>();
                foreach (var v in model.Vertices.OfType<ConditionalVertex>())
                {
                    // Определяем эффективные цели (пропуская цепочки JumpPoints)
                    var targetL = GetEffectiveTarget(v.LBS);
                    var targetR = GetEffectiveTarget(v.RBS);

                    // Если ветки ведут в одну и ту же вершину (логически)
                    bool redundant = false;
                    
                    if (ReferenceEquals(targetL, targetR) && targetL != null)
                    {
                        redundant = true;
                    }
                    
                    if (redundant)
                    {
                        // Цель для перенаправления родителей
                        // Используем оригинальную ссылку (например LBS), если она ведет к цели,
                        // чтобы сохранить цепочку JumpPoint, если она нужна для других путей?
                        // Нет, если мы удаляем условие, мы должны направить родителей на общую цель.
                        // Лучше использовать LBS, так как он ближе.
                        var target = v.LBS;
                        
                        // Если вершина ссылается сама на себя (бесконечный цикл внутри условия),
                        // удаление приведет к потере связности и "висячим" ссылкам.
                        // Оставляем её как есть.
                        if (ReferenceEquals(target, v))
                        {
                            continue;
                        }
                        
                        // Если LBS и RBS разные объекты (JumpPoints), но ведут в одно место.
                        // Мы берем LBS как target.

                        if (incoming.TryGetValue(v, out var parents))
                        {
                            foreach (var p in parents)
                            {
                                if (ReferenceEquals(p.Next, v)) p.Next = target;
                                if (p is ConditionalVertex pcv)
                                {
                                    if (ReferenceEquals(pcv.LBS, v)) pcv.LBS = target;
                                    if (ReferenceEquals(pcv.RBS, v)) pcv.RBS = target;
                                }
                            }
                        }
                        toRemove.Add(v);
                    }
                }

                if (toRemove.Count > 0)
                {
                    foreach (var v in toRemove)
                        model.RemoveVertex(v);
                    changed = true;
                }
            }
        }

        private static IBDVertex? GetEffectiveTarget(IBDVertex? v)
        {
            if (v == null) return null;
            var curr = v;
            var visited = new HashSet<IBDVertex>();
            while (curr is JumpPoint jp)
            {
                if (!visited.Add(curr)) return curr; // Loop detected
                curr = jp.Next; // JumpPoint always has Next (or null)
                if (curr == null) return null;
            }
            return curr;
        }

        private static void InsertJumpPoints(AbstractAutomata model, ConversionContext ctx)
        {
            // 0. Пре-процессинг: Перенаправление переходов на StartVertex
            // Любой переход на StartVertex (Yн) должен быть перенаправлен на StartVertex.Next.
            // Это решает проблему "Loop to Start", которая создает некорректные структуры в ЛСА (↓1 w↑1 без тела).
            if (model.Start != null && model.Start.Next != null)
            {
                var startNode = model.Start;
                var realStart = startNode.Next;
                
                // Находим всех, кто ссылается на Start
                // (Это не очень эффективно O(V), но граф небольшой)
                foreach (var v in model.Vertices)
                {
                    if (v == startNode) continue; // Skip self

                    if (v is ConditionalVertex cv)
                    {
                        if (ReferenceEquals(cv.LBS, startNode)) cv.LBS = realStart;
                        if (ReferenceEquals(cv.RBS, startNode)) cv.RBS = realStart;
                    }
                    else
                    {
                        if (ReferenceEquals(v.Next, startNode)) v.Next = realStart;
                    }
                }
            }

            // 0.5. Анализ основного цикла (Main Chain) для определения ветвей
            var mainVertices = GetMainLoopVertices(model);

            // 1. Сбор входящих ребер
            // Map: Target -> List<(Source, EdgeType)>
            // EdgeType: 0 = Next/RBS, 1 = LBS
            var incoming = new Dictionary<IBDVertex, List<(IBDVertex Source, int Type)>>();

            foreach (var v in model.Vertices)
                incoming[v] = new List<(IBDVertex, int)>();

            foreach (var v in model.Vertices.ToList())
            {
                if (v.Next != null)
                {
                    if (!incoming.ContainsKey(v.Next)) incoming[v.Next] = new List<(IBDVertex, int)>();
                    incoming[v.Next].Add((v, 0));
                }

                if (v is ConditionalVertex cv)
                {
                    if (cv.LBS != null)
                    {
                        if (!incoming.ContainsKey(cv.LBS)) incoming[cv.LBS] = new List<(IBDVertex, int)>();
                        incoming[cv.LBS].Add((cv, 1));
                    }
                    if (cv.RBS != null)
                    {
                        if (!incoming.ContainsKey(cv.RBS)) incoming[cv.RBS] = new List<(IBDVertex, int)>();
                        incoming[cv.RBS].Add((cv, 0));
                    }
                }
            }

            // 2. Вставка JumpPoint
            // Используем топологическую сортировку (Reverse Post-Order logic), как в StructuralMerger,
            // чтобы гарантировать правильный порядок ID точек перехода (Parent < Child).
            var postOrder = GetPostOrder(model);
            var orderMap = postOrder.Select((v, i) => (v, i)).ToDictionary(x => x.v, x => x.i);

            var targets = incoming.Keys.OrderByDescending(v => 
            {
                if (v is EndVertex) return int.MinValue; // EndVertex обрабатываем последним в списке Descending (чтобы он был в конце приоритетов?)
                // В StructuralMerger: EndVertex -> int.MinValue.
                // OrderByDescending: Максимальные значения идут первыми.
                // Если мы хотим Parent (ближе к корню, выше индекс в PostOrder) -> Child (глубже, ниже индекс),
                // То PostOrder: Root имеет MAX индекс, Leaves имеют 0.
                // OrderByDescending по индексу PostOrder: Сначала Root, потом Leaves.
                // ID присваиваются последовательно: 1, 2, 3...
                // Значит Root получает ID=1, Child получает ID=N.
                // Это соответствует правилу Parent < Child.
                
                // EndVertex в StructuralMerger возвращает int.MinValue.
                // Значит он будет в самом КОНЦЕ списка (последним).
                // Значит он получит самый БОЛЬШОЙ ID.
                return orderMap.TryGetValue(v, out int idx) ? idx : -1;
            }).ToList();
            
            foreach (var target in targets)
            {
                if (target is JumpPoint) continue;
                if (target is StartVertex) continue; 

                var edges = incoming[target];
                if (edges.Count == 0) continue;

                // Условия вставки JumpPoint:
                // 1. In-Degree > 1 (слияние потоков)
                // 2. Есть входящее ребро типа LBS (требование синтаксиса ↑j)
                bool needsJumpPoint = edges.Count > 1 || edges.Any(e => e.Type == 1);

                // 3. Спец. логика для EndVertex: всегда создаем JumpPoint, если он является целью перехода.
                // Это необходимо, так как AAToLASConverter не может сгенерировать переход w^ к EndVertex без JumpPoint,
                // если EndVertex не является следующим элементом в последовательности (например, при переходе из ветви).
                if (!needsJumpPoint && target is EndVertex)
                {
                    needsJumpPoint = true;
                }

                if (needsJumpPoint)
                {
                    int jpIndex = ctx.JumpPointCounter++;
                    var jp = model.EnsureJumpPoint(jpIndex);
                    
                    model.LinkNext(jp, target);
                    
                    foreach (var (source, type) in edges)
                    {
                        if (source is ConditionalVertex cv)
                        {
                            if (type == 1) // LBS
                            {
                                if (ReferenceEquals(cv.LBS, target)) cv.LBS = jp;
                            }
                            else // RBS
                            {
                                if (ReferenceEquals(cv.RBS, target)) cv.RBS = jp;
                            }
                        }
                        else
                        {
                            if (ReferenceEquals(source.Next, target)) source.Next = jp;
                        }
                    }

                    // model.LinkNext(jp, target); // Moved up
                }
            }
        }

        private static HashSet<IBDVertex> GetMainLoopVertices(AbstractAutomata model)
        {
            var visited = new HashSet<IBDVertex>();
            if (model.Start == null) return visited;

            var queue = new Queue<IBDVertex>();
            queue.Enqueue(model.Start);
            visited.Add(model.Start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                // Next
                if (current.Next != null && visited.Add(current.Next))
                {
                    queue.Enqueue(current.Next);
                }

                // Conditional LBS (treated as Main path by GenerateCore)
                if (current is ConditionalVertex cv && cv.LBS != null && visited.Add(cv.LBS))
                {
                    queue.Enqueue(cv.LBS);
                }
            }
            return visited;
        }

        private static List<IBDVertex> GetPostOrder(AbstractAutomata model)
        {
            var result = new List<IBDVertex>();
            var visited = new HashSet<IBDVertex>();
            
            if (model.Start != null)
                Visit(model.Start, visited, result);
                
            // Добавляем недостижимые вершины (если есть)
            foreach (var v in model.Vertices)
            {
                if (!visited.Contains(v))
                    Visit(v, visited, result);
            }
            
            return result;

            void Visit(IBDVertex u, HashSet<IBDVertex> vis, List<IBDVertex> res)
            {
                if (!vis.Add(u)) return;

                if (u is ConditionalVertex cv)
                {
                    // Посещаем RBS, затем LBS (Порядок важен для Reverse Post-Order нумерации).
                    if (cv.RBS != null) Visit(cv.RBS, vis, res);
                    if (cv.LBS != null) Visit(cv.LBS, vis, res);
                }
                else
                {
                    if (u.Next != null) Visit(u.Next, vis, res);
                }
                
                res.Add(u);
            }
        }

        private class ParsedMAS
        {
            public HashSet<string> AllVertexIds { get; } = new HashSet<string>();
            public List<ParsedRow> Rows { get; } = new List<ParsedRow>();
        }

        private class ParsedRow
        {
            public string SourceId { get; set; } = string.Empty;
            public List<ParsedCell> Cells { get; } = new List<ParsedCell>();
        }

        private class ParsedCell
        {
            public string TargetId { get; set; } = string.Empty;
            public string Formula { get; set; } = string.Empty;
        }

        private static ParsedMAS ParseMASTable(string table)
        {
            var lines = table.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(l => l.Trim())
                             .Where(l => !string.IsNullOrEmpty(l))
                             .ToList();

            int headerLineIndex = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("│"))
                {
                    headerLineIndex = i;
                    break;
                }
            }

            if (headerLineIndex == -1)
                throw new FormatException("Не удалось найти строку заголовков в МСА.");

            var headerLine = lines[headerLineIndex];
            var columnHeaders = ParseLine(headerLine);
            
            if (columnHeaders.Count < 2)
                throw new FormatException("Некорректный формат заголовков МСА.");

            var targetIds = columnHeaders.Skip(1).Select(h => h.Trim()).ToList();
            var result = new ParsedMAS();
            
            foreach (var t in targetIds)
                result.AllVertexIds.Add(t);

            for (int i = headerLineIndex + 1; i < lines.Count; i++)
            {
                var line = lines[i];
                if (!line.StartsWith("│")) continue;

                var cells = ParseLine(line);
                if (cells.Count == 0) continue;

                var sourceId = cells[0].Trim();
                if (string.IsNullOrEmpty(sourceId)) continue;

                result.AllVertexIds.Add(sourceId);

                var row = new ParsedRow { SourceId = sourceId };
                
                for (int j = 0; j < targetIds.Count; j++)
                {
                    string formula = j + 1 < cells.Count ? cells[j + 1].Trim() : "";
                    
                    if (!string.IsNullOrEmpty(formula))
                    {
                        row.Cells.Add(new ParsedCell 
                        { 
                            TargetId = targetIds[j], 
                            Formula = formula 
                        });
                    }
                }
                result.Rows.Add(row);
            }

            return result;
        }

        private static List<string> ParseLine(string line)
        {
            if (line.StartsWith("│")) line = line.Substring(1);
            if (line.EndsWith("│")) line = line.Substring(0, line.Length - 1);
            return line.Split('│').ToList();
        }

        private static void BuildConditionTree(AbstractAutomata model, IBDVertex source, List<(string Formula, IBDVertex Target)> transitions, ConversionContext ctx)
        {
            var unconditional = transitions.FirstOrDefault(t => t.Formula == "1");
            if (unconditional.Target != null)
            {
                model.LinkNext(source, unconditional.Target);
                return;
            }

            var allVariables = new HashSet<string>();
            var parsedFormulas = new List<(LogicNode Node, IBDVertex Target)>();

            foreach (var t in transitions)
            {
                string f = t.Formula.Replace("˄", "&").Replace("˅", "|").Replace("¬", "!");
                
                try 
                {
                    var node = ExpressionParser.Parse(f);
                    parsedFormulas.Add((node, t.Target));
                    ExtractVariables(node, allVariables);
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Ошибка парсинга формулы '{t.Formula}': {ex.Message}");
                }
            }

            // Используем эвристику сортировки переменных для минимизации размера BDD и восстановления оригинальной структуры.
            // Сортировка по способности переменной разделять множества целевых вершин (Min Target Intersection).
            var sortedVars = SortVariablesBySeparationPower(allVariables, parsedFormulas);
            
            // Определяем fallback вершину на случай неполных условий
            // Если условия не покрывают все случаи, подразумевается, что автомат остается в текущем состоянии (цикл).
            // Это особенно важно для Yн, где неявный цикл означает повторную проверку условий (как в ЛСА).
            var fallback = source;

            var root = BuildRecursive(model, sortedVars, 0, parsedFormulas, new Dictionary<string, bool>(), ctx, fallback);
            
            if (root != null)
                model.LinkNext(source, root);
        }

        private static void ExtractVariables(LogicNode node, HashSet<string> vars)
        {
            var expr = new LogicalExpression(node);
            foreach (var v in expr.Variables)
                vars.Add(v);
        }

        private static IBDVertex BuildRecursive(
            AbstractAutomata model, 
            List<string> vars, 
            int varIndex, 
            List<(LogicNode Node, IBDVertex Target)> formulas,
            Dictionary<string, bool> context,
            ConversionContext ctx,
            IBDVertex fallback)
        {
            if (varIndex >= vars.Count)
            {
                foreach (var item in formulas)
                {
                    var expr = new LogicalExpression(item.Node);
                    if (expr.Evaluate(context))
                    {
                        return item.Target;
                    }
                }
                // Если ни одна формула не подошла, возвращаем fallback
                return fallback; 
            }

            var currentVarName = vars[varIndex];
            
            // Ветка True (1)
            context[currentVarName] = true;
            var trueBranch = BuildRecursive(model, vars, varIndex + 1, formulas, context, ctx, fallback);

            // Ветка False (0)
            context[currentVarName] = false;
            var falseBranch = BuildRecursive(model, vars, varIndex + 1, formulas, context, ctx, fallback);
            
            context.Remove(currentVarName);

            // Редукция (BDD style)
            if (trueBranch == falseBranch)
            {
                return trueBranch;
            }

            // Дедупликация узлов
            var key = (currentVarName, trueBranch, falseBranch);
            if (ctx.NodeCache.TryGetValue(key, out var existing))
            {
                return existing;
            }

            ParseVarName(currentVarName, out string prefix, out int number);
            
            var condVertex = new ConditionalVertex(prefix, number);
            model.AddVertex(condVertex);
            // В AbstractAutomata: RBS - ветка True (↑), LBS - ветка False (→)
            model.SetConditionalBranches(condVertex, falseBranch, trueBranch);

            ctx.NodeCache[key] = condVertex;
            return condVertex;
        }

        private static List<string> SortVariablesBySeparationPower(
            HashSet<string> allVariables, 
            List<(LogicNode Node, IBDVertex Target)> formulas)
        {
            // 1. Calculate MinIndex and Frequency for each variable
            // Variables appearing in earlier formulas should be processed first.
            // Variables appearing in more formulas (higher frequency) are more "central".
            var varMinIndex = new Dictionary<string, int>();
            var varFrequency = new Dictionary<string, int>();
            
            foreach (var v in allVariables) 
            {
                varMinIndex[v] = int.MaxValue;
                varFrequency[v] = 0;
            }

            for (int i = 0; i < formulas.Count; i++)
            {
                var varsInFormula = new HashSet<string>();
                ExtractVariables(formulas[i].Node, varsInFormula);
                foreach (var v in varsInFormula)
                {
                    if (i < varMinIndex[v])
                        varMinIndex[v] = i;
                    
                    varFrequency[v]++;
                }
            }

            var scores = new List<(string Var, int MinIndex, int Score, int Frequency)>();

            foreach (var v in allVariables)
            {
                var trueTargets = new HashSet<IBDVertex>();
                var falseTargets = new HashSet<IBDVertex>();

                foreach (var f in formulas)
                {
                    if (IsSatisfiable(f.Node, v, true))
                        trueTargets.Add(f.Target);
                    
                    if (IsSatisfiable(f.Node, v, false))
                        falseTargets.Add(f.Target);
                }

                int intersection = trueTargets.Intersect(falseTargets).Count();
                int totalSize = trueTargets.Count + falseTargets.Count;
                
                // Primary: Minimize Intersection
                // Secondary: Minimize Total Size (prefer splits that eliminate more targets)
                int score = intersection * 1000 + totalSize;
                
                scores.Add((v, varMinIndex[v], score, varFrequency[v]));
            }

            // Change sort order: 
            // 1. Score (Separation Power) - Essential for BDD size
            // 2. MinIndex (Position) - Respect original structure order
            // 3. Frequency (Importance) - Break ties by picking most used variables (Information Gain)
            // 4. Var (Name) - Deterministic fallback
            return scores.OrderBy(x => x.Score)
                         .ThenBy(x => x.MinIndex)
                         .ThenByDescending(x => x.Frequency)
                         .ThenBy(x => x.Var)
                         .Select(x => x.Var)
                         .ToList();
        }

        private static bool IsSatisfiable(LogicNode node, string varName, bool varValue)
        {
            // Returns true if the node CAN evaluate to TRUE given varName == varValue.
            // All other variables are considered "unknown" (can be true or false).
            
            if (node is VariableNode v)
            {
                if (v.Name == varName) return varValue;
                return true; // Unknown variable can be chosen to make this true
            }
            
            if (node is UnaryNode u)
            {
                // !A is satisfiable if A is falsifiable
                string op = u.Operator.ToString();
                if (op == "!" || op == "~" || op == "¬")
                {
                    return IsFalsifiable(u.Operand, varName, varValue);
                }
                return true; // Unknown operator
            }
            
            if (node is BinaryNode b)
            {
                string op = b.Operator.ToString();
                if (op == "&" || op == "˄" || op == "*")
                {
                    // A & B is satisfiable if BOTH are satisfiable
                    return IsSatisfiable(b.Left, varName, varValue) && IsSatisfiable(b.Right, varName, varValue);
                }
                if (op == "|" || op == "˅" || op == "+")
                {
                    // A | B is satisfiable if EITHER is satisfiable
                    return IsSatisfiable(b.Left, varName, varValue) || IsSatisfiable(b.Right, varName, varValue);
                }
            }
            
            return true; 
        }

        private static bool IsFalsifiable(LogicNode node, string varName, bool varValue)
        {
            // Returns true if the node CAN evaluate to FALSE given varName == varValue.
            
            if (node is VariableNode v)
            {
                if (v.Name == varName) return !varValue;
                return true; // Unknown variable can be chosen to make this false
            }
            
            if (node is UnaryNode u)
            {
                // !A is falsifiable if A is satisfiable
                string op = u.Operator.ToString();
                if (op == "!" || op == "~" || op == "¬")
                {
                    return IsSatisfiable(u.Operand, varName, varValue);
                }
                return true; 
            }
            
            if (node is BinaryNode b)
            {
                string op = b.Operator.ToString();
                if (op == "&" || op == "˄" || op == "*")
                {
                    // A & B is falsifiable if EITHER is falsifiable
                    return IsFalsifiable(b.Left, varName, varValue) || IsFalsifiable(b.Right, varName, varValue);
                }
                if (op == "|" || op == "˅" || op == "+")
                {
                    // A | B is falsifiable if BOTH are falsifiable
                    return IsFalsifiable(b.Left, varName, varValue) && IsFalsifiable(b.Right, varName, varValue);
                }
            }
            
            return true;
        }

        private static void ParseVarName(string varName, out string prefix, out int number)
        {
            var match = Regex.Match(varName, @"^([XPxp])(\d+)$");
            if (match.Success)
            {
                prefix = match.Groups[1].Value.ToUpper();
                number = int.Parse(match.Groups[2].Value);
            }
            else
            {
                prefix = "X"; 
                number = 0; 
                if (int.TryParse(string.Join("", varName.Where(char.IsDigit)), out int n))
                    number = n;
            }
        }
    }
}
