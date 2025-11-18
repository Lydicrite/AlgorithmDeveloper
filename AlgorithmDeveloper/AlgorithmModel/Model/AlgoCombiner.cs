using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AlgorithmDeveloper.AlgorithmModel.TransitionSystem.MAS.MatrixAlgorithmSchema;

namespace AlgorithmDeveloper.AlgorithmModel.Model
{
    public class AlgoCombiner
    {
        /// <summary>
        /// Список двоичных строк, которыми кодируются объединяемые <see cref="AlgoModel"/>.
        /// </summary>
        private static List<string> _binaryCodes = new List<string>();
        /// <summary>
        /// Список значений (ConditionalVertex.Value) новых P-переменных, которыми кодируются объединяемые <see cref="AlgoModel"/>.
        /// </summary>
        private static List<string> _pVarCodes = new List<string>();
        /// <summary>
        /// Список ID новых условных вершин, добавленных в объединённый <see cref="AlgoModel"/>. ID этих вершин начинается с 'P'.
        /// </summary>
        private static HashSet<string> _newPVertices = new HashSet<string>();



        /// <summary>
        /// Объединяет переданные <see cref="AlgoModel"/> в единый объединённый <see cref="AlgoModel"/>.
        /// </summary>
        /// <param name="algoModels">Список объединяемых <see cref="AlgoModel"/>.</param>
        /// <param name="binaryCodes">Список двоичных кодов, используемых для кодирования объединяемых <see cref="AlgoModel"/>.</param>
        /// <param name="varCodes">Список из значений кодирующих условных вершин, используемых для кодирования объединяемых <see cref="AlgoModel"/>.</param>
        /// <param name="newVertices">Список ID новых условных вершин, добавленных в объединённый <see cref="AlgoModel"/>.</param>
        /// <returns>Объединённая МСА.</returns>
        public static AlgoModel CombineAlgorithms
        (
            in List<AlgoModel> algoModels, ref List<string> binaryCodes,
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
        /// Оптимизирует порядок объединяемые <see cref="AlgoModel"/> для минимизации расстояний (для генерации кодов Грея) на основе их матрицы схожести.
        /// </summary>
        /// <param name="algoModels">Список объединяемых <see cref="AlgoModel"/>.</param>
        /// <param name="similarityMatrix">Матрица смежности <see cref="AlgoModel"/>.</param>
        /// <returns>Список <see cref="AlgoModel"/> в оптимальном порядке.</returns>
        private static List<AlgoModel> OptimizeOrder(List<AlgoModel> algoModels, int[,] similarityMatrix)
        {
            // Жадный алгоритм построения порядка
            var ordered = new List<AlgoModel> { algoModels[0] };
            var remaining = new List<AlgoModel>(algoModels.Skip(1));

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
        /// Возвращает словарь схожести <see cref="AlgoModel"/> в формате [ключ: "B(AA{i}, AA{j})", значение: "количество равных значимых элементов"].
        /// </summary>
        /// <param name="algoModels">Список объединяемых <see cref="AlgoModel"/>.</param>
        /// <returns>Словарь схожести <see cref="AlgoModel"/>.</returns>
        public static Dictionary<string, int> GetSimilarityDictionary(List<AlgoModel> algoModels)
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
        /// Вычисляет матрицу схожести (попарную) для всех участвующих в объединении <see cref="AlgoModel"/>.
        /// </summary>
        /// <param name="algoModels">Список объединяемых <see cref="AlgoModel"/>.</param>
        /// <returns>Попарная матрица схожести <see cref="AlgoModel"/>.</returns>
        private static int[,] CalculateSimilarityMatrix(List<AlgoModel> algoModels)
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
        /// Генерирует коды Грея для <paramref name="count"/> объединяемых <see cref="AlgoModel"/>.
        /// </summary>
        /// <param name="count">Количество объединяемых <see cref="AlgoModel"/>.</param>
        /// <param name="bits">Количество условных P-вершин, используемых для кодирования.</param>
        /// <returns>Список строк с кодами Грея для кодирования объединяемых <see cref="AlgoModel"/>.</returns>
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