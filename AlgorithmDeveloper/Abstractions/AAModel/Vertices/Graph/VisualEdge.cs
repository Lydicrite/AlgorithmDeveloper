using System.Drawing;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;

namespace AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph
{
    public enum EdgeType
    {
        Direct,     // Прямая связь (Next)
        TrueBranch, // Ветвь "Да" (RBS)
        FalseBranch,// Ветвь "Нет" (LBS)
        BackLoop    // Обратная связь
    }

    public class VisualEdge
    {
        public IBDVertex Source { get; }
        public IBDVertex Target { get; }
        public EdgeType Type { get; }

        /// <summary>
        /// Точки, образующие ломаную линию (ортогональную).
        /// </summary>
        public List<Point> Points { get; set; } = new();

        /// <summary>
        /// Активна ли связь (для визуализации прохода).
        /// </summary>
        public bool IsActive { get; set; }

        public VisualEdge(IBDVertex source, IBDVertex target, EdgeType type)
        {
            Source = source;
            Target = target;
            Type = type;
        }
    }
}
