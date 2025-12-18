using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using System.Text;

namespace AlgorithmDeveloper.Abstractions.LAS
{
    /// <summary>
    /// Генератор логических схем алгоритма (ЛСА) из модели AlgoModel.
    /// Формирует валидную ЛСА-строку по графу вершин.
    /// </summary>
    public static class AAToLASConverter
    {
        /// <summary>
        /// Генерирует ЛСА из модели. Не использует исходный текст, только структуру графа.
        /// Внутренне ведёт подробный лог выполнения, который можно отключить.
        /// </summary>
        public static string Convert(AbstractAutomata model, out string verboseLog, bool enableLogging = false)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var ctx = new ConverterContext(model, enableLogging);
            var result = ctx.GenerateCore();
            verboseLog = ctx.GetVerboseLog();
            return result;
        }

        private sealed class ConverterContext
        {
            private readonly AbstractAutomata _model;
            private readonly bool _enableLogging;
            private readonly StringBuilder _sb = new StringBuilder();
            private string? _lastToken = null;
            private readonly HashSet<IBDVertex> _visited = new HashSet<IBDVertex>();
            private readonly HashSet<int> _printedBoundaries = new HashSet<int>();
            private readonly List<int> _pendingBoundaries = new List<int>();
            private readonly Dictionary<int, JumpPoint> _jumpPointByIndex = new Dictionary<int, JumpPoint>();
            private bool _printedEnd = false;
            private readonly StringBuilder? _logSb;

            public ConverterContext(AbstractAutomata model, bool enableLogging)
            {
                _model = model;
                _enableLogging = enableLogging;
                _logSb = enableLogging ? new StringBuilder(1024) : null;
            }

            public string GetVerboseLog() => _enableLogging ? _logSb!.ToString() : string.Empty;
            private string FormatSet(IEnumerable<string> items) => string.Join(", ", items);
            private string FormatVisited() => FormatSet(_visited.Select(v => GetID(v)));
            private string FormatPrinted() => FormatSet(_printedBoundaries.OrderBy(i => i).Select(i => i.ToString()));
            private string FormatPending() => FormatSet(_pendingBoundaries.Select(i => i.ToString()));
            private string FormatJumpPoints() => FormatSet(_jumpPointByIndex.OrderBy(kv => kv.Key).Select(kv =>
                $"{kv.Key}:↓{kv.Key}→{(kv.Value.Next == null ? "null" : GetID(kv.Value.Next))}"));

            private void LogSnapshot(string title, string? result = null, string? extra = null)
            {
                if (!_enableLogging) return;
                _logSb!.AppendLine("┌────────────────────────────────────────────────");
                _logSb.AppendLine($"│ {title}");
                _logSb.AppendLine($"│\tТокены: {_sb}");
                _logSb.AppendLine($"│\tvisited: [{FormatVisited()}]");
                _logSb.AppendLine($"│\tprintedBoundaries: [{FormatPrinted()}]");
                _logSb.AppendLine($"│\tpendingBoundaries: [{FormatPending()}]");
                _logSb.AppendLine($"│\tjumpPointByIndex: [{FormatJumpPoints()}]");
                _logSb.AppendLine($"│\tlastToken: {_lastToken ?? "null"}");
                _logSb.AppendLine($"│\tprintedEnd: {_printedEnd}");
                if (!string.IsNullOrEmpty(result))
                    _logSb.AppendLine($"├─ Результат: {result}");
                if (!string.IsNullOrEmpty(extra))
                    _logSb.AppendLine($"└─ Инфо: {extra}");
                else
                    _logSb.AppendLine("└────────────────────────────────────────────────");
            }

            private void RegisterJumpPoint(JumpPoint jp)
            {
                if (!_jumpPointByIndex.ContainsKey(jp.JumpIndex))
                    _jumpPointByIndex[jp.JumpIndex] = jp;
                if (_enableLogging)
                {
                    var existed = _printedBoundaries.Contains(jp.JumpIndex) || _pendingBoundaries.Contains(jp.JumpIndex);
                    LogSnapshot("Вызов <RegisterJumpPoint>",
                        existed ? $"Индекс {jp.JumpIndex} уже в использовании (printed/pending)" : $"Зарегистрирована точка ↓{jp.JumpIndex}",
                        $"Next: {(jp.Next == null ? "null" : GetID(jp.Next))}");
                }
            }

