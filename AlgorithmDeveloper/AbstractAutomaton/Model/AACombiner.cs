using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AlgorithmDeveloper.AlgorithmModel.TransitionSystem.MAS.MatrixAlgorithmSchema;

namespace AlgorithmDeveloper.AlgorithmModel.Model
{
    public class AACombiner
    {
        /// <summary>
        /// Список двоичных строк, которыми кодируются объединяемые <see cref="AbstractAutomaton"/>.
        /// </summary>
        private static List<string> _binaryCodes = new List<string>();
        /// <summary>
        /// Список значений (ConditionalVertex.Value) новых P-переменных, которыми кодируются объединяемые <see cref="AbstractAutomaton"/>.
        /// </summary>
        private static List<string> _pVarCodes = new List<string>();
        /// <summary>
        /// Список ID новых условных вершин, добавленных в объединённый <see cref="AbstractAutomaton"/>. ID этих вершин начинается с 'P'.
        /// </summary>
        private static HashSet<string> _newPVertices = new HashSet<string>();



        /// <summary>
        /// Объединяет переданные <see cref="AbstractAutomaton"/> в единый объединённый <see cref="AbstractAutomaton"/>.
        /// </summary>
        /// <param name="algoModels">Список объединяемых <see cref="AbstractAutomaton"/>.</param>
        /// <param name="binaryCodes">Список двоичных кодов, используемых для кодирования объединяемых <see cref="AbstractAutomaton"/>.</param>
        /// <param name="varCodes">Список из значений кодирующих условных вершин, используемых для кодирования объединяемых <see cref="AbstractAutomaton"/>.</param>
        /// <param name="newVertices">Список ID новых условных вершин, добавленных в объединённый <see cref="AbstractAutomaton"/>.</param>
        /// <returns>Объединённая МСА.</returns>
        public static AbstractAutomaton CombineAlgorithms
        (
            in List<AbstractAutomaton> algoModels, ref List<string> binaryCodes,
            ref List<string> varCodes, ref HashSet<string> newVertices
        )
        {
            _newPVertices.Clear();
            _binaryCodes.Clear();
            _pVarCodes.Clear();

            // Шаг 1: Вычисление попарной схожести
            var similarityMatrix = CalculateSimilarityMatrix(algoModels);

            // Шаг 2: Оптимизация порядка алгоритмов
            var orderedAlgos = OptimizeOrder(algoModels, similarityMatrix);

            // Шаг 3: Генерация кодов с минимальным расстоянием
            int n = (int)Math.Ceiling(Math.Log(algoModels.Count, 2));
            GenerateGrayCodes(orderedAlgos.Count, n);

            /// TODO
            return null;
        }





        #region Подготовка к объединению

        /// <summary>
        /// Оптимизирует порядок объединяемые <see cref="AbstractAutomaton"/> для минимизации расстояний (для генерации кодов Грея) на основе их матрицы схожести.
        /// </summary>
        /// <param name="algoModels">Список объединяемых <see cref="AbstractAutomaton"/>.</param>
        /// <param name="similarityMatrix">Матрица смежности <see cref="AbstractAutomaton"/>.</param>
        /// <returns>Список <see cref="AbstractAutomaton"/> в оптимальном порядке.</returns>
        private static List<AbstractAutomaton> OptimizeOrder(List<AbstractAutomaton> algoModels, int[,] similarityMatrix)
        {
            // Жадный алгоритм построения порядка
            var ordered = new List<AbstractAutomaton> { algoModels[0] };
            var remaining = new List<AbstractAutomaton>(algoModels.Skip(1));

            while (remaining.Count > 0)
            {
                int lastIndex = algoModels.IndexOf(ordered.Last());
                var next = remaining
                    .OrderByDescending(m => similarityMatrix[
                        lastIndex,
                        algoModels.IndexOf(m)
                    ])
                    .First();

                ordered.Add(next);
                remaining.Remove(next);
            }

            return ordered;
        }

        /// <summary>
        /// Возвращает словарь схожести <see cref="AbstractAutomaton"/> в формате [ключ: "B(AA{i}, AA{j})", значение: "количество равных значимых элементов"].
        /// </summary>
        /// <param name="algoModels">Список объединяемых <see cref="AbstractAutomaton"/>.</param>
        /// <returns>Словарь схожести <see cref="AbstractAutomaton"/>.</returns>
        public static Dictionary<string, int> GetSimilarityDictionary(List<AbstractAutomaton> algoModels)
        {
            var similarityDict = new Dictionary<string, int>();

            for (int i = 0; i < algoModels.Count; i++)
            {
                for (int j = i + 1; j < algoModels.Count; j++)
                {
                    int similarity = algoModels[i]!.MAS!.CalculateSimilarity(algoModels[j]!.MAS!);
                    string key = $"B(M{i + 1}, M{j + 1})";
                    similarityDict.Add(key, similarity);
                }
            }

            return similarityDict;
        }

        /// <summary>
        /// Вычисляет матрицу схожести (попарную) для всех участвующих в объединении <see cref="AbstractAutomaton"/>.
        /// </summary>
        /// <param name="algoModels">Список объединяемых <see cref="AbstractAutomaton"/>.</param>
        /// <returns>Попарная матрица схожести <see cref="AbstractAutomaton"/>.</returns>
        private static int[,] CalculateSimilarityMatrix(List<AbstractAutomaton> algoModels)
        {
            int size = algoModels.Count;
            var matrix = new int[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (i != j)
                        matrix[i, j] = algoModels[i]!.MAS!.CalculateSimilarity(algoModels[j]!.MAS!);

            return matrix;
        }

        /// <summary>
        /// Генерирует коды Грея для <paramref name="count"/> объединяемых <see cref="AbstractAutomaton"/>.
        /// </summary>
        /// <param name="count">Количество объединяемых <see cref="AbstractAutomaton"/>.</param>
        /// <param name="bits">Количество условных P-вершин, используемых для кодирования.</param>
        /// <returns>Список строк с кодами Грея для кодирования объединяемых <see cref="AbstractAutomaton"/>.</returns>
        private static void GenerateGrayCodes(int count, int bits)
        {
            var codes = new List<string>();
            for (int i = 0; i < count; i++)
            {
                int grayCode = i ^ (i >> 1);
                codes.Add(Convert.ToString(grayCode, 2)
                    .PadLeft(bits, '0')
                    .Substring(0, bits));
            }
            _binaryCodes = codes;
        }

        #endregion
    }
}