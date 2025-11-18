namespace AlgorithmDeveloper.Resources.UI.Forms
{
    partial class TerminalForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TerminalForm));
            _mainAlgoWorkspaceTerminal = new RichTextBox();
            toolStripButton1 = new ToolStripButton();
            toolStripButton2 = new ToolStripButton();
            toolStripButton3 = new ToolStripButton();
            _mainToolStrip = new AlgorithmDeveloper.Resources.UI.Controls.CustomizableToolStrip.CustomizableToolStrip();
            _mainToolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // _mainAlgoWorkspaceTerminal
            // 
            _mainAlgoWorkspaceTerminal.BackColor = Color.FromArgb(16, 16, 16);
            _mainAlgoWorkspaceTerminal.BorderStyle = BorderStyle.FixedSingle;
            _mainAlgoWorkspaceTerminal.Dock = DockStyle.Fill;
            _mainAlgoWorkspaceTerminal.ForeColor = Color.Silver;
            _mainAlgoWorkspaceTerminal.Location = new Point(0, 25);
            _mainAlgoWorkspaceTerminal.Margin = new Padding(4, 3, 4, 3);
            _mainAlgoWorkspaceTerminal.Name = "_mainAlgoWorkspaceTerminal";
            _mainAlgoWorkspaceTerminal.Size = new Size(664, 416);
            _mainAlgoWorkspaceTerminal.TabIndex = 6;
            _mainAlgoWorkspaceTerminal.Text = "";
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.ForeColor = Color.FromArgb(214, 214, 214);
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(23, 22);
            toolStripButton1.Text = "_btnCopy";
            // 
            // toolStripButton2
            // 
            toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton2.ForeColor = Color.FromArgb(214, 214, 214);
            toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
            toolStripButton2.ImageTransparentColor = Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new Size(23, 22);
            toolStripButton2.Text = "toolStripButton2";
            // 
            // toolStripButton3
            // 
            toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton3.ForeColor = Color.FromArgb(214, 214, 214);
            toolStripButton3.Image = (Image)resources.GetObject("toolStripButton3.Image");
            toolStripButton3.ImageTransparentColor = Color.Magenta;
            toolStripButton3.Name = "toolStripButton3";
            toolStripButton3.Size = new Size(23, 22);
            toolStripButton3.Text = "toolStripButton3";
            // 
            // _mainToolStrip
            // 
            _mainToolStrip.ArrowColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip.AutoSize = false;
            _mainToolStrip.CheckedBackColor = Color.Transparent;
            _mainToolStrip.CheckedBackColor2 = null;
            _mainToolStrip.CheckedBorderColor = Color.Transparent;
            _mainToolStrip.CheckedTextColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip.DisabledArrowColor = SystemColors.ControlDarkDark;
            _mainToolStrip.DisabledTextColor = SystemColors.ControlDarkDark;
            _mainToolStrip.DropDownBackColor = Color.FromArgb(46, 46, 46);
            _mainToolStrip.DropDownBorderColor = Color.FromArgb(66, 66, 66);
            _mainToolStrip.DropDownBorderEnabled = true;
            _mainToolStrip.DropDownCornerRadius = 0;
            _mainToolStrip.DropDownOverrideBackground = true;
            _mainToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            _mainToolStrip.HoverArrowColor = Color.FromArgb(250, 250, 250);
            _mainToolStrip.HoverBackColor = Color.FromArgb(61, 61, 61);
            _mainToolStrip.HoverBackColor2 = null;
            _mainToolStrip.HoverBorderColor = Color.FromArgb(112, 112, 112);
            _mainToolStrip.HoverTextColor = Color.FromArgb(250, 250, 250);
            _mainToolStrip.Items.AddRange(new ToolStripItem[] { toolStripButton1, toolStripButton2, toolStripButton3 });
            _mainToolStrip.Location = new Point(0, 0);
            _mainToolStrip.Margin = new Padding(3);
            _mainToolStrip.Name = "_mainToolStrip";
            _mainToolStrip.OutlineColor = Color.FromArgb(250, 250, 250);
            _mainToolStrip.OutlineCornerRadius = 0;
            _mainToolStrip.OverrideBackground = true;
            _mainToolStrip.PressedArrowColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip.PressedBackColor = Color.FromArgb(46, 46, 46);
            _mainToolStrip.PressedBackColor2 = null;
            _mainToolStrip.PressedBorderColor = Color.FromArgb(66, 66, 66);
            _mainToolStrip.PressedTextColor = Color.FromArgb(214, 214, 214);
            _mainToolStrip.SeparatorColor = Color.FromArgb(66, 66, 66);
            _mainToolStrip.SeparatorLightColor = Color.FromArgb(80, 80, 80);
            _mainToolStrip.Size = new Size(664, 25);
            _mainToolStrip.TabIndex = 5;
            _mainToolStrip.TextColor = Color.FromArgb(214, 214, 214);
            // 
            // TerminalForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(31, 31, 31);
            ClientSize = new Size(664, 441);
            Controls.Add(_mainAlgoWorkspaceTerminal);
            Controls.Add(_mainToolStrip);
            ForeColor = Color.Silver;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MaximumSize = new Size(800, 600);
            Name = "TerminalForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Терминал";
            _mainToolStrip.ResumeLayout(false);
            _mainToolStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        public RichTextBox _mainAlgoWorkspaceTerminal;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private ToolStripButton toolStripButton3;
        private Controls.CustomizableToolStrip.CustomizableToolStrip _mainToolStrip;
    }
}