            private string GetID(IBDVertex v)
            {
                switch (v)
                {
                    case OperatorVertex op:
                        return !string.IsNullOrEmpty(op.ID) ? op.ID! : ($"Y{op.Index}");
                    case ConditionalVertex cv:
                        return !string.IsNullOrEmpty(cv.ID) ? cv.ID! : ($"{cv.Prefix}{cv.Index}");
                    case StartVertex:
                        return "Yн";
                    case EndVertex:
                        return "Yк";
                    case JumpPoint jp:
                        return $"↓{jp.JumpIndex}";
                    default:
                        return v.ID ?? string.Empty;
                }
            }

            private void AppendToken(string token)
            {
                if (_sb.Length > 0) _sb.Append(' ');
                _sb.Append(token);
                var prev = _lastToken;
                _lastToken = token;
                if (_enableLogging)
                {
                    LogSnapshot("Вызов <AppendToken>", $"Добавлен токен: {token}", $"lastToken: {(prev ?? "null")} → {_lastToken}");
                }
            }

            private void AppendJumpPoint(int index)
            {
                AppendToken($"↓{index}");
                _printedBoundaries.Add(index);
                if (_enableLogging)
                {
                    LogSnapshot("Вызов <AppendJumpPoint>", $"Закрыта граница ↓{index}");
                }
            }

            private void AddPending(int index, string reason)
            {
                if (!_pendingBoundaries.Contains(index))
                {
                    _pendingBoundaries.Add(index);
                    if (_enableLogging)
                        LogSnapshot("Изменение <pendingBoundaries>", $"Добавлена ↓{index}", $"Причина: {reason}");
                }
            }

            private void RemovePending(int index, string reason)
            {
                if (_pendingBoundaries.Remove(index) && _enableLogging)
                {
                    LogSnapshot("Изменение <pendingBoundaries>", $"Удалена ↓{index}", $"Причина: {reason}");
                }
            }

