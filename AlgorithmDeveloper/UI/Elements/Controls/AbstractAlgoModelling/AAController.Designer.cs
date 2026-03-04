namespace AlgorithmDeveloper.UI.Elements.Controls.AbstractAlgoModelling
{
    partial class AAController
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AAController));
            _aapMainTLP = new TableLayoutPanel();
            _viewportGB = new GroupBox();
            _viewport = new AlgorithmDeveloper.UI.Elements.Controls.Viewports.ImageViewport();
            _aapInteractionGB = new GroupBox();
            _interactionTabControl = new AlgorithmDeveloper.UI.Elements.Controls.CustomizableTabControl.CustomizableTabControl();
            _interactionTabControl_InteractiveModeTab = new TabPage();
            _interactiveModeTLP = new TableLayoutPanel();
            _buttonsTLP = new TableLayoutPanel();
            _stopWorkImitation = new Button();
            _startWorkImitation = new Button();
            _terminal = new AlgorithmDeveloper.UI.Elements.Controls.Terminal.TerminalControl();
            _interactionTabControl_FastModeTab = new TabPage();
            _fastModeTLP = new TableLayoutPanel();
            _fastModeButtonsTLP = new TableLayoutPanel();
            _btnFastStop = new Button();
            _btnFastStart = new Button();
            _fastTerminal = new AlgorithmDeveloper.UI.Elements.Controls.Terminal.TerminalControl();
            _interactionTabControl_AAInfoTab = new TabPage();
            _createReportButton = new Button();
            _infoLabel = new Label();
            _saveFileDialog = new SaveFileDialog();
            _aapMainTLP.SuspendLayout();
            _viewportGB.SuspendLayout();
            _aapInteractionGB.SuspendLayout();
            _interactionTabControl.SuspendLayout();
            _interactionTabControl_InteractiveModeTab.SuspendLayout();
            _interactiveModeTLP.SuspendLayout();
            _buttonsTLP.SuspendLayout();
            _interactionTabControl_FastModeTab.SuspendLayout();
            _fastModeTLP.SuspendLayout();
            _fastModeButtonsTLP.SuspendLayout();
            _interactionTabControl_AAInfoTab.SuspendLayout();
            SuspendLayout();
            // 
            // _aapMainTLP
            // 
            _aapMainTLP.ColumnCount = 2;
            _aapMainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 449F));
            _aapMainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _aapMainTLP.Controls.Add(_viewportGB, 1, 0);
            _aapMainTLP.Controls.Add(_aapInteractionGB, 0, 0);
            _aapMainTLP.Dock = DockStyle.Fill;
            _aapMainTLP.Location = new Point(0, 0);
            _aapMainTLP.Name = "_aapMainTLP";
            _aapMainTLP.RowCount = 1;
            _aapMainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _aapMainTLP.Size = new Size(742, 437);
            _aapMainTLP.TabIndex = 0;
            // 
            // _viewportGB
            // 
            _viewportGB.Controls.Add(_viewport);
            _viewportGB.Dock = DockStyle.Fill;
            _viewportGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _viewportGB.ForeColor = Color.Silver;
            _viewportGB.Location = new Point(452, 3);
            _viewportGB.Name = "_viewportGB";
            _viewportGB.Size = new Size(287, 431);
            _viewportGB.TabIndex = 1;
            _viewportGB.TabStop = false;
            _viewportGB.Text = "Визуализация";
            // 
            // _viewport
            // 
            _viewport.BackColor = Color.FromArgb(48, 48, 48);
            _viewport.Dock = DockStyle.Fill;
            _viewport.FigureFont = new Font("Arial", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            _viewport.ForeColor = Color.Silver;
            _viewport.Location = new Point(3, 23);
            _viewport.Name = "_viewport";
            _viewport.Size = new Size(281, 405);
            _viewport.TabIndex = 0;
            // 
            // _aapInteractionGB
            // 
            _aapInteractionGB.Controls.Add(_interactionTabControl);
            _aapInteractionGB.Dock = DockStyle.Fill;
            _aapInteractionGB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _aapInteractionGB.ForeColor = Color.Silver;
            _aapInteractionGB.Location = new Point(3, 3);
            _aapInteractionGB.Name = "_aapInteractionGB";
            _aapInteractionGB.Size = new Size(443, 431);
            _aapInteractionGB.TabIndex = 0;
            _aapInteractionGB.TabStop = false;
            _aapInteractionGB.Text = "Взаимодействие";
            // 
            // _interactionTabControl
            // 
            _interactionTabControl.Controls.Add(_interactionTabControl_InteractiveModeTab);
            _interactionTabControl.Controls.Add(_interactionTabControl_FastModeTab);
            _interactionTabControl.Controls.Add(_interactionTabControl_AAInfoTab);
            _interactionTabControl.Cursor = Cursors.Hand;
            _interactionTabControl.DisplayStyle = CustomizableTabControl.Styles.TabStyle.Dark;
            _interactionTabControl.DisplayStyleProvider.BackgroundColor = Color.FromArgb(48, 48, 48);
            _interactionTabControl.DisplayStyleProvider.BackgroundColorDisabled = Color.DimGray;
            _interactionTabControl.DisplayStyleProvider.BackgroundColorHot = Color.FromArgb(61, 61, 61);
            _interactionTabControl.DisplayStyleProvider.BackgroundColorSelected = Color.FromArgb(31, 31, 31);
            _interactionTabControl.DisplayStyleProvider.BorderColor = Color.FromArgb(66, 66, 66);
            _interactionTabControl.DisplayStyleProvider.BorderColorHot = Color.FromArgb(172, 172, 172);
            _interactionTabControl.DisplayStyleProvider.BorderColorSelected = Color.FromArgb(250, 250, 250);
            _interactionTabControl.DisplayStyleProvider.CloserColor = Color.FromArgb(255, 58, 58);
            _interactionTabControl.DisplayStyleProvider.CloserColorActive = Color.FromArgb(255, 0, 0);
            _interactionTabControl.DisplayStyleProvider.FocusColor = Color.FromArgb(250, 250, 250);
            _interactionTabControl.DisplayStyleProvider.FocusColorSecondary = Color.FromArgb(250, 250, 250);
            _interactionTabControl.DisplayStyleProvider.FocusTrack = true;
            _interactionTabControl.DisplayStyleProvider.HotTrack = true;
            _interactionTabControl.DisplayStyleProvider.ImageAlign = ContentAlignment.MiddleLeft;
            _interactionTabControl.DisplayStyleProvider.Opacity = 1F;
            _interactionTabControl.DisplayStyleProvider.Overlap = 0;
            _interactionTabControl.DisplayStyleProvider.Padding = new Point(6, 3);
            _interactionTabControl.DisplayStyleProvider.Radius = 10;
            _interactionTabControl.DisplayStyleProvider.ShowTabCloser = false;
            _interactionTabControl.DisplayStyleProvider.TextColor = Color.FromArgb(214, 214, 214);
            _interactionTabControl.DisplayStyleProvider.TextColorDisabled = Color.FromArgb(64, 64, 64);
            _interactionTabControl.DisplayStyleProvider.TextColorHot = Color.FromArgb(250, 250, 250);
            _interactionTabControl.DisplayStyleProvider.TextColorSelected = Color.FromArgb(250, 250, 250);
            _interactionTabControl.Dock = DockStyle.Fill;
            _interactionTabControl.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _interactionTabControl.Location = new Point(3, 23);
            _interactionTabControl.Margin = new Padding(1);
            _interactionTabControl.Name = "_interactionTabControl";
            _interactionTabControl.SelectedIndex = 0;
            _interactionTabControl.Size = new Size(437, 405);
            _interactionTabControl.TabBackgroundColor = Color.FromArgb(48, 48, 48);
            _interactionTabControl.TabBackgroundColorDisabled = Color.DimGray;
            _interactionTabControl.TabBackgroundColorHot = Color.FromArgb(61, 61, 61);
            _interactionTabControl.TabBackgroundColorSelected = Color.FromArgb(31, 31, 31);
            _interactionTabControl.TabBorderColor = Color.FromArgb(66, 66, 66);
            _interactionTabControl.TabBorderColorHot = Color.FromArgb(172, 172, 172);
            _interactionTabControl.TabBorderColorSelected = Color.FromArgb(250, 250, 250);
            _interactionTabControl.TabCloserColor = Color.FromArgb(255, 58, 58);
            _interactionTabControl.TabCloserColorActive = Color.FromArgb(255, 0, 0);
            _interactionTabControl.TabFocusColor = Color.FromArgb(250, 250, 250);
            _interactionTabControl.TabFocusColorSecondary = Color.FromArgb(250, 250, 250);
            _interactionTabControl.TabFocusTrack = true;
            _interactionTabControl.TabIndex = 7;
            _interactionTabControl.TabRadius = 10;
            _interactionTabControl.TabTextColor = Color.FromArgb(214, 214, 214);
            _interactionTabControl.TabTextColorDisabled = Color.FromArgb(64, 64, 64);
            _interactionTabControl.TabTextColorHot = Color.FromArgb(250, 250, 250);
            _interactionTabControl.TabTextColorSelected = Color.FromArgb(250, 250, 250);
            // 
            // _interactionTabControl_InteractiveModeTab
            // 
            _interactionTabControl_InteractiveModeTab.BackColor = Color.FromArgb(31, 31, 31);
            _interactionTabControl_InteractiveModeTab.Controls.Add(_interactiveModeTLP);
            _interactionTabControl.SetForbidClose(_interactionTabControl_InteractiveModeTab, true);
            _interactionTabControl.SetForbidEntry(_interactionTabControl_InteractiveModeTab, true);
            _interactionTabControl.SetForbidHide(_interactionTabControl_InteractiveModeTab, true);
            _interactionTabControl_InteractiveModeTab.ForeColor = Color.FromArgb(214, 214, 214);
            _interactionTabControl_InteractiveModeTab.Location = new Point(4, 27);
            _interactionTabControl_InteractiveModeTab.Name = "_interactionTabControl_InteractiveModeTab";
            _interactionTabControl_InteractiveModeTab.Padding = new Padding(3);
            _interactionTabControl_InteractiveModeTab.Size = new Size(405, 374);
            _interactionTabControl_InteractiveModeTab.TabIndex = 0;
            _interactionTabControl_InteractiveModeTab.Text = "Интерактивный режим";
            // 
            // _interactiveModeTLP
            // 
            _interactiveModeTLP.ColumnCount = 1;
            _interactiveModeTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _interactiveModeTLP.Controls.Add(_buttonsTLP, 0, 0);
            _interactiveModeTLP.Controls.Add(_terminal, 0, 1);
            _interactiveModeTLP.Dock = DockStyle.Fill;
            _interactiveModeTLP.Location = new Point(3, 3);
            _interactiveModeTLP.Name = "_interactiveModeTLP";
            _interactiveModeTLP.RowCount = 2;
            _interactiveModeTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            _interactiveModeTLP.RowStyles.Add(new RowStyle());
            _interactiveModeTLP.Size = new Size(399, 368);
            _interactiveModeTLP.TabIndex = 0;
            // 
            // _buttonsTLP
            // 
            _buttonsTLP.ColumnCount = 2;
            _buttonsTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _buttonsTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _buttonsTLP.Controls.Add(_stopWorkImitation, 1, 0);
            _buttonsTLP.Controls.Add(_startWorkImitation, 0, 0);
            _buttonsTLP.Dock = DockStyle.Fill;
            _buttonsTLP.Location = new Point(3, 3);
            _buttonsTLP.Name = "_buttonsTLP";
            _buttonsTLP.RowCount = 1;
            _buttonsTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _buttonsTLP.Size = new Size(393, 44);
            _buttonsTLP.TabIndex = 0;
            // 
            // _stopWorkImitation
            // 
            _stopWorkImitation.BackColor = Color.FromArgb(64, 64, 64);
            _stopWorkImitation.BackgroundImageLayout = ImageLayout.None;
            _stopWorkImitation.Cursor = Cursors.Hand;
            _stopWorkImitation.Dock = DockStyle.Fill;
            _stopWorkImitation.Enabled = false;
            _stopWorkImitation.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _stopWorkImitation.FlatStyle = FlatStyle.Flat;
            _stopWorkImitation.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _stopWorkImitation.Location = new Point(199, 3);
            _stopWorkImitation.Name = "_stopWorkImitation";
            _stopWorkImitation.Size = new Size(191, 38);
            _stopWorkImitation.TabIndex = 11;
            _stopWorkImitation.Text = "Остановить";
            _stopWorkImitation.UseVisualStyleBackColor = false;
            // 
            // _startWorkImitation
            // 
            _startWorkImitation.BackColor = Color.FromArgb(64, 64, 64);
            _startWorkImitation.BackgroundImageLayout = ImageLayout.None;
            _startWorkImitation.Cursor = Cursors.Hand;
            _startWorkImitation.Dock = DockStyle.Fill;
            _startWorkImitation.Enabled = false;
            _startWorkImitation.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _startWorkImitation.FlatStyle = FlatStyle.Flat;
            _startWorkImitation.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _startWorkImitation.Location = new Point(3, 3);
            _startWorkImitation.Name = "_startWorkImitation";
            _startWorkImitation.Size = new Size(190, 38);
            _startWorkImitation.TabIndex = 10;
            _startWorkImitation.Text = "Запустить";
            _startWorkImitation.UseVisualStyleBackColor = false;
            // 
            // _terminal
            // 
            _terminal.BackColor = Color.FromArgb(31, 31, 31);
            _terminal.BinaryInputMode = true;
            _terminal.Dock = DockStyle.Fill;
            _terminal.Location = new Point(3, 53);
            _terminal.Name = "_terminal";
            _terminal.Size = new Size(393, 354);
            _terminal.TabIndex = 1;
            // 
            // _interactionTabControl_FastModeTab
            // 
            _interactionTabControl_FastModeTab.BackColor = Color.FromArgb(31, 31, 31);
            _interactionTabControl_FastModeTab.Controls.Add(_fastModeTLP);
            _interactionTabControl.SetForbidClose(_interactionTabControl_FastModeTab, true);
            _interactionTabControl_FastModeTab.ForeColor = Color.FromArgb(214, 214, 214);
            _interactionTabControl_FastModeTab.Location = new Point(4, 27);
            _interactionTabControl_FastModeTab.Name = "_interactionTabControl_FastModeTab";
            _interactionTabControl_FastModeTab.Padding = new Padding(3);
            _interactionTabControl_FastModeTab.Size = new Size(429, 374);
            _interactionTabControl_FastModeTab.TabIndex = 1;
            _interactionTabControl_FastModeTab.Text = "Быстрый режим";
            // 
            // _fastModeTLP
            // 
            _fastModeTLP.ColumnCount = 1;
            _fastModeTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _fastModeTLP.Controls.Add(_fastModeButtonsTLP, 0, 0);
            _fastModeTLP.Controls.Add(_fastTerminal, 0, 1);
            _fastModeTLP.Dock = DockStyle.Fill;
            _fastModeTLP.Location = new Point(3, 3);
            _fastModeTLP.Name = "_fastModeTLP";
            _fastModeTLP.RowCount = 2;
            _fastModeTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            _fastModeTLP.RowStyles.Add(new RowStyle());
            _fastModeTLP.Size = new Size(423, 368);
            _fastModeTLP.TabIndex = 1;
            // 
            // _fastModeButtonsTLP
            // 
            _fastModeButtonsTLP.ColumnCount = 2;
            _fastModeButtonsTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _fastModeButtonsTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _fastModeButtonsTLP.Controls.Add(_btnFastStop, 1, 0);
            _fastModeButtonsTLP.Controls.Add(_btnFastStart, 0, 0);
            _fastModeButtonsTLP.Dock = DockStyle.Fill;
            _fastModeButtonsTLP.Location = new Point(3, 3);
            _fastModeButtonsTLP.Name = "_fastModeButtonsTLP";
            _fastModeButtonsTLP.RowCount = 1;
            _fastModeButtonsTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _fastModeButtonsTLP.Size = new Size(417, 44);
            _fastModeButtonsTLP.TabIndex = 0;
            // 
            // _btnFastStop
            // 
            _btnFastStop.BackColor = Color.FromArgb(64, 64, 64);
            _btnFastStop.BackgroundImageLayout = ImageLayout.None;
            _btnFastStop.Cursor = Cursors.Hand;
            _btnFastStop.Dock = DockStyle.Fill;
            _btnFastStop.Enabled = false;
            _btnFastStop.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _btnFastStop.FlatStyle = FlatStyle.Flat;
            _btnFastStop.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _btnFastStop.Location = new Point(211, 3);
            _btnFastStop.Name = "_btnFastStop";
            _btnFastStop.Size = new Size(203, 38);
            _btnFastStop.TabIndex = 11;
            _btnFastStop.Text = "Остановить";
            _btnFastStop.UseVisualStyleBackColor = false;
            // 
            // _btnFastStart
            // 
            _btnFastStart.BackColor = Color.FromArgb(64, 64, 64);
            _btnFastStart.BackgroundImageLayout = ImageLayout.None;
            _btnFastStart.Cursor = Cursors.Hand;
            _btnFastStart.Dock = DockStyle.Fill;
            _btnFastStart.Enabled = false;
            _btnFastStart.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _btnFastStart.FlatStyle = FlatStyle.Flat;
            _btnFastStart.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _btnFastStart.Location = new Point(3, 3);
            _btnFastStart.Name = "_btnFastStart";
            _btnFastStart.Size = new Size(202, 38);
            _btnFastStart.TabIndex = 10;
            _btnFastStart.Text = "Запустить";
            _btnFastStart.UseVisualStyleBackColor = false;
            // 
            // _fastTerminal
            // 
            _fastTerminal.BackColor = Color.FromArgb(31, 31, 31);
            _fastTerminal.BinaryInputMode = true;
            _fastTerminal.Dock = DockStyle.Fill;
            _fastTerminal.Location = new Point(3, 53);
            _fastTerminal.Name = "_fastTerminal";
            _fastTerminal.Size = new Size(417, 354);
            _fastTerminal.TabIndex = 1;
            // 
            // _interactionTabControl_AAInfoTab
            // 
            _interactionTabControl_AAInfoTab.BackColor = Color.FromArgb(31, 31, 31);
            _interactionTabControl_AAInfoTab.Controls.Add(_createReportButton);
            _interactionTabControl_AAInfoTab.Controls.Add(_infoLabel);
            _interactionTabControl_AAInfoTab.ForeColor = Color.FromArgb(214, 214, 214);
            _interactionTabControl_AAInfoTab.Location = new Point(4, 26);
            _interactionTabControl_AAInfoTab.Name = "_interactionTabControl_AAInfoTab";
            _interactionTabControl_AAInfoTab.Size = new Size(192, 70);
            _interactionTabControl_AAInfoTab.TabIndex = 2;
            _interactionTabControl_AAInfoTab.Text = "Создание отчёта";
            // 
            // _createReportButton
            // 
            _createReportButton.BackColor = Color.FromArgb(64, 64, 64);
            _createReportButton.BackgroundImageLayout = ImageLayout.None;
            _createReportButton.Cursor = Cursors.Hand;
            _createReportButton.Dock = DockStyle.Bottom;
            _createReportButton.Enabled = false;
            _createReportButton.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _createReportButton.FlatStyle = FlatStyle.Flat;
            _createReportButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _createReportButton.Location = new Point(0, 22);
            _createReportButton.Name = "_createReportButton";
            _createReportButton.Size = new Size(192, 48);
            _createReportButton.TabIndex = 11;
            _createReportButton.Text = "Создать отчёт и сохранить как...";
            _createReportButton.UseVisualStyleBackColor = false;
            // 
            // _infoLabel
            // 
            _infoLabel.AutoSize = true;
            _infoLabel.Font = new Font("Segoe UI", 9F);
            _infoLabel.Location = new Point(3, 10);
            _infoLabel.Name = "_infoLabel";
            _infoLabel.Size = new Size(368, 210);
            _infoLabel.TabIndex = 0;
            _infoLabel.Text = resources.GetString("_infoLabel.Text");
            // 
            // _saveFileDialog
            // 
            _saveFileDialog.RestoreDirectory = true;
            // 
            // AAController
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(31, 31, 31);
            Controls.Add(_aapMainTLP);
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(224, 224, 224);
            Name = "AAController";
            Size = new Size(742, 437);
            _aapMainTLP.ResumeLayout(false);
            _viewportGB.ResumeLayout(false);
            _aapInteractionGB.ResumeLayout(false);
            _interactionTabControl.ResumeLayout(false);
            _interactionTabControl_InteractiveModeTab.ResumeLayout(false);
            _interactiveModeTLP.ResumeLayout(false);
            _buttonsTLP.ResumeLayout(false);
            _interactionTabControl_FastModeTab.ResumeLayout(false);
            _fastModeTLP.ResumeLayout(false);
            _fastModeButtonsTLP.ResumeLayout(false);
            _interactionTabControl_AAInfoTab.ResumeLayout(false);
            _interactionTabControl_AAInfoTab.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _aapMainTLP;
        private GroupBox _viewportGB;
        private GroupBox _aapInteractionGB;
        private Viewports.ImageViewport _viewport;
        private CustomizableTabControl.CustomizableTabControl _interactionTabControl;
        private TabPage _interactionTabControl_InteractiveModeTab;
        private TableLayoutPanel _interactiveModeTLP;
        public Terminal.TerminalControl _terminal;
        private TableLayoutPanel _buttonsTLP;
        private TabPage _interactionTabControl_FastModeTab;
        private TableLayoutPanel _fastModeTLP;
        private TableLayoutPanel _fastModeButtonsTLP;
        private Button _btnFastStop;
        private Button _btnFastStart;
        public Terminal.TerminalControl _fastTerminal;

       
        private Label _infoLabel;      
        private Button _stopWorkImitation;
        private Button _startWorkImitation;
        private TabPage _interactionTabControl_AAInfoTab;
        private Button _createReportButton;
        private SaveFileDialog _saveFileDialog;
    }
}