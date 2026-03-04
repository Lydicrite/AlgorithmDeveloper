using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using LogicalExpressions;
using LogicalExpressions.Core;
using LogicalExpressions.Parsing;
using LogicalExpressions.Core.Nodes;

namespace AlgorithmDeveloper.Abstractions.TransitionSystem.MAS
{
    /// <summary>
    /// Инструмент для объединения нескольких моделей алгоритмов (АА) через их матричные схемы (МСА).
    /// Реализует методику, описанную в "Объединение АА через МСА.md".
    /// </summary>
    public static class MASCombiner
    {
        /// <summary>
        /// Объединяет несколько моделей алгоритмов в одну.
        /// </summary>
        /// <param name="models">Список исходных моделей.</param>
        /// <returns>Объединенная модель.</returns>
        public static AbstractAutomata Combine(params AbstractAutomata[] models)
        {
            if (models == null || models.Length == 0)
                throw new ArgumentException("Список моделей для объединения пуст.");
            if (models.Length == 1)
                return models[0]; // TODO: Should we clone it? For now, return as is or maybe clone via MAS roundtrip.
            
            // 1. Преобразуем модели в структуру данных для манипуляций (MASData)
            var schemas = models.Select(m => 
            {
                // Убедимся, что МСА актуальна
                m.Update(); 
                return MASData.FromMAS(m.MAS!);
            }).ToList();

            // 2. Упорядочиваем матрицы по схожести (жадный алгоритм)
            var orderedSchemas = OrderBySimilarity(schemas);

            // 2.5. Находим максимальный индекс P-переменных во всех исходных моделях
            int maxPIndex = -1;
            foreach (var m in models)
            {
                foreach (var v in m.Vertices.OfType<ConditionalVertex>())
                {
                    if (string.Equals(v.Prefix, "P", StringComparison.OrdinalIgnoreCase))
                    {
                        if (v.Index > maxPIndex)
                            maxPIndex = v.Index;
                    }
                }
            }
            // Начальный индекс для новых переменных кодирования
            int startPIndex = maxPIndex + 1;

            // 3. Генерируем коды Грея и модифицируем матрицы
            int q = orderedSchemas.Count;
            int n = (int)Math.Ceiling(Math.Log2(q));
            // Если q=1, n=0. Но мы объединяем >=2, так что n>=1.
            
            var modifiedSchemas = new List<MASData>();
            for (int i = 0; i < q; i++)
            {
                var code = GetGrayCode(i, n);
                var codingCondition = BuildCodingCondition(code, n, startPIndex);
                var mod = ModifySchema(orderedSchemas[i], codingCondition);
                modifiedSchemas.Add(mod);
            }

            // 4. Строим объединенную матрицу (Union)
            var unionSchema = UnionSchemas(modifiedSchemas);

            // 5. Минимизация и упрощение (включая Shift Distribution)
            // Важно: SimplifySchema должен знать о НОВЫХ переменных P, чтобы пытаться их оптимизировать.
            // Но метод SimplifySchema принимает nVars (количество переменных P0..Pn-1).
            // Сейчас переменные смещены на startPIndex.
            // Нужно адаптировать SimplifySchema, чтобы он работал с P[startPIndex] ... P[startPIndex + n - 1].
            SimplifySchema(unionSchema, startPIndex, n);

            // 6. Конвертация в строку МСА и затем в АА
            var masString = unionSchema.ToString();
            // Отключаем оптимизацию конвертера, так как она может конфликтовать с оптимизациями МСА
            // и приводить к некорректному определению достижимости в сложных случаях с общими вершинами.
            var resultAA = MASToAAConverter.Convert(masString, optimize: false);
            
            return resultAA;
        }





        #region Внутренние структуры и методы

        private class MASData
        {
            public List<string> Headers { get; set; } = new();
            // (Source, Target) -> Formula
            public Dictionary<(string, string), string> Cells { get; set; } = new();

            public static MASData FromMAS(MatrixAlgorithmSchema mas)
            {
                var data = new MASData
                {
                    Headers = mas.Headers.ToList()
                };

                for (int r = 0; r < mas.Rows.Count; r++)
                {
                    var row = mas.Rows[r];
                    var source = row.Header;
                    for (int c = 1; c < mas.Headers.Count; c++) // Skip 0 (Header column)
                    {
                        var target = mas.Headers[c];
                        if (c < row.Transitions.Count)
                        {
                            var val = row.Transitions[c];
                            if (!string.IsNullOrWhiteSpace(val))
                            {
                                data.Cells[(source, target)] = val.Trim();
                            }
                        }
                    }
                }
                return data;
            }

