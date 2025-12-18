using AlgorithmDeveloper.UI.Elements.Controls.CustomizableTabControl.Styles;

namespace AlgorithmDeveloper
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            _mainToolStrip = new AlgorithmDeveloper.UI.Elements.Controls.CustomizableToolStrip.CustomizableToolStrip();
            _mainToolStrip_File = new ToolStripDropDownButton();
            _mainToolStrip_File_Open = new ToolStripMenuItem();
            _mainToolStrip_File_S1 = new ToolStripSeparator();
            _mainToolStrip_File_Save = new ToolStripMenuItem();
            _mainToolStrip_File_SaveAs = new ToolStripMenuItem();
            _mainToolStrip_View = new ToolStripDropDownButton();
            _mainTabControl_MainPage = new TabPage();
            _mainTLP = new TableLayoutPanel();
            _subTLP = new TableLayoutPanel();
            _pagesGB = new GroupBox();
            _pagesFLP = new FlowLayoutPanel();
            _modelSelectGB = new GroupBox();
            _createWorkspacePageB = new Button();
            _labelWorkspaceModelType = new Label();
            _workspaceModelTypeCB = new ComboBox();
            _mainPage_TerminalGB = new GroupBox();
            _mainTerminal = new RichTextBox();
            _mainTabControl = new AlgorithmDeveloper.UI.Elements.Controls.CustomizableTabControl.CustomizableTabControl();
            _mainTabControl_SettingsPage = new TabPage();
            _mainToolStrip.SuspendLayout();
            _mainTabControl_MainPage.SuspendLayout();
            _mainTLP.SuspendLayout();
            _subTLP.SuspendLayout();
            _pagesGB.SuspendLayout();
            _modelSelectGB.SuspendLayout();
            _mainPage_TerminalGB.SuspendLayout();
            _mainTabControl.SuspendLayout();
            SuspendLayout();
            // 
            // _mainToolStrip
            // 
            _mainToolStrip.ArrowColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip.AutoSize = false;
            _mainToolStrip.CheckedBackColor = Color.Transparent;
            _mainToolStrip.CheckedBackColor2 = null;
            _mainToolStrip.CheckedBorderColor = Color.Transparent;
            _mainToolStrip.CheckedTextColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip.DisabledArrowColor = SystemColors.ControlDarkDark;
            _mainToolStrip.DisabledTextColor = SystemColors.ControlDarkDark;
            _mainToolStrip.Dock = DockStyle.None;
            _mainToolStrip.DropDownBackColor = Color.FromArgb(46, 46, 46);
            _mainToolStrip.DropDownBorderColor = Color.FromArgb(66, 66, 66);
            _mainToolStrip.DropDownBorderEnabled = true;
            _mainToolStrip.DropDownCornerRadius = 0;
            _mainToolStrip.DropDownOverrideBackground = true;
            _mainToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            _mainToolStrip.HoverArrowColor = Color.FromArgb(250, 250, 250);
            _mainToolStrip.HoverBackColor = Color.FromArgb(61, 61, 61);
            _mainToolStrip.HoverBackColor2 = null;
            _mainToolStrip.HoverBorderColor = Color.FromArgb(112, 112, 112);
            _mainToolStrip.HoverTextColor = Color.FromArgb(250, 250, 250);
            _mainToolStrip.Items.AddRange(new ToolStripItem[] { _mainToolStrip_File, _mainToolStrip_View });
            _mainToolStrip.Location = new Point(4, 3);
            _mainToolStrip.Margin = new Padding(3);
            _mainToolStrip.Name = "_mainToolStrip";
            _mainToolStrip.OutlineColor = Color.FromArgb(250, 250, 250);
            _mainToolStrip.OutlineCornerRadius = 0;
            _mainToolStrip.OverrideBackground = true;
            _mainToolStrip.PressedArrowColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip.PressedBackColor = Color.FromArgb(46, 46, 46);
            _mainToolStrip.PressedBackColor2 = null;
            _mainToolStrip.PressedBorderColor = Color.FromArgb(66, 66, 66);
            _mainToolStrip.PressedTextColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip.SeparatorColor = Color.FromArgb(66, 66, 66);
            _mainToolStrip.SeparatorLightColor = Color.FromArgb(80, 80, 80);
            _mainToolStrip.Size = new Size(756, 25);
            _mainToolStrip.TabIndex = 4;
            _mainToolStrip.TextColor = Color.FromArgb(214, 214, 214);
            // 
            // _mainToolStrip_File
            // 
            _mainToolStrip_File.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _mainToolStrip_File.DropDownItems.AddRange(new ToolStripItem[] { _mainToolStrip_File_Open, _mainToolStrip_File_S1, _mainToolStrip_File_Save, _mainToolStrip_File_SaveAs });
            _mainToolStrip_File.ForeColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip_File.Image = (Image)resources.GetObject("_mainToolStrip_File.Image");
            _mainToolStrip_File.ImageTransparentColor = Color.Magenta;
            _mainToolStrip_File.Name = "_mainToolStrip_File";
            _mainToolStrip_File.Size = new Size(49, 22);
            _mainToolStrip_File.Text = "Файл";
            // 
            // _mainToolStrip_File_Open
            // 
            _mainToolStrip_File_Open.ForeColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip_File_Open.Name = "_mainToolStrip_File_Open";
            _mainToolStrip_File_Open.Size = new Size(162, 22);
            _mainToolStrip_File_Open.Text = "Открыть...";
            // 
            // _mainToolStrip_File_S1
            // 
            _mainToolStrip_File_S1.ForeColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip_File_S1.Name = "_mainToolStrip_File_S1";
            _mainToolStrip_File_S1.Size = new Size(159, 6);
            // 
            // _mainToolStrip_File_Save
            // 
            _mainToolStrip_File_Save.ForeColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip_File_Save.Name = "_mainToolStrip_File_Save";
            _mainToolStrip_File_Save.Size = new Size(162, 22);
            _mainToolStrip_File_Save.Text = "Сохранить";
            // 
            // _mainToolStrip_File_SaveAs
            // 
            _mainToolStrip_File_SaveAs.ForeColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip_File_SaveAs.Name = "_mainToolStrip_File_SaveAs";
            _mainToolStrip_File_SaveAs.Size = new Size(162, 22);
            _mainToolStrip_File_SaveAs.Text = "Сохранить как...";
            // 
            // _mainToolStrip_View
            // 
            _mainToolStrip_View.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _mainToolStrip_View.ForeColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip_View.Image = (Image)resources.GetObject("_mainToolStrip_View.Image");
            _mainToolStrip_View.ImageTransparentColor = Color.Magenta;
            _mainToolStrip_View.Name = "_mainToolStrip_View";
            _mainToolStrip_View.Size = new Size(40, 22);
            _mainToolStrip_View.Text = "Вид";
            // 
            // _mainTabControl_MainPage
            // 
            _mainTabControl_MainPage.BackColor = Color.FromArgb(31, 31, 31);
            _mainTabControl_MainPage.Controls.Add(_mainTLP);
            _mainTabControl.SetForbidClose(_mainTabControl_MainPage, true);
            _mainTabControl.SetForbidEntry(_mainTabControl_MainPage, true);
            _mainTabControl.SetForbidHide(_mainTabControl_MainPage, true);
            _mainTabControl_MainPage.ForeColor = Color.FromArgb(214, 214, 214);
            _mainTabControl_MainPage.Location = new Point(4, 25);
            _mainTabControl_MainPage.Name = "_mainTabControl_MainPage";
            _mainTabControl_MainPage.Padding = new Padding(3);
            _mainTabControl_MainPage.Size = new Size(756, 472);
            _mainTabControl_MainPage.TabIndex = 0;
            _mainTabControl_MainPage.Text = "Главное меню";
            // 
            // _mainTLP
            // 
            _mainTLP.ColumnCount = 1;
            _mainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _mainTLP.Controls.Add(_subTLP, 0, 0);
            _mainTLP.Controls.Add(_mainPage_TerminalGB, 0, 1);
            _mainTLP.Dock = DockStyle.Fill;
            _mainTLP.Location = new Point(3, 3);
            _mainTLP.Name = "_mainTLP";
            _mainTLP.RowCount = 2;
            _mainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 220F));
            _mainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            _mainTLP.Size = new Size(750, 466);
            _mainTLP.TabIndex = 0;
            // 
            // _subTLP
            // 
            _subTLP.ColumnCount = 2;
            _subTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _subTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 252F));
            _subTLP.Controls.Add(_pagesGB, 2, 0);
            _subTLP.Controls.Add(_modelSelectGB, 0, 0);
            _subTLP.Dock = DockStyle.Fill;
            _subTLP.Location = new Point(3, 3);
            _subTLP.Name = "_subTLP";
            _subTLP.RowCount = 1;
            _subTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _subTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _subTLP.Size = new Size(744, 214);
            _subTLP.TabIndex = 2;
            // 
            // _pagesGB
            // 
            _pagesGB.Controls.Add(_pagesFLP);
            _pagesGB.Dock = DockStyle.Fill;
            _pagesGB.ForeColor = Color.FromArgb(224, 224, 224);
            _pagesGB.Location = new Point(495, 3);
            _pagesGB.Name = "_pagesGB";
            _pagesGB.Size = new Size(246, 208);
            _pagesGB.TabIndex = 0;
            _pagesGB.TabStop = false;
            _pagesGB.Text = "Вкладки";
            // 
            // _pagesFLP
            // 
            _pagesFLP.AutoScroll = true;
            _pagesFLP.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _pagesFLP.Dock = DockStyle.Fill;
            _pagesFLP.FlowDirection = FlowDirection.TopDown;
            _pagesFLP.Location = new Point(3, 19);
            _pagesFLP.Name = "_pagesFLP";
            _pagesFLP.Size = new Size(240, 186);
            _pagesFLP.TabIndex = 0;
            _pagesFLP.WrapContents = false;
            // 
            // _modelSelectGB
            // 
            _modelSelectGB.Controls.Add(_createWorkspacePageB);
            _modelSelectGB.Controls.Add(_labelWorkspaceModelType);
            _modelSelectGB.Controls.Add(_workspaceModelTypeCB);
            _modelSelectGB.ForeColor = Color.FromArgb(224, 224, 224);
            _modelSelectGB.Location = new Point(3, 3);
            _modelSelectGB.Name = "_modelSelectGB";
            _modelSelectGB.Size = new Size(486, 58);
            _modelSelectGB.TabIndex = 1;
            _modelSelectGB.TabStop = false;
            _modelSelectGB.Text = "Выбор модели для имитации работы";
            // 
            // _createWorkspacePageB
            // 
            _createWorkspacePageB.BackColor = Color.FromArgb(64, 64, 64);
            _createWorkspacePageB.BackgroundImageLayout = ImageLayout.None;
            _createWorkspacePageB.Cursor = Cursors.Hand;
            _createWorkspacePageB.Enabled = false;
            _createWorkspacePageB.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _createWorkspacePageB.FlatStyle = FlatStyle.Flat;
            _createWorkspacePageB.Location = new Point(259, 20);
            _createWorkspacePageB.Name = "_createWorkspacePageB";
            _createWorkspacePageB.Size = new Size(221, 27);
            _createWorkspacePageB.TabIndex = 3;
            _createWorkspacePageB.Text = "Создать рабочую вкладку";
            _createWorkspacePageB.UseVisualStyleBackColor = false;
            _createWorkspacePageB.Click += AddNewWorkspace;
            // 
            // _labelWorkspaceModelType
            // 
            _labelWorkspaceModelType.AutoSize = true;
            _labelWorkspaceModelType.Location = new Point(6, 25);
            _labelWorkspaceModelType.Name = "_labelWorkspaceModelType";
            _labelWorkspaceModelType.Size = new Size(53, 15);
            _labelWorkspaceModelType.TabIndex = 2;
            _labelWorkspaceModelType.Text = "Модель:";
            // 
            // _workspaceModelTypeCB
            // 
            _workspaceModelTypeCB.BackColor = Color.FromArgb(64, 64, 64);
            _workspaceModelTypeCB.FlatStyle = FlatStyle.Flat;
            _workspaceModelTypeCB.ForeColor = Color.FromArgb(224, 224, 224);
            _workspaceModelTypeCB.FormattingEnabled = true;
            _workspaceModelTypeCB.Items.AddRange(new object[] { "Абстрактный алгоритм", "ДКА Мили", "ДКА Мура" });
            _workspaceModelTypeCB.Location = new Point(65, 22);
            _workspaceModelTypeCB.Name = "_workspaceModelTypeCB";
            _workspaceModelTypeCB.Size = new Size(188, 23);
            _workspaceModelTypeCB.TabIndex = 1;
            _workspaceModelTypeCB.Text = "(список моделей)";
            _workspaceModelTypeCB.SelectedIndexChanged += EnableAddNWB;
            // 
            // _mainPage_TerminalGB
            // 
            _mainPage_TerminalGB.Controls.Add(_mainTerminal);
            _mainPage_TerminalGB.Dock = DockStyle.Fill;
            _mainPage_TerminalGB.ForeColor = Color.FromArgb(224, 224, 224);
            _mainPage_TerminalGB.Location = new Point(3, 223);
            _mainPage_TerminalGB.Name = "_mainPage_TerminalGB";
            _mainPage_TerminalGB.Size = new Size(744, 240);
            _mainPage_TerminalGB.TabIndex = 1;
            _mainPage_TerminalGB.TabStop = false;
            _mainPage_TerminalGB.Text = "Терминал";
            // 
            // _mainTerminal
            // 
            _mainTerminal.BackColor = Color.FromArgb(16, 16, 16);
            _mainTerminal.BorderStyle = BorderStyle.FixedSingle;
            _mainTerminal.Dock = DockStyle.Fill;
            _mainTerminal.Font = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _mainTerminal.ForeColor = Color.Silver;
            _mainTerminal.Location = new Point(3, 19);
            _mainTerminal.Margin = new Padding(4, 3, 4, 3);
            _mainTerminal.Name = "_mainTerminal";
            _mainTerminal.ReadOnly = true;
            _mainTerminal.Size = new Size(738, 218);
            _mainTerminal.TabIndex = 1;
            _mainTerminal.Text = "";
            _mainTerminal.WordWrap = false;
            // 
            // _mainTabControl
            // 
            _mainTabControl.Controls.Add(_mainTabControl_MainPage);
            _mainTabControl.Controls.Add(_mainTabControl_SettingsPage);
            _mainTabControl.DisplayStyle = TabStyle.Dark;
            _mainTabControl.DisplayStyleProvider.BackgroundColor = Color.FromArgb(48, 48, 48);
            _mainTabControl.DisplayStyleProvider.BackgroundColorDisabled = Color.DimGray;
            _mainTabControl.DisplayStyleProvider.BackgroundColorHot = Color.FromArgb(61, 61, 61);
            _mainTabControl.DisplayStyleProvider.BackgroundColorSelected = Color.FromArgb(31, 31, 31);
            _mainTabControl.DisplayStyleProvider.BorderColor = Color.FromArgb(66, 66, 66);
            _mainTabControl.DisplayStyleProvider.BorderColorHot = Color.FromArgb(172, 172, 172);
            _mainTabControl.DisplayStyleProvider.BorderColorSelected = Color.FromArgb(250, 250, 250);
            _mainTabControl.DisplayStyleProvider.CloserColor = Color.FromArgb(255, 58, 58);
            _mainTabControl.DisplayStyleProvider.CloserColorActive = Color.FromArgb(255, 0, 0);
            _mainTabControl.DisplayStyleProvider.FocusColor = Color.FromArgb(250, 250, 250);
            _mainTabControl.DisplayStyleProvider.FocusColorSecondary = Color.FromArgb(250, 250, 250);
            _mainTabControl.DisplayStyleProvider.FocusTrack = true;
            _mainTabControl.DisplayStyleProvider.HotTrack = true;
            _mainTabControl.DisplayStyleProvider.ImageAlign = ContentAlignment.MiddleLeft;
            _mainTabControl.DisplayStyleProvider.Opacity = 1F;
            _mainTabControl.DisplayStyleProvider.Overlap = 0;
            _mainTabControl.DisplayStyleProvider.Padding = new Point(6, 3);
            _mainTabControl.DisplayStyleProvider.Radius = 10;
            _mainTabControl.DisplayStyleProvider.ShowTabCloser = false;
            _mainTabControl.DisplayStyleProvider.TextColor = Color.FromArgb(214, 214, 214);
            _mainTabControl.DisplayStyleProvider.TextColorDisabled = Color.FromArgb(64, 64, 64);
            _mainTabControl.DisplayStyleProvider.TextColorHot = Color.FromArgb(250, 250, 250);
            _mainTabControl.DisplayStyleProvider.TextColorSelected = Color.FromArgb(250, 250, 250);
            _mainTabControl.Location = new Point(0, 40);
            _mainTabControl.Margin = new Padding(3, 15, 3, 3);
            _mainTabControl.Name = "_mainTabControl";
            _mainTabControl.SelectedIndex = 0;
            _mainTabControl.Size = new Size(764, 501);
            _mainTabControl.TabBackgroundColor = Color.FromArgb(48, 48, 48);
            _mainTabControl.TabBackgroundColorDisabled = Color.DimGray;
            _mainTabControl.TabBackgroundColorHot = Color.FromArgb(61, 61, 61);
            _mainTabControl.TabBackgroundColorSelected = Color.FromArgb(31, 31, 31);
            _mainTabControl.TabBorderColor = Color.FromArgb(66, 66, 66);
            _mainTabControl.TabBorderColorHot = Color.FromArgb(172, 172, 172);
            _mainTabControl.TabBorderColorSelected = Color.FromArgb(250, 250, 250);
            _mainTabControl.TabCloserColor = Color.FromArgb(255, 58, 58);
            _mainTabControl.TabCloserColorActive = Color.FromArgb(255, 0, 0);
            _mainTabControl.TabFocusColor = Color.FromArgb(250, 250, 250);
            _mainTabControl.TabFocusColorSecondary = Color.FromArgb(250, 250, 250);
            _mainTabControl.TabFocusTrack = true;
            _mainTabControl.TabIndex = 6;
            _mainTabControl.TabRadius = 10;
            _mainTabControl.TabTextColor = Color.FromArgb(214, 214, 214);
            _mainTabControl.TabTextColorDisabled = Color.FromArgb(64, 64, 64);
            _mainTabControl.TabTextColorHot = Color.FromArgb(250, 250, 250);
            _mainTabControl.TabTextColorSelected = Color.FromArgb(250, 250, 250);
            // 
            // _mainTabControl_SettingsPage
            // 
            _mainTabControl_SettingsPage.BackColor = Color.FromArgb(31, 31, 31);
            _mainTabControl.SetForbidClose(_mainTabControl_SettingsPage, true);
            _mainTabControl_SettingsPage.ForeColor = Color.FromArgb(214, 214, 214);
            _mainTabControl_SettingsPage.Location = new Point(4, 26);
            _mainTabControl_SettingsPage.Name = "_mainTabControl_SettingsPage";
            _mainTabControl_SettingsPage.Padding = new Padding(3);
            _mainTabControl_SettingsPage.Size = new Size(756, 471);
            _mainTabControl_SettingsPage.TabIndex = 1;
            _mainTabControl_SettingsPage.Text = "Настройки";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(31, 31, 31);
            ClientSize = new Size(764, 541);
            Controls.Add(_mainTabControl);
            Controls.Add(_mainToolStrip);
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(224, 224, 224);
            MaximumSize = new Size(780, 580);
            MinimumSize = new Size(780, 580);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AlgoDev 1.0 - программа для имитации работы моделей абстрактных алгоритмов и автоматов";
            Load += MainForm_Load;
            _mainToolStrip.ResumeLayout(false);
            _mainToolStrip.PerformLayout();
            _mainTabControl_MainPage.ResumeLayout(false);
            _mainTLP.ResumeLayout(false);
            _subTLP.ResumeLayout(false);
            _pagesGB.ResumeLayout(false);
            _modelSelectGB.ResumeLayout(false);
            _modelSelectGB.PerformLayout();
            _mainPage_TerminalGB.ResumeLayout(false);
            _mainTabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private UI.Elements.Controls.CustomizableToolStrip.CustomizableToolStrip _mainToolStrip;
        private ToolStripDropDownButton _mainToolStrip_File;
        private ToolStripDropDownButton _mainToolStrip_View;
        private ToolStripMenuItem _mainToolStrip_File_Open;
        private ToolStripSeparator _mainToolStrip_File_S1;
        private ToolStripMenuItem _mainToolStrip_File_Save;
        private ToolStripMenuItem _mainToolStrip_File_SaveAs;
        private TabPage _mainTabControl_MainPage;
        private TableLayoutPanel _mainTLP;
        private GroupBox _mainPage_TerminalGB;
        private UI.Elements.Controls.CustomizableTabControl.CustomizableTabControl _mainTabControl;
        private TabPage _mainTabControl_SettingsPage;
        private TableLayoutPanel _subTLP;
        private GroupBox _pagesGB;
        private FlowLayoutPanel _pagesFLP;
        private GroupBox _modelSelectGB;
        private Button _createWorkspacePageB;
        private Label _labelWorkspaceModelType;
        private ComboBox _workspaceModelTypeCB;
        public RichTextBox _mainTerminal;
    }
}
