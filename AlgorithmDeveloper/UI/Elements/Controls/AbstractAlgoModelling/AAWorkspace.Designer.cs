namespace AlgorithmDeveloper.UI.Elements.Controls.AbstractAlgoModelling
{
    partial class AAWorkspace
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AAWorkspace));
            _abstractAlgoWorkspaceTC = new AlgorithmDeveloper.UI.Elements.Controls.CustomizableTabControl.CustomizableTabControl();
            _abstractAlgoWorkspaceTC_MainPage = new TabPage();
            _abstractAlgoWorkspaceMainTLP = new TableLayoutPanel();
            _subTLP = new TableLayoutPanel();
            _pagesGB = new GroupBox();
            _pagesFLP = new FlowLayoutPanel();
            _createAAModelGB = new GroupBox();
            _createNewAAModelGB = new GroupBox();
            _tryParseLASB = new Button();
            _createNewAAModelB = new Button();
            _lasInputRTB = new LASInputRichTextBox();
            _combineAAModelsGB = new GroupBox();
            _combineModelsB = new Button();
            _modelsToCombineGB = new GroupBox();
            _modelsToCombineFLP = new FlowLayoutPanel();
            _mainPage_TerminalGB = new GroupBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            _mainAlgoWorkspaceTerminal = new RichTextBox();
            _toolTip = new ToolTip(components);
            _abstractAlgoWorkspaceTC.SuspendLayout();
            _abstractAlgoWorkspaceTC_MainPage.SuspendLayout();
            _abstractAlgoWorkspaceMainTLP.SuspendLayout();
            _subTLP.SuspendLayout();
            _pagesGB.SuspendLayout();
            _createAAModelGB.SuspendLayout();
            _createNewAAModelGB.SuspendLayout();
            _combineAAModelsGB.SuspendLayout();
            _modelsToCombineGB.SuspendLayout();
            _mainPage_TerminalGB.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // _abstractAlgoWorkspaceTC
            // 
            _abstractAlgoWorkspaceTC.Controls.Add(_abstractAlgoWorkspaceTC_MainPage);
            _abstractAlgoWorkspaceTC.DisplayStyle = CustomizableTabControl.Styles.TabStyle.Dark;
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.BackgroundColor = Color.FromArgb(31, 31, 31);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.BackgroundColorDisabled = Color.DimGray;
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.BackgroundColorHot = Color.FromArgb(31, 31, 31);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.BackgroundColorSelected = Color.FromArgb(31, 31, 31);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.BorderColor = Color.FromArgb(140, 160, 180);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.BorderColorHot = Color.FromArgb(70, 110, 185);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.BorderColorSelected = Color.FromArgb(70, 140, 205);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.CloserColor = Color.FromArgb(255, 58, 58);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.CloserColorActive = Color.FromArgb(255, 0, 0);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.FocusColor = Color.FromArgb(70, 140, 205);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.FocusColorSecondary = Color.FromArgb(70, 140, 205);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.FocusTrack = true;
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.HotTrack = true;
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.ImageAlign = ContentAlignment.MiddleLeft;
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.Opacity = 1F;
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.Overlap = 0;
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.Padding = new Point(6, 3);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.Radius = 10;
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.ShowTabCloser = false;
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.TextColor = Color.FromArgb(140, 160, 180);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.TextColorDisabled = Color.FromArgb(64, 64, 64);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.TextColorHot = Color.FromArgb(70, 110, 185);
            _abstractAlgoWorkspaceTC.DisplayStyleProvider.TextColorSelected = Color.FromArgb(70, 140, 205);
            _abstractAlgoWorkspaceTC.Dock = DockStyle.Fill;
            _abstractAlgoWorkspaceTC.Location = new Point(0, 0);
            _abstractAlgoWorkspaceTC.Name = "_abstractAlgoWorkspaceTC";
            _abstractAlgoWorkspaceTC.SelectedIndex = 0;
            _abstractAlgoWorkspaceTC.Size = new Size(750, 466);
            _abstractAlgoWorkspaceTC.TabBackgroundColor = Color.FromArgb(31, 31, 31);
            _abstractAlgoWorkspaceTC.TabBackgroundColorDisabled = Color.DimGray;
            _abstractAlgoWorkspaceTC.TabBackgroundColorHot = Color.FromArgb(31, 31, 31);
            _abstractAlgoWorkspaceTC.TabBackgroundColorSelected = Color.FromArgb(31, 31, 31);
            _abstractAlgoWorkspaceTC.TabBorderColor = Color.FromArgb(140, 160, 180);
            _abstractAlgoWorkspaceTC.TabBorderColorHot = Color.FromArgb(70, 110, 185);
            _abstractAlgoWorkspaceTC.TabBorderColorSelected = Color.FromArgb(70, 140, 205);
            _abstractAlgoWorkspaceTC.TabCloserColor = Color.FromArgb(255, 58, 58);
            _abstractAlgoWorkspaceTC.TabCloserColorActive = Color.FromArgb(255, 0, 0);
            _abstractAlgoWorkspaceTC.TabFocusColor = Color.FromArgb(70, 140, 205);
            _abstractAlgoWorkspaceTC.TabFocusColorSecondary = Color.FromArgb(70, 140, 205);
            _abstractAlgoWorkspaceTC.TabFocusTrack = true;
            _abstractAlgoWorkspaceTC.TabIndex = 8;
            _abstractAlgoWorkspaceTC.TabRadius = 10;
            _abstractAlgoWorkspaceTC.TabTextColor = Color.FromArgb(140, 160, 180);
            _abstractAlgoWorkspaceTC.TabTextColorDisabled = Color.FromArgb(64, 64, 64);
            _abstractAlgoWorkspaceTC.TabTextColorHot = Color.FromArgb(70, 110, 185);
            _abstractAlgoWorkspaceTC.TabTextColorSelected = Color.FromArgb(70, 140, 205);
            // 
            // _abstractAlgoWorkspaceTC_MainPage
            // 
            _abstractAlgoWorkspaceTC_MainPage.BackColor = Color.FromArgb(31, 31, 31);
            _abstractAlgoWorkspaceTC_MainPage.Controls.Add(_abstractAlgoWorkspaceMainTLP);
            _abstractAlgoWorkspaceTC.SetForbidClose(_abstractAlgoWorkspaceTC_MainPage, true);
            _abstractAlgoWorkspaceTC.SetForbidEntry(_abstractAlgoWorkspaceTC_MainPage, true);
            _abstractAlgoWorkspaceTC.SetForbidHide(_abstractAlgoWorkspaceTC_MainPage, true);
            _abstractAlgoWorkspaceTC_MainPage.ForeColor = Color.FromArgb(214, 214, 214);
            _abstractAlgoWorkspaceTC_MainPage.Location = new Point(4, 25);
            _abstractAlgoWorkspaceTC_MainPage.Name = "_abstractAlgoWorkspaceTC_MainPage";
            _abstractAlgoWorkspaceTC_MainPage.Padding = new Padding(3);
            _abstractAlgoWorkspaceTC_MainPage.Size = new Size(742, 437);
            _abstractAlgoWorkspaceTC_MainPage.TabIndex = 0;
            _abstractAlgoWorkspaceTC_MainPage.Text = "Информация и действия";
            // 
            // _abstractAlgoWorkspaceMainTLP
            // 
            _abstractAlgoWorkspaceMainTLP.ColumnCount = 1;
            _abstractAlgoWorkspaceMainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _abstractAlgoWorkspaceMainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _abstractAlgoWorkspaceMainTLP.Controls.Add(_subTLP, 0, 0);
            _abstractAlgoWorkspaceMainTLP.Controls.Add(_mainPage_TerminalGB, 0, 1);
            _abstractAlgoWorkspaceMainTLP.Dock = DockStyle.Fill;
            _abstractAlgoWorkspaceMainTLP.Location = new Point(3, 3);
            _abstractAlgoWorkspaceMainTLP.Name = "_abstractAlgoWorkspaceMainTLP";
            _abstractAlgoWorkspaceMainTLP.RowCount = 2;
            _abstractAlgoWorkspaceMainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 62.1809731F));
            _abstractAlgoWorkspaceMainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 37.8190269F));
            _abstractAlgoWorkspaceMainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _abstractAlgoWorkspaceMainTLP.Size = new Size(736, 431);
            _abstractAlgoWorkspaceMainTLP.TabIndex = 0;
            // 
            // _subTLP
            // 
            _subTLP.ColumnCount = 2;
            _subTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _subTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 246F));
            _subTLP.Controls.Add(_pagesGB, 2, 0);
            _subTLP.Controls.Add(_createAAModelGB, 0, 0);
            _subTLP.Dock = DockStyle.Fill;
            _subTLP.Location = new Point(3, 3);
            _subTLP.Name = "_subTLP";
            _subTLP.RowCount = 1;
            _subTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _subTLP.Size = new Size(730, 262);
            _subTLP.TabIndex = 3;
            // 
            // _pagesGB
            // 
            _pagesGB.Controls.Add(_pagesFLP);
            _pagesGB.Dock = DockStyle.Fill;
            _pagesGB.ForeColor = Color.FromArgb(224, 224, 224);
            _pagesGB.Location = new Point(487, 3);
            _pagesGB.Name = "_pagesGB";
            _pagesGB.Size = new Size(240, 256);
            _pagesGB.TabIndex = 0;
            _pagesGB.TabStop = false;
            _pagesGB.Text = "Открытые модели";
            // 
            // _pagesFLP
            // 
            _pagesFLP.AutoScroll = true;
            _pagesFLP.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _pagesFLP.Dock = DockStyle.Fill;
            _pagesFLP.FlowDirection = FlowDirection.TopDown;
            _pagesFLP.Location = new Point(3, 19);
            _pagesFLP.Name = "_pagesFLP";
            _pagesFLP.Size = new Size(234, 234);
            _pagesFLP.TabIndex = 0;
            _pagesFLP.WrapContents = false;
            _pagesFLP.ControlAdded += _pagesFLP_ControlAdded;
            // 
            // _createAAModelGB
            // 
            _createAAModelGB.Controls.Add(_createNewAAModelGB);
            _createAAModelGB.Controls.Add(_combineAAModelsGB);
            _createAAModelGB.ForeColor = Color.FromArgb(224, 224, 224);
            _createAAModelGB.Location = new Point(3, 3);
            _createAAModelGB.Name = "_createAAModelGB";
            _createAAModelGB.Size = new Size(478, 256);
            _createAAModelGB.TabIndex = 1;
            _createAAModelGB.TabStop = false;
            _createAAModelGB.Text = "Создание модели";
            // 
            // _createNewAAModelGB
            // 
            _createNewAAModelGB.Controls.Add(_tryParseLASB);
            _createNewAAModelGB.Controls.Add(_createNewAAModelB);
            _createNewAAModelGB.Controls.Add(_lasInputRTB);
            _createNewAAModelGB.ForeColor = Color.Silver;
            _createNewAAModelGB.Location = new Point(6, 19);
            _createNewAAModelGB.Name = "_createNewAAModelGB";
            _createNewAAModelGB.Size = new Size(466, 111);
            _createNewAAModelGB.TabIndex = 8;
            _createNewAAModelGB.TabStop = false;
            _createNewAAModelGB.Text = "Новая модель по ЛСА";
            // 
            // _tryParseLASB
            // 
            _tryParseLASB.BackColor = Color.FromArgb(64, 64, 64);
            _tryParseLASB.BackgroundImageLayout = ImageLayout.None;
            _tryParseLASB.Cursor = Cursors.Hand;
            _tryParseLASB.Enabled = false;
            _tryParseLASB.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _tryParseLASB.FlatStyle = FlatStyle.Flat;
            _tryParseLASB.Location = new Point(6, 80);
            _tryParseLASB.Name = "_tryParseLASB";
            _tryParseLASB.Size = new Size(215, 25);
            _tryParseLASB.TabIndex = 9;
            _tryParseLASB.Text = "Проверить ЛСА";
            _tryParseLASB.UseVisualStyleBackColor = false;
            _tryParseLASB.Click += CheckLASB_Click;
            // 
            // _createNewAAModelB
            // 
            _createNewAAModelB.BackColor = Color.FromArgb(64, 64, 64);
            _createNewAAModelB.BackgroundImageLayout = ImageLayout.None;
            _createNewAAModelB.Cursor = Cursors.Hand;
            _createNewAAModelB.Enabled = false;
            _createNewAAModelB.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _createNewAAModelB.FlatStyle = FlatStyle.Flat;
            _createNewAAModelB.Location = new Point(245, 80);
            _createNewAAModelB.Name = "_createNewAAModelB";
            _createNewAAModelB.Size = new Size(215, 25);
            _createNewAAModelB.TabIndex = 7;
            _createNewAAModelB.Text = "Создать модель";
            _createNewAAModelB.UseVisualStyleBackColor = false;
            _createNewAAModelB.Click += CreateNewAAModelB_Click;
            // 
            // _lasInputRTB
            // 
            _lasInputRTB.BackColor = Color.FromArgb(48, 48, 48);
            _lasInputRTB.BorderStyle = BorderStyle.None;
            _lasInputRTB.ForeColor = Color.Silver;
            _lasInputRTB.Location = new Point(6, 22);
            _lasInputRTB.Name = "_lasInputRTB";
            _lasInputRTB.PlaceholderColor = Color.FromArgb(96, 96, 96);
            _lasInputRTB.PlaceholderText = "Пример: Yн X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк";
            _lasInputRTB.Size = new Size(454, 52);
            _lasInputRTB.TabIndex = 8;
            _lasInputRTB.Text = "";
            _toolTip.SetToolTip(_lasInputRTB, resources.GetString("_lasInputRTB.ToolTip"));
            _lasInputRTB.TextChanged += EnableCheckLASB;
            // 
            // _combineAAModelsGB
            // 
            _combineAAModelsGB.Controls.Add(_combineModelsB);
            _combineAAModelsGB.Controls.Add(_modelsToCombineGB);
            _combineAAModelsGB.ForeColor = Color.Silver;
            _combineAAModelsGB.Location = new Point(6, 136);
            _combineAAModelsGB.Name = "_combineAAModelsGB";
            _combineAAModelsGB.Size = new Size(466, 111);
            _combineAAModelsGB.TabIndex = 7;
            _combineAAModelsGB.TabStop = false;
            _combineAAModelsGB.Text = "Новая модель как объединение открытых моделей";
            // 
            // _combineModelsB
            // 
            _combineModelsB.BackColor = Color.FromArgb(64, 64, 64);
            _combineModelsB.BackgroundImageLayout = ImageLayout.None;
            _combineModelsB.Cursor = Cursors.Hand;
            _combineModelsB.Enabled = false;
            _combineModelsB.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _combineModelsB.FlatStyle = FlatStyle.Flat;
            _combineModelsB.ForeColor = Color.Silver;
            _combineModelsB.Location = new Point(306, 29);
            _combineModelsB.Name = "_combineModelsB";
            _combineModelsB.Size = new Size(154, 75);
            _combineModelsB.TabIndex = 8;
            _combineModelsB.Text = "Объединить выбранные модели";
            _combineModelsB.UseVisualStyleBackColor = false;
            // 
            // _modelsToCombineGB
            // 
            _modelsToCombineGB.Controls.Add(_modelsToCombineFLP);
            _modelsToCombineGB.ForeColor = Color.DarkGray;
            _modelsToCombineGB.Location = new Point(6, 22);
            _modelsToCombineGB.Name = "_modelsToCombineGB";
            _modelsToCombineGB.Size = new Size(294, 83);
            _modelsToCombineGB.TabIndex = 0;
            _modelsToCombineGB.TabStop = false;
            _modelsToCombineGB.Text = "Выбор моделей для объединения";
            // 
            // _modelsToCombineFLP
            // 
            _modelsToCombineFLP.AutoScroll = true;
            _modelsToCombineFLP.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _modelsToCombineFLP.Dock = DockStyle.Fill;
            _modelsToCombineFLP.FlowDirection = FlowDirection.TopDown;
            _modelsToCombineFLP.Location = new Point(3, 19);
            _modelsToCombineFLP.Name = "_modelsToCombineFLP";
            _modelsToCombineFLP.Size = new Size(288, 61);
            _modelsToCombineFLP.TabIndex = 1;
            _modelsToCombineFLP.WrapContents = false;
            // 
            // _mainPage_TerminalGB
            // 
            _mainPage_TerminalGB.Controls.Add(tableLayoutPanel1);
            _mainPage_TerminalGB.Dock = DockStyle.Fill;
            _mainPage_TerminalGB.ForeColor = Color.FromArgb(224, 224, 224);
            _mainPage_TerminalGB.Location = new Point(3, 271);
            _mainPage_TerminalGB.Name = "_mainPage_TerminalGB";
            _mainPage_TerminalGB.Size = new Size(730, 157);
            _mainPage_TerminalGB.TabIndex = 2;
            _mainPage_TerminalGB.TabStop = false;
            _mainPage_TerminalGB.Text = "Терминал";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 95.30387F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 4.69613266F));
            tableLayoutPanel1.Controls.Add(_mainAlgoWorkspaceTerminal, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 19);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(724, 135);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // _mainAlgoWorkspaceTerminal
            // 
            _mainAlgoWorkspaceTerminal.BackColor = Color.FromArgb(16, 16, 16);
            _mainAlgoWorkspaceTerminal.BorderStyle = BorderStyle.FixedSingle;
            _mainAlgoWorkspaceTerminal.Dock = DockStyle.Fill;
            _mainAlgoWorkspaceTerminal.ForeColor = Color.Silver;
            _mainAlgoWorkspaceTerminal.Location = new Point(4, 3);
            _mainAlgoWorkspaceTerminal.Margin = new Padding(4, 3, 4, 3);
            _mainAlgoWorkspaceTerminal.Name = "_mainAlgoWorkspaceTerminal";
            _mainAlgoWorkspaceTerminal.ReadOnly = true;
            _mainAlgoWorkspaceTerminal.Size = new Size(682, 129);
            _mainAlgoWorkspaceTerminal.TabIndex = 2;
            _mainAlgoWorkspaceTerminal.Text = "";
            // 
            // _toolTip
            // 
            _toolTip.AutoPopDelay = 5000;
            _toolTip.BackColor = Color.FromArgb(48, 48, 48);
            _toolTip.ForeColor = Color.Silver;
            _toolTip.InitialDelay = 200;
            _toolTip.ReshowDelay = 100;
            _toolTip.ToolTipTitle = "Информация";
            // 
            // AbstractAlgoWorkspace
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(31, 31, 31);
            Controls.Add(_abstractAlgoWorkspaceTC);
            DoubleBuffered = true;
            Name = "AbstractAlgoWorkspace";
            Size = new Size(750, 466);
            _abstractAlgoWorkspaceTC.ResumeLayout(false);
            _abstractAlgoWorkspaceTC_MainPage.ResumeLayout(false);
            _abstractAlgoWorkspaceMainTLP.ResumeLayout(false);
            _subTLP.ResumeLayout(false);
            _pagesGB.ResumeLayout(false);
            _createAAModelGB.ResumeLayout(false);
            _createNewAAModelGB.ResumeLayout(false);
            _combineAAModelsGB.ResumeLayout(false);
            _modelsToCombineGB.ResumeLayout(false);
            _mainPage_TerminalGB.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private CustomizableTabControl.CustomizableTabControl _abstractAlgoWorkspaceTC;
        private TabPage _abstractAlgoWorkspaceTC_MainPage;
        private TableLayoutPanel _abstractAlgoWorkspaceMainTLP;
        private GroupBox _mainPage_TerminalGB;
        private ToolTip _toolTip;
        private TableLayoutPanel _subTLP;
        private GroupBox _pagesGB;
        private FlowLayoutPanel _pagesFLP;
        private GroupBox _createAAModelGB;
        private GroupBox _createNewAAModelGB;
        private Button _tryParseLASB;
        private Button _createNewAAModelB;
        private LASInputRichTextBox _lasInputRTB;
        private GroupBox _combineAAModelsGB;
        private GroupBox _modelsToCombineGB;
        private Button _combineModelsB;
        private FlowLayoutPanel _modelsToCombineFLP;
        private TableLayoutPanel tableLayoutPanel1;
        public RichTextBox _mainAlgoWorkspaceTerminal;
    }
}
