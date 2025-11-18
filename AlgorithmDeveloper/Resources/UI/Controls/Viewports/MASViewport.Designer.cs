namespace AlgorithmDeveloper.Resources.UI.Controls.Viewports
{
    partial class MASViewport
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            _dgv = new DataGridView();
            _lbl = new Label();
            ((System.ComponentModel.ISupportInitialize)_dgv).BeginInit();
            SuspendLayout();
            // 
            // _dgv
            // 
            _dgv.AllowUserToAddRows = false;
            _dgv.AllowUserToDeleteRows = false;
            _dgv.AllowUserToResizeColumns = false;
            _dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(31, 31, 31);
            dataGridViewCellStyle1.Font = new Font("Cascadia Code", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(230, 230, 230);
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(62, 62, 62);
            dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(250, 250, 250);
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            _dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            _dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            _dgv.BackgroundColor = Color.FromArgb(31, 31, 31);
            _dgv.BorderStyle = BorderStyle.None;
            _dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(31, 31, 31);
            dataGridViewCellStyle2.Font = new Font("Cascadia Code", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(230, 230, 230);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(62, 62, 62);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(250, 250, 250);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            _dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            _dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(31, 31, 31);
            dataGridViewCellStyle3.Font = new Font("Cascadia Code", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(224, 224, 224);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(62, 62, 62);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(250, 250, 250);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            _dgv.DefaultCellStyle = dataGridViewCellStyle3;
            _dgv.GridColor = Color.FromArgb(64, 64, 64);
            _dgv.Location = new Point(3, 20);
            _dgv.Name = "_dgv";
            _dgv.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(31, 31, 31);
            dataGridViewCellStyle4.Font = new Font("Cascadia Code", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            dataGridViewCellStyle4.ForeColor = Color.FromArgb(230, 230, 230);
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(62, 62, 62);
            dataGridViewCellStyle4.SelectionForeColor = Color.FromArgb(250, 250, 250);
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            _dgv.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            _dgv.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;
            _dgv.RowTemplate.DefaultCellStyle.BackColor = Color.FromArgb(31, 31, 31);
            _dgv.RowTemplate.DefaultCellStyle.Font = new Font("Cascadia Code", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _dgv.RowTemplate.DefaultCellStyle.ForeColor = Color.FromArgb(230, 230, 230);
            _dgv.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.FromArgb(62, 62, 62);
            _dgv.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.FromArgb(250, 250, 250);
            _dgv.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            _dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            _dgv.Size = new Size(160, 78);
            _dgv.TabIndex = 2;
            // 
            // _lbl
            // 
            _lbl.AutoSize = true;
            _lbl.Font = new Font("Cascadia Code", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            _lbl.Location = new Point(3, 0);
            _lbl.Name = "_lbl";
            _lbl.Size = new Size(48, 17);
            _lbl.TabIndex = 3;
            _lbl.Text = "_name";
            // 
            // MASViewport
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoSize = true;
            BackColor = Color.FromArgb(31, 31, 31);
            Controls.Add(_lbl);
            Controls.Add(_dgv);
            ForeColor = Color.FromArgb(224, 224, 224);
            MinimumSize = new Size(150, 100);
            Name = "MASViewport";
            Size = new Size(166, 101);
            ((System.ComponentModel.ISupportInitialize)_dgv).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView _dgv;
        private Label _lbl;
    }
}
