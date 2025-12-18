namespace AlgorithmDeveloper.UI.Elements.Controls.Viewports
{
    partial class SvgViewport
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
        private Microsoft.Web.WebView2.WinForms.WebView2 _webView;
        private void InitializeComponent()
        {
            _webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            SuspendLayout();
            _webView.AllowExternalDrop = false;
            _webView.CreationProperties = null;
            _webView.DefaultBackgroundColor = System.Drawing.Color.Transparent;
            _webView.Dock = System.Windows.Forms.DockStyle.Fill;
            _webView.Name = "_webView";
            _webView.TabIndex = 0;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(31, 31, 31);
            Controls.Add(_webView);
            Name = "SvgViewport";
            Size = new System.Drawing.Size(496, 392);
            ResumeLayout(false);
        }
    }
}

