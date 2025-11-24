using AlgorithmDeveloper.AlgorithmModel.LAS;
using AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling;
using AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl;
using AlgorithmDeveloper.Resources.UI.Controls.Viewports;
using WinRT;

namespace AlgorithmDeveloper
{
    public partial class MainFormV2 : Form
    {
        // Статический парсер используется напрямую

        public MainFormV2()
        {
            InitializeComponent();
            // Подписка на событие загрузки формы, чтобы выполнить привязку клавиатур
            this.Load += MainFormV2_Load;
        }

        private void MainFormV2_Load(object sender, EventArgs e)
        {
            using (var font = new Font("Consolas", 10f))
            {
                _mainInfoPort.Font = font;
            }
            _mainInfoPort.WordWrap = false;

            lasGraphicKeyboard1.BindTargets(_lasInputRTB1, _lasInputRTB2);

            // Явно укажем активные цели, чтобы ввод начинался даже без фокуса
            lasGraphicKeyboard1.ActiveTarget = _lasInputRTB1;

            InitVisualizationSettingsBindings();

            _masFLP.SizeChanged += (s, e) => { UpdateMasFLPLayout(); };
            _masFLP.ControlAdded += (s, e) => { UpdateMasFLPLayout(); SortMasViewportChildren(); };
            _masFLP.ControlRemoved += (s, e) => { UpdateMasFLPLayout(); SortMasViewportChildren(); };
            UpdateMasFLPLayout();

            _mainScrollPanel.SizeChanged += (s, e) => { AdjustMainLayoutRowHeights(); };
            AdjustMainLayoutRowHeights();

            this.SizeChanged += (s, e) => { AdjustMainLayoutRowHeights(); };
        }




        #region Приватная часть

        private void InitVisualizationSettingsBindings()
        {
            try
            {
                containerCP.Enabled = true;
                borderNUD.Enabled = true;
                cirlceDiameterNUD.Enabled = true;
                transitionLightPenNUD.Enabled = true;
                transitionBlackPenNUD.Enabled = true;
                transitionLightPenCP.Enabled = true;
                transitionBlackPenCP.Enabled = true;
                innerStateCP.Enabled = true;
                inactiveBorderCP.Enabled = true;
                activeBorderCP.Enabled = true;

                VisualizationSettings.ContainerColor = containerCP.BackColor;
                VisualizationSettings.FigureStrokeWidth = (float)borderNUD.Value;
                VisualizationSettings.FigureHalfWidth = (float)cirlceDiameterNUD.Value;
                VisualizationSettings.FigureHalfHeight = Math.Max(20f, VisualizationSettings.FigureHalfWidth * 0.5f);
                VisualizationSettings.EdgeActiveColor = transitionLightPenCP.BackColor;
                VisualizationSettings.EdgeActiveWidth = (float)transitionLightPenNUD.Value;
                VisualizationSettings.EdgeInactiveColor = transitionBlackPenCP.BackColor;
                VisualizationSettings.EdgeInactiveWidth = (float)transitionBlackPenNUD.Value;

                borderNUD.ValueChanged += (s, e) => { VisualizationSettings.FigureStrokeWidth = (float)borderNUD.Value; RefreshViews(); };
                cirlceDiameterNUD.ValueChanged += (s, e) => { VisualizationSettings.FigureHalfWidth = (float)cirlceDiameterNUD.Value; VisualizationSettings.FigureHalfHeight = Math.Max(20f, VisualizationSettings.FigureHalfWidth * 0.5f); RefreshViews(); };
                transitionLightPenNUD.ValueChanged += (s, e) => { VisualizationSettings.EdgeActiveWidth = (float)transitionLightPenNUD.Value; RefreshViews(); };
                transitionBlackPenNUD.ValueChanged += (s, e) => { VisualizationSettings.EdgeInactiveWidth = (float)transitionBlackPenNUD.Value; RefreshViews(); };

                containerCP.BackColorChanged += (s, e) => { VisualizationSettings.ContainerColor = containerCP.BackColor; RefreshViews(); };
                activeBorderCP.BackColorChanged += (s, e) => { VisualizationSettings.FigureStrokeColor = activeBorderCP.BackColor; RefreshViews(); };
                inactiveBorderCP.BackColorChanged += (s, e) => { VisualizationSettings.FigureStrokeColor = inactiveBorderCP.BackColor; RefreshViews(); };
                highlightedBorderCP.BackColorChanged += (s, e) => { VisualizationSettings.FigureFillColor = highlightedBorderCP.BackColor; RefreshViews(); };
                innerStateCP.BackColorChanged += (s, e) => { VisualizationSettings.FigureTextColor = innerStateCP.BackColor; RefreshViews(); };
                transitionLightPenCP.BackColorChanged += (s, e) => { VisualizationSettings.EdgeActiveColor = transitionLightPenCP.BackColor; RefreshViews(); };
                transitionBlackPenCP.BackColorChanged += (s, e) => { VisualizationSettings.EdgeInactiveColor = transitionBlackPenCP.BackColor; RefreshViews(); };
            }
            catch { }
        }

