using AlgorithmDeveloper.AlgorithmModel;
using AlgorithmDeveloper.AlgorithmModel.LAS;
using AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling
{
    public partial class AbstractAlgoWorkspace : UserControl
    {
        private TabFlowBinder? _tabBinder;

        public AbstractAlgoWorkspace()
        {
            InitializeComponent();

            _tabBinder = TabFlowBinder.Attach(_pagesFLP, _abstractAlgoWorkspaceTC);
            Disposed += (s, e) => _tabBinder?.Dispose();

            // Статический парсер используется напрямую
        }





        private void EnableCheckLASB(object sender, EventArgs e)
        {
            _tryParseLASB.Enabled = _lasInputRTB.Text != string.Empty && _lasInputRTB.Text.Contains("Yк");
        }

        private void CheckLASB_Click(object sender, EventArgs e)
        {
            ParsingAggregateException? ex = null;
            _createNewAAModelB.Enabled = LASParser.TryParse(_lasInputRTB.Text, out ex);

            if (ex != null)
            {
                _mainAlgoWorkspaceTerminal.Text = ex.Message;
            }
            else
            {
                _tryParseLASB.Enabled = false;
                _createNewAAModelB.Enabled = true;
                _lasInputRTB.Enabled = false;
                _mainAlgoWorkspaceTerminal.Text = string.Empty;
            }
        }

        private void CreateNewAAModelB_Click(object sender, EventArgs e)
        {
            int index = _abstractAlgoWorkspaceTC.TabPages
               .Cast<TabPage>()
               .ToList()
               .FindLastIndex(p => p.Text.Contains("AA"));

            TabPage newPage = new TabPage()
            {
                Text = $"AA{(index == -1 ? 0 : index)}",
                BackColor = Color.FromArgb(31, 31, 31),
                ForeColor = Color.FromArgb(214, 214, 214)
            };

            var aaControl = new AbstractAlgoController()
            {
                Location = new Point(0, 0),
                Model = LASParser.Parse(_lasInputRTB.Text)
            };

            newPage.Controls.Add(aaControl);

            _abstractAlgoWorkspaceTC.TabPages.Add(newPage);

            _abstractAlgoWorkspaceTC.SelectedIndex = _abstractAlgoWorkspaceTC.TabPages.Count - 1;

            _tryParseLASB.Enabled = false;
            _createNewAAModelB.Enabled = false;
            _lasInputRTB.Text = string.Empty;
            _lasInputRTB.Enabled = true;
            _mainAlgoWorkspaceTerminal.Text = aaControl.Model?.Information ?? "Модель абстрактного алгоритма успешно создана!";
        }

        private void _pagesFLP_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control != null && e.Control.GetType() == typeof(TabEntryControl))
            {
                var chkbox = new System.Windows.Forms.CheckBox();

                chkbox.Text = (e.Control as TabEntryControl)!.TabPage!.Text;
                chkbox.FlatStyle = FlatStyle.Popup;
                chkbox.Margin = new Padding(6, 6, 3, 3);
                chkbox.Name = $"{Text}_ChkB";
                chkbox.ForeColor = Color.DarkGray;
                chkbox.Font = new Font("Segoe UI", 9f);

                chkbox.Size = new Size(60, 19);

                _modelsToCombineFLP.Invoke((Action)(() =>
                {
                    _modelsToCombineFLP.Controls.Add(chkbox);
                }));
            }
        }
    }
}

// Yн w↑1 ↓1 X1 ↑1 X2 ↑3 w↑2 ↓2 Y2 w↑4 ↓3 Y1 X5 ↑6 w↑4 ↓4 X3 ↑3 X4 ↑2 w↑5 ↓5 Y3 w↑10 ↓6 Y4 w↑7 ↓7 X6 ↑8 w↑5 ↓8 X7 ↑9 w↑10 ↓9 Y5 w↑7 ↓10 Yк