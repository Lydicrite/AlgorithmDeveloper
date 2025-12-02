using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;
using AlgorithmDeveloper.AAModel;
using AlgorithmDeveloper.AAModel.TransitionSystem.MAS;
using LogicalExpressions.Parsing;
using LogicalExpressions.Core;

namespace AlgorithmDeveloper.AAModel.TransitionSystem.Converters
{
    /// <summary>
    /// Конвертер для восстановления модели абстрактного автомата (AbstractAutomaton) 
    /// из матричной схемы алгоритма (MatrixAlgorithmSchema) или системы формул перехода.
    /// </summary>
    public static class MSAToAAConverter
    {
        /// <summary>
        /// Генерирует AbstractAutomaton на основе переданной МСА.
        /// </summary>
        public static AbstractAutomaton Convert(MatrixAlgorithmSchema mas)
        {
            if (mas == null) throw new ArgumentNullException(nameof(mas));

            var model = new AbstractAutomaton();
            var formulas = mas.TransitionFormulas;

            // 1. Создание вершин (Start, End, Operator)
            // Собираем все идентификаторы вершин, которые есть в формулах
            var vertexIds = new HashSet<string>(StringComparer.Ordinal);

            // YStart всегда есть в формулах
            foreach (var f in formulas)
            {
                if (f.YStart.ID != null)
                    vertexIds.Add(f.YStart.ID);
            }
            // YTo в условиях
            foreach (var f in formulas)
            {
                foreach (var c in f.Conditions)
                {
                    if (c.YTo.ID != null)
                        vertexIds.Add(c.YTo.ID);
                }
            }

            var vertexMap = new Dictionary<string, IBDVertex>(StringComparer.Ordinal);

            foreach (var id in vertexIds)
            {
                IBDVertex? v = null;
                if (id == "Yн")
                {
                    v = new StartVertex();
                }
                else if (id == "Yк")
                {
                    v = new EndVertex();
                }
                else if (id.StartsWith("Y"))
                {
                    if (int.TryParse(id.Substring(1), out int idx))
                    {
                        v = new OperatorVertex(idx);
                    }
                }

                if (v != null)
                {
                    model.AddVertex(v);
                    vertexMap[id] = v;
                }
            }

            // 2. Построение логики переходов для каждой исходной вершины
            // var treeCache = new Dictionary<string, IBDVertex>(); // Move inside to avoid pollution

            foreach (var f in formulas)
            {
                var treeCache = new Dictionary<string, IBDVertex>();

                var startId = f.YStart.ID;
                if (startId == null || !vertexMap.TryGetValue(startId, out var startVertex))
                    continue;

                // Группируем условия по целевой вершине: TargetID -> List<ExpressionString>
                // Если несколько условий ведут в одну вершину, объединяем их через OR (v)
                var transitions = new List<(string TargetId, string Expression)>();
                
                var groups = f.Conditions
                    .GroupBy(c => c.YTo.ID)
                    .Where(g => g.Key != null);

                foreach (var g in groups)
                {
                    // Собираем выражения для данной цели.
                    // Каждое Condition.LogicalExpression — это конъюнкция (AND).
                    // Если их несколько, значит (AND) OR (AND).
                    var exprs = g.Select(c => SanitizeExpression(c.LogicalExpression)).Where(e => !string.IsNullOrEmpty(e)).ToList();
                    
                    // Пустое выражение в списке условий обычно означает "Безусловный переход" (1),
                    // но TransitionCondition.Expression возвращает пустую строку только если условий нет.
                    // Если условий нет, значит это "1".
                    if (g.Any(c => c.XValues.Count == 0))
                    {
                        exprs.Add("1"); 
                    }

                    if (exprs.Count > 0)
                    {
                        // Объединяем через OR
                        string combined = exprs.Count == 1 ? exprs[0] : $"({string.Join(") | (", exprs)})";
                        transitions.Add((g.Key!, combined));
                    }
                }

                // Строим дерево решений
                if (transitions.Count > 0)
                {
                    var nextNode = BuildDecisionTree(model, transitions, vertexMap, treeCache);
                    if (nextNode != null)
                    {
                        model.LinkNext(startVertex, nextNode);
                    }
                }
            }

            // 3. Вставка точек перехода (JumpPoints)
            // Правило 1: Точка перехода как единственная вершина по левой ветви ConditionalVertex.
            // Правило 2: Точка перехода в местах слияния 2 и более путей.

            ApplyJumpPoints(model);

            model.Update(); // Обновляем внутренние структуры
            return model;
        }

