using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmDeveloper.AlgorithmModel.LAS.RandomLASGenerator
{
    /// <summary>
    /// Контекст генерации ЛСА для отслеживания состояния и ограничений
    /// </summary>
    public class LASGenerationContext
    {
        /// <summary>
        /// Текущее состояние генерации
        /// </summary>
        public LASGenerationState CurrentState { get; set; } = LASGenerationState.Start;

        /// <summary>
        /// Сгенерированные токены
        /// </summary>
        public List<string> GeneratedTokens { get; } = new List<string>();

        /// <summary>
        /// Счетчик для генерации уникальных ID операторных вершин
        /// </summary>
        public int OperatorVertexIdCounter { get; set; } = 1;

        /// <summary>
        /// Счетчик для генерации уникальных ID условных вершин
        /// </summary>
        public int ConditionalVertexIdCounter { get; set; } = 1;

        /// <summary>
        /// Счетчик для генерации уникальных ID переходов
        /// </summary>
        public int JumpIdCounter { get; set; } = 1;

        /// <summary>
        /// Стек открытых переходов (ID переходов, которые нужно закрыть точками перехода)
        /// </summary>
        public Stack<int> OpenJumps { get; } = new Stack<int>();

        /// <summary>
        /// Список уже определенных точек перехода (закрытых ↓j), на которые можно делать обратные переходы
        /// </summary>
        public List<int> ClosedJumpPoints { get; } = new List<int>();

        /// <summary>
        /// Стек состояний для возврата после обработки ветвей условных вершин
        /// </summary>
        public Stack<LASGenerationState> StateStack { get; } = new Stack<LASGenerationState>();



        /// <summary>
        /// Кадр ветвления для текущей условной вершины: фиксирует, что левая/правая ветвь непустая
        /// </summary>
        public class BranchFrame
        {
            public bool LeftHasContent { get; set; } = false;
            public bool RightHasContent { get; set; } = false;
        }

        /// <summary>
        /// Стек кадров ветвления для вложенных условных конструкций
        /// </summary>
        public Stack<BranchFrame> BranchStack { get; } = new Stack<BranchFrame>();



        /// <summary>
        /// Признак: последний добавленный токен был операторной вершиной (Yk)
        /// </summary>
        public bool LastTokenWasOperator { get; set; } = false;

        /// <summary>
        /// Признак: последний добавленный токен был точкой перехода (↓j)
        /// </summary>
        public bool LastTokenWasJumpPoint { get; set; } = false;

        /// <summary>
        /// Признак: последний добавленный токен был оператором безусловного перехода (w↑j)
        /// Используется для запрета подряд идущих операторов перехода (например, "w↑2 w↑3").
        /// Допускается последовательность "↑i w↑j" сразу после условной вершины.
        /// </summary>
        public bool LastTokenWasUnconditionalJumpOperator { get; set; } = false;

        /// <summary>
        /// Количество сгенерированных операторных вершин
        /// </summary>
        public int OperatorVertexCount => GeneratedTokens.Count(t => t.StartsWith("Y") && !t.Equals("Yн") && !t.Equals("Yк"));

        /// <summary>
        /// Количество сгенерированных условных вершин
        /// </summary>
        public int ConditionalVertexCount => GeneratedTokens.Count(t => t.StartsWith("X") || t.StartsWith("P"));

        /// <summary>
        /// Общее количество токенов
        /// </summary>
        public int TokenCount => GeneratedTokens.Count;

        /// <summary>
        /// Проверяет, можно ли завершить генерацию
        /// </summary>
        public bool CanFinish => OpenJumps.Count == 0 && 
                                (CurrentState == LASGenerationState.AfterOperator || 
                                 CurrentState == LASGenerationState.AfterStart ||
                                 CurrentState == LASGenerationState.AfterJumpPoint);

        /// <summary>
        /// Добавляет токен в список сгенерированных
        /// </summary>
        public void AddToken(string token)
        {
            GeneratedTokens.Add(token);
        }

        /// <summary>
        /// Получает строковое представление сгенерированной ЛСА
        /// </summary>
        public string GetGeneratedLAS()
        {
            return string.Join(" ", GeneratedTokens);
        }

        /// <summary>
        /// Сбрасывает контекст для новой генерации
        /// </summary>
        public void Reset()
        {
            CurrentState = LASGenerationState.Start;
            GeneratedTokens.Clear();
            OperatorVertexIdCounter = 1;
            ConditionalVertexIdCounter = 1;
            JumpIdCounter = 1;
            OpenJumps.Clear();
            ClosedJumpPoints.Clear();
            StateStack.Clear();
            BranchStack.Clear();
            LastTokenWasOperator = false;
            LastTokenWasJumpPoint = false;
            LastTokenWasUnconditionalJumpOperator = false;
            
            // Сбрасываем счетчик обратных переходов, чтобы каждый запуск генерации
            // имел собственный лимит на количество циклов
            CycleJumpCount = 0;
        }

        /// <summary>
        /// Счетчик сгенерированных обратных переходов (для ограничения числа циклов)
        /// </summary>
        public int CycleJumpCount { get; set; } = 0;

        /// <summary>
        /// Проверяет, удовлетворяет ли текущий контекст заданным ограничениям
        /// </summary>
        public bool SatisfiesConstraints(LASGenerationOptions options)
        {
            if (options.MinTokenCount.HasValue && TokenCount < options.MinTokenCount.Value)
                return false;

            if (options.MaxTokenCount.HasValue && TokenCount > options.MaxTokenCount.Value)
                return false;

            if (options.OperatorVertexCount.HasValue && OperatorVertexCount != options.OperatorVertexCount.Value)
                return false;

            if (options.ConditionalVertexCount.HasValue && ConditionalVertexCount != options.ConditionalVertexCount.Value)
                return false;

            if (options.MinOperatorVertexCount.HasValue && OperatorVertexCount < options.MinOperatorVertexCount.Value)
                return false;

            if (options.MaxOperatorVertexCount.HasValue && OperatorVertexCount > options.MaxOperatorVertexCount.Value)
                return false;

            if (options.MinConditionalVertexCount.HasValue && ConditionalVertexCount < options.MinConditionalVertexCount.Value)
                return false;

            if (options.MaxConditionalVertexCount.HasValue && ConditionalVertexCount > options.MaxConditionalVertexCount.Value)
                return false;

            return true;
        }
    }
}