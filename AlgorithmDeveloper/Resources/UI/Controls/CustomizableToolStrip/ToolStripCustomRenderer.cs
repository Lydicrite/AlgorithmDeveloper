using System.Drawing.Drawing2D;

namespace AlgorithmDeveloper.Resources.UI.Controls.CustomizableToolStrip
{
    /// <summary>
    /// Кастомный рендерер на базе ToolStripProfessionalRenderer с поддержкой:
    /// <br>- пользовательской окантовки (полосы/рамки) ToolStrip с заданием радиуса и толщины;</br>
    /// <br>- возможности отключить скруглённые блики базового рендера и при желании перекрасить фон.</br>
    /// </summary>
    /// <remarks>
    /// Использует таблицу цветов ProfessionalColorTable (например, CustomizableToolStripColorTable) для всех прочих элементов.
    /// </remarks>
    public sealed class ToolStripCustomRenderer : ToolStripProfessionalRenderer
    {
        #region Параметры окантовки ToolStrip

        /// <summary>
        /// Включить пользовательскую окантовку ToolStrip вместо стандартной.
        /// </summary>
        public bool OutlineEnabled { get; set; } = true;
        /// <summary>
        /// Цвет окантовки ToolStrip.
        /// </summary>
        public Color OutlineColor { get; set; } = Color.Transparent;
        /// <summary>
        /// Толщина окантовки в пикселях.
        /// </summary>
        public int OutlineThickness { get; set; } = 1;
        /// <summary>
        /// Радиус скругления углов окантовки.
        /// </summary>
        public int OutlineCornerRadius { get; set; } = 0;

        #endregion



        #region Базовый фон и скруглённые края

        /// <summary>
        /// Перекрашивать фон ToolStrip вручную, игнорируя стандартные градиенты.
        /// </summary>
        public bool OverrideBackground { get; set; } = true;
        /// <summary>
        /// Отключить «скруглённые блики» стандартного ProfessionalRenderer.
        /// </summary>
        public bool DisableRoundedEdges { get; set; } = true;

        #endregion



        #region Параметры выпадающих меню (DropDown)

        /// <summary>
        /// Включить пользовательскую рамку у выпадающих меню (ToolStripDropDown).
        /// </summary>
        public bool DropDownBorderEnabled { get; set; } = true;
        /// <summary>
        /// Цвет рамки выпадающего меню.
        /// </summary>
        public Color DropDownBorderColor { get; set; } = Color.FromArgb(66, 66, 66);
        /// <summary>
        /// Толщина рамки выпадающего меню.
        /// </summary>
        public int DropDownBorderThickness { get; set; } = 1;
        /// <summary>
        /// Радиус скругления рамки выпадающего меню.
        /// </summary>
        public int DropDownCornerRadius { get; set; } = 0;
        /// <summary>
        /// Перекрашивать фон выпадающего меню собственным цветом.
        /// </summary>
        public bool DropDownOverrideBackground { get; set; } = true;
        /// <summary>
        /// Цвет фона выпадающего меню при включённом <see cref="DropDownOverrideBackground"/>.
        /// </summary>
        public Color DropDownBackColor { get; set; } = Color.FromArgb(46, 46, 46);

        #endregion



        /// <summary>
        /// Создаёт рендерер с указанной таблицей цветов.
        /// </summary>
        /// <param name="table">Таблица цветов ProfessionalColorTable (например, <see cref="CustomizableToolStripColorTable"/>).</param>
        public ToolStripCustomRenderer(ProfessionalColorTable table) : base(table)
        {
            // Отключаем скруглённые края стандартного рендера, если указано
            if (DisableRoundedEdges)
                RoundedEdges = false;
        }

        /// <summary>
        /// Инициализация каждого ToolStrip: отключение скруглённых краёв и настройка выпадающих меню.
        /// </summary>
        /// <param name="toolStrip">Инициализируемый ToolStrip.</param>
        protected override void Initialize(ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);
            // На всякий случай синхронизируем RoundedEdges
            if (DisableRoundedEdges)
                RoundedEdges = false;
        
            // Убираем левую белую колонку ImageMargin/CheckMargin у выпадающих меню
            if (toolStrip is ToolStripDropDownMenu ddm)
            {
                ddm.ShowImageMargin = false;
                ddm.ShowCheckMargin = false;
            }
        }

        /// <summary>
        /// Настройка рамки выпадающих меню.
        /// </summary>
        public ToolStripCustomRenderer WithDropDownBorder(Color color, int thickness = 1, int cornerRadius = 0)
        {
            DropDownBorderEnabled = true;
            DropDownBorderColor = color;
            DropDownBorderThickness = Math.Max(1, thickness);
            DropDownCornerRadius = Math.Max(0, cornerRadius);
            return this;
        }

        /// <summary>
        /// Настройка фона выпадающих меню.
        /// </summary>
        public ToolStripCustomRenderer WithDropDownBackground(Color backColor)
        {
            DropDownOverrideBackground = true;
            DropDownBackColor = backColor;
            return this;
        }





        #region Override-ы событий

