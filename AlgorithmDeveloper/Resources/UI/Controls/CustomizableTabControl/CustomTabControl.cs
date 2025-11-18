using AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl.Styles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl
{
    /// <summary>
    /// Настраиваемый контрол вкладок с расширенными возможностями кастомизации
    /// </summary>
    [ToolboxBitmap(typeof(TabControl))]
        [ProvideProperty("ForbidHide", typeof(TabPage))]
        [ProvideProperty("ForbidClose", typeof(TabPage))]
        [ProvideProperty("ForbidEntry", typeof(TabPage))]
        public class CustomTabControl : TabControl, IExtenderProvider
    {
        #region Конструкция

        /// <summary>
        /// Инициализирует новый экземпляр класса CustomTabControl
        /// </summary>
        public CustomTabControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw, true);

            _BackBuffer = new Bitmap(Width, Height);
            _BackBufferGraphics = Graphics.FromImage(_BackBuffer);
            _TabBuffer = new Bitmap(Width, Height);
            _TabBufferGraphics = Graphics.FromImage(_TabBuffer);

            DisplayStyle = TabStyle.Default;
        }

        /// <summary>
        /// Вызывается при создании контрола
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            OnFontChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Получает параметры создания окна
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                if (RightToLeftLayout)
                    cp.ExStyle = cp.ExStyle | NativeUIUtils.WS_EX_LAYOUTRTL | NativeUIUtils.WS_EX_NOINHERITLAYOUT;

                return cp;
            }
        }

        /// <summary>
        /// Освобождает ресурсы, используемые контролом
        /// </summary>
        /// <param name="disposing">true для освобождения управляемых и неуправляемых ресурсов; false только для неуправляемых</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _BackImage?.Dispose();
                _BackBufferGraphics?.Dispose();
                _BackBuffer?.Dispose();
                _TabBufferGraphics?.Dispose();
                _TabBuffer?.Dispose();
            }
        }

        #endregion Конструкция





        #region Приватные переменные

        /// <summary>
        /// Изображение фона
        /// </summary>
        private Bitmap? _BackImage;
        /// <summary>
        /// Буфер фона
        /// </summary>
        private Bitmap _BackBuffer;
        /// <summary>
        /// Графический контекст буфера фона
        /// </summary>
        private Graphics _BackBufferGraphics;
        /// <summary>
        /// Буфер вкладок
        /// </summary>
        private Bitmap _TabBuffer;
        /// <summary>
        /// Графический контекст буфера вкладок
        /// </summary>
        private Graphics _TabBufferGraphics;
        /// <summary>
        /// Предыдущее значение прокрутки
        /// </summary>
        private int _OldValue;
        /// <summary>
        /// Текущий стиль отображения
        /// </summary>
        private TabStyle _Style;
        /// <summary>
        /// Провайдер стиля
        /// </summary>
        private TabStyleProvider? _StyleProvider;
        /// <summary>
        /// Список всех страниц вкладок (включая скрытые)
        /// </summary>
        private List<TabPage>? _TabPages;
        /// <summary>
        /// Вкладка, на которую был произведен клик
        /// </summary>
        private TabPage? _ClickedTabPage; 
        /// <summary>
        /// Последняя позиция мыши
        /// </summary>
        private Point _LastMousePosition;

        // Extender storage for per-TabPage permissions
        private readonly Dictionary<TabPage, bool> _ForbidHide = new();
        private readonly Dictionary<TabPage, bool> _ForbidClose = new();
        private readonly Dictionary<TabPage, bool> _ForbidEntry = new();

        // Events fired when permissions change for a specific TabPage
        public event EventHandler<TabControlEventArgs>? TabHidePermissionChanged;
        public event EventHandler<TabControlEventArgs>? TabClosePermissionChanged;
        public event EventHandler<TabControlEventArgs>? TabEntryPermissionChanged;

        #endregion Приватные переменные





        #region Публичные свойства

        [Browsable(false)]
        public bool IsMouseDown { get; private set; }

        // IExtenderProvider implementation and extender properties
        public bool CanExtend(object extendee)
        {
            return extendee is TabPage page && page.Parent == this;
        }

        [Category("Поведение"), Description("Запрещает скрытие вкладки"), DefaultValue(false)]
        public bool GetForbidHide(TabPage page)
        {
            if (page == null) return false;
            return _ForbidHide.TryGetValue(page, out var v) ? v : false;
        }

        public void SetForbidHide(TabPage page, bool value)
        {
            if (page == null) return;
            bool old = GetForbidHide(page);
            if (old == value) return;
            _ForbidHide[page] = value;
            int index = TabPages.IndexOf(page);
            TabHidePermissionChanged?.Invoke(this, new TabControlEventArgs(page, index, TabControlAction.Selected));
            Invalidate();
        }

        [Category("Поведение"), Description("Запрещает закрытие вкладки"), DefaultValue(false)]
        public bool GetForbidClose(TabPage page)
        {
            if (page == null) return false;
            return _ForbidClose.TryGetValue(page, out var v) ? v : false;
        }

        public void SetForbidClose(TabPage page, bool value)
        {
            if (page == null) return;
            bool old = GetForbidClose(page);
            if (old == value) return;
            _ForbidClose[page] = value;
            int index = TabPages.IndexOf(page);
            TabClosePermissionChanged?.Invoke(this, new TabControlEventArgs(page, index, TabControlAction.Selected));
            Invalidate();
        }

        [Category("Поведение"), Description("Запрещает создание управляющего элемента TabEntryControl для вкладки"), DefaultValue(false)]
        public bool GetForbidEntry(TabPage page)
        {
            if (page == null) return false;
            return _ForbidEntry.TryGetValue(page, out var v) ? v : false;
        }

        public void SetForbidEntry(TabPage page, bool value)
        {
            if (page == null) return;
            bool old = GetForbidEntry(page);
            if (old == value) return;
            _ForbidEntry[page] = value;
            int index = TabPages.IndexOf(page);
            TabEntryPermissionChanged?.Invoke(this, new TabControlEventArgs(page, index, TabControlAction.Selected));
            Invalidate();
        }

        /// <summary>
        /// Получает или задает провайдер стиля отображения
        /// </summary>
        [Category("Внешний вид"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Провайдер, определяющий внешний вид вкладок")]
        public TabStyleProvider DisplayStyleProvider
        {
            get
            {
                if (_StyleProvider == null)
                    DisplayStyle = TabStyle.Default;

                return _StyleProvider!;
            }
            set { _StyleProvider = value; }
        }


        /// <summary>
        /// Получает или задает стиль отображения вкладок
        /// </summary>
        [Category("Внешний вид"), DefaultValue(typeof(TabStyle), "Default"), RefreshProperties(RefreshProperties.All)]
        [Description("Определяет стиль отображения вкладок")]
        public TabStyle DisplayStyle
        {
            get { return _Style; }
            set
            {
                if (_Style != value)
                {
                    _Style = value;
                    _StyleProvider = TabStyleProvider.CreateProvider(this);
                    Invalidate();
                }
            }
        }


        /// <summary>
        /// Получает или задает возможность многострочного отображения вкладок
        /// </summary>
        [Category("Внешний вид"), RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Определяет, могут ли вкладки отображаться в несколько строк")]
        public new bool Multiline
        {
            get { return base.Multiline; }
            set
            {
                base.Multiline = value;
                Invalidate();
            }
        }


        // Скрываем атрибут Padding, чтобы его нельзя было изменить.
        // Мы обрабатываем это в провайдере стиля.
        /// <summary>
        /// Получает или задает отступы (управляется провайдером стиля)
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Point Padding
        {
            get { return DisplayStyleProvider.Padding; }
            set { DisplayStyleProvider.Padding = value; }
        }


        /// <summary>
        /// Получает или задает макет справа налево
        /// </summary>
        public override bool RightToLeftLayout
        {
            get { return base.RightToLeftLayout; }
            set
            {
                base.RightToLeftLayout = value;
                UpdateStyles();
            }
        }


        // Скрываем атрибут HotTrack, чтобы его нельзя было изменить.
        // Мы обрабатываем это в провайдере стиля.
        /// <summary>
        /// Получает или задает отслеживание при наведении (управляется провайдером стиля)
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool HotTrack
        {
            get { return DisplayStyleProvider.HotTrack; }
            set { DisplayStyleProvider.HotTrack = value; }
        }


        /// <summary>
        /// Получает или задает выравнивание вкладок
        /// </summary>
        [Category("Внешний вид")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Определяет расположение вкладок относительно содержимого")]
        public new TabAlignment Alignment
        {
            get { return base.Alignment; }
            set
            {
                base.Alignment = value;

                switch (value)
                {
                    case TabAlignment.Top:
                    case TabAlignment.Bottom:
                        Multiline = false;
                        break;

                    case TabAlignment.Left:
                    case TabAlignment.Right:
                        Multiline = true;
                        break;
                }
            }
        }


        // Скрываем атрибут Appearance, чтобы его нельзя было изменить.
        // Мы не хотим этого, так как делаем всю отрисовку сами.
        /// <summary>
        /// Получает или задает внешний вид (фиксировано как Normal)
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TabAppearance Appearance
        {
            get { return base.Appearance; }
            set
            {
                // Не разрешаем установку других внешних видов, так как мы делаем всю отрисовку сами
                base.Appearance = TabAppearance.Normal;
            }
        }


        /// <summary>
        /// Получает прямоугольник отображения содержимого
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                // Специальная обработка для скрытия вкладок
                if (_Style == TabStyle.None)
                    return new Rectangle(0, 0, Width, Height);
                else
                {
                    int itemHeight;

                    if (Alignment <= TabAlignment.Bottom)
                        itemHeight = ItemSize.Height;
                    else
                        itemHeight = ItemSize.Width;

                    int tabStripHeight = 5 + (itemHeight * RowCount);
                    var rect = new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);

                    switch (Alignment)
                    {
                        case TabAlignment.Top:
                            rect = new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
                            break;

                        case TabAlignment.Bottom:
                            rect = new Rectangle(4, 4, Width - 8, Height - tabStripHeight - 4);
                            break;

                        case TabAlignment.Left:
                            rect = new Rectangle(tabStripHeight, 4, Width - tabStripHeight - 4, Height - 8);
                            break;

                        case TabAlignment.Right:
                            rect = new Rectangle(4, 4, Width - tabStripHeight - 4, Height - 8);
                            break;
                    }

                    return rect;
                }
            }
        }


        /// <summary>
        /// Получает индекс активной вкладки (под курсором мыши)
        /// </summary>
        [Browsable(false)]
        public int ActiveIndex
        {
            get
            {
                var hitTestInfo = new NativeUIUtils.TCHITTESTINFO(PointToClient(Control.MousePosition));
                int index = NativeUIUtils.SendMessage(Handle, NativeUIUtils.TCM_HITTEST, IntPtr.Zero, NativeUIUtils.ToIntPtr(hitTestInfo)).ToInt32();

                if (index == -1)
                    return -1;
                else
                {
                    if (TabPages[index].Enabled)
                        return index;
                    else
                        return -1;
                }
            }
        }


        /// <summary>
        /// Получает активную вкладку (под курсором мыши)
        /// </summary>
        [Browsable(false)]
        public TabPage? ActiveTab
        {
            get
            {
                int activeIndex = ActiveIndex;

                if (activeIndex > -1)
                    return TabPages[activeIndex];
                else
                    return null;
            }
        }


        /// <summary>
        /// Получает или задает возможность закрытия вкладок средней кнопкой мыши
        /// </summary>
        [Category("Поведение"), DefaultValue(false), RefreshProperties(RefreshProperties.All)]
        [Description("Определяет, можно ли закрывать вкладки средней кнопкой мыши")]
        public bool EnableMiddleClickTabClosing { get; set; }

        #endregion Публичные свойства





        #region Методы расширения

        /// <summary>
        /// Закрывает указанную вкладку
        /// </summary>
        /// <param name="page">Страница вкладки для закрытия</param>
        public void CloseTab(TabPage page)
        {
            // Allow closing both visible and hidden tabs
            if (page == null)
                return;

            // Блокируем закрытие, если свойство ForbidClose установлено
            if (GetForbidClose(page))
                return;

            bool isVisible = TabPages.Contains(page);
            int index = isVisible ? TabPages.IndexOf(page) : (_TabPages != null ? _TabPages.IndexOf(page) : -1);

            var args = new TabControlCancelEventArgs(page, index, false, TabControlAction.Deselecting);
            OnTabClosing(args);

            if (!args.Cancel)
            {
                if (isVisible)
                {
                    TabPages.Remove(page);
                }

                // Dispose even if the tab is currently hidden
                try { page.Dispose(); } catch { /* ignore */ }

                // Cleanup permission dictionaries and backup storage
                _ForbidHide.Remove(page);
                _ForbidClose.Remove(page);
                _ForbidEntry.Remove(page);
                if (_TabPages != null)
                    _TabPages.Remove(page);
            }
        }

        /// <summary>
        /// Закрывает вкладку по индексу
        /// </summary>
        /// <param name="index">Индекс вкладки для закрытия</param>
        public void CloseTab(int index)
        {
            if (IsValidTabIndex(index))
                CloseTab(_TabPages![index]);
        }

        /// <summary>
        /// Закрывает вкладку по ключу
        /// </summary>
        /// <param name="key">Ключ вкладки для закрытия</param>
        public void CloseTab(string key)
        {
            if (TabPages.ContainsKey(key))
            {
                CloseTab(TabPages[key]!);
            }
            else
            {
                // Attempt to close hidden tab by key via backup
                BackupTabPages();
                if (_TabPages != null)
                {
                    TabPage? tab = _TabPages.Find(page => page.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                    if (tab != null)
                        CloseTab(tab);
                }
            }
        }

        /// <summary>
        /// Скрывает указанную вкладку
        /// </summary>
        /// <param name="page">Страница вкладки для скрытия</param>
        public void HideTab(TabPage page)
        {
            if (page != null && TabPages.Contains(page))
            {
                // Блокируем скрытие, если свойство ForbidHide установлено
                if (GetForbidHide(page))
                    return;

                int index = TabPages.IndexOf(page);
                BackupTabPages();
                TabPages.Remove(page);
                OnTabHidden(new TabControlEventArgs(page, index, TabControlAction.Deselected));
            }
        }

        /// <summary>
        /// Скрывает вкладку по индексу
        /// </summary>
        /// <param name="index">Индекс вкладки для скрытия</param>
        public void HideTab(int index)
        {
            if (IsValidTabIndex(index))
                HideTab(_TabPages![index]);
        }

        /// <summary>
        /// Скрывает вкладку по ключу
        /// </summary>
        /// <param name="key">Ключ вкладки для скрытия</param>
        public void HideTab(string key)
        {
            if (TabPages.ContainsKey(key))
                HideTab(TabPages[key]!);
        }

        /// <summary>
        /// Показывает указанную вкладку
        /// </summary>
        /// <param name="page">Страница вкладки для отображения</param>
        public void ShowTab(TabPage page)
        {
            if (page != null)
            {
                // Ensure backup exists and is in sync before showing
                BackupTabPages();

                if (_TabPages != null)
                {
                    if (!TabPages.Contains(page))
                    {
                        if (_TabPages.Contains(page))
                        {
                            // Get insert point from backup of pages
                            int pageIndex = _TabPages.IndexOf(page);

                            if (pageIndex > 0)
                            {
                                int start = pageIndex - 1;

                                // Check for presence of earlier pages in the visible tabs
                                for (int index = start; index >= 0; index--)
                                {
                                    if (TabPages.Contains(_TabPages[index]))
                                    {
                                        // Set insert point to the Right of the last present tab
                                        pageIndex = TabPages.IndexOf(_TabPages[index]) + 1;
                                        break;
                                    }
                                }
                            }

                            // Insert the page, or add to the end
                            if ((pageIndex >= 0) && (pageIndex < TabPages.Count))
                                TabPages.Insert(pageIndex, page);
                            else
                                TabPages.Add(page);
                        }
                        else
                        {
                            // Page was added after the initial backup; add it and sync backup
                            TabPages.Add(page);
                            _TabPages.Add(page);
                        }
                    }
                }
                else
                {
                    // If the page is not found at all then just add it
                    if (!TabPages.Contains(page))
                        TabPages.Add(page);
                }

                if (TabPages.Contains(page))
                {
                    int idx = TabPages.IndexOf(page);
                    OnTabShown(new TabControlEventArgs(page, idx, TabControlAction.Selected));
                }
            }
        }

        /// <summary>
        /// Показывает вкладку по индексу
        /// </summary>
        /// <param name="index">Индекс вкладки для отображения</param>
        public void ShowTab(int index)
        {
            if (IsValidTabIndex(index))
                ShowTab(_TabPages![index]);
        }

        /// <summary>
        /// Показывает вкладку по ключу
        /// </summary>
        /// <param name="key">Ключ вкладки для отображения</param>
        public void ShowTab(string key)
        {
            if (_TabPages != null)
            {
                TabPage? tab = _TabPages.Find(page => page.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                ShowTab(tab!);
            }
        }

        /// <summary>
        /// Проверяет, является ли индекс вкладки допустимым
        /// </summary>
        /// <param name="index">Индекс для проверки</param>
        /// <returns>true, если индекс допустим; иначе false</returns>
        private bool IsValidTabIndex(int index)
        {
            BackupTabPages();
            return (index >= 0) && (index < _TabPages!.Count);
        }

        /// <summary>
        /// Создает резервную копию списка страниц вкладок
        /// </summary>
        private void BackupTabPages()
        {
            if (_TabPages == null)
            {
                _TabPages = new List<TabPage>();

                foreach (TabPage page in TabPages)
                    _TabPages.Add(page);
            }
            else
            {
                // Keep backup in sync: add newly added pages that aren't tracked yet
                foreach (TabPage page in TabPages)
                {
                    if (!_TabPages.Contains(page))
                        _TabPages.Add(page);
                }

                // Only remove pages that have been disposed (closed)
                for (int i = _TabPages.Count - 1; i >= 0; i--)
                {
                    var candidate = _TabPages[i];
                    if (candidate.IsDisposed)
                        _TabPages.RemoveAt(i);
                }
            }
        }

        #endregion Методы расширения





        #region Перетаскивание

        /// <summary>
        /// Обрабатывает нажатие кнопки мыши для начала перетаскивания
        /// </summary>
        /// <param name="e">Аргументы события мыши</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            IsMouseDown = true;
            Invalidate();

            if (AllowDrop)
            {
                int clickedTabIndex = GetHoveredTabIndex();

                if (clickedTabIndex >= 0)
                    _ClickedTabPage = TabPages[clickedTabIndex];
            }
        }

        /// <summary>
        /// Обрабатывает отпускание кнопки мыши для завершения перетаскивания
        /// </summary>
        /// <param name="e">Аргументы события мыши</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            IsMouseDown = false;
            _ClickedTabPage = null;
            Invalidate();
        }

        /// <summary>
        /// Обрабатывает движение мыши для перетаскивания и обновления кнопки закрытия
        /// </summary>
        /// <param name="e">Аргументы события мыши</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_StyleProvider!.ShowTabCloser)
            {
                Rectangle tabRect = _StyleProvider.GetTabRect(ActiveIndex);

                if (tabRect.Contains(MousePosition))
                    Invalidate();
            }

            if (AllowDrop)
            {
                // Порог перетаскивания. 5 кажется хорошим числом здесь.
                if (Math.Abs(e.Location.X - _LastMousePosition.X) < 5)
                    return;

                _LastMousePosition = e.Location;

                // Нажата ли левая кнопка мыши? Была ли нажата какая-либо вкладка?
                if ((e.Button != MouseButtons.Left) || (_ClickedTabPage == null))
                    return;

                // Начинаем перетаскивание
                DoDragDrop(_ClickedTabPage, DragDropEffects.All);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseDown = false;
            Invalidate();
        }

        /// <summary>
        /// Обрабатывает событие перетаскивания над контролом
        /// </summary>
        /// <param name="e">Аргументы события перетаскивания</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (AllowDrop)
                HandleDragDrop(e);
        }

        /// <summary>
        /// Обрабатывает логику перетаскивания вкладок
        /// </summary>
        /// <param name="e">Аргументы события перетаскивания</param>
        private void HandleDragDrop(DragEventArgs e)
        {
            // Перетаскивается ли вкладка?
            if (e.Data!.GetData(typeof(TabPage)) == null)
                return;

            var draggedTab = (TabPage)e.Data.GetData(typeof(TabPage))!;
            int draggedTabIndex = TabPages.IndexOf(draggedTab);
            int hoveredTabIndex = GetHoveredTabIndex();

            if (hoveredTabIndex < 0)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            TabPage hoveredTab = TabPages[hoveredTabIndex];
            e.Effect = DragDropEffects.Move;

            // Перетаскивание еще не выполнено
            if (draggedTab == hoveredTab)
                return;

            // Меняем местами перетаскиваемую и целевую вкладки - избегаем переключения
            Rectangle draggedTabRect = GetTabRect(draggedTabIndex);
            Rectangle hoveredTabRect = GetTabRect(hoveredTabIndex);

            if (draggedTabRect.Width < hoveredTabRect.Width)
            {
                Point tcLocation = PointToScreen(Location);

                if (draggedTabIndex < hoveredTabIndex)
                {
                    if ((e.X - tcLocation.X) > ((hoveredTabRect.X + hoveredTabRect.Width) - draggedTabRect.Width))
                        SwapTabPages(draggedTab, hoveredTab);
                }
                else if (draggedTabIndex > hoveredTabIndex)
                {
                    if ((e.X - tcLocation.X) < (hoveredTabRect.X + draggedTabRect.Width))
                        SwapTabPages(draggedTab, hoveredTab);
                }
            }
            else
                SwapTabPages(draggedTab, hoveredTab);

            // Выбираем новую позицию перетащенной вкладки
            SelectedIndex = TabPages.IndexOf(draggedTab);
        }

        /// <summary>
        /// Получает индекс вкладки, над которой находится курсор мыши
        /// </summary>
        /// <returns>Индекс вкладки или -1, если курсор не над вкладкой</returns>
        private int GetHoveredTabIndex()
        {
            for (int i = 0; i < TabPages.Count; i++)
            {
                if (GetTabRect(i).Contains(PointToClient(Cursor.Position)))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Меняет местами две вкладки
        /// </summary>
        /// <param name="src">Исходная вкладка</param>
        /// <param name="dst">Целевая вкладка</param>
        private void SwapTabPages(TabPage src, TabPage dst)
        {
            int index_src = TabPages.IndexOf(src);
            int index_dst = TabPages.IndexOf(dst);

            TabPages[index_dst] = src;
            TabPages[index_src] = dst;

            Invalidate();
        }

        #endregion Перетаскивание





        #region События

        /// <summary>
        /// Происходит при горизонтальной прокрутке вкладок
        /// </summary>
        [Category("Действие")]
        [Description("Происходит при горизонтальной прокрутке вкладок")]
        public event ScrollEventHandler? HScroll;

        /// <summary>
        /// Происходит при клике на изображение вкладки
        /// </summary>
        [Category("Действие")]
        [Description("Происходит при клике на изображение вкладки")]
        public event EventHandler<TabControlEventArgs>? TabImageClick;

        /// <summary>
        /// Происходит при закрытии вкладки
        /// </summary>
        [Category("Действие")]
        [Description("Происходит при попытке закрытия вкладки")]
        public event EventHandler<TabControlCancelEventArgs>? TabClosing;

        /// <summary>
        /// Происходит при скрытии вкладки
        /// </summary>
        [Category("Действие")]
        [Description("Происходит при скрытии вкладки")]
        public event EventHandler<TabControlEventArgs>? TabHidden;

        /// <summary>
        /// Происходит при отображении вкладки
        /// </summary>
        [Category("Действие")]
        [Description("Происходит при отображении вкладки")]
        public event EventHandler<TabControlEventArgs>? TabShown;

        #endregion События





        #region Обработка событий базового класса

        /// <summary>
        /// Обрабатывает изменение шрифта контрола
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected override void OnFontChanged(EventArgs e)
        {
            IntPtr hFont = Font.ToHfont();

            NativeUIUtils.SendMessage(Handle, NativeUIUtils.WM_SETFONT, hFont, (IntPtr)(-1));
            NativeUIUtils.SendMessage(Handle, NativeUIUtils.WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);

            UpdateStyles();

            if (Visible)
                Invalidate();
        }

        /// <summary>
        /// Обрабатывает изменение размера контрола
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected override void OnResize(EventArgs e)
        {
            // Пересоздаем буфер для ручной двойной буферизации
            if (Width > 0 && Height > 0)
            {
                _BackImage?.Dispose();
                _BackImage = null;

                _BackBufferGraphics?.Dispose();
                _BackBuffer?.Dispose();

                _BackBuffer = new Bitmap(Width, Height);
                _BackBufferGraphics = Graphics.FromImage(_BackBuffer);

                _TabBufferGraphics?.Dispose();
                _TabBuffer?.Dispose();

                _TabBuffer = new Bitmap(Width, Height);
                _TabBufferGraphics = Graphics.FromImage(_TabBuffer);
            }

            base.OnResize(e);
        }

        /// <summary>
        /// Обрабатывает изменение цвета фона родительского контрола
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected override void OnParentBackColorChanged(EventArgs e)
        {
            _BackImage?.Dispose();
            _BackImage = null;

            base.OnParentBackColorChanged(e);
        }

        /// <summary>
        /// Обрабатывает изменение фонового изображения родительского контрола
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected override void OnParentBackgroundImageChanged(EventArgs e)
        {
            _BackImage?.Dispose();
            _BackImage = null;

            base.OnParentBackgroundImageChanged(e);
        }

        /// <summary>
        /// Обрабатывает изменение размера родительского контрола
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private void OnParentResize(object sender, EventArgs e)
        {
            if (Visible)
                Invalidate();
        }

        /// <summary>
        /// Обрабатывает изменение родительского контрола
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent != null)
                Parent.Resize += OnParentResize!;
        }

        /// <summary>
        /// Обрабатывает выбор вкладки
        /// </summary>
        /// <param name="e">Аргументы события отмены выбора вкладки</param>
        protected override void OnSelecting(TabControlCancelEventArgs e)
        {
            base.OnSelecting(e);

            // Не разрешаем выбор отключенных вкладок
            if (e.Action == TabControlAction.Selecting && e.TabPage != null && !e.TabPage.Enabled)
                e.Cancel = true;
        }

        /// <summary>
        /// Обрабатывает перемещение контрола
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected override void OnMove(EventArgs e)
        {
            if (Width > 0 && Height > 0)
            {
                _BackImage?.Dispose();
                _BackImage = null;
            }

            base.OnMove(e);
            Invalidate();
        }

        /// <summary>
        /// Обрабатывает добавление дочернего контрола
        /// </summary>
        /// <param name="e">Аргументы события контрола</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (Visible)
                Invalidate();
        }

        /// <summary>
        /// Обрабатывает удаление дочернего контрола
        /// </summary>
        /// <param name="e">Аргументы события контрола</param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            if (Visible)
                Invalidate();
        }

        /// <summary>
        /// Обрабатывает мнемонические клавиши для переключения вкладок
        /// </summary>
        /// <param name="charCode">Код символа мнемоники</param>
        /// <returns>true, если мнемоника была обработана; иначе false</returns>
        protected override bool ProcessMnemonic(char charCode)
        {
            foreach (TabPage page in TabPages)
            {
                if (IsMnemonic(charCode, page.Text))
                {
                    SelectedTab = page;
                    return true;
                }
            }

            return base.ProcessMnemonic(charCode);
        }

        /// <summary>
        /// Обрабатывает сообщения Windows
        /// </summary>
        /// <param name="m">Сообщение Windows для обработки</param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeUIUtils.WM_HSCROLL:
                    // Вызываем событие прокрутки при прокрутке скроллера
                    base.WndProc(ref m);
                    OnHScroll(new ScrollEventArgs((ScrollEventType)NativeUIUtils.LoWord(m.WParam), _OldValue, NativeUIUtils.HiWord(m.WParam), ScrollOrientation.HorizontalScroll));
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// Обрабатывает клик мыши по контролу
        /// </summary>
        /// <param name="e">Аргументы события мыши</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            int index = ActiveIndex;

            // Если мы кликаем по изображению, то вызываем событие ImageClicked перед стандартным событием клика мыши
            // если есть обработчик.
            if (index > -1 && TabImageClick != null
                && (TabPages[index].ImageIndex > -1 || !string.IsNullOrEmpty(TabPages[index].ImageKey))
                && GetTabImageRect(index).Contains(MousePosition))
            {
                OnTabImageClick(new TabControlEventArgs(TabPages[index], index, TabControlAction.Selected));
                base.OnMouseClick(e); // Вызываем базовое событие
            }
            else if (!DesignMode && index > -1 && _StyleProvider!.ShowTabCloser && GetTabCloserRect(index).Contains(MousePosition)
                || e.Button == MouseButtons.Middle && EnableMiddleClickTabClosing)
            {
                // Если мы кликаем по кнопке закрытия, то удаляем вкладку вместо вызова стандартного события клика мыши
                // но сначала вызываем событие закрытия вкладки
                CloseTab(ActiveTab);
            }
            else
                base.OnMouseClick(e); // Вызываем базовое событие

            Invalidate();
        }

        /// <summary>
        /// Вызывает событие клика по изображению вкладки
        /// </summary>
        /// <param name="e">Аргументы события вкладки</param>
        protected virtual void OnTabImageClick(TabControlEventArgs e)
        {
            TabImageClick?.Invoke(this, e);
        }

        /// <summary>
        /// Вызывает событие закрытия вкладки
        /// </summary>
        /// <param name="e">Аргументы события отмены закрытия вкладки</param>
        protected virtual void OnTabClosing(TabControlCancelEventArgs e)
        {
            TabClosing?.Invoke(this, e);
        }

        /// <summary>
        /// Вызывает событие скрытия вкладки
        /// </summary>
        /// <param name="e">Аргументы события вкладки</param>
        protected virtual void OnTabHidden(TabControlEventArgs e)
        {
            TabHidden?.Invoke(this, e);
        }

        /// <summary>
        /// Вызывает событие отображения вкладки
        /// </summary>
        /// <param name="e">Аргументы события вкладки</param>
        protected virtual void OnTabShown(TabControlEventArgs e)
        {
            TabShown?.Invoke(this, e);
        }

        /// <summary>
        /// Вызывает событие горизонтальной прокрутки
        /// </summary>
        /// <param name="e">Аргументы события прокрутки</param>
        protected virtual void OnHScroll(ScrollEventArgs e)
        {
            // Перерисовываем перемещенные вкладки
            Invalidate();

            // Вызываем событие
            HScroll?.Invoke(this, e);

            if (e.Type == ScrollEventType.EndScroll)
                _OldValue = e.NewValue;
        }

        #endregion Обработка событий базового класса





        #region Базовые методы рисования

        /// <summary>
        /// Обрабатывает событие перерисовки контрола
        /// </summary>
        /// <param name="e">Аргументы события рисования</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            CustomPaint(e.Graphics);
        }

        /// <summary>
        /// Выполняет пользовательскую отрисовку контрола
        /// </summary>
        /// <param name="screenGraphics">Графический контекст экрана</param>
        private void CustomPaint(Graphics screenGraphics)
        {
            // Мы рендерим в битмап, который затем рисуется за один раз, а не используем
            // двойную буферизацию, встроенную в контрол, так как встроенная буферизация
            // нарушает отрисовку фона.
            // Аналогично объект BufferedGraphics из .Net 2.0 вызывает нарушение отрисовки фона,
            // поэтому мы используем эту технику буферизации из .Net 1.1.

            if (Width > 0 && Height > 0)
            {
                if (_BackImage == null)
                {
                    // Cached Background Image
                    _BackImage = new Bitmap(Width, Height);

                    var backGraphics = Graphics.FromImage(_BackImage);
                    backGraphics.Clear(Color.Transparent);

                    PaintTransparentBackground(backGraphics, ClientRectangle);
                }

                _BackBufferGraphics.Clear(Color.Transparent);
                _BackBufferGraphics.DrawImageUnscaled(_BackImage, 0, 0);

                _TabBufferGraphics.Clear(Color.Transparent);

                if (TabCount > 0)
                {
                    // When Top or Bottom and scrollable we need to clip the sides from painting the tabs.
                    // Left and Right are always multiline.
                    if (Alignment <= TabAlignment.Bottom && !Multiline)
                        _TabBufferGraphics.Clip = new Region(new RectangleF(ClientRectangle.X + 3, ClientRectangle.Y, ClientRectangle.Width - 6, ClientRectangle.Height));

                    // Draw each tabPage from Right to Left. We do it this way to handle
                    // the overlap correctly.
                    if (Multiline)
                    {
                        for (int row = 0; row < RowCount; row++)
                        {
                            for (int index = TabCount - 1; index >= 0; index--)
                            {
                                if (index != SelectedIndex && (RowCount == 1 || GetTabRow(index) == row))
                                    DrawTabPage(index, _TabBufferGraphics);
                            }
                        }
                    }
                    else
                    {
                        for (int index = TabCount - 1; index >= 0; index--)
                        {
                            if (index != SelectedIndex)
                                DrawTabPage(index, _TabBufferGraphics);
                        }
                    }

                    // The selected tab must be drawn last so it appears on Top
                    if (SelectedIndex > -1)
                        DrawTabPage(SelectedIndex, _TabBufferGraphics);
                }

                _TabBufferGraphics.Flush();

                // Paint the tabs on Top of the background

                // Create a new color matrix and set the alpha value to 0.5
                var alphaMatrix = new ColorMatrix();
                alphaMatrix.Matrix00 = alphaMatrix.Matrix11 = alphaMatrix.Matrix22 = alphaMatrix.Matrix44 = 1;
                alphaMatrix.Matrix33 = _StyleProvider.Opacity;

                // Create a new image attribute object and set the color matrix to
                // the one just created.
                using (var alphaAttributes = new ImageAttributes())
                {
                    alphaAttributes.SetColorMatrix(alphaMatrix);

                    // Draw the original image with the image attributes specified
                    _BackBufferGraphics.DrawImage(_TabBuffer,
                        new Rectangle(0, 0, _TabBuffer.Width, _TabBuffer.Height),
                        0, 0, _TabBuffer.Width, _TabBuffer.Height, GraphicsUnit.Pixel,
                        alphaAttributes);
                }

                _BackBufferGraphics.Flush();

                // Now paint this to the screen

                // We want to paint the whole tabStrip and border every time
                // so that the hot areas update correctly, along with any overlaps.

                // Paint the tabs etc.
                if (RightToLeftLayout)
                    screenGraphics.DrawImageUnscaled(_BackBuffer, -1, 0);
                else
                    screenGraphics.DrawImageUnscaled(_BackBuffer, 0, 0);
            }
        }

        /// <summary>
        /// Рисует прозрачный фон контрола
        /// </summary>
        /// <param name="graphics">Графический контекст для рисования</param>
        /// <param name="clipRect">Прямоугольник обрезки</param>
        protected void PaintTransparentBackground(Graphics graphics, Rectangle clipRect)
        {
            if (Parent != null)
            {
                // Устанавливаем cliprect относительно родителя
                clipRect.Offset(Location);

                // Сохраняем текущее состояние перед любыми действиями
                GraphicsState state = graphics.Save();

                // Устанавливаем объект графики относительно родителя
                graphics.TranslateTransform(-Location.X, -Location.Y);
                graphics.SmoothingMode = SmoothingMode.HighSpeed;

                // Рисуем родителя
                var e = new PaintEventArgs(graphics, clipRect);

                try
                {
                    InvokePaintBackground(Parent, e);
                    InvokePaint(Parent, e);
                }
                finally
                {
                    // Восстанавливаем состояние графики и clipRect в их исходные местоположения
                    graphics.Restore(state);
                    clipRect.Offset(-Location.X, -Location.Y);
                }
            }
        }

        /// <summary>
        /// Рисует страницу вкладки
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <param name="graphics">Графический контекст для рисования</param>
        private void DrawTabPage(int index, Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighSpeed;

            // Рисуем границу страницы только для активной вкладки,
            // чтобы у неактивных вкладок не появлялась горизонтальная «палка» от границы страницы.
            if (index == SelectedIndex)
            {
                // 1) Закрашиваем фон страницы контента целиком (без клиппинга)
                using (GraphicsPath tabPageBorderPath = GetTabPageBorder(index))
                {
                    using (Brush fillBrush = _StyleProvider!.GetPageBackgroundBrush(index))
                        graphics.FillPath(fillBrush, tabPageBorderPath);
                }

                // 2) Рисуем сам таб, его картинку и текст (без клиппинга)
                if (_Style != TabStyle.None)
                {
                    _StyleProvider.PaintTab(index, graphics);
                    DrawTabImage(index, graphics);
                    DrawTabText(index, graphics);
                }

                // 3) Рисуем границу самого таба (без клиппинга)
                using (GraphicsPath tabBorderPath = _StyleProvider!.GetTabBorder(index))
                {
                    DrawTabBorder(tabBorderPath, index, graphics);
                }

                // 4) Рисуем ТОЛЬКО границу страницы с клиппингом, исключающим область полосы вкладок
                using (GraphicsPath pageBorderPath = new GraphicsPath())
                {
                    Rectangle pageBounds = GetPageBounds(index);
                    Rectangle tabBounds = _StyleProvider!.GetTabRect(index);
                    AddPageBorder(pageBorderPath, pageBounds, tabBounds);
                    pageBorderPath.CloseFigure();

                    // Вычисляем прямоугольник полосы вкладок (tab strip)
                    int itemThickness = (Alignment <= TabAlignment.Bottom) ? ItemSize.Height : ItemSize.Width;
                    int tabStripThickness = 5 + (itemThickness * RowCount);
                    Rectangle tabStripRect;

                    switch (Alignment)
                    {
                        case TabAlignment.Top:
                            tabStripRect = new Rectangle(0, 0, Width, tabStripThickness);
                            break;
                        case TabAlignment.Bottom:
                            tabStripRect = new Rectangle(0, Height - tabStripThickness, Width, tabStripThickness);
                            break;
                        case TabAlignment.Left:
                            tabStripRect = new Rectangle(0, 0, tabStripThickness, Height);
                            break;
                        default: // TabAlignment.Right
                            tabStripRect = new Rectangle(Width - tabStripThickness, 0, tabStripThickness, Height);
                            break;
                    }

                    // Исключаем полосу вкладок из области рисования границы страницы
                    GraphicsState state = graphics.Save();
                    graphics.SetClip(new Rectangle(0, 0, Width, Height));
                    graphics.ExcludeClip(tabStripRect);
                    DrawTabBorder(pageBorderPath, index, graphics);
                    graphics.Restore(state);
                }
            }
            else
            {
                // Для неактивных вкладок границу страницы не рисуем — только сам таб
                if (_Style != TabStyle.None)
                {
                    _StyleProvider.PaintTab(index, graphics);
                    DrawTabImage(index, graphics);
                    DrawTabText(index, graphics);
                }

                using (GraphicsPath tabBorderPath = _StyleProvider!.GetTabBorder(index))
                {
                    DrawTabBorder(tabBorderPath, index, graphics);
                }
            }
        }

        /// <summary>
        /// Рисует границу вкладки
        /// </summary>
        /// <param name="path">Графический путь границы</param>
        /// <param name="index">Индекс вкладки</param>
        /// <param name="graphics">Графический контекст для рисования</param>
        private void DrawTabBorder(GraphicsPath path, int index, Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            Color borderColor;

            if (index == SelectedIndex)
                borderColor = _StyleProvider.BorderColorSelected;
            else if (_StyleProvider.HotTrack && index == ActiveIndex)
                borderColor = _StyleProvider.BorderColorHot;
            else
                borderColor = _StyleProvider.BorderColor;

            using (var borderPen = new Pen(borderColor))
                graphics.DrawPath(borderPen, path);
        }

        /// <summary>
        /// Рисует текст на вкладке
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <param name="graphics">Графический контекст для рисования</param>
        private void DrawTabText(int index, Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle tabBounds = GetTabTextRect(index);

            if (SelectedIndex == index)
            {
                using (Brush textBrush = new SolidBrush(_StyleProvider!.TextColorSelected))
                    graphics.DrawString(TabPages[index].Text, Font, textBrush, tabBounds, GetStringFormat());
            }
            else
            {
                if (TabPages[index].Enabled)
                {
                    // Проверяем, наведена ли мышь на эту вкладку и включено ли отслеживание
                    if (_StyleProvider.HotTrack && index == ActiveIndex)
                    {
                        using (Brush textBrush = new SolidBrush(_StyleProvider!.TextColorHot))
                            graphics.DrawString(TabPages[index].Text, Font, textBrush, tabBounds, GetStringFormat());
                    }
                    else
                    {
                        using (Brush textBrush = new SolidBrush(_StyleProvider!.TextColor))
                            graphics.DrawString(TabPages[index].Text, Font, textBrush, tabBounds, GetStringFormat());
                    }
                }
                else
                {
                    using (Brush textBrush = new SolidBrush(_StyleProvider!.TextColorDisabled))
                        graphics.DrawString(TabPages[index].Text, Font, textBrush, tabBounds, GetStringFormat());
                }
            }
        }

        /// <summary>
        /// Рисует изображение на вкладке
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <param name="graphics">Графический контекст для рисования</param>
        private void DrawTabImage(int index, Graphics graphics)
        {
            Image tabImage = null;

            if (TabPages[index].ImageIndex > -1 && ImageList != null && ImageList.Images.Count > TabPages[index].ImageIndex)
            {
                tabImage = ImageList.Images[TabPages[index].ImageIndex];
            }
            else if (!string.IsNullOrEmpty(TabPages[index].ImageKey)
                && !TabPages[index].ImageKey.Equals("(none)", StringComparison.OrdinalIgnoreCase)
                && ImageList != null && ImageList.Images.ContainsKey(TabPages[index].ImageKey))
            {
                tabImage = ImageList.Images[TabPages[index].ImageKey];
            }

            if (tabImage != null)
            {
                if (RightToLeftLayout)
                    tabImage.RotateFlip(RotateFlipType.RotateNoneFlipX);

                Rectangle imageRect = GetTabImageRect(index);

                if (TabPages[index].Enabled)
                    graphics.DrawImage(tabImage, imageRect);
                else
                    ControlPaint.DrawImageDisabled(graphics, tabImage, imageRect.X, imageRect.Y, Color.Transparent);
            }
        }

        #endregion Базовые методы рисования





        #region Форматирование строк

        /// <summary>
        /// Получает формат строки для отображения текста на вкладках
        /// </summary>
        /// <returns>Объект StringFormat с настройками форматирования</returns>
        private StringFormat GetStringFormat()
        {
            StringFormat? format = null;

            // Поворачиваем текст на 90 градусов для левых и правых вкладок
            switch (Alignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    format = new StringFormat();
                    break;

                case TabAlignment.Left:
                case TabAlignment.Right:
                    format = new StringFormat(StringFormatFlags.DirectionVertical);
                    break;
            }

            format!.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            if (FindForm() != null && FindForm().KeyPreview)
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
            else
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;

            if (RightToLeft == RightToLeft.Yes)
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

            return format;
        }

        #endregion Форматирование строк





        #region Границы и прямоугольники вкладок

        /// <summary>
        /// Получает графический путь границы страницы вкладки
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Графический путь границы страницы</returns>
        private GraphicsPath GetTabPageBorder(int index)
        {
            var path = new GraphicsPath();
            Rectangle pageBounds = GetPageBounds(index);
            Rectangle tabBounds = _StyleProvider!.GetTabRect(index);

            _StyleProvider.AddTabBorder(path, tabBounds);
            AddPageBorder(path, pageBounds, tabBounds);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Получает границы страницы вкладки
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Прямоугольник границ страницы</returns>
        public Rectangle GetPageBounds(int index)
        {
            Rectangle pageBounds = TabPages[index].Bounds;
            pageBounds.Width += 1;
            pageBounds.Height += 1;
            pageBounds.X -= 1;
            pageBounds.Y -= 1;

            if (pageBounds.Bottom > Height - 4)
                pageBounds.Height -= pageBounds.Bottom - Height + 4;

            return pageBounds;
        }

        /// <summary>
        /// Получает прямоугольник для отображения текста на вкладке
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Прямоугольник для текста</returns>
        private Rectangle GetTabTextRect(int index)
        {
            Rectangle textRect;

            using (GraphicsPath path = _StyleProvider!.GetTabBorder(index))
            {
                RectangleF tabBounds = path.GetBounds();
                textRect = new Rectangle((int)tabBounds.X, (int)tabBounds.Y, (int)tabBounds.Width, (int)tabBounds.Height);

                // Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
                switch (Alignment)
                {
                    case TabAlignment.Top:
                        textRect.Y += 4;
                        textRect.Height -= 6;
                        break;

                    case TabAlignment.Bottom:
                        textRect.Y += 2;
                        textRect.Height -= 6;
                        break;

                    case TabAlignment.Left:
                        textRect.X += 4;
                        textRect.Width -= 6;
                        break;

                    case TabAlignment.Right:
                        textRect.X += 2;
                        textRect.Width -= 6;
                        break;
                }

                // If there is an image allow for it
                if (ImageList != null && (TabPages[index].ImageIndex > -1
                    || (!string.IsNullOrEmpty(TabPages[index].ImageKey) && !TabPages[index].ImageKey.Equals("(none)", StringComparison.OrdinalIgnoreCase))))
                {
                    Rectangle imageRect = GetTabImageRect(index);

                    if ((_StyleProvider.ImageAlign & NativeUIUtils.AnyLeftAlign) != 0)
                    {
                        if (Alignment <= TabAlignment.Bottom)
                        {
                            textRect.X = imageRect.Right + 4;
                            textRect.Width -= textRect.Right - (int)tabBounds.Right;
                        }
                        else
                        {
                            textRect.Y = imageRect.Y + 4;
                            textRect.Height -= textRect.Bottom - (int)tabBounds.Bottom;
                        }

                        textRect = HandleTabCloser(index, textRect, tabBounds);
                    }
                    else if ((_StyleProvider.ImageAlign & NativeUIUtils.AnyCenterAlign) != 0)
                        textRect = HandleTabCloser(index, textRect, tabBounds);
                    else
                    {
                        if (Alignment <= TabAlignment.Bottom)
                            textRect.Width -= (int)tabBounds.Right - imageRect.X + 4;
                        else
                            textRect.Height -= (int)tabBounds.Bottom - imageRect.Y + 4;

                        textRect = HandleTabCloser(index, textRect, tabBounds);
                    }
                }
                else
                    textRect = HandleTabCloser(index, textRect, tabBounds);

                // Ensure it fits inside the path at the centre line
                if (Alignment <= TabAlignment.Bottom)
                {
                    while (!path.IsVisible(textRect.Right, textRect.Y) && textRect.Width > 0)
                        textRect.Width -= 1;

                    while (!path.IsVisible(textRect.X, textRect.Y) && textRect.Width > 0)
                    {
                        textRect.X += 1;
                        textRect.Width -= 1;
                    }
                }
                else
                {
                    while (!path.IsVisible(textRect.X, textRect.Bottom) && textRect.Height > 0)
                        textRect.Height -= 1;

                    while (!path.IsVisible(textRect.X, textRect.Y) && textRect.Height > 0)
                    {
                        textRect.Y += 1;
                        textRect.Height -= 1;
                    }
                }
            }

            return textRect;
        }

        private Rectangle HandleTabCloser(int index, Rectangle textRect, RectangleF tabBounds)
        {
            // If there is a closer allow for it
            if (_StyleProvider.ShowTabCloser)
            {
                Rectangle closerRect = GetTabCloserRect(index);

                if (Alignment <= TabAlignment.Bottom)
                {
                    if (RightToLeftLayout)
                    {
                        textRect.Width -= closerRect.Right + 4 - textRect.X;
                        textRect.X = closerRect.Right + 4;
                    }
                    else
                        textRect.Width -= (int)tabBounds.Right - closerRect.X + 4;
                }
                else
                {
                    if (RightToLeftLayout)
                    {
                        textRect.Height -= closerRect.Bottom + 4 - textRect.Y;
                        textRect.Y = closerRect.Bottom + 4;
                    }
                    else
                        textRect.Height -= (int)tabBounds.Bottom - closerRect.Y + 4;
                }
            }

            return textRect;
        }

        /// <summary>
        /// Получает номер ряда, в котором находится вкладка
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Номер ряда вкладки</returns>
        public int GetTabRow(int index)
        {
            // Все вычисления будут использовать этот прямоугольник как базовую точку
            // потому что itemsize не возвращает правильную ширину.
            Rectangle rect = GetTabRect(index);

            int row = -1;

            switch (Alignment)
            {
                case TabAlignment.Top:
                    row = (rect.Y - 2) / rect.Height;
                    break;

                case TabAlignment.Bottom:
                    row = ((Height - rect.Y - 2) / rect.Height) - 1;
                    break;

                case TabAlignment.Left:
                    row = (rect.X - 2) / rect.Width;
                    break;

                case TabAlignment.Right:
                    row = ((Width - rect.X - 2) / rect.Width) - 1;
                    break;
            }

            return row;
        }

        /// <summary>
        /// Получает позицию вкладки в сетке (ряд, колонка)
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Точка с координатами ряда и колонки</returns>
        public Point GetTabPosition(int index)
        {
            // Если мы не в многострочном режиме, то колонка - это индекс, а ряд - 0
            if (!Multiline)
                return new Point(0, index);

            // Если есть только один ряд, то колонка - это индекс
            if (RowCount == 1)
                return new Point(0, index);

            // Мы в настоящем многострочном сценарии
            int row = GetTabRow(index);
            Rectangle rect = GetTabRect(index);
            int column = -1;

            // Сканируем слева направо по рядам, пропуская к следующему ряду, если это не тот, который нам нужен
            for (int testIndex = 0; testIndex < TabCount; testIndex++)
            {
                Rectangle testRect = GetTabRect(testIndex);

                if (Alignment <= TabAlignment.Bottom)
                {
                    if (testRect.Y == rect.Y)
                        column += 1;
                }
                else
                {
                    if (testRect.X == rect.X)
                        column += 1;
                }

                if (testRect.Location.Equals(rect.Location))
                    return new Point(row, column);
            }

            return new Point(0, 0);
        }

        /// <summary>
        /// Определяет, является ли вкладка первой в своем ряду
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>true, если вкладка первая в ряду; иначе false</returns>
        public bool IsFirstTabInRow(int index)
        {
            if (index < 0)
                return false;

            bool firstTabinRow = (index == 0);

            if (!firstTabinRow)
            {
                if (Alignment <= TabAlignment.Bottom)
                {
                    if (GetTabRect(index).X == 2)
                        firstTabinRow = true;
                }
                else
                {
                    if (GetTabRect(index).Y == 2)
                        firstTabinRow = true;
                }
            }

            return firstTabinRow;
        }

        /// <summary>
        /// Добавляет границу страницы к графическому пути
        /// </summary>
        /// <param name="path">Графический путь для добавления границы</param>
        /// <param name="pageBounds">Границы страницы</param>
        /// <param name="tabBounds">Границы вкладки</param>
        private void AddPageBorder(GraphicsPath path, Rectangle pageBounds, Rectangle tabBounds)
        {
            switch (Alignment)
            {
                case TabAlignment.Top:
                    path.AddLine(tabBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, tabBounds.X, pageBounds.Y);
                    break;

                case TabAlignment.Bottom:
                    path.AddLine(tabBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, tabBounds.Right, pageBounds.Bottom);
                    break;

                case TabAlignment.Left:
                    path.AddLine(pageBounds.X, tabBounds.Y, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, tabBounds.Bottom);
                    break;

                case TabAlignment.Right:
                    path.AddLine(pageBounds.Right, tabBounds.Bottom, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, tabBounds.Y);
                    break;
            }
        }

        /// <summary>
        /// Получает прямоугольник для отображения изображения на вкладке
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Прямоугольник изображения</returns>
        private Rectangle GetTabImageRect(int index)
        {
            using (GraphicsPath tabBorderPath = _StyleProvider!.GetTabBorder(index))
                return GetTabImageRect(tabBorderPath);
        }

        /// <summary>
        /// Получает прямоугольник для отображения изображения на основе графического пути
        /// </summary>
        /// <param name="tabBorderPath">Графический путь границы вкладки</param>
        /// <returns>Прямоугольник изображения</returns>
        private Rectangle GetTabImageRect(GraphicsPath tabBorderPath)
        {
            Rectangle imageRect;
            RectangleF rect = tabBorderPath.GetBounds();

            // Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
            switch (Alignment)
            {
                case TabAlignment.Top:
                    rect.Y += 4;
                    rect.Height -= 6;
                    break;

                case TabAlignment.Bottom:
                    rect.Y += 2;
                    rect.Height -= 6;
                    break;

                case TabAlignment.Left:
                    rect.X += 4;
                    rect.Width -= 6;
                    break;

                case TabAlignment.Right:
                    rect.X += 2;
                    rect.Width -= 6;
                    break;
            }

            // Ensure image is fully visible
            if (Alignment <= TabAlignment.Bottom)
            {
                if ((_StyleProvider.ImageAlign & NativeUIUtils.AnyLeftAlign) != 0)
                {
                    imageRect = new Rectangle((int)rect.X, (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 16) / 2), 16, 16);

                    while (!tabBorderPath.IsVisible(imageRect.X, imageRect.Y))
                        imageRect.X += 1;

                    imageRect.X += 4;
                }
                else if ((_StyleProvider.ImageAlign & NativeUIUtils.AnyCenterAlign) != 0)
                    imageRect = new Rectangle((int)rect.X + (int)Math.Floor((double)(((int)rect.Right - (int)rect.X - (int)rect.Height + 2) / 2)), (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 16) / 2), 16, 16);
                else
                {
                    imageRect = new Rectangle((int)rect.Right, (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 16) / 2), 16, 16);

                    while (!tabBorderPath.IsVisible(imageRect.Right, imageRect.Y))
                        imageRect.X -= 1;

                    imageRect.X -= 4;

                    // Move it in further to allow for the tab closer
                    if (_StyleProvider.ShowTabCloser && !RightToLeftLayout)
                        imageRect.X -= 10;
                }
            }
            else
            {
                if ((_StyleProvider.ImageAlign & NativeUIUtils.AnyLeftAlign) != 0)
                {
                    imageRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 16) / 2), (int)rect.Y, 16, 16);

                    while (!tabBorderPath.IsVisible(imageRect.X, imageRect.Y))
                        imageRect.Y += 1;

                    imageRect.Y += 4;
                }
                else if ((_StyleProvider.ImageAlign & NativeUIUtils.AnyCenterAlign) != 0)
                    imageRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 16) / 2), (int)rect.Y + (int)Math.Floor((double)(((int)rect.Bottom - (int)rect.Y - (int)rect.Width + 2) / 2)), 16, 16);
                else
                {
                    imageRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 16) / 2), (int)rect.Bottom, 16, 16);

                    while (!tabBorderPath.IsVisible(imageRect.X, imageRect.Bottom))
                        imageRect.Y -= 1;

                    imageRect.Y -= 4;

                    // Сдвигаем дальше, чтобы освободить место для кнопки закрытия
                    if (_StyleProvider!.ShowTabCloser && !RightToLeftLayout)
                        imageRect.Y -= 10;
                }
            }

            return imageRect;
        }

        /// <summary>
        /// Получает прямоугольник для кнопки закрытия вкладки
        /// </summary>
        /// <param name="index">Индекс вкладки</param>
        /// <returns>Прямоугольник кнопки закрытия</returns>
        public Rectangle GetTabCloserRect(int index)
        {
            Rectangle closerRect;

            using (GraphicsPath path = _StyleProvider!.GetTabBorder(index))
            {
                RectangleF rect = path.GetBounds();

                // Делаем его короче или тоньше, чтобы поместиться в высоту или ширину из-за отступа, добавленного к вкладке для рисования
                switch (Alignment)
                {
                    case TabAlignment.Top:
                        rect.Y += 4;
                        rect.Height -= 6;
                        break;

                    case TabAlignment.Bottom:
                        rect.Y += 2;
                        rect.Height -= 6;
                        break;

                    case TabAlignment.Left:
                        rect.X += 4;
                        rect.Width -= 6;
                        break;

                    case TabAlignment.Right:
                        rect.X += 2;
                        rect.Width -= 6;
                        break;
                }

                if (Alignment <= TabAlignment.Bottom)
                {
                    if (RightToLeftLayout)
                    {
                        closerRect = new Rectangle((int)rect.Left, (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 6) / 2), 6, 6);

                        while (!path.IsVisible(closerRect.Left, closerRect.Y) && closerRect.Right < Width)
                            closerRect.X += 1;

                        closerRect.X += 4;
                    }
                    else
                    {
                        closerRect = new Rectangle((int)rect.Right, (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 6) / 2), 6, 6);

                        while (!path.IsVisible(closerRect.Right, closerRect.Y) && closerRect.Right > -6)
                            closerRect.X -= 1;

                        closerRect.X -= 4;
                    }
                }
                else
                {
                    if (RightToLeftLayout)
                    {
                        closerRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 6) / 2), (int)rect.Top, 6, 6);

                        while (!path.IsVisible(closerRect.X, closerRect.Top) && closerRect.Bottom < Height)
                            closerRect.Y += 1;

                        closerRect.Y += 4;
                    }
                    else
                    {
                        closerRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 6) / 2), (int)rect.Bottom, 6, 6);

                        while (!path.IsVisible(closerRect.X, closerRect.Bottom) && closerRect.Top > -6)
                            closerRect.Y -= 1;

                        closerRect.Y -= 4;
                    }
                }
            }

            return closerRect;
        }

        /// <summary>
        /// Получает позицию мыши относительно контрола с учетом RTL макета
        /// </summary>
        public new Point MousePosition
        {
            get
            {
                Point loc = PointToClient(Control.MousePosition);

                if (RightToLeftLayout)
                    loc.X = (Width - loc.X);

                return loc;
            }
        }

        #endregion Границы и прямоугольники вкладок





        #region Свойства дизайнера

        /// <summary>
        /// Получает или задает радиус скругления углов вкладок
        /// </summary>
        [Category("Стиль вкладок"), DefaultValue(2)]
        [Description("Определяет радиус скругления углов вкладок")]
        public int TabRadius
        {
            get { return DisplayStyleProvider.Radius; }
            set { DisplayStyleProvider.Radius = value; }
        }

        /// <summary>
        /// Получает или задает величину перекрытия вкладок
        /// </summary>
        [Category("Стиль вкладок"), DefaultValue(0)]
        [Description("Определяет величину перекрытия между соседними вкладками")]
        public int TabOverlap
        {
            get { return DisplayStyleProvider.Overlap; }
            set { DisplayStyleProvider.Overlap = value; }
        }

        /// <summary>
        /// Получает или задает отслеживание фокуса на вкладках
        /// </summary>
        [Category("Стиль вкладок"), DefaultValue(false)]
        [Description("Определяет, будет ли отображаться индикатор фокуса на вкладках")]
        public bool TabFocusTrack
        {
            get { return DisplayStyleProvider.FocusTrack; }
            set { DisplayStyleProvider.FocusTrack = value; }
        }

        /// <summary>
        /// Получает или задает отображение кнопки закрытия на вкладках
        /// </summary>
        [Category("Стиль вкладок"), DefaultValue(false)]
        [Description("Определяет, будет ли отображаться кнопка закрытия на каждой вкладке")]
        public bool ShowTabCloser
        {
            get { return DisplayStyleProvider.ShowTabCloser; }
            set { DisplayStyleProvider.ShowTabCloser = value; }
        }

        /// <summary>
        /// Получает или задает прозрачность вкладок
        /// </summary>
        [Category("Стиль вкладок"), DefaultValue(1.0f)]
        [Description("Определяет уровень прозрачности вкладок (от 0 до 1)")]
        public float TabOpacity
        {
            get { return DisplayStyleProvider.Opacity; }
            set { DisplayStyleProvider.Opacity = value; }
        }

        /// <summary>
        /// Получает или задает цвет границы выбранной вкладки
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет границы выбранной вкладки")]
        public Color TabBorderColorSelected
        {
            get { return DisplayStyleProvider.BorderColorSelected; }
            set { DisplayStyleProvider.BorderColorSelected = value; }
        }

        /// <summary>
        /// Получает или задает цвет границы при наведении на вкладку
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет границы вкладки при наведении мыши")]
        public Color TabBorderColorHot
        {
            get { return DisplayStyleProvider.BorderColorHot; }
            set { DisplayStyleProvider.BorderColorHot = value; }
        }

        /// <summary>
        /// Получает или задает цвет границы обычной вкладки
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет границы обычной вкладки")]
        public Color TabBorderColor
        {
            get { return DisplayStyleProvider.BorderColor; }
            set { DisplayStyleProvider.BorderColor = value; }
        }

        /// <summary>
        /// Получает или задает цвет текста на вкладках
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет текста на вкладках")]
        public Color TabTextColor
        {
            get { return DisplayStyleProvider.TextColor; }
            set { DisplayStyleProvider.TextColor = value; }
        }

        /// <summary>
        /// Получает или задает цвет текста выбранной вкладки
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет текста выбранной вкладки")]
        public Color TabTextColorSelected
        {
            get { return DisplayStyleProvider.TextColorSelected; }
            set { DisplayStyleProvider.TextColorSelected = value; }
        }

        /// <summary>
        /// Получает или задает цвет текста отключенной вкладки
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет текста отключенной вкладки")]
        public Color TabTextColorDisabled
        {
            get { return DisplayStyleProvider.TextColorDisabled; }
            set { DisplayStyleProvider.TextColorDisabled = value; }
        }

        /// <summary>
        /// Получает или задает цвет индикатора фокуса
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "Orange")]
        [Description("Цвет индикатора фокуса на вкладке")]
        public Color TabFocusColor
        {
            get { return DisplayStyleProvider.FocusColor; }
            set { DisplayStyleProvider.FocusColor = value; }
        }

        /// <summary>
        /// Получает или задает цвет кнопки закрытия при наведении
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "Black")]
        [Description("Цвет кнопки закрытия при наведении")]
        public Color TabCloserColorActive
        {
            get { return DisplayStyleProvider.CloserColorActive; }
            set { DisplayStyleProvider.CloserColorActive = value; }
        }

        /// <summary>
        /// Получает или задает цвет кнопки закрытия в обычном состоянии
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "DarkGray")]
        [Description("Цвет кнопки закрытия в обычном состоянии")]
        public Color TabCloserColor
        {
            get { return DisplayStyleProvider.CloserColor; }
            set { DisplayStyleProvider.CloserColor = value; }
        }

        /// <summary>
        /// Получает или задает выравнивание изображения на вкладках
        /// </summary>
        [Category("Стиль вкладок"), DefaultValue(typeof(ContentAlignment), "MiddleLeft")]
        [Description("Определяет выравнивание изображения на вкладке")]
        public ContentAlignment TabImageAlign
        {
            get { return DisplayStyleProvider.ImageAlign; }
            set { DisplayStyleProvider.ImageAlign = value; }
        }

        /// <summary>
        /// Получает или задает второй цвет индикатора фокуса
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Второй цвет для градиента индикатора фокуса на вкладке")]
        public Color TabFocusColorSecondary
        {
            get { return DisplayStyleProvider.FocusColorSecondary; }
            set { DisplayStyleProvider.FocusColorSecondary = value; }
        }

        /// <summary>
        /// Получает или задает цвет фона обычной вкладки
        /// </summary>
        [Category("Цвета фона вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет фона обычной вкладки")]
        public Color TabBackgroundColor
        {
            get { return DisplayStyleProvider.BackgroundColor; }
            set { DisplayStyleProvider.BackgroundColor = value; }
        }

        /// <summary>
        /// Получает или задает цвет фона вкладки при наведении
        /// </summary>
        [Category("Цвета фона вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет фона вкладки при наведении мыши")]
        public Color TabBackgroundColorHot
        {
            get { return DisplayStyleProvider.BackgroundColorHot; }
            set { DisplayStyleProvider.BackgroundColorHot = value; }
        }

        /// <summary>
        /// Получает или задает цвет фона выбранной вкладки
        /// </summary>
        [Category("Цвета фона вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет фона выбранной вкладки")]
        public Color TabBackgroundColorSelected
        {
            get { return DisplayStyleProvider.BackgroundColorSelected; }
            set { DisplayStyleProvider.BackgroundColorSelected = value; }
        }

        /// <summary>
        /// Получает или задает цвет фона отключенной вкладки
        /// </summary>
        [Category("Цвета фона вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет фона отключенной вкладки")]
        public Color TabBackgroundColorDisabled
        {
            get { return DisplayStyleProvider.BackgroundColorDisabled; }
            set { DisplayStyleProvider.BackgroundColorDisabled = value; }
        }

        /// <summary>
        /// Получает или задает цвет текста при наведении на вкладку
        /// </summary>
        [Category("Цвета вкладок"), DefaultValue(typeof(Color), "")]
        [Description("Цвет текста вкладки при наведении мыши")]
        public Color TabTextColorHot
        {
            get { return DisplayStyleProvider.TextColorHot; }
            set { DisplayStyleProvider.TextColorHot = value; }
        }

        #endregion Свойства дизайнера
    }
}
