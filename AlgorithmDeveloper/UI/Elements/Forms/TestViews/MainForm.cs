using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.Combining;
using AlgorithmDeveloper.Abstractions.LAS;
using AlgorithmDeveloper.Abstractions.TransitionSystem.MAS;
using AlgorithmDeveloper.UI.Elements.Controls.Viewports;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph.Layout;
using System.Windows.Interop;

using AlgorithmDeveloper.UI.Elements.Utils;

namespace AlgorithmDeveloper
{
    public partial class MainForm : Form
    {
        private string _currentFilePath = string.Empty;
        private bool _isDirty = false;

        #region Перемещение и изменение размеров окна

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int RESIZE_HANDLE_SIZE = 10;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);

                if (this.WindowState == FormWindowState.Normal)
                {
                    if ((int)m.Result == 0x1) // HTCLIENT
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32());
                        Point clientPoint = this.PointToClient(screenPoint);

                        if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)13; // HTTOPLEFT
                            else if (clientPoint.X < (this.Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)12; // HTTOP
                            else
                                m.Result = (IntPtr)14; // HTTOPRIGHT
                        }
                        else if (clientPoint.Y <= (this.Size.Height - RESIZE_HANDLE_SIZE))
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)10; // HTLEFT
                            else if (clientPoint.X >= (this.Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)11; // HTRIGHT
                        }
                        else
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)16; // HTBOTTOMLEFT
                            else if (clientPoint.X < (this.Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)15; // HTBOTTOM
                            else
                                m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                        }
                    }
                }
                return;
            }
            base.WndProc(ref m);
        }

        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Global Hotkeys
            if (keyData == (Keys.Control | Keys.S))
            {
                SaveCombinedPair(null, EventArgs.Empty);
                return true;
            }
            if (keyData == (Keys.Control | Keys.Shift | Keys.S))
            {
                SaveCombinedPairAs(null, EventArgs.Empty);
                return true;
            }
            if (keyData == (Keys.Control | Keys.L))
            {
                LoadCombinedPair(null, EventArgs.Empty);
                return true;
            }
            if (keyData == (Keys.Control | Keys.D))
            {
                ResetWorkspace();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public MainForm()
        {
            InitializeComponent();
            _resizeTimer = new System.Windows.Forms.Timer();
            this.Load += MainForm_Load;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // Drag and Drop
            this.AllowDrop = true;
            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;

            // Window controls wiring
            _headerStrip.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };
            
            _closeButton.Click += (s, e) => Close();
            _maximizeButton.Click += (s, e) => WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            _minimizeButton.Click += (s, e) => WindowState = FormWindowState.Minimized;

            this.Resize += (s, e) =>
            {
                _maximizeButton.Text = WindowState == FormWindowState.Maximized ? "❐" : "☐";
                _maximizeButton.ToolTipText = WindowState == FormWindowState.Maximized ? "Восстановить" : "Развернуть";
            };

            // Wiring File Menu Actions
            _loadToCombinePair.Click += LoadCombinedPair;
            _saveToCombinePair.Click += SaveCombinedPair;
            _saveToCombinePairAs.Click += SaveCombinedPairAs;
            _gotoProgramFolder.Click += OpenProgramDataFolder;
            _clearAllGAS.Click += (s, e) => ResetWorkspace();

            UpdateRecentFilesMenu();

            // Load settings on startup
            try
            {
                VisualizationSettings.Load(AppDataManager.GetPath("visualization_settings.json"));
            }
            catch { }

            this.FormClosing += (s, e) =>
            {
                if (!PromptSaveIfDirty())
                {
                    e.Cancel = true;
                    return;
                }

                try
                {
                    VisualizationSettings.Save(AppDataManager.GetPath("visualization_settings.json"));
                }
                catch { }
            };
        }

        private void MainForm_Load(object? sender, EventArgs e)
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

            _lasInputRTB1.TextChanged += (s, e) => SetDirty(true);
            _lasInputRTB2.TextChanged += (s, e) => SetDirty(true);
            _lasInputRTB1.TextChanged += EnableCheckLASButton;
            _lasInputRTB2.TextChanged += EnableCheckLASButton;

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

        #region Отрисовка

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
                _drawStepDelayNUD.Enabled = true;
                _linkIndentNUD.Enabled = true;

                // Layout Settings
                _layeringStrategyCB.DataSource = Enum.GetValues(typeof(LayeringStrategy));
                _layeringStrategyCB.SelectedItem = VisualizationSettings.LayoutLayeringStrategy;
                _normalizeComponentsCB.Checked = VisualizationSettings.LayoutNormalizeComponents;
                _hSpacingNUD.Value = VisualizationSettings.HorizontalSpacing;
                _vSpacingNUD.Value = VisualizationSettings.VerticalSpacing;
                _startXNUD.Value = VisualizationSettings.StartX;
                _startYNUD.Value = VisualizationSettings.StartY;

                VisualizationSettings.ContainerColor = _viewportCP.BackColor;
                VisualizationSettings.FigureStrokeWidth = (float)_borderNUD.Value;
                VisualizationSettings.FigureHalfWidth = (float)_vertexSizeNUD.Value;
                VisualizationSettings.FigureHalfHeight = Math.Max(20f, VisualizationSettings.FigureHalfWidth * 0.5f);
                VisualizationSettings.EdgeActiveColor = _transitionLightPenCP.BackColor;
                VisualizationSettings.EdgeActiveWidth = (float)_transitionLightPenNUD.Value;
                VisualizationSettings.EdgeInactiveColor = _transitionDarkPenCP.BackColor;
                VisualizationSettings.EdgeInactiveWidth = (float)_transitionDarkPenNUD.Value;
                VisualizationSettings.VisualizationDelay = (int)(_drawStepDelayNUD.Value * 1000);
                VisualizationSettings.LinkIndent = (float)_linkIndentNUD.Value;
                
                VisualizationSettings.FigureStrokeColor = _inactiveBorderCP.BackColor;
                VisualizationSettings.FigureStrokeActiveColor = _activeBorderCP.BackColor;
                VisualizationSettings.FigureFillColor = _vertexInactInnerCP.BackColor;
                VisualizationSettings.FigureFillActiveColor = _vertexActInnerCP.BackColor;

                _borderNUD.ValueChanged += (s, e) => { VisualizationSettings.FigureStrokeWidth = (float)_borderNUD.Value; RefreshViews(); };
                _vertexSizeNUD.ValueChanged += (s, e) => { VisualizationSettings.FigureHalfWidth = (float)_vertexSizeNUD.Value; VisualizationSettings.FigureHalfHeight = Math.Max(20f, VisualizationSettings.FigureHalfWidth * 0.5f); RefreshViews(); };
                _transitionLightPenNUD.ValueChanged += (s, e) => { VisualizationSettings.EdgeActiveWidth = (float)_transitionLightPenNUD.Value; RefreshViews(); };
                _transitionDarkPenNUD.ValueChanged += (s, e) => { VisualizationSettings.EdgeInactiveWidth = (float)_transitionDarkPenNUD.Value; RefreshViews(); };
                _drawStepDelayNUD.ValueChanged += (s, e) => { VisualizationSettings.VisualizationDelay = (int)(_drawStepDelayNUD.Value * 1000); };
                _linkIndentNUD.ValueChanged += (s, e) => { VisualizationSettings.LinkIndent = (float)_linkIndentNUD.Value; ReapplyLayouts(); };

                _layeringStrategyCB.SelectedIndexChanged += (s, e) => { if (_layeringStrategyCB.SelectedItem is LayeringStrategy strat) { VisualizationSettings.LayoutLayeringStrategy = strat; ReapplyLayouts(); } };
                _normalizeComponentsCB.CheckedChanged += (s, e) => { _normalizeComponentsCB.FlatAppearance.BorderColor = _normalizeComponentsCB.Checked ? Color.FromArgb(0, 170, 0) : Color.Maroon; VisualizationSettings.LayoutNormalizeComponents = _normalizeComponentsCB.Checked; ReapplyLayouts(); };
                _hSpacingNUD.ValueChanged += (s, e) => { VisualizationSettings.HorizontalSpacing = (int)_hSpacingNUD.Value; ReapplyLayouts(); };
                _vSpacingNUD.ValueChanged += (s, e) => { VisualizationSettings.VerticalSpacing = (int)_vSpacingNUD.Value; ReapplyLayouts(); };
                _startXNUD.ValueChanged += (s, e) => { VisualizationSettings.StartX = (int)_startXNUD.Value; ReapplyLayouts(); };
                _startYNUD.ValueChanged += (s, e) => { VisualizationSettings.StartY = (int)_startYNUD.Value; ReapplyLayouts(); };

                _allowBranchSwappingCB.CheckedChanged += (s, e) =>
                {
                    _allowBranchSwappingCB.FlatAppearance.BorderColor = _allowBranchSwappingCB.Checked ? Color.FromArgb(0, 170, 0) : Color.Maroon;
                    ReapplyLayouts();
                };

                _viewportCP.Click += ColorPicker_Click;
                _activeBorderCP.Click += ColorPicker_Click;
                _inactiveBorderCP.Click += ColorPicker_Click;
                _vertexInactInnerCP.Click += ColorPicker_Click;
                _vertexActInnerCP.Click += ColorPicker_Click;
                _transitionLightPenCP.Click += ColorPicker_Click;
                _transitionDarkPenCP.Click += ColorPicker_Click;

                _drawJumpPointsCB.CheckedChanged += _drawJumpPoints_CheckedChanged;
            }
            catch { }
        }

        private void _drawJumpPoints_CheckedChanged(object? sender, EventArgs e)
        {
            _drawJumpPointsCB.FlatAppearance.BorderColor = _drawJumpPointsCB.Checked ? Color.FromArgb(0, 170, 0) : Color.Maroon;
            
            _algoController1?.SetShowJumpPoints(_drawJumpPointsCB.Checked);
            _algoController2?.SetShowJumpPoints(_drawJumpPointsCB.Checked);
            _algoController3?.SetShowJumpPoints(_drawJumpPointsCB.Checked);
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

        private void ReapplyLayouts()
        {
            try
            {
                _algoController1?.ReapplyLayout();
                _algoController2?.ReapplyLayout();
                _algoController3?.ReapplyLayout();
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
                    _masFLP.AutoScroll = true;
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

        #endregion





        #region Работа с АА на главной вкладке

        private void EnableCheckLASButton(object? sender, EventArgs e)
        {
            if (sender == _lasInputRTB1)
                _tryParseLAS1_B.Enabled = _lasInputRTB1.Text != string.Empty && _lasInputRTB1.Text.Contains("Yк");
            if (sender == _lasInputRTB2)
                _tryParseLAS2_B.Enabled = _lasInputRTB2.Text != string.Empty && _lasInputRTB2.Text.Contains("Yк");
        }

        private void CheckLAS(object sender, EventArgs e)
        {
            try
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

                if (_algoController1.Model != null && _algoController2.Model != null)
                {
                    _createGAS3_B.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogError
                (
                    $"Возникла ошибка" +
                    $"\n\tИсточник: {ex.Source}" +
                    $"\n\tСообщение: {ex.Message}"
                );
            }
        }

        private void CreageInitialGAS(object sender, EventArgs e)
        {
            try
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
                }

                if (_algoController1.Model != null && _algoController2.Model != null)
                {
                    _createGAS3_B.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogError
                (
                    $"Возникла ошибка" +
                    $"\n\tИсточник: {ex.Source}" +
                    $"\n\tСообщение: {ex.Message}"
                );
            }
        }

        private void CreateCombinedGAS(object sender, EventArgs e)
        {
            try
            {
                if (_algoController1.Model != null && _algoController2.Model != null)
                {
                    var combinedModelFromMASCombiner = MASCombiner.Combine(_algoController1.Model, _algoController2.Model);
                    var combinedModelFromStructCombiner = StructuralMerger.Combine(_algoController1.Model, _algoController2.Model);

                    string msgMas = string.Empty;
                    bool isMasCorrect = combinedModelFromMASCombiner.CheckCorrectness(out msgMas);
                    int masCount = combinedModelFromMASCombiner.Vertices.Count();

                    string msgStruct = string.Empty;
                    bool isStructCorrect = combinedModelFromStructCombiner.CheckCorrectness(out msgStruct);
                    int structCount = combinedModelFromStructCombiner.Vertices.Count();

                    LogInfo($"Результаты объединения:\n" +
                            $"- MASCombiner: {masCount} вершин, Корректна: {isMasCorrect}\n" +
                            $"- StructuralMerger: {structCount} вершин, Корректна: {isStructCorrect}");

                    if (!isMasCorrect && !string.IsNullOrWhiteSpace(msgMas)) 
                        LogInfo($"Детали ошибок MASCombiner: {msgMas}");
                    if (!isStructCorrect && !string.IsNullOrWhiteSpace(msgStruct)) 
                        LogInfo($"Детали ошибок StructuralMerger: {msgStruct}");

                    AbstractAutomata? combinedModel = null;

                    if (isMasCorrect && isStructCorrect)
                    {
                        if (masCount <= structCount)
                        {
                            combinedModel = combinedModelFromMASCombiner;
                            LogSuccess($"Выбрана модель MASCombiner (обе корректны, выбрана с меньшим/равным числом вершин).");
                        }
                        else
                        {
                            combinedModel = combinedModelFromStructCombiner;
                            LogSuccess($"Выбрана модель StructuralMerger (обе корректны, выбрана с меньшим числом вершин).");
                        }
                    }
                    else if (isMasCorrect)
                    {
                        combinedModel = combinedModelFromMASCombiner;
                        LogSuccess("Выбрана модель MASCombiner (единственная корректная).");
                    }
                    else if (isStructCorrect)
                    {
                        combinedModel = combinedModelFromStructCombiner;
                        LogSuccess("Выбрана модель StructuralMerger (единственная корректная).");
                    }
                    else
                    {
                        LogError("Не удалось синтезировать корректную модель ни одним из методов.");
                        return;
                    }

                    _algoController3.Model = combinedModel;
                    _lasInputRTB3.Text = _algoController3.Model.LAS;

                    var masVP = new MASViewport();
                    masVP.FillData("МСА 3", _algoController3.Model?.MAS?.DataTable!);
                    masVP.Tag = 3;

                    _masFLP.Invoke((Action)(() =>
                    {
                        _masFLP.SuspendLayout();
                        _masFLP.Controls.Add(masVP);
                        _masFLP.ResumeLayout();
                    }));
                    SortMasViewportChildren();
                    UpdateMasFLPLayout();

                    LogSuccess("Модель третьего алгоритма синтезирована успешно.");

                    _createGAS3_B.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                LogError
                (
                    $"Возникла ошибка" +
                    $"\n\tИсточник: {ex.Source}" +
                    $"\n\tСообщение: {ex.Message}"
                );
            }
        }

        #endregion





        #region Логгирование в журнал

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
            if (_mainInfoPort.TextLength > 0)
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





        #region Меню Файл

        private void UpdateRecentFilesMenu()
        {
            _showLastToCombinePairs.DropDownItems.Clear();
            var files = RecentFilesManager.RecentFiles;

            if (files.Count == 0)
            {
                var item = new ToolStripMenuItem("Нет последних файлов");
                item.Enabled = false;
                item.ForeColor = Color.DimGray;
                _showLastToCombinePairs.DropDownItems.Add(item);
                return;
            }

            for (int i = 0; i < files.Count; i++)
            {
                string path = files[i];
                string text = $"{i + 1}. \"{path}\"";
                var item = new ToolStripMenuItem(text);
                item.Tag = path;
                item.ForeColor = Color.FromArgb(214, 214, 214);
                item.Click += (s, e) => LoadCombinedPairFromFile((string)((ToolStripMenuItem)s!).Tag!);
                _showLastToCombinePairs.DropDownItems.Add(item);
            }
        }

        private void SetDirty(bool value)
        {
            _isDirty = value;
            string title = "Algorithm Developer";
            if (!string.IsNullOrEmpty(_currentFilePath))
                title += $" - {_currentFilePath}";
            if (_isDirty)
                title += "*";
            
            this.Text = title;
        }

        private bool PromptSaveIfDirty()
        {
            if (!_isDirty) return true;

            var result = MessageBox.Show(
                "Сохранить изменения перед продолжением?", 
                "Несохраненные изменения", 
                MessageBoxButtons.YesNoCancel, 
                MessageBoxIcon.Question);

            if (result == DialogResult.Cancel) return false;

            if (result == DialogResult.Yes)
            {
                SaveCombinedPair(null, EventArgs.Empty);
            }

            return true;
        }

        private bool ResetWorkspace()
        {
            if (!PromptSaveIfDirty()) return false;

            // Сброс полей ввода
            _lasInputRTB1.Text = "";
            _lasInputRTB1.ReadOnly = false;
            _lasInputRTB1.BackColor = Color.FromArgb(48, 48, 48);

            _lasInputRTB2.Text = "";
            _lasInputRTB2.ReadOnly = false;
            _lasInputRTB2.BackColor = Color.FromArgb(48, 48, 48);

            _lasInputRTB3.Text = "";
            _lasInputRTB3.BackColor = Color.FromArgb(48, 48, 48);

            // Сброс кнопок
            _tryParseLAS1_B.Enabled = false;
            _createGAS1_B.Enabled = false;
            _tryParseLAS2_B.Enabled = false;
            _createGAS2_B.Enabled = false;
            _createGAS3_B.Enabled = false;

            // Очистка моделей
            _algoController1.Model = null;
            _algoController2.Model = null;
            _algoController3.Model = null;

            // Очистка визуализации
            _masFLP.Controls.Clear();
            _algoController1.UpdateVisualizationSettings(); // Trigger redraw/clear
            _algoController2.UpdateVisualizationSettings();
            _algoController3.UpdateVisualizationSettings();

            // Сброс текущего файла
            _currentFilePath = string.Empty;

            // Очистка лога
            _mainInfoPort.Clear();
            LogInfo("Рабочее пространство очищено.");
            
            SetDirty(false);
            return true;
        }

        private void MainForm_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object? sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                LoadCombinedPairFromFile(files[0]);
            }
        }

        private void LoadCombinedPair(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            ofd.Title = "Загрузить объединяемую пару";
            
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadCombinedPairFromFile(ofd.FileName);
            }
        }

        private void LoadCombinedPairFromFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                LogError($"Файл не найден: {filePath}");
                UpdateRecentFilesMenu(); // Обновит список, удалив несуществующие
                return;
            }

            // Сбрасываем перед загрузкой (с проверкой изменений)
            if (!ResetWorkspace()) return;

            try
            {
                var json = System.IO.File.ReadAllText(filePath);
                var data = System.Text.Json.JsonSerializer.Deserialize<CombinedPairData>(json);

                if (data != null)
                {
                    _currentFilePath = filePath;
                    
                    // Добавляем в историю
                    RecentFilesManager.AddFile(filePath);
                    UpdateRecentFilesMenu();

                    // Заполняем текстовые поля
                    _lasInputRTB1.Text = data.Las1;
                    _lasInputRTB2.Text = data.Las2;

                    LogInfo($"Загружен файл: {_currentFilePath}");
                    
                    // Сбрасываем флаг изменений после успешной загрузки
                    SetDirty(false);

                    // Автоматически запускаем процесс создания моделей
                    
                    // 1. Проверяем и создаем первую модель
                    if (!string.IsNullOrWhiteSpace(_lasInputRTB1.Text))
                    {
                        ParsingAggregateException? ex = null;
                        if (LASParser.TryParse(_lasInputRTB1.Text, out ex))
                        {
                            // Эмуляция нажатия _createGAS1_B
                            _algoController1.Model = LASParser.Parse(_lasInputRTB1.Text);
                            _tryParseLAS1_B.Enabled = false;
                            _createGAS1_B.Enabled = false;
                            _lasInputRTB1.ReadOnly = true;
                            _lasInputRTB1.BackColor = Color.FromArgb(76, 76, 76);
                            
                            AddMasViewport(1, "МСА 1", _algoController1.Model?.MAS?.DataTable!);
                        }
                        else
                        {
                            LogError($"Ошибка парсинга LAS 1: {ex?.Message}");
                        }
                    }

                    // 2. Проверяем и создаем вторую модель
                    if (!string.IsNullOrWhiteSpace(_lasInputRTB2.Text))
                    {
                        ParsingAggregateException? ex = null;
                        if (LASParser.TryParse(_lasInputRTB2.Text, out ex))
                        {
                            _algoController2.Model = LASParser.Parse(_lasInputRTB2.Text);
                            _tryParseLAS2_B.Enabled = false;
                            _createGAS2_B.Enabled = false;
                            _lasInputRTB2.ReadOnly = true;
                            _lasInputRTB2.BackColor = Color.FromArgb(76, 76, 76);
                            
                            AddMasViewport(2, "МСА 2", _algoController2.Model?.MAS?.DataTable!);
                        }
                        else
                        {
                            LogError($"Ошибка парсинга LAS 2: {ex?.Message}");
                        }
                    }

                    // 3. Если обе модели созданы, объединяем
                    if (_algoController1.Model != null && _algoController2.Model != null)
                    {
                        CreateCombinedGAS(this, EventArgs.Empty);
                    }
                }
            }
            catch (System.Text.Json.JsonException jex)
            {
                LogError($"Ошибка валидации JSON: Файл имеет неверный формат.\n{jex.Message}");
                MessageBox.Show("Выбранный файл не является корректным JSON файлом для этого приложения.", "Ошибка формата", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LogError($"Ошибка при загрузке файла: {ex.Message}");
            }
        }

        private void AddMasViewport(int tag, string title, System.Data.DataTable data)
        {
             // Удаляем существующий с таким тегом, если есть
            var existing = _masFLP.Controls.OfType<MASViewport>().FirstOrDefault(v => v.Tag is int t && t == tag);
            if (existing != null)
            {
                _masFLP.Controls.Remove(existing);
                existing.Dispose();
            }

            var masVP = new MASViewport();
            masVP.FillData(title, data);
            masVP.Tag = tag;

            _masFLP.Invoke((Action)(() =>
            {
                _masFLP.SuspendLayout();
                _masFLP.Controls.Add(masVP);
                _masFLP.ResumeLayout();
            }));
            SortMasViewportChildren();
            UpdateMasFLPLayout();
        }

        private void SaveCombinedPair(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                SaveCombinedPairAs(sender, e);
                return;
            }
            
            SaveToFile(_currentFilePath);
        }

        private void SaveCombinedPairAs(object? sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            sfd.Title = "Сохранить объединяемую пару";
            
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _currentFilePath = sfd.FileName;
                SaveToFile(_currentFilePath);
            }
        }

        private void SaveToFile(string path)
        {
            try
            {
                var data = new CombinedPairData
                {
                    Las1 = _lasInputRTB1.Text,
                    Las2 = _lasInputRTB2.Text
                };
                
                var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(path, json);
                LogSuccess($"Файл успешно сохранён: {path}");
                SetDirty(false);
            }
            catch (Exception ex)
            {
                LogError($"Ошибка сохранения файла: {ex.Message}");
            }
        }

        private void OpenProgramDataFolder(object? sender, EventArgs e)
        {
            try
            {
                AppDataManager.EnsureDirectoryExists();
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = AppDataManager.AppDataFolder,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                LogError($"Не удалось открыть папку: {ex.Message}");
            }
        }

        #endregion





        #endregion
    }
}
