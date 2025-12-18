using AlgorithmDeveloper.UI.Elements.Controls.AbstractAlgoModelling;
using AlgorithmDeveloper.UI.Elements.Controls.CustomizableTabControl;
using WinRT;

namespace AlgorithmDeveloper
{
    public partial class MainForm : Form
    {
        private TabFlowBinder? _tabBinder;

        public MainForm()
        {
            InitializeComponent();

            // Привязываем панель с элементами управления вкладками к основному таб-контролу
            _tabBinder = TabFlowBinder.Attach(_pagesFLP, _mainTabControl);
            // Освобождение ресурсов биндерa при закрытии формы
            Disposed += (s, e) => _tabBinder?.Dispose();

            // Скрываем страницу с настройками
            _mainTabControl.HideTab(_mainTabControl_SettingsPage);
        }

        private void AddNewWorkspace(object sender, EventArgs e)
        {
            string workspaceType = GetWorkspaceType();

            int index = _mainTabControl.TabPages
                .Cast<TabPage>()
                .ToList()
                .FindLastIndex(p => p.Text.Contains(workspaceType));

            TabPage newPage = new TabPage()
            {
                Text = $"{workspaceType}_W{(index == -1 ? 0 : index)}",
                BackColor = Color.FromArgb(31, 31, 31),
                ForeColor = Color.FromArgb(214, 214, 214)
            };

            if (workspaceType == "AbstAlgo")
                newPage.Controls.Add(new AAWorkspace()
                {
                    Location = new Point(3, 5),
                });

            _mainTabControl.TabPages.Add(newPage);

            _mainTabControl.SelectedIndex = _mainTabControl.TabPages.Count - 1;

        }

        private void EnableAddNWB(object sender, EventArgs e)
        {
            _createWorkspacePageB.Enabled = _workspaceModelTypeCB.SelectedIndex != -1;
        }





        #region Приватная часть

        private string GetWorkspaceType()
        {
            switch (_workspaceModelTypeCB.SelectedIndex)
            {
                case 0:
                    return "AbstAlgo";
                case 1:
                    return "FSMealyM";
                case 2:
                    return "FSMooreM";
                default:
                    return "_invalid";
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            using (var font = new Font("Consolas", 10f))
            {
                _mainTerminal.Font = font;
            }

            _mainTerminal.WordWrap = false;
        }


        #endregion
    }
}
