using AlgorithmDeveloper.UI.Elements.Controls.Viewports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlgorithmDeveloper.Abstractions.AAModel;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Graph.Layout;
using AlgorithmDeveloper.Abstractions.AAModel.Vertices.Geometry;
using AlgorithmDeveloper.UI.Elements.Controls.Terminal;

namespace AlgorithmDeveloper.UI.Elements.Controls.AbstractAlgoModelling
{
    public partial class AAController : UserControl
    {
        private AbstractAutomata? _model = null;
        private CancellationTokenSource? _simulationCts;
        private bool _isSimulating = false;
        
        // Для ожидания ввода пользователя
        private TaskCompletionSource<string>? _inputTcs;

        /// <summary>
        /// Получает или задает модель алгоритма, связанного с этим контроллером.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AbstractAutomata? Model
        {
            get
            {
                return _model;
            }
            set 
            { 
                if (_model != null)
                    _model.Clear();
                _model = value;
                UpdateVisualization();
                UpdateUIState();
            }
        }

        public AAController()
        {
            InitializeComponent();
            _startWorkImitation.Click += StartSimulation_Click;
            _stopWorkImitation.Click += StopSimulation_Click;
            _btnFastStart.Click += StartFastSimulation_Click;
            _btnFastStop.Click += StopFastSimulation_Click;
            _terminal.InputReceived += Terminal_InputReceived;
            _fastTerminal.InputReceived += Terminal_InputReceived;
            _terminal.ShowTimestamp = false;
            _fastTerminal.ShowTimestamp = false;

            _interactionTabControl.Selecting += MainTabControl_Selecting;
            
            _createReportButton.Click += CreateReportButton_Click;
        }