            public override string ToString()
            {
                // Генерация строкового представления таблицы МСА
                var sb = new StringBuilder();

                // 1. Вычисляем ширину столбцов
                var colWidths = new int[Headers.Count];
                // Заголовки
                for (int i = 0; i < Headers.Count; i++)
                    colWidths[i] = Math.Max(colWidths[i], Headers[i].Length + 2);
                
                // Данные
                // Строки идут в том же порядке, что и Headers (обычно), но для надежности берем Headers как источник строк
                // МСА квадратная по заголовкам? Обычно да.
                foreach (var src in Headers)
                {
                    colWidths[0] = Math.Max(colWidths[0], src.Length + 2);
                    for (int i = 1; i < Headers.Count; i++)
                    {
                        var tgt = Headers[i];
                        string cell = Cells.TryGetValue((src, tgt), out var f) ? f : "";
                        colWidths[i] = Math.Max(colWidths[i], cell.Length + 2);
                    }
                }

                // Вспомогательные методы для рисования
                string Border(string left, string mid, string right, char fill) =>
                    left + string.Join(mid, colWidths.Select(w => new string(fill, w))) + right;
                
                string Row(string[] values)
                {
                    var parts = new List<string>();
                    for(int i=0; i<values.Length; i++)
                        parts.Add(Center(values[i], colWidths[i]));
                    return "│" + string.Join("│", parts) + "│";
                }

                sb.AppendLine(Border("┌", "┬", "┐", '─'));
                
                // Заголовки (первая ячейка пустая или Yн?)
                // В оригинальном ToString() Headers[0] пропускается в заголовках колонок, там пустая ячейка
                var headerVals = new string[Headers.Count];
                headerVals[0] = ""; 
                for(int i=1; i<Headers.Count; i++) headerVals[i] = Headers[i];
                sb.AppendLine(Row(headerVals));
                
                sb.AppendLine(Border("├", "┼", "┤", '─'));

                for (int r = 0; r < Headers.Count; r++)
                {
                    var src = Headers[r];
                    var vals = new string[Headers.Count];
                    vals[0] = src;
                    for (int c = 1; c < Headers.Count; c++)
                    {
                        var tgt = Headers[c];
                        vals[c] = Cells.TryGetValue((src, tgt), out var f) ? f : "";
                    }
                    sb.AppendLine(Row(vals));
                    
                    if (r < Headers.Count - 1)
                        sb.AppendLine(Border("├", "┼", "┤", '─'));
                }

                sb.AppendLine(Border("└", "┴", "┘", '─'));
                return sb.ToString();
            }

            private static string Center(string s, int width)
            {
                if (s.Length >= width) return s;
                int pad = width - s.Length;
                int l = pad / 2;
                return new string(' ', l) + s + new string(' ', pad - l);
            }
        }

        private static List<MASData> OrderBySimilarity(List<MASData> list)
        {
            if (list.Count <= 2) return list;

            var result = new List<MASData>();
            var remaining = new HashSet<MASData>(list);

            // Начинаем с первой (как в описании методики: "Выбирается первая матрица")
            var current = list[0];
            result.Add(current);
            remaining.Remove(current);

            while (remaining.Count > 0)
            {
                MASData? bestNext = null;
                int maxSim = -1;

                foreach (var cand in remaining)
                {
                    int sim = CalculateSimilarity(current, cand);
                    if (sim > maxSim)
                    {
                        maxSim = sim;
                        bestNext = cand;
                    }
                }

                if (bestNext != null)
                {
                    result.Add(bestNext);
                    remaining.Remove(bestNext);
                    current = bestNext;
                }
                else
                {
                    // Should not happen unless remaining is empty
                    break;
                }
            }

            return result;
        }

        private static int CalculateSimilarity(MASData a, MASData b)
        {
            int score = 0;
            // Пересечение строк и столбцов
            var commonRows = a.Headers.Intersect(b.Headers).ToList();
            var commonCols = a.Headers.Intersect(b.Headers).ToList(); // Headers includes source column, effectively vertices

            foreach (var src in commonRows)
            {
                foreach (var tgt in commonCols)
                {
                    if (src == tgt) continue; // Usually diagonals are empty or loops, check logic? Logic says (Yi, Yj).
                    
                    bool hasA = a.Cells.TryGetValue((src, tgt), out var fA);
                    bool hasB = b.Cells.TryGetValue((src, tgt), out var fB);

                    if (hasA && hasB)
                    {
                        if (IsSignificant(fA) && IsSignificant(fB) && fA == fB)
                            score++;
                    }
                }
            }
            return score;
        }

