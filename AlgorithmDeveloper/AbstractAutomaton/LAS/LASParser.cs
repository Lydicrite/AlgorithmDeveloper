using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AlgorithmDeveloper.AAModel.Utils;
using AlgorithmDeveloper.AAModel;
using AlgorithmDeveloper.AAModel.LAS;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;

namespace AlgorithmDeveloper.AAModel.LAS
{
    /// <summary>
    /// Парсер логических схем алгоритмов (ЛСА) в объекты <see cref="AbstractAutomaton"/>.
    /// Выполнен как статический и потокобезопасен: весь изменяемый контекст локален вызову.
    /// </summary>
    internal static class LASParser
    {
        private sealed class ParserContext
        {
            public int ConditionalVertexCounter = 0;
            public readonly HashSet<string> OperatorVertexIds = new HashSet<string>();
            public readonly HashSet<string> ConditionalVertexIds = new HashSet<string>();
            public readonly HashSet<string> JumpPointIds = new HashSet<string>();
        }

        /// <summary>
        /// Преобразует входную строку <paramref name="input"/> в объект <see cref="AbstractAutomaton"/>.
        /// </summary>
        /// <param name="input">Входная строка с ЛСА.</param>
        /// <returns>Объект <see cref="AbstractAutomaton"/>, заполненный по указанному во входной строке алгоритму</returns>
        /// <exception cref="ParsingAggregateException">Возникает при обнаружении ошибок парсинга.</exception>
        public static AbstractAutomaton Parse(string input)
        {
            var ctx = new ParserContext();
            var errors = new List<ParsingError>();
            var parsedPositions = new Dictionary<int, bool>();

            input = PreprocessInput(input);
            var tokens = Tokenize(input, ref errors);
            for (int i = 0; i < tokens.Count; i++)
                parsedPositions.Add(i, false);

            var model = new AbstractAutomaton();

            try
            {
                if (tokens.FirstOrDefault() != "Yн")
                    AddError(errors, "ЛСА должна начинаться с 'Yн'", 0);

                if (!tokens.Contains("Yк"))
                    AddError(errors, "ЛСА должна содержать и заканчиваться на 'Yк'", 0);

                ValidateJumpOperatorsAndPointsExistence(tokens, ref errors);
                ValidateNoConsecutiveJumpPoints(tokens, ref errors);
                ValidateNoConsecutiveJumpOperators(tokens, ref errors);
                ValidateNoMeaninglessConditionalRightBranch(tokens, ref errors);

                int position = 1;
                var start = model.AddVertex(new StartVertex());
                IBDVertex? previousElement = start;

                ParseElements(ctx, model, tokens, ref position, ref parsedPositions, ref previousElement, ref errors);

                ValidateStartEndUniqueness(model, ref errors);
                ValidateConditionalVertices(model, ref errors);
            }
            catch (ParsingAggregateException) { }

            if (errors.Count > 0)
                throw new ParsingAggregateException(errors);

            model.InitialLAS = input;
            model.Update();
            return model;
        }

        /// <summary>
        /// Проверяет возможность преобразования входной строки <paramref name="input"/> в объект <see cref="AbstractAutomaton"/>
        /// с созданием объекта в случае успеха.
        /// </summary>
        /// <param name="input">Входная строка с ЛСА.</param>
        /// <param name="errors">Список ошибок парсинга, если они есть.</param>
        /// <returns><see langword="true"/>, если парсинг прошел без ошибок; иначе <see langword="false"/>.</returns>
        public static bool TryParse(string input, out AbstractAutomaton? model, out List<ParsingError> errors)
        {
            model = null;
            errors = new List<ParsingError>();
            try
            {
                model = Parse(input);
                var ok = model.AreAllVerticesReachable(out var unreachableIds);
                if (!ok)
                {
                    var list = new List<ParsingError>();
                    AddError(list, RussianGrammar.FormatUnreachableVerticesMessage(unreachableIds), 0);
                    errors = list;
                    return false;
                }
                return true;
            }
            catch (ParsingAggregateException ex)
            {
                errors = ex.Errors;
                return false;
            }
        }

