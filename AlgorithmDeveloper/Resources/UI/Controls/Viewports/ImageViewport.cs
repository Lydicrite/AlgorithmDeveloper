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
using AlgorithmDeveloper.AlgorithmModel.Model.Vertices.Vizualization;

namespace AlgorithmDeveloper.Resources.UI.Controls.Viewports
{
    public partial class ImageViewport : UserControl
    {
        private Image? _image;
        private IEnumerable<IFigure>? _figures;
        private float _zoom = 1f;                       // масштаб: 1 = 100%
        private PointF _offset = new PointF(0, 0);      // сдвиг панорамирования в пикселях устройства
        private bool _panning;
        private Point _lastMouse;
        private bool _spaceDown;
        private readonly object _renderLock = new object();

        private bool _updatingScrollBars = false;

        // Дебаунс для события метки
        private CancellationTokenSource? _markCts;
        private Point _lastMouseMovePoint;
        private int _debounceMs = 1;

        // Жест масштабирования Ctrl + Shift + ПКМ
        private bool _scalingGestureActive;
        private Point _scaleLastMouse;
        private PointF _scaleAnchorImg;

        private bool IsInDesignMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime || (Site?.DesignMode ?? DesignMode);

        public ImageViewport()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            BackColor = Color.DimGray;
            TabStop = true;
            InitializeScrollBars();
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
                    Invalidate();
                }
            }
        }

        #endregion





        #region Векторная отрисовка фигур

        /// <summary>
        /// Отрисовывает все векторные фигуры.
        /// </summary>
        private void DrawVectorFigures(Graphics g)
        {
            using var pen = new Pen(_figureStrokeColor, _figureStrokeWidth)
            {
                Alignment = PenAlignment.Center
            };
            using var fillBrush = new SolidBrush(_figureFillColor);
            using var textBrush = new SolidBrush(_figureTextColor);
            using var centeredFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoClip
            };

            foreach (var figure in _figures!)
            {
                DrawFigure(g, figure, pen, fillBrush, textBrush, centeredFormat);
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
            if (figure is AlgoDev.Model.Vertices.IBDVertex vertex)
            {
                return vertex.ID ?? string.Empty;
            }
            return string.Empty;
        }

        #endregion



        #region Свойства стиля векторной отрисовки

        private Color _figureStrokeColor = Color.Black;
        private Color _figureFillColor = Color.White;
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

            // До масштабирования: позиция якорной точки в клиентских координатах
            var anchorClientBefore = ImageToClient(anchorImg);

            Zoom = newZoom; // вызовет Invalidate

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

            // Если есть фигуры, находим границы контента
            if (_figures != null && _figures.Any())
            {
                var bounds = GetFiguresBounds();
                if (!bounds.IsEmpty)
                {
                    float x = Math.Max(bounds.Left, Math.Min(bounds.Right, worldPt.X));
                    float y = Math.Max(bounds.Top, Math.Min(bounds.Bottom, worldPt.Y));
                    return new PointF(x, y);
                }
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
            _markCts?.Cancel();
            var cts = new CancellationTokenSource();
            _markCts = cts;
            var token = cts.Token;
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(_debounceMs, token);
                    if (token.IsCancellationRequested) return;

                    // Выполняем доступ к изображению и вычисления ТОЛЬКО в UI-потоке
                    if (IsHandleCreated)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            if (token.IsCancellationRequested) return;
                            if (_image == null) return; // нет изображения — координаты метки не вычисляем

                            var clientPt = _lastMouseMovePoint;
                            var imgPtF = ClientToImageF(clientPt);
                            var clamped = ClampToImage(imgPtF);
                            var rounded = new Point((int)Math.Round(clamped.X), (int)Math.Round(clamped.Y));

                            OnMarkChanged(rounded);
                        }));
                    }
                }
                catch (OperationCanceledException) { }
            }, token);
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
            ResetView();
            UpdateScrollBars();
            Invalidate();
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

        private void UpdateScrollBars()
        {
            if (_updatingScrollBars)
                return;
            if (_image == null || _zoom <= 0)
            {
                if (_hScrollBar != null) _hScrollBar.Visible = false;
                if (_vScrollBar != null) _vScrollBar.Visible = false;
                return;
            }

            _updatingScrollBars = true;

            var viewRect = GetImageViewRect();
            var imgW = _image.Width * _zoom;
            var imgH = _image.Height * _zoom;

            // Горизонтальный
            if (_hScrollBar != null)
            {
                if (imgW > viewRect.Width)
                {
                    _hScrollBar.Visible = true;
                    int max = (int)Math.Ceiling(imgW - viewRect.Width);
                    _hScrollBar.Minimum = 0;
                    _hScrollBar.Maximum = Math.Max(0, max);
                    _hScrollBar.LargeChange = Math.Max(1, viewRect.Width / 4);
                    _hScrollBar.SmallChange = Math.Max(1, viewRect.Width / 20);
                    int val = (int)Math.Round(-(_offset.X - viewRect.Left));
                    _hScrollBar.Value = Math.Max(_hScrollBar.Minimum, Math.Min(_hScrollBar.Maximum, val));
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
                if (imgH > viewRect.Height)
                {
                    _vScrollBar.Visible = true;
                    int max = (int)Math.Ceiling(imgH - viewRect.Height);
                    _vScrollBar.Minimum = 0;
                    _vScrollBar.Maximum = Math.Max(0, max);
                    _vScrollBar.LargeChange = Math.Max(1, viewRect.Height / 4);
                    _vScrollBar.SmallChange = Math.Max(1, viewRect.Height / 20);
                    int val = (int)Math.Round(-(_offset.Y - viewRect.Top));
                    _vScrollBar.Value = Math.Max(_vScrollBar.Minimum, Math.Min(_vScrollBar.Maximum, val));
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
                SetOffsetClamped(new PointF(viewRect.Left - _hScrollBar.Value, _offset.Y));
            }
        }

        private void VScrollBar_ValueChanged(object? sender, EventArgs e)
        {
            if (_updatingScrollBars) return;
            var viewRect = GetImageViewRect();
            if (_vScrollBar != null)
            {
                SetOffsetClamped(new PointF(_offset.X, viewRect.Top - _vScrollBar.Value));
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

            if (_figures != null && _figures.Any())
            {
                var bounds = GetFiguresBounds();
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
