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
            _mainFLP = new FlowLayoutPanel();
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
            groupBox1 = new GroupBox();
            _createGAS3_B = new Button();
            _lasInputRTB3 = new AlgorithmDeveloper.Resources.UI.Controls.LASInputRichTextBox();
            _mainPage_masGB = new GroupBox();
            _masFLP = new FlowLayoutPanel();
            groupBox2 = new GroupBox();
            _mainTerminal = new RichTextBox();
            _mainTabControl_GAS1 = new TabPage();
            _algoController1 = new AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling.AbstractAlgoController();
            _mainTabControl_GAS2 = new TabPage();
            _algoController2 = new AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling.AbstractAlgoController();
            _mainTabControl_GAS3 = new TabPage();
            _algoController3 = new AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling.AbstractAlgoController();
            _mainTabControl_SettingsPage = new TabPage();
            _toolTip = new ToolTip(components);
            _mainTabControl.SuspendLayout();
            _mainTabControl_MainPage.SuspendLayout();
            _mainFLP.SuspendLayout();
            _mainGB.SuspendLayout();
            groupBox3.SuspendLayout();
            _createGAS2_GB.SuspendLayout();
            _createGAS1_GB.SuspendLayout();
            groupBox1.SuspendLayout();
            _mainPage_masGB.SuspendLayout();
            groupBox2.SuspendLayout();
            _mainTabControl_GAS1.SuspendLayout();
            _mainTabControl_GAS2.SuspendLayout();
            _mainTabControl_GAS3.SuspendLayout();
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
            _mainTabControl_MainPage.Controls.Add(_mainFLP);
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
            // _mainFLP
            // 
            _mainFLP.AutoScroll = true;
            _mainFLP.Controls.Add(_mainGB);
            _mainFLP.Controls.Add(groupBox1);
            _mainFLP.Controls.Add(_mainPage_masGB);
            _mainFLP.Controls.Add(groupBox2);
            _mainFLP.Dock = DockStyle.Fill;
            _mainFLP.FlowDirection = FlowDirection.TopDown;
            _mainFLP.Location = new Point(3, 3);
            _mainFLP.Name = "_mainFLP";
            _mainFLP.Size = new Size(765, 515);
            _mainFLP.TabIndex = 0;
            _mainFLP.WrapContents = false;
            // 
            // _mainGB
            // 
            _mainGB.Controls.Add(groupBox3);
            _mainGB.Controls.Add(_createGAS2_GB);
            _mainGB.Controls.Add(_createGAS1_GB);
            _mainGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _mainGB.ForeColor = Color.FromArgb(224, 224, 224);
            _mainGB.Location = new Point(3, 3);
            _mainGB.Name = "_mainGB";
            _mainGB.Size = new Size(740, 276);
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
            _createGAS2_GB.Controls.Add(_tryParseLAS2_B);
            _createGAS2_GB.Controls.Add(_createGAS2_B);
            _createGAS2_GB.Controls.Add(_lasInputRTB2);
            _createGAS2_GB.Font = new Font("Segoe UI", 9.75F);
            _createGAS2_GB.ForeColor = Color.FromArgb(224, 224, 224);
            _createGAS2_GB.Location = new Point(175, 149);
            _createGAS2_GB.Name = "_createGAS2_GB";
            _createGAS2_GB.Size = new Size(559, 117);
            _createGAS2_GB.TabIndex = 11;
            _createGAS2_GB.TabStop = false;
            _createGAS2_GB.Text = "Ввод и обработка ЛСА второго алгоритма";
            // 
            // _tryParseLAS2_B
            // 
            _tryParseLAS2_B.BackColor = Color.FromArgb(64, 64, 64);
            _tryParseLAS2_B.BackgroundImageLayout = ImageLayout.None;
            _tryParseLAS2_B.Cursor = Cursors.Hand;
            _tryParseLAS2_B.Enabled = false;
            _tryParseLAS2_B.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _tryParseLAS2_B.FlatStyle = FlatStyle.Flat;
            _tryParseLAS2_B.Font = new Font("Segoe UI", 9.75F);
            _tryParseLAS2_B.Location = new Point(348, 17);
            _tryParseLAS2_B.Name = "_tryParseLAS2_B";
            _tryParseLAS2_B.Size = new Size(205, 40);
            _tryParseLAS2_B.TabIndex = 9;
            _tryParseLAS2_B.Text = "Проверить введённую ЛСА";
            _tryParseLAS2_B.UseVisualStyleBackColor = false;
            _tryParseLAS2_B.Click += CheckLAS;
            // 
            // _createGAS2_B
            // 
            _createGAS2_B.BackColor = Color.FromArgb(64, 64, 64);
            _createGAS2_B.BackgroundImageLayout = ImageLayout.None;
            _createGAS2_B.Cursor = Cursors.Hand;
            _createGAS2_B.Enabled = false;
            _createGAS2_B.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _createGAS2_B.FlatStyle = FlatStyle.Flat;
            _createGAS2_B.Font = new Font("Segoe UI", 9.75F);
            _createGAS2_B.Location = new Point(348, 65);
            _createGAS2_B.Margin = new Padding(3, 1, 3, 3);
            _createGAS2_B.Name = "_createGAS2_B";
            _createGAS2_B.Size = new Size(205, 40);
            _createGAS2_B.TabIndex = 7;
            _createGAS2_B.Text = "Построить модель алгоритма";
            _createGAS2_B.UseVisualStyleBackColor = false;
            _createGAS2_B.Click += CreageGAS;
            // 
            // _lasInputRTB2
            // 
            _lasInputRTB2.AllowDrop = true;
            _lasInputRTB2.BackColor = Color.FromArgb(48, 48, 48);
            _lasInputRTB2.BorderStyle = BorderStyle.None;
            _lasInputRTB2.ForeColor = Color.FromArgb(224, 224, 224);
            _lasInputRTB2.Location = new Point(6, 17);
            _lasInputRTB2.Name = "_lasInputRTB2";
            _lasInputRTB2.PlaceholderColor = Color.FromArgb(96, 96, 96);
            _lasInputRTB2.PlaceholderText = "Yн X1 ↑1 w↑2 ↓1 Y3 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк";
            _lasInputRTB2.ShortcutsEnabled = false;
            _lasInputRTB2.Size = new Size(336, 88);
            _lasInputRTB2.TabIndex = 8;
            _lasInputRTB2.Text = "";
            _lasInputRTB2.TextChanged += EnableCheckLASButton;
            // 
            // _createGAS1_GB
            // 
            _createGAS1_GB.Controls.Add(_tryParseLAS1_B);
            _createGAS1_GB.Controls.Add(_createGAS1_B);
            _createGAS1_GB.Controls.Add(_lasInputRTB1);
            _createGAS1_GB.Font = new Font("Segoe UI", 9.75F);
            _createGAS1_GB.ForeColor = Color.FromArgb(224, 224, 224);
            _createGAS1_GB.Location = new Point(175, 26);
            _createGAS1_GB.Margin = new Padding(3, 14, 3, 3);
            _createGAS1_GB.Name = "_createGAS1_GB";
            _createGAS1_GB.Size = new Size(559, 117);
            _createGAS1_GB.TabIndex = 10;
            _createGAS1_GB.TabStop = false;
            _createGAS1_GB.Text = "Ввод и обработка ЛСА первого алгоритма";
            // 
            // _tryParseLAS1_B
            // 
            _tryParseLAS1_B.BackColor = Color.FromArgb(64, 64, 64);
            _tryParseLAS1_B.BackgroundImageLayout = ImageLayout.None;
            _tryParseLAS1_B.Cursor = Cursors.Hand;
            _tryParseLAS1_B.Enabled = false;
            _tryParseLAS1_B.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _tryParseLAS1_B.FlatStyle = FlatStyle.Flat;
            _tryParseLAS1_B.Font = new Font("Segoe UI", 9.75F);
            _tryParseLAS1_B.Location = new Point(348, 17);
            _tryParseLAS1_B.Name = "_tryParseLAS1_B";
            _tryParseLAS1_B.Size = new Size(205, 40);
            _tryParseLAS1_B.TabIndex = 9;
            _tryParseLAS1_B.Text = "Проверить введённую ЛСА";
            _tryParseLAS1_B.UseVisualStyleBackColor = false;
            _tryParseLAS1_B.Click += CheckLAS;
            // 
            // _createGAS1_B
            // 
            _createGAS1_B.BackColor = Color.FromArgb(64, 64, 64);
            _createGAS1_B.BackgroundImageLayout = ImageLayout.None;
            _createGAS1_B.Cursor = Cursors.Hand;
            _createGAS1_B.Enabled = false;
            _createGAS1_B.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _createGAS1_B.FlatStyle = FlatStyle.Flat;
            _createGAS1_B.Font = new Font("Segoe UI", 9.75F);
            _createGAS1_B.Location = new Point(348, 65);
            _createGAS1_B.Margin = new Padding(3, 1, 3, 3);
            _createGAS1_B.Name = "_createGAS1_B";
            _createGAS1_B.Size = new Size(205, 40);
            _createGAS1_B.TabIndex = 7;
            _createGAS1_B.Text = "Построить модель алгоритма";
            _createGAS1_B.UseVisualStyleBackColor = false;
            _createGAS1_B.Click += CreageGAS;
            // 
            // _lasInputRTB1
            // 
            _lasInputRTB1.AllowDrop = true;
            _lasInputRTB1.BackColor = Color.FromArgb(48, 48, 48);
            _lasInputRTB1.BorderStyle = BorderStyle.None;
            _lasInputRTB1.ForeColor = Color.FromArgb(224, 224, 224);
            _lasInputRTB1.Location = new Point(6, 17);
            _lasInputRTB1.Name = "_lasInputRTB1";
            _lasInputRTB1.PlaceholderColor = Color.FromArgb(96, 96, 96);
            _lasInputRTB1.PlaceholderText = "Yн X0 ↑1 Y0 w↑2 ↓1 Y1 w↑2 ↓2 X2 ↑3 w↑4 ↓3 Y2 w↑2 ↓4 Yк";
            _lasInputRTB1.ShortcutsEnabled = false;
            _lasInputRTB1.Size = new Size(336, 88);
            _lasInputRTB1.TabIndex = 8;
            _lasInputRTB1.Text = "";
            _lasInputRTB1.TextChanged += EnableCheckLASButton;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(_createGAS3_B);
            groupBox1.Controls.Add(_lasInputRTB3);
            groupBox1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBox1.ForeColor = Color.FromArgb(224, 224, 224);
            groupBox1.Location = new Point(3, 296);
            groupBox1.Margin = new Padding(3, 14, 3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(740, 93);
            groupBox1.TabIndex = 13;
            groupBox1.TabStop = false;
            groupBox1.Text = "Объединённый алгоритм";
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
            _createGAS3_B.Location = new Point(521, 26);
            _createGAS3_B.Margin = new Padding(3, 1, 3, 3);
            _createGAS3_B.Name = "_createGAS3_B";
            _createGAS3_B.Size = new Size(213, 56);
            _createGAS3_B.TabIndex = 7;
            _createGAS3_B.Text = "Провести объединение и построить модель алгоритма";
            _createGAS3_B.UseVisualStyleBackColor = false;
            // 
            // _lasInputRTB3
            // 
            _lasInputRTB3.AllowDrop = true;
            _lasInputRTB3.BackColor = Color.FromArgb(48, 48, 48);
            _lasInputRTB3.BorderStyle = BorderStyle.None;
            _lasInputRTB3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _lasInputRTB3.ForeColor = Color.FromArgb(224, 224, 224);
            _lasInputRTB3.Location = new Point(6, 26);
            _lasInputRTB3.Name = "_lasInputRTB3";
            _lasInputRTB3.PlaceholderColor = Color.FromArgb(96, 96, 96);
            _lasInputRTB3.PlaceholderText = "Здесь появится ЛСА объединённого алгоритма";
            _lasInputRTB3.ShortcutsEnabled = false;
            _lasInputRTB3.Size = new Size(509, 56);
            _lasInputRTB3.TabIndex = 8;
            _lasInputRTB3.Text = "";
            // 
            // _mainPage_masGB
            // 
            _mainPage_masGB.Controls.Add(_masFLP);
            _mainPage_masGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _mainPage_masGB.ForeColor = Color.FromArgb(224, 224, 224);
            _mainPage_masGB.Location = new Point(3, 406);
            _mainPage_masGB.Margin = new Padding(3, 14, 3, 3);
            _mainPage_masGB.Name = "_mainPage_masGB";
            _mainPage_masGB.Size = new Size(740, 350);
            _mainPage_masGB.TabIndex = 14;
            _mainPage_masGB.TabStop = false;
            _mainPage_masGB.Text = "Матричные схемы алгоритмов";
            // 
            // _masFLP
            // 
            _masFLP.AutoScroll = true;
            _masFLP.Dock = DockStyle.Fill;
            _masFLP.FlowDirection = FlowDirection.TopDown;
            _masFLP.Location = new Point(3, 23);
            _masFLP.Name = "_masFLP";
            _masFLP.Size = new Size(734, 324);
            _masFLP.TabIndex = 0;
            _masFLP.WrapContents = false;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(_mainTerminal);
            groupBox2.ForeColor = Color.FromArgb(224, 224, 224);
            groupBox2.Location = new Point(3, 760);
            groupBox2.Margin = new Padding(3, 1, 3, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(740, 200);
            groupBox2.TabIndex = 15;
            groupBox2.TabStop = false;
            groupBox2.Text = "Журнал";
            // 
            // _mainTerminal
            // 
            _mainTerminal.BackColor = Color.FromArgb(16, 16, 16);
            _mainTerminal.BorderStyle = BorderStyle.FixedSingle;
            _mainTerminal.DetectUrls = false;
            _mainTerminal.Dock = DockStyle.Fill;
            _mainTerminal.Font = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _mainTerminal.ForeColor = Color.FromArgb(224, 224, 224);
            _mainTerminal.Location = new Point(3, 19);
            _mainTerminal.Margin = new Padding(4, 3, 4, 3);
            _mainTerminal.Name = "_mainTerminal";
            _mainTerminal.ReadOnly = true;
            _mainTerminal.Size = new Size(734, 178);
            _mainTerminal.TabIndex = 3;
            _mainTerminal.Text = "";
            _mainTerminal.WordWrap = false;
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
            _algoController3.ForeColor = Color.FromArgb(224, 224, 224);
            _algoController3.Location = new Point(0, 0);
            _algoController3.Name = "_algoController3";
            _algoController3.Size = new Size(192, 70);
            _algoController3.TabIndex = 1;
            // 
            // _mainTabControl_SettingsPage
            // 
            _mainTabControl_SettingsPage.BackColor = Color.FromArgb(31, 31, 31);
            _mainTabControl.SetForbidClose(_mainTabControl_SettingsPage, true);
            _mainTabControl_SettingsPage.ForeColor = Color.FromArgb(214, 214, 214);
            _mainTabControl_SettingsPage.Location = new Point(4, 26);
            _mainTabControl_SettingsPage.Name = "_mainTabControl_SettingsPage";
            _mainTabControl_SettingsPage.Padding = new Padding(3);
            _mainTabControl_SettingsPage.Size = new Size(192, 70);
            _mainTabControl_SettingsPage.TabIndex = 1;
            _mainTabControl_SettingsPage.Text = "Настройки визуализации";
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
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
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
            _mainFLP.ResumeLayout(false);
            _mainGB.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            _createGAS2_GB.ResumeLayout(false);
            _createGAS1_GB.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            _mainPage_masGB.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            _mainTabControl_GAS1.ResumeLayout(false);
            _mainTabControl_GAS2.ResumeLayout(false);
            _mainTabControl_GAS3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Resources.UI.Controls.CustomizableTabControl.CustomTabControl _mainTabControl;
        private TabPage _mainTabControl_SettingsPage;
        private TabPage _mainTabControl_GAS1;
        private TabPage _mainTabControl_GAS2;
        private TabPage _mainTabControl_GAS3;
        private ToolTip _toolTip;
        public Resources.UI.Controls.AbstractAlgoModelling.AbstractAlgoController _algoController1;
        public Resources.UI.Controls.AbstractAlgoModelling.AbstractAlgoController _algoController2;
        public Resources.UI.Controls.AbstractAlgoModelling.AbstractAlgoController _algoController3;
        private TabPage _mainTabControl_MainPage;
        private FlowLayoutPanel _mainFLP;
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
        private GroupBox groupBox1;
        private Button _createGAS3_B;
        private Resources.UI.Controls.LASInputRichTextBox _lasInputRTB3;
        private GroupBox _mainPage_masGB;
        private FlowLayoutPanel _masFLP;
        private GroupBox groupBox2;
        public RichTextBox _mainTerminal;
        private Resources.UI.Controls.LASInputControls.LASGraphicKeyboard lasGraphicKeyboard1;
    }
}
