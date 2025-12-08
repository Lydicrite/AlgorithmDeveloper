using AlgorithmDeveloper.AAModel.LAS;
using AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling;
using AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl;
using AlgorithmDeveloper.Resources.UI.Controls.Viewports;
using WinRT;

namespace AlgorithmDeveloper
{
    public partial class MainFormV2 : Form
    {
        public MainFormV2()
        {
            InitializeComponent();
            this.Load += MainFormV2_Load;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void MainFormV2_Load(object? sender, EventArgs e)
        {
            _mainInfoPort.Font = new Font("Consolas", 10f);
            _mainInfoPort.WordWrap = false;
            lasGraphicKeyboard1.BindTargets(_lasInputRTB1, _lasInputRTB2);
            lasGraphicKeyboard1.ActiveTarget = _lasInputRTB1;
            InitVisualizationSettingsBindings();

            DoubleBuffered = true;
            var dbProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            dbProp?.SetValue(_masFLP, true);
            dbProp?.SetValue(_mainScrollPanel, true);
            dbProp?.SetValue(_mainTLP, true);

            _resizeTimer = new System.Windows.Forms.Timer();
            _resizeTimer.Interval = 100;
            _resizeTimer.Tick += (s2, e2) =>
            {
                _resizeTimer.Stop();
                if (_resizePending)
                {
                    _resizePending = false;
                    AdjustMainLayoutRowHeights();
                }
                if (_masLayoutPending)
                {
                    _masLayoutPending = false;
                    UpdateMasFLPLayout();
                }
            };

            _masFLP.SizeChanged += (s, e) => { _masLayoutPending = true; ScheduleResizeAdjust(); };
            _masFLP.ControlAdded += (s, e) => { UpdateMasFLPLayout(); SortMasViewportChildren(); };
            _masFLP.ControlRemoved += (s, e) => { UpdateMasFLPLayout(); SortMasViewportChildren(); };
            UpdateMasFLPLayout();

            _mainScrollPanel.SizeChanged += (s, e) => { _resizePending = true; ScheduleResizeAdjust(); };
            AdjustMainLayoutRowHeights();

            this.SizeChanged += (s, e) => { _resizePending = true; ScheduleResizeAdjust(); };
        }




        #region Приватная часть

        private System.Windows.Forms.Timer _resizeTimer = null!;
        private bool _resizePending;
        private bool _masLayoutPending;

        private void ScheduleResizeAdjust()
        {
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }

        private void InitVisualizationSettingsBindings()
        {
            try
            {
                _viewportCP.Enabled = true;
                _borderNUD.Enabled = true;
                _vertexSizeNUD.Enabled = true;
                _transitionLightPenNUD.Enabled = true;
                _transitionDarkPenNUD.Enabled = true;
                _transitionLightPenCP.Enabled = true;
                _transitionDarkPenCP.Enabled = true;
                _vertexInactInnerCP.Enabled = true;
                _vertexActInnerCP.Enabled = true;
                _inactiveBorderCP.Enabled = true;
                _activeBorderCP.Enabled = true;

                VisualizationSettings.ContainerColor = _viewportCP.BackColor;
                VisualizationSettings.FigureStrokeWidth = (float)_borderNUD.Value;
                VisualizationSettings.FigureHalfWidth = (float)_vertexSizeNUD.Value;
                VisualizationSettings.FigureHalfHeight = Math.Max(20f, VisualizationSettings.FigureHalfWidth * 0.5f);
                VisualizationSettings.EdgeActiveColor = _transitionLightPenCP.BackColor;
                VisualizationSettings.EdgeActiveWidth = (float)_transitionLightPenNUD.Value;
                VisualizationSettings.EdgeInactiveColor = _transitionDarkPenCP.BackColor;
                VisualizationSettings.EdgeInactiveWidth = (float)_transitionDarkPenNUD.Value;
                
                VisualizationSettings.FigureStrokeColor = _inactiveBorderCP.BackColor;
                VisualizationSettings.FigureStrokeActiveColor = _activeBorderCP.BackColor;
                VisualizationSettings.FigureFillColor = _vertexInactInnerCP.BackColor;
                VisualizationSettings.FigureFillActiveColor = _vertexActInnerCP.BackColor;

                _borderNUD.ValueChanged += (s, e) => { VisualizationSettings.FigureStrokeWidth = (float)_borderNUD.Value; RefreshViews(); };
                _vertexSizeNUD.ValueChanged += (s, e) => { VisualizationSettings.FigureHalfWidth = (float)_vertexSizeNUD.Value; VisualizationSettings.FigureHalfHeight = Math.Max(20f, VisualizationSettings.FigureHalfWidth * 0.5f); RefreshViews(); };
                _transitionLightPenNUD.ValueChanged += (s, e) => { VisualizationSettings.EdgeActiveWidth = (float)_transitionLightPenNUD.Value; RefreshViews(); };
                _transitionDarkPenNUD.ValueChanged += (s, e) => { VisualizationSettings.EdgeInactiveWidth = (float)_transitionDarkPenNUD.Value; RefreshViews(); };

                _viewportCP.Click += ColorPicker_Click;
                _activeBorderCP.Click += ColorPicker_Click;
                _inactiveBorderCP.Click += ColorPicker_Click;
                _vertexInactInnerCP.Click += ColorPicker_Click;
                _vertexActInnerCP.Click += ColorPicker_Click;
                _transitionLightPenCP.Click += ColorPicker_Click;
                _transitionDarkPenCP.Click += ColorPicker_Click;
            }
            catch { }
        }

        private void ColorPicker_Click(object? sender, EventArgs e)
        {
            if (sender is not PictureBox pb) return;

            using var cd = new ColorDialog();
            cd.Color = pb.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pb.BackColor = cd.Color;

                // Обновляем настройки
                if (sender == _viewportCP) VisualizationSettings.ContainerColor = pb.BackColor;
                else if (sender == _activeBorderCP) VisualizationSettings.FigureStrokeActiveColor = pb.BackColor;
                else if (sender == _inactiveBorderCP) VisualizationSettings.FigureStrokeColor = pb.BackColor;
                else if (sender == _vertexInactInnerCP) VisualizationSettings.FigureFillActiveColor = pb.BackColor;
                else if (sender == _vertexActInnerCP) VisualizationSettings.FigureFillColor = pb.BackColor;
                else if (sender == _transitionLightPenCP) VisualizationSettings.EdgeActiveColor = pb.BackColor;
                else if (sender == _transitionDarkPenCP) VisualizationSettings.EdgeInactiveColor = pb.BackColor;

                RefreshViews();
            }
        }

