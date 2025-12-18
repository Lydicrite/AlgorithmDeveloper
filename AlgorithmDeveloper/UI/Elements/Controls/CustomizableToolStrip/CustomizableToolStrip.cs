using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmDeveloper.UI.Elements.Controls.CustomizableToolStrip
{
    /// <summary>
    /// Кастомный ToolStrip с интегрированным ToolStripCustomRenderer.
    /// Предоставляет все свойства рендерера для редактирования в Visual Studio Designer.
    /// </summary>
    [ToolboxItem(true)]
    [Description("Кастомный ToolStrip с расширенными возможностями рендеринга")]
    public partial class CustomizableToolStrip : ToolStrip
    {
        private ToolStripCustomRenderer? _customRenderer;
        private CustomizableToolStripColorTable? _colorTable;

        #region Конструкторы

        /// <summary>
        /// Инициализирует новый экземпляр CustomizableToolStrip.
        /// </summary>
        public CustomizableToolStrip()
        {
            InitializeComponent();
            InitializeCustomRenderer();
        }

        /// <summary>
        /// Инициализирует новый экземпляр CustomizableToolStrip с указанными элементами.
        /// </summary>
        /// <param name="items">Элементы для добавления в ToolStrip.</param>
        public CustomizableToolStrip(params ToolStripItem[] items) : base(items)
        {
            InitializeComponent();
            InitializeCustomRenderer();
        }

        #endregion

        #region Инициализация

        private void InitializeComponent()
        {
            // Базовые настройки
            SuspendLayout();
            AutoSize = false;
            GripStyle = ToolStripGripStyle.Hidden;
            ResumeLayout(false);
        }

        private void InitializeCustomRenderer()
        {
            _colorTable = new CustomizableToolStripColorTable();
            _customRenderer = new ToolStripCustomRenderer(_colorTable);
            Renderer = _customRenderer;

            // Применяем цвета текста к существующим элементам
            ApplyTextColorsToItems();
        }

        #endregion





        #region Свойства окантовки ToolStrip

        /// <summary>
        /// Включить пользовательскую окантовку ToolStrip вместо стандартной.
        /// </summary>
        [Category("Appearance")]
        [Description("Включить пользовательскую окантовку ToolStrip вместо стандартной")]
        [DefaultValue(true)]
        public bool OutlineEnabled
        {
            get => _customRenderer?.OutlineEnabled ?? true;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.OutlineEnabled = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет окантовки ToolStrip.
        /// </summary>
        [Category("Appearance")]
        [Description("Цвет окантовки ToolStrip")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color OutlineColor
        {
            get => _customRenderer?.OutlineColor ?? SystemColors.ControlDark;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.OutlineColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Толщина окантовки в пикселях.
        /// </summary>
        [Category("Appearance")]
        [Description("Толщина окантовки в пикселях")]
        [DefaultValue(1)]
        public int OutlineThickness
        {
            get => _customRenderer?.OutlineThickness ?? 1;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.OutlineThickness = Math.Max(0, value);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Радиус скругления углов окантовки.
        /// </summary>
        [Category("Appearance")]
        [Description("Радиус скругления углов окантовки")]
        [DefaultValue(3)]
        public int OutlineCornerRadius
        {
            get => _customRenderer?.OutlineCornerRadius ?? 3;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.OutlineCornerRadius = Math.Max(0, value);
                    Invalidate();
                }
            }
        }

        #endregion





        #region Свойства базового фона и скруглённых краёв

        /// <summary>
        /// Перекрашивать фон ToolStrip вручную, игнорируя стандартные градиенты.
        /// </summary>
        [Category("Appearance")]
        [Description("Перекрашивать фон ToolStrip вручную, игнорируя стандартные градиенты")]
        [DefaultValue(false)]
        public bool OverrideBackground
        {
            get => _customRenderer?.OverrideBackground ?? false;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.OverrideBackground = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Отключить «скруглённые края» стандартного ProfessionalRenderer.
        /// </summary>
        [Category("Appearance")]
        [Description("Отключить «скруглённые края» стандартного ProfessionalRenderer")]
        [DefaultValue(true)]
        public bool DisableRoundedEdges
        {
            get => _customRenderer?.DisableRoundedEdges ?? true;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.DisableRoundedEdges = value;
                    _customRenderer.RoundedEdges = !value;
                    Invalidate();
                }
            }
        }

        #endregion





        #region Свойства выпадающих меню (DropDown)

        /// <summary>
        /// Включить пользовательскую рамку у выпадающих меню (ToolStripDropDown).
        /// </summary>
        [Category("DropDown")]
        [Description("Включить пользовательскую рамку у выпадающих меню")]
        [DefaultValue(false)]
        public bool DropDownBorderEnabled
        {
            get => _customRenderer?.DropDownBorderEnabled ?? false;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.DropDownBorderEnabled = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет рамки выпадающего меню.
        /// </summary>
        [Category("DropDown")]
        [Description("Цвет рамки выпадающего меню")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DropDownBorderColor
        {
            get => _customRenderer?.DropDownBorderColor ?? Color.Gray;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.DropDownBorderColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Толщина рамки выпадающего меню.
        /// </summary>
        [Category("DropDown")]
        [Description("Толщина рамки выпадающего меню")]
        [DefaultValue(1)]
        public int DropDownBorderThickness
        {
            get => _customRenderer?.DropDownBorderThickness ?? 1;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.DropDownBorderThickness = Math.Max(0, value);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Радиус скругления рамки выпадающего меню.
        /// </summary>
        [Category("DropDown")]
        [Description("Радиус скругления рамки выпадающего меню")]
        [DefaultValue(2)]
        public int DropDownCornerRadius
        {
            get => _customRenderer?.DropDownCornerRadius ?? 2;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.DropDownCornerRadius = Math.Max(0, value);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Перекрашивать фон выпадающего меню собственным цветом.
        /// </summary>
        [Category("DropDown")]
        [Description("Перекрашивать фон выпадающего меню собственным цветом")]
        [DefaultValue(false)]
        public bool DropDownOverrideBackground
        {
            get => _customRenderer?.DropDownOverrideBackground ?? false;
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.DropDownOverrideBackground = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет фона выпадающего меню при включённом DropDownOverrideBackground.
        /// </summary>
        [Category("DropDown")]
        [Description("Цвет фона выпадающего меню при включённом DropDownOverrideBackground")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DropDownBackColor
        {
            get => _customRenderer?.DropDownBackColor ?? Color.FromArgb(45, 45, 48);
            set
            {
                if (_customRenderer != null)
                {
                    _customRenderer.DropDownBackColor = value;
                    Invalidate();
                }
            }
        }

        #endregion





        #region Свойства цветовой таблицы

        /// <summary>
        /// Цвет фона при наведении.
        /// </summary>
        [Category("Colors")]
        [Description("Цвет фона при наведении")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverBackColor
        {
            get => _colorTable?.HoverBackColor ?? Color.FromArgb(230, 245, 255);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.HoverBackColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет рамки при наведении.
        /// </summary>
        [Category("Colors")]
        [Description("Цвет рамки при наведении")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverBorderColor
        {
            get => _colorTable?.HoverBorderColor ?? Color.FromArgb(86, 156, 214);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.HoverBorderColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет фона при нажатии.
        /// </summary>
        [Category("Colors")]
        [Description("Цвет фона при нажатии")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PressedBackColor
        {
            get => _colorTable?.PressedBackColor ?? Color.FromArgb(204, 232, 255);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.PressedBackColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет рамки при нажатии.
        /// </summary>
        [Category("Colors")]
        [Description("Цвет рамки при нажатии")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PressedBorderColor
        {
            get => _colorTable?.PressedBorderColor ?? Color.FromArgb(51, 153, 255);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.PressedBorderColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет фона для отмеченных элементов.
        /// </summary>
        [Category("Colors")]
        [Description("Цвет фона для отмеченных элементов")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CheckedBackColor
        {
            get => _colorTable?.CheckedBackColor ?? Color.FromArgb(255, 240, 240);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.CheckedBackColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет рамки для отмеченных элементов.
        /// </summary>
        [Category("Colors")]
        [Description("Цвет рамки для отмеченных элементов")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CheckedBorderColor
        {
            get => _colorTable?.CheckedBorderColor ?? Color.FromArgb(229, 195, 101);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.CheckedBorderColor = value;
                    Invalidate();
                }
            }
        }

        #endregion





        #region Свойства градиентных цветов

        /// <summary>
        /// Второй цвет фона при наведении для создания градиента.
        /// </summary>
        [Category("Colors")]
        [Description("Второй цвет фона при наведении для создания градиента")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color? HoverBackColor2
        {
            get => _colorTable?.HoverBackColor2;
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.HoverBackColor2 = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Второй цвет фона при нажатии для создания градиента.
        /// </summary>
        [Category("Colors")]
        [Description("Второй цвет фона при нажатии для создания градиента")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color? PressedBackColor2
        {
            get => _colorTable?.PressedBackColor2;
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.PressedBackColor2 = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Второй цвет фона для отмеченных элементов для создания градиента.
        /// </summary>
        [Category("Colors")]
        [Description("Второй цвет фона для отмеченных элементов для создания градиента")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color? CheckedBackColor2
        {
            get => _colorTable?.CheckedBackColor2;
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.CheckedBackColor2 = value;
                    Invalidate();
                }
            }
        }

        #endregion





        #region Свойства цветов текста

        /// <summary>
        /// Базовый цвет текста элементов (обычное состояние).
        /// </summary>
        [Category("Text Colors")]
        [Description("Базовый цвет текста элементов (обычное состояние)")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TextColor
        {
            get => _colorTable?.TextColor ?? SystemColors.ControlText;
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.TextColor = value;
                    ApplyTextColorsToItems();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет текста при наведении.
        /// </summary>
        [Category("Text Colors")]
        [Description("Цвет текста при наведении")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverTextColor
        {
            get => _colorTable?.HoverTextColor ?? SystemColors.ControlText;
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.HoverTextColor = value;
                    ApplyTextColorsToItems();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет текста при нажатии.
        /// </summary>
        [Category("Text Colors")]
        [Description("Цвет текста при нажатии")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PressedTextColor
        {
            get => _colorTable?.PressedTextColor ?? SystemColors.ControlText;
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.PressedTextColor = value;
                    ApplyTextColorsToItems();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет текста для отмеченных (Checked) элементов.
        /// </summary>
        [Category("Text Colors")]
        [Description("Цвет текста для отмеченных (Checked) элементов")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CheckedTextColor
        {
            get => _colorTable?.CheckedTextColor ?? SystemColors.ControlText;
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.CheckedTextColor = value;
                    ApplyTextColorsToItems();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет текста для отключённых (Disabled) элементов.
        /// </summary>
        [Category("Text Colors")]
        [Description("Цвет текста для отключённых (Disabled) элементов")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DisabledTextColor
        {
            get => _colorTable?.DisabledTextColor ?? SystemColors.GrayText;
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.DisabledTextColor = value;
                    ApplyTextColorsToItems();
                    Invalidate();
                }
            }
        }

        #endregion





        #region Свойства цветов треугольников (стрелок)

        /// <summary>
        /// Цвет треугольников (стрелок) у DropDownButton и ToolStripMenuItem в обычном состоянии.
        /// </summary>
        [Category("Arrow Colors")]
        [Description("Цвет треугольников (стрелок) у DropDownButton и ToolStripMenuItem в обычном состоянии")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ArrowColor
        {
            get => _colorTable?.ArrowColor ?? Color.FromArgb(214, 214, 214);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.ArrowColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет треугольников (стрелок) при наведении.
        /// </summary>
        [Category("Arrow Colors")]
        [Description("Цвет треугольников (стрелок) при наведении")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverArrowColor
        {
            get => _colorTable?.HoverArrowColor ?? Color.FromArgb(250, 250, 250);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.HoverArrowColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет треугольников (стрелок) при нажатии.
        /// </summary>
        [Category("Arrow Colors")]
        [Description("Цвет треугольников (стрелок) при нажатии")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PressedArrowColor
        {
            get => _colorTable?.PressedArrowColor ?? Color.FromArgb(214, 214, 214);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.PressedArrowColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет треугольников (стрелок) для отключённых элементов.
        /// </summary>
        [Category("Arrow Colors")]
        [Description("Цвет треугольников (стрелок) для отключённых элементов")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DisabledArrowColor
        {
            get => _colorTable?.DisabledArrowColor ?? SystemColors.ControlDarkDark;
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.DisabledArrowColor = value;
                    Invalidate();
                }
            }
        }

        #endregion





        #region Свойства цветов сепараторов

        /// <summary>
        /// Цвет сепараторов (разделителей) в ToolStrip.
        /// </summary>
        [Category("Separator Colors")]
        [Description("Цвет сепараторов (разделителей) в ToolStrip")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SeparatorColor
        {
            get => _colorTable?.SeparatorColor ?? Color.FromArgb(112, 112, 112);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.SeparatorColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет светлой части сепаратора (для создания эффекта объёма).
        /// </summary>
        [Category("Separator Colors")]
        [Description("Цвет светлой части сепаратора (для создания эффекта объёма)")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SeparatorLightColor
        {
            get => _colorTable?.SeparatorLightColor ?? Color.FromArgb(80, 80, 80);
            set
            {
                if (_colorTable != null)
                {
                    _colorTable.SeparatorLightColor = value;
                    Invalidate();
                }
            }
        }

        #endregion





        #region Методы

        /// <summary>
        /// Настройка рамки выпадающих меню.
        /// </summary>
        /// <param name="color">Цвет рамки</param>
        /// <param name="thickness">Толщина рамки</param>
        /// <param name="cornerRadius">Радиус скругления</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public CustomizableToolStrip WithDropDownBorder(Color color, int thickness = 1, int cornerRadius = 2)
        {
            _customRenderer?.WithDropDownBorder(color, thickness, cornerRadius);
            Invalidate();
            return this;
        }

        /// <summary>
        /// Настройка фона выпадающих меню.
        /// </summary>
        /// <param name="backColor">Цвет фона</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public CustomizableToolStrip WithDropDownBackground(Color backColor)
        {
            _customRenderer?.WithDropDownBackground(backColor);
            Invalidate();
            return this;
        }

        /// <summary>
        /// Получить доступ к внутреннему рендереру для дополнительной настройки.
        /// </summary>
        /// <returns>Экземпляр ToolStripCustomRenderer</returns>
        public ToolStripCustomRenderer GetRenderer()
        {
            return _customRenderer ?? throw new InvalidOperationException("Renderer не инициализирован");
        }

        /// <summary>
        /// Получить доступ к цветовой таблице для дополнительной настройки.
        /// </summary>
        /// <returns>Экземпляр CustomizableToolStripColorTable</returns>
        public CustomizableToolStripColorTable GetColorTable()
        {
            return _colorTable ?? throw new InvalidOperationException("ColorTable не инициализирована");
        }

        /// <summary>
        /// Применяет цвета текста ко всем дочерним элементам ToolStrip.
        /// </summary>
        private void ApplyTextColorsToItems()
        {
            if (_colorTable == null) return;

            foreach (ToolStripItem item in Items)
            {
                ApplyTextColorToItem(item);
            }
        }

        /// <summary>
        /// Применяет цвета текста к конкретному элементу ToolStrip.
        /// </summary>
        /// <param name="item">Элемент для применения цветов</param>
        private void ApplyTextColorToItem(ToolStripItem item)
        {
            if (_colorTable == null || item == null) return;

            // Применяем базовый цвет текста
            item.ForeColor = _colorTable.TextColor;

            // Для выпадающих элементов применяем цвета к подэлементам
            if (item is ToolStripDropDownItem dropDownItem)
            {
                foreach (ToolStripItem subItem in dropDownItem.DropDownItems)
                {
                    ApplyTextColorToItem(subItem);
                }
            }
        }

        /// <summary>
        /// Настройка градиентных цветов для состояния Hover.
        /// </summary>
        /// <param name="beginColor">Начальный цвет градиента</param>
        /// <param name="endColor">Конечный цвет градиента</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public CustomizableToolStrip WithHoverGradient(Color beginColor, Color endColor)
        {
            if (_colorTable != null)
            {
                _colorTable.HoverBackColor = beginColor;
                _colorTable.HoverBackColor2 = endColor;
                Invalidate();
            }
            return this;
        }

        /// <summary>
        /// Настройка градиентных цветов для состояния Pressed.
        /// </summary>
        /// <param name="beginColor">Начальный цвет градиента</param>
        /// <param name="endColor">Конечный цвет градиента</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public CustomizableToolStrip WithPressedGradient(Color beginColor, Color endColor)
        {
            if (_colorTable != null)
            {
                _colorTable.PressedBackColor = beginColor;
                _colorTable.PressedBackColor2 = endColor;
                Invalidate();
            }
            return this;
        }

        /// <summary>
        /// Настройка градиентных цветов для состояния Checked.
        /// </summary>
        /// <param name="beginColor">Начальный цвет градиента</param>
        /// <param name="endColor">Конечный цвет градиента</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public CustomizableToolStrip WithCheckedGradient(Color beginColor, Color endColor)
        {
            if (_colorTable != null)
            {
                _colorTable.CheckedBackColor = beginColor;
                _colorTable.CheckedBackColor2 = endColor;
                Invalidate();
            }
            return this;
        }

        /// <summary>
        /// Настройка всех цветов текста одновременно.
        /// </summary>
        /// <param name="normal">Цвет текста в обычном состоянии</param>
        /// <param name="hover">Цвет текста при наведении</param>
        /// <param name="pressed">Цвет текста при нажатии</param>
        /// <param name="checked">Цвет текста для отмеченных элементов</param>
        /// <param name="disabled">Цвет текста для отключённых элементов</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public CustomizableToolStrip WithTextColors(Color normal, Color hover, Color pressed, Color @checked, Color disabled)
        {
            if (_colorTable != null)
            {
                _colorTable.TextColor = normal;
                _colorTable.HoverTextColor = hover;
                _colorTable.PressedTextColor = pressed;
                _colorTable.CheckedTextColor = @checked;
                _colorTable.DisabledTextColor = disabled;
                ApplyTextColorsToItems();
                Invalidate();
            }
            return this;
        }

        /// <summary>
        /// Настройка цветов треугольников (стрелок) для всех состояний.
        /// </summary>
        /// <param name="normal">Цвет треугольников в обычном состоянии</param>
        /// <param name="hover">Цвет треугольников при наведении</param>
        /// <param name="pressed">Цвет треугольников при нажатии</param>
        /// <param name="disabled">Цвет треугольников для отключённых элементов</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public CustomizableToolStrip WithArrowColors(Color normal, Color hover, Color pressed, Color disabled)
        {
            if (_colorTable != null)
            {
                _colorTable.ArrowColor = normal;
                _colorTable.HoverArrowColor = hover;
                _colorTable.PressedArrowColor = pressed;
                _colorTable.DisabledArrowColor = disabled;
                Invalidate();
            }
            return this;
        }

        /// <summary>
        /// Настройка цветов сепараторов.
        /// </summary>
        /// <param name="dark">Цвет тёмной части сепаратора</param>
        /// <param name="light">Цвет светлой части сепаратора</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public CustomizableToolStrip WithSeparatorColors(Color dark, Color light)
        {
            if (_colorTable != null)
            {
                _colorTable.SeparatorColor = dark;
                _colorTable.SeparatorLightColor = light;
                Invalidate();
            }
            return this;
        }

        /// <summary>
        /// Настройка цвета границы DropDown в открытом состоянии.
        /// </summary>
        /// <param name="color">Цвет границы</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public CustomizableToolStrip WithDropDownOpenBorder(Color color)
        {
            if (_colorTable != null)
            {
                _colorTable.DropDownOpenBorderColor = color;
                Invalidate();
            }
            return this;
        }

        #endregion





        #region Переопределения

        /// <summary>
        /// Переопределение для автоматического применения цветов к новым элементам.
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            base.OnItemAdded(e);

            // Применяем цвета к новому элементу
            if (e.Item != null)
            {
                ApplyTextColorToItem(e.Item);
            }
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        /// <param name="disposing">Освобождать ли управляемые ресурсы</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // ToolStripCustomRenderer не реализует IDisposable
                _customRenderer = null;
                _colorTable = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
