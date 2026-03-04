using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Geometry;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgorithmDeveloper.UI.Elements.Controls.Viewports
{
    public partial class ImageViewport : UserControl
    {
        private Image? _image;
        private IEnumerable<IFigure>? _figures;
        private IEnumerable<VisualEdge>? _edges;
        private float _zoom = 1f;                       // масштаб: 1 = 100%
        private PointF _offset = new PointF(0, 0);      // сдвиг панорамирования в пикселях устройства
        private bool _panning;
        private Point _lastMouse;
        private bool _spaceDown;
        private readonly object _renderLock = new object();

        // Cached Graphics Resources
        private Pen? _cachedNormalPen;
        private Pen? _cachedActivePen;
        private Pen? _cachedEdgeInactivePen;
        private Pen? _cachedEdgeActivePen;
        private AdjustableArrowCap? _cachedArrowCap;
        private SolidBrush? _cachedNormalBrush;
        private SolidBrush? _cachedActiveBrush;
        private SolidBrush? _cachedTextBrush;

        private bool _updatingScrollBars = false;

        // Дебаунс для события метки
        private System.Windows.Forms.Timer _debounceTimer;
        private Point _lastMouseMovePoint;
        private int _debounceMs = 15;

        // Жест масштабирования Ctrl + Shift + ПКМ
        private bool _scalingGestureActive;
        private Point _scaleLastMouse;
        private PointF _scaleAnchorImg;

        private bool IsInDesignMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime || (Site?.DesignMode ?? DesignMode);

        private bool _requiresFitToContent = false;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && _requiresFitToContent)
            {
                // Используем BeginInvoke, чтобы дать время на отработку layout (Resize),
                // если контрол только что стал видимым и меняет размер (например, в TabControl).
                if (IsHandleCreated)
                {
                    BeginInvoke(new Action(() =>
                    {
                        FitToContent();
                        _requiresFitToContent = false;
                    }));
                }
                else
                {
                    _requiresFitToContent = false;
                }
            }
        }

        public ImageViewport()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            BackColor = Color.DimGray;
            TabStop = true;
            InitializeScrollBars();
            InitializeContextMenu();

            _debounceTimer = new System.Windows.Forms.Timer();
            _debounceTimer.Interval = _debounceMs;
            _debounceTimer.Tick += DebounceTimer_Tick;
        }

        private void DebounceTimer_Tick(object? sender, EventArgs e)
        {
            _debounceTimer.Stop();
            if (_image == null) return;

            var clientPt = _lastMouseMovePoint;
            var imgPtF = ClientToImageF(clientPt);
            var clamped = ClampToImage(imgPtF);
            var rounded = new Point((int)Math.Round(clamped.X), (int)Math.Round(clamped.Y));

            OnMarkChanged(rounded);
        }

        private void InitializeContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            var fitToContentItem = new ToolStripMenuItem("Вписать в окно");
            fitToContentItem.Click += (s, e) => FitToContent();

            var helpItem = new ToolStripMenuItem("Справка по управлению");
            helpItem.Click += (s, e) => ShowHelpMessageBox();

            contextMenu.Items.Add(fitToContentItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(helpItem);

            this.ContextMenuStrip = contextMenu;
        }

        private void ShowHelpMessageBox()
        {
            var helpText = new StringBuilder();
            helpText.AppendLine("Жесты управления:");
            helpText.AppendLine("\n1) Панорамирование: ('Пробел' + ЛКМ + движение мыши) или (СКМ + движение мыши).\n");
            helpText.AppendLine("\n2) Масштабирование: ('Ctrl' + колесо мыши);" +
                                "\n\tвращение вверх - увеличение," +
                                "\n\tвращение вниз - уменьшение.\n");
            helpText.AppendLine("\n3) Быстрое масштабирование: ('Ctrl' + 'Shift' + ПКМ + движение мыши);" +
                                "\n\tдвижение вверх - увеличение," +
                                "\n\tдвижение вниз - уменьшение.\n");
            helpText.AppendLine("\n4) Увеличение: двойной клик ЛКМ или клавиша '+'.\n");
            helpText.AppendLine("\n5) Уменьшение: клавиша '-'.");
            
            MessageBox.Show(helpText.ToString(), "Справка по управлению", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Инициализация скроллбаров.
        /// </summary>
        private void InitializeScrollBars()
        {
            _hScrollBar = new HScrollBar
            {
                Dock = DockStyle.Bottom,
                Visible = false
            };
            _hScrollBar.ValueChanged += HScrollBar_ValueChanged;

            _vScrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                Visible = false
            };
            _vScrollBar.ValueChanged += VScrollBar_ValueChanged;

            Controls.Add(_hScrollBar);
            Controls.Add(_vScrollBar);
        }





        #region Публичные войства

        [Category("Масштабирование изображения"), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(0.00525f)]
        [Description("Скорость изменения масштаба при жесте масштабирования \"Ctrl + Shift + ПКМ + Движение мыши\".")]
        public float ScaleGestureSensitivity { get; set; } = 0.00525f;

        /// <summary>
        /// Изображение для просмотра.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image? Image
        {
            get => _image;
            set
            {
                if (_image == value) return;
                _image = value;
                _requiresFitToContent = !Visible;
                ResetView();
                UpdateScrollBars();
                Invalidate();
            }
        }

        /// <summary>
        /// Текущий множитель масштаба (ограничен в пределах от 1% до 25600%).
        /// </summary>
        [Category("Масштабирование изображения"), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(1.0f)]
        [Description("Текущий масштаб отображания.")]
        public float Zoom
        {
            get => _zoom;
            set
            {
                var clamped = Math.Max(0.01f, Math.Min(256f, value));
                if (Math.Abs(clamped - _zoom) > float.Epsilon)
                {
                    _zoom = clamped;
                    UpdateScrollBars();
                    Invalidate();
                    OnZoomChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Смещение (точка, где будет находиться центр изображения при панорамировании).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PointF Offset
        {
            get => _offset;
            private set
            {
                if (_offset != value)
                {
                    SetOffsetClamped(value);
                }
            }
        }

        public bool ShouldSerializeImage() => false;

        public void ResetImage() => Image = null;

        /// <summary>
        /// Коллекция векторных фигур для отрисовки.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IFigure>? Figures
        {
            get => _figures;
            set
            {
                if (_figures != value)
                {
                    _figures = value;
                    _requiresFitToContent = !Visible;
                    FitToContent();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Коллекция связей (ребер) для отрисовки.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<VisualEdge>? Edges
        {
            get => _edges;
            set
            {
                if (_edges != value)
                {
                    _edges = value;
                    Invalidate();
                }
            }
        }

        #endregion





        #region Векторная отрисовка фигур

        /// <summary>
        /// Отрисовывает все связи (ребра).
        /// </summary>
        private void DrawEdges(Graphics g)
        {
            EnsureGraphicsResources();

            if (_edges == null) return;

            // Сначала рисуем неактивные ребра, чтобы они были на заднем плане
            foreach (var edge in _edges)
            {
                if (edge.IsActive) continue;
                if (edge.Points == null || edge.Points.Count < 2) continue;

                g.DrawLines(_cachedEdgeInactivePen!, edge.Points.ToArray());
            }

            // Затем рисуем активные ребра поверх неактивных
            foreach (var edge in _edges)
            {
                if (!edge.IsActive) continue;
                if (edge.Points == null || edge.Points.Count < 2) continue;

                g.DrawLines(_cachedEdgeActivePen!, edge.Points.ToArray());
            }
        }

        /// <summary>
        /// Отрисовывает все векторные фигуры.
        /// </summary>
        private void DrawVectorFigures(Graphics g)
        {
            EnsureGraphicsResources();

            using var centeredFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoClip
            };

            foreach (var figure in _figures!)
            {
                var pen = figure.IsActive ? _cachedActivePen! : _cachedNormalPen!;
                var fillBrush = figure.IsActive ? _cachedActiveBrush! : _cachedNormalBrush!;
                DrawFigure(g, figure, pen, fillBrush, _cachedTextBrush!, centeredFormat);
            }

            // Отрисовка меток ветвей (0 и 1)
            DrawBranchLabels(g);
        }

        private void DrawBranchLabels(Graphics g)
        {
            if (_edges == null) return;

            // Настройки шрифта для меток
            float fontSize = VisualizationSettings.FontSize * 0.6f;
            using var font = new Font(_figureFont.FontFamily, fontSize, _figureFont.Style);
            
            // Смещение меток
            float offset = IFigure.HalfHeight * 2 * 0.25f;

            foreach (var edge in _edges)
            {
                string? label = null;
                if (edge.Type == EdgeType.FalseBranch) label = "0";
                else if (edge.Type == EdgeType.TrueBranch) label = "1";

                if (label != null && edge.Points != null && edge.Points.Count > 0)
                {
                    // Точка начала перехода
                    var startPoint = edge.Points[0];
                    var sourceCenter = edge.Source is IFigure fig ? fig.Center : startPoint;
                    
                    // Вычисляем позицию метки в зависимости от направления выхода
                    float x = startPoint.X;
                    float y = startPoint.Y;

                    bool isLeft = startPoint.X < sourceCenter.X - 5;
                    bool isRight = startPoint.X > sourceCenter.X + 5;
                    bool isBottom = !isLeft && !isRight;

                    if (isLeft)
                    {
                        // Выход влево: метка над линией, чуть левее
                        x -= offset * 0.5f;
                        y -= offset * 0.8f;
                    }
                    else if (isRight)
                    {
                        // Выход вправо: метка над линией, чуть правее
                        x += offset * 0.5f;
                        y -= offset * 0.8f;
                    }
                    else // isBottom
                    {
                        // Выход снизу:
                        // Если "0" (False) - слева от линии
                        // Если "1" (True) - справа от линии
                        y += offset * 0.5f;
                        if (edge.Type == EdgeType.FalseBranch) x -= offset * 0.8f;
                        else x += offset * 0.8f;
                    }

                    // Цвет метки совпадает с цветом ребра
                    var color = edge.IsActive ? VisualizationSettings.EdgeActiveColor : VisualizationSettings.EdgeInactiveColor;
                    using var brush = new SolidBrush(color);

                    // Центрируем текст относительно (x, y)
                    var size = g.MeasureString(label, font);
                    g.DrawString(label, font, brush, x - size.Width / 2, y - size.Height / 2);
                }
            }
        }

        private void EnsureGraphicsResources()
        {
            // Normal Pen
            if (_cachedNormalPen == null)
            {
                _cachedNormalPen = new Pen(_figureStrokeColor, _figureStrokeWidth) { Alignment = PenAlignment.Center };
            }
            else
            {
                if (_cachedNormalPen.Color != _figureStrokeColor) _cachedNormalPen.Color = _figureStrokeColor;
                if (Math.Abs(_cachedNormalPen.Width - _figureStrokeWidth) > 0.001f) _cachedNormalPen.Width = _figureStrokeWidth;
            }

            // Active Pen
            if (_cachedActivePen == null)
            {
                _cachedActivePen = new Pen(_figureStrokeActiveColor, _figureStrokeWidth) { Alignment = PenAlignment.Center };
            }
            else
            {
                if (_cachedActivePen.Color != _figureStrokeActiveColor) _cachedActivePen.Color = _figureStrokeActiveColor;
                if (Math.Abs(_cachedActivePen.Width - _figureStrokeWidth) > 0.001f) _cachedActivePen.Width = _figureStrokeWidth;
            }

            // Normal Brush
            if (_cachedNormalBrush == null)
            {
                _cachedNormalBrush = new SolidBrush(_figureFillColor);
            }
            else if (_cachedNormalBrush.Color != _figureFillColor)
            {
                _cachedNormalBrush.Color = _figureFillColor;
            }

            // Active Brush
            if (_cachedActiveBrush == null)
            {
                _cachedActiveBrush = new SolidBrush(_figureFillActiveColor);
            }
            else if (_cachedActiveBrush.Color != _figureFillActiveColor)
            {
                _cachedActiveBrush.Color = _figureFillActiveColor;
            }

            // Text Brush
            if (_cachedTextBrush == null)
            {
                _cachedTextBrush = new SolidBrush(_figureTextColor);
            }
            else if (_cachedTextBrush.Color != _figureTextColor)
            {
                _cachedTextBrush.Color = _figureTextColor;
            }

            // Arrow Cap
            if (_cachedArrowCap == null)
            {
                _cachedArrowCap = new AdjustableArrowCap(4, 4, true);
            }

            // Корректировка размера стрелок при малом масштабе
            // GDI+ может ограничивать минимальную толщину линии в 1px, из-за чего стрелки (рассчитываемые от толщины) 
            // становятся непропорционально большими. Мы уменьшаем их размер, чтобы компенсировать это.
            float nominalEdgeWidth = VisualizationSettings.EdgeInactiveWidth;
            float screenEdgeWidth = nominalEdgeWidth * _zoom;
            float baseArrowSize = 4f; // Базовый размер (множитель толщины)

            float targetArrowSize = baseArrowSize;
            
            // Если толщина линии на экране приближается к 1.35px или меньше,
            // начинаем плавно уменьшать относительный размер стрелки.
            // Порог 1.75 выбран экспериментально,чтобы сгладить переход и сделать стрелки визуально менее массивными при отдалении.
            float threshold = 1.75f;
            if (screenEdgeWidth < threshold && screenEdgeWidth > 0.001f)
            {
                // Линейная интерполяция: чем тоньше линия, тем меньше множитель стрелки.
                targetArrowSize = baseArrowSize * (screenEdgeWidth / threshold);
            }

            if (Math.Abs(_cachedArrowCap.Width - targetArrowSize) > 0.001f)
            {
                _cachedArrowCap.Width = targetArrowSize;
                _cachedArrowCap.Height = targetArrowSize;
            }

            // Edge Inactive Pen
            if (_cachedEdgeInactivePen == null)
            {
                _cachedEdgeInactivePen = new Pen(VisualizationSettings.EdgeInactiveColor, VisualizationSettings.EdgeInactiveWidth);
                _cachedEdgeInactivePen.CustomEndCap = _cachedArrowCap;
            }
            else
            {
                if (_cachedEdgeInactivePen.Color != VisualizationSettings.EdgeInactiveColor) _cachedEdgeInactivePen.Color = VisualizationSettings.EdgeInactiveColor;
                if (Math.Abs(_cachedEdgeInactivePen.Width - VisualizationSettings.EdgeInactiveWidth) > 0.001f) _cachedEdgeInactivePen.Width = VisualizationSettings.EdgeInactiveWidth;
                _cachedEdgeInactivePen.CustomEndCap = _cachedArrowCap;
            }

            // Edge Active Pen
            if (_cachedEdgeActivePen == null)
            {
                _cachedEdgeActivePen = new Pen(VisualizationSettings.EdgeActiveColor, VisualizationSettings.EdgeActiveWidth);
                _cachedEdgeActivePen.CustomEndCap = _cachedArrowCap;
            }
            else
            {
                if (_cachedEdgeActivePen.Color != VisualizationSettings.EdgeActiveColor) _cachedEdgeActivePen.Color = VisualizationSettings.EdgeActiveColor;
                if (Math.Abs(_cachedEdgeActivePen.Width - VisualizationSettings.EdgeActiveWidth) > 0.001f) _cachedEdgeActivePen.Width = VisualizationSettings.EdgeActiveWidth;
                _cachedEdgeActivePen.CustomEndCap = _cachedArrowCap;
            }
        }

        /// <summary>
        /// Отрисовывает одну фигуру в зависимости от её типа.
        /// </summary>
        private void DrawFigure(Graphics g, IFigure figure, Pen pen, Brush fillBrush, Brush textBrush, StringFormat centeredFormat)
        {
            var shape = figure.GridGeometry.Shape;

            switch (shape)
            {
                case FigureShape.Ellipse:
                    DrawEllipseFigure(g, figure, pen, fillBrush, textBrush, centeredFormat);
                    break;
                case FigureShape.Rectangle:
                    DrawRectangleFigure(g, figure, pen, fillBrush, textBrush, centeredFormat);
                    break;
                case FigureShape.Diamond:
                    DrawDiamondFigure(g, figure, pen, fillBrush, textBrush, centeredFormat);
                    break;
                case FigureShape.Circle:
                    DrawCircleFigure(g, figure, pen, fillBrush, textBrush);
                    break;
            }
        }

        /// <summary>
        /// Отрисовка эллипса.
        /// Прямоугольник: x = cx - hw, y = cy - hh, w = 2*hw, h = 2*hh.
        /// </summary>
        private void DrawEllipseFigure(Graphics g, IFigure figure, Pen pen, Brush fillBrush, Brush textBrush, StringFormat centeredFormat)
        {
            var center = figure.Center;
            float hw = IFigure.HalfWidth;
            float hh = IFigure.HalfHeight;

            // Прямоугольник ограничивающий эллипс
            var rect = new RectangleF(center.X - hw, center.Y - hh, 2 * hw, 2 * hh);

            // Заливка и контур
            g.FillEllipse(fillBrush, rect);
            g.DrawEllipse(pen, rect);

            // Текст по центру
            string id = GetFigureId(figure);
            if (!string.IsNullOrEmpty(id))
            {
                g.DrawString(id, _figureFont, textBrush, rect, centeredFormat);
            }
        }

        /// <summary>
        /// Отрисовка прямоугольника.
        /// Прямоугольник: x = cx - hw, y = cy - hh, w = 2*hw, h = 2*hh.
        /// </summary>
        private void DrawRectangleFigure(Graphics g, IFigure figure, Pen pen, Brush fillBrush, Brush textBrush, StringFormat centeredFormat)
        {
            var center = figure.Center;
            float hw = IFigure.HalfWidth;
            float hh = IFigure.HalfHeight;

            var rect = new RectangleF(center.X - hw, center.Y - hh, 2 * hw, 2 * hh);

            // Заливка и контур
            g.FillRectangle(fillBrush, rect);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

            // Текст по центру
            string id = GetFigureId(figure);
            if (!string.IsNullOrEmpty(id))
            {
                g.DrawString(id, _figureFont, textBrush, rect, centeredFormat);
            }
        }

        /// <summary>
        /// Отрисовка ромба.
        /// Точки: top(cx, cy - hh), right(cx + hw, cy), bottom(cx, cy + hh), left(cx - hw, cy).
        /// </summary>
        private void DrawDiamondFigure(Graphics g, IFigure figure, Pen pen, Brush fillBrush, Brush textBrush, StringFormat centeredFormat)
        {
            var center = figure.Center;
            float cx = center.X;
            float cy = center.Y;
            float hw = IFigure.HalfWidth;
            float hh = IFigure.HalfHeight;

            // Четыре точки ромба
            var points = new PointF[]
            {
                new PointF(cx, cy - hh),         // top
                new PointF(cx + hw, cy),         // right
                new PointF(cx, cy + hh),         // bottom
                new PointF(cx - hw, cy)          // left
            };

            // Заливка и контур
            g.FillPolygon(fillBrush, points);
            g.DrawPolygon(pen, points);

            // Текст по центру (в прямоугольнике, ограничивающем ромб)
            string id = GetFigureId(figure);
            if (!string.IsNullOrEmpty(id))
            {
                var textRect = new RectangleF(cx - hw, cy - hh, 2 * hw, 2 * hh);
                g.DrawString(id, _figureFont, textBrush, textRect, centeredFormat);
            }
        }

        /// <summary>
        /// Отрисовка круга (маленькая точка).
        /// Радиус: r = min(HalfWidth * 0.5, HalfHeight * 0.5).
        /// Текст справа сверху от круга.
        /// </summary>
        private void DrawCircleFigure(Graphics g, IFigure figure, Pen pen, Brush fillBrush, Brush textBrush)
        {
            var center = figure.Center;
            float cx = center.X;
            float cy = center.Y;

            // Радиус круга (маленький)
            float r = Math.Min(IFigure.HalfWidth * 0.5f, IFigure.HalfHeight * 0.5f);

            // Прямоугольник круга
            var circleRect = new RectangleF(cx - r, cy - r, 2 * r, 2 * r);

            // Заливка и контур (создаем жирную точку)
            g.FillEllipse(fillBrush, circleRect);
            g.DrawEllipse(pen, circleRect);

            // Текст справа сверху от круга
            string id = GetFigureId(figure);
            if (!string.IsNullOrEmpty(id))
            {
                var labelSize = g.MeasureString(id, _figureFont);
                var labelPoint = new PointF(cx + r + 6, cy - r - labelSize.Height * 0.5f);
                g.DrawString(id, _figureFont, textBrush, labelPoint);
            }
        }

        /// <summary>
        /// Получает идентификатор фигуры для отображения.
        /// </summary>
        private string GetFigureId(IFigure figure)
        {
            // Если фигура также реализует IBDVertex, берем ID оттуда
            if (figure is IBDVertex vertex)
            {
                return vertex.ID ?? string.Empty;
            }
            return string.Empty;
        }

        #endregion





        #region Свойства стиля векторной отрисовки

        private Color _figureStrokeColor = Color.Black;
        private Color _figureFillColor = Color.White;
        private Color _figureStrokeActiveColor = Color.Orange;
        private Color _figureFillActiveColor = Color.LightGray;
        private Color _figureTextColor = Color.Black;
        private float _figureStrokeWidth = 2f;
        private Font _figureFont = SystemFonts.DefaultFont;

        /// <summary>
        /// Цвет контура фигур.
        /// </summary>
        [Category("Векторная отрисовка")]
        [Description("Цвет контура фигур.")]
        [DefaultValue(typeof(Color), "Black")]
        public Color FigureStrokeColor
        {
            get => _figureStrokeColor;
            set
            {
                if (_figureStrokeColor != value)
                {
                    _figureStrokeColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет контура активных фигур.
        /// </summary>
        [Category("Векторная отрисовка")]
        [Description("Цвет контура активных фигур.")]
        [DefaultValue(typeof(Color), "Orange")]
        public Color FigureStrokeActiveColor
        {
            get => _figureStrokeActiveColor;
            set
            {
                if (_figureStrokeActiveColor != value)
                {
                    _figureStrokeActiveColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет заливки фигур.
        /// </summary>
        [Category("Векторная отрисовка")]
        [Description("Цвет заливки фигур.")]
        [DefaultValue(typeof(Color), "White")]
        public Color FigureFillColor
        {
            get => _figureFillColor;
            set
            {
                if (_figureFillColor != value)
                {
                    _figureFillColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет заливки активных фигур.
        /// </summary>
        [Category("Векторная отрисовка")]
        [Description("Цвет заливки активных фигур.")]
        [DefaultValue(typeof(Color), "LightGray")]
        public Color FigureFillActiveColor
        {
            get => _figureFillActiveColor;
            set
            {
                if (_figureFillActiveColor != value)
                {
                    _figureFillActiveColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет текста на фигурах.
        /// </summary>
        [Category("Векторная отрисовка")]
        [Description("Цвет текста на фигурах.")]
        [DefaultValue(typeof(Color), "Black")]
        public Color FigureTextColor
        {
            get => _figureTextColor;
            set
            {
                if (_figureTextColor != value)
                {
                    _figureTextColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Толщина контура фигур.
        /// </summary>
        [Category("Векторная отрисовка")]
        [Description("Толщина контура фигур.")]
        [DefaultValue(2f)]
        public float FigureStrokeWidth
        {
            get => _figureStrokeWidth;
            set
            {
                if (Math.Abs(_figureStrokeWidth - value) > float.Epsilon)
                {
                    _figureStrokeWidth = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Шрифт для текста на фигурах.
        /// </summary>
        [Category("Векторная отрисовка")]
        [Description("Шрифт для текста на фигурах.")]
        [DefaultValue(typeof(Font), "DefaultFont")]
        public Font FigureFont
        {
            get => _figureFont;
            set
            {
                if (_figureFont != value)
                {
                    _figureFont = value ?? SystemFonts.DefaultFont;
                    Invalidate();
                }
            }
        }

        #endregion





        #region События

        /// <summary>
        /// Обработчик события изменения масштаба (может быть null).
        /// </summary>
        public event EventHandler? ZoomChanged;
        protected virtual void OnZoomChanged(EventArgs e) => ZoomChanged?.Invoke(this, e);

        public event EventHandler<Point>? MarkChanged; // координаты в изображении
        protected virtual void OnMarkChanged(Point imgPoint) => MarkChanged?.Invoke(this, imgPoint);

        private void ZoomAround(PointF anchorImg, float newZoom)
        {
            // Масштабирование работает как с изображением, так и с векторными фигурами
            var anchorClientBefore = ImageToClient(anchorImg);
            Zoom = newZoom; // вызовет перерисовку и обновление скроллов
            var anchorClientAfter = ImageToClient(anchorImg);
            var shift = new PointF(anchorClientBefore.X - anchorClientAfter.X, anchorClientBefore.Y - anchorClientAfter.Y);
            Offset = new PointF(_offset.X + shift.X, _offset.Y + shift.Y);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateScrollBars();
            Invalidate(); // не сбрасываем вид при ресайзе, чтобы не терять текущий масштаб/позицию
        }

        /// <summary>
        /// Отрисовка контрола. Применяет трансформации масштабирования и смещения.
        /// Порядок трансформаций: 1) TranslateTransform, 2) ScaleTransform.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // Настройки качества рендеринга
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.Clear(BackColor);

            // Если нет ни изображения, ни фигур - показываем placeholder
            if (_image == null && (_figures == null || !_figures.Any()))
            {
                DrawPlaceholder(g);
                return;
            }

            lock (_renderLock)
            {
                // Сохраняем состояние графики
                var state = g.Save();

                // Применяем трансформации: сначала смещение, затем масштаб
                g.TranslateTransform(_offset.X, _offset.Y);
                g.ScaleTransform(_zoom, _zoom);

                // Рисуем растровое изображение если есть
                if (_image != null)
                {
                    g.DrawImage(_image, Point.Empty);
                }

                // Рисуем векторные фигуры
                if (_figures != null)
                {
                    if (_edges != null)
                        DrawEdges(g);

                    DrawVectorFigures(g);
                }

                // Восстанавливаем состояние графики
                g.Restore(state);
            }
        }

        private void DrawPlaceholder(Graphics g)
        {
            using var br = new SolidBrush(Color.FromArgb(40, Color.White));
            g.FillRectangle(br, ClientRectangle);
            var text = "NO CONTENT";
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var font = new Font(Font.FontFamily, 12f, FontStyle.Italic);
            using var brush = new SolidBrush(Color.WhiteSmoke);
            g.DrawString(text, font, brush, ClientRectangle, sf);
        }



        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();

            // Альтернативное панорамирование: СКМ
            if (e.Button == MouseButtons.Middle)
            {
                _panning = true;
                _lastMouse = e.Location;
                Cursor = Cursors.Hand;
                Capture = true;
                return;
            }

            // Панорамирование: Space + ЛКМ
            if (e.Button == MouseButtons.Left && _spaceDown)
            {
                _panning = true;
                _lastMouse = e.Location;
                Cursor = Cursors.Hand;
                Capture = true;
                return;
            }

            // Жест масштабирования: ПКМ + Ctrl + Shift
            if (e.Button == MouseButtons.Right && (ModifierKeys & Keys.Control) == Keys.Control && (ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                _scalingGestureActive = true;
                _scaleLastMouse = e.Location;
                _scaleAnchorImg = ClampToContentOrImage(ClientToImageF(e.Location));
                Cursor = Cursors.SizeNS;
                Capture = true;

                // Однократно обновим координаты метки в начале жеста
                var rounded = new Point((int)Math.Round(_scaleAnchorImg.X), (int)Math.Round(_scaleAnchorImg.Y));
                if (IsHandleCreated)
                {
                    BeginInvoke(new Action(() => OnMarkChanged(rounded)));
                }
                return;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left)
            {
                _panning = false;
                if (!_spaceDown) Cursor = Cursors.Default;
                Capture = false;
            }

            if (e.Button == MouseButtons.Right)
            {
                if (_scalingGestureActive)
                {
                    _scalingGestureActive = false;
                    Cursor = Cursors.Default;
                    Capture = false;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_panning)
            {
                var dx = e.X - _lastMouse.X;
                var dy = e.Y - _lastMouse.Y;
                _lastMouse = e.Location;
                Offset = new PointF(_offset.X + dx, _offset.Y + dy);
            }
            else if (_scalingGestureActive)
            {
                // Движение вниз — увеличить, вверх — уменьшить
                int dy = e.Y - _scaleLastMouse.Y;
                if (dy != 0)
                {
                    float factor = (float)Math.Exp(ScaleGestureSensitivity * dy);
                    float targetZoom = Math.Max(0.01f, Math.Min(256f, _zoom * factor));
                    ZoomAround(_scaleAnchorImg, targetZoom);
                    _scaleLastMouse = e.Location;
                }
            }

            _lastMouseMovePoint = e.Location;
            if (!_scalingGestureActive) // блокируем обновление метки во время жеста масштабирования
                DebounceMarkUpdate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left)
            {
                var anchor = ClampToContentOrImage(ClientToImageF(e.Location));
                float newZoom = Math.Max(0.01f, Math.Min(256f, _zoom * 1.5f));
                ZoomAround(anchor, newZoom);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if ((ModifierKeys & Keys.Control) == 0)
                return;

            // ZTH: сведение к метке (пиксель под курсором) или ближайшему
            var clientPt = e.Location;
            var imagePtF = ClientToImageF(clientPt);
            var anchorImg = ClampToContentOrImage(imagePtF);

            float oldZoom = _zoom;
            float zoomDelta = e.Delta > 0 ? 1.1f : 1f / 1.1f;
            float newZoom = Math.Max(0.01f, Math.Min(256f, _zoom * zoomDelta));
            if (Math.Abs(newZoom - oldZoom) < 0.0001f)
                return;

            // // До масштабирования: позиция якорной точки в клиентских координатах
            var anchorClientBefore = ImageToClient(anchorImg);

            Zoom = newZoom; // вызовет Invalidate
            // Debug.WriteLine($"Масштаб: {Zoom}");

            // После масштабирования: вычислим новый offset так, чтобы anchorClient совпала
            var anchorClientAfter = ImageToClient(anchorImg);
            var shift = new PointF(anchorClientBefore.X - anchorClientAfter.X, anchorClientBefore.Y - anchorClientAfter.Y);
            Offset = new PointF(_offset.X + shift.X, _offset.Y + shift.Y);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Space)
                return true;
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space)
            {
                _spaceDown = true;
                if (ClientRectangle.Contains(PointToClient(Cursor.Position)))
                    Cursor = Cursors.Hand;
            }

            // Горячие клавиши зума +/-
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus)
            {
                var pt = PointToClient(Cursor.Position);
                PointF anchor = ClientRectangle.Contains(pt) ? ClampToContentOrImage(ClientToImageF(pt)) : ClampToContentOrImage(ClientToImageF(new Point(Width / 2, Height / 2)));
                ZoomAround(anchor, Math.Max(0.01f, Math.Min(256f, _zoom * 1.1f)));
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
            {
                var pt = PointToClient(Cursor.Position);
                PointF anchor = ClientRectangle.Contains(pt) ? ClampToContentOrImage(ClientToImageF(pt)) : ClampToContentOrImage(ClientToImageF(new Point(Width / 2, Height / 2)));
                ZoomAround(anchor, Math.Max(0.01f, Math.Min(256f, _zoom / 1.1f)));
                e.Handled = true;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Space)
            {
                _spaceDown = false;
                if (!_panning)
                    Cursor = Cursors.Default;
            }
        }

        #endregion





        #region Преобразования координат

        public PointF ClientToImageF(Point clientPt)
        {
            if (_zoom <= 0)
                return new PointF(float.NaN, float.NaN);
            var x = (clientPt.X - _offset.X) / _zoom;
            var y = (clientPt.Y - _offset.Y) / _zoom;
            return new PointF(x, y);
        }

        public PointF ImageToClient(PointF imgPt)
        {
            var x = imgPt.X * _zoom + _offset.X;
            var y = imgPt.Y * _zoom + _offset.Y;
            return new PointF(x, y);
        }

        private PointF ClampToImage(PointF imgPt)
        {
            if (_image == null)
                return imgPt;
            float x = Math.Max(0, Math.Min(_image.Width - 1, imgPt.X));
            float y = Math.Max(0, Math.Min(_image.Height - 1, imgPt.Y));
            return new PointF(x, y);
        }

        /// <summary>
        /// Ограничивает точку границами контента (изображения или фигур).
        /// </summary>
        private PointF ClampToContentOrImage(PointF worldPt)
        {
            // Если есть изображение, используем его границы
            if (_image != null)
            {
                float x = Math.Max(0, Math.Min(_image.Width - 1, worldPt.X));
                float y = Math.Max(0, Math.Min(_image.Height - 1, worldPt.Y));
                return new PointF(x, y);
            }

            // Иначе границы векторного контента
            var bounds = GetVectorContentBounds();
            if (!bounds.IsEmpty)
            {
                float x = Math.Max(bounds.Left, Math.Min(bounds.Right, worldPt.X));
                float y = Math.Max(bounds.Top, Math.Min(bounds.Bottom, worldPt.Y));
                return new PointF(x, y);
            }

            // Если нет контента, возвращаем точку как есть
            return worldPt;
        }

        /// <summary>
        /// Вычисляет границы всех фигур.
        /// </summary>
        private RectangleF GetFiguresBounds()
        {
            if (_figures == null || !_figures.Any())
                return RectangleF.Empty;

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var figure in _figures)
            {
                var center = figure.Center;
                float hw = IFigure.HalfWidth;
                float hh = IFigure.HalfHeight;

                minX = Math.Min(minX, center.X - hw);
                minY = Math.Min(minY, center.Y - hh);
                maxX = Math.Max(maxX, center.X + hw);
                maxY = Math.Max(maxY, center.Y + hh);
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        #endregion





        private Rectangle GetImageViewRect()
        {
            var rect = ClientRectangle;
            if (_vScrollBar != null && _vScrollBar.Visible)
                rect.Width -= _vScrollBar.Width;
            if (_hScrollBar != null && _hScrollBar.Visible)
                rect.Height -= _hScrollBar.Height;
            return rect;
        }

        private void DebounceMarkUpdate()
        {
            if (IsInDesignMode) return;
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }



        public void ResetView()
        {
            if (_image == null)
            {
                _offset = new PointF(0, 0);
                _zoom = 1f;
                OnZoomChanged(EventArgs.Empty);
                return;
            }

            var client = ClientSize;
            if (client.Width <= 0 || client.Height <= 0)
                return;

            var viewRect = GetImageViewRect();
            float zoomX = (float)viewRect.Width / _image.Width;
            float zoomY = (float)viewRect.Height / _image.Height;
            _zoom = Math.Min(zoomX, zoomY);
            // Центрируем в пределах viewRect
            var imgSize = new SizeF(_image.Width * _zoom, _image.Height * _zoom);
            _offset = new PointF(viewRect.Left + (viewRect.Width - imgSize.Width) / 2f,
                                  viewRect.Top + (viewRect.Height - imgSize.Height) / 2f);
            OnZoomChanged(EventArgs.Empty);
        }

        public void FitToWindow()
        {
            if (_image == null && _figures != null && _figures.Any())
            {
                FitToContent();
            }
            else
            {
                ResetView();
                UpdateScrollBars();
                Invalidate();
            }
        }

        /// <summary>
        /// Автоматически масштабирует и центрирует контент (изображение или фигуры) по центру области просмотра с отступами.
        /// </summary>
        public void FitToContent()
        {
            var bounds = GetContentBounds();
            if (bounds.IsEmpty)
            {
                _zoom = 1f;
                _offset = PointF.Empty;
                UpdateScrollBars();
                Invalidate();
                return;
            }

            var viewRect = GetImageViewRect();
            
            // Если область просмотра слишком мала (например, при инициализации или сворачивании),
            // отменяем пересчет масштаба, чтобы избежать некорректного (очень маленького) зума.
            if (viewRect.Width < 20 || viewRect.Height < 20)
                return;

            float padding = 20f;
            float availableW = Math.Max(1, viewRect.Width - 2 * padding);
            float availableH = Math.Max(1, viewRect.Height - 2 * padding);

            // Вычисляем необходимый зум для вмещения контента
            float zoomX = availableW / bounds.Width;
            float zoomY = availableH / bounds.Height;
            float newZoom = Math.Min(zoomX, zoomY);
            
            // Ограничиваем зум разумными пределами
            newZoom = Math.Max(0.01f, Math.Min(256f, newZoom));

            _zoom = newZoom;

            // Центрируем
            float centerX = viewRect.Left + viewRect.Width / 2f;
            float centerY = viewRect.Top + viewRect.Height / 2f;
            
            float contentCenterX = bounds.Left + bounds.Width / 2f;
            float contentCenterY = bounds.Top + bounds.Height / 2f;

            // Offset = Center - ContentCenter * Zoom
            _offset = new PointF(centerX - contentCenterX * _zoom, centerY - contentCenterY * _zoom);

            UpdateScrollBars();
            Invalidate();
            OnZoomChanged(EventArgs.Empty);
        }

        public void ZoomTo100Percent()
        {
            if (_image == null) return;
            var viewRect = GetImageViewRect();
            var viewCenter = new Point(viewRect.Left + viewRect.Width / 2, viewRect.Top + viewRect.Height / 2);
            var anchorImg = ClampToImage(ClientToImageF(viewCenter));
            ZoomAround(anchorImg, 1f);
        }





        #region Обновление позиций и размеров областей прокрутки

        /// <summary>
        /// Вычисляет границы контента (изображения или фигур).
        /// </summary>
        private RectangleF GetContentBounds()
        {
            if (_image != null)
                return new RectangleF(0, 0, _image.Width, _image.Height);

            return GetVectorContentBounds();
        }

        private RectangleF GetVectorContentBounds()
        {
            RectangleF bounds = RectangleF.Empty;

            if (_figures != null && _figures.Any())
                bounds = GetFiguresBounds();

            if (_edges != null && _edges.Any())
            {
                var edgesBounds = GetEdgesBounds();
                if (bounds.IsEmpty)
                    bounds = edgesBounds;
                else if (!edgesBounds.IsEmpty)
                    bounds = RectangleF.Union(bounds, edgesBounds);
            }
            return bounds;
        }

        private RectangleF GetEdgesBounds()
        {
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;
            bool hasPoints = false;

            foreach (var edge in _edges!)
            {
                if (edge.Points == null) continue;
                foreach (var pt in edge.Points)
                {
                    if (pt.X < minX) minX = pt.X;
                    if (pt.Y < minY) minY = pt.Y;
                    if (pt.X > maxX) maxX = pt.X;
                    if (pt.Y > maxY) maxY = pt.Y;
                    hasPoints = true;
                }
            }

            if (!hasPoints) return RectangleF.Empty;
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        private void UpdateScrollBars()
        {
            if (_updatingScrollBars)
                return;
            
            var contentBounds = GetContentBounds();
            if (contentBounds.IsEmpty || _zoom <= 0)
            {
                if (_hScrollBar != null) _hScrollBar.Visible = false;
                if (_vScrollBar != null) _vScrollBar.Visible = false;
                return;
            }

            _updatingScrollBars = true;

            var viewRect = GetImageViewRect();
            
            // Размеры контента в масштабе
            float contentW = contentBounds.Width * _zoom;
            float contentH = contentBounds.Height * _zoom;

            // Горизонтальный
            if (_hScrollBar != null)
            {
                if (contentW > viewRect.Width)
                {
                    _hScrollBar.Visible = true;
                    // Максимум скроллбара равен полному размеру контента
                    _hScrollBar.Maximum = (int)Math.Ceiling(contentW);
                    // LargeChange равен размеру видимой области
                    _hScrollBar.LargeChange = Math.Max(1, viewRect.Width);
                    _hScrollBar.Minimum = 0;
                    _hScrollBar.SmallChange = Math.Max(1, viewRect.Width / 20);
                    
                    float minX = contentBounds.Left;
                    // Offset.X = viewRect.Left - Value - minX * zoom
                    // Value = viewRect.Left - Offset.X - minX * zoom
                    float valF = viewRect.Left - _offset.X - (minX * _zoom);
                    
                    int val = (int)Math.Round(valF);
                    // Ограничиваем значение допустимым диапазоном скроллбара
                    int maxVal = Math.Max(0, _hScrollBar.Maximum - _hScrollBar.LargeChange + 1);
                    _hScrollBar.Value = Math.Max(_hScrollBar.Minimum, Math.Min(maxVal, val));
                }
                else
                {
                    _hScrollBar.Visible = false;
                }
            }

            // Вертикальный
            viewRect = GetImageViewRect(); // пересчитать, если поменялась видимость H
            if (_vScrollBar != null)
            {
                if (contentH > viewRect.Height)
                {
                    _vScrollBar.Visible = true;
                    // Максимум скроллбара равен полному размеру контента
                    _vScrollBar.Maximum = (int)Math.Ceiling(contentH);
                    // LargeChange равен размеру видимой области
                    _vScrollBar.LargeChange = Math.Max(1, viewRect.Height);
                    _vScrollBar.Minimum = 0;
                    _vScrollBar.SmallChange = Math.Max(1, viewRect.Height / 20);
                    
                    float minY = contentBounds.Top;
                    float valF = viewRect.Top - _offset.Y - (minY * _zoom);
                    
                    int val = (int)Math.Round(valF);
                    int maxVal = Math.Max(0, _vScrollBar.Maximum - _vScrollBar.LargeChange + 1);
                    _vScrollBar.Value = Math.Max(_vScrollBar.Minimum, Math.Min(maxVal, val));
                }
                else
                {
                    _vScrollBar.Visible = false;
                }
            }

            _updatingScrollBars = false;
        }

        private void HScrollBar_ValueChanged(object? sender, EventArgs e)
        {
            if (_updatingScrollBars) return;
            var viewRect = GetImageViewRect();
            if (_hScrollBar != null)
            {
                var contentBounds = GetContentBounds();
                float minX = contentBounds.IsEmpty ? 0 : contentBounds.Left;
                
                // Desired Offset.X = viewRect.Left - Value - minX * zoom
                float desiredX = viewRect.Left - _hScrollBar.Value - (minX * _zoom);
                SetOffsetClamped(new PointF(desiredX, _offset.Y));
            }
        }

        private void VScrollBar_ValueChanged(object? sender, EventArgs e)
        {
            if (_updatingScrollBars) return;
            var viewRect = GetImageViewRect();
            if (_vScrollBar != null)
            {
                var contentBounds = GetContentBounds();
                float minY = contentBounds.IsEmpty ? 0 : contentBounds.Top;

                // Desired Offset.Y = viewRect.Top - Value - minY * zoom
                float desiredY = viewRect.Top - _vScrollBar.Value - (minY * _zoom);
                SetOffsetClamped(new PointF(_offset.X, desiredY));
            }
        }

        private void SetOffsetClamped(PointF desired)
        {
            var viewRect = GetImageViewRect();

            // 1) Если есть изображение — используем существующую логику клампа по изображению
            if (_image != null)
            {
                float imgW = _image.Width * _zoom;
                float imgH = _image.Height * _zoom;

                float clampedX = desired.X;
                float clampedY = desired.Y;

                // Горизонталь: если можем обеспечить видимость >= половины изображения, фиксируем центр в пределах viewRect
                if (imgW <= 2 * viewRect.Width)
                {
                    // центр = offset + imgW/2, удерживаем в [viewRect.Left, viewRect.Right]
                    float centerX = desired.X + imgW / 2f;
                    if (centerX < viewRect.Left)
                        clampedX += (viewRect.Left - centerX);
                    if (centerX > viewRect.Right)
                        clampedX -= (centerX - viewRect.Right);
                }
                else
                {
                    // Стандартный кламп по краям
                    float minOffsetX = viewRect.Right - imgW;
                    float maxOffsetX = viewRect.Left;
                    clampedX = Math.Max(minOffsetX, Math.Min(maxOffsetX, desired.X));
                }

                // Вертикаль: аналогично
                if (imgH <= 2 * viewRect.Height)
                {
                    float centerY = desired.Y + imgH / 2f;
                    if (centerY < viewRect.Top)
                        clampedY += (viewRect.Top - centerY);
                    if (centerY > viewRect.Bottom)
                        clampedY -= (centerY - viewRect.Bottom);
                }
                else
                {
                    float minOffsetY = viewRect.Bottom - imgH;
                    float maxOffsetY = viewRect.Top;
                    clampedY = Math.Max(minOffsetY, Math.Min(maxOffsetY, desired.Y));
                }

                _offset = new PointF(clampedX, clampedY);
                UpdateScrollBars();
                Invalidate();
                return;
            }

            // 2) Изображения нет — клампим по векторным фигурам, если они есть
            float clampedFx = desired.X;
            float clampedFy = desired.Y;

            var bounds = GetVectorContentBounds();
            if (!bounds.IsEmpty)
            {
                float contW = bounds.Width * _zoom;
                float contH = bounds.Height * _zoom;

                // Положение контента (левый верхний угол) в клиентских координатах при желаемом offset
                float topLeftX = bounds.Left * _zoom + desired.X;
                float topLeftY = bounds.Top * _zoom + desired.Y;

                // Горизонталь: логика аналогична изображению, но учитывает смещение bounds.Left
                if (contW <= 2 * viewRect.Width)
                {
                    float centerX = topLeftX + contW / 2f;
                    if (centerX < viewRect.Left)
                        clampedFx += (viewRect.Left - centerX);
                    if (centerX > viewRect.Right)
                        clampedFx -= (centerX - viewRect.Right);
                }
                else
                {
                    float minTopLeftX = viewRect.Right - contW;
                    float maxTopLeftX = viewRect.Left;
                    float newTopLeftX = Math.Max(minTopLeftX, Math.Min(maxTopLeftX, topLeftX));
                    clampedFx += (newTopLeftX - topLeftX);
                }

                // Вертикаль
                if (contH <= 2 * viewRect.Height)
                {
                    float centerY = topLeftY + contH / 2f;
                    if (centerY < viewRect.Top)
                        clampedFy += (viewRect.Top - centerY);
                    if (centerY > viewRect.Bottom)
                        clampedFy -= (centerY - viewRect.Bottom);
                }
                else
                {
                    float minTopLeftY = viewRect.Bottom - contH;
                    float maxTopLeftY = viewRect.Top;
                    float newTopLeftY = Math.Max(minTopLeftY, Math.Min(maxTopLeftY, topLeftY));
                    clampedFy += (newTopLeftY - topLeftY);
                }
            }

            // Запоминаем смещение и обязательно перерисовываем, даже когда нет изображения
            _offset = new PointF(clampedFx, clampedFy);
            UpdateScrollBars(); // спрячет скроллы при отсутствии изображения
            Invalidate();
        }

        #endregion
    }
}
