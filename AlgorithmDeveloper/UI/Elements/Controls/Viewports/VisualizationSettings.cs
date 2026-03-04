using System.Drawing;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph.Layout;

namespace AlgorithmDeveloper.UI.Elements.Controls.Viewports
{
    public static class  VisualizationSettings
    {
        // Sugiyama Layout Settings
        public static LayeringStrategy LayoutLayeringStrategy { get; set; } = LayeringStrategy.ShortestPath;
        public static bool LayoutNormalizeComponents { get; set; } = true;

        public static float FigureHalfWidth { get; set; } = 50f;
        public static float FigureHalfHeight { get; set; } = 25f;
        public static int FigureHalfWidthRounded => (int)System.Math.Round(FigureHalfWidth);
        public static int FigureHalfHeightRounded => (int)System.Math.Round(FigureHalfHeight);
        public static float CanvasPadding { get; set; } = 40f;
        public static float LinkIndent { get; set; } = 6f;
        public static int FontSize { get; set; } = 18;
        public static int VisualizationDelay { get; set; } = 750;

        public static Color FigureStrokeColor { get; set; } = Color.Black;
        public static Color FigureStrokeActiveColor { get; set; } = Color.Orange;
        public static Color FigureFillColor { get; set; } = Color.White;
        public static Color FigureFillActiveColor { get; set; } = Color.LightGray;
        public static Color FigureTextColor { get; set; } = Color.Black;
        public static float FigureStrokeWidth { get; set; } = 2f;

        public static Color EdgeActiveColor { get; set; } = Color.LimeGreen;
        public static float EdgeActiveWidth { get; set; } = 3f;
        public static Color EdgeInactiveColor { get; set; } = Color.Black;
        public static float EdgeInactiveWidth { get; set; } = 2f;

        public static Color ContainerColor { get; set; } = Color.FromArgb(31, 31, 31);

        public static int HorizontalSpacing { get; set; } = 140;
        public static int VerticalSpacing { get; set; } = 120;
        public static int StartX { get; set; } = 100;
        public static int StartY { get; set; } = 100;

        public static string FigureStrokeColorHex => ToHex(FigureStrokeColor);
        public static string FigureStrokeActiveColorHex => ToHex(FigureStrokeActiveColor);
        public static string FigureFillColorHex  => ToHex(FigureFillColor);
        public static string FigureFillActiveColorHex => ToHex(FigureFillActiveColor);
        public static string FigureTextColorHex  => ToHex(FigureTextColor);
        public static string EdgeActiveColorHex  => ToHex(EdgeActiveColor);
        public static string EdgeInactiveColorHex => ToHex(EdgeInactiveColor);

        private static string ToHex(Color c) => "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        #region Persistence

        public class SettingsData
        {
            public LayeringStrategy LayoutLayeringStrategy { get; set; }
            public bool LayoutNormalizeComponents { get; set; }
            public float FigureHalfWidth { get; set; }
            public float FigureHalfHeight { get; set; }
            public float LinkIndent { get; set; }
            public int VisualizationDelay { get; set; }
            
            public int FigureStrokeColor { get; set; }
            public int FigureStrokeActiveColor { get; set; }
            public int FigureFillColor { get; set; }
            public int FigureFillActiveColor { get; set; }
            public float FigureStrokeWidth { get; set; }
            
            public int EdgeActiveColor { get; set; }
            public float EdgeActiveWidth { get; set; }
            public int EdgeInactiveColor { get; set; }
            public float EdgeInactiveWidth { get; set; }
            
            public int ContainerColor { get; set; }
            
            public int HorizontalSpacing { get; set; }
            public int VerticalSpacing { get; set; }
            public int StartX { get; set; }
            public int StartY { get; set; }
        }

        public static void Save(string filePath)
        {
            var data = new SettingsData
            {
                LayoutLayeringStrategy = LayoutLayeringStrategy,
                LayoutNormalizeComponents = LayoutNormalizeComponents,
                FigureHalfWidth = FigureHalfWidth,
                FigureHalfHeight = FigureHalfHeight,
                LinkIndent = LinkIndent,
                VisualizationDelay = VisualizationDelay,
                FigureStrokeColor = FigureStrokeColor.ToArgb(),
                FigureStrokeActiveColor = FigureStrokeActiveColor.ToArgb(),
                FigureFillColor = FigureFillColor.ToArgb(),
                FigureFillActiveColor = FigureFillActiveColor.ToArgb(),
                FigureStrokeWidth = FigureStrokeWidth,
                EdgeActiveColor = EdgeActiveColor.ToArgb(),
                EdgeActiveWidth = EdgeActiveWidth,
                EdgeInactiveColor = EdgeInactiveColor.ToArgb(),
                EdgeInactiveWidth = EdgeInactiveWidth,
                ContainerColor = ContainerColor.ToArgb(),
                HorizontalSpacing = HorizontalSpacing,
                VerticalSpacing = VerticalSpacing,
                StartX = StartX,
                StartY = StartY
            };

            var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, json);
        }

        public static void Load(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) return;

            try
            {
                var json = System.IO.File.ReadAllText(filePath);
                var data = System.Text.Json.JsonSerializer.Deserialize<SettingsData>(json);

                if (data != null)
                {
                    LayoutLayeringStrategy = data.LayoutLayeringStrategy;
                    LayoutNormalizeComponents = data.LayoutNormalizeComponents;
                    FigureHalfWidth = data.FigureHalfWidth;
                    FigureHalfHeight = data.FigureHalfHeight;
                    LinkIndent = data.LinkIndent;
                    VisualizationDelay = data.VisualizationDelay;
                    FigureStrokeColor = Color.FromArgb(data.FigureStrokeColor);
                    FigureStrokeActiveColor = Color.FromArgb(data.FigureStrokeActiveColor);
                    FigureFillColor = Color.FromArgb(data.FigureFillColor);
                    FigureFillActiveColor = Color.FromArgb(data.FigureFillActiveColor);
                    FigureStrokeWidth = data.FigureStrokeWidth;
                    EdgeActiveColor = Color.FromArgb(data.EdgeActiveColor);
                    EdgeActiveWidth = data.EdgeActiveWidth;
                    EdgeInactiveColor = Color.FromArgb(data.EdgeInactiveColor);
                    EdgeInactiveWidth = data.EdgeInactiveWidth;
                    ContainerColor = Color.FromArgb(data.ContainerColor);
                    HorizontalSpacing = data.HorizontalSpacing;
                    VerticalSpacing = data.VerticalSpacing;
                    StartX = data.StartX;
                    StartY = data.StartY;
                }
            }
            catch { }
        }

        #endregion
    }
}