        /// <summary>
        /// Проверяет возможность преобразования входной строки <paramref name="input"/> в объект <see cref="AbstractAutomaton"/>
        /// с созданием объекта в случае успеха.
        /// </summary>
        /// <param name="input">Входная строка с ЛСА.</param>
        /// <param name="exception">Исключение с ошибками парсинга, если они есть.</param>
        /// <returns><see langword="true"/>, если парсинг прошел без ошибок; иначе <see langword="false"/>.</returns>
        public static bool TryParse(string input, out AbstractAutomaton? model, out ParsingAggregateException? exception)
        {
            model = null;
            exception = null;
            try
            {
                model = Parse(input);
                var ok = model.AreAllVerticesReachable(out var unreachableIds);
                if (!ok)
                {
                    var list = new List<ParsingError>();
                    AddError(list, RussianGrammar.FormatUnreachableVerticesMessage(unreachableIds), 0);
                    exception = new ParsingAggregateException(list);
                    return false;
                }
                return true;
            }
            catch (ParsingAggregateException ex)
            {
                exception = ex;
                return false;
            }
        }

        /// <summary>
        /// Проверяет возможность преобразования входной строки <paramref name="input"/> в объект <see cref="AbstractAutomaton"/>
        /// без создания объекта.
        /// </summary>
        /// <param name="input">Входная строка с ЛСА.</param>
        /// <param name="exception">Исключение с ошибками парсинга, если они есть.</param>
        /// <returns><see langword="true"/>, если парсинг прошел без ошибок; иначе <see langword="false"/>.</returns>
        public static bool TryParse(string input, out ParsingAggregateException? exception)
        {
            exception = null;
            try
            {
                var model = Parse(input);
                var ok = model.AreAllVerticesReachable(out var unreachableIds);
                if (!ok)
                {
                    var list = new List<ParsingError>();
                    AddError(list, RussianGrammar.FormatUnreachableVerticesMessage(unreachableIds), 0);
                    exception = new ParsingAggregateException(list);
                    return false;
                }
                return true;
            }
            catch (ParsingAggregateException ex)
            {
                exception = ex;
                return false;
            }
        }

        /// <summary>
        /// Проверяет возможность преобразования входной строки <paramref name="input"/> в объект <see cref="AbstractAutomaton"/>
        /// без создания самого объекта.
        /// </summary>
        /// <param name="input">Входная строка с ЛСА.</param>
        /// <param name="errors">Список ошибок парсинга, если они есть.</param>
        /// <returns><see langword="true"/>, если парсинг прошел без ошибок; иначе <see langword="false"/>.</returns>
        public static bool TryParse(string input, out List<ParsingError> errors)
        {
            var ctx = new ParserContext();
            errors = new List<ParsingError>();
            var parsedPositions = new Dictionary<int, bool>();
            AbstractAutomaton? tempModel = null;

            try
            {
                input = PreprocessInput(input);
                var tokens = Tokenize(input, ref errors);
                for (int i = 0; i < tokens.Count; i++)
                    parsedPositions.Add(i, false);

                // Валидация: перед каждой точкой перехода ↓i должен стоять безусловный переход w↑i
                // ValidateJumpPointPrecedence(tokens, ref errors);

                // Создаем временную модель для валидации структуры
                tempModel = new AbstractAutomaton();

                if (tokens.FirstOrDefault() != "Yн")
                    AddError(errors, "ЛСА должна начинаться с 'Yн'", 0);

                if (!tokens.Contains("Yк"))
                    AddError(errors, "ЛСА должна содержать и заканчиваться на 'Yк'", 0);

                ValidateJumpOperatorsAndPointsExistence(tokens, ref errors);
                ValidateNoConsecutiveJumpPoints(tokens, ref errors);
                ValidateNoConsecutiveJumpOperators(tokens, ref errors);
                ValidateNoMeaninglessConditionalRightBranch(tokens, ref errors);

                int position = 1;
                var start = tempModel.AddVertex(new StartVertex());
                IBDVertex? previousElement = start;

                ParseElements(ctx, tempModel, tokens, ref position, ref parsedPositions, ref previousElement, ref errors);

                ValidateStartEndUniqueness(tempModel, ref errors);
                ValidateConditionalVertices(tempModel, ref errors);
            }
            catch (ParsingAggregateException ex)
            {
                errors.AddRange(ex.Errors);
            }
            catch (Exception ex)
            {
                // На случай других непредвиденных ошибок
                AddError(errors, $"Непредвиденная ошибка при парсинге: {ex.Message}", 0);
            }

            // Дополнительная проверка достижимости всех вершин,
            // если структура и базовые проверки прошли без ошибок
            if (errors.Count == 0 && tempModel != null)
            {
                var ok = tempModel.AreAllVerticesReachable(out var unreachableIds);
                if (!ok)
                    AddError(errors, RussianGrammar.FormatUnreachableVerticesMessage(unreachableIds), 0);
            }

            return errors.Count == 0;
        }





