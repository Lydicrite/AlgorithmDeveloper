using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmDeveloper.UI.Elements.Controls.CustomizableToolStrip
{
    /// <summary>
    /// Профессиональная таблица цветов для рендерера ToolStrip/MenuStrip, позволяющая тонко управлять
    /// цветами подсветки (Hover/Pressed/Checked) и рамок без необходимости писать собственный рендерер.
    /// Подходит тем, кто хочет остаться на стандартном ToolStripProfessionalRenderer, но со своими цветами.
    /// </summary>
    public sealed class CustomizableToolStripColorTable : ProfessionalColorTable
    {
        /// <summary>
        /// Цвет фона при наведении.
        /// </summary>
        public Color HoverBackColor { get; set; } = Color.FromArgb(61, 61, 61);
        /// <summary>
        /// Цвет рамки при наведении.
        /// </summary>
        public Color HoverBorderColor { get; set; } = Color.FromArgb(112, 112, 112);
        /// <summary>
        /// Второй цвет фона при наведении (используется при градиенте). Если не задан, используется <see cref="HoverBackColor"/>.
        /// </summary>
        public Color? HoverBackColor2 { get; set; }



        /// <summary>
        /// Цвет фона при нажатии.
        /// </summary>
        public Color PressedBackColor { get; set; } = Color.FromArgb(46, 46, 46);
        /// <summary>
        /// Цвет рамки при нажатии.
        /// </summary>
        public Color PressedBorderColor { get; set; } = Color.FromArgb(66, 66, 66);
        /// <summary>
        /// Второй цвет фона при нажатии (используется при градиенте). Если не задан, используется <see cref="PressedBackColor"/>.
        /// </summary>
        public Color? PressedBackColor2 { get; set; }



        /// <summary>
        /// Цвет фона в состоянии Checked.
        /// </summary>
        public Color CheckedBackColor { get; set; } = Color.Transparent;
        /// <summary>
        /// Цвет рамки в состоянии Checked.
        /// </summary>
        public Color CheckedBorderColor { get; set; } = Color.Transparent;
        /// <summary>
        /// Второй цвет фона в состоянии Checked (используется при градиенте). Если не задан, используется <see cref="CheckedBackColor"/>.
        /// </summary>
        public Color? CheckedBackColor2 { get; set; }



        /// <summary>
        /// Базовый цвет текста элементов (обычное состояние).
        /// </summary>
        public Color TextColor { get; set; } = Color.FromArgb(214, 214, 214);
        /// <summary>
        /// Цвет текста при наведении.
        /// </summary>
        public Color HoverTextColor { get; set; } = Color.FromArgb(250, 250, 250);
        /// <summary>
        /// Цвет текста при нажатии.
        /// </summary>
        public Color PressedTextColor { get; set; } = Color.FromArgb(214, 214, 214);
        /// <summary>
        /// Цвет текста для отмеченных (Checked) элементов.
        /// </summary>
        public Color CheckedTextColor { get; set; } = Color.FromArgb(214, 214, 214);
        /// <summary>
        /// Цвет текста для отключённых (Disabled) элементов.
        /// </summary>
        public Color DisabledTextColor { get; set; } = SystemColors.ControlDarkDark;



        /// <summary>
        /// Цвет треугольников (стрелок) у DropDownButton и ToolStripMenuItem в обычном состоянии.
        /// </summary>
        public Color ArrowColor { get; set; } = Color.FromArgb(214, 214, 214);
        /// <summary>
        /// Цвет треугольников (стрелок) при наведении.
        /// </summary>
        public Color HoverArrowColor { get; set; } = Color.FromArgb(250, 250, 250);
        /// <summary>
        /// Цвет треугольников (стрелок) при нажатии.
        /// </summary>
        public Color PressedArrowColor { get; set; } = Color.FromArgb(214, 214, 214);
        /// <summary>
        /// Цвет треугольников (стрелок) для отключённых элементов.
        /// </summary>
        public Color DisabledArrowColor { get; set; } = SystemColors.ControlDarkDark;



        /// <summary>
        /// Цвет сепараторов (разделителей) в ToolStrip.
        /// </summary>
        public Color SeparatorColor { get; set; } = Color.FromArgb(112, 112, 112);
        /// <summary>
        /// Цвет светлой части сепаратора (для создания эффекта объёма).
        /// </summary>
        public Color SeparatorLightColor { get; set; } = Color.FromArgb(80, 80, 80);



        /// <summary>
        /// Цвет границы DropDownButton в состоянии, когда открыто DropDown-меню.
        /// </summary>
        public Color DropDownOpenBorderColor { get; set; } = Color.FromArgb(128, 128, 128);



        /// <summary>
        /// Создаёт таблицу цветов с настройками по умолчанию.
        /// </summary>
        public CustomizableToolStripColorTable() { }



        /// <summary>
        /// Устанавливает цвета для состояния Hover.
        /// </summary>
        public CustomizableToolStripColorTable WithHoverColors(Color back, Color border)
        {
            HoverBackColor = back;
            HoverBorderColor = border;
            return this;
        }

        /// <summary>
        /// Устанавливает цвета для состояния Hover с градиентом (Begin/End).
        /// </summary>
        public CustomizableToolStripColorTable WithHoverColors(Color backBegin, Color backEnd, Color border)
        {
            HoverBackColor = backBegin;
            HoverBackColor2 = backEnd;
            HoverBorderColor = border;
            return this;
        }

        /// <summary>
        /// Устанавливает цвета для состояния Pressed.
        /// </summary>
        public CustomizableToolStripColorTable WithPressedColors(Color back, Color border)
        {
            PressedBackColor = back;
            PressedBorderColor = border;
            return this;
        }

        /// <summary>
        /// Устанавливает цвета для состояния Pressed с градиентом (Begin/End).
        /// </summary>
        public CustomizableToolStripColorTable WithPressedColors(Color backBegin, Color backEnd, Color border)
        {
            PressedBackColor = backBegin;
            PressedBackColor2 = backEnd;
            PressedBorderColor = border;
            return this;
        }

        /// <summary>
        /// Устанавливает цвета для состояния Checked.
        /// </summary>
        public CustomizableToolStripColorTable WithCheckedColors(Color back, Color border)
        {
            CheckedBackColor = back;
            CheckedBorderColor = border;
            return this;
        }

        /// <summary>
        /// Устанавливает цвета для состояния Checked с градиентом (Begin/End).
        /// </summary>
        public CustomizableToolStripColorTable WithCheckedColors(Color backBegin, Color backEnd, Color border)
        {
            CheckedBackColor = backBegin;
            CheckedBackColor2 = backEnd;
            CheckedBorderColor = border;
            return this;
        }



        public CustomizableToolStripColorTable WithHoverGradient(Color begin, Color end)
        {
            HoverBackColor = begin;
            HoverBackColor2 = end;
            return this;
        }

        public CustomizableToolStripColorTable WithPressedGradient(Color begin, Color end)
        {
            PressedBackColor = begin;
            PressedBackColor2 = end;
            return this;
        }

        public CustomizableToolStripColorTable WithCheckedGradient(Color begin, Color end)
        {
            CheckedBackColor = begin;
            CheckedBackColor2 = end;
            return this;
        }

        public CustomizableToolStripColorTable WithTextColors(Color normal, Color hover, Color pressed, Color @checked, Color disabled)
        {
            TextColor = normal;
            HoverTextColor = hover;
            PressedTextColor = pressed;
            CheckedTextColor = @checked;
            DisabledTextColor = disabled;
            return this;
        }



        /// <summary>
        /// Устанавливает цвета треугольников (стрелок) для всех состояний.
        /// </summary>
        public CustomizableToolStripColorTable WithArrowColors(Color normal, Color hover, Color pressed, Color disabled)
        {
            ArrowColor = normal;
            HoverArrowColor = hover;
            PressedArrowColor = pressed;
            DisabledArrowColor = disabled;
            return this;
        }

        /// <summary>
        /// Устанавливает цвета сепараторов.
        /// </summary>
        public CustomizableToolStripColorTable WithSeparatorColors(Color dark, Color light)
        {
            SeparatorColor = dark;
            SeparatorLightColor = light;
            return this;
        }

        /// <summary>
        /// Устанавливает цвет границы DropDown в открытом состоянии.
        /// </summary>
        public CustomizableToolStripColorTable WithDropDownOpenBorder(Color color)
        {
            DropDownOpenBorderColor = color;
            return this;
        }

        /// <summary>
        /// Пресет с мягкими голубыми фонами и системным акцентом рамки.
        /// </summary>
        public static CustomizableToolStripColorTable AccentWindows() => new();

        /// <summary>
        /// Пресет с более контрастным Hover/Pressed.
        /// </summary>
        public static CustomizableToolStripColorTable LightBlueStrong() => new()
        {
            HoverBackColor = Color.FromArgb(220, 240, 255),
            HoverBorderColor = Color.FromArgb(30, 144, 255),
            PressedBackColor = Color.FromArgb(200, 230, 255),
            PressedBorderColor = Color.FromArgb(0, 120, 215),
            CheckedBackColor = Color.FromArgb(224, 238, 255),
            CheckedBorderColor = Color.FromArgb(86, 156, 214)
        };





        #region Переопределения для MenuStrip/ToolStrip/Buttons

        // Menu items (hover)
        public override Color MenuItemSelected => HoverBackColor;
        public override Color MenuItemSelectedGradientBegin => HoverBackColor;
        public override Color MenuItemSelectedGradientEnd => HoverBackColor2 ?? HoverBackColor;

        // Menu items (pressed)
        public override Color MenuItemPressedGradientBegin => PressedBackColor;
        public override Color MenuItemPressedGradientEnd => PressedBackColor2 ?? PressedBackColor;

        // Menu item borders
        public override Color MenuItemBorder => HoverBorderColor;

        // Buttons (hover)
        public override Color ButtonSelectedHighlight => HoverBackColor;
        public override Color ButtonSelectedGradientBegin => HoverBackColor;
        public override Color ButtonSelectedGradientEnd => HoverBackColor2 ?? HoverBackColor;
        public override Color ButtonSelectedBorder => HoverBorderColor;
        public override Color ButtonSelectedHighlightBorder => HoverBorderColor;

        // Buttons (pressed)
        public override Color ButtonPressedHighlight => PressedBackColor;
        public override Color ButtonPressedGradientBegin => PressedBackColor;
        public override Color ButtonPressedGradientEnd => PressedBackColor2 ?? PressedBackColor;
        public override Color ButtonPressedBorder => PressedBorderColor;
        public override Color ButtonPressedHighlightBorder => PressedBorderColor;

        // Checked visuals
        public override Color CheckBackground => CheckedBackColor;
        public override Color CheckSelectedBackground => CheckedBackColor;
        public override Color CheckPressedBackground => PressedBackColor;
        public override Color ButtonCheckedHighlightBorder => CheckedBorderColor;

        // Опционально: слегка тонируем тулбары, чтобы фон меню не конфликтовал
        public override Color ToolStripGradientBegin => SystemColors.Control;
        public override Color ToolStripGradientMiddle => SystemColors.Control;
        public override Color ToolStripGradientEnd => SystemColors.Control;
        public override Color ToolStripBorder => SystemColors.ControlDark;

        // Сепараторы
        public override Color SeparatorDark => SeparatorColor;
        public override Color SeparatorLight => SeparatorLightColor;

        #endregion
    }
}