        private void CreateReportButton_Click(object? sender, EventArgs e)
        {
            if (_model == null) return;

            _saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            _saveFileDialog.FileName = $"AAReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string info = _model.Information;
                    System.IO.File.WriteAllText(_saveFileDialog.FileName, info);
                    MessageBox.Show($"Отчёт успешно сохранён!\nФайл с отчётом: {_saveFileDialog.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MainTabControl_Selecting(object? sender, TabControlCancelEventArgs e)
        {
            if (_isSimulating)
            {
                e.Cancel = true;
            }
        }

        private TerminalControl ActiveTerminal => _interactionTabControl.SelectedTab == _interactionTabControl_FastModeTab 
            ? _fastTerminal 
            : _terminal;

        public void UpdateVisualizationSettings()
        {
            _viewport.FigureStrokeColor = VisualizationSettings.FigureStrokeColor;
            _viewport.FigureFillColor = VisualizationSettings.FigureFillColor;
            _viewport.FigureTextColor = VisualizationSettings.FigureTextColor;
            _viewport.FigureStrokeWidth = VisualizationSettings.FigureStrokeWidth;
            _viewport.FigureStrokeActiveColor = VisualizationSettings.FigureStrokeActiveColor;
            _viewport.FigureFillActiveColor = VisualizationSettings.FigureFillActiveColor;
            _viewport.BackColor = VisualizationSettings.ContainerColor;
            _viewport.Invalidate();
        }

        private bool _showJumpPoints = false;

        public void SetShowJumpPoints(bool show)
        {
            _showJumpPoints = show;
            UpdateVisualization();
        }

        /// <summary>
        /// Обновляет визуализацию модели во Viewports.
        /// </summary>
        private void UpdateVisualization()
        {
            UpdateVisualizationSettings();
            if (_model == null || _model.Vertices.Count == 0)
            {
                _viewport.Figures = null;
                return;
            }

            // Размещаем вершины для визуализации
            ArrangeVertices();

            _viewport.Figures = _model.Vertices.Where(v => _showJumpPoints || v is not JumpPoint).OfType<IFigure>();
            _viewport.FitToWindow();
        }

        /// <summary>
        /// Размещает вершины модели по координатам для визуализации.
        /// Теперь делегирует работу классу алгоритма компоновки.
        /// </summary>
        private void ArrangeVertices()
        {
            if (_model == null)
                return;

            _model.Update(_showJumpPoints);
            // Включаем нормализацию слоёв по несвязанным компонентам; стратегия по умолчанию: shortest-path
            ILayoutAlgorithm layout = new SugiyamaLayoutAlgorithm
            (
                LayeringStrategy.ShortestPath,
                normalizeComponents: true,
                VisualizationSettings.HorizontalSpacing,
                VisualizationSettings.VerticalSpacing,
                VisualizationSettings.StartX,
                VisualizationSettings.StartY
            );
            _model.ApplyLayout(layout, _showJumpPoints);
        }





        #region Лоигика моделирования

        private void UpdateUIState()
        {
            bool hasModel = _model != null && _model.Vertices.Count > 0 && _model.Start != null;
            
            _startWorkImitation.Enabled = hasModel && !_isSimulating;
            _stopWorkImitation.Enabled = _isSimulating;

            _btnFastStart.Enabled = hasModel && !_isSimulating;
            _btnFastStop.Enabled = _isSimulating;

            _createReportButton.Enabled = hasModel;

            _interactionTabControl.SetForbidEntry(_interactionTabControl_InteractiveModeTab, _isSimulating);
            _interactionTabControl.SetForbidEntry(_interactionTabControl_FastModeTab, _isSimulating);
            _interactionTabControl.SetForbidEntry(_interactionTabControl_AAInfoTab, _isSimulating);
        }

        private async void StartSimulation_Click(object? sender, EventArgs e)
        {
            if (_model == null || _model.Start == null) return;

            _isSimulating = true;
            UpdateUIState();

            var term = ActiveTerminal;
            term.Clear();
            term.LogInfo("Запуск моделирования...\n\n\n");
            _model.ResetConditions();
            
            _simulationCts = new CancellationTokenSource();
            
            try
            {
                await RunSimulationLoop(_simulationCts.Token, term);
            }
            catch (OperationCanceledException)
            {
                term.LogInfo("Моделирование остановлено пользователем.");
            }
            catch (Exception ex)
            {
                term.LogError($"Ошибка моделирования: {ex.Message}");
            }
            finally
            {
                // Задержка перед сбросом подсветки
                try
                {
                    await Task.Delay(VisualizationSettings.VisualizationDelay);
                }
                catch { }
                StopSimulation();
            }
        }

        private async void StartFastSimulation_Click(object? sender, EventArgs e)
        {
            if (_model == null || _model.Start == null) return;

            _isSimulating = true;
            UpdateUIState();

            var term = ActiveTerminal;
            term.Clear();
            term.LogInfo("Запуск моделирования...\n");
            _model.ResetConditions();
            
            _simulationCts = new CancellationTokenSource();
            var token = _simulationCts.Token;

            try
            {
                // Запрос входной строки
                var conditionals = _model.Vertices
                    .OfType<ConditionalVertex>()
                    .DistinctBy(v => v.ID)
                    .OrderBy(v => v, _model.ConditionalsBindingComparer)
                    .ToList();

                if (conditionals.Count > 0)
                {
                    string orderStr = string.Join(", ", conditionals.Select(c => c.ID));
                    term.LogInfo($"Порядок условных вершин в АА: {orderStr}");
                    term.AppendText($"\nВведите строку условий ({conditionals.Count} символа 0/1): ", Color.Cyan);
                    
                    term.InputEnabled = true;
                    // Включаем бинарный режим ввода с ограничением длины
                    term.BinaryInputMode = true;
                    term.BinaryInputLimit = conditionals.Count;

                    string inputStr = "";
                    _inputTcs = new TaskCompletionSource<string>();
                    using (token.Register(() => _inputTcs.TrySetCanceled()))
                    {
                        inputStr = await _inputTcs.Task;
                    }
                    _inputTcs = null;
                    term.InputEnabled = false;
                    // Сбрасываем режим и лимит
                    term.BinaryInputMode = false;
                    term.BinaryInputLimit = 1;

                    // Установка условий
                    try
                    {
                        _model.SetConditionsFromBinary(inputStr);
                        term.LogInfo($"Условия установлены: {inputStr}\n\n\n");
                    }
                    catch (Exception ex)
                    {
                        term.LogError($"Ошибка формата условий: {ex.Message}");
                        throw new OperationCanceledException(); // Прерываем
                    }
                }
                else
                {
                    term.LogInfo("В алгоритме нет условных вершин.\n\n\n");
                }

                await RunSimulationLoop(token, term);
            }
            catch (OperationCanceledException)
            {
                term.LogInfo("Моделирование остановлено пользователем.");
            }
            catch (Exception ex)
            {
                term.LogError($"Ошибка моделирования: {ex.Message}");
            }
            finally
            {
                try
                {
                    await Task.Delay(VisualizationSettings.VisualizationDelay);
                }
                catch { }
                StopSimulation();
            }
        }

        private void StopSimulation_Click(object? sender, EventArgs e)
        {
            _simulationCts?.Cancel();
            _inputTcs?.TrySetCanceled();
        }

        private void StopFastSimulation_Click(object? sender, EventArgs e)
        {
             _simulationCts?.Cancel();
             _inputTcs?.TrySetCanceled();
        }

        private void StopSimulation()
        {
            _isSimulating = false;
            _simulationCts = null;
            _inputTcs = null;

            // Сброс подсветки
            if (_model != null)
            {
                foreach (var v in _model.Vertices.OfType<IFigure>())
                {
                    v.IsActive = false;
                }
                _model.ResetConditions();
            }
            _viewport.Invalidate();
            
            UpdateUIState();
            ActiveTerminal.LogInfo("Моделирование завершено.");
        }

        private async Task RunSimulationLoop(CancellationToken token, TerminalControl term)
        {
            var visited = new Dictionary<IBDVertex, int>(); // Vertex -> Step index
            var path = new List<IBDVertex>();
            
            IBDVertex? current = _model!.Start;
            int step = 0;
            bool cycleDetected = false;
            IBDVertex? cycleStart = null;
            int cycleRunCount = 0;

            while (current != null)
            {
                token.ThrowIfCancellationRequested();

                // Пропускаем точки перехода (JumpPoints), переходя сразу к следующей вершине
                while (current is JumpPoint)
                {
                    token.ThrowIfCancellationRequested();
                    current = _model.GetNext(current);
                    if (current == null) break;
                }

                if (current == null) break;

                // 1.0. Ждём
                await Task.Delay(VisualizationSettings.VisualizationDelay, token);

                // 1.1. Подсвечиваем вершину
                if (current is IFigure fig)
                {
                    foreach (var v in _model.Vertices.OfType<IFigure>()) v.IsActive = false;
                    fig.IsActive = true;
                    _viewport.Invalidate();
                    
                    if (!string.IsNullOrEmpty(current.Description))
                        term.LogInfo(current.Description);
                    else
                        term.LogInfo($"Переход в вершину {current.ID}");
                }

                // Логика обнаружения цикла
                if (!cycleDetected && visited.ContainsKey(current))
                {
                    cycleDetected = true;
                    cycleStart = current;
                    cycleRunCount = 0;
                    term.LogInfo($"Обнаружен цикл! Начинается прогон цикла (2 раза)...");
                }

                if (cycleDetected && current == cycleStart)
                {
                    cycleRunCount++;
                    if (cycleRunCount > 2)
                    {
                        HighlightCycle(visited, path, cycleStart);
                        term.LogSuccess("\n\n\nЦикл пройден дважды. Симуляция остановлена.");
                        return;
                    }
                }

                if (!visited.ContainsKey(current))
                    visited[current] = path.Count;
                path.Add(current);

                // 1.2. Ждём
                await Task.Delay(VisualizationSettings.VisualizationDelay, token);

                if (current is EndVertex)
                {
                    term.LogSuccess("\n\n\nДостигнута конечная вершина (Yк).");
                    return;
                }

                // Переход
                if (current is ConditionalVertex cond)
                {
                    if (cond.Value == null)
                    {
                        term.AppendText($"\nВведите значение для условия {cond.ID} (0/1): ", Color.Cyan);
                        term.InputEnabled = true;
                        
                        // Ждем ввода
                        _inputTcs = new TaskCompletionSource<string>();
                        using (token.Register(() => _inputTcs.TrySetCanceled()))
                        {
                            string input = await _inputTcs.Task;
                            bool val = input == "1";
                            _model.SetConditionalValue(cond, val); // Устанавливаем для всех копий
                            term.LogInfo($"Условию {cond.ID} присвоено значение: {(val ? "1" : "0")}");
                        }
                        _inputTcs = null;
                        term.InputEnabled = false;
                    }
                }

                var next = _model.GetNext(current);
                current = next;
                step++;
            }
        }

        private void HighlightCycle(Dictionary<IBDVertex, int> visited, List<IBDVertex> path, IBDVertex cycleStart)
        {
            int startIndex = visited[cycleStart];
            var cycleNodes = new HashSet<IBDVertex>();
            for (int i = startIndex; i < path.Count; i++)
            {
                cycleNodes.Add(path[i]);
            }

            foreach (var v in cycleNodes.OfType<IFigure>())
            {
                v.IsActive = true;
            }
            _viewport.Invalidate();
        }

        private void Terminal_InputReceived(object? sender, string text)
        {
            _inputTcs?.TrySetResult(text);
        }

        #endregion
    }
}