        #region Предподготовка

        /// <summary>
        /// Выполняет подготовку строки к парсингу, нормализуя регистр и удаляя те символы, что не отвечают за создание элементов алгоритма.
        /// </summary>
        /// <param name="input">Строка, содержащая ЛСА.</param>
        /// <returns>Строка, содержащая ЛСА, отредактированная нужным образом.</returns>
        private static string PreprocessInput(string input)
        {
            // Удаляем скобки и сепараторы
            input = Regex.Replace(input, @"[()|]", " ");

            // Нормализуем регистр
            input = Regex.Replace(input, @"\b([yxp])(\d+)\b", m =>
                m.Groups[1].Value.ToUpper() + m.Groups[2].Value);

            input = Regex.Replace(input, @"\b(w↑\d+)\b", m => m.Value.ToLower());

            return Regex.Replace(input, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Выполняет токенизацию для строки с ЛСА.
        /// </summary>
        /// <param name="input">Строка, содержащая ЛСА, отредактированная нужным образом.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Список токенов ЛСА, используемых для создания элементов алгоритма.</returns>
        private static List<string> Tokenize(string input, ref List<ParsingError> errors)
        {
            var tokenPattern = @"(Yн|Yк|w↑\d+|↑\d+|↓\d+|X\d+|P\d+|Y\d+)";
            var matches = Regex.Matches(input, tokenPattern, RegexOptions.IgnoreCase);

            if (matches.Count == 0)
                AddError(errors, "Некорректный формат ЛСА", 0);

            return matches.Cast<Match>()
                .Select(m => m.Value)
                .ToList();
        }

        #endregion





        #region Методы парсинга элементов

        /// <summary>
        /// Создаёт и связывает между собой все элементы ЛСА в соответствии с устройством токенов, представляющих их.
        /// <br></br> Созданные элементы добавляются в объект <see cref="AbstractAutomaton"/>.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/>, в который добавляются созданные элементы.</param>
        /// <param name="tokens">Список токенов ЛСА.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="previousElement">Ссылка на предыдущий элемент ЛСА (возможно значение <see langword="null"/>).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void ParseElements
        (
            ParserContext ctx, AbstractAutomaton model, List<string> tokens, ref int position, ref Dictionary<int, bool> parsedPositions,
            ref IBDVertex? previousElement, ref List<ParsingError> errors
        )
        {
            while (position < tokens.Count)
            {
                // Если предыдущий элемент — конечная вершина, дальнейшее связывание недопустимо
                if (previousElement is EndVertex)
                    break;

                var token = tokens[position];
                if (token == "Yк")
                {
                    var end = HandleEndVertex(model, position, ref parsedPositions, ref errors);
                    LinkPrevious(previousElement, end, ref errors);
                    position++;
                    break;
                }

                // Обработка безусловного перехода на верхнем уровне: w↑i
                if (token.StartsWith("w↑"))
                {
                    int jIndex = int.Parse(token[2..]);
                    var jp = model.EnsureJumpPoint(jIndex);
                    LinkPrevious(previousElement, jp, ref errors);
                    parsedPositions[position] = true;
                    position++;
                    // Сбрасываем previousElement, чтобы не создать самосвязь при последующем ↓i
                    previousElement = null;
                    continue;
                }

                var element = ParseElement(ctx, model, tokens, ref position, ref parsedPositions, ref errors);
                if (element != null)
                {
                    LinkPrevious(previousElement, element, ref errors);
                    previousElement = element;

                    while (position < tokens.Count)
                    {
                        // Обработка безусловного перехода внутри последовательности: w↑i
                        var nextTokenCheck = tokens[position];

                        // Если перед ↓i стоит w↑i (субалгоритм завершён безусловным переходом), не связываем предыдущий элемент с точкой
                        if (nextTokenCheck.StartsWith('↓') && position > 0 && tokens[position - 1].StartsWith("w↑"))
                        {
                            previousElement = null;
                        }

                        if (nextTokenCheck.StartsWith("w↑"))
                        {
                            int jIndex = int.Parse(nextTokenCheck[2..]);
                            var jpInner = model.EnsureJumpPoint(jIndex);
                            LinkPrevious(element, jpInner, ref errors);
                            parsedPositions[position] = true;
                            position++;
                            // Завершаем текущую последовательность после безусловного перехода.
                            // Сбрасываем previousElement, чтобы не создавать самосвязь при последующем ↓i
                            previousElement = null;
                            break;
                        }

                        var nextElement = ParseElement(ctx, model, tokens, ref position, ref parsedPositions, ref errors);
                        if (nextElement != null)
                        {
                            LinkPrevious(element, nextElement, ref errors);
                            previousElement = nextElement;
                            element = nextElement;

                            // Если достигли конечной вершины, прекращаем последовательность
                            if (nextElement is EndVertex)
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Создаёт и возвращает элемент ЛСА на основе токена, стоящего на позиции <paramref name="position"/> в списке токенов.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/>, в который добавляются созданные элементы.</param>
        /// <param name="tokens">Список токенов ЛСА.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Новый элемент ЛСА (возможно значение <see langword="null"/>).</returns>
        private static IBDVertex? ParseElement(ParserContext ctx, AbstractAutomaton model, List<string> tokens, ref int position, ref Dictionary<int, bool> parsedPositions, ref List<ParsingError> errors)
        {
            if (position >= tokens.Count) return null;

            var token = tokens[position++];
            switch (token)
            {
                case "Yн":
                    AddError(errors, "'Yн' может быть только в начале ЛСА", position);
                    return null;

                case "Yк":
                    return HandleEndVertex(model, position - 1, ref parsedPositions, ref errors);

                case var x when x.StartsWith('X') || x.StartsWith('P'):
                    string prefix = x.Substring(0, 1);
                    int originalNumber = int.Parse(x.Substring(1));
                    return ParseConditionalVertex(ctx, model, tokens, ref position, ref parsedPositions, prefix, originalNumber, ref errors);

                case var y when y.StartsWith('Y'):
                    if (y == "Yн")
                    {
                        AddError(errors, "'Yн' может быть только в начале ЛСА", position - 1);
                        return null;
                    }

                    var opVertex = CreateOperatorVertex(ctx, model, position - 1, ref parsedPositions, int.Parse(y[1..]), ref errors);
                    return opVertex;

                case var jp when jp.StartsWith('↓'):
                    return CreateJumpPoint(ctx, model, position - 1, ref parsedPositions, int.Parse(jp[1..]), ref errors);

                case var jo when jo.StartsWith('↑') || jo.StartsWith("w↑"):
                    return null;

                default:
                    AddError(errors, $"Неизвестный токен: \'{token}\'", position);
                    return null;
            }
        }

        #endregion





        #region Парсинг условной вершины

        /// <summary>
        /// Создаёт и возвращает условную вершину на основе токена, стоящего на позиции <paramref name="position"/> в списке токенов.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/>, в который добавляются созданные элементы.</param>
        /// <param name="tokens">Список токенов ЛСА.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="prefix">Префикс условной вершины (X или P).</param>
        /// <param name="originalNumber">Оригинальный номер условной вершины.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Новая условная вершина с указанными параметрами.</returns>
        private static ConditionalVertex ParseConditionalVertex
        (
            ParserContext ctx, AbstractAutomaton model, List<string> tokens, ref int position, ref Dictionary<int, bool> parsedPositions,
            string prefix, int originalNumber, ref List<ParsingError> errors
        )
        {
            int index = ++ctx.ConditionalVertexCounter;
            var vertexId = $"{prefix}{originalNumber}";

            // Проверка уникальности ID условной вершины
            // Разрешаем дубликаты: if (ctx.ConditionalVertexIds.Contains(vertexId)) ...
            ctx.ConditionalVertexIds.Add(vertexId);

            var vertex = model.AddVertex(new ConditionalVertex(prefix, originalNumber));

            // Парсим LBS (условный оператор ↑j)
            var lbs = ParseJumpOperatorForCondition(model, tokens, ref position, ref parsedPositions, ref errors);
            if (lbs == null)
            {
                AddError(errors, $"Ожидается условный оператор перехода '↑j' для \"{prefix}{originalNumber}\"", position);
                return vertex;
            }

            // Парсим RBS (субалгоритм)
            int initialPosition = position;
            var rbs = ParseSubAlgorithm(ctx, model, tokens, ref position, ref parsedPositions, ref errors);

            if (rbs == null)
                AddError(errors, $"Ожидается субалгоритм для \'{prefix}{originalNumber}\'", initialPosition);

            // Устанавливаем ветви
            model.SetConditionalBranches(vertex, lbs, rbs);

            return vertex;
        }

        /// <summary>
        /// Парсит оператор условного перехода (↑j) для Xi.LBS.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/>, в который добавляются созданные элементы.</param>
        /// <param name="tokens">Список токенов ЛСА.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Оператор условного перехода, являющийся левым потомком Xi.</returns>
        private static IBDVertex? ParseJumpOperatorForCondition
        (
            AbstractAutomaton model, List<string> tokens, ref int position, ref Dictionary<int, bool> parsedPositions,
            ref List<ParsingError> errors
        )
        {
            if (position >= tokens.Count) return null;

            var token = tokens[position];
            if (!token.StartsWith('↑'))
            {
                AddError(errors, $"Ожидается условный оператор перехода, получен: {token}", position);
                return null;
            }

            int index = int.Parse(token[1..]);
            var jumpPoint = model.EnsureJumpPoint(index);
            parsedPositions[position] = true;
            position++;
            return jumpPoint;
        }

        /// <summary>
        /// Работает со вложенным алгоритмом, являющимся ветвью какой-либо условной вершины.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/>, в который добавляются созданные элементы.</param>
        /// <param name="tokens">Список токенов ЛСА.</param>
        /// <param name="position">Ссылка на текущую позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Новый элемент ЛСА (возможно значение <see langword="null"/>), являющийся потомком или началом одной из двух ветвей условной вершины.</returns>
        private static IBDVertex? ParseSubAlgorithm(ParserContext ctx, AbstractAutomaton model, List<string> tokens, ref int position, ref Dictionary<int, bool> parsedPositions, ref List<ParsingError> errors)
        {
            int startPos = position;
            IBDVertex? firstElement = null;
            IBDVertex? currentElement = null;
            bool exitFlag = false;

            while (position < tokens.Count && !exitFlag)
            {
                var currentToken = tokens[position];

                // Явные выходы: внешний Yк — завершаем ветвь, ↓ — граница субалгоритма без связывания
                if (currentToken == "Yк" && currentElement != null)
                {
                    var nextElement = ParseElement(ctx, model, tokens, ref position, ref parsedPositions, ref errors);
                    if (currentElement != null && nextElement != null)
                        model.LinkNext(currentElement, nextElement);
                    break;
                }
                if (currentToken.StartsWith('↓'))
                {
                    // Не связываем напрямую с точкой перехода, субалгоритм завершается до её обработки
                    break;
                }

                // Обработка безусловного перехода: w↑j (обрабатываем до парсинга элемента)
                if (currentToken.StartsWith("w↑"))
                {
                    int jIndex = int.Parse(currentToken[2..]);
                    var jp = model.EnsureJumpPoint(jIndex);
                    if (firstElement == null) firstElement = jp;
                    if (currentElement != null)
                        model.LinkNext(currentElement, jp);
                    parsedPositions[position] = true;
                    position++;
                    exitFlag = true;
                    continue;
                }

                if (parsedPositions.TryGetValue(position, out var isParsed) && isParsed)
                    break;

                var element = ParseElement(ctx, model, tokens, ref position, ref parsedPositions, ref errors);
                if (element == null) break;
                parsedPositions[position - 1] = true;

                // Обработка Yк как допустимого завершения субалгоритма
                if (element is EndVertex)
                {
                    if (firstElement == null) firstElement = element;
                    if (currentElement != null)
                        model.LinkNext(currentElement, element);
                    exitFlag = true;
                    continue;
                }

                // Стандартное связывание элементов
                if (firstElement == null)
                {
                    firstElement = element;
                    currentElement = element;
                }
                else if (currentElement != null)
                {
                    model.LinkNext(currentElement, element);
                    currentElement = element;
                }
            }

            // Обработка случая, когда Yк - единственный элемент
            if (firstElement is EndVertex)
                return firstElement;

            if (firstElement == null)
            {
                AddError(errors, "Субалгоритм не может быть пустым", startPos);
                return null;
            }

            return firstElement;
        }

        #endregion





        #region Методы, поддерживающие создание, обработку и связывание элементов.

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="model"/> конечную вершину.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/>, в который добавляются созданные элементы.</param>
        /// <param name="pos">Текущая позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенная конечная вершина ЛСА.</returns>
        private static EndVertex? HandleEndVertex(AbstractAutomaton model, int pos, ref Dictionary<int, bool> parsedPositions, ref List<ParsingError> errors)
        {
            if (model.End != null)
            {
                AddError(errors, "Конечная вершина 'Yк' может быть только одна", pos);
                return null;
            }

            var end = model.AddVertex(new EndVertex());
            parsedPositions[pos] = true;
            return end;
        }

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="model"/> операторную вершину.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/>, в который добавляются созданные элементы.</param>
        /// <param name="pos">Текущая позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="index">Индекс операторной вершины.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенная операторная вершина ЛСА.</returns>
        private static OperatorVertex CreateOperatorVertex(ParserContext ctx, AbstractAutomaton model, int pos, ref Dictionary<int, bool> parsedPositions, int index, ref List<ParsingError> errors)
        {
            var vertexId = $"Y{index}";

            // Проверка уникальности ID операторной вершины
            if (ctx.OperatorVertexIds.Contains(vertexId))
            {
                AddError(errors, $"Операторная вершина '{vertexId}' уже существует в алгоритме", pos);
            }
            else
            {
                ctx.OperatorVertexIds.Add(vertexId);
            }

            var vertex = model.AddVertex(new OperatorVertex(index));
            parsedPositions[pos] = true;
            return vertex;
        }

        /// <summary>
        /// Создаёт, настраивает и добавляет в <paramref name="model"/> точку перехода.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/>, в который добавляются созданные элементы.</param>
        /// <param name="pos">Текущая позицию в списке токенов для прохода по нему.</param>
        /// <param name="parsedPositions">Словарь, определяющий состояние элементов на позициях (пропарсены или нет).</param>
        /// <param name="index">Индекс точки перехода.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        /// <returns>Настроенная точка перехода ЛСА.</returns>
        private static JumpPoint CreateJumpPoint(ParserContext ctx, AbstractAutomaton model, int pos, ref Dictionary<int, bool> parsedPositions, int index, ref List<ParsingError> errors)
        {
            var jumpPointId = $"↓{index}";

            // Проверка повторного объявления точки перехода в тексте ЛСА
            if (ctx.JumpPointIds.Contains(jumpPointId))
            {
                AddError(errors, $"Точка перехода '{jumpPointId}' уже существует в алгоритме", pos);
            }
            else
            {
                ctx.JumpPointIds.Add(jumpPointId);
            }

            // В модели исключаем дубликаты через EnsureJumpPoint
            var jumpPoint = model.EnsureJumpPoint(index);
            parsedPositions[pos] = true;
            return jumpPoint;
        }

        /// <summary>
        /// Связывает предыдущий элемент с текущим, если предыдущий элемент не равен <see langword="null"/>.
        /// </summary>
        /// <param name="previous">Предыдущий элемент ЛСА (возможно значение <see langword="null"/>).</param>
        /// <param name="current">Текущий элемент ЛСА (возможно значение <see langword="null"/>).</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void LinkPrevious(IBDVertex? previous, IBDVertex? current, ref List<ParsingError> errors)
        {
            if (previous == null || current == null) return;
            // Защита от самосвязывания (например, когда w↑i непосредственно предшествует ↓i той же точки)
            if (ReferenceEquals(previous, current)) return;
            previous.Next = current;
        }

        #endregion





        #region Методы валидации

        /// <summary>
        /// Проверяет уникальность начальной и конечной вершин.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/> для проверки.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void ValidateStartEndUniqueness(AbstractAutomaton model, ref List<ParsingError> errors)
        {
            var startVertices = model.Vertices.OfType<StartVertex>().Count();
            var endVertices = model.Vertices.OfType<EndVertex>().Count();

            if (startVertices > 1)
                AddError(errors, "Начальная вершина 'Yн' может быть только одна", 0);

            if (endVertices > 1)
                AddError(errors, "Конечная вершина 'Yк' может быть только одна", 0);

            if (endVertices == 0)
                AddError(errors, "ЛСА должна содержать конечную вершину 'Yк'", 0);
        }

        /// <summary>
        /// Проверяет корректность условных вершин.
        /// </summary>
        /// <param name="model">Объект <see cref="AbstractAutomaton"/> для проверки.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void ValidateConditionalVertices(AbstractAutomaton model, ref List<ParsingError> errors)
        {
            var conditionalVertices = model.Vertices.OfType<ConditionalVertex>().ToList();

            foreach (var vertex in conditionalVertices)
            {
                if (vertex.LBS == null)
                    AddError(errors, $"Условная вершина '{vertex.ID}' должна иметь левую ветвь (LBS)", 0);

                if (vertex.RBS == null)
                    AddError(errors, $"Условная вершина '{vertex.ID}' должна иметь правую ветвь (RBS)", 0);
            }
        }

        /// <summary>
        /// Запрещает конструкции вида "{X/P}i ↑{j} w↑{j}", где правая ветвь условной вершины
        /// состоит только из безусловного перехода на ту же точку, что указана в LBS.
        /// </summary>
        /// <param name="tokens">Список токенов ЛСА.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void ValidateNoMeaninglessConditionalRightBranch(List<string> tokens, ref List<ParsingError> errors)
        {
            // Ищем последовательности: Xk/Pk, затем ↑j, затем w↑j (тот же j)
            for (int i = 0; i + 2 < tokens.Count; i++)
            {
                var t0 = tokens[i];
                var t1 = tokens[i + 1];
                var t2 = tokens[i + 2];

                bool isConditional = t0.StartsWith('X') || t0.StartsWith('P');
                if (!isConditional) continue;
                if (!t1.StartsWith('↑')) continue;
                if (!t2.StartsWith("w↑")) continue;

                // Сравниваем идентификаторы точки
                if (int.TryParse(t1[1..], out var j1) && int.TryParse(t2[2..], out var j2))
                {
                    if (j1 == j2)
                    {
                        AddError(
                            errors,
                            $"Недопустимая конструкция у условной вершины: правая ветвь не может состоять только из безусловного перехода на ту же точку. Запрещено: '{t0} {t1} {t2}'",
                            i + 2
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет соответствие между операторами перехода (↑j, w↑j) и точками перехода (↓j) в исходном токенизированном ЛСА.
        /// </summary>
        /// <param name="tokens">Список токенов ЛСА.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void ValidateJumpOperatorsAndPointsExistence(List<string> tokens, ref List<ParsingError> errors)
        {
            var operatorFirstPositions = new Dictionary<int, int>(); // индекс j -> первая позиция оператора (↑j или w↑j)
            var pointFirstPositions = new Dictionary<int, int>();    // индекс j -> первая позиция точки (↓j)

            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token.StartsWith("w↑"))
                {
                    if (int.TryParse(token.Substring(2), out int idx))
                    {
                        if (!operatorFirstPositions.ContainsKey(idx))
                            operatorFirstPositions[idx] = i;
                    }
                }
                else if (token.StartsWith('↑'))
                {
                    if (int.TryParse(token.Substring(1), out int idx))
                    {
                        if (!operatorFirstPositions.ContainsKey(idx))
                            operatorFirstPositions[idx] = i;
                    }
                }
                else if (token.StartsWith('↓'))
                {
                    if (int.TryParse(token.Substring(1), out int idx))
                    {
                        if (!pointFirstPositions.ContainsKey(idx))
                            pointFirstPositions[idx] = i;
                    }
                }
            }

            // Проверка: каждый оператор перехода должен иметь соответствующую точку ↓j
            foreach (var kv in operatorFirstPositions)
            {
                int idx = kv.Key;
                int pos = kv.Value;
                if (!pointFirstPositions.ContainsKey(idx))
                {
                    AddError(errors, $"Для оператора перехода с индексом {idx} отсутствует соответствующая точка '↓{idx}'", pos);
                }
            }

            // Обратная проверка: для каждой точки ↓j должен существовать хотя бы один оператор ↑j или w↑j
            foreach (var kv in pointFirstPositions)
            {
                int idx = kv.Key;
                int pos = kv.Value;
                if (!operatorFirstPositions.ContainsKey(idx))
                {
                    AddError(errors, $"Для точки перехода '↓{idx}' отсутствует хоть один оператор перехода ('↑{idx}' или 'w↑{idx}')", pos);
                }
            }
        }

        /// <summary>
        /// Проверяет отсутствие недопустимых последовательностей точек перехода:
        /// подряд идущих точек ("↓i ↓j") или разделённых только безусловными переходами ("w↑k"),
        /// например: "↓i w↑k ↓j".
        /// Между точками перехода должна находиться хотя бы одна вершина (Yx, Xx, Px, Yн, Yк).
        /// </summary>
        /// <param name="tokens">Список токенов ЛСА.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void ValidateNoConsecutiveJumpPoints(List<string> tokens, ref List<ParsingError> errors)
        {
            bool awaitingVertexAfterPoint = false;
            int lastPointPos = -1;

            for (int i = 0; i < tokens.Count; i++)
            {
                var t = tokens[i];

                // Вершины (считаются разделителями между точками)
                bool isVertex = t == "Yн" || t == "Yк" || t.StartsWith('Y') || t.StartsWith('X') || t.StartsWith('P');

                if (t.StartsWith('↓'))
                {
                    if (awaitingVertexAfterPoint)
                    {
                        AddError(errors, "Недопустимо ставить несколько точек перехода подряд или разделять их только безусловными переходами (требуется вершина между точками)", i);
                    }
                    awaitingVertexAfterPoint = true;
                    lastPointPos = i;
                    continue;
                }

                // Безусловные и условные операторы перехода (↑j, w↑j) не снимают требование наличия вершины
                if (t.StartsWith('↑') || t.StartsWith("w↑"))
                {
                    continue;
                }

                if (isVertex)
                {
                    // Появилась вершина — требование выполнено, отменяем флаг ожидания
                    awaitingVertexAfterPoint = false;
                }
            }
        }

        /// <summary>
        /// Проверяет отсутствие подряд идущих операторов перехода (jump-операторов):
        /// "w↑i w↑j", "w↑i ↑j", "↑i ↑j". Допускается последовательность "↑i w↑j"
        /// сразу после условной вершины (X/P).
        /// </summary>
        /// <param name="tokens">Список токенов ЛСА.</param>
        /// <param name="errors">Ссылка на список ошибок парсера.</param>
        private static void ValidateNoConsecutiveJumpOperators(List<string> tokens, ref List<ParsingError> errors)
        {
            for (int i = 0; i + 1 < tokens.Count; i++)
            {
                var a = tokens[i];
                var b = tokens[i + 1];

                bool aIsJumpOp = a.StartsWith("w↑") || a.StartsWith('↑');
                bool bIsJumpOp = b.StartsWith("w↑") || b.StartsWith('↑');

                if (!aIsJumpOp || !bIsJumpOp)
                    continue;

                // Исключение: "↑i w↑j" допустимо (после условной вершины)
                if (a.StartsWith('↑') && b.StartsWith("w↑"))
                    continue;

                AddError(
                    errors,
                    $"Недопустима последовательность операторов перехода: '{a} {b}'",
                    i + 1
                );
            }
        }

        /// <summary>
        /// Добавляет ошибку в список ошибок парсера.
        /// </summary>
        /// <param name="errors">Список ошибок парсера.</param>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="position">Позиция в списке токенов, где произошла ошибка.</param>
        private static void AddError(List<ParsingError> errors, string message, int position)
        {
            errors.Add(new ParsingError(message, position));
        }

        #endregion
    }
}
