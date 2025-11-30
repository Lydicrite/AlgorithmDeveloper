using AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl.Styles;
using System.Windows.Forms;

namespace AlgorithmDeveloper
{
    partial class MainFormV2
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
            components = new System.ComponentModel.Container();
            _mainTabControl = new AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl.CustomTabControl();
            _mainTabControl_MainPage = new TabPage();
            _mainScrollPanel = new Panel();
            _mainTLP = new TableLayoutPanel();
            _mainGB = new GroupBox();
            _lasKeyboardGB = new GroupBox();
            lasGraphicKeyboard1 = new AlgorithmDeveloper.Resources.UI.Controls.LASInputControls.LASGraphicKeyboard();
            _createGAS2_GB = new GroupBox();
            _tryParseLAS2_B = new Button();
            _createGAS2_B = new Button();
            _lasInputRTB2 = new AlgorithmDeveloper.Resources.UI.Controls.LASInputRichTextBox();
            _createGAS1_GB = new GroupBox();
            _tryParseLAS1_B = new Button();
            _createGAS1_B = new Button();
            _lasInputRTB1 = new AlgorithmDeveloper.Resources.UI.Controls.LASInputRichTextBox();
            _combinedAlgoGB = new GroupBox();
            _createGAS3_B = new Button();
            _lasInputRTB3 = new AlgorithmDeveloper.Resources.UI.Controls.LASInputRichTextBox();
            _masGB = new GroupBox();
            _masFLP = new FlowLayoutPanel();
            _infoGB = new GroupBox();
            _mainInfoPort = new RichTextBox();
            _mainTabControl_GAS1 = new TabPage();
            _algoController1 = new AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling.AAController();
            _mainTabControl_GAS2 = new TabPage();
            _algoController2 = new AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling.AAController();
            _mainTabControl_GAS3 = new TabPage();
            _algoController3 = new AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling.AAController();
            _mainTabControl_SettingsPage = new TabPage();
            settingsTLP = new TableLayoutPanel();
            settingsLeftFLP = new FlowLayoutPanel();
            _workFieldParamsGB = new GroupBox();
            label15 = new Label();
            drawStepDelayNUD = new NumericUpDown();
            _viewportCP = new PictureBox();
            label14 = new Label();
            _verticesVisualParamsGB = new GroupBox();
            _vertexActInnerCP = new PictureBox();
            label1 = new Label();
            _vertexInactInnerCP = new PictureBox();
            label8 = new Label();
            label6 = new Label();
            label7 = new Label();
            label5 = new Label();
            _inactiveBorderCP = new PictureBox();
            _activeBorderCP = new PictureBox();
            label4 = new Label();
            _borderNUD = new NumericUpDown();
            _vertexSizeNUD = new NumericUpDown();
            _transitionsVisualParamsGB = new GroupBox();
            _inactTransitionsGB = new GroupBox();
            label12 = new Label();
            label13 = new Label();
            _transitionDarkPenCP = new PictureBox();
            _transitionDarkPenNUD = new NumericUpDown();
            _actTransitionsGB = new GroupBox();
            label10 = new Label();
            label11 = new Label();
            _transitionLightPenCP = new PictureBox();
            _transitionLightPenNUD = new NumericUpDown();
            settingsRightFLP = new FlowLayoutPanel();
            _toolTip = new ToolTip(components);
            _colorPicker = new ColorDialog();
            _mainTabControl.SuspendLayout();
            _mainTabControl_MainPage.SuspendLayout();
            _mainScrollPanel.SuspendLayout();
            _mainTLP.SuspendLayout();
            _mainGB.SuspendLayout();
            _lasKeyboardGB.SuspendLayout();
            _createGAS2_GB.SuspendLayout();
            _createGAS1_GB.SuspendLayout();
            _combinedAlgoGB.SuspendLayout();
            _masGB.SuspendLayout();
            _infoGB.SuspendLayout();
            _mainTabControl_GAS1.SuspendLayout();
            _mainTabControl_GAS2.SuspendLayout();
            _mainTabControl_GAS3.SuspendLayout();
            _mainTabControl_SettingsPage.SuspendLayout();
            settingsTLP.SuspendLayout();
            settingsLeftFLP.SuspendLayout();
            _workFieldParamsGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)drawStepDelayNUD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_viewportCP).BeginInit();
            _verticesVisualParamsGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_vertexActInnerCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_vertexInactInnerCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_inactiveBorderCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_activeBorderCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_borderNUD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_vertexSizeNUD).BeginInit();
            _transitionsVisualParamsGB.SuspendLayout();
            _inactTransitionsGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_transitionDarkPenCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_transitionDarkPenNUD).BeginInit();
            _actTransitionsGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_transitionLightPenCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_transitionLightPenNUD).BeginInit();
            SuspendLayout();
            // 
            // _mainTabControl
            // 
            _mainTabControl.Controls.Add(_mainTabControl_MainPage);
            _mainTabControl.Controls.Add(_mainTabControl_GAS1);
            _mainTabControl.Controls.Add(_mainTabControl_GAS2);
            _mainTabControl.Controls.Add(_mainTabControl_GAS3);
            _mainTabControl.Controls.Add(_mainTabControl_SettingsPage);
            _mainTabControl.Cursor = Cursors.Hand;
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
            _mainTabControl.Dock = DockStyle.Fill;
            _mainTabControl.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _mainTabControl.Location = new Point(0, 0);
            _mainTabControl.Margin = new Padding(1);
            _mainTabControl.Name = "_mainTabControl";
            _mainTabControl.SelectedIndex = 0;
            _mainTabControl.Size = new Size(779, 556);
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
            // _mainTabControl_MainPage
            // 
            _mainTabControl_MainPage.BackColor = Color.FromArgb(31, 31, 31);
            _mainTabControl_MainPage.Controls.Add(_mainScrollPanel);
            _mainTabControl.SetForbidClose(_mainTabControl_MainPage, true);
            _mainTabControl.SetForbidEntry(_mainTabControl_MainPage, true);
            _mainTabControl.SetForbidHide(_mainTabControl_MainPage, true);
            _mainTabControl_MainPage.ForeColor = Color.FromArgb(214, 214, 214);
            _mainTabControl_MainPage.Location = new Point(4, 31);
            _mainTabControl_MainPage.Name = "_mainTabControl_MainPage";
            _mainTabControl_MainPage.Padding = new Padding(3);
            _mainTabControl_MainPage.Size = new Size(771, 521);
            _mainTabControl_MainPage.TabIndex = 0;
            _mainTabControl_MainPage.Text = "Главное меню";
            // 
            // _mainScrollPanel
            // 
            _mainScrollPanel.AutoScroll = true;
            _mainScrollPanel.Controls.Add(_mainTLP);
            _mainScrollPanel.Dock = DockStyle.Fill;
            _mainScrollPanel.Location = new Point(3, 3);
            _mainScrollPanel.Name = "_mainScrollPanel";
            _mainScrollPanel.Size = new Size(765, 515);
            _mainScrollPanel.TabIndex = 0;
            // 
            // _mainTLP
            // 
            _mainTLP.AutoSize = true;
            _mainTLP.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _mainTLP.ColumnCount = 1;
            _mainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _mainTLP.Controls.Add(_mainGB, 0, 0);
            _mainTLP.Controls.Add(_combinedAlgoGB, 0, 1);
            _mainTLP.Controls.Add(_masGB, 0, 2);
            _mainTLP.Controls.Add(_infoGB, 0, 3);
            _mainTLP.Dock = DockStyle.Top;
            _mainTLP.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            _mainTLP.Location = new Point(0, 0);
            _mainTLP.Name = "_mainTLP";
            _mainTLP.RowCount = 4;
            _mainTLP.RowStyles.Add(new RowStyle());
            _mainTLP.RowStyles.Add(new RowStyle());
            _mainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 350F));
            _mainTLP.RowStyles.Add(new RowStyle());
            _mainTLP.Size = new Size(748, 989);
            _mainTLP.TabIndex = 0;
            // 
            // _mainGB
            // 
            _mainGB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _mainGB.Controls.Add(_lasKeyboardGB);
            _mainGB.Controls.Add(_createGAS2_GB);
            _mainGB.Controls.Add(_createGAS1_GB);
            _mainGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _mainGB.ForeColor = Color.FromArgb(224, 224, 224);
            _mainGB.Location = new Point(3, 3);
            _mainGB.MinimumSize = new Size(740, 276);
            _mainGB.Name = "_mainGB";
            _mainGB.Size = new Size(742, 276);
            _mainGB.TabIndex = 8;
            _mainGB.TabStop = false;
            _mainGB.Text = "Объединяемые алгоритмы";
            // 
            // _lasKeyboardGB
            // 
            _lasKeyboardGB.Controls.Add(lasGraphicKeyboard1);
            _lasKeyboardGB.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _lasKeyboardGB.ForeColor = Color.FromArgb(224, 224, 224);
            _lasKeyboardGB.Location = new Point(6, 26);
            _lasKeyboardGB.Name = "_lasKeyboardGB";
            _lasKeyboardGB.Size = new Size(163, 240);
            _lasKeyboardGB.TabIndex = 14;
            _lasKeyboardGB.TabStop = false;
            _lasKeyboardGB.Text = "Клавиатура ввода ЛСА";
            // 
            // lasGraphicKeyboard1
            // 
            lasGraphicKeyboard1.BackColor = Color.FromArgb(31, 31, 31);
            lasGraphicKeyboard1.Font = new Font("Segoe UI", 8.25F);
            lasGraphicKeyboard1.ForeColor = Color.FromArgb(224, 224, 224);
            lasGraphicKeyboard1.Location = new Point(12, 24);
            lasGraphicKeyboard1.MaximumSize = new Size(139, 205);
            lasGraphicKeyboard1.MinimumSize = new Size(139, 205);
            lasGraphicKeyboard1.Name = "lasGraphicKeyboard1";
            lasGraphicKeyboard1.Size = new Size(139, 205);
            lasGraphicKeyboard1.TabIndex = 0;
            // 
            // _createGAS2_GB
            // 
            _createGAS2_GB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _createGAS2_GB.Controls.Add(_tryParseLAS2_B);
            _createGAS2_GB.Controls.Add(_createGAS2_B);
            _createGAS2_GB.Controls.Add(_lasInputRTB2);
            _createGAS2_GB.Font = new Font("Segoe UI", 9.75F);
            _createGAS2_GB.ForeColor = Color.FromArgb(224, 224, 224);
            _createGAS2_GB.Location = new Point(175, 149);
            _createGAS2_GB.Name = "_createGAS2_GB";
            _createGAS2_GB.Size = new Size(561, 117);
            _createGAS2_GB.TabIndex = 11;
            _createGAS2_GB.TabStop = false;
            _createGAS2_GB.Text = "Ввод и обработка ЛСА второго алгоритма";
            // 
            // _tryParseLAS2_B
            // 
            _tryParseLAS2_B.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _tryParseLAS2_B.BackColor = Color.FromArgb(64, 64, 64);
            _tryParseLAS2_B.BackgroundImageLayout = ImageLayout.None;
            _tryParseLAS2_B.Cursor = Cursors.Hand;
            _tryParseLAS2_B.Enabled = false;
            _tryParseLAS2_B.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _tryParseLAS2_B.FlatStyle = FlatStyle.Flat;
            _tryParseLAS2_B.Font = new Font("Segoe UI", 9.75F);
            _tryParseLAS2_B.Location = new Point(350, 17);
            _tryParseLAS2_B.Name = "_tryParseLAS2_B";
            _tryParseLAS2_B.Size = new Size(205, 40);
            _tryParseLAS2_B.TabIndex = 9;
            _tryParseLAS2_B.Text = "Проверить введённую ЛСА";
            _tryParseLAS2_B.UseVisualStyleBackColor = false;
            _tryParseLAS2_B.Click += CheckLAS;
            // 
            // _createGAS2_B
            // 
            _createGAS2_B.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _createGAS2_B.BackColor = Color.FromArgb(64, 64, 64);
            _createGAS2_B.BackgroundImageLayout = ImageLayout.None;
            _createGAS2_B.Cursor = Cursors.Hand;
            _createGAS2_B.Enabled = false;
            _createGAS2_B.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _createGAS2_B.FlatStyle = FlatStyle.Flat;
            _createGAS2_B.Font = new Font("Segoe UI", 9.75F);
            _createGAS2_B.Location = new Point(350, 65);
            _createGAS2_B.Margin = new Padding(3, 1, 3, 3);
            _createGAS2_B.Name = "_createGAS2_B";
            _createGAS2_B.Size = new Size(205, 40);
            _createGAS2_B.TabIndex = 7;
            _createGAS2_B.Text = "Построить модель алгоритма";
            _createGAS2_B.UseVisualStyleBackColor = false;
            _createGAS2_B.Click += CreageInitialGAS;
            // 
            // _lasInputRTB2
            // 
            _lasInputRTB2.AllowDrop = true;
            _lasInputRTB2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _lasInputRTB2.BackColor = Color.FromArgb(48, 48, 48);
            _lasInputRTB2.BorderStyle = BorderStyle.None;
            _lasInputRTB2.ForeColor = Color.FromArgb(224, 224, 224);
            _lasInputRTB2.Location = new Point(6, 17);
            _lasInputRTB2.Name = "_lasInputRTB2";
            _lasInputRTB2.PlaceholderColor = Color.FromArgb(96, 96, 96);
            _lasInputRTB2.PlaceholderText = "Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк";
            _lasInputRTB2.ShortcutsEnabled = false;
            _lasInputRTB2.Size = new Size(338, 88);
            _lasInputRTB2.TabIndex = 8;
            _lasInputRTB2.Text = "";
            _lasInputRTB2.TextChanged += EnableCheckLASButton;
            // 
            // _createGAS1_GB
            // 
            _createGAS1_GB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _createGAS1_GB.Controls.Add(_tryParseLAS1_B);
            _createGAS1_GB.Controls.Add(_createGAS1_B);
            _createGAS1_GB.Controls.Add(_lasInputRTB1);
            _createGAS1_GB.Font = new Font("Segoe UI", 9.75F);
            _createGAS1_GB.ForeColor = Color.FromArgb(224, 224, 224);
            _createGAS1_GB.Location = new Point(175, 26);
            _createGAS1_GB.Margin = new Padding(3, 14, 3, 3);
            _createGAS1_GB.Name = "_createGAS1_GB";
            _createGAS1_GB.Size = new Size(561, 117);
            _createGAS1_GB.TabIndex = 10;
            _createGAS1_GB.TabStop = false;
            _createGAS1_GB.Text = "Ввод и обработка ЛСА первого алгоритма";
            // 
            // _tryParseLAS1_B
            // 
            _tryParseLAS1_B.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _tryParseLAS1_B.BackColor = Color.FromArgb(64, 64, 64);
            _tryParseLAS1_B.BackgroundImageLayout = ImageLayout.None;
            _tryParseLAS1_B.Cursor = Cursors.Hand;
            _tryParseLAS1_B.Enabled = false;
            _tryParseLAS1_B.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _tryParseLAS1_B.FlatStyle = FlatStyle.Flat;
            _tryParseLAS1_B.Font = new Font("Segoe UI", 9.75F);
            _tryParseLAS1_B.Location = new Point(350, 17);
            _tryParseLAS1_B.Name = "_tryParseLAS1_B";
            _tryParseLAS1_B.Size = new Size(205, 40);
            _tryParseLAS1_B.TabIndex = 9;
            _tryParseLAS1_B.Text = "Проверить введённую ЛСА";
            _tryParseLAS1_B.UseVisualStyleBackColor = false;
            _tryParseLAS1_B.Click += CheckLAS;
            // 
            // _createGAS1_B
            // 
            _createGAS1_B.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _createGAS1_B.BackColor = Color.FromArgb(64, 64, 64);
            _createGAS1_B.BackgroundImageLayout = ImageLayout.None;
            _createGAS1_B.Cursor = Cursors.Hand;
            _createGAS1_B.Enabled = false;
            _createGAS1_B.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _createGAS1_B.FlatStyle = FlatStyle.Flat;
            _createGAS1_B.Font = new Font("Segoe UI", 9.75F);
            _createGAS1_B.Location = new Point(350, 65);
            _createGAS1_B.Margin = new Padding(3, 1, 3, 3);
            _createGAS1_B.Name = "_createGAS1_B";
            _createGAS1_B.Size = new Size(205, 40);
            _createGAS1_B.TabIndex = 7;
            _createGAS1_B.Text = "Построить модель алгоритма";
            _createGAS1_B.UseVisualStyleBackColor = false;
            _createGAS1_B.Click += CreageInitialGAS;
            // 
            // _lasInputRTB1
            // 
            _lasInputRTB1.AllowDrop = true;
            _lasInputRTB1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _lasInputRTB1.BackColor = Color.FromArgb(48, 48, 48);
            _lasInputRTB1.BorderStyle = BorderStyle.None;
            _lasInputRTB1.ForeColor = Color.FromArgb(224, 224, 224);
            _lasInputRTB1.Location = new Point(6, 17);
            _lasInputRTB1.Name = "_lasInputRTB1";
            _lasInputRTB1.PlaceholderColor = Color.FromArgb(96, 96, 96);
            _lasInputRTB1.PlaceholderText = "Yн X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк";
            _lasInputRTB1.ShortcutsEnabled = false;
            _lasInputRTB1.Size = new Size(338, 88);
            _lasInputRTB1.TabIndex = 8;
            _lasInputRTB1.Text = "";
            _lasInputRTB1.TextChanged += EnableCheckLASButton;
            // 
            // _combinedAlgoGB
            // 
            _combinedAlgoGB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _combinedAlgoGB.Controls.Add(_createGAS3_B);
            _combinedAlgoGB.Controls.Add(_lasInputRTB3);
            _combinedAlgoGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _combinedAlgoGB.ForeColor = Color.FromArgb(224, 224, 224);
            _combinedAlgoGB.Location = new Point(3, 296);
            _combinedAlgoGB.Margin = new Padding(3, 14, 3, 3);
            _combinedAlgoGB.MinimumSize = new Size(740, 123);
            _combinedAlgoGB.Name = "_combinedAlgoGB";
            _combinedAlgoGB.Size = new Size(742, 123);
            _combinedAlgoGB.TabIndex = 13;
            _combinedAlgoGB.TabStop = false;
            _combinedAlgoGB.Text = "Объединённый алгоритм";
            // 
            // _createGAS3_B
            // 
            _createGAS3_B.BackColor = Color.FromArgb(64, 64, 64);
            _createGAS3_B.BackgroundImageLayout = ImageLayout.None;
            _createGAS3_B.Cursor = Cursors.Hand;
            _createGAS3_B.Enabled = false;
            _createGAS3_B.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _createGAS3_B.FlatStyle = FlatStyle.Flat;
            _createGAS3_B.Font = new Font("Segoe UI", 9.75F);
            _createGAS3_B.Location = new Point(6, 23);
            _createGAS3_B.Margin = new Padding(3, 1, 3, 3);
            _createGAS3_B.Name = "_createGAS3_B";
            _createGAS3_B.Size = new Size(240, 88);
            _createGAS3_B.TabIndex = 7;
            _createGAS3_B.Text = "Провести объединение и построить модель объединённого алгоритма";
            _createGAS3_B.UseVisualStyleBackColor = false;
            _createGAS3_B.Click += CreateCombinedGAS;
            // 
            // _lasInputRTB3
            // 
            _lasInputRTB3.AllowDrop = true;
            _lasInputRTB3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _lasInputRTB3.BackColor = Color.FromArgb(48, 48, 48);
            _lasInputRTB3.BorderStyle = BorderStyle.None;
            _lasInputRTB3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _lasInputRTB3.ForeColor = Color.FromArgb(224, 224, 224);
            _lasInputRTB3.Location = new Point(252, 23);
            _lasInputRTB3.Margin = new Padding(3, 1, 3, 3);
            _lasInputRTB3.Name = "_lasInputRTB3";
            _lasInputRTB3.PlaceholderColor = Color.FromArgb(96, 96, 96);
            _lasInputRTB3.PlaceholderText = "Здесь появится ЛСА объединённого алгоритма";
            _lasInputRTB3.ShortcutsEnabled = false;
            _lasInputRTB3.Size = new Size(484, 88);
            _lasInputRTB3.TabIndex = 8;
            _lasInputRTB3.Text = "";
            // 
            // _masGB
            // 
            _masGB.Controls.Add(_masFLP);
            _masGB.Dock = DockStyle.Fill;
            _masGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _masGB.ForeColor = Color.FromArgb(224, 224, 224);
            _masGB.Location = new Point(3, 436);
            _masGB.Margin = new Padding(3, 14, 3, 3);
            _masGB.MinimumSize = new Size(740, 179);
            _masGB.Name = "_masGB";
            _masGB.Size = new Size(742, 333);
            _masGB.TabIndex = 14;
            _masGB.TabStop = false;
            _masGB.Text = "Матричные схемы алгоритмов";
            // 
            // _masFLP
            // 
            _masFLP.AutoScroll = true;
            _masFLP.Dock = DockStyle.Fill;
            _masFLP.FlowDirection = FlowDirection.TopDown;
            _masFLP.Location = new Point(3, 23);
            _masFLP.Name = "_masFLP";
            _masFLP.Size = new Size(736, 307);
            _masFLP.TabIndex = 0;
            _masFLP.WrapContents = false;
            // 
            // _infoGB
            // 
            _infoGB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _infoGB.Controls.Add(_mainInfoPort);
            _infoGB.ForeColor = Color.FromArgb(224, 224, 224);
            _infoGB.Location = new Point(3, 786);
            _infoGB.Margin = new Padding(3, 14, 3, 3);
            _infoGB.MinimumSize = new Size(740, 200);
            _infoGB.Name = "_infoGB";
            _infoGB.Size = new Size(742, 200);
            _infoGB.TabIndex = 15;
            _infoGB.TabStop = false;
            _infoGB.Text = "Журнал";
            // 
            // _mainInfoPort
            // 
            _mainInfoPort.BackColor = Color.FromArgb(16, 16, 16);
            _mainInfoPort.BorderStyle = BorderStyle.FixedSingle;
            _mainInfoPort.DetectUrls = false;
            _mainInfoPort.Dock = DockStyle.Fill;
            _mainInfoPort.Font = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _mainInfoPort.ForeColor = Color.FromArgb(224, 224, 224);
            _mainInfoPort.Location = new Point(3, 25);
            _mainInfoPort.Margin = new Padding(3, 5, 3, 3);
            _mainInfoPort.Name = "_mainInfoPort";
            _mainInfoPort.ReadOnly = true;
            _mainInfoPort.Size = new Size(736, 172);
            _mainInfoPort.TabIndex = 3;
            _mainInfoPort.Text = "";
            _mainInfoPort.WordWrap = false;
            // 
            // _mainTabControl_GAS1
            // 
            _mainTabControl_GAS1.BackColor = Color.FromArgb(31, 31, 31);
            _mainTabControl_GAS1.Controls.Add(_algoController1);
            _mainTabControl_GAS1.Location = new Point(4, 26);
            _mainTabControl_GAS1.Name = "_mainTabControl_GAS1";
            _mainTabControl_GAS1.Size = new Size(192, 70);
            _mainTabControl_GAS1.TabIndex = 2;
            _mainTabControl_GAS1.Text = "ГСА 1";
            // 
            // _algoController1
            // 
            _algoController1.BackColor = Color.FromArgb(31, 31, 31);
            _algoController1.Dock = DockStyle.Fill;
            _algoController1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _algoController1.ForeColor = Color.FromArgb(224, 224, 224);
            _algoController1.Location = new Point(0, 0);
            _algoController1.Name = "_algoController1";
            _algoController1.Size = new Size(192, 70);
            _algoController1.TabIndex = 0;
            // 
            // _mainTabControl_GAS2
            // 
            _mainTabControl_GAS2.BackColor = Color.FromArgb(31, 31, 31);
            _mainTabControl_GAS2.Controls.Add(_algoController2);
            _mainTabControl_GAS2.Location = new Point(4, 26);
            _mainTabControl_GAS2.Name = "_mainTabControl_GAS2";
            _mainTabControl_GAS2.Size = new Size(192, 70);
            _mainTabControl_GAS2.TabIndex = 3;
            _mainTabControl_GAS2.Text = "ГСА 2";
            // 
            // _algoController2
            // 
            _algoController2.BackColor = Color.FromArgb(31, 31, 31);
            _algoController2.Dock = DockStyle.Fill;
            _algoController2.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _algoController2.ForeColor = Color.FromArgb(224, 224, 224);
            _algoController2.Location = new Point(0, 0);
            _algoController2.Name = "_algoController2";
            _algoController2.Size = new Size(192, 70);
            _algoController2.TabIndex = 1;
            // 
            // _mainTabControl_GAS3
            // 
            _mainTabControl_GAS3.BackColor = Color.FromArgb(31, 31, 31);
            _mainTabControl_GAS3.Controls.Add(_algoController3);
            _mainTabControl_GAS3.Location = new Point(4, 26);
            _mainTabControl_GAS3.Name = "_mainTabControl_GAS3";
            _mainTabControl_GAS3.Size = new Size(192, 70);
            _mainTabControl_GAS3.TabIndex = 4;
            _mainTabControl_GAS3.Text = "ГСА 3";
            // 
            // _algoController3
            // 
            _algoController3.BackColor = Color.FromArgb(31, 31, 31);
            _algoController3.Dock = DockStyle.Fill;
            _algoController3.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _algoController3.ForeColor = Color.FromArgb(224, 224, 224);
            _algoController3.Location = new Point(0, 0);
            _algoController3.Name = "_algoController3";
            _algoController3.Size = new Size(192, 70);
            _algoController3.TabIndex = 1;
            // 
            // _mainTabControl_SettingsPage
            // 
            _mainTabControl_SettingsPage.BackColor = Color.FromArgb(31, 31, 31);
            _mainTabControl_SettingsPage.Controls.Add(settingsTLP);
            _mainTabControl.SetForbidClose(_mainTabControl_SettingsPage, true);
            _mainTabControl_SettingsPage.ForeColor = Color.FromArgb(214, 214, 214);
            _mainTabControl_SettingsPage.Location = new Point(4, 31);
            _mainTabControl_SettingsPage.Name = "_mainTabControl_SettingsPage";
            _mainTabControl_SettingsPage.Padding = new Padding(3);
            _mainTabControl_SettingsPage.Size = new Size(771, 521);
            _mainTabControl_SettingsPage.TabIndex = 1;
            _mainTabControl_SettingsPage.Text = "Настройки";
            // 
            // settingsTLP
            // 
            settingsTLP.ColumnCount = 2;
            settingsTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            settingsTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            settingsTLP.Controls.Add(settingsLeftFLP, 0, 0);
            settingsTLP.Controls.Add(settingsRightFLP, 1, 0);
            settingsTLP.Dock = DockStyle.Fill;
            settingsTLP.Location = new Point(3, 3);
            settingsTLP.Name = "settingsTLP";
            settingsTLP.RowCount = 1;
            settingsTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            settingsTLP.Size = new Size(765, 515);
            settingsTLP.TabIndex = 0;
            // 
            // settingsLeftFLP
            // 
            settingsLeftFLP.AutoScroll = true;
            settingsLeftFLP.Controls.Add(_workFieldParamsGB);
            settingsLeftFLP.Controls.Add(_verticesVisualParamsGB);
            settingsLeftFLP.Controls.Add(_transitionsVisualParamsGB);
            settingsLeftFLP.Dock = DockStyle.Fill;
            settingsLeftFLP.FlowDirection = FlowDirection.TopDown;
            settingsLeftFLP.Location = new Point(3, 3);
            settingsLeftFLP.Name = "settingsLeftFLP";
            settingsLeftFLP.Size = new Size(376, 509);
            settingsLeftFLP.TabIndex = 0;
            settingsLeftFLP.WrapContents = false;
            // 
            // _workFieldParamsGB
            // 
            _workFieldParamsGB.Controls.Add(label15);
            _workFieldParamsGB.Controls.Add(drawStepDelayNUD);
            _workFieldParamsGB.Controls.Add(_viewportCP);
            _workFieldParamsGB.Controls.Add(label14);
            _workFieldParamsGB.FlatStyle = FlatStyle.Popup;
            _workFieldParamsGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _workFieldParamsGB.ForeColor = Color.LightGray;
            _workFieldParamsGB.Location = new Point(3, 3);
            _workFieldParamsGB.Margin = new Padding(3, 3, 10, 3);
            _workFieldParamsGB.Name = "_workFieldParamsGB";
            _workFieldParamsGB.Size = new Size(346, 97);
            _workFieldParamsGB.TabIndex = 36;
            _workFieldParamsGB.TabStop = false;
            _workFieldParamsGB.Text = "Параметры рабочей области";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 9.75F);
            label15.ForeColor = Color.Gainsboro;
            label15.Location = new Point(9, 59);
            label15.Margin = new Padding(1, 0, 1, 0);
            label15.Name = "label15";
            label15.Size = new Size(169, 17);
            label15.TabIndex = 32;
            label15.Text = "Шаг отрисовки работы (c):";
            // 
            // drawStepDelayNUD
            // 
            drawStepDelayNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            drawStepDelayNUD.BackColor = Color.FromArgb(48, 48, 48);
            drawStepDelayNUD.DecimalPlaces = 2;
            drawStepDelayNUD.Enabled = false;
            drawStepDelayNUD.Font = new Font("Segoe UI", 9.75F);
            drawStepDelayNUD.ForeColor = Color.Gainsboro;
            drawStepDelayNUD.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            drawStepDelayNUD.Location = new Point(263, 53);
            drawStepDelayNUD.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            drawStepDelayNUD.MaximumSize = new Size(77, 0);
            drawStepDelayNUD.Minimum = new decimal(new int[] { 10, 0, 0, 131072 });
            drawStepDelayNUD.MinimumSize = new Size(77, 0);
            drawStepDelayNUD.Name = "drawStepDelayNUD";
            drawStepDelayNUD.Size = new Size(77, 25);
            drawStepDelayNUD.TabIndex = 31;
            drawStepDelayNUD.Value = new decimal(new int[] { 75, 0, 0, 131072 });
            // 
            // _viewportCP
            // 
            _viewportCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _viewportCP.BackColor = Color.FromArgb(96, 96, 96);
            _viewportCP.BorderStyle = BorderStyle.Fixed3D;
            _viewportCP.Enabled = false;
            _viewportCP.Location = new Point(263, 22);
            _viewportCP.MaximumSize = new Size(77, 25);
            _viewportCP.MinimumSize = new Size(77, 25);
            _viewportCP.Name = "_viewportCP";
            _viewportCP.Size = new Size(77, 25);
            _viewportCP.TabIndex = 30;
            _viewportCP.TabStop = false;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 9.75F);
            label14.ForeColor = Color.Gainsboro;
            label14.Location = new Point(9, 30);
            label14.Margin = new Padding(1, 0, 1, 0);
            label14.Name = "label14";
            label14.Size = new Size(148, 17);
            label14.TabIndex = 28;
            label14.Text = "Цвет рабочей области:";
            // 
            // _verticesVisualParamsGB
            // 
            _verticesVisualParamsGB.Controls.Add(_vertexActInnerCP);
            _verticesVisualParamsGB.Controls.Add(label1);
            _verticesVisualParamsGB.Controls.Add(_vertexInactInnerCP);
            _verticesVisualParamsGB.Controls.Add(label8);
            _verticesVisualParamsGB.Controls.Add(label6);
            _verticesVisualParamsGB.Controls.Add(label7);
            _verticesVisualParamsGB.Controls.Add(label5);
            _verticesVisualParamsGB.Controls.Add(_inactiveBorderCP);
            _verticesVisualParamsGB.Controls.Add(_activeBorderCP);
            _verticesVisualParamsGB.Controls.Add(label4);
            _verticesVisualParamsGB.Controls.Add(_borderNUD);
            _verticesVisualParamsGB.Controls.Add(_vertexSizeNUD);
            _verticesVisualParamsGB.FlatStyle = FlatStyle.Popup;
            _verticesVisualParamsGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _verticesVisualParamsGB.ForeColor = Color.LightGray;
            _verticesVisualParamsGB.Location = new Point(3, 106);
            _verticesVisualParamsGB.Margin = new Padding(3, 3, 10, 3);
            _verticesVisualParamsGB.Name = "_verticesVisualParamsGB";
            _verticesVisualParamsGB.Size = new Size(346, 224);
            _verticesVisualParamsGB.TabIndex = 34;
            _verticesVisualParamsGB.TabStop = false;
            _verticesVisualParamsGB.Text = "Параметры отрисовки вершин";
            // 
            // _vertexActInnerCP
            // 
            _vertexActInnerCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _vertexActInnerCP.BackColor = Color.LightGray;
            _vertexActInnerCP.BorderStyle = BorderStyle.Fixed3D;
            _vertexActInnerCP.Enabled = false;
            _vertexActInnerCP.Location = new Point(263, 181);
            _vertexActInnerCP.MaximumSize = new Size(77, 25);
            _vertexActInnerCP.MinimumSize = new Size(77, 25);
            _vertexActInnerCP.Name = "_vertexActInnerCP";
            _vertexActInnerCP.Size = new Size(77, 25);
            _vertexActInnerCP.TabIndex = 32;
            _vertexActInnerCP.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F);
            label1.ForeColor = Color.Gainsboro;
            label1.Location = new Point(9, 189);
            label1.Margin = new Padding(1, 0, 1, 0);
            label1.Name = "label1";
            label1.Size = new Size(223, 17);
            label1.TabIndex = 31;
            label1.Text = "Цвет заливки неактивной вершины:";
            // 
            // _vertexInactInnerCP
            // 
            _vertexInactInnerCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _vertexInactInnerCP.BackColor = Color.LightGray;
            _vertexInactInnerCP.BorderStyle = BorderStyle.Fixed3D;
            _vertexInactInnerCP.Enabled = false;
            _vertexInactInnerCP.Location = new Point(263, 150);
            _vertexInactInnerCP.MaximumSize = new Size(77, 25);
            _vertexInactInnerCP.MinimumSize = new Size(77, 25);
            _vertexInactInnerCP.Name = "_vertexInactInnerCP";
            _vertexInactInnerCP.Size = new Size(77, 25);
            _vertexInactInnerCP.TabIndex = 30;
            _vertexInactInnerCP.TabStop = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9.75F);
            label8.ForeColor = Color.Gainsboro;
            label8.Location = new Point(9, 158);
            label8.Margin = new Padding(1, 0, 1, 0);
            label8.Name = "label8";
            label8.Size = new Size(209, 17);
            label8.TabIndex = 28;
            label8.Text = "Цвет заливки активной вершины:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.75F);
            label6.ForeColor = Color.Gainsboro;
            label6.Location = new Point(9, 127);
            label6.Margin = new Padding(1, 0, 1, 0);
            label6.Name = "label6";
            label6.Size = new Size(170, 17);
            label6.TabIndex = 24;
            label6.Text = "Цвет неактивной границы: ";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9.75F);
            label7.ForeColor = Color.Gainsboro;
            label7.Location = new Point(9, 96);
            label7.Margin = new Padding(1, 0, 1, 0);
            label7.Name = "label7";
            label7.Size = new Size(152, 17);
            label7.TabIndex = 23;
            label7.Text = "Цвет активной границы:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9.75F);
            label5.ForeColor = Color.Gainsboro;
            label5.Location = new Point(9, 59);
            label5.Margin = new Padding(1, 0, 1, 0);
            label5.Name = "label5";
            label5.Size = new Size(114, 17);
            label5.TabIndex = 17;
            label5.Text = "Ширина границы:";
            // 
            // _inactiveBorderCP
            // 
            _inactiveBorderCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _inactiveBorderCP.BackColor = Color.Black;
            _inactiveBorderCP.BorderStyle = BorderStyle.Fixed3D;
            _inactiveBorderCP.Enabled = false;
            _inactiveBorderCP.Location = new Point(263, 119);
            _inactiveBorderCP.MaximumSize = new Size(77, 25);
            _inactiveBorderCP.MinimumSize = new Size(77, 25);
            _inactiveBorderCP.Name = "_inactiveBorderCP";
            _inactiveBorderCP.Size = new Size(77, 25);
            _inactiveBorderCP.TabIndex = 26;
            _inactiveBorderCP.TabStop = false;
            // 
            // _activeBorderCP
            // 
            _activeBorderCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _activeBorderCP.BackColor = Color.LimeGreen;
            _activeBorderCP.BorderStyle = BorderStyle.Fixed3D;
            _activeBorderCP.Enabled = false;
            _activeBorderCP.Location = new Point(263, 88);
            _activeBorderCP.MaximumSize = new Size(77, 25);
            _activeBorderCP.MinimumSize = new Size(77, 25);
            _activeBorderCP.Name = "_activeBorderCP";
            _activeBorderCP.Size = new Size(77, 25);
            _activeBorderCP.TabIndex = 25;
            _activeBorderCP.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F);
            label4.ForeColor = Color.Gainsboro;
            label4.Location = new Point(9, 28);
            label4.Margin = new Padding(1, 0, 1, 0);
            label4.Name = "label4";
            label4.Size = new Size(115, 17);
            label4.TabIndex = 16;
            label4.Text = "Размер вершины:";
            // 
            // _borderNUD
            // 
            _borderNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _borderNUD.BackColor = Color.FromArgb(48, 48, 48);
            _borderNUD.Enabled = false;
            _borderNUD.Font = new Font("Segoe UI", 9.75F);
            _borderNUD.ForeColor = Color.Gainsboro;
            _borderNUD.Location = new Point(263, 57);
            _borderNUD.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            _borderNUD.MaximumSize = new Size(77, 0);
            _borderNUD.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            _borderNUD.MinimumSize = new Size(77, 0);
            _borderNUD.Name = "_borderNUD";
            _borderNUD.Size = new Size(77, 25);
            _borderNUD.TabIndex = 15;
            _borderNUD.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // _vertexSizeNUD
            // 
            _vertexSizeNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _vertexSizeNUD.BackColor = Color.FromArgb(48, 48, 48);
            _vertexSizeNUD.Enabled = false;
            _vertexSizeNUD.Font = new Font("Segoe UI", 9.75F);
            _vertexSizeNUD.ForeColor = Color.Gainsboro;
            _vertexSizeNUD.Location = new Point(263, 26);
            _vertexSizeNUD.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            _vertexSizeNUD.MaximumSize = new Size(77, 0);
            _vertexSizeNUD.Minimum = new decimal(new int[] { 30, 0, 0, 0 });
            _vertexSizeNUD.MinimumSize = new Size(77, 0);
            _vertexSizeNUD.Name = "_vertexSizeNUD";
            _vertexSizeNUD.Size = new Size(77, 25);
            _vertexSizeNUD.TabIndex = 14;
            _vertexSizeNUD.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // _transitionsVisualParamsGB
            // 
            _transitionsVisualParamsGB.Controls.Add(_inactTransitionsGB);
            _transitionsVisualParamsGB.Controls.Add(_actTransitionsGB);
            _transitionsVisualParamsGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _transitionsVisualParamsGB.ForeColor = Color.LightGray;
            _transitionsVisualParamsGB.Location = new Point(3, 336);
            _transitionsVisualParamsGB.Margin = new Padding(3, 3, 10, 3);
            _transitionsVisualParamsGB.Name = "_transitionsVisualParamsGB";
            _transitionsVisualParamsGB.Size = new Size(346, 210);
            _transitionsVisualParamsGB.TabIndex = 35;
            _transitionsVisualParamsGB.TabStop = false;
            _transitionsVisualParamsGB.Text = "Параметры отрисовки переходов";
            // 
            // _inactTransitionsGB
            // 
            _inactTransitionsGB.Controls.Add(label12);
            _inactTransitionsGB.Controls.Add(label13);
            _inactTransitionsGB.Controls.Add(_transitionDarkPenCP);
            _inactTransitionsGB.Controls.Add(_transitionDarkPenNUD);
            _inactTransitionsGB.Font = new Font("Segoe UI", 9.75F);
            _inactTransitionsGB.ForeColor = Color.LightGray;
            _inactTransitionsGB.Location = new Point(6, 114);
            _inactTransitionsGB.Name = "_inactTransitionsGB";
            _inactTransitionsGB.Size = new Size(334, 82);
            _inactTransitionsGB.TabIndex = 25;
            _inactTransitionsGB.TabStop = false;
            _inactTransitionsGB.Text = "Неактивные переходы";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9.75F);
            label12.ForeColor = Color.Gainsboro;
            label12.Location = new Point(4, 59);
            label12.Margin = new Padding(1, 0, 1, 0);
            label12.Name = "label12";
            label12.Size = new Size(79, 17);
            label12.TabIndex = 34;
            label12.Text = "Цвет линии:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 9.75F);
            label13.ForeColor = Color.Gainsboro;
            label13.Location = new Point(4, 33);
            label13.Margin = new Padding(1, 0, 1, 0);
            label13.Name = "label13";
            label13.Size = new Size(104, 17);
            label13.TabIndex = 33;
            label13.Text = "Толщина линии:";
            // 
            // _transitionDarkPenCP
            // 
            _transitionDarkPenCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _transitionDarkPenCP.BackColor = Color.Black;
            _transitionDarkPenCP.BorderStyle = BorderStyle.Fixed3D;
            _transitionDarkPenCP.Enabled = false;
            _transitionDarkPenCP.Location = new Point(251, 56);
            _transitionDarkPenCP.MaximumSize = new Size(77, 20);
            _transitionDarkPenCP.MinimumSize = new Size(77, 20);
            _transitionDarkPenCP.Name = "_transitionDarkPenCP";
            _transitionDarkPenCP.Size = new Size(77, 20);
            _transitionDarkPenCP.TabIndex = 32;
            _transitionDarkPenCP.TabStop = false;
            // 
            // _transitionDarkPenNUD
            // 
            _transitionDarkPenNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _transitionDarkPenNUD.BackColor = Color.FromArgb(48, 48, 48);
            _transitionDarkPenNUD.DecimalPlaces = 1;
            _transitionDarkPenNUD.Enabled = false;
            _transitionDarkPenNUD.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _transitionDarkPenNUD.ForeColor = Color.Gainsboro;
            _transitionDarkPenNUD.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            _transitionDarkPenNUD.Location = new Point(251, 30);
            _transitionDarkPenNUD.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            _transitionDarkPenNUD.MaximumSize = new Size(77, 0);
            _transitionDarkPenNUD.Minimum = new decimal(new int[] { 10, 0, 0, 65536 });
            _transitionDarkPenNUD.MinimumSize = new Size(77, 0);
            _transitionDarkPenNUD.Name = "_transitionDarkPenNUD";
            _transitionDarkPenNUD.Size = new Size(77, 20);
            _transitionDarkPenNUD.TabIndex = 31;
            _transitionDarkPenNUD.Value = new decimal(new int[] { 30, 0, 0, 65536 });
            // 
            // _actTransitionsGB
            // 
            _actTransitionsGB.Controls.Add(label10);
            _actTransitionsGB.Controls.Add(label11);
            _actTransitionsGB.Controls.Add(_transitionLightPenCP);
            _actTransitionsGB.Controls.Add(_transitionLightPenNUD);
            _actTransitionsGB.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _actTransitionsGB.ForeColor = Color.LightGray;
            _actTransitionsGB.Location = new Point(6, 26);
            _actTransitionsGB.Name = "_actTransitionsGB";
            _actTransitionsGB.Size = new Size(334, 82);
            _actTransitionsGB.TabIndex = 24;
            _actTransitionsGB.TabStop = false;
            _actTransitionsGB.Text = "Активные переходы";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 9.75F);
            label10.ForeColor = Color.Gainsboro;
            label10.Location = new Point(4, 59);
            label10.Margin = new Padding(1, 0, 1, 0);
            label10.Name = "label10";
            label10.Size = new Size(79, 17);
            label10.TabIndex = 30;
            label10.Text = "Цвет линии:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 9.75F);
            label11.ForeColor = Color.Gainsboro;
            label11.Location = new Point(4, 33);
            label11.Margin = new Padding(1, 0, 1, 0);
            label11.Name = "label11";
            label11.Size = new Size(104, 17);
            label11.TabIndex = 29;
            label11.Text = "Толщина линии:";
            // 
            // _transitionLightPenCP
            // 
            _transitionLightPenCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _transitionLightPenCP.BackColor = Color.LimeGreen;
            _transitionLightPenCP.BorderStyle = BorderStyle.Fixed3D;
            _transitionLightPenCP.Enabled = false;
            _transitionLightPenCP.Location = new Point(251, 56);
            _transitionLightPenCP.MaximumSize = new Size(77, 20);
            _transitionLightPenCP.MinimumSize = new Size(77, 20);
            _transitionLightPenCP.Name = "_transitionLightPenCP";
            _transitionLightPenCP.Size = new Size(77, 20);
            _transitionLightPenCP.TabIndex = 27;
            _transitionLightPenCP.TabStop = false;
            // 
            // _transitionLightPenNUD
            // 
            _transitionLightPenNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _transitionLightPenNUD.BackColor = Color.FromArgb(48, 48, 48);
            _transitionLightPenNUD.DecimalPlaces = 1;
            _transitionLightPenNUD.Enabled = false;
            _transitionLightPenNUD.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _transitionLightPenNUD.ForeColor = Color.Gainsboro;
            _transitionLightPenNUD.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            _transitionLightPenNUD.Location = new Point(251, 30);
            _transitionLightPenNUD.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            _transitionLightPenNUD.MaximumSize = new Size(77, 0);
            _transitionLightPenNUD.Minimum = new decimal(new int[] { 10, 0, 0, 65536 });
            _transitionLightPenNUD.MinimumSize = new Size(77, 0);
            _transitionLightPenNUD.Name = "_transitionLightPenNUD";
            _transitionLightPenNUD.Size = new Size(77, 20);
            _transitionLightPenNUD.TabIndex = 26;
            _transitionLightPenNUD.Value = new decimal(new int[] { 30, 0, 0, 65536 });
            // 
            // settingsRightFLP
            // 
            settingsRightFLP.AutoScroll = true;
            settingsRightFLP.Dock = DockStyle.Fill;
            settingsRightFLP.FlowDirection = FlowDirection.TopDown;
            settingsRightFLP.Location = new Point(385, 3);
            settingsRightFLP.Name = "settingsRightFLP";
            settingsRightFLP.Size = new Size(377, 509);
            settingsRightFLP.TabIndex = 1;
            settingsRightFLP.WrapContents = false;
            // 
            // _toolTip
            // 
            _toolTip.AutoPopDelay = 50000;
            _toolTip.InitialDelay = 500;
            _toolTip.ReshowDelay = 100;
            _toolTip.ToolTipTitle = "Информация";
            // 
            // _colorPicker
            // 
            _colorPicker.AnyColor = true;
            _colorPicker.Color = Color.Lime;
            _colorPicker.FullOpen = true;
            _colorPicker.ShowHelp = true;
            // 
            // MainFormV2
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(31, 31, 31);
            ClientSize = new Size(779, 556);
            Controls.Add(_mainTabControl);
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(224, 224, 224);
            MinimumSize = new Size(795, 595);
            Name = "MainFormV2";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AlgoDev 1.1 - программа для имитации работы моделей абстрактных алгоритмов";
            _mainTabControl.ResumeLayout(false);
            _mainTabControl_MainPage.ResumeLayout(false);
            _mainScrollPanel.ResumeLayout(false);
            _mainScrollPanel.PerformLayout();
            _mainTLP.ResumeLayout(false);
            _mainGB.ResumeLayout(false);
            _lasKeyboardGB.ResumeLayout(false);
            _createGAS2_GB.ResumeLayout(false);
            _createGAS1_GB.ResumeLayout(false);
            _combinedAlgoGB.ResumeLayout(false);
            _masGB.ResumeLayout(false);
            _infoGB.ResumeLayout(false);
            _mainTabControl_GAS1.ResumeLayout(false);
            _mainTabControl_GAS2.ResumeLayout(false);
            _mainTabControl_GAS3.ResumeLayout(false);
            _mainTabControl_SettingsPage.ResumeLayout(false);
            settingsTLP.ResumeLayout(false);
            settingsLeftFLP.ResumeLayout(false);
            _workFieldParamsGB.ResumeLayout(false);
            _workFieldParamsGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)drawStepDelayNUD).EndInit();
            ((System.ComponentModel.ISupportInitialize)_viewportCP).EndInit();
            _verticesVisualParamsGB.ResumeLayout(false);
            _verticesVisualParamsGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_vertexActInnerCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)_vertexInactInnerCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)_inactiveBorderCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)_activeBorderCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)_borderNUD).EndInit();
            ((System.ComponentModel.ISupportInitialize)_vertexSizeNUD).EndInit();
            _transitionsVisualParamsGB.ResumeLayout(false);
            _inactTransitionsGB.ResumeLayout(false);
            _inactTransitionsGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_transitionDarkPenCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)_transitionDarkPenNUD).EndInit();
            _actTransitionsGB.ResumeLayout(false);
            _actTransitionsGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_transitionLightPenCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)_transitionLightPenNUD).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Resources.UI.Controls.CustomizableTabControl.CustomTabControl _mainTabControl;
        private TabPage _mainTabControl_SettingsPage;
        private TableLayoutPanel settingsTLP;
        private FlowLayoutPanel settingsLeftFLP;
        private FlowLayoutPanel settingsRightFLP;
        private TabPage _mainTabControl_GAS1;
        private TabPage _mainTabControl_GAS2;
        private TabPage _mainTabControl_GAS3;
        private Panel _mainScrollPanel;
        private ToolTip _toolTip;
        private TabPage _mainTabControl_MainPage;
        private TableLayoutPanel _mainTLP;
        private GroupBox _mainGB;
        private GroupBox _createGAS2_GB;
        private Button _tryParseLAS2_B;
        private Button _createGAS2_B;
        private Resources.UI.Controls.LASInputRichTextBox _lasInputRTB2;
        private GroupBox _createGAS1_GB;
        private Button _tryParseLAS1_B;
        private Resources.UI.Controls.LASInputRichTextBox _lasInputRTB1;
        private Button _createGAS1_B;
        private GroupBox _lasKeyboardGB;
        private GroupBox _combinedAlgoGB;
        private Button _createGAS3_B;
        private Resources.UI.Controls.LASInputRichTextBox _lasInputRTB3;
        private GroupBox _masGB;
        private FlowLayoutPanel _masFLP;
        private GroupBox _infoGB;
        public RichTextBox _mainInfoPort;
        private Resources.UI.Controls.LASInputControls.LASGraphicKeyboard lasGraphicKeyboard1;
        private Resources.UI.Controls.AbstractAlgoModelling.AAController _algoController1;
        private Resources.UI.Controls.AbstractAlgoModelling.AAController _algoController2;
        private Resources.UI.Controls.AbstractAlgoModelling.AAController _algoController3;
        private GroupBox _workFieldParamsGB;
        private Label label15;
        private NumericUpDown drawStepDelayNUD;
        private PictureBox _viewportCP;
        private Label label14;
        private GroupBox _transitionsVisualParamsGB;
        private GroupBox _inactTransitionsGB;
        private Label label12;
        private Label label13;
        private PictureBox _transitionDarkPenCP;
        private NumericUpDown _transitionDarkPenNUD;
        private GroupBox _actTransitionsGB;
        private Label label10;
        private Label label11;
        private PictureBox _transitionLightPenCP;
        private NumericUpDown _transitionLightPenNUD;
        private GroupBox _verticesVisualParamsGB;
        private PictureBox _vertexInactInnerCP;
        private Label label8;
        private Label label6;
        private Label label7;
        private Label label5;
        private PictureBox _inactiveBorderCP;
        private PictureBox _activeBorderCP;
        private Label label4;
        private NumericUpDown _borderNUD;
        private NumericUpDown _vertexSizeNUD;
        private ColorDialog _colorPicker;
        private PictureBox _vertexActInnerCP;
        private Label label1;
    }
}