        private void RefreshViews()
        {
            try
            {
                if (_algoController1?.Model != null) _algoController1.Model = _algoController1.Model;
                if (_algoController2?.Model != null) _algoController2.Model = _algoController2.Model;
                if (_algoController3?.Model != null) _algoController3.Model = _algoController3.Model;
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
                    _mainInfoPort.Text = ex.Message;
                }
                else
                {
                    _tryParseLAS1_B.Enabled = false;
                    _createGAS1_B.Enabled = true;

                    _lasInputRTB1.ReadOnly = true;
                    // _lasInputRTB1.Cursor = Cursors.Arrow;
                    _lasInputRTB1.BackColor = Color.FromArgb(76, 76, 76);

                    _mainInfoPort.Text = string.Empty;
                }
            }

            if (sender == _tryParseLAS2_B)
            {
                _createGAS2_B.Enabled = LASParser.TryParse(_lasInputRTB2.Text, out ex);

                if (ex != null)
                {
                    _mainInfoPort.Text = ex.Message;
                }
                else
                {
                    _tryParseLAS2_B.Enabled = false;
                    _createGAS2_B.Enabled = true;

                    _lasInputRTB2.ReadOnly = true;
                    // _lasInputRTB2.Cursor = Cursors.Arrow;
                    _lasInputRTB2.BackColor = Color.FromArgb(76, 76, 76);

                    _mainInfoPort.Text = string.Empty;
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

                _mainInfoPort.Text = "Модель первого алгоритма создана успешно!";

                var masVP = new MASViewport();
                masVP.FillData("МСА 1 - матричная схема первого алгоритма", _algoController1.Model?.MAS?.DataTable!);
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

                _mainInfoPort.Text = "Модель второго алгоритма создана успешно!";

                var masVP = new MASViewport();
                masVP.FillData("МСА 2 - матричная схема второго алгоритма", _algoController2.Model?.MAS?.DataTable!);
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

        private void AdjustMainLayoutRowHeights()
        {
            try
            {
                int contentPadding = _mainTLP.Padding.Vertical + _mainTLP.Margin.Vertical + _mainScrollPanel.Padding.Vertical;
                int fixedRowsHeight = GetFullHeight(_mainGB) + GetFullHeight(_combinedAlgoGB) + GetFullHeight(_infoGB);
                int available = Math.Max(0, _mainScrollPanel.ClientSize.Height - contentPadding);
                int target = available - fixedRowsHeight;
                if (target < _masGB.MinimumSize.Height) target = _masGB.MinimumSize.Height;
                _mainTLP.RowStyles[2].SizeType = SizeType.Absolute;
                _mainTLP.RowStyles[2].Height = target;
                _mainTLP.PerformLayout();
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
                    _masFLP.FlowDirection = FlowDirection.TopDown;
                    _masFLP.WrapContents = false;
                    _masFLP.AutoScroll = true;
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
                _masFLP.PerformLayout();
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