        private static bool IsSignificant(string? f)
        {
            if (string.IsNullOrWhiteSpace(f)) return false;
            f = f!.Trim();
            return f != "0" && f != "1";
        }

        private static int GetGrayCode(int index, int n)
        {
            // Binary to Gray: num ^ (num >> 1)
            // Но нам нужна последовательность.
            // Стандартная последовательность кодов Грея для n бит.
            // Или просто берем index-й элемент последовательности?
            // "Каждой матрице... присваивается код Грея Cq... соседние коды отличаются в одном разряде".
            // Значит, нам нужна функция Gray(i).
            return index ^ (index >> 1);
        }

        private static string BuildCodingCondition(int grayCode, int n, int startPIndex)
        {
            // Формируем конъюнкцию P-переменных, начиная с startPIndex
            var parts = new List<string>();
            for (int i = 0; i < n; i++)
            {
                // Переменные Pk, где k = startPIndex + i
                int pIndex = startPIndex + i;
                bool val = ((grayCode >> i) & 1) == 1;
                string p = $"P{pIndex}";
                parts.Add(val ? p : $"¬{p}");
            }
            return string.Join(" ˄ ", parts);
        }

        private static MASData ModifySchema(MASData original, string condition)
        {
            var mod = new MASData { Headers = new List<string>(original.Headers) };
            foreach (var kv in original.Cells)
            {
                string f = kv.Value;
                if (f == "1")
                    mod.Cells[kv.Key] = condition;
                else
                    mod.Cells[kv.Key] = $"({f}) ˄ ({condition})";
            }
            return mod;
        }

        private static MASData UnionSchemas(List<MASData> schemas)
        {
            var union = new MASData();
            
            // Объединение заголовков (вершин)
            var allHeaders = new HashSet<string>();
            foreach (var s in schemas)
                foreach (var h in s.Headers)
                    allHeaders.Add(h);
            
            // Сортировка заголовков: Yн, Y0..Yn, Yк
            union.Headers = SortHeaders(allHeaders);

            // Объединение ячеек
            // Для каждой пары (src, tgt) из UnionHeaders собираем формулы из всех схем
            foreach (var src in union.Headers)
            {
                foreach (var tgt in union.Headers)
                {
                    var formulas = new List<string>();
                    foreach (var s in schemas)
                    {
                        if (s.Cells.TryGetValue((src, tgt), out var f))
                            formulas.Add(f);
                    }

                    if (formulas.Count > 0)
                    {
                        // Объединяем через ИЛИ
                        // Optimization: if any is "1" (should not happen after modification usually, unless code condition is 1?), result is 1?
                        // But formulas are (f & P). 
                        union.Cells[(src, tgt)] = string.Join(" ˅ ", formulas);
                    }
                }
            }

            return union;
        }

        private static List<string> SortHeaders(HashSet<string> headers)
        {
            var list = new List<string>();
            bool hasStart = headers.Contains("Yн");
            bool hasEnd = headers.Contains("Yк");
            
            if (hasStart) list.Add("Yн");
            
            var ops = headers
                .Where(h => h != "Yн" && h != "Yк")
                .OrderBy(h => 
                {
                    // Y1, Y2, Y10... Ordinal sort might be Y1, Y10, Y2. Need numeric.
                    if (h.StartsWith("Y") && int.TryParse(h.Substring(1), out int n))
                        return n;
                    return int.MaxValue;
                })
                .ToList();
            list.AddRange(ops);

            if (hasEnd) list.Add("Yк");
            
            return list;
        }