        private void RefreshViews()
        {
            try
            {
                _algoController1?.UpdateVisualizationSettings();
                _algoController2?.UpdateVisualizationSettings();
                _algoController3?.UpdateVisualizationSettings();
            }
            catch { }
        }

        private void AdjustMainLayoutRowHeights()
        {
            try
            {
                int contentPadding = _mainTLP.Padding.Vertical + _mainTLP.Margin.Vertical + _mainScrollPanel.Padding.Vertical;
                int fixedRowsHeight = GetFullHeight(_mainGB) + GetFullHeight(_combinedAlgoGB) + GetFullHeight(_infoGB);
                int available = Math.Max(0, _mainScrollPanel.ClientSize.Height - contentPadding);
                int target = available - fixedRowsHeight;
                if (target < _masGB.MinimumSize.Height) target = _masGB.MinimumSize.Height;
                _mainTLP.SuspendLayout();
                _mainTLP.RowStyles[2].SizeType = SizeType.Absolute;
                _mainTLP.RowStyles[2].Height = target;
                _mainTLP.ResumeLayout(true);
            }
            catch { }
        }

        private int GetFullHeight(Control c)
        {
            return c.Height + c.Margin.Top + c.Margin.Bottom;
        }

        private void UpdateMasFLPLayout()
        {
            try
            {
                if (_masFLP.Controls.Count == 0)
                {
                    _masFLP.SuspendLayout();
                    _masFLP.FlowDirection = FlowDirection.TopDown;
                    _masFLP.WrapContents = false;
                    _masFLP.AutoScroll = true;
                    _masFLP.ResumeLayout(true);
                    return;
                }

                int availableWidth = _masFLP.ClientSize.Width - _masFLP.Padding.Horizontal;
                int totalWidth = 0;
                foreach (Control c in _masFLP.Controls)
                {
                    if (c is MASViewport)
                    {
                        totalWidth += c.Width + c.Margin.Horizontal;
                    }
                }

                bool fitsOneRow = totalWidth <= Math.Max(0, availableWidth);
                _masFLP.SuspendLayout();
                if (fitsOneRow)
                {
                    _masFLP.FlowDirection = FlowDirection.LeftToRight;
                    _masFLP.WrapContents = false;
                    _masFLP.AutoScroll = false;
                }
                else
                {
                    _masFLP.FlowDirection = FlowDirection.TopDown;
                    _masFLP.WrapContents = false;
                    _masFLP.AutoScroll = true;
                }
                _masFLP.ResumeLayout(true);
            }
            catch { }
        }

