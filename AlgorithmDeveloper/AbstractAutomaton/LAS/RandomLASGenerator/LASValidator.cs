using System.Collections.Generic;
using AlgorithmDeveloper.AAModel;

namespace AlgorithmDeveloper.AAModel.LAS.RandomLASGenerator
{
    /// <summary>
    /// Валидатор для проверки корректности сгенерированных ЛСА
    /// </summary>
    public class LASValidator
    {

        /// <summary>
        /// Проверяет корректность ЛСА строки
        /// </summary>
        /// <param name="lasString">Строка с ЛСА для проверки</param>
        /// <returns>true, если ЛСА корректна; иначе false</returns>
        public bool IsValid(string lasString)
        {
            if (string.IsNullOrWhiteSpace(lasString))
                return false;

            try
            {
                return LASParser.TryParse(lasString, out AbstractAutomaton? model, out List<ParsingError> errors);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет корректность ЛСА и возвращает детальную информацию об ошибках
        /// </summary>
        /// <param name="las">Строка ЛСА для проверки</param>
        /// <param name="model">Созданная модель алгоритма (если валидация успешна)</param>
        /// <param name="errorMessage">Сообщение об ошибке (если валидация неуспешна)</param>
        /// <returns>true, если ЛСА корректна; false в противном случае</returns>
        public bool TryValidate(string las, out AbstractAutomaton? model, out string errorMessage)
        {
            var result = LASParser.TryParse(las, out model, out ParsingAggregateException? exception);
            errorMessage = exception?.Message ?? string.Empty;
            return result;
        }

        /// <summary>
        /// Выполняет полный парсинг ЛСА строки
        /// </summary>
        /// <param name="lasString">Строка с ЛСА для парсинга</param>
        /// <returns>Модель алгоритма</returns>
        /// <exception cref="ParsingAggregateException">Возникает при ошибках парсинга</exception>
        public AbstractAutomaton Parse(string lasString)
        {
            return LASParser.Parse(lasString);
        }
    }
}