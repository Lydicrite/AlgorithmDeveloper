using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl
{
    /// <summary>
    /// Элемент для управления вкладкой: отображает название и кнопки Показать/Скрыть/Закрыть
    /// Сделан совместимым с Visual Studio Designer (имеет параметрless-конструктор и InitializeComponent).
    /// </summary>
    public partial class TabEntryControl : UserControl
    {
        public TabPage? TabPage { get; private set; }
        public CustomTabControl? TabControl { get; private set; }

        private bool _isProtected;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool IsProtected
        {
            get => _isProtected;
            set
            {
                _isProtected = value;
                if (!IsDisposed)
                    UpdateState();
            }
        }

        public TabEntryControl()
        {
            InitializeComponent();
            this.Disposed += TabEntryControl_Disposed;
            this.MouseDoubleClick += Entry_DoubleClick;
            _textBox.MouseDoubleClick += Entry_DoubleClick;
        }

        public TabEntryControl(CustomTabControl tabControl, TabPage tabPage) : this()
        {
            TabControl = tabControl ?? throw new ArgumentNullException(nameof(tabControl));
            TabPage = tabPage ?? throw new ArgumentNullException(nameof(tabPage));

            HookTabControlEvents();

            UpdateTitle();
            UpdateState();
        }

        public void UpdateTitle()
        {
            if (!IsDisposed && TabPage != null)
            {
                _textBox.Text = TabPage.Text;
            }
            else if (!IsDisposed && TabPage == null)
            {
                _textBox.Text = "Вкладка";
            }
        }

        public void UpdateState()
        {
            bool hasTabControl = TabControl != null;
            bool hasPage = TabPage != null && !TabPage.IsDisposed;

            bool isVisible = hasTabControl && hasPage && TabControl!.TabPages.Contains(TabPage!);
            bool isDisposed = TabPage != null && TabPage.IsDisposed;

            // Если вкладка защищена (главная), отключаем обе кнопки
            bool disableForProtected = IsProtected;

            // Новые запреты из CustomTabControl (доступны только если есть TabControl и страница)
            bool forbidHide = hasTabControl && hasPage ? TabControl!.GetForbidHide(TabPage!) : false;
            bool forbidClose = hasTabControl && hasPage ? TabControl!.GetForbidClose(TabPage!) : false;

            // Переключатель доступен, если нет запрещающих факторов
            _btnToggle.Enabled = !isDisposed && hasTabControl && hasPage && !disableForProtected && !forbidHide;
            _btnToggle.BackgroundImage = isVisible ? Properties.Resources.ButtonHide : Properties.Resources.ButtonShow;

            // Кнопка закрытия доступна, если нет запрещающих факторов
            _btnClose.Enabled = !isDisposed && hasPage && !disableForProtected && !forbidClose;
        }

        private void SafeToggle(object? sender, EventArgs e)
        {
            if (TabControl != null && TabPage != null && !TabPage.IsDisposed)
            {
                bool isVisible = TabControl.TabPages.Contains(TabPage);
                if (isVisible)
                {
                    TabControl.HideTab(TabPage);
                }
                else
                {
                    TabControl.ShowTab(TabPage);
                }
                UpdateState();
            }
        }

        private void SafeClose(object? sender, EventArgs e)
        {
            if (TabControl != null && TabPage != null && !TabPage.IsDisposed)
            {
                TabControl.CloseTab(TabPage);
            }
        }





        #region Подписка/отписка на события CustomTabControl

        private void HookTabControlEvents()
        {
            if (TabControl != null)
            {
                TabControl.TabHidden += TabControl_TabHidden;
                TabControl.TabShown += TabControl_TabShown;
                TabControl.TabHidePermissionChanged += TabControl_PermissionsChanged;
                TabControl.TabClosePermissionChanged += TabControl_PermissionsChanged;
            }
        }

        private void UnhookTabControlEvents()
        {
            if (TabControl != null)
            {
                TabControl.TabHidden -= TabControl_TabHidden;
                TabControl.TabShown -= TabControl_TabShown;
                TabControl.TabHidePermissionChanged -= TabControl_PermissionsChanged;
                TabControl.TabClosePermissionChanged -= TabControl_PermissionsChanged;
            }
        }

        private void TabControl_TabHidden(object? sender, TabControlEventArgs e)
        {
            if (!IsDisposed && TabPage != null && ReferenceEquals(e.TabPage, TabPage))
            {
                UpdateState();
            }
        }

        private void TabControl_TabShown(object? sender, TabControlEventArgs e)
        {
            if (!IsDisposed && TabPage != null && ReferenceEquals(e.TabPage, TabPage))
            {
                UpdateState();
            }
        }

        private void TabControl_PermissionsChanged(object? sender, TabControlEventArgs e)
        {
            if (!IsDisposed && TabPage != null && ReferenceEquals(e.TabPage, TabPage))
            {
                UpdateState();
            }
        }

        private Control? _parentContainer;

        #endregion





        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            UnhookParentContainerEvents();
            _parentContainer = this.Parent;
            HookParentContainerEvents();
            UpdateWidthToContainer();
        }

        private void HookParentContainerEvents()
        {
            if (_parentContainer != null)
            {
                _parentContainer.SizeChanged += ParentContainer_SizeChanged;
                if (_parentContainer is FlowLayoutPanel flp)
                {
                    flp.Layout += ParentContainer_Layout;
                }
            }
        }

        private void UnhookParentContainerEvents()
        {
            if (_parentContainer != null)
            {
                _parentContainer.SizeChanged -= ParentContainer_SizeChanged;
                if (_parentContainer is FlowLayoutPanel flp)
                {
                    flp.Layout -= ParentContainer_Layout;
                }
            }
        }

        private void ParentContainer_SizeChanged(object? sender, EventArgs e)
        {
            UpdateWidthToContainer();
        }

        private void ParentContainer_Layout(object? sender, LayoutEventArgs e)
        {
            UpdateWidthToContainer();
        }

        private void UpdateWidthToContainer()
        {
            if (IsDisposed)
                return;

            var parent = _parentContainer ?? this.Parent;
            if (parent == null)
                return;

            int containerWidth = parent.DisplayRectangle.Width;
            int targetWidth = containerWidth - this.Margin.Left - this.Margin.Right;

            if (targetWidth < this.MinimumSize.Width)
                targetWidth = this.MinimumSize.Width;

            int maxWidth = this.MaximumSize.Width > 0 ? this.MaximumSize.Width : int.MaxValue;
            if (targetWidth > maxWidth)
                targetWidth = maxWidth;

            this.Width = targetWidth;
        }

        private void TabEntryControl_Disposed(object? sender, EventArgs e)
        {
            UnhookTabControlEvents();
            UnhookParentContainerEvents();
        }

        private void Entry_DoubleClick(object? sender, MouseEventArgs e)
        {
            if (TabControl != null && TabPage != null && !TabPage.IsDisposed)
            {
                // Показать вкладку (если была скрыта) и активировать её
                TabControl.ShowTab(TabPage);
                try
                {
                    TabControl.SelectedTab = TabPage;
                }
                catch { /* игнорируем, если нельзя выбрать */ }
                UpdateState();
            }
        }
    }
}