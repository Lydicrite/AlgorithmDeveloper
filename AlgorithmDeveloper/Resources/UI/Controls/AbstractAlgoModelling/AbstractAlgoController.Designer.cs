namespace AlgorithmDeveloper.Resources.UI.Controls.AbstractAlgoModelling
{
    partial class AbstractAlgoController
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
            _aapMainTLP = new TableLayoutPanel();
            _viewportGB = new GroupBox();
            _viewport = new AlgorithmDeveloper.Resources.UI.Controls.Viewports.ImageViewport();
            _aapInteractionGB = new GroupBox();
            _aapMainTLP.SuspendLayout();
            _viewportGB.SuspendLayout();
            SuspendLayout();
            // 
            // _aapMainTLP
            // 
            _aapMainTLP.ColumnCount = 2;
            _aapMainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.00374F));
            _aapMainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 69.99626F));
            _aapMainTLP.Controls.Add(_viewportGB, 1, 0);
            _aapMainTLP.Controls.Add(_aapInteractionGB, 0, 0);
            _aapMainTLP.Dock = DockStyle.Fill;
            _aapMainTLP.Location = new Point(0, 0);
            _aapMainTLP.Name = "_aapMainTLP";
            _aapMainTLP.RowCount = 1;
            _aapMainTLP.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _aapMainTLP.Size = new Size(742, 437);
            _aapMainTLP.TabIndex = 0;
            // 
            // _viewportGB
            // 
            _viewportGB.Controls.Add(_viewport);
            _viewportGB.Dock = DockStyle.Fill;
            _viewportGB.ForeColor = Color.Silver;
            _viewportGB.Location = new Point(225, 3);
            _viewportGB.Name = "_viewportGB";
            _viewportGB.Size = new Size(514, 431);
            _viewportGB.TabIndex = 1;
            _viewportGB.TabStop = false;
            _viewportGB.Text = "Визуализация";
            // 
            // _viewport
            // 
            _viewport.BackColor = Color.FromArgb(48, 48, 48);
            _viewport.Dock = DockStyle.Fill;
            _viewport.FigureFont = new Font("Arial", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            _viewport.ForeColor = Color.Silver;
            _viewport.Location = new Point(3, 19);
            _viewport.Name = "_viewport";
            _viewport.Size = new Size(508, 409);
            _viewport.TabIndex = 0;
            // 
            // _aapInteractionGB
            // 
            _aapInteractionGB.Dock = DockStyle.Fill;
            _aapInteractionGB.ForeColor = Color.Silver;
            _aapInteractionGB.Location = new Point(3, 3);
            _aapInteractionGB.Name = "_aapInteractionGB";
            _aapInteractionGB.Size = new Size(216, 431);
            _aapInteractionGB.TabIndex = 0;
            _aapInteractionGB.TabStop = false;
            _aapInteractionGB.Text = "Взаимодействие";
            // 
            // AbstractAlgoController
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(31, 31, 31);
            Controls.Add(_aapMainTLP);
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(224, 224, 224);
            Name = "AbstractAlgoController";
            Size = new Size(742, 437);
            _aapMainTLP.ResumeLayout(false);
            _viewportGB.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _aapMainTLP;
        private GroupBox _viewportGB;
        private GroupBox _aapInteractionGB;
        private Viewports.ImageViewport _viewport;
    }
}
