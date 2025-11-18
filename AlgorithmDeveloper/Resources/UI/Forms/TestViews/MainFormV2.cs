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
                _mainTerminal.Font = font;
            }
            _mainTerminal.WordWrap = false;

            lasGraphicKeyboard1.BindTargets(_lasInputRTB1, _lasInputRTB2);
           
            // Явно укажем активные цели, чтобы ввод начинался даже без фокуса
            lasGraphicKeyboard1.ActiveTarget = _lasInputRTB1;
        }




        #region Приватная часть

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
                    _mainTerminal.Text = ex.Message;
                }
                else
                {
                    _tryParseLAS1_B.Enabled = false;
                    _createGAS1_B.Enabled = true;

                    _lasInputRTB1.ReadOnly = true;
                    // _lasInputRTB1.Cursor = Cursors.Arrow;
                    _lasInputRTB1.BackColor = Color.FromArgb(76, 76, 76);

                    _mainTerminal.Text = string.Empty;
                }
            }

            if (sender == _tryParseLAS2_B)
            {
                _createGAS2_B.Enabled = LASParser.TryParse(_lasInputRTB2.Text, out ex);

                if (ex != null)
                {
                    _mainTerminal.Text = ex.Message;
                }
                else
                {
                    _tryParseLAS2_B.Enabled = false;
                    _createGAS2_B.Enabled = true;

                    _lasInputRTB2.ReadOnly = true;
                    // _lasInputRTB2.Cursor = Cursors.Arrow;
                    _lasInputRTB2.BackColor = Color.FromArgb(76, 76, 76);

                    _mainTerminal.Text = string.Empty;
                }
            }
        }

        private void CreageGAS(object sender, EventArgs e)
        {
            if (sender == _createGAS1_B)
            {
                _algoController1.Model = LASParser.Parse(_lasInputRTB1.Text);

                _tryParseLAS1_B.Enabled = false;
                _createGAS1_B.Enabled = false;

                _lasInputRTB1.ReadOnly = true;
                // _lasInputRTB1.Cursor = Cursors.Arrow;
                _lasInputRTB1.BackColor = Color.FromArgb(76, 76, 76);

                _mainTerminal.Text = "Модель первого алгоритма создана успешно!";

                var masVP = new MASViewport();
                masVP.FillData("МСА 1", _algoController1.Model?.MAS?.DataTable);

                _masFLP.Invoke((Action)(() =>
                {
                    _masFLP.SuspendLayout();
                    _masFLP.Controls.Add(masVP);
                    _masFLP.ResumeLayout();
                }));

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

                _mainTerminal.Text = "Модель второго алгоритма создана успешно!";

                var masVP = new MASViewport();
                masVP.FillData("МСА 2", _algoController2.Model?.MAS?.DataTable);

                _masFLP.Invoke((Action)(() =>
                {
                    _masFLP.SuspendLayout();
                    _masFLP.Controls.Add(masVP);
                    _masFLP.ResumeLayout();
                }));

                /*
                _btnRuns2.Enabled = true;
                _btnCycles2.Enabled = true;
                _btnTable2.Enabled = true;
                _btnAll2.Enabled = true;
                */
            }
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