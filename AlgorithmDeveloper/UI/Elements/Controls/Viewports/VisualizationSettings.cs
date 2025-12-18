using System.Drawing;

namespace AlgorithmDeveloper.UI.Elements.Controls.Viewports
{
    public static class VisualizationSettings
    {
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

        public static int HorizontalSpacing { get; set; } = 150;
        public static int VerticalSpacing { get; set; } = 100;
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
    }
}
