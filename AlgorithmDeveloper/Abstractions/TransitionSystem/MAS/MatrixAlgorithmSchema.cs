using LogicalExpressions;
using LogicalExpressions.Parsing;
using LogicalExpressions.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.AAModel.Utils;

namespace AlgorithmDeveloper.Abstractions.TransitionSystem.MAS
{
    /// <summary>
    /// Матричная схема алгоритма (МСА) — табличное представление формул переходов.
    /// <br></br>Строки и столбцы соответствуют вершинам Yн, Yi, Yк.
    /// <br></br>В ячейке (Yi, Yj) содержится условие перехода из Yi в Yj.
    /// </summary>
    public sealed class MatrixAlgorithmSchema
    {
        /// <summary>
        /// Строка МСА.
        /// </summary>
        public sealed class MASRow
        {
            public string Header { get; }
            public List<string> Transitions { get; }

            public MASRow(string header, List<string> transitions)
            {
                Header = header;
                Transitions = transitions;
            }
        }

        /// <summary>
        /// Заголовки столбцов и строк (идентификаторы вершин: Yн, Yi, Yк) в едином порядке.
        /// Первый всегда Yн; прочие по возрастанию ID; Yк — последним, если присутствует.
        /// </summary>
        public IReadOnlyList<string> Headers { get; private set; }

        /// <summary>
        /// Строки матрицы. Каждая строка соответствует одной формуле перехода.
        /// </summary>
        public IReadOnlyList<MASRow> Rows { get; private set; }

        /// <summary>
        /// Формулы перехода, представляемые этой МСА.
        /// </summary>
        public IReadOnlyList<TransitionFormula> TransitionFormulas { get; private set; }

        /// <summary>
        /// DataTable для отображения МСА в DataGridView.
        /// </summary>
        public DataTable? DataTable { get; private set; }
       


        private MatrixAlgorithmSchema(IReadOnlyList<string> headers, IReadOnlyList<MASRow> rows, IReadOnlyList<TransitionFormula> formulas)
        {
            Headers = headers;
            Rows = rows;
            TransitionFormulas = formulas;
            DataTable = ToDataTable();
        }

        /// <summary>
        /// Создаёт МСА из набора формул перехода.
        /// </summary>
        /// <param name="formulas">Отсортированный список формул перехода.</param>
        /// <param name="view">Вид строкового представления условий в ячейках (по умолчанию: логический).</param>
        public static MatrixAlgorithmSchema FromFormulas(in List<TransitionFormula> formulas, OutputView view = OutputView.LogicalExpression)
        {
            if (formulas == null) throw new ArgumentNullException(nameof(formulas));

            // Сбор всех вершин для построения порядка обхода
            var allVertices = new HashSet<IBDVertex>();
            foreach (var f in formulas)
            {
                allVertices.Add(f.YStart);
                foreach (var c in f.Conditions)
                {
                    allVertices.Add(c.YTo);
                }
            }

            var startNode = allVertices.OfType<StartVertex>().FirstOrDefault();

            // Создаем сортировщик
            var order = new VertexTraversalOrder(startNode, allVertices);
            var comparer = new VertexComparer(order);

            // 1) Определяем порядок строк: Yн первым, затем по алгоритму
            var ordered = formulas
                .OrderBy(f => f.YStart, comparer)
                .ToList();

            // 2) Заголовки столбцов: все Y-вершины, участвующие в переходах (источники и цели)
            // Исключаем условные вершины, так как в МСА заголовки - это Y
            // (VertexSorter обрабатывает все типы, но здесь нам нужны только Y)
            var yVertices = allVertices
                .Where(v => !(v is ConditionalVertex) && !(v is JumpPoint))
                .OrderBy(v => v, comparer)
                .Select(v => v.ID ?? string.Empty)
                .Distinct()
                .ToList();

            // Если список пуст (странно), возьмем из формул
            if (yVertices.Count == 0 && ordered.Count > 0)
            {
                yVertices = ordered.Select(f => f.YStart.ID ?? string.Empty).ToList();
            }

            var headers = yVertices;

            // 3) Формируем строки матрицы
            var rows = new List<MASRow>(ordered.Count);
            foreach (var f in ordered)
            {
                var rowHeader = f.YStart.ID ?? string.Empty;

                // Группируем условия по целевой вершине
                var byTarget = f.Conditions
                    .GroupBy(c => c.YTo.ID ?? string.Empty)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var cells = new List<string>(headers.Count);
                foreach (var col in headers)
                {
                    byTarget.TryGetValue(col, out var conds);
                    cells.Add(RenderCell(conds, view));
                }

                rows.Add(new MASRow(rowHeader, cells));
            }

            return new MatrixAlgorithmSchema(headers, rows, formulas);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var columnWidths = CalculateColumnWidths();

            // Верхняя граница
            sb.AppendLine(BuildTopBorder(columnWidths));

            // Заголовки столбцов
            sb.AppendLine(BuildHeaderLine(columnWidths));
            sb.AppendLine(BuildMiddleBorder(columnWidths));

            // Строки данных
            for (int i = 0; i < Rows.Count; i++)
            {
                sb.AppendLine(BuildDataLine(Rows[i].Header, Rows[i].Transitions, columnWidths));
                if (i < Rows.Count - 1)
                    sb.AppendLine(BuildMiddleBorder(columnWidths));
            }

            // Нижняя граница
            sb.AppendLine(BuildBottomBorder(columnWidths));

            return sb.ToString();
        }