        private static void SimplifySchema(MASData schema, int startPIndex, int nVars)
        {
            // 1. Сначала просто логическое упрощение всех ячеек
            var keys = schema.Cells.Keys.ToList();
            foreach (var key in keys)
            {
                schema.Cells[key] = SimplifyFormula(schema.Cells[key]);
            }

            // 2. Shift Distribution (оптимизация по столбцам)
            // Анализируем каждый P_k (от startPIndex до startPIndex + nVars - 1)
            for (int k = startPIndex; k < startPIndex + nVars; k++)
            {
                string pName = $"P{k}";
                string notPName = $"¬P{k}"; // В нашей нотации '¬'

                // Проходим по всем столбцам (Target)
                // Исключаем Yн (в него обычно не входят, но если входят - ок)
                foreach (var target in schema.Headers)
                {
                    // Собираем использование P_k во всех входящих в Target переходах
                    bool? commonVal = null; // null = еще не встретили, true = P, false = !P
                    bool conflict = false;

                    // Ищем входящие: это ячейки (*, target)
                    foreach (var source in schema.Headers)
                    {
                        if (!schema.Cells.TryGetValue((source, target), out var f)) continue;
                        
                        // Анализируем формулу f на наличие P_k
                        // Простой анализ строки может быть ненадежен (например P1 vs P12), но у нас P1..PN.
                        // Используем парсер или токенизацию?
                        // Лучше просто проверить вхождение токена.
                        
                        bool hasPos = ContainsToken(f, pName);
                        bool hasNeg = ContainsToken(f, notPName) || ContainsToken(f, "!" + pName);

                        if (hasPos && hasNeg)
                        {
                            conflict = true; break; // В одной формуле и то и то? (парадокс или 0), либо разные ветки.
                        }
                        if (!hasPos && !hasNeg)
                        {
                            // Не входит -> не накладывает ограничений? Или "значение не меняется"?
                            // Если не входит, то мы не знаем значение P_k.
                            // Методика: "встречается только в одном виде...".
                            // Если не встречается, то не считается.
                            continue;
                        }

                        bool val = hasPos; // true if P, false if !P
                        if (commonVal == null)
                        {
                            commonVal = val;
                        }
                        else if (commonVal != val)
                        {
                            conflict = true;
                            break;
                        }
                    }

                    if (!conflict && commonVal.HasValue)
                    {
                        // Оптимизация возможна!
                        // Подставляем значение commonVal в строку Target (исходящие переходы)
                        // То есть в формулы Cells[(target, *)] заменяем Pk на 1 (если val) или 0.
                        bool pValue = commonVal.Value;
                        
                        foreach (var dest in schema.Headers)
                        {
                            if (schema.Cells.TryGetValue((target, dest), out var f))
                            {
                                // Заменяем Pk -> 1/0
                                // В строке: Pk -> "1", !Pk -> "0" (если pValue=true)
                                //           Pk -> "0", !Pk -> "1" (если pValue=false)
                                
                                // Аккуратно с заменой, чтобы не сломать P11 при замене P1.
                                // Используем Regex или парсер.
                                // Проще через парсер: Parse -> Eval with context -> String
                                
                                schema.Cells[(target, dest)] = SubstituteAndSimplify(f, pName, pValue);
                            }
                        }
                    }
                }
            }
        }

        private static string SimplifyFormula(string formula)
        {
            try
            {
                var f = formula.Replace("˄", "&").Replace("˅", "|").Replace("¬", "!");
                var node = ExpressionParser.Parse(f);
                var le = new LogicalExpression(node);
                var simple = le.Minimize();
                return simple.ToString().Replace("&", "˄").Replace("|", "˅").Replace("~", "¬").Replace("!", "¬");
            }
            catch
            {
                return formula;
            }
        }

        private static string SubstituteAndSimplify(string formula, string varName, bool val)
        {
            try
            {
                var f = formula.Replace("˄", "&").Replace("˅", "|").Replace("¬", "!");
                var node = ExpressionParser.Parse(f);
                
                // Нам нужно не полностью вычислить, а только подставить одну переменную (Partial Evaluation).
                // LogicalExpression.Evaluate возвращает bool.
                // Можно сделать Simplify с контекстом? Библиотека может не поддерживать.
                // Ручная подстановка в строке и повторный парсинг.
                
                // Заменяем токен varName на "1" или "0".
                // Regex \bName\b
                string pattern = $@"\b{varName}\b";
                string replacement = val ? "1" : "0";
                
                string subst = System.Text.RegularExpressions.Regex.Replace(f, pattern, replacement);
                
                var node2 = ExpressionParser.Parse(subst);
                var le = new LogicalExpression(node2);
                var simple = le.Minimize();
                return simple.ToString().Replace("&", "˄").Replace("|", "˅").Replace("~", "¬").Replace("!", "¬");
            }
            catch
            {
                return formula;
            }
        }

        private static bool ContainsToken(string text, string token)
        {
            // Простая проверка с границами слова, но учитываем '¬'
            // Если token начинается с ¬, он может быть не словом.
            // ¬P1 -> Contains "¬P1".
            return text.Contains(token); 
        }

        #endregion
    }
}