            // Печать ветви (субалгоритма)
            private int EmitBranch(IBDVertex? start, HashSet<IBDVertex> branchVisited)
            {
                int lastBoundaryIndex = -1;
                var current = start;
                if (_enableLogging)
                {
                    LogSnapshot("Вход в функцию <EmitBranch>", null, $"Старт: {(current == null ? "null" : GetID(current))}; branchVisited: {branchVisited.Count}");
                }
                while (current != null)
                {
                    // Если ветка начинается непосредственно с точки перехода, её необходимо
                    // завершить печатью безусловного перехода на эту точку, даже если точка
                    // уже была посещена в рамках текущей ветви (возвратная правая ветвь).
                    // Это соответствует спецификации: внутри ветви допустимы точки и
                    // безусловные переходы на них; встреча точки в начале ветви печатает w↑i.
                    if (current is JumpPoint jpImmediate)
                    {
                        RegisterJumpPoint(jpImmediate);
                        AppendToken($"w↑{jpImmediate.JumpIndex}");
                        AddPending(jpImmediate.JumpIndex, "встречена точка в ветви (начало/немедленно)");
                        lastBoundaryIndex = jpImmediate.JumpIndex;
                        if (_enableLogging) LogSnapshot("Выход из <EmitBranch>", $"Ветка завершена на ↓{lastBoundaryIndex}");
                        return lastBoundaryIndex;
                    }

                    if (!branchVisited.Add(current))
                    {
                        if (_enableLogging) LogSnapshot("Выход из <EmitBranch>", $"Повторное посещение, возврат lastBoundaryIndex={lastBoundaryIndex}");
                        return lastBoundaryIndex;
                    }

                    // JumpPoint уже обработан выше; остальные фигуры учитываем в глобальном visited
                    _visited.Add(current);

                    switch (current)
                    {
                        case OperatorVertex op:
                            AppendToken(GetID(op));
                            if (op.Next is JumpPoint jpn)
                            {
                                RegisterJumpPoint(jpn);
                                AppendToken($"w↑{jpn.JumpIndex}");
                                AddPending(jpn.JumpIndex, "безусловный переход из ветви");
                                lastBoundaryIndex = jpn.JumpIndex;
                                if (_enableLogging) LogSnapshot("Выход из <EmitBranch>", $"Ветка завершена на ↓{lastBoundaryIndex}");
                                return lastBoundaryIndex;
                            }
                            current = op.Next;
                            break;

                        case ConditionalVertex cv:
                            var lbsJp = cv.LBS as JumpPoint;
                            AppendToken(GetID(cv));
                            if (lbsJp != null)
                                AppendToken($"↑{lbsJp.JumpIndex}");

                            int before = _pendingBoundaries.Count;
                            var rb = EmitBranch(cv.RBS, branchVisited);
                            var newBounds = _pendingBoundaries.Skip(before).ToList();
                            lastBoundaryIndex = rb >= 0 ? rb : (newBounds.Count > 0 ? newBounds.Last() : -1);

                            if (lbsJp != null)
                            {
                                RegisterJumpPoint(lbsJp);
                                if (lbsJp.Next is EndVertex)
                                {
                                    if (_enableLogging) LogSnapshot("Условная ветвь", $"Левая ↓{lbsJp.JumpIndex} ведёт к Yк, закрытие отложено", $"newBounds: [{FormatSet(newBounds.Select(n => n.ToString()))}]");
                                    return lastBoundaryIndex;
                                }
                                if (_printedBoundaries.Contains(lbsJp.JumpIndex))
                                {
                                    if (_enableLogging) LogSnapshot("Условная ветвь", $"Левая ↓{lbsJp.JumpIndex} уже закрыта, возврат", $"lastBoundaryIndex={lastBoundaryIndex}");
                                    return lastBoundaryIndex;
                                }
                                if (!_printedBoundaries.Contains(lbsJp.JumpIndex))
                                    AppendJumpPoint(lbsJp.JumpIndex);
                                if (newBounds.Contains(lbsJp.JumpIndex))
                                    RemovePending(lbsJp.JumpIndex, "закрыта внутри ветви условной вершины");
                                current = lbsJp.Next ?? cv.LBS;
                            }
                            else
                            {
                                foreach (var b in newBounds)
                                {
                                    if (!_printedBoundaries.Contains(b))
                                        AppendJumpPoint(b);
                                }
                                foreach (var b in newBounds)
                                    RemovePending(b, "закрыта внутри ветви (без явной левой точки)");
                                current = cv.LBS;
                            }
                            break;

                        case EndVertex:
                            if (_enableLogging) LogSnapshot("Выход из <EmitBranch>", $"Достигнут Yк в ветви, возврат lastBoundaryIndex={lastBoundaryIndex}");
                            return lastBoundaryIndex;

                        case JumpPoint jp:
                            // До этого места мы не должны доходить, так как JumpPoint
                            // обрабатывается немедленно в начале итерации. Оставлено
                            // как защита от непредвидённых случаев; поведение идентично.
                            RegisterJumpPoint(jp);
                            AppendToken($"w↑{jp.JumpIndex}");
                            AddPending(jp.JumpIndex, "встречена точка в ветви");
                            lastBoundaryIndex = jp.JumpIndex;
                            if (_enableLogging) LogSnapshot("Выход из <EmitBranch>", $"Ветка завершена на ↓{lastBoundaryIndex}");
                            return lastBoundaryIndex;

                        case StartVertex:
                            current = current.Next;
                            break;

                        default:
                            current = current.Next;
                            break;
                    }
                }

                if (_enableLogging) LogSnapshot("Выход из <EmitBranch>", $"Возврат lastBoundaryIndex={lastBoundaryIndex}");
                return lastBoundaryIndex;
            }