        private void SortMasViewportChildren()
        {
            try
            {
                var ordered = _masFLP.Controls
                    .OfType<MASViewport>()
                    .Select(v => new { View = v, Ord = v.Tag is int i ? i : 999 })
                    .OrderBy(x => x.Ord)
                    .Select(x => x.View)
                    .ToList();

                for (int i = 0; i < ordered.Count; i++)
                {
                    _masFLP.Controls.SetChildIndex(ordered[i], i);
                }
            }
            catch { }
        }




        private void EnableCheckLASButton(object sender, EventArgs e)
        {
            if (sender == _lasInputRTB1)
                _tryParseLAS1_B.Enabled = _lasInputRTB1.Text != string.Empty && _lasInputRTB1.Text.Contains("Yк");
            if (sender == _lasInputRTB2)
                _tryParseLAS2_B.Enabled = _lasInputRTB2.Text != string.Empty && _lasInputRTB2.Text.Contains("Yк");
        }

        private void CheckLAS(object sender, EventArgs e)
        {
            ParsingAggregateException? ex = null;

            if (sender == _tryParseLAS1_B)
            {
                _createGAS1_B.Enabled = LASParser.TryParse(_lasInputRTB1.Text, out ex);

                if (ex != null)
                {
                    LogError(ex.Message);
                    if (_mainScrollPanel.VerticalScroll.Visible) _mainScrollPanel.ScrollControlIntoView(_infoGB);
                }
                else
                {
                    _tryParseLAS1_B.Enabled = false;
                    _createGAS1_B.Enabled = true;

                    _lasInputRTB1.ReadOnly = true;
                    // _lasInputRTB1.Cursor = Cursors.Arrow;
                    _lasInputRTB1.BackColor = Color.FromArgb(76, 76, 76);
                }
            }

            if (sender == _tryParseLAS2_B)
            {
                _createGAS2_B.Enabled = LASParser.TryParse(_lasInputRTB2.Text, out ex);

                if (ex != null)
                {
                    LogError(ex.Message);
                    if (_mainScrollPanel.VerticalScroll.Visible) _mainScrollPanel.ScrollControlIntoView(_infoGB);
                }
                else
                {
                    _tryParseLAS2_B.Enabled = false;
                    _createGAS2_B.Enabled = true;

                    _lasInputRTB2.ReadOnly = true;
                    // _lasInputRTB2.Cursor = Cursors.Arrow;
                    _lasInputRTB2.BackColor = Color.FromArgb(76, 76, 76);
                }
            }
        }

