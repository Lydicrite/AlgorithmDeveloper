using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmDeveloper.AlgorithmModel.LAS.RandomLASGenerator
{
    /// <summary>
    /// Генератор случайных ЛСА (Логических Схем Алгоритмов) на основе конечного автомата.
    /// Выполнен как статический и потокобезопасен: контекст создаётся на каждый вызов.
    /// </summary>
    public static class RandomLASGenerator
    {

        /// <inheritdoc />
        public static string GenerateByMinTokenCount(int minTokenCount, int? seed = null)
        {
            var options = LASGenerationOptions.WithMinTokenCount(minTokenCount, seed);
            return Generate(options);
        }

        /// <inheritdoc />
        public static string GenerateByOperatorVertexCount(int operatorVertexCount, int? seed = null)
        {
            var options = LASGenerationOptions.WithOperatorVertexCount(operatorVertexCount, seed);
            return Generate(options);
        }

        /// <inheritdoc />
        public static string GenerateByConditionalVertexCount(int conditionalVertexCount, int? seed = null)
        {
            var options = LASGenerationOptions.WithConditionalVertexCount(conditionalVertexCount, seed);
            return Generate(options);
        }

        /// <inheritdoc />
        public static string Generate(LASGenerationOptions options)
        {
            if (options.Seed.HasValue)
            {
                var seededRandom = new Random(options.Seed.Value);
                return GenerateWithRandom(options, seededRandom);
            }

            return GenerateWithRandom(options, Random.Shared);
        }

        private static string GenerateWithRandom(LASGenerationOptions options, Random random)
        {
            for (int attempt = 0; attempt < options.MaxAttempts; attempt++)
            {
                var context = new LASGenerationContext();
                
                try
                {
                    if (GenerateLAS(options, random, context))
                    {
                        var result = context.GetGeneratedLAS();
                        if (ValidateGeneratedLAS(result))
                        {
                            return result;
                        }
                    }
                }
                catch (Exception)
                {
                    // Игнорируем ошибки генерации и пробуем снова
                    continue;
                }
            }

            throw new InvalidOperationException($"Не удалось сгенерировать корректную ЛСА за {options.MaxAttempts} попыток с заданными ограничениями.");
        }

        private static bool GenerateLAS(LASGenerationOptions options, Random random, LASGenerationContext context)
        {
            // Начинаем с генерации стартовой вершины
            context.AddToken("Yн");
            context.CurrentState = LASGenerationState.AfterStart;

            while (context.CurrentState != LASGenerationState.Finished)
            {
                if (!GenerateNextToken(options, random, context))
                {
                    return false;
                }

                // Проверяем ограничения на каждом шаге
                if (options.MaxTokenCount.HasValue && context.TokenCount > options.MaxTokenCount.Value)
                {
                    return false;
                }

                if (!options.MaxTokenCount.HasValue)
                {
                    var cap = options.MinTokenCount.HasValue ? Math.Max(50, options.MinTokenCount.Value * 10) : 1000;
                    if (context.TokenCount > cap)
                    {
                        return false;
                    }
                }
            }

            return context.SatisfiesConstraints(options);
        }

        private static bool GenerateNextToken(LASGenerationOptions options, Random random, LASGenerationContext context)
        {
            switch (context.CurrentState)
            {
                case LASGenerationState.AfterStart:
                case LASGenerationState.AfterOperator:
                case LASGenerationState.AfterJumpPoint:
                    return GenerateFromNormalState(options, random, context);

                case LASGenerationState.AfterConditional:
                    return GenerateLeftBranch(random, context);

                case LASGenerationState.InLeftBranch:
                    return GenerateInBranch(options, random, context, true);

                case LASGenerationState.InRightBranch:
                    return GenerateInBranch(options, random, context, false);

                default:
                    return false;
            }
        }

        private static bool GenerateFromNormalState(LASGenerationOptions options, Random random, LASGenerationContext context)
        {
            var possibleActions = GetPossibleActions(options, context);
            if (possibleActions.Count == 0)
            {
                return false;
            }

            var weights = GetActionWeights(possibleActions, options, context);
            var selectedAction = SelectWeightedAction(possibleActions, weights, random);

            return ExecuteAction(selectedAction, options, random, context);
        }

        private static List<string> GetPossibleActions(LASGenerationOptions options, LASGenerationContext context)
        {
            var actions = new List<string>();

            // Всегда можем генерировать операторные и условные вершины
            actions.Add("operator");
            actions.Add("conditional");

            // Можем генерировать безусловный переход к открытой точке (w↑j),
            // только если есть открытые переходы и мы не находимся сразу после точки
            if (context.CurrentState != LASGenerationState.AfterJumpPoint &&
                context.OpenJumps.Count > 0 &&
                !context.LastTokenWasUnconditionalJumpOperator)
            {
                actions.Add("jump");
            }

            // Можем завершить, если нет открытых переходов
            if (context.CanFinish)
            {
                actions.Add("finish");
            }

            // Можем закрыть переход, если есть открытые переходы,
            // но не допускаем подряд идущие точки
            if (context.OpenJumps.Count > 0 && !context.LastTokenWasJumpPoint)
            {
                actions.Add("close_jump");
            }

            // Обратный переход (цикл) возможен, если уже были определены точки ↓j и разрешено опциями
            if (options.EnableCycles &&
                context.ClosedJumpPoints.Count >= options.MinClosedJumpPointsForCycle &&
                !context.LastTokenWasJumpPoint &&
                !context.LastTokenWasUnconditionalJumpOperator)
            {
                actions.Add("back_jump");
            }

            return actions;
        }

        private static List<double> GetActionWeights(List<string> actions, LASGenerationOptions options, LASGenerationContext context)
        {
            var weights = new List<double>();

            foreach (var action in actions)
            {
                switch (action)
                {
                    case "operator":
                        weights.Add(GetOperatorWeight(options, context));
                        break;
                    case "conditional":
                        weights.Add(GetConditionalWeight(options, context));
                        break;
                    case "jump":
                        weights.Add(GetJumpWeight(options, context));
                        break;
                    case "back_jump":
                        weights.Add(GetBackJumpWeight(options, context));
                        break;
                    case "finish":
                        weights.Add(GetFinishWeight(options, context));
                        break;
                    case "close_jump":
                        weights.Add(GetCloseJumpWeight(options, context));
                        break;
                    case "end_branch":
                        weights.Add(GetEndBranchWeight(options, context));
                        break;
                    default:
                        weights.Add(0.1);
                        break;
                }
            }

            return weights;
        }

        private static double GetOperatorWeight(LASGenerationOptions options, LASGenerationContext context)
        {
            if (options.OperatorVertexCount.HasValue)
            {
                if (context.OperatorVertexCount >= options.OperatorVertexCount.Value)
                    return 0.0;
                if (context.OperatorVertexCount < options.OperatorVertexCount.Value)
                    return 0.8;
            }

            if (options.MaxOperatorVertexCount.HasValue && context.OperatorVertexCount >= options.MaxOperatorVertexCount.Value)
                return 0.0;

            var baseWeight = options.OperatorVertexProbability;
            if (options.MinTokenCount.HasValue && context.TokenCount >= options.MinTokenCount.Value)
            {
                baseWeight *= 0.5;
            }
            return baseWeight;
        }

        private static double GetConditionalWeight(LASGenerationOptions options, LASGenerationContext context)
        {
            if (options.ConditionalVertexCount.HasValue)
            {
                if (context.ConditionalVertexCount >= options.ConditionalVertexCount.Value)
                    return 0.0;
                if (context.ConditionalVertexCount < options.ConditionalVertexCount.Value)
                    return 0.8;
            }

            if (options.MaxConditionalVertexCount.HasValue && context.ConditionalVertexCount >= options.MaxConditionalVertexCount.Value)
                return 0.0;

            var baseWeight = options.ConditionalVertexProbability;
            if (options.MinTokenCount.HasValue && context.TokenCount >= options.MinTokenCount.Value)
            {
                baseWeight *= 0.5;
            }
            // Если накопилось много открытых переходов, уменьшаем вероятность новых развилок
            if (context.OpenJumps.Count >= 2)
            {
                baseWeight *= 0.6;
            }
            if (context.OpenJumps.Count >= 3)
            {
                baseWeight *= 0.66; // дополнительное мягкое снижение
            }
            return baseWeight;
        }

        private static double GetFinishWeight(LASGenerationOptions options, LASGenerationContext context)
        {
            // Увеличиваем вероятность завершения, если приближаемся к ограничениям
            if (options.MaxTokenCount.HasValue)
            {
                var ratio = (double)context.TokenCount / options.MaxTokenCount.Value;
                if (ratio > 0.8)
                    return 0.6;
            }

            // Проверяем, достигли ли мы минимальных требований
            if (options.MinTokenCount.HasValue && context.TokenCount < options.MinTokenCount.Value)
                return 0.0;

            if (options.OperatorVertexCount.HasValue && context.OperatorVertexCount < options.OperatorVertexCount.Value)
                return 0.0;

            if (options.ConditionalVertexCount.HasValue && context.ConditionalVertexCount < options.ConditionalVertexCount.Value)
                return 0.0;

            if (!options.MaxTokenCount.HasValue && options.MinTokenCount.HasValue && context.TokenCount >= options.MinTokenCount.Value)
            {
                var beyond = context.TokenCount - options.MinTokenCount.Value;
                var ramp = Math.Min(0.5, beyond * 0.02);
                return Math.Max(options.FinishProbability, options.FinishProbability + ramp);
            }

            return options.FinishProbability;
        }

        private static double GetJumpWeight(LASGenerationOptions options, LASGenerationContext context)
        {
            var baseWeight = options.JumpProbability;
            if (options.MinTokenCount.HasValue && context.TokenCount >= options.MinTokenCount.Value)
            {
                baseWeight = Math.Max(0.02, baseWeight * 0.25);
                if (context.OpenJumps.Count > 0)
                {
                    baseWeight *= 0.5;
                }
            }
            return baseWeight;
        }

        private static double GetCloseJumpWeight(LASGenerationOptions options, LASGenerationContext context)
        {
            var baseWeight = 0.3;
            if (context.OpenJumps.Count > 0 && options.MinTokenCount.HasValue && context.TokenCount >= options.MinTokenCount.Value)
            {
                baseWeight = 0.5;
            }
            // Дополнительно усиливаем закрытие при большом количестве открытых переходов
            if (context.OpenJumps.Count >= 2)
            {
                baseWeight = Math.Max(baseWeight, 0.6);
            }
            if (context.OpenJumps.Count >= 3)
            {
                baseWeight = Math.Max(baseWeight, 0.75);
            }
            return baseWeight;
        }

        private static double GetBackJumpWeight(LASGenerationOptions options, LASGenerationContext context)
        {
            if (!options.EnableCycles)
                return 0.0;

            if (context.ClosedJumpPoints.Count < options.MinClosedJumpPointsForCycle)
                return 0.0;

            if (options.MaxCycleJumpCount.HasValue && context.CycleJumpCount >= options.MaxCycleJumpCount.Value)
                return 0.0;

            var baseWeight = options.CycleJumpProbability;
            // Когда много открытых переходов, немного снижаем шанс новых петель
            if (context.OpenJumps.Count >= 2)
            {
                baseWeight *= 0.7;
            }
            if (context.OpenJumps.Count >= 3)
            {
                baseWeight *= 0.6;
            }

            return baseWeight;
        }

        private static string SelectWeightedAction(List<string> actions, List<double> weights, Random random)
        {
            var totalWeight = weights.Sum();
            if (totalWeight == 0)
            {
                return actions[random.Next(actions.Count)];
            }

            var randomValue = random.NextDouble() * totalWeight;
            var currentWeight = 0.0;

            for (int i = 0; i < actions.Count; i++)
            {
                currentWeight += weights[i];
                if (randomValue <= currentWeight)
                {
                    return actions[i];
                }
            }

            return actions.Last();
        }

        private static bool ExecuteAction(string action, LASGenerationOptions options, Random random, LASGenerationContext context)
        {
            switch (action)
            {
                case "operator":
                    return GenerateOperatorVertex(context);
                case "conditional":
                    return GenerateConditionalVertex(random, context);
                case "jump":
                    return GenerateJump(context);
                case "back_jump":
                    return GenerateBackJump(options, random, context);
                case "finish":
                    return GenerateFinish(context);
                case "close_jump":
                    return GenerateJumpPoint(context);
                default:
                    return false;
            }
        }

        private static bool GenerateOperatorVertex(LASGenerationContext context)
        {
            var id = context.OperatorVertexIdCounter++;
            context.AddToken($"Y{id}");
            context.CurrentState = LASGenerationState.AfterOperator;
            context.LastTokenWasOperator = true;
            context.LastTokenWasJumpPoint = false;
            context.LastTokenWasUnconditionalJumpOperator = false;
            return true;
        }

        private static bool GenerateConditionalVertex(Random random, LASGenerationContext context)
        {
            var id = context.ConditionalVertexIdCounter++;
            var vertexType = random.Next(2) == 0 ? "X" : "P";
            // Запоминаем состояние окружения для корректного возврата после правой ветви
            var prevState = context.CurrentState;
            context.AddToken($"{vertexType}{id}");
            context.CurrentState = LASGenerationState.AfterConditional;
            context.LastTokenWasOperator = false;
            context.LastTokenWasJumpPoint = false;
            context.LastTokenWasUnconditionalJumpOperator = false;
            // Открываем кадр ветвления для данной условной вершины
            context.BranchStack.Push(new LASGenerationContext.BranchFrame());
            // Подготовим целевое состояние возврата после завершения правой ветви
            var returnState = (prevState == LASGenerationState.InLeftBranch || prevState == LASGenerationState.InRightBranch)
                ? prevState
                : LASGenerationState.AfterOperator;
            context.StateStack.Push(returnState);
            return true;
        }

        private static bool GenerateJump(LASGenerationContext context)
        {
            // Генерируем безусловный переход к верхней открытой точке (w↑j) и сразу закрываем её ↓j
            if (context.OpenJumps.Count == 0)
                return false;

            var jumpId = context.OpenJumps.Peek();
            context.AddToken($"w↑{jumpId}");
            context.LastTokenWasOperator = false;
            context.LastTokenWasJumpPoint = false;
            context.LastTokenWasUnconditionalJumpOperator = true;

            // Отмечаем наличие контента в активной ветви
            MarkCurrentBranchHasContent(context);

            var closed = GenerateJumpPoint(context);
            if (!closed)
                return false;

            return true;
        }

        private static bool GenerateJumpPoint(LASGenerationContext context)
        {
            if (context.OpenJumps.Count == 0)
                return false;

            // Запрет подряд идущих точек перехода
            if (context.LastTokenWasJumpPoint)
                return false;

            var jumpId = context.OpenJumps.Pop();

            // Правило: после Y перед точкой должен стоять безусловный переход w↑j
            if (context.LastTokenWasOperator)
            {
                context.AddToken($"w↑{jumpId}");
                context.LastTokenWasOperator = false;
                context.LastTokenWasJumpPoint = false;
                context.LastTokenWasUnconditionalJumpOperator = true;
            }

            context.AddToken($"↓{jumpId}");
            // Запоминаем определенную точку для возможных обратных переходов в будущем
            context.ClosedJumpPoints.Add(jumpId);
            // Закрытие точки перехода также считается контентом ветви, если точка была сгенерирована внутри ветви
            MarkCurrentBranchHasContent(context);
            context.CurrentState = LASGenerationState.AfterJumpPoint;
            context.LastTokenWasJumpPoint = true;
            context.LastTokenWasUnconditionalJumpOperator = false;
            return true;
        }

        private static bool GenerateFinish(LASGenerationContext context)
        {
            context.AddToken("Yк");
            context.CurrentState = LASGenerationState.Finished;
            context.LastTokenWasOperator = false;
            context.LastTokenWasJumpPoint = false;
            context.LastTokenWasUnconditionalJumpOperator = false;
            return true;
        }

        private static bool GenerateLeftBranch(Random random, LASGenerationContext context)
        {
            // Сразу после условной вершины генерируем оператор условного перехода ↑j
            var jumpId = context.JumpIdCounter++;
            context.AddToken($"↑{jumpId}");
            context.OpenJumps.Push(jumpId);
            context.CurrentState = LASGenerationState.InLeftBranch;
            context.LastTokenWasOperator = false;
            context.LastTokenWasJumpPoint = false;
            context.LastTokenWasUnconditionalJumpOperator = false;
            // Появление ↑j считается контентом в левой ветви
            MarkCurrentBranchHasContent(context);
            return true;
        }

        private static bool GenerateInBranch(LASGenerationOptions options, Random random, LASGenerationContext context, bool isLeftBranch)
        {
            // В ветви можем генерировать операторные вершины, условные вершины или переходы
            var possibleActions = new List<string> { "operator", "conditional" };

            // Безусловный переход к открытой точке разрешён только если есть открытые точки и
            // предыдущий токен не был точкой перехода (исключаем последовательности вида ↓i w↑j ↓j)
            if (context.OpenJumps.Count > 0 && !context.LastTokenWasJumpPoint && !context.LastTokenWasUnconditionalJumpOperator)
            {
                possibleActions.Add("jump");
            }

            // Дополнительно разрешаем обратные переходы (циклы), если есть закрытые точки
            if (options.EnableCycles &&
                context.ClosedJumpPoints.Count >= options.MinClosedJumpPointsForCycle &&
                !context.LastTokenWasJumpPoint &&
                !context.LastTokenWasUnconditionalJumpOperator)
            {
                possibleActions.Add("back_jump");
            }

            // Разрешаем завершить ветвь только если она непустая
            if (context.BranchStack.Count > 0)
            {
                var frame = context.BranchStack.Peek();
                var hasContent = isLeftBranch ? frame.LeftHasContent : frame.RightHasContent;
                if (hasContent)
                {
                    possibleActions.Add("end_branch");
                }

                // Блокируем шаблон "{X/P}i ↑{j} w↑{j}":
                // В правой ветви, если она ещё пуста, запрещаем сразу безусловный переход на открытую точку
                if (!isLeftBranch && !frame.RightHasContent)
                {
                    possibleActions.Remove("jump");
                }
            }

            // Закрывать переход можно только если есть открытые переходы и предыдущий токен не точка
            if (context.OpenJumps.Count > 0 && !context.LastTokenWasJumpPoint)
            {
                possibleActions.Add("close_jump");
            }

            var weights = GetActionWeights(possibleActions, options, context);
            var selectedAction = SelectWeightedAction(possibleActions, weights, random);

            switch (selectedAction)
            {
                case "operator":
                    {
                        var ok = GenerateOperatorVertex(context);
                        if (ok) MarkCurrentBranchHasContent(context);
                        return ok;
                    }
                case "conditional":
                    {
                        var ok = GenerateConditionalVertex(random, context);
                        if (ok) MarkCurrentBranchHasContent(context);
                        return ok;
                    }
                case "jump":
                    return GenerateJump(context);
                case "back_jump":
                    {
                        var ok = GenerateBackJump(options, random, context);
                        if (ok) MarkCurrentBranchHasContent(context);
                        return ok;
                    }
                case "close_jump":
                    return GenerateJumpPoint(context);
                case "end_branch":
                    return EndBranch(context, isLeftBranch);
                default:
                    return false;
            }
        }

        private static double GetEndBranchWeight(LASGenerationOptions options, LASGenerationContext context)
        {
            var baseWeight = 0.1;
            if (options.MinTokenCount.HasValue && context.TokenCount >= options.MinTokenCount.Value)
            {
                baseWeight += 0.2;
            }
            if (context.OpenJumps.Count > 0)
            {
                baseWeight += 0.1;
            }
            return Math.Min(baseWeight, 0.6);
        }

        private static bool EndBranch(LASGenerationContext context, bool isLeftBranch)
        {
            if (isLeftBranch)
            {
                context.CurrentState = LASGenerationState.InRightBranch;
                // Сигнализируем, что правая ветвь начинает наполняться
                if (context.BranchStack.Count > 0)
                {
                    var frame = context.BranchStack.Peek();
                    frame.RightHasContent = frame.RightHasContent; // без изменений, отметка произойдет при первом токене
                }
            }
            else
            {
                // Завершаем правую ветвь
                if (context.StateStack.Count > 0)
                {
                    context.CurrentState = context.StateStack.Pop();
                }
                else
                {
                    context.CurrentState = LASGenerationState.AfterOperator;
                }
                // Выходим из кадра ветвления
                if (context.BranchStack.Count > 0)
                {
                    context.BranchStack.Pop();
                }
            }
            return true;
        }

        private static bool GenerateBackJump(LASGenerationOptions options, Random random, LASGenerationContext context)
        {
            if (!options.EnableCycles)
                return false;

            var count = context.ClosedJumpPoints.Count;
            if (count == 0)
                return false;

            if (options.MaxCycleJumpCount.HasValue && context.CycleJumpCount >= options.MaxCycleJumpCount.Value)
                return false;

            // Выбираем цель с учётом смещения к недавним точкам
            var bias = Math.Max(0.0, Math.Min(1.0, options.CycleRecencyBias));
            int index;
            if (bias <= 0.0001)
            {
                index = random.Next(count);
            }
            else
            {
                // Вес точки i (от старой к новой) = bias^(distance), где distance = (count - i - 1)
                var weights = new double[count];
                for (int i = 0; i < count; i++)
                {
                    var distance = count - i - 1;
                    weights[i] = Math.Pow(bias, distance);
                }
                // Нормированный выбор
                var sum = weights.Sum();
                var r = random.NextDouble() * sum;
                double acc = 0.0;
                index = count - 1; // запасной вариант — самая новая
                for (int i = 0; i < count; i++)
                {
                    acc += weights[i];
                    if (r <= acc)
                    {
                        index = i;
                        break;
                    }
                }
            }

            var jumpId = context.ClosedJumpPoints[index];
            context.AddToken($"w↑{jumpId}");
            context.LastTokenWasOperator = false;
            context.LastTokenWasJumpPoint = false;
            context.LastTokenWasUnconditionalJumpOperator = true;
            context.CurrentState = LASGenerationState.AfterOperator;
            context.CycleJumpCount++;
            return true;
        }

        /// <summary>
        /// Помечает, что текущая активная ветвь содержит контент
        /// </summary>
        private static void MarkCurrentBranchHasContent(LASGenerationContext context)
        {
            if (context.BranchStack.Count == 0)
                return;

            var frame = context.BranchStack.Peek();
            if (context.CurrentState == LASGenerationState.InLeftBranch)
            {
                frame.LeftHasContent = true;
            }
            else if (context.CurrentState == LASGenerationState.InRightBranch)
            {
                frame.RightHasContent = true;
            }
        }

        private static bool ValidateGeneratedLAS(string las)
        {
            try
            {
                return LASParser.TryParse(las, out AlgoModel? _, out List<ParsingError> _);
            }
            catch
            {
                return false;
            }
        }
    }
}