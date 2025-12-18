
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AlgorithmDeveloper.UI.Elements.Controls.Terminal
{
    partial class TerminalControl
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
            _outputRTB = new RichTextBox();
            _inputPanel = new Panel();
            _inputGB = new GroupBox();
            _inputTB = new TextBox();
            _inputPanel.SuspendLayout();
            _inputGB.SuspendLayout();
            SuspendLayout();
            // 
            // _outputRTB
            // 
            _outputRTB.BackColor = Color.FromArgb(31, 31, 31);
            _outputRTB.Dock = DockStyle.Fill;
            _outputRTB.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _outputRTB.ForeColor = Color.FromArgb(224, 224, 224);
            _outputRTB.Location = new Point(0, 0);
            _outputRTB.Name = "_outputRTB";
            _outputRTB.ReadOnly = true;
            _outputRTB.ScrollBars = RichTextBoxScrollBars.Vertical;
            _outputRTB.Size = new Size(400, 242);
            _outputRTB.TabIndex = 0;
            _outputRTB.Text = "";
            // 
            // _inputPanel
            // 
            _inputPanel.BackColor = Color.FromArgb(45, 45, 48);
            _inputPanel.BorderStyle = BorderStyle.Fixed3D;
            _inputPanel.Controls.Add(_inputGB);
            _inputPanel.Dock = DockStyle.Bottom;
            _inputPanel.Location = new Point(0, 242);
            _inputPanel.Name = "_inputPanel";
            _inputPanel.Padding = new Padding(5);
            _inputPanel.Size = new Size(400, 58);
            _inputPanel.TabIndex = 1;
            // 
            // _inputGB
            // 
            _inputGB.Controls.Add(_inputTB);
            _inputGB.Dock = DockStyle.Fill;
            _inputGB.ForeColor = Color.FromArgb(224, 224, 224);
            _inputGB.Location = new Point(5, 5);
            _inputGB.Name = "_inputGB";
            _inputGB.Padding = new Padding(10, 3, 10, 3);
            _inputGB.Size = new Size(386, 44);
            _inputGB.TabIndex = 0;
            _inputGB.TabStop = false;
            _inputGB.Text = "Поле для ввода значений";
            // 
            // _inputTB
            // 
            _inputTB.BackColor = Color.FromArgb(31, 31, 31);
            _inputTB.BorderStyle = BorderStyle.None;
            _inputTB.Dock = DockStyle.Fill;
            _inputTB.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _inputTB.ForeColor = Color.FromArgb(224, 224, 224);
            _inputTB.Location = new Point(10, 19);
            _inputTB.Margin = new Padding(10, 3, 10, 3);
            _inputTB.Name = "_inputTB";
            _inputTB.Size = new Size(366, 15);
            _inputTB.TabIndex = 1;
            // 
            // TerminalControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(31, 31, 31);
            Controls.Add(_outputRTB);
            Controls.Add(_inputPanel);
            Name = "TerminalControl";
            Size = new Size(400, 300);
            _inputPanel.ResumeLayout(false);
            _inputGB.ResumeLayout(false);
            _inputGB.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox _outputRTB;
        private System.Windows.Forms.Panel _inputPanel;
        private GroupBox _inputGB;
        private TextBox _inputTB;
    }
}
