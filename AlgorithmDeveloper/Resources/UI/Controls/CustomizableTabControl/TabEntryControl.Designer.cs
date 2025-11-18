using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AlgorithmDeveloper.Resources.UI.Controls.CustomizableTabControl
{
    public partial class TabEntryControl
    {
        private IContainer? components = null;

        /// <summary>
        /// Освобождение всех используемых ресурсов.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Метод, требуемый для поддержки конструктора — не изменяйте
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            _textBox = new TextBox();
            _btnToggle = new Button();
            _btnClose = new Button();
            SuspendLayout();
            // 
            // _textBox
            // 
            _textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _textBox.BackColor = Color.FromArgb(48, 48, 48);
            _textBox.BorderStyle = BorderStyle.FixedSingle;
            _textBox.ForeColor = Color.FromArgb(224, 224, 224);
            _textBox.Location = new Point(3, 2);
            _textBox.Margin = new Padding(3, 2, 3, 3);
            _textBox.Name = "_textBox";
            _textBox.PlaceholderText = "Имя вкладки";
            _textBox.ReadOnly = true;
            _textBox.ShortcutsEnabled = false;
            _textBox.Size = new Size(123, 23);
            _textBox.TabIndex = 3;
            _textBox.TextAlign = HorizontalAlignment.Center;
            _textBox.WordWrap = false;
            // 
            // _btnToggle
            // 
            _btnToggle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnToggle.BackColor = Color.FromArgb(31, 31, 31);
            _btnToggle.BackgroundImage = Properties.Resources.ButtonHide;
            _btnToggle.BackgroundImageLayout = ImageLayout.Center;
            _btnToggle.Cursor = Cursors.Hand;
            _btnToggle.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _btnToggle.FlatStyle = FlatStyle.Flat;
            _btnToggle.Location = new Point(132, 2);
            _btnToggle.Margin = new Padding(3, 2, 3, 3);
            _btnToggle.MaximumSize = new Size(23, 23);
            _btnToggle.MinimumSize = new Size(23, 23);
            _btnToggle.Name = "_btnToggle";
            _btnToggle.Size = new Size(23, 23);
            _btnToggle.TabIndex = 4;
            _btnToggle.Text = "\r\n";
            _btnToggle.UseVisualStyleBackColor = false;
            _btnToggle.Click += SafeToggle;
            // 
            // _btnClose
            // 
            _btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnClose.BackColor = Color.FromArgb(31, 31, 31);
            _btnClose.BackgroundImage = Properties.Resources.ButtonClose;
            _btnClose.BackgroundImageLayout = ImageLayout.Center;
            _btnClose.Cursor = Cursors.Hand;
            _btnClose.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.Location = new Point(161, 2);
            _btnClose.Margin = new Padding(3, 2, 3, 3);
            _btnClose.MaximumSize = new Size(23, 23);
            _btnClose.MinimumSize = new Size(23, 23);
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new Size(23, 23);
            _btnClose.TabIndex = 5;
            _btnClose.UseVisualStyleBackColor = false;
            _btnClose.Click += SafeClose;
            // 
            // TabEntryControl
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(31, 31, 31);
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(_textBox);
            Controls.Add(_btnToggle);
            Controls.Add(_btnClose);
            ForeColor = Color.FromArgb(224, 224, 224);
            Margin = new Padding(3, 1, 22, 3);
            MaximumSize = new Size(0, 29);
            MinimumSize = new Size(189, 29);
            Name = "TabEntryControl";
            Size = new Size(189, 27);
            ResumeLayout(false);
            PerformLayout();
        }
        private TextBox _textBox;
        private Button _btnToggle;
        private Button _btnClose;
    }
}