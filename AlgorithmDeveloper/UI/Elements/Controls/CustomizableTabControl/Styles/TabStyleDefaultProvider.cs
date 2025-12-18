using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AlgorithmDeveloper.UI.Elements.Controls.CustomizableTabControl.Styles
{
    /// <summary>
    /// Провайдер стандартного стиля для вкладок
    /// </summary>
    public class TabStyleDefaultProvider : TabStyleProvider
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса TabStyleDefaultProvider
        /// </summary>
        /// <param name="tabControl">Контрол вкладок для применения стиля</param>
        public TabStyleDefaultProvider(CustomizableTabControl tabControl) : base(tabControl)
        {
            _FocusTrack = true;
            _Radius = 2;
        }

        /// <summary>
        /// Добавляет границу вкладки к графическому пути для стандартного стиля
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
        /// Получает прямоугольник вкладки с учетом особенностей стандартного стиля
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Прямоугольник вкладки</returns>
        public override Rectangle GetTabRect(int index)
        {
            if (index < 0)
                return new Rectangle();

            Rectangle tabBounds = base.GetTabRect(index);
            bool firstTabinRow = _TabControl.IsFirstTabInRow(index);

            // Делаем невыбранные вкладки меньше, а выбранные больше
            if (index != _TabControl.SelectedIndex)
            {
                switch (_TabControl.Alignment)
                {
                    case TabAlignment.Top:
                        tabBounds.Y += 1;
                        tabBounds.Height -= 1;
                        break;

                    case TabAlignment.Bottom:
                        tabBounds.Height -= 1;
                        break;

                    case TabAlignment.Left:
                        tabBounds.X += 1;
                        tabBounds.Width -= 1;
                        break;

                    case TabAlignment.Right:
                        tabBounds.Width -= 1;
                        break;
                }
            }
            else
            {
                switch (_TabControl.Alignment)
                {
                    case TabAlignment.Top:
                        if (tabBounds.Y > 0)
                        {
                            tabBounds.Y -= 1;
                            tabBounds.Height += 1;
                        }

                        if (firstTabinRow)
                            tabBounds.Width += 1;
                        else
                        {
                            tabBounds.X -= 1;
                            tabBounds.Width += 2;
                        }
                        break;

                    case TabAlignment.Bottom:
                        if (tabBounds.Bottom < _TabControl.Bottom)
                            tabBounds.Height += 1;

                        if (firstTabinRow)
                            tabBounds.Width += 1;
                        else
                        {
                            tabBounds.X -= 1;
                            tabBounds.Width += 2;
                        }
                        break;

                    case TabAlignment.Left:
                        if (tabBounds.X > 0)
                        {
                            tabBounds.X -= 1;
                            tabBounds.Width += 1;
                        }

                        if (firstTabinRow)
                            tabBounds.Height += 1;
                        else
                        {
                            tabBounds.Y -= 1;
                            tabBounds.Height += 2;
                        }
                        break;

                    case TabAlignment.Right:
                        if (tabBounds.Right < _TabControl.Right)
                            tabBounds.Width += 1;

                        if (firstTabinRow)
                            tabBounds.Height += 1;
                        else
                        {
                            tabBounds.Y -= 1;
                            tabBounds.Height += 2;
                        }
                        break;
                }
            }

            // Корректируем первую вкладку в ряду для выравнивания с табпейджем
            EnsureFirstTabIsInView(ref tabBounds, index);

            return tabBounds;
        }
    }
}
