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
            groupBox3 = new GroupBox();
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
            groupBox8 = new GroupBox();
            label15 = new Label();
            drawStepDelayNUD = new NumericUpDown();
            containerCP = new PictureBox();
            label14 = new Label();
            groupBox5 = new GroupBox();
            innerStateCP = new PictureBox();
            highlightedBorderCP = new PictureBox();
            label8 = new Label();
            label9 = new Label();
            inactiveBorderCP = new PictureBox();
            activeBorderCP = new PictureBox();
            label6 = new Label();
            label7 = new Label();
            label5 = new Label();
            label4 = new Label();
            borderNUD = new NumericUpDown();
            cirlceDiameterNUD = new NumericUpDown();
            groupBox6 = new GroupBox();
            groupBox7 = new GroupBox();
            label12 = new Label();
            label13 = new Label();
            transitionBlackPenCP = new PictureBox();
            transitionBlackPenNUD = new NumericUpDown();
            groupBox9 = new GroupBox();
            label10 = new Label();
            label11 = new Label();
            transitionLightPenCP = new PictureBox();
            transitionLightPenNUD = new NumericUpDown();
            _toolTip = new ToolTip(components);
            _mainTabControl.SuspendLayout();
            _mainTabControl_MainPage.SuspendLayout();
            _mainScrollPanel.SuspendLayout();
            _mainTLP.SuspendLayout();
            _mainGB.SuspendLayout();
            groupBox3.SuspendLayout();
            _createGAS2_GB.SuspendLayout();
            _createGAS1_GB.SuspendLayout();
            _combinedAlgoGB.SuspendLayout();
            _masGB.SuspendLayout();
            _infoGB.SuspendLayout();
            _mainTabControl_GAS1.SuspendLayout();
            _mainTabControl_GAS2.SuspendLayout();
            _mainTabControl_GAS3.SuspendLayout();
            _mainTabControl_SettingsPage.SuspendLayout();
            groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)drawStepDelayNUD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)containerCP).BeginInit();
            groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)innerStateCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)highlightedBorderCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)inactiveBorderCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)activeBorderCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)borderNUD).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cirlceDiameterNUD).BeginInit();
            groupBox6.SuspendLayout();
            groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)transitionBlackPenCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)transitionBlackPenNUD).BeginInit();
            groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)transitionLightPenCP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)transitionLightPenNUD).BeginInit();
            SuspendLayout();
            // 
            // _mainTabControl
            // 
            _mainTabControl.Controls.Add(_mainTabControl_MainPage);
            _mainTabControl.Controls.Add(_mainTabControl_GAS1);
            _mainTabControl.Controls.Add(_mainTabControl_GAS2);
            _mainTabControl.Controls.Add(_mainTabControl_GAS3);
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
            _mainGB.Controls.Add(groupBox3);
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
            // groupBox3
            // 
            groupBox3.Controls.Add(lasGraphicKeyboard1);
            groupBox3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox3.ForeColor = Color.FromArgb(224, 224, 224);
            groupBox3.Location = new Point(6, 26);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(163, 240);
            groupBox3.TabIndex = 14;
            groupBox3.TabStop = false;
            groupBox3.Text = "Клавиатура ввода ЛСА";
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
            _mainTabControl_GAS3.Location = new Point(4, 31);
            _mainTabControl_GAS3.Name = "_mainTabControl_GAS3";
            _mainTabControl_GAS3.Size = new Size(771, 521);
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
            _algoController3.Size = new Size(771, 521);
            _algoController3.TabIndex = 1;
            // 
            // _mainTabControl_SettingsPage
            // 
            _mainTabControl_SettingsPage.BackColor = Color.FromArgb(31, 31, 31);
            _mainTabControl_SettingsPage.Controls.Add(groupBox8);
            _mainTabControl_SettingsPage.Controls.Add(groupBox5);
            _mainTabControl_SettingsPage.Controls.Add(groupBox6);
            _mainTabControl.SetForbidClose(_mainTabControl_SettingsPage, true);
            _mainTabControl_SettingsPage.ForeColor = Color.FromArgb(214, 214, 214);
            _mainTabControl_SettingsPage.Location = new Point(4, 26);
            _mainTabControl_SettingsPage.Name = "_mainTabControl_SettingsPage";
            _mainTabControl_SettingsPage.Padding = new Padding(3);
            _mainTabControl_SettingsPage.Size = new Size(192, 70);
            _mainTabControl_SettingsPage.TabIndex = 1;
            _mainTabControl_SettingsPage.Text = "Настройки визуализации";
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(label15);
            groupBox8.Controls.Add(drawStepDelayNUD);
            groupBox8.Controls.Add(containerCP);
            groupBox8.Controls.Add(label14);
            groupBox8.FlatStyle = FlatStyle.Popup;
            groupBox8.Font = new Font("Microsoft YaHei UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox8.ForeColor = Color.LightGray;
            groupBox8.Location = new Point(8, 6);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(230, 76);
            groupBox8.TabIndex = 27;
            groupBox8.TabStop = false;
            groupBox8.Text = "Параметры рабочей области";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label15.ForeColor = Color.Gainsboro;
            label15.Location = new Point(3, 48);
            label15.Margin = new Padding(1, 0, 1, 0);
            label15.Name = "label15";
            label15.Size = new Size(141, 13);
            label15.TabIndex = 32;
            label15.Text = "Шаг отрисовки работы (c):";
            // 
            // drawStepDelayNUD
            // 
            drawStepDelayNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            drawStepDelayNUD.BackColor = Color.FromArgb(48, 48, 48);
            drawStepDelayNUD.DecimalPlaces = 2;
            drawStepDelayNUD.Enabled = false;
            drawStepDelayNUD.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            drawStepDelayNUD.ForeColor = Color.Gainsboro;
            drawStepDelayNUD.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            drawStepDelayNUD.Location = new Point(147, 46);
            drawStepDelayNUD.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            drawStepDelayNUD.MaximumSize = new Size(77, 0);
            drawStepDelayNUD.Minimum = new decimal(new int[] { 10, 0, 0, 131072 });
            drawStepDelayNUD.MinimumSize = new Size(77, 0);
            drawStepDelayNUD.Name = "drawStepDelayNUD";
            drawStepDelayNUD.Size = new Size(77, 20);
            drawStepDelayNUD.TabIndex = 31;
            drawStepDelayNUD.Value = new decimal(new int[] { 75, 0, 0, 131072 });
            // 
            // containerCP
            // 
            containerCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            containerCP.BackColor = Color.FromArgb(96, 96, 96);
            containerCP.BorderStyle = BorderStyle.Fixed3D;
            containerCP.Enabled = false;
            containerCP.Location = new Point(147, 20);
            containerCP.MaximumSize = new Size(77, 20);
            containerCP.MinimumSize = new Size(77, 20);
            containerCP.Name = "containerCP";
            containerCP.Size = new Size(77, 20);
            containerCP.TabIndex = 30;
            containerCP.TabStop = false;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label14.ForeColor = Color.Gainsboro;
            label14.Location = new Point(3, 22);
            label14.Margin = new Padding(1, 0, 1, 0);
            label14.Name = "label14";
            label14.Size = new Size(123, 13);
            label14.TabIndex = 28;
            label14.Text = "Цвет рабочей области:";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(innerStateCP);
            groupBox5.Controls.Add(highlightedBorderCP);
            groupBox5.Controls.Add(label8);
            groupBox5.Controls.Add(label9);
            groupBox5.Controls.Add(inactiveBorderCP);
            groupBox5.Controls.Add(activeBorderCP);
            groupBox5.Controls.Add(label6);
            groupBox5.Controls.Add(label7);
            groupBox5.Controls.Add(label5);
            groupBox5.Controls.Add(label4);
            groupBox5.Controls.Add(borderNUD);
            groupBox5.Controls.Add(cirlceDiameterNUD);
            groupBox5.FlatStyle = FlatStyle.Popup;
            groupBox5.Font = new Font("Microsoft YaHei UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox5.ForeColor = Color.LightGray;
            groupBox5.Location = new Point(8, 88);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(230, 173);
            groupBox5.TabIndex = 25;
            groupBox5.TabStop = false;
            groupBox5.Text = "Параметры отрисовки вершин";
            // 
            // innerStateCP
            // 
            innerStateCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            innerStateCP.BackColor = Color.LightGray;
            innerStateCP.BorderStyle = BorderStyle.Fixed3D;
            innerStateCP.Enabled = false;
            innerStateCP.Location = new Point(149, 147);
            innerStateCP.MaximumSize = new Size(77, 20);
            innerStateCP.MinimumSize = new Size(77, 20);
            innerStateCP.Name = "innerStateCP";
            innerStateCP.Size = new Size(77, 20);
            innerStateCP.TabIndex = 30;
            innerStateCP.TabStop = false;
            // 
            // highlightedBorderCP
            // 
            highlightedBorderCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            highlightedBorderCP.BackColor = Color.DarkGray;
            highlightedBorderCP.BorderStyle = BorderStyle.Fixed3D;
            highlightedBorderCP.Enabled = false;
            highlightedBorderCP.Location = new Point(149, 121);
            highlightedBorderCP.MaximumSize = new Size(77, 20);
            highlightedBorderCP.MinimumSize = new Size(77, 20);
            highlightedBorderCP.Name = "highlightedBorderCP";
            highlightedBorderCP.Size = new Size(77, 20);
            highlightedBorderCP.TabIndex = 29;
            highlightedBorderCP.TabStop = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label8.ForeColor = Color.Gainsboro;
            label8.Location = new Point(3, 149);
            label8.Margin = new Padding(1, 0, 1, 0);
            label8.Name = "label8";
            label8.Size = new Size(136, 13);
            label8.TabIndex = 28;
            label8.Text = "Цвет заливки состояния:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label9.ForeColor = Color.Gainsboro;
            label9.Location = new Point(3, 123);
            label9.Margin = new Padding(1, 0, 1, 0);
            label9.Name = "label9";
            label9.Size = new Size(137, 13);
            label9.TabIndex = 27;
            label9.Text = "Цвет подсветки границы:";
            // 
            // inactiveBorderCP
            // 
            inactiveBorderCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inactiveBorderCP.BackColor = Color.Black;
            inactiveBorderCP.BorderStyle = BorderStyle.Fixed3D;
            inactiveBorderCP.Enabled = false;
            inactiveBorderCP.Location = new Point(149, 95);
            inactiveBorderCP.MaximumSize = new Size(77, 20);
            inactiveBorderCP.MinimumSize = new Size(77, 20);
            inactiveBorderCP.Name = "inactiveBorderCP";
            inactiveBorderCP.Size = new Size(77, 20);
            inactiveBorderCP.TabIndex = 26;
            inactiveBorderCP.TabStop = false;
            // 
            // activeBorderCP
            // 
            activeBorderCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            activeBorderCP.BackColor = Color.LimeGreen;
            activeBorderCP.BorderStyle = BorderStyle.Fixed3D;
            activeBorderCP.Enabled = false;
            activeBorderCP.Location = new Point(149, 69);
            activeBorderCP.MaximumSize = new Size(77, 20);
            activeBorderCP.MinimumSize = new Size(77, 20);
            activeBorderCP.Name = "activeBorderCP";
            activeBorderCP.Size = new Size(77, 20);
            activeBorderCP.TabIndex = 25;
            activeBorderCP.TabStop = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label6.ForeColor = Color.Gainsboro;
            label6.Location = new Point(3, 97);
            label6.Margin = new Padding(1, 0, 1, 0);
            label6.Name = "label6";
            label6.Size = new Size(146, 13);
            label6.TabIndex = 24;
            label6.Text = "Цвет неактивной границы: ";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label7.ForeColor = Color.Gainsboro;
            label7.Location = new Point(3, 71);
            label7.Margin = new Padding(1, 0, 1, 0);
            label7.Name = "label7";
            label7.Size = new Size(131, 13);
            label7.TabIndex = 23;
            label7.Text = "Цвет активной границы:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label5.ForeColor = Color.Gainsboro;
            label5.Location = new Point(3, 45);
            label5.Margin = new Padding(1, 0, 1, 0);
            label5.Name = "label5";
            label5.Size = new Size(95, 13);
            label5.TabIndex = 17;
            label5.Text = "Ширина границы:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label4.ForeColor = Color.Gainsboro;
            label4.Location = new Point(3, 19);
            label4.Margin = new Padding(1, 0, 1, 0);
            label4.Name = "label4";
            label4.Size = new Size(98, 13);
            label4.TabIndex = 16;
            label4.Text = "Размер вершины:";
            // 
            // borderNUD
            // 
            borderNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            borderNUD.BackColor = Color.FromArgb(48, 48, 48);
            borderNUD.Enabled = false;
            borderNUD.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            borderNUD.ForeColor = Color.Gainsboro;
            borderNUD.Location = new Point(149, 43);
            borderNUD.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            borderNUD.MaximumSize = new Size(77, 0);
            borderNUD.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            borderNUD.MinimumSize = new Size(77, 0);
            borderNUD.Name = "borderNUD";
            borderNUD.Size = new Size(77, 20);
            borderNUD.TabIndex = 15;
            borderNUD.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // cirlceDiameterNUD
            // 
            cirlceDiameterNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cirlceDiameterNUD.BackColor = Color.FromArgb(48, 48, 48);
            cirlceDiameterNUD.Enabled = false;
            cirlceDiameterNUD.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            cirlceDiameterNUD.ForeColor = Color.Gainsboro;
            cirlceDiameterNUD.Location = new Point(149, 17);
            cirlceDiameterNUD.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            cirlceDiameterNUD.MaximumSize = new Size(77, 0);
            cirlceDiameterNUD.Minimum = new decimal(new int[] { 30, 0, 0, 0 });
            cirlceDiameterNUD.MinimumSize = new Size(77, 0);
            cirlceDiameterNUD.Name = "cirlceDiameterNUD";
            cirlceDiameterNUD.Size = new Size(77, 20);
            cirlceDiameterNUD.TabIndex = 14;
            cirlceDiameterNUD.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(groupBox7);
            groupBox6.Controls.Add(groupBox9);
            groupBox6.Font = new Font("Microsoft YaHei UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox6.ForeColor = Color.LightGray;
            groupBox6.Location = new Point(8, 267);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(230, 167);
            groupBox6.TabIndex = 26;
            groupBox6.TabStop = false;
            groupBox6.Text = "Отрисовка переходов";
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(label12);
            groupBox7.Controls.Add(label13);
            groupBox7.Controls.Add(transitionBlackPenCP);
            groupBox7.Controls.Add(transitionBlackPenNUD);
            groupBox7.Font = new Font("Times New Roman", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox7.ForeColor = Color.LightGray;
            groupBox7.Location = new Point(6, 93);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(218, 70);
            groupBox7.TabIndex = 25;
            groupBox7.TabStop = false;
            groupBox7.Text = "Неактивные переходы";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label12.ForeColor = Color.Gainsboro;
            label12.Location = new Point(3, 45);
            label12.Margin = new Padding(1, 0, 1, 0);
            label12.Name = "label12";
            label12.Size = new Size(68, 13);
            label12.TabIndex = 34;
            label12.Text = "Цвет линии:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label13.ForeColor = Color.Gainsboro;
            label13.Location = new Point(3, 19);
            label13.Margin = new Padding(1, 0, 1, 0);
            label13.Name = "label13";
            label13.Size = new Size(89, 13);
            label13.TabIndex = 33;
            label13.Text = "Толщина линии:";
            // 
            // transitionBlackPenCP
            // 
            transitionBlackPenCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            transitionBlackPenCP.BackColor = Color.Black;
            transitionBlackPenCP.BorderStyle = BorderStyle.Fixed3D;
            transitionBlackPenCP.Enabled = false;
            transitionBlackPenCP.Location = new Point(135, 43);
            transitionBlackPenCP.MaximumSize = new Size(77, 20);
            transitionBlackPenCP.MinimumSize = new Size(77, 20);
            transitionBlackPenCP.Name = "transitionBlackPenCP";
            transitionBlackPenCP.Size = new Size(77, 20);
            transitionBlackPenCP.TabIndex = 32;
            transitionBlackPenCP.TabStop = false;
            // 
            // transitionBlackPenNUD
            // 
            transitionBlackPenNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            transitionBlackPenNUD.BackColor = Color.FromArgb(48, 48, 48);
            transitionBlackPenNUD.DecimalPlaces = 1;
            transitionBlackPenNUD.Enabled = false;
            transitionBlackPenNUD.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            transitionBlackPenNUD.ForeColor = Color.Gainsboro;
            transitionBlackPenNUD.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            transitionBlackPenNUD.Location = new Point(135, 17);
            transitionBlackPenNUD.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            transitionBlackPenNUD.MaximumSize = new Size(77, 0);
            transitionBlackPenNUD.Minimum = new decimal(new int[] { 10, 0, 0, 65536 });
            transitionBlackPenNUD.MinimumSize = new Size(77, 0);
            transitionBlackPenNUD.Name = "transitionBlackPenNUD";
            transitionBlackPenNUD.Size = new Size(77, 20);
            transitionBlackPenNUD.TabIndex = 31;
            transitionBlackPenNUD.Value = new decimal(new int[] { 30, 0, 0, 65536 });
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(label10);
            groupBox9.Controls.Add(label11);
            groupBox9.Controls.Add(transitionLightPenCP);
            groupBox9.Controls.Add(transitionLightPenNUD);
            groupBox9.Font = new Font("Times New Roman", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            groupBox9.ForeColor = Color.LightGray;
            groupBox9.Location = new Point(6, 17);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(218, 70);
            groupBox9.TabIndex = 24;
            groupBox9.TabStop = false;
            groupBox9.Text = "Активные переходы";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label10.ForeColor = Color.Gainsboro;
            label10.Location = new Point(3, 45);
            label10.Margin = new Padding(1, 0, 1, 0);
            label10.Name = "label10";
            label10.Size = new Size(68, 13);
            label10.TabIndex = 30;
            label10.Text = "Цвет линии:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label11.ForeColor = Color.Gainsboro;
            label11.Location = new Point(3, 19);
            label11.Margin = new Padding(1, 0, 1, 0);
            label11.Name = "label11";
            label11.Size = new Size(89, 13);
            label11.TabIndex = 29;
            label11.Text = "Толщина линии:";
            // 
            // transitionLightPenCP
            // 
            transitionLightPenCP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            transitionLightPenCP.BackColor = Color.LimeGreen;
            transitionLightPenCP.BorderStyle = BorderStyle.Fixed3D;
            transitionLightPenCP.Enabled = false;
            transitionLightPenCP.Location = new Point(135, 43);
            transitionLightPenCP.MaximumSize = new Size(77, 20);
            transitionLightPenCP.MinimumSize = new Size(77, 20);
            transitionLightPenCP.Name = "transitionLightPenCP";
            transitionLightPenCP.Size = new Size(77, 20);
            transitionLightPenCP.TabIndex = 27;
            transitionLightPenCP.TabStop = false;
            // 
            // transitionLightPenNUD
            // 
            transitionLightPenNUD.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            transitionLightPenNUD.BackColor = Color.FromArgb(48, 48, 48);
            transitionLightPenNUD.DecimalPlaces = 1;
            transitionLightPenNUD.Enabled = false;
            transitionLightPenNUD.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            transitionLightPenNUD.ForeColor = Color.Gainsboro;
            transitionLightPenNUD.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            transitionLightPenNUD.Location = new Point(135, 17);
            transitionLightPenNUD.Maximum = new decimal(new int[] { 100, 0, 0, 65536 });
            transitionLightPenNUD.MaximumSize = new Size(77, 0);
            transitionLightPenNUD.Minimum = new decimal(new int[] { 10, 0, 0, 65536 });
            transitionLightPenNUD.MinimumSize = new Size(77, 0);
            transitionLightPenNUD.Name = "transitionLightPenNUD";
            transitionLightPenNUD.Size = new Size(77, 20);
            transitionLightPenNUD.TabIndex = 26;
            transitionLightPenNUD.Value = new decimal(new int[] { 30, 0, 0, 65536 });
            // 
            // _toolTip
            // 
            _toolTip.AutoPopDelay = 50000;
            _toolTip.InitialDelay = 500;
            _toolTip.ReshowDelay = 100;
            _toolTip.ToolTipTitle = "Информация";
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
            groupBox3.ResumeLayout(false);
            _createGAS2_GB.ResumeLayout(false);
            _createGAS1_GB.ResumeLayout(false);
            _combinedAlgoGB.ResumeLayout(false);
            _masGB.ResumeLayout(false);
            _infoGB.ResumeLayout(false);
            _mainTabControl_GAS1.ResumeLayout(false);
            _mainTabControl_GAS2.ResumeLayout(false);
            _mainTabControl_GAS3.ResumeLayout(false);
            _mainTabControl_SettingsPage.ResumeLayout(false);
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)drawStepDelayNUD).EndInit();
            ((System.ComponentModel.ISupportInitialize)containerCP).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)innerStateCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)highlightedBorderCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)inactiveBorderCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)activeBorderCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)borderNUD).EndInit();
            ((System.ComponentModel.ISupportInitialize)cirlceDiameterNUD).EndInit();
            groupBox6.ResumeLayout(false);
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)transitionBlackPenCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)transitionBlackPenNUD).EndInit();
            groupBox9.ResumeLayout(false);
            groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)transitionLightPenCP).EndInit();
            ((System.ComponentModel.ISupportInitialize)transitionLightPenNUD).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Resources.UI.Controls.CustomizableTabControl.CustomTabControl _mainTabControl;
        private TabPage _mainTabControl_SettingsPage;
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
        private GroupBox groupBox3;
        private GroupBox _combinedAlgoGB;
        private Button _createGAS3_B;
        private Resources.UI.Controls.LASInputRichTextBox _lasInputRTB3;
        private GroupBox _masGB;
        private FlowLayoutPanel _masFLP;
        private GroupBox _infoGB;
        public RichTextBox _mainInfoPort;
        private Resources.UI.Controls.LASInputControls.LASGraphicKeyboard lasGraphicKeyboard1;
        private GroupBox groupBox8;
        private Label label15;
        private NumericUpDown drawStepDelayNUD;
        private PictureBox containerCP;
        private Label label14;
        private GroupBox groupBox5;
        private PictureBox innerStateCP;
        private PictureBox highlightedBorderCP;
        private Label label8;
        private Label label9;
        private PictureBox inactiveBorderCP;
        private PictureBox activeBorderCP;
        private Label label6;
        private Label label7;
        private Label label5;
        private Label label4;
        private NumericUpDown borderNUD;
        private NumericUpDown cirlceDiameterNUD;
        private GroupBox groupBox6;
        private GroupBox groupBox7;
        private Label label12;
        private Label label13;
        private PictureBox transitionBlackPenCP;
        private NumericUpDown transitionBlackPenNUD;
        private GroupBox groupBox9;
        private Label label10;
        private Label label11;
        private PictureBox transitionLightPenCP;
        private NumericUpDown transitionLightPenNUD;
        private Resources.UI.Controls.AbstractAlgoModelling.AAController _algoController1;
        private Resources.UI.Controls.AbstractAlgoModelling.AAController _algoController2;
        private Resources.UI.Controls.AbstractAlgoModelling.AAController _algoController3;
    }
}