            public string GenerateCore()
            {
                AppendToken("Yн");
                if (_enableLogging) LogSnapshot("Вход в функцию <Convert>");

                if (_model.Start?.Next is JumpPoint startJump)
                {
                    RegisterJumpPoint(startJump);
                    AppendToken($"w↑{startJump.JumpIndex}");
                }

                IBDVertex? currentMain = _model.Start?.Next;

                for (;;)
                {
                    while (currentMain != null)
                    {
                        if (!(currentMain is JumpPoint) && !_visited.Add(currentMain))
                            break;

                        switch (currentMain)
                        {
                            case OperatorVertex op:
                                AppendToken(GetID(op));
                                if (op.Next is JumpPoint jpn)
                                {
                                    RegisterJumpPoint(jpn);
                                    AppendToken($"w↑{jpn.JumpIndex}");
                                    // Регистрируем отложенную границу для случая,
                                    // когда обработка JumpPoint выберет альтернативную
                                    // границу и не напечатает текущую ↓j немедленно.
                                    AddPending(jpn.JumpIndex, "безусловный переход на верхнем уровне");
                                    currentMain = jpn;
                                    break;
                                }
                                currentMain = op.Next;
                                break;

                            case ConditionalVertex cv:
                                var leftJpMain = cv.LBS as JumpPoint;
                                AppendToken(GetID(cv));
                                if (leftJpMain != null)
                                    AppendToken($"↑{leftJpMain.JumpIndex}");
                                var beforePending = _pendingBoundaries.Count;
                                var branchBoundaryIndex = EmitBranch(cv.RBS, new HashSet<IBDVertex>());
                                if (_printedEnd) { currentMain = null; break; }

                                // Если правая ветвь завершилась безусловным переходом на точку,
                                // закрываем её и продолжаем с продолжения этой точки,
                                // если индекс правой границы меньше индекса левой, либо левая — 0 (особый случай).
                                if (branchBoundaryIndex >= 0 &&
                                    !_printedBoundaries.Contains(branchBoundaryIndex) &&
                                    _jumpPointByIndex.TryGetValue(branchBoundaryIndex, out var branchJp) &&
                                    !(branchJp.Next is EndVertex) &&
                                    (leftJpMain == null || branchBoundaryIndex < leftJpMain.JumpIndex || (leftJpMain != null && leftJpMain.JumpIndex == 0)))
                                {
                                    AppendJumpPoint(branchBoundaryIndex);
                                    RemovePending(branchBoundaryIndex, "закрыта после правой ветви (приоритет ранней границы)");
                                    currentMain = branchJp.Next;
                                    if (currentMain != null && !(currentMain is JumpPoint) && _visited.Contains(currentMain))
                                    {
                                        currentMain = null;
                                    }
                                    break;
                                }

                                var leftBoundaryAlreadyPrinted = leftJpMain != null && _printedBoundaries.Contains(leftJpMain.JumpIndex);
                                bool shouldDeferLeftBoundary = false;
                                if (leftJpMain != null && leftJpMain.Next is EndVertex)
                                {
                                    shouldDeferLeftBoundary = _pendingBoundaries.Any(b => b < leftJpMain.JumpIndex &&
                                        _jumpPointByIndex.TryGetValue(b, out var jp) && !(jp.Next is EndVertex));
                                }

                                // Возвращаем стандартную семантику: левую границу можно закрывать
                                // даже если ранее не было w↑i, согласно допускаемым правилам парсера.
                                if (leftJpMain != null)
                                {
                                    RegisterJumpPoint(leftJpMain);
                                    if (shouldDeferLeftBoundary)
                                        AddPending(leftJpMain.JumpIndex, "отложена левая граница основного условия");
                                    if (!shouldDeferLeftBoundary && !leftBoundaryAlreadyPrinted)
                                    {
                                        AppendJumpPoint(leftJpMain.JumpIndex);
                                        if (_pendingBoundaries.Contains(leftJpMain.JumpIndex))
                                            RemovePending(leftJpMain.JumpIndex, "закрыта совпадающая граница после правой ветви");
                                    }
                                }
                                else if (cv.LBS is JumpPoint lbsMain) { RegisterJumpPoint(lbsMain); }

                                if (cv.LBS is JumpPoint lbsMain2)
                                {
                                    var startCont = (_model.Start?.Next is JumpPoint sjp) ? sjp.Next : _model.Start?.Next;
                                    if (startCont != null && lbsMain2.Next != null && startCont.Equals(lbsMain2.Next))
                                    {
                                        currentMain = null;
                                        break;
                                    }
                                }
                                if (leftBoundaryAlreadyPrinted)
                                {
                                    if (branchBoundaryIndex >= 0)
                                    {
                                        if (!_printedBoundaries.Contains(branchBoundaryIndex))
                                            AppendJumpPoint(branchBoundaryIndex);
                                        RemovePending(branchBoundaryIndex, "закрыта после правой ветви (левая уже закрыта)");
                                        if (_jumpPointByIndex.TryGetValue(branchBoundaryIndex, out var jpCont))
                                        {
                                            if (jpCont.Next is EndVertex)
                                            {
                                                currentMain = null;
                                            }
                                            else
                                            {
                                                currentMain = jpCont.Next;
                                                if (currentMain != null && !(currentMain is JumpPoint) && _visited.Contains(currentMain))
                                                {
                                                    currentMain = null;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            currentMain = (cv.LBS as JumpPoint)?.Next ?? cv.LBS;
                                            if (currentMain != null && !(currentMain is JumpPoint) && _visited.Contains(currentMain))
                                            {
                                                currentMain = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        currentMain = null;
                                    }
                                }
                                else
                                {
                                    currentMain = (cv.LBS as JumpPoint)?.Next ?? cv.LBS;
                                    if (currentMain != null && !(currentMain is JumpPoint) && _visited.Contains(currentMain))
                                    {
                                        currentMain = null;
                                    }
                                }
                                break;

                            case EndVertex:
                                bool hasNextBoundary = _pendingBoundaries.Any(b => _jumpPointByIndex.TryGetValue(b, out var jp2) && !(jp2.Next is EndVertex));
                                if (hasNextBoundary)
                                {
                                    var nextBoundary = _pendingBoundaries.First(b => _jumpPointByIndex.TryGetValue(b, out var jp2) && !(jp2.Next is EndVertex));
                                    if (_jumpPointByIndex.TryGetValue(nextBoundary, out var jpToCont))
                                    {
                                        if (!_printedBoundaries.Contains(nextBoundary))
                                            AppendJumpPoint(nextBoundary);
                                        _pendingBoundaries.Remove(nextBoundary);
                                        currentMain = jpToCont.Next;
                                    }
                                    else
                                    {
                                        foreach (var b in _pendingBoundaries)
                                        {
                                            if (!_printedBoundaries.Contains(b))
                                                AppendJumpPoint(b);
                                        }
                                        AppendToken("Yк");
                                        _printedEnd = true;
                                        if (_enableLogging) LogSnapshot("Завершение", "Напечатан Yк и все отложенные границы", $"Всего границ: {_pendingBoundaries.Count}");
                                        currentMain = null;
                                    }
                                }
                                else
                                {
                                    foreach (var b in _pendingBoundaries)
                                    {
                                        if (!_printedBoundaries.Contains(b))
                                            AppendJumpPoint(b);
                                    }
                                    AppendToken("Yк");
                                    _printedEnd = true;
                                    if (_enableLogging) LogSnapshot("Завершение", "Напечатан Yк и все отложенные границы", $"Всего границ: {_pendingBoundaries.Count}");
                                    currentMain = null;
                                }
                                break;

                            case JumpPoint jp:
                                RegisterJumpPoint(jp);
                                if (jp.Next is EndVertex)
                                {
                                    bool hasAltBoundary = _pendingBoundaries.Any(b => _jumpPointByIndex.TryGetValue(b, out var jpa) && !(jpa.Next is EndVertex));
                                    if (hasAltBoundary)
                                    {
                                        var altBoundary = _pendingBoundaries.First(b => _jumpPointByIndex.TryGetValue(b, out var jpa) && !(jpa.Next is EndVertex));
                                        if (_jumpPointByIndex.TryGetValue(altBoundary, out var altJp))
                                        {
                                            if (!_printedBoundaries.Contains(altBoundary))
                                                AppendJumpPoint(altBoundary);
                                            RemovePending(altBoundary, "закрыта более ранняя граница при точке, ведущей к завершению");
                                            currentMain = altJp.Next;
                                            if (currentMain != null && !(currentMain is JumpPoint) && _visited.Contains(currentMain))
                                            {
                                                currentMain = null;
                                            }
                                            break;
                                        }
                                    }
                                }

                                if (!_printedBoundaries.Contains(jp.JumpIndex))
                                {
                                    AppendJumpPoint(jp.JumpIndex);
                                }
                                RemovePending(jp.JumpIndex, "закрытие при обработке точки в основном проходе");
                                currentMain = jp.Next;
                                if (currentMain is not JumpPoint && _visited.Contains(currentMain!))
                                {
                                    currentMain = null;
                                }
                                break;

                            case StartVertex:
                                currentMain = currentMain.Next;
                                break;

                            default:
                                currentMain = currentMain.Next;
                                break;
                        }
                    }

                    if (!_printedEnd)
                    {
                        bool hasNextBoundary = _pendingBoundaries.Any(b => _jumpPointByIndex.TryGetValue(b, out var jp2) && !(jp2.Next is EndVertex));
                        if (hasNextBoundary)
                        {
                            var nextBoundary = _pendingBoundaries.First(b => _jumpPointByIndex.TryGetValue(b, out var jp2) && !(jp2.Next is EndVertex));
                            if (!_printedBoundaries.Contains(nextBoundary))
                                AppendJumpPoint(nextBoundary);
                            RemovePending(nextBoundary, "продолжение после завершения основного прохода");
                            if (_jumpPointByIndex.TryGetValue(nextBoundary, out var jpToCont))
                            {
                                currentMain = jpToCont.Next;
                                continue; // повторить основной проход без goto
                            }
                        }
                        foreach (var b in _pendingBoundaries)
                        {
                            if (!_printedBoundaries.Contains(b))
                                AppendJumpPoint(b);
                        }
                        AppendToken("Yк");
                        _printedEnd = true;
                    }
                    break; // выйти из внешнего for(;;)
                }

                return _sb.ToString();
            }
        }
    }
}