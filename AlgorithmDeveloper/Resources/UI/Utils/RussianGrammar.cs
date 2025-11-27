using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmDeveloper.AlgorithmModel.Utils
{
    internal static class RussianGrammar
    {
        public static string ChooseNounForm(int count, string one, string few, string many)
        {
            int mod100 = count % 100;
            int mod10 = count % 10;
            if (mod100 >= 11 && mod100 <= 14) return many;
            if (mod10 == 1) return one;
            if (mod10 >= 2 && mod10 <= 4) return few;
            return many;
        }

        public static string ChooseImpersonalFoundVerb(int count)
        {
            int mod100 = count % 100;
            int mod10 = count % 10;
            if (mod100 >= 11 && mod100 <= 14) return "Найдено";
            if (mod10 == 1) return "Найдена";
            return "Найдено";
        }

        public static string BuildFoundHeader(int count, string one, string few, string many, string tail)
        {
            var verb = ChooseImpersonalFoundVerb(count);
            var noun = ChooseNounForm(count, one, few, many);
            return $"{verb} {count} {noun}{tail}";
        }

        public static string FormatUnreachableVerticesMessage(List<string> ids)
        {
            int count = ids?.Count ?? 0;
            if (count == 0) return "Недостижимые вершины не обнаружены";
            if (count == 1) return $"Недостижимая вершина: '{ids[0]}'";
            var quoted = ids.Select(id => $"'{id}'");
            return $"Недостижимые вершины: [{string.Join(", ", quoted)}]";
        }
    }
}
