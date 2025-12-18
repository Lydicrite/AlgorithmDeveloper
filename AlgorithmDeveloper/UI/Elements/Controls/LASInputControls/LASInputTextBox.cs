using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmDeveloper.UI.Elements.Controls.LASInputControls
{
    public partial class LASInputTextBox : TextBox
    {
        /// <summary>
        /// Автоматически переключать раскладку на английскую при фокусе.
        /// </summary>
        [Category("Behavior")]
        [Description("Автоматически переключать раскладку на английскую при фокусе.")]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SwitchToEnglishOnFocus { get; set; } = true;

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            LASInputHandle.HandleEnter(SwitchToEnglishOnFocus);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            LASInputHandle.HandleKeyDown(this, e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            LASInputHandle.HandleKeyPress(this, e);
        }
    }
}
