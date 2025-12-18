using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AlgorithmDeveloper.UI.Elements.Controls.CustomizableTabControl.Styles
{
    /// <summary>
    /// Провайдер темного стиля для вкладок
    /// </summary>
    public class TabStyleDarkProvider : TabStyleProvider
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса TabStyleDarkProvider
        /// </summary>
        /// <param name="tabControl">Контрол вкладок для применения стиля</param>
        public TabStyleDarkProvider(CustomizableTabControl tabControl) : base(tabControl)
        {
            _BorderColor = Color.FromArgb(66, 66, 66);
            _BorderColorHot = Color.FromArgb(172, 172, 172);
            _BorderColorSelected = Color.FromArgb(250, 250, 250);
            _TextColor = Color.FromArgb(214, 214, 214);
            _TextColorSelected = Color.FromArgb(250, 250, 250);
            _TextColorDisabled = Color.FromArgb(128, 128, 128);
            _CloserColor = Color.FromArgb(255, 58, 58);
            _CloserColorActive = Color.FromArgb(255, 0,0);

            _HotTrack = true;
            _FocusTrack = true;

            _Radius = 10;
            _Overlap = 0;
            _Opacity = 1F;

            _Padding = new Point(6, 3);
        }

        /// <summary>
        /// Получает кисть для фона вкладки в темном стиле
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Кисть для рисования фона</returns>
        protected override Brush GetTabBackgroundBrush(int index)
        {
            Rectangle tabRect = GetTabRect(index);
            Color backgroundColor;

            // Определяем цвет фона в зависимости от состояния вкладки
            if (_TabControl.SelectedIndex == index)
            {
                // Выбранная вкладка
                backgroundColor = !_BackgroundColorSelected.IsEmpty ? _BackgroundColorSelected : Color.FromArgb(31, 31, 31);
            }
            else if (!_TabControl.TabPages[index].Enabled)
            {
                // Отключенная вкладка
                backgroundColor = !_BackgroundColorDisabled.IsEmpty ? _BackgroundColorDisabled : Color.FromArgb(24, 24, 24);
            }
            else if (_HotTrack && index == _TabControl.ActiveIndex)
            {
                // Вкладка при наведении
                backgroundColor = !_BackgroundColorHot.IsEmpty ? _BackgroundColorHot : Color.FromArgb(60, 60, 60);
            }
            else
            {
                // Обычная вкладка
                backgroundColor = !_BackgroundColor.IsEmpty ? _BackgroundColor : Color.FromArgb(48, 48, 48);
            }

            return new LinearGradientBrush(tabRect, backgroundColor, backgroundColor, LinearGradientMode.Vertical);
        }

        /// <summary>
        /// Добавляет границу вкладки к графическому пути для темного стиля
        /// </summary>
        /// <param name="path">Графический путь для добавления границы</param>
        /// <param name="tabBounds">Границы вкладки</param>
        public override void AddTabBorder(GraphicsPath path, Rectangle tabBounds)
        {
            switch (_TabControl.Alignment)
            {
                case TabAlignment.Top:
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    break;

                case TabAlignment.Bottom:
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    break;

                case TabAlignment.Left:
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    break;

                case TabAlignment.Right:
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    break;
            }
        }

        /// <summary>
        /// Получает прямоугольник вкладки с учетом особенностей темного стиля
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Прямоугольник вкладки</returns>
        public override Rectangle GetTabRect(int index)
        {
            if (index < 0)
                return new Rectangle();

            Rectangle tabBounds = base.GetTabRect(index);

            switch (_TabControl.Alignment)
            {
                case TabAlignment.Top:
                    tabBounds.Height += 1;
                    break;

                case TabAlignment.Bottom:
                    tabBounds = new Rectangle(tabBounds.Location.X, tabBounds.Location.Y - 1, tabBounds.Width, tabBounds.Height + 1);
                    break;
            }

            EnsureFirstTabIsInView(ref tabBounds, index);

            return tabBounds;
        }
    }
}
