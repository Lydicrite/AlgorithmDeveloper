namespace AlgorithmDeveloper.Resources.UI.Controls.Viewports
{
    partial class ImageViewport
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
            if (disposing)
            {
                _markCts?.Cancel();
                _markCts?.Dispose();
                components?.Dispose();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private HScrollBar? _hScrollBar;
        private VScrollBar? _vScrollBar;

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // ImageViewport
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(32, 32, 32);
            Name = "ImageViewport";
            Size = new Size(496, 392);
            ResumeLayout(false);

        }
    }
}
