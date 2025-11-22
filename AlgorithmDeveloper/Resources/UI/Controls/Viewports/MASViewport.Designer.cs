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
            _lv = new ListView();
            _lbl = new Label();
            SuspendLayout();
            // 
            // _lv
            // 
            _lv.BackColor = Color.FromArgb(31, 31, 31);
            _lv.BorderStyle = BorderStyle.None;
            _lv.Font = new Font("Cascadia Code", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _lv.ForeColor = Color.FromArgb(230, 230, 230);
            _lv.GridLines = true;
            _lv.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _lv.HideSelection = true;
            _lv.Location = new Point(3, 20);
            _lv.MultiSelect = false;
            _lv.Name = "_lv";
            _lv.OwnerDraw = true;
            _lv.Size = new Size(160, 78);
            _lv.TabIndex = 2;
            _lv.UseCompatibleStateImageBehavior = false;
            _lv.View = View.Details;
            _lv.ColumnWidthChanging += _lv_ColumnWidthChanging;
            _lv.DrawColumnHeader += _lv_DrawColumnHeader;
            _lv.DrawItem += _lv_DrawItem;
            _lv.DrawSubItem += _lv_DrawSubItem;
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
            Controls.Add(_lv);
            ForeColor = Color.FromArgb(224, 224, 224);
            Margin = new Padding(3, 3, 3, 10);
            MinimumSize = new Size(150, 100);
            Name = "MASViewport";
            Size = new Size(166, 101);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView _lv;
        private Label _lbl;
    }
}