        #region Индексаторы и вспомогательные методы

        /// <summary>
        /// Возвращает строку МСА по её индексу.
        /// </summary>
        public MASRow this[int rowIndex] => Rows[rowIndex];

        /// <summary>
        /// Возвращает строку МСА по идентификатору стартовой вершины (Header).
        /// Бросает исключение, если строка с таким идентификатором не найдена.
        /// </summary>
        public MASRow this[string vertexId]
        {
            get
            {
                var id = vertexId ?? string.Empty;
                var row = Rows.FirstOrDefault(r => string.Equals(r.Header, id, StringComparison.Ordinal));
                if (row == null)
                    throw new KeyNotFoundException($"Строка МСА с идентификатором '{id}' не найдена.");
                return row;
            }
        }

        /// <summary>
        /// Возвращает текст ячейки по индексам строки и столбца.
        /// Если индекс вне диапазона, выбрасывает исключение.
        /// </summary>
        public string this[int rowIndex, int columnIndex]
        {
            get
            {
                if (rowIndex < 0 || rowIndex >= Rows.Count)
                    throw new ArgumentOutOfRangeException(nameof(rowIndex));
                if (columnIndex < 0 || columnIndex >= Headers.Count)
                    throw new ArgumentOutOfRangeException(nameof(columnIndex));

                var row = Rows[rowIndex];
                return row.Transitions.Count > columnIndex ? row.Transitions[columnIndex] : string.Empty;
            }
        }

        /// <summary>
        /// Возвращает формулу перехода по индексу строки.
        /// </summary>
        public TransitionFormula? GetTransitionFormulaByRowIndex(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return null;
            var id = Rows[rowIndex].Header ?? string.Empty;
            return TransitionFormulas.FirstOrDefault(f => string.Equals(f.YStart.ID ?? string.Empty, id, StringComparison.Ordinal));
        }

        /// <summary>
        /// Возвращает формулу перехода по идентификатору вершины.
        /// </summary>
        public TransitionFormula? GetTransitionFormulaByVertexId(string vertexId)
        {
            var id = vertexId ?? string.Empty;
            return TransitionFormulas.FirstOrDefault(f => string.Equals(f.YStart.ID ?? string.Empty, id, StringComparison.Ordinal));
        }

        /// <summary>
        /// Возвращает список условий (термов) для ячейки (rowIndex, columnIndex).
        /// </summary>
        public IReadOnlyList<TransitionCondition> GetConditions(int rowIndex, int columnIndex)
        {
            var formula = GetTransitionFormulaByRowIndex(rowIndex);
            if (formula == null)
                return Array.Empty<TransitionCondition>();
            if (columnIndex < 0 || columnIndex >= Headers.Count)
                return Array.Empty<TransitionCondition>();

            var toId = Headers[columnIndex] ?? string.Empty;
            return formula.Conditions
                .Where(c => string.Equals(c.YTo.ID ?? string.Empty, toId, StringComparison.Ordinal))
                .ToList();
        }

        /// <summary>
        /// Находит список условий (термов) по идентификаторам вершин-источника и назначения.
        /// </summary>
        public IReadOnlyList<TransitionCondition> FindConditions(string fromVertexId, string toVertexId)
        {
            var formula = GetTransitionFormulaByVertexId(fromVertexId);
            if (formula == null)
                return Array.Empty<TransitionCondition>();
            var toId = toVertexId ?? string.Empty;
            return formula.Conditions
                .Where(c => string.Equals(c.YTo.ID ?? string.Empty, toId, StringComparison.Ordinal))
                .ToList();
        }

        #endregion





        #region Объединение МСА



        #region Проверка схожести с другими МСА

        /// <summary>
        /// Вычисляет коэффициент схожести с другой матрицей.
        /// </summary>
        /// <param name="other">Другая матрица для сравнения.</param>
        /// <returns>Количество совпадающих значимых ячеек.</returns>
        public int CalculateSimilarity(MatrixAlgorithmSchema other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            int similarity = 0;
            var commonRows = Rows.Select(r => r.Header)
                                     .Intersect(other.Rows.Select(r => r.Header))
                                     .ToList();

            var commonColumns = Headers
                                    .Intersect(other.Headers).ToList();

            foreach (var rowHeader in commonRows)
            {
                var thisRow = this[rowHeader];
                var otherRow = other[rowHeader];

                foreach (var colHeader in commonColumns)
                {
                    int thisColIndex = IndexOf(Headers, colHeader);
                    int otherColIndex = IndexOf(other.Headers, colHeader);

                    if (thisColIndex >= 0 && otherColIndex >= 0 &&
                        thisColIndex < thisRow.Transitions.Count &&
                        otherColIndex < otherRow.Transitions.Count)
                    {
                        string thisCell = thisRow.Transitions[thisColIndex];
                        string otherCell = otherRow.Transitions[otherColIndex];

                        if (IsSignificantCell(thisCell) &&
                            IsSignificantCell(otherCell) &&
                            thisCell == otherCell)
                        {
                            similarity++;
                        }
                    }
                }
            }

            return similarity;
        }

        /// <summary>
        /// Проверка значимости ячейки.
        /// </summary>
        /// <param name="value">Содержимое ячейки.</param>
        /// <returns><see langword="true"/>, если искомая ячейка является значимой, иначе <see langword="false"/>.</returns>
        private static bool IsSignificantCell(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            value = value.Trim();
            return value != "0" && value != "1" && value != " " && value != string.Empty;
        }

        private static int IndexOf(IReadOnlyList<string> list, string value)
        {
            if (list == null) return -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(list[i], value, StringComparison.Ordinal))
                    return i;
            }
            return -1;
        }

        #endregion



        #endregion





        #region Рендеринг в текст

        /// <summary>
        /// Преобразует список условий для одной ячейки в строку.
        /// </summary>
        private static string RenderCell(List<TransitionCondition>? conditions, OutputView view)
        {
            if (conditions == null || conditions.Count == 0)
                return string.Empty;

            // Сортировка условий внутри ячейки по целевой вершине уже выполнена на уровне формулы,
            // здесь упорядочим только сами термы для стабильности по их логическому выражению.
            var ordered = conditions
                .OrderBy(c => c.LogicalExpression, StringComparer.Ordinal)
                .ToList();

            if (view == OutputView.LogicalExpression)
            {
                // Пробуем упростить логическое выражение через библиотеку LogicalExpressions
                try
                {
                    var exprs = new List<string>();
                    foreach (var c in conditions)
                    {
                        // Если выражение пустое - это безусловный переход (1)
                        string e = c.LogicalExpression;
                        exprs.Add(string.IsNullOrEmpty(e) ? "1" : $"({e})");
                    }

                    string fullExpr = string.Join(" | ", exprs)
                        .Replace("˄", "&")
                        .Replace("˅", "|")
                        .Replace("¬", "!");

                    var node = ExpressionParser.Parse(fullExpr);
                    var le = new LogicalExpression(node);
                    
                    // Минимизируем (BDD)
                    var simplified = le.Minimize();

                    var result = simplified.ToString()
                        .Replace("&", "˄")
                        .Replace("|", "˅")
                        .Replace("~", "¬");

                    // Удаляем внешние скобки, если они обрамляют всё выражение
                    if (result.StartsWith("(") && result.EndsWith(")"))
                    {
                         result = result.Substring(1, result.Length - 2);
                    }
                    
                    return result;
                }
                catch
                {
                    // Fallback
                    var parts = ordered.Select(c => string.IsNullOrEmpty(c.LogicalExpression) ? "1" : c.LogicalExpression);
                    return string.Join(" ˅ ", parts);
                }
            }
            else
            {
                // В человекочитаемом виде отображаем X-часть без упоминания целевого Y (так требуют ячейки).
                var parts = ordered.Select(c => string.IsNullOrEmpty(c.HumanReadableExpression) ? "1" : c.HumanReadableExpression);
                return string.Join(" ˅ ", parts);
            }
        }

        /// <summary>
        /// Форматирует строку с заголовками столбцов МСА.
        /// </summary>
        private string BuildHeaderLine(List<int> widths)
        {
            var sb = new StringBuilder();
            sb.Append('│');
            sb.Append(new string(' ', widths[0]));
            sb.Append('│');

            for (int i = 1; i < Headers.Count; i++)
            {
                sb.Append(Center(Headers[i], widths[i]));
                sb.Append('│');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Вычисляет оптимальные ширины столбцов для форматирования.
        /// </summary>
        private List<int> CalculateColumnWidths()
        {
            var widths = new List<int>();

            // Первая колонка — заголовки строк (Rows.Header)
            int firstColumnWidth = Rows.Count == 0 ? 2 : Rows.Max(r => r.Header?.Length ?? 0) + 2;
            widths.Add(firstColumnWidth);

            // Остальные столбцы — заголовки без Yн (пропускаем Headers[0])
            for (int i = 1; i < Headers.Count; i++)
            {
                int maxWidth = (Headers[i]?.Length ?? 0) + 2;
                if (Rows.Count > 0)
                {
                    int maxDataWidth = Rows.Max(r => r.Transitions.Count > i ? ProcessConditions(r.Transitions[i]).Length : 0);
                    maxWidth = Math.Max(maxWidth, maxDataWidth + 2);
                }
                widths.Add(maxWidth);
            }

            return widths;
        }

        private static string BuildTopBorder(List<int> widths)
        {
            return "┌" + string.Join("┬", widths.Select(w => new string('─', w))) + "┐";
        }

        private static string BuildMiddleBorder(List<int> widths)
        {
            return "├" + string.Join("┼", widths.Select(w => new string('─', w))) + "┤";
        }

        private static string BuildBottomBorder(List<int> widths)
        {
            return "└" + string.Join("┴", widths.Select(w => new string('─', w))) + "┘";
        }

        private static string BuildDataLine(string rowHeader, List<string> transitions, List<int> widths)
        {
            var sb = new StringBuilder();
            sb.Append('│');
            sb.Append(PadRight(rowHeader, widths[0]));
            sb.Append('│');

            for (int i = 1; i < widths.Count; i++)
            {
                string cell = transitions.Count > i ? transitions[i] : string.Empty;
                sb.Append(Center(ProcessConditions(cell), widths[i]));
                sb.Append('│');
            }

            return sb.ToString();
        }

        private static string ProcessConditions(string s)
        {
            return s ?? string.Empty;
        }

        private static string PadRight(string s, int width)
        {
            if (s == null) s = string.Empty;
            if (s.Length >= width) return s;
            return s + new string(' ', width - s.Length);
        }

        private static string Center(string s, int width)
        {
            if (s == null) s = string.Empty;
            if (s.Length >= width) return s;
            int left = (width - s.Length) / 2;
            int right = width - s.Length - left;
            return new string(' ', left) + s + new string(' ', right);
        }

        #endregion





        #region Представление в DataGridView

        /// <summary>
        /// Создает DataTable для привязки к DataGridView.
        /// </summary>
        private DataTable ToDataTable()
        {
            var dt = new DataTable();

            // Добавляем столбец для заголовков строк
            dt.Columns.Add(" ", typeof(string));

            // Добавляем столбцы для вершин назначения
            foreach (var header in Headers.Skip(1)) // Пропускаем Yн в заголовках
            {
                dt.Columns.Add(header, typeof(string));
            }

            // Заполняем данные
            foreach (var row in Rows)
            {
                var dataRow = dt.NewRow();
                dataRow[0] = row.Header; // Заголовок строки

                for (int i = 1; i < Headers.Count; i++)
                {
                    dataRow[i] = i < row.Transitions.Count
                        ? row.Transitions[i]
                        : string.Empty;
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        #endregion
    }
}