        /// <inheritdoc />
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Специальная обработка рамки выпадающего меню
            if (e.ToolStrip is ToolStripDropDown)
            {
                if (DropDownBorderEnabled && DropDownBorderThickness > 0 && DropDownBorderColor.A > 0)
                {
                    var g = e.Graphics;
                    var rect = e.AffectedBounds; rect.Width = Math.Max(0, rect.Width - 1); rect.Height = Math.Max(0, rect.Height - 1);
                    var oldSm = g.SmoothingMode; g.SmoothingMode = SmoothingMode.AntiAlias;
                    using var path = CreateRoundRectPath(rect, DropDownCornerRadius);
                    using var pen = new Pen(DropDownBorderColor, DropDownBorderThickness);
                    g.DrawPath(pen, path);
                    g.SmoothingMode = oldSm;
                }
                else base.OnRenderToolStripBorder(e);
                return;
            }

            if (!OutlineEnabled || OutlineThickness <= 0 || OutlineColor.A == 0) return;
            var g2 = e.Graphics;
            var rect2 = e.AffectedBounds; rect2.Width = Math.Max(0, rect2.Width - 1); rect2.Height = Math.Max(0, rect2.Height - 1);
            var oldSm2 = g2.SmoothingMode; g2.SmoothingMode = SmoothingMode.AntiAlias;
            using var path2 = CreateRoundRectPath(rect2, OutlineCornerRadius);
            using var pen2 = new Pen(OutlineColor, OutlineThickness);
            g2.DrawPath(pen2, path2);
            g2.SmoothingMode = oldSm2;
        }

        /// <inheritdoc />
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            // Перекрашиваем фон выпадающих меню при необходимости
            if (e.ToolStrip is ToolStripDropDown && DropDownOverrideBackground)
            {
                using var b = new SolidBrush(DropDownBackColor);
                e.Graphics.FillRectangle(b, e.AffectedBounds);
                return;
            }

            if (OverrideBackground)
            {
                using var b = new SolidBrush(e.ToolStrip.BackColor);
                e.Graphics.FillRectangle(b, e.AffectedBounds);
                return;
            }
            base.OnRenderToolStripBackground(e);
        }

        /// <summary>
        /// Перерисовка левой области ImageMargin у выпадающих меню, чтобы она соответствовала фону и не была белой.
        /// </summary>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is ToolStripDropDown)
            {
                var rect = e.AffectedBounds;
                using var b = new SolidBrush(DropDownOverrideBackground ? DropDownBackColor : e.ToolStrip.BackColor);
                e.Graphics.FillRectangle(b, rect);
                return;
            }
            base.OnRenderImageMargin(e);
        }

        /// <inheritdoc />
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            var item = e.Item;
            var table = ColorTable as CustomizableToolStripColorTable;
            if (table != null)
            {
                bool enabled = item.Enabled;
                bool isMenuItem = item is ToolStripMenuItem;
                bool isChecked = isMenuItem && ((ToolStripMenuItem)item).Checked && enabled;
                bool isPressed = (item.Pressed || isMenuItem && (((ToolStripMenuItem)item).Pressed || ((ToolStripMenuItem)item).DropDown?.Visible == true)) && enabled;
                bool isSelected = item.Selected && enabled && !isPressed && !isChecked;

                if (!enabled)
                    e.TextColor = table.DisabledTextColor;
                else if (isPressed)
                    e.TextColor = table.PressedTextColor;
                else if (isChecked)
                    e.TextColor = table.CheckedTextColor;
                else if (isSelected)
                    e.TextColor = table.HoverTextColor;
                else
                    e.TextColor = table.TextColor;
            }
            base.OnRenderItemText(e);
        }

        /// <inheritdoc />
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            var table = ColorTable as CustomizableToolStripColorTable;
            if (table != null && e.Item != null)
            {
                var item = e.Item;
                bool enabled = item.Enabled;
                bool isMenuItem = item is ToolStripMenuItem;
                bool isChecked = isMenuItem && ((ToolStripMenuItem)item).Checked && enabled;
                bool isPressed = (item.Pressed || isMenuItem && (((ToolStripMenuItem)item).Pressed || ((ToolStripMenuItem)item).DropDown?.Visible == true)) && enabled;
                bool isSelected = item.Selected && enabled && !isPressed && !isChecked;

                Color arrowColor;
                if (!enabled)
                    arrowColor = table.DisabledArrowColor;
                else if (isPressed)
                    arrowColor = table.PressedArrowColor;
                else if (isSelected)
                    arrowColor = table.HoverArrowColor;
                else
                    arrowColor = table.ArrowColor;

                e.ArrowColor = arrowColor;
            }
            base.OnRenderArrow(e);
        }

        private static GraphicsPath CreateRoundRectPath(Rectangle bounds, int radius)
        {
            int r = Math.Min(Math.Max(0, radius), Math.Min(bounds.Width, bounds.Height) / 2);
            var path = new GraphicsPath();
            if (r <= 0) { path.AddRectangle(bounds); path.CloseFigure(); return path; }
            int d = r * 2; var arc = new Rectangle(bounds.X, bounds.Y, d, d);
            path.AddArc(arc, 180, 90); arc.X = bounds.Right - d; path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - d; path.AddArc(arc, 0, 90); arc.X = bounds.Left; path.AddArc(arc, 90, 90);
            path.CloseFigure(); return path;
        }

        #endregion
    }
}
