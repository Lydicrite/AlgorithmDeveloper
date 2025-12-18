using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AlgorithmDeveloper.UI.Elements.Controls.CustomizableTabControl.Styles
{
    /// <summary>
    /// Абстрактный базовый класс для провайдеров стилей вкладок
    /// </summary>
    public abstract class TabStyleProvider
    {
        #region Конструктор

        /// <summary>
        /// Инициализирует новый экземпляр класса TabStyleProvider
        /// </summary>
        /// <param name="tabControl">Контрол вкладок для применения стиля</param>
        protected TabStyleProvider(CustomizableTabControl tabControl)
        {
            _TabControl = tabControl;

            _BorderColor = Color.Empty;
            _BorderColorSelected = Color.Empty;
            _FocusColor = Color.Orange;

            if (_TabControl.RightToLeftLayout)
                _ImageAlign = ContentAlignment.MiddleRight;
            else
                _ImageAlign = ContentAlignment.MiddleLeft;

            HotTrack = true;

            // Должно быть установлено после _Overlap, так как это используется в расчетах фактического отступа
            Padding = new Point(6, 3);
        }

        #endregion Конструктор





        #region Фабричные методы

        /// <summary>
        /// Создает провайдер стиля в зависимости от стиля отображения контрола
        /// </summary>
        /// <param name="tabControl">Контрол вкладок</param>
        /// <returns>Соответствующий провайдер стиля</returns>
        public static TabStyleProvider CreateProvider(CustomizableTabControl tabControl)
        {
            TabStyleProvider provider;

            // В зависимости от стиля отображения tabControl генерируем соответствующий провайдер
            switch (tabControl.DisplayStyle)
            {
                case TabStyle.None:
                    provider = new TabStyleNoneProvider(tabControl);
                    break;

                case TabStyle.Default:
                    provider = new TabStyleDefaultProvider(tabControl);
                    break;

                case TabStyle.Dark:
                    provider = new TabStyleDarkProvider(tabControl);
                    break;

                default:
                    provider = new TabStyleDefaultProvider(tabControl);
                    break;
            }

            provider._Style = tabControl.DisplayStyle;
            return provider;
        }

        #endregion Фабричные методы





        #region Защищенные переменные

        /// <summary>
        /// Ссылка на контрол вкладок
        /// </summary>
        protected CustomizableTabControl _TabControl;
        /// <summary>
        /// Отступы вкладки
        /// </summary>
        protected Point _Padding;
        /// <summary>
        /// Включение отслеживания при наведении
        /// </summary>
        protected bool _HotTrack;
        /// <summary>
        /// Стиль отображения
        /// </summary>
        protected TabStyle _Style = TabStyle.Default;
        /// <summary>
        /// Выравнивание изображения
        /// </summary>
        protected ContentAlignment _ImageAlign;
        /// <summary>
        /// Радиус скругления углов
        /// </summary>
        protected int _Radius = 2;
        /// <summary>
        /// Перекрытие вкладок
        /// </summary>
        protected int _Overlap;
        /// <summary>
        /// Отслеживание фокуса
        /// </summary>
        protected bool _FocusTrack;
        /// <summary>
        /// Прозрачность
        /// </summary>
        protected float _Opacity = 1;
        /// <summary>
        /// Показывать кнопку закрытия вкладки
        /// </summary>
        protected bool _ShowTabCloser;
        /// <summary>
        /// Цвет границы выбранной вкладки
        /// </summary>
        protected Color _BorderColorSelected = Color.Empty;
        /// <summary>
        /// Цвет границы обычной вкладки
        /// </summary>
        protected Color _BorderColor = Color.Empty;
        /// <summary>
        /// Цвет границы при наведении
        /// </summary>
        protected Color _BorderColorHot = Color.Empty;
        /// <summary>
        /// Цвет активной кнопки закрытия
        /// </summary>
        protected Color _CloserColorActive = Color.Black;
        /// <summary>
        /// Цвет кнопки закрытия
        /// </summary>
        protected Color _CloserColor = Color.DarkGray;
        /// <summary>
        /// Цвет фокуса
        /// </summary>
        protected Color _FocusColor = Color.Empty;
        /// <summary>
        /// Второй цвет для индикатора фокуса (градиент)
        /// </summary>
        protected Color _FocusColorSecondary = Color.Empty;
        /// <summary>
        /// Цвет фона обычной вкладки
        /// </summary>
        protected Color _BackgroundColor = Color.Empty;
        /// <summary>
        /// Цвет фона вкладки при наведении
        /// </summary>
        protected Color _BackgroundColorHot = Color.Empty;
        /// <summary>
        /// Цвет фона выбранной вкладки
        /// </summary>
        protected Color _BackgroundColorSelected = Color.Empty;
        /// <summary>
        /// Цвет фона отключенной вкладки
        /// </summary>
        protected Color _BackgroundColorDisabled = Color.Empty;
        /// <summary>
        /// Цвет текста
        /// </summary>
        protected Color _TextColor = Color.Empty;
        /// <summary>
        /// Цвет текста при наведении
        /// </summary>
        protected Color _TextColorHot = Color.Empty;
        /// <summary>
        /// Цвет текста выбранной вкладки
        /// </summary>
        protected Color _TextColorSelected = Color.Empty;     
        /// <summary>
        /// Цвет текста отключенной вкладки
        /// </summary>
        protected Color _TextColorDisabled = Color.Empty;

        #endregion Защищенные переменные





        #region Переопределяемые методы

        /// <summary>
        /// Добавляет границу вкладки к графическому пути
        /// </summary>
        /// <param name="path">Графический путь</param>
        /// <param name="tabBounds">Границы вкладки</param>
        public abstract void AddTabBorder(GraphicsPath path, Rectangle tabBounds);

        /// <summary>
        /// Получает прямоугольник вкладки
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Прямоугольник вкладки</returns>
        public virtual Rectangle GetTabRect(int index)
        {
            if (index < 0)
                return new Rectangle();

            Rectangle tabBounds = _TabControl.GetTabRect(index);

            if (_TabControl.RightToLeftLayout)
                tabBounds.X = _TabControl.Width - tabBounds.Right;

            bool firstTabinRow = _TabControl.IsFirstTabInRow(index);

            // Расширяем для перекрытия с tabPage
            switch (_TabControl.Alignment)
            {
                case TabAlignment.Top:
                    tabBounds.Height += 2;
                    break;

                case TabAlignment.Bottom:
                    tabBounds.Height += 2;
                    tabBounds.Y -= 2;
                    break;

                case TabAlignment.Left:
                    tabBounds.Width += 2;
                    break;

                case TabAlignment.Right:
                    tabBounds.X -= 2;
                    tabBounds.Width += 2;
                    break;
            }

            // Создаем перекрытие, если это не первая вкладка в ряду для выравнивания с tabPage
            if ((!firstTabinRow || _TabControl.RightToLeftLayout) && _Overlap > 0)
            {
                if (_TabControl.Alignment <= TabAlignment.Bottom)
                {
                    tabBounds.X -= _Overlap;
                    tabBounds.Width += _Overlap;
                }
                else
                {
                    tabBounds.Y -= _Overlap;
                    tabBounds.Height += _Overlap;
                }
            }

            // Корректируем первую вкладку в ряду для выравнивания с tabPage
            EnsureFirstTabIsInView(ref tabBounds, index);

            return tabBounds;
        }

        /// <summary>
        /// Обеспечивает видимость первой вкладки в ряду
        /// </summary>
        /// <param name="tabBounds">Границы вкладки для корректировки</param>
        /// <param name="index">Индекс вкладки</param>
        protected virtual void EnsureFirstTabIsInView(ref Rectangle tabBounds, int index)
        {
            // Корректируем первую вкладку в ряду для выравнивания с tabPage.
            // Убеждаемся, что мы перемещаем только видимые вкладки, так как они могут быть прокручены за пределы видимости.

            bool firstTabinRow = _TabControl.IsFirstTabInRow(index);

            if (firstTabinRow)
            {
                if (_TabControl.Alignment <= TabAlignment.Bottom)
                {
                    if (_TabControl.RightToLeftLayout)
                    {
                        if (tabBounds.Left < _TabControl.Right)
                        {
                            int tabPageRight = _TabControl.GetPageBounds(index).Right;

                            if (tabBounds.Right > tabPageRight)
                                tabBounds.Width -= tabBounds.Right - tabPageRight;
                        }
                    }
                    else
                    {
                        if (tabBounds.Right > 0)
                        {
                            int tabPageX = _TabControl.GetPageBounds(index).X;

                            if (tabBounds.X < tabPageX)
                            {
                                tabBounds.Width -= tabPageX - tabBounds.X;
                                tabBounds.X = tabPageX;
                            }
                        }
                    }
                }
                else
                {
                    if (_TabControl.RightToLeftLayout)
                    {
                        if (tabBounds.Top < _TabControl.Bottom)
                        {
                            int tabPageBottom = _TabControl.GetPageBounds(index).Bottom;

                            if (tabBounds.Bottom > tabPageBottom)
                                tabBounds.Height -= tabBounds.Bottom - tabPageBottom;
                        }
                    }
                    else
                    {
                        if (tabBounds.Bottom > 0)
                        {
                            int tabPageY = _TabControl.GetPageBounds(index).Location.Y;

                            if (tabBounds.Y < tabPageY)
                            {
                                tabBounds.Height -= tabPageY - tabBounds.Y;
                                tabBounds.Y = tabPageY;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получает кисть для фона вкладки
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Кисть для рисования фона</returns>
        protected virtual Brush GetTabBackgroundBrush(int index)
        {
            LinearGradientBrush? fillBrush = null;

            // Захватываем цвета в зависимости от состояния выбора вкладки
            // Используем настраиваемые цвета, если они заданы, иначе значения по умолчанию
            var dark = Color.FromArgb(207, 207, 207);
            var light = Color.FromArgb(242, 242, 242);

            if (_TabControl.SelectedIndex == index)
            {
                // Выбранная вкладка
                if (!_BackgroundColorSelected.IsEmpty)
                {
                    light = _BackgroundColorSelected;
                    dark = _BackgroundColorSelected;
                }
                else
                {
                    dark = SystemColors.ControlLight;
                    light = SystemColors.Window;
                }
            }
            else if (!_TabControl.TabPages[index].Enabled)
            {
                // Отключенная вкладка
                if (!_BackgroundColorDisabled.IsEmpty)
                {
                    light = _BackgroundColorDisabled;
                    dark = _BackgroundColorDisabled;
                }
                else
                {
                    light = dark;
                }
            }
            else if (_HotTrack && index == _TabControl.ActiveIndex)
            {
                // Вкладка при наведении
                if (!_BackgroundColorHot.IsEmpty)
                {
                    light = _BackgroundColorHot;
                    dark = _BackgroundColorHot;
                }
                else
                {
                    // Включаем отслеживание при наведении (значения по умолчанию)
                    light = Color.FromArgb(234, 246, 253);
                    dark = Color.FromArgb(167, 217, 245);
                }
            }
            else
            {
                // Обычная вкладка
                if (!_BackgroundColor.IsEmpty)
                {
                    light = _BackgroundColor;
                    dark = _BackgroundColor;
                }
                // Иначе используем значения по умолчанию, уже установленные выше
            }

            // Получаем правильно выровненный градиент
            Rectangle tabBounds = GetTabRect(index);
            tabBounds.Inflate(3, 3);
            tabBounds.X -= 1;
            tabBounds.Y -= 1;

            switch (_TabControl.Alignment)
            {
                case TabAlignment.Top:
                    if (_TabControl.SelectedIndex == index)
                        dark = light;
                    fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
                    break;

                case TabAlignment.Bottom:
                    fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
                    break;

                case TabAlignment.Left:
                    fillBrush = new LinearGradientBrush(tabBounds, dark, light, LinearGradientMode.Horizontal);
                    break;

                case TabAlignment.Right:
                    fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Horizontal);
                    break;
            }

            // Добавляем смешивание
            if (fillBrush != null)
                fillBrush.Blend = GetBackgroundBlend();
            
            return fillBrush ?? new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
        }

        #endregion Переопределяемые методы





        #region Базовые свойства

        /// <summary>
        /// Получает или задает стиль отображения
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabStyle DisplayStyle
        {
            get { return _Style; }
            set { _Style = value; }
        }

        /// <summary>
        /// Получает или задает выравнивание изображения
        /// </summary>
        [Category("Внешний вид")]
        [Description("Определяет выравнивание изображения на вкладке")]
        public ContentAlignment ImageAlign
        {
            get { return _ImageAlign; }
            set
            {
                _ImageAlign = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает отступы вкладки
        /// </summary>
        [Category("Внешний вид")]
        [Description("Определяет внутренние отступы вкладки")]
        public Point Padding
        {
            get { return _Padding; }
            set
            {
                _Padding = value;
                // Эта строка вызовет пересоздание дескриптора, тем самым инвалидируя контрол
                if (_ShowTabCloser)
                {
                    if (value.X + _Radius / 2 < -6)
                        ((TabControl)_TabControl).Padding = new Point(0, value.Y);
                    else
                        ((TabControl)_TabControl).Padding = new Point(value.X + _Radius / 2 + 6, value.Y);
                }
                else
                {
                    if (value.X + _Radius / 2 < 1)
                        ((TabControl)_TabControl).Padding = new Point(0, value.Y);
                    else
                        ((TabControl)_TabControl).Padding = new Point(value.X + _Radius / 2 - 1, value.Y);
                }
            }
        }

        /// <summary>
        /// Получает или задает радиус скругления углов вкладки
        /// </summary>
        [Category("Внешний вид"), DefaultValue(1), Browsable(true)]
        [Description("Определяет радиус скругления углов вкладки")]
        public int Radius
        {
            get { return _Radius; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Радиус должен быть больше 1", nameof(value));

                _Radius = value;
                Padding = _Padding;
            }
        }

        /// <summary>
        /// Получает или задает величину перекрытия вкладок
        /// </summary>
        [Category("Внешний вид")]
        [Description("Определяет величину перекрытия между соседними вкладками")]
        public int Overlap
        {
            get { return _Overlap; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Вкладки не могут иметь отрицательное перекрытие", nameof(value));

                _Overlap = value;
            }
        }

        /// <summary>
        /// Получает или задает отслеживание фокуса
        /// </summary>
        [Category("Внешний вид")]
        [Description("Определяет, будет ли отображаться индикатор фокуса")]
        public bool FocusTrack
        {
            get { return _FocusTrack; }
            set
            {
                _FocusTrack = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает отслеживание при наведении
        /// </summary>
        [Category("Внешний вид")]
        [Description("Определяет, будет ли вкладка подсвечиваться при наведении мыши")]
        public bool HotTrack
        {
            get { return _HotTrack; }
            set
            {
                _HotTrack = value;
                ((TabControl)_TabControl).HotTrack = value;
            }
        }

        /// <summary>
        /// Получает или задает отображение кнопки закрытия вкладки
        /// </summary>
        [Category("Внешний вид")]
        [Description("Определяет, будет ли отображаться кнопка закрытия на вкладке")]
        public bool ShowTabCloser
        {
            get { return _ShowTabCloser; }
            set
            {
                _ShowTabCloser = value;
                Padding = _Padding;
            }
        }

        /// <summary>
        /// Получает или задает прозрачность вкладки
        /// </summary>
        [Category("Внешний вид")]
        [Description("Определяет уровень прозрачности вкладки (от 0 до 1)")]
        public float Opacity
        {
            get { return _Opacity; }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("Прозрачность должна быть между 0 и 1", nameof(value));

                _Opacity = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет границы выбранной вкладки
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "")]
        [Description("Цвет границы выбранной вкладки")]
        public Color BorderColorSelected
        {
            get { return _BorderColorSelected; }
            set
            {
                _BorderColorSelected = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет границы при наведении
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "")]
        [Description("Цвет границы вкладки при наведении мыши")]
        public Color BorderColorHot
        {
            get
            {
                if (_BorderColorHot.IsEmpty)
                    return SystemColors.ControlDark;
                else
                    return _BorderColorHot;
            }
            set
            {
                if (value.Equals(SystemColors.ControlDark))
                    _BorderColorHot = Color.Empty;
                else
                    _BorderColorHot = value;

                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет границы обычной вкладки
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "")]
        [Description("Цвет границы обычной вкладки")]
        public Color BorderColor
        {
            get
            {
                if (_BorderColor.IsEmpty)
                    return SystemColors.ControlDark;
                else
                    return _BorderColor;
            }
            set
            {
                if (value.Equals(SystemColors.ControlDark))
                    _BorderColor = Color.Empty;
                else
                    _BorderColor = value;

                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет текста вкладки
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "")]
        [Description("Цвет текста на вкладке")]
        public Color TextColor
        {
            get
            {
                if (_TextColor.IsEmpty)
                    return SystemColors.ControlText;
                else
                    return _TextColor;
            }
            set
            {
                if (value.Equals(SystemColors.ControlText))
                    _TextColor = Color.Empty;
                else
                    _TextColor = value;

                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет текста выбранной вкладки
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "")]
        [Description("Цвет текста выбранной вкладки")]
        public Color TextColorSelected
        {
            get
            {
                if (_TextColorSelected.IsEmpty)
                    return SystemColors.ControlText;
                else
                    return _TextColorSelected;
            }
            set
            {
                if (value.Equals(SystemColors.ControlText))
                    _TextColorSelected = Color.Empty;
                else
                    _TextColorSelected = value;

                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет текста отключенной вкладки
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "")]
        [Description("Цвет текста отключенной вкладки")]
        public Color TextColorDisabled
        {
            get
            {
                if (_TextColor.IsEmpty)
                    return SystemColors.ControlDark;
                else
                    return _TextColorDisabled;
            }
            set
            {
                if (value.Equals(SystemColors.ControlDark))
                    _TextColorDisabled = Color.Empty;
                else
                    _TextColorDisabled = value;

                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет индикатора фокуса
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "Orange")]
        [Description("Цвет индикатора фокуса на вкладке")]
        public Color FocusColor
        {
            get { return _FocusColor; }
            set
            {
                _FocusColor = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет активной кнопки закрытия
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "Black")]
        [Description("Цвет кнопки закрытия при наведении")]
        public Color CloserColorActive
        {
            get { return _CloserColorActive; }
            set
            {
                _CloserColorActive = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет кнопки закрытия
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "DarkGrey")]
        [Description("Цвет кнопки закрытия в обычном состоянии")]
        public Color CloserColor
        {
            get { return _CloserColor; }
            set
            {
                _CloserColor = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает второй цвет индикатора фокуса
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "")]
        [Description("Второй цвет для градиента индикатора фокуса на вкладке")]
        public Color FocusColorSecondary
        {
            get
            {
                if (_FocusColorSecondary.IsEmpty)
                    return SystemColors.ControlLight;
                else
                    return _FocusColorSecondary;
            }
            set
            {
                if (value.Equals(SystemColors.ControlLight))
                    _FocusColorSecondary = Color.Empty;
                else
                    _FocusColorSecondary = value;

                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет фона обычной вкладки
        /// </summary>
        [Category("Цвета фона вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет фона обычной вкладки")]
        public Color BackgroundColor
        {
            get { return _BackgroundColor; }
            set
            {
                _BackgroundColor = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет фона вкладки при наведении
        /// </summary>
        [Category("Цвета фона вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет фона вкладки при наведении мыши")]
        public Color BackgroundColorHot
        {
            get { return _BackgroundColorHot; }
            set
            {
                _BackgroundColorHot = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет фона выбранной вкладки
        /// </summary>
        [Category("Цвета фона вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет фона выбранной вкладки")]
        public Color BackgroundColorSelected
        {
            get { return _BackgroundColorSelected; }
            set
            {
                _BackgroundColorSelected = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет фона отключенной вкладки
        /// </summary>
        [Category("Цвета фона вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет фона отключенной вкладки")]
        public Color BackgroundColorDisabled
        {
            get { return _BackgroundColorDisabled; }
            set
            {
                _BackgroundColorDisabled = value;
                _TabControl.Invalidate();
            }
        }

        /// <summary>
        /// Получает или задает цвет текста при наведении на вкладку
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(Color), "")]
        [Description("Цвет текста вкладки при наведении мыши")]
        public Color TextColorHot
        {
            get
            {
                if (_TextColorHot.IsEmpty)
                    return SystemColors.ControlText;
                else
                    return _TextColorHot;
            }
            set
            {
                if (value.Equals(SystemColors.ControlText))
                    _TextColorHot = Color.Empty;
                else
                    _TextColorHot = value;

                _TabControl.Invalidate();
            }
        }

        #endregion Базовые свойства





        #region Рисование

        /// <summary>
        /// Рисует вкладку
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <param name="graphics">Объект Graphics для рисования</param>
        public void PaintTab(int index, Graphics graphics)
        {
            using (GraphicsPath tabpath = GetTabBorder(index))
            {
                using (Brush fillBrush = GetTabBackgroundBrush(index))
                {
                    // Рисуем фон
                    graphics.FillPath(fillBrush, tabpath);

                    // Рисуем индикатор фокуса
                    if (_TabControl.Focused)
                        DrawTabFocusIndicator(tabpath, index, graphics);

                    // Рисуем кнопку закрытия
                    DrawTabCloser(index, graphics);
                }
            }
        }

        /// <summary>
        /// Рисует кнопку закрытия вкладки
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <param name="graphics">Объект Graphics для рисования</param>
        protected virtual void DrawTabCloser(int index, Graphics graphics)
        {
            if (_ShowTabCloser)
            {
                Rectangle closerRect = _TabControl.GetTabCloserRect(index);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath closerPath = GetCloserPath(closerRect))
                {
                    if (closerRect.Contains(_TabControl.MousePosition))
                    {
                        using (var closerPen = new Pen(_CloserColorActive))
                            graphics.DrawPath(closerPen, closerPath);
                    }
                    else
                    {
                        using (var closerPen = new Pen(_CloserColor))
                            graphics.DrawPath(closerPen, closerPath);
                    }
                }
            }
        }

        /// <summary>
        /// Получает графический путь для кнопки закрытия
        /// </summary>
        /// <param name="closerRect">Прямоугольник кнопки закрытия</param>
        /// <returns>Графический путь кнопки закрытия</returns>
        protected static GraphicsPath GetCloserPath(Rectangle closerRect)
        {
            var closerPath = new GraphicsPath();

            closerPath.AddLine(closerRect.X, closerRect.Y, closerRect.Right, closerRect.Bottom);
            closerPath.CloseFigure();
            closerPath.AddLine(closerRect.Right, closerRect.Y, closerRect.X, closerRect.Bottom);
            closerPath.CloseFigure();

            return closerPath;
        }

        /// <summary>
        /// Рисует индикатор фокуса на вкладке
        /// </summary>
        /// <param name="tabpath">Графический путь вкладки</param>
        /// <param name="index">Индекс вкладки</param>
        /// <param name="graphics">Объект Graphics для рисования</param>
        private void DrawTabFocusIndicator(GraphicsPath tabpath, int index, Graphics graphics)
        {
            if (_FocusTrack && _TabControl.Focused && index == _TabControl.SelectedIndex && !_TabControl.IsMouseDown)
            {
                Brush? focusBrush = null;
                RectangleF pathRect = tabpath.GetBounds();
                Rectangle focusRect = Rectangle.Empty;

                // Используем настраиваемый второй цвет фокуса или значение по умолчанию
                Color secondaryFocusColor = FocusColorSecondary;

                switch (_TabControl.Alignment)
                {
                    case TabAlignment.Top:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, (int)pathRect.Width, 4);
                        focusBrush = new LinearGradientBrush(focusRect, _FocusColor, secondaryFocusColor, LinearGradientMode.Vertical);
                        break;

                    case TabAlignment.Bottom:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Bottom - 4, (int)pathRect.Width, 4);
                        focusBrush = new LinearGradientBrush(focusRect, secondaryFocusColor, _FocusColor, LinearGradientMode.Vertical);
                        break;

                    case TabAlignment.Left:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, 4, (int)pathRect.Height);
                        focusBrush = new LinearGradientBrush(focusRect, _FocusColor, secondaryFocusColor, LinearGradientMode.Horizontal);
                        break;

                    case TabAlignment.Right:
                        focusRect = new Rectangle((int)pathRect.Right - 4, (int)pathRect.Y, 4, (int)pathRect.Height);
                        focusBrush = new LinearGradientBrush(focusRect, secondaryFocusColor, _FocusColor, LinearGradientMode.Horizontal);
                        break;
                }

                // Убеждаемся, что полоса фокуса не выходит за пределы вкладки
                var focusRegion = new Region(focusRect);
                focusRegion.Intersect(tabpath);

                if (focusBrush != null)
                    graphics.FillRegion(focusBrush, focusRegion);

                focusRegion.Dispose();
                focusBrush?.Dispose();
            }
        }

        #endregion Рисование





        #region Кисти фона

        /// <summary>
        /// Получает настройки смешивания для фона
        /// </summary>
        /// <returns>Объект Blend с настройками смешивания</returns>
        private Blend GetBackgroundBlend()
        {
            float[] relativeIntensities = [0f, 0.7f, 1f];
            float[] relativePositions = [0f, 0.6f, 1f];

            // Стеклянный вид для вкладок, выровненных по верху
            if (_TabControl.Alignment == TabAlignment.Top)
            {
                relativeIntensities = [0f, 0.5f, 1f, 1f];
                relativePositions = [0f, 0.5f, 0.51f, 1f];
            }

            var blend = new Blend
            {
                Factors = relativeIntensities,
                Positions = relativePositions
            };

            return blend;
        }

        /// <summary>
        /// Получает кисть для фона страницы вкладки
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Кисть для рисования фона страницы</returns>
        public virtual Brush GetPageBackgroundBrush(int index)
        {
            // Захватываем цвета в зависимости от состояния выбора вкладки
            var light = Color.FromArgb(242, 242, 242);

            if (_TabControl.Alignment == TabAlignment.Top)
                light = Color.FromArgb(207, 207, 207);

            if (_TabControl.SelectedIndex == index)
                light = SystemColors.Window;
            else if (!_TabControl.TabPages[index].Enabled)
                light = Color.FromArgb(207, 207, 207);
            else if (_HotTrack && index == _TabControl.ActiveIndex)
                light = Color.FromArgb(234, 246, 253); // Включаем отслеживание при наведении

            return new SolidBrush(light);
        }

        #endregion Кисти фона





        #region Границы и прямоугольники вкладок

        /// <summary>
        /// Получает графический путь границы вкладки
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Графический путь границы</returns>
        public GraphicsPath GetTabBorder(int index)
        {
            var path = new GraphicsPath();
            Rectangle tabBounds = GetTabRect(index);

            AddTabBorder(path, tabBounds);
            path.CloseFigure();

            return path;
        }

        #endregion Границы и прямоугольники вкладок
    }
}