        private void CreageInitialGAS(object sender, EventArgs e)
        {
            if (sender == _createGAS1_B)
            {
                _algoController1.Model = LASParser.Parse(_lasInputRTB1.Text);

                _tryParseLAS1_B.Enabled = false;
                _createGAS1_B.Enabled = false;

                _lasInputRTB1.ReadOnly = true;
                // _lasInputRTB1.Cursor = Cursors.Arrow;
                _lasInputRTB1.BackColor = Color.FromArgb(76, 76, 76);
                LogSuccess("Модель первого алгоритма создана успешно.");

                var masVP = new MASViewport();
                masVP.FillData("МСА 1", _algoController1.Model?.MAS?.DataTable!);
                masVP.Tag = 1;

                _masFLP.Invoke((Action)(() =>
                {
                    _masFLP.SuspendLayout();
                    _masFLP.Controls.Add(masVP);
                    _masFLP.ResumeLayout();
                }));
                SortMasViewportChildren();
                UpdateMasFLPLayout();

                /*
                _btnRuns1.Enabled = true;
                _btnCycles1.Enabled = true;
                _btnTable1.Enabled = true;
                _btnAll1.Enabled = true;
                */
            }

            if (sender == _createGAS2_B)
            {
                _algoController2.Model = LASParser.Parse(_lasInputRTB2.Text);

                _tryParseLAS2_B.Enabled = false;
                _createGAS2_B.Enabled = false;

                _lasInputRTB2.ReadOnly = true;
                // _lasInputRTB2.Cursor = Cursors.Arrow;
                _lasInputRTB2.BackColor = Color.FromArgb(76, 76, 76);
                LogSuccess("Модель второго алгоритма создана успешно.");

                var masVP = new MASViewport();
                masVP.FillData("МСА 2", _algoController2.Model?.MAS?.DataTable!);
                masVP.Tag = 2;

                _masFLP.Invoke((Action)(() =>
                {
                    _masFLP.SuspendLayout();
                    _masFLP.Controls.Add(masVP);
                    _masFLP.ResumeLayout();
                }));
                SortMasViewportChildren();
                UpdateMasFLPLayout();

                /*
                _btnRuns2.Enabled = true;
                _btnCycles2.Enabled = true;
                _btnTable2.Enabled = true;
                _btnAll2.Enabled = true;
                */
            }
        }

        private void CreateCombinedGAS(object sender, EventArgs e)
        {
            /// TODO: Реализовать создание комбинированной МСА из двух моделей - будет в будущем, алгоритм объединения пока не готов
        }





        private void AppendText(RichTextBox terminal, string text, Color color, int fontSize, bool bold)
        {
            terminal.Invoke((Action)(() =>
            {
                terminal.SelectionStart = terminal.TextLength;
                terminal.SelectionLength = 0;
                terminal.SelectionColor = color;
                terminal.SelectionFont = new Font(terminal.Font.FontFamily, fontSize, bold ? FontStyle.Bold : FontStyle.Regular);
                terminal.AppendText(text);
                terminal.SelectionColor = terminal.ForeColor;
                terminal.ScrollToCaret();
            }));
        }

        private void LogMessage(string message, Color tsColor)
        {
            int fs = (int)Math.Round(_mainInfoPort.Font.Size);
            AppendText(_mainInfoPort, "\n\n\n", _mainInfoPort.ForeColor, fs, false);
            AppendText(_mainInfoPort, $"[{DateTime.Now:dd.MM.yyyy, HH:mm}] -> ", tsColor, fs, false);
            AppendText(_mainInfoPort, message, _mainInfoPort.ForeColor, fs, false);
        }

        private void LogSuccess(string message)
        {
            LogMessage(message, Color.DarkSeaGreen);
        }

        private void LogError(string message)
        {
            LogMessage(message, Color.IndianRed);
        }

        private void LogInfo(string message)
        {
            LogMessage(message, _mainInfoPort.ForeColor);
        }



        #endregion





        #region Доп. кнопки

        /*
        private void ShowRuns(object sender, EventArgs e)
        {
            if (sender == _btnRuns1)
                _mainTerminal.Text = _algoController1.Model?.RunsInfo ?? "������ �� �������.";
            if (sender == _btnRuns2)
                _mainTerminal.Text = _algoController2.Model?.RunsInfo ?? "������ �� �������.";
        }

        private void ShowCycles(object sender, EventArgs e)
        {
            if (sender == _btnCycles1)
                _mainTerminal.Text = _algoController1.Model?.CyclesInfo ?? "������ �� �������.";
            if (sender == _btnCycles2)
                _mainTerminal.Text = _algoController2.Model?.CyclesInfo ?? "������ �� �������.";
        }

        private void ShowTables(object sender, EventArgs e)
        {
            if (sender == _btnTable1)
                _mainTerminal.Text = _algoController1.Model?.TransitionsAndMASInfo ?? "������ �� �������.";
            if (sender == _btnTable2)
                _mainTerminal.Text = _algoController2.Model?.TransitionsAndMASInfo ?? "������ �� �������.";
        }

        private void ShowAll(object sender, EventArgs e)
        {
            if (sender == _btnAll1)
                _mainTerminal.Text = _algoController1.Model?.Information ?? "������ �� �������.";
            if (sender == _btnAll2)
                _mainTerminal.Text = _algoController2.Model?.Information ?? "������ �� �������.";
        }
        */

        #endregion
    }
}
