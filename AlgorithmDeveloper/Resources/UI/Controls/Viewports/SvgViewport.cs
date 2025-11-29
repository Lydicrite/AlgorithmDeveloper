using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using AlgorithmDeveloper.AlgorithmModel;
using AlgorithmDeveloper.AlgoDev.Model.Vertices;
using AlgorithmDeveloper.AlgorithmModel.Model.Vertices.Vizualization;

namespace AlgorithmDeveloper.Resources.UI.Controls.Viewports
{
    public partial class SvgViewport : UserControl
    {
        private AbstractAutomaton? _model;
        private IEnumerable<IFigure>? _figures;
        private string? _pendingHtml;

        public SvgViewport()
        {
            InitializeComponent();
            BackColor = Color.FromArgb(48, 48, 48);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AbstractAutomaton? Model
        {
            get => _model;
            set
            {
                _model = value;
                Render();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IFigure>? Figures
        {
            get => _figures;
            set
            {
                _figures = value;
                Render();
            }
        }

        protected override async void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (_webView.CoreWebView2 == null)
            {
                try
                {
                    await _webView.EnsureCoreWebView2Async();
                }
                catch { }
            }
            if (_pendingHtml != null && _webView.CoreWebView2 != null)
            {
                _webView.NavigateToString(_pendingHtml);
                _pendingHtml = null;
            }
        }

        public void Render()
        {
            if (_figures == null || !_figures.Any())
            {
                NavigateHtml(BuildEmptyHtml());
                return;
            }

            var svg = BuildSvg();
            var html = WrapHtml(svg);
            NavigateHtml(html);
        }

        private void NavigateHtml(string html)
        {
            if (_webView.CoreWebView2 != null)
            {
                _webView.NavigateToString(html);
            }
            else
            {
                _pendingHtml = html;
            }
        }

        private string BuildEmptyHtml()
        {
            var bg = VisualizationSettings.ContainerColor;
            return $"<html><head><meta charset='utf-8'/><style>html,body{{margin:0; background:#{bg.R:X2}{bg.G:X2}{bg.B:X2};}}</style></head><body><div style='color:#ccc;font-family:sans-serif;display:flex;align-items:center;justify-content:center;height:100%;'>NO CONTENT</div></body></html>";
        }

        private string WrapHtml(string svg)
        {
            var sb = new StringBuilder();
            var bg = VisualizationSettings.ContainerColor;
            sb.Append("<html><head><meta charset='utf-8'/>");
            sb.Append("<style>html,body{margin:0;height:100%;background:#" + bg.R.ToString("X2") + bg.G.ToString("X2") + bg.B.ToString("X2") + ";} #root{width:100%;height:100%;overflow:auto;} svg{width:100%;height:100%;}</style>");
            sb.Append("</head><body><div id='root'>");
            sb.Append(svg);
            sb.Append("</div></body></html>");
            return sb.ToString();
        }

        private string BuildSvg()
        {
            float hw = VisualizationSettings.FigureHalfWidth;
            float hh = VisualizationSettings.FigureHalfHeight;
            float pad = VisualizationSettings.CanvasPadding;

            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            foreach (var f in _figures!)
            {
                var c = f.Center;
                minX = Math.Min(minX, c.X - hw);
                minY = Math.Min(minY, c.Y - hh);
                maxX = Math.Max(maxX, c.X + hw);
                maxY = Math.Max(maxY, c.Y + hh);
            }
            if (float.IsInfinity(minX) || float.IsInfinity(minY))
            {
                minX = minY = 0; maxX = 100; maxY = 100;
            }

            float vbX = minX - pad;
            float vbY = minY - pad;
            float vbW = (maxX - minX) + pad * 2f;
            float vbH = (maxY - minY) + pad * 2f;

            var sb = new StringBuilder();
            sb.Append($"<svg xmlns='http://www.w3.org/2000/svg' viewBox='{vbX} {vbY} {vbW} {vbH}'>");
            sb.Append("<defs>");
            sb.Append("<marker id='arrow' markerWidth='10' markerHeight='10' refX='10' refY='5' orient='auto' viewBox='0 0 10 10'><path d='M0,0 L10,5 L0,10 Z' fill='" + VisualizationSettings.EdgeActiveColorHex + "'/></marker>");
            sb.Append("</defs>");

            foreach (var f in _figures!)
            {
                AppendFigure(sb, f);
            }

            if (_model != null)
            {
                foreach (var parent in _model.Vertices.Where(v => v is IGraphFigure && v is not JumpPoint))
                {
                    var pe = parent as IGraphElement;
                    if (pe == null) continue;
                    foreach (var rawChild in pe.Children)
                    {
                        var child = SkipJump(rawChild);
                        if (child is IGraphFigure)
                        {
                            AppendEdge(sb, (IFigure)parent, (IFigure)child);
                        }
                    }
                }
            }

            sb.Append("</svg>");
            return sb.ToString();
        }

        private static IBDVertex? SkipJump(IBDVertex? v)
        {
            while (v is JumpPoint jp)
                v = jp.GetNext(null);
            return v;
        }

        private void AppendFigure(StringBuilder sb, IFigure f)
        {
            var c = f.Center;
            float hw = VisualizationSettings.FigureHalfWidth;
            float hh = VisualizationSettings.FigureHalfHeight;
            var stroke = VisualizationSettings.FigureStrokeColorHex;
            var fill = VisualizationSettings.FigureFillColorHex;
            var text = VisualizationSettings.FigureTextColorHex;
            float sw = VisualizationSettings.FigureStrokeWidth;

            switch (f.GridGeometry.Shape)
            {
                case FigureShape.Ellipse:
                    sb.Append($"<ellipse cx='{c.X}' cy='{c.Y}' rx='{hw}' ry='{hh}' fill='{fill}' stroke='{stroke}' stroke-width='{sw}'/>");
                    AppendLabel(sb, f, c.X, c.Y);
                    break;
                case FigureShape.Rectangle:
                    sb.Append($"<rect x='{c.X - hw}' y='{c.Y - hh}' width='{2 * hw}' height='{2 * hh}' fill='{fill}' stroke='{stroke}' stroke-width='{sw}' rx='6' ry='6'/>");
                    AppendLabel(sb, f, c.X, c.Y);
                    break;
                case FigureShape.Diamond:
                    sb.Append("<polygon points='" +
                        string.Join(" ", new[] {
                            new PointF(c.X, c.Y - hh),
                            new PointF(c.X + hw, c.Y),
                            new PointF(c.X, c.Y + hh),
                            new PointF(c.X - hw, c.Y)
                        }.Select(p => $"{p.X},{p.Y}")) +
                        $"' fill='{fill}' stroke='{stroke}' stroke-width='{sw}'/>");
                    AppendLabel(sb, f, c.X, c.Y);
                    break;
                case FigureShape.Circle:
                    float r = Math.Min(hw * 0.5f, hh * 0.5f);
                    sb.Append($"<circle cx='{c.X}' cy='{c.Y}' r='{r}' fill='{fill}' stroke='{stroke}' stroke-width='{sw}'/>");
                    AppendSmallLabel(sb, f, c.X + r + 6, c.Y - r);
                    break;
            }
        }

        private void AppendLabel(StringBuilder sb, IFigure f, float x, float y)
        {
            string id = GetFigureId(f);
            if (string.IsNullOrEmpty(id)) return;
            sb.Append($"<text x='{x}' y='{y}' text-anchor='middle' dominant-baseline='middle' fill='{VisualizationSettings.FigureTextColorHex}' font-family='Segoe UI, Arial' font-size='{VisualizationSettings.FontSize}'>" + Escape(id) + "</text>");
        }

        private void AppendSmallLabel(StringBuilder sb, IFigure f, float x, float y)
        {
            string id = GetFigureId(f);
            if (string.IsNullOrEmpty(id)) return;
            sb.Append($"<text x='{x}' y='{y}' fill='{VisualizationSettings.FigureTextColorHex}' font-family='Segoe UI, Arial' font-size='{Math.Max(10, VisualizationSettings.FontSize - 6)}'>" + Escape(id) + "</text>");
        }

        private static string Escape(string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private string GetFigureId(IFigure f)
        {
            if (f is IBDVertex v)
                return v.ID ?? string.Empty;
            return string.Empty;
        }

        private void AppendEdge(StringBuilder sb, IFigure from, IFigure to)
        {
            var fc = from.Center;
            var tc = to.Center;
            float hw = VisualizationSettings.FigureHalfWidth;
            float hh = VisualizationSettings.FigureHalfHeight;
            float indent = VisualizationSettings.LinkIndent;
            float midY = (fc.Y + hh + tc.Y - hh) / 2f;

            float sx = fc.X;
            float sy = fc.Y + hh + indent;
            float ex = tc.X;
            float ey = tc.Y - hh - indent;

            int lane = (int)(Math.Abs(fc.X - ex) / Math.Max(1, VisualizationSettings.HorizontalSpacing));
            float laneOffset = Math.Min(VisualizationSettings.VerticalSpacing / 3f, 6f * lane);
            float hy = midY + laneOffset;

            var color = VisualizationSettings.EdgeActiveColorHex;
            float sw = VisualizationSettings.EdgeActiveWidth;

            sb.Append("<polyline points='" +
                      $"{sx},{sy} {sx},{hy} {ex},{hy} {ex},{ey}" +
                      $"' fill='none' stroke='{color}' stroke-width='{sw}' marker-end='url(#arrow)'/>");
        }
    }
}

