using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl.Styles
{
    /// <summary>
    /// Провайдер стиля для скрытых вкладок (без отображения)
    /// </summary>
    public class TabStyleNoneProvider : TabStyleProvider
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса TabStyleNoneProvider
        /// </summary>
        /// <param name="tabControl">Контрол вкладок для применения стиля</param>
        public TabStyleNoneProvider(CustomTabControl tabControl) : base(tabControl)
        {
            // Ничего не делаем - вкладки скрыты
        }

        /// <summary>
        /// Добавляет границу вкладки к графическому пути (не реализовано для скрытого стиля)
        /// </summary>
        /// <param name="path">Графический путь для добавления границы</param>
        /// <param name="tabBounds">Границы вкладки</param>
        public override void AddTabBorder(GraphicsPath path, Rectangle tabBounds)
        {
            // Ничего не делаем - вкладки скрыты
        }
    }
}