        private static string SanitizeExpression(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr)) return "1";
            // Замена спецсимволов на стандартные операторы C#/LogicalExpressions
            // ˄ -> & (AND)
            // ˅ -> | (OR)
            // ¬ -> ! (NOT)
            return expr.Replace("˄", "&")
                       .Replace("˅", "|")
                       .Replace("¬", "!");
        }

        /// <summary>
        /// Строит дерево условных вершин, ведущее к целевым вершинам.
        /// </summary>
        private static IBDVertex? BuildDecisionTree(AbstractAutomaton model, List<(string TargetId, string Expression)> transitions, Dictionary<string, IBDVertex> vertexMap, Dictionary<string, IBDVertex> cache)
        {
            // 1. Проверка на безусловный переход / тавтологию
            foreach (var t in transitions)
            {
                if (IsTautology(t.Expression))
                {
                    return vertexMap.ContainsKey(t.TargetId) ? vertexMap[t.TargetId] : null;
                }
            }

            // 2. Если переходов нет
            if (transitions.Count == 0) return null;

            // Кэширование
            var ordered = transitions.OrderBy(t => t.TargetId, StringComparer.Ordinal)
                                     .ThenBy(t => t.Expression, StringComparer.Ordinal);
            string key = string.Join(";", ordered.Select(t => $"{t.TargetId}:{t.Expression}"));

            if (cache.TryGetValue(key, out var cached)) return cached;

            // 3. Выбор переменной для ветвления
            var variables = GetVariables(transitions);
            if (variables.Count == 0)
            {
                // Если переменных нет, но мы здесь — значит выражения константные, но не "1".
                // Это может быть "0", который мы должны игнорировать, или ошибка логики.
                // Возвращаем первый попавшийся, если он не 0.
                // Но по идее IsTautology выше должен был поймать "1".
                return null; 
            }

            var splitVar = variables.First(); // Берем первую по алфавиту (или порядку)

            // 4. Разделение на ветви
            // True Branch (Variable = 1)
            var trueTransitions = EvaluateTransitions(transitions, splitVar, true);
            // False Branch (Variable = 0)
            var falseTransitions = EvaluateTransitions(transitions, splitVar, false);

            var rbs = BuildDecisionTree(model, trueTransitions, vertexMap, cache); // Right = True
            var lbs = BuildDecisionTree(model, falseTransitions, vertexMap, cache); // Left = False

            // Оптимизация: если обе ветви ведут в одно и то же место
            if (rbs == lbs && rbs != null)
            {
                cache[key] = rbs;
                return rbs;
            }

            // Создаем условную вершину
            // Парсим имя переменной (X1, P2...)
            var match = Regex.Match(splitVar, @"^([XP])(\d+)$");
            string prefix = match.Success ? match.Groups[1].Value : "X";
            int index = match.Success ? int.Parse(match.Groups[2].Value) : 0;

            // Важно: создаем новую вершину каждый раз, так как это дерево разбора.
            // Дубликаты условных вершин допустимы в модели (разные вхождения).
            var condVertex = model.AddVertex(new ConditionalVertex(prefix, index));
            model.SetConditionalBranches(condVertex, lbs, rbs);
            
            cache[key] = condVertex;
            return condVertex;
        }

        private static List<string> GetVariables(List<(string TargetId, string Expression)> transitions)
        {
            var vars = new HashSet<string>();
            foreach (var t in transitions)
            {
                // Парсим выражение, извлекаем переменные
                try
                {
                    var node = ExpressionParser.Parse(t.Expression);
                    // Простой обход дерева или использование API, если есть.
                    // LogicalExpression имеет свойство Variables, но оно требует создания экземпляра.
                    var le = new LogicalExpression(node);
                    foreach (var v in le.Variables) vars.Add(v);
                }
                catch { /* игнорируем ошибки парсинга */ }
            }
            return vars.OrderBy(v => v, StringComparer.Ordinal).ToList();
        }

        private static bool IsTautology(string expression)
        {
            try
            {
                if (expression == "1") return true;
                var node = ExpressionParser.Parse(expression);
                var le = new LogicalExpression(node);
                return le.IsTautology();
            }
            catch
            {
                return false;
            }
        }

        private static List<(string TargetId, string Expression)> EvaluateTransitions(List<(string TargetId, string Expression)> transitions, string variable, bool value)
        {
            var result = new List<(string TargetId, string Expression)>();
            string valStr = value ? "1" : "0";

            foreach (var t in transitions)
            {
                // Заменяем переменную на константу
                // Используем Regex для точной замены слова
                string newExpr = Regex.Replace(t.Expression, $@"\b{variable}\b", valStr);
                
                // Пытаемся упростить
                try 
                {
                    var node = ExpressionParser.Parse(newExpr);
                    var le = new LogicalExpression(node);
                    
                    if (le.IsContradiction()) continue; // Выражение ложно, переход невозможен
                    
                    // Если выражение стало "True", заменяем на "1"
                    if (le.IsTautology())
                    {
                        result.Add((t.TargetId, "1"));
                    }
                    else
                    {
                        // Иначе нормализуем или минимизируем
                        var simplified = le.Minimize();
                        // Получаем строку обратно.
                        // LogicalExpression.ToString() обычно возвращает формулу.
                        result.Add((t.TargetId, simplified.ToString()));
                    }
                }
                catch
                {
                    // Если ошибка, оставляем как есть (на всякий случай)
                    result.Add((t.TargetId, newExpr));
                }
            }
            return result;
        }

        private static void ApplyJumpPoints(AbstractAutomaton model)
        {
            // 1. Идентификация точек слияния (InDegree > 1)
            // Собираем входящие ребра для всех вершин
            var inEdges = new Dictionary<IBDVertex, List<IBDVertex>>();
            foreach (var v in model.Vertices)
            {
                if (!inEdges.ContainsKey(v)) inEdges[v] = new List<IBDVertex>();
                
                var next = model.GetNext(v); // Осторожно, GetNext для Conditional зависит от Value.
                // Нам нужно структурное next.
                // Для ConditionalVertex это LBS и RBS.
                // Для остальных - Next.
                
                if (v is ConditionalVertex cv)
                {
                    if (cv.LBS != null) AddEdge(inEdges, cv, cv.LBS);
                    if (cv.RBS != null) AddEdge(inEdges, cv, cv.RBS);
                }
                else if (v.Next != null)
                {
                    AddEdge(inEdges, v, v.Next);
                }
            }

            // Счетчик для индексов JumpPoint
            int jumpIndexCounter = 1;

            // Список изменений, чтобы не ломать итератор
            // Target -> JumpPoint
            var mergeJumpPoints = new Dictionary<IBDVertex, JumpPoint>();

            foreach (var kv in inEdges)
            {
                var target = kv.Key;
                var sources = kv.Value;

                if (target is StartVertex) continue; // В Yн нельзя войти (обычно)
                
                // Если более 1 входящего пути ИЛИ (цель - оператор/конец/Start? и т.д.)
                // Правило "2 и более путей"
                if (sources.Count > 1)
                {
                    // Создаем JumpPoint
                    var jp = model.AddVertex(new JumpPoint(jumpIndexCounter++));
                    mergeJumpPoints[target] = jp;
                    
                    // Перенаправляем источники на JP
                    foreach (var src in sources)
                    {
                        RedirectNext(model, src, target, jp);
                    }
                    // JP указывает на Target
                    model.LinkNext(jp, target);
                }
            }

            // 2. Правило левой ветви: LBS у ConditionalVertex должен быть JumpPoint
            // Итерируемся по всем ConditionalVertex
            // Нужно учитывать, что мы могли уже вставить JumpPoint на шаге 1.
            
            var conditionals = model.Vertices.OfType<ConditionalVertex>().ToList();
            foreach (var cv in conditionals)
            {
                if (cv.LBS == null) continue;

                var target = cv.LBS;
                
                // Проверяем, является ли target уже JumpPoint
                if (target is JumpPoint) continue;

                // Если нет, вставляем JumpPoint
                // Но нужно проверить, не вставили ли мы его на предыдущем шаге (mergeJumpPoints)
                // Если target был в mergeJumpPoints, то cv.LBS уже должен указывать на JP (через RedirectNext).
                // Но мы итерируемся по `conditionals` из `model.Vertices`. RedirectNext меняет свойства LBS/RBS.
                // Так что cv.LBS уже обновлен.
                
                // Если после обновления это всё ещё не JumpPoint, значит InDegree было 1.
                // Создаем эксклюзивный JumpPoint для этой ветви.
                var jp = model.AddVertex(new JumpPoint(jumpIndexCounter++));
                
                // Перенаправляем LBS
                model.SetConditionalBranches(cv, jp, cv.RBS);
                model.LinkNext(jp, target);
            }
        }

        private static void AddEdge(Dictionary<IBDVertex, List<IBDVertex>> inEdges, IBDVertex from, IBDVertex to)
        {
            if (!inEdges.ContainsKey(to)) inEdges[to] = new List<IBDVertex>();
            inEdges[to].Add(from);
        }

        private static void RedirectNext(AbstractAutomaton model, IBDVertex src, IBDVertex oldTarget, IBDVertex newTarget)
        {
            if (src is ConditionalVertex cv)
            {
                if (ReferenceEquals(cv.LBS, oldTarget))
                    cv.LBS = newTarget;
                if (ReferenceEquals(cv.RBS, oldTarget))
                    cv.RBS = newTarget;
            }
            else
            {
                if (ReferenceEquals(src.Next, oldTarget))
                    src.Next = newTarget;
            }
        }
    }
}
