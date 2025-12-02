using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgorithmDeveloper.AAModel.LAS.RandomLASGenerator;
using AlgorithmDeveloper.AAModel.LAS;
using AlgorithmDeveloper.AAModel;
using AlgorithmDeveloper.AAModel.TransitionSystem.Converters;
using AlgorithmDeveloper.AAModel.TransitionSystem;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using LogicalExpressions.Parsing;
using LogicalExpressions.Core;

namespace AlgorithmDeveloper.AAModel.LAS.RandomLASGenerator.Tests
{
    [TestClass]
    [assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 8)]
    public class RandomMSAToAAConverterTests
    {
        private LASValidator CreateValidator() => new LASValidator();

        private static string DescribeLAS(string las)
        {
            var tokens = las.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var operatorCount = tokens.Count(t => t.StartsWith("Y") && t != "Yн" && t != "Yк");
            var conditionalCount = tokens.Count(t => t.StartsWith("X") || t.StartsWith("P"));
            return $"ЛСА: {las}" +
                $"\n\tТокенов: {tokens.Length}" +
                $"\n\tОператорных вершин: {operatorCount}" +
                $"\n\tУсловных вершин: {conditionalCount}";
        }

        private static LASGenerationOptions BuildOptions
        (
            int? minTokens = null,
            int? operatorVertices = null,
            int? conditionalVertices = null,
            int? seed = null,
            int? maxAttempts = null
        )
        {
            var options = new LASGenerationOptions
            {
                MinTokenCount = minTokens,
                OperatorVertexCount = operatorVertices,
                ConditionalVertexCount = conditionalVertices,
                Seed = seed
            };

            if (maxAttempts.HasValue)
                options.MaxAttempts = maxAttempts.Value;

            return options;
        }

        private static bool SafeGenerate(LASGenerationOptions options, out string las, out string? genError)
        {
            try
            {
                las = RandomLASGenerator.Generate(options);
                genError = null;
                return true;
            }
            catch (Exception ex)
            {
                las = string.Empty;
                genError = ex.Message;
                return false;
            }
        }

        [TestMethod]
        [DataRow(100, 5, 10000)]
        [DataRow(100, 8, 10000)]
        [DataRow(100, 12, 10000)]
        [DataRow(100, 20, 10000)]
        [DataRow(100, 30, 1000000)]
        [DataRow(100, 40, 1000000)]
        public void E2E_RandomMSAConversion_Equivalence(int lasCount, int minTokens, int maxAttempts)
        {
            var val = CreateValidator();
            var options = BuildOptions(minTokens: minTokens, maxAttempts: maxAttempts);

            for (int i = 0; i < lasCount; i++)
            {
                // 1. Генерируем случайную ЛСА
                Assert.IsTrue(SafeGenerate(options, out var las, out var genErr),
                    $"Генерация не удалась. Ошибка: {genErr}\nОпции: MinTokens = {minTokens}, MaxAttempts = {maxAttempts}");

                // 2. Парсим в АА 1
                Assert.IsTrue(val.TryValidate(las, out var aa1, out var errors1), 
                    $"ЛСА не парсится.\n{DescribeLAS(las)}\nОшибки парсера: {errors1}");
                Assert.IsNotNull(aa1, "Парсер вернул пустую модель для ЛСА 1.");

                // 3. Строим МСА для АА 1 (Legacy mode для полного покрытия)
                var mas1 = BuildLegacyMAS(aa1!);
                Assert.IsNotNull(mas1, "Не удалось построить МСА для АА 1.");

                // 4. Конвертируем МСА -> АА 2
                AbstractAutomaton aa2;
                try
                {
                    aa2 = MSAToAAConverter.Convert(mas1);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Конвертация МСА -> АА 2 упала с ошибкой: {ex.Message}\nЛСА: {las}\nМСА:\n{mas1}");
                    return;
                }

                // 5. Строим МСА для АА 2
                var mas2 = BuildLegacyMAS(aa2);
                Assert.IsNotNull(mas2, "Не удалось построить МСА для АА 2.");

                // 6. Сравниваем МСА 1 и МСА 2
                // Сначала заголовки
                CollectionAssert.AreEqual((System.Collections.ICollection)mas1.Headers, (System.Collections.ICollection)mas2.Headers, 
                    $"Заголовки МСА не совпадают.\nЛСА: {las}");

                // Сравниваем ячейки
                foreach (var row1 in mas1.Rows)
                {
                    string rowHeader = row1.Header;
                    var row2 = mas2[rowHeader];

                    for (int c = 0; c < mas1.Headers.Count; c++)
                    {
                        string cell1 = row1.Transitions[c];
                        string cell2 = row2.Transitions[c];

                        bool equivalent = AreLogicallyEquivalent(cell1, cell2);
                        
                        if (!equivalent)
                        {
                            // Детальный вывод ошибки
                            var msg = new StringBuilder();
                            msg.AppendLine($"Логическое несоответствие в МСА.");
                            msg.AppendLine($"Тест: {i + 1}/{lasCount}, Tokens: {minTokens}");
                            msg.AppendLine($"ЛСА: {las}");
                            msg.AppendLine($"Строка: {rowHeader}, Столбец: {mas1.Headers[c]}");
                            msg.AppendLine($"Ожидалось: {cell1}");
                            msg.AppendLine($"Получено:  {cell2}");
                            
                            Assert.Fail(msg.ToString());
                        }
                    }
                }

                Console.WriteLine($"[Тест {i + 1} / {lasCount}] OK. ЛСА: {las}");
            }
        }

        private AlgorithmDeveloper.AAModel.TransitionSystem.MAS.MatrixAlgorithmSchema BuildLegacyMAS(AbstractAutomaton aa)
        {
            aa.Update(); // Ensure basics are updated
            var formulas = TransitionSystemBuilder.BuildAll(
                aa, 
                TransitionBuildMode.Legacy
            );
            return AlgorithmDeveloper.AAModel.TransitionSystem.MAS.MatrixAlgorithmSchema.FromFormulas(formulas);
        }

        private bool AreLogicallyEquivalent(string expr1, string expr2)
        {
            if (string.IsNullOrWhiteSpace(expr1)) expr1 = "0";
            if (string.IsNullOrWhiteSpace(expr2)) expr2 = "0";

            expr1 = Sanitize(expr1);
            expr2 = Sanitize(expr2);

            try
            {
                var node1 = ExpressionParser.Parse(expr1);
                var node2 = ExpressionParser.Parse(expr2);
                
                var le1 = new LogicalExpression(node1);
                var le2 = new LogicalExpression(node2);

                return le1.EquivalentTo(le2);
            }
            catch
            {
                return false;
            }
        }

        private string Sanitize(string expr)
        {
            return expr.Replace("˄", "&").Replace("˅", "|").Replace("¬", "!");
        }
    }
}
