using AlgorithmDeveloper.Resources.UI.Controls.LASInputControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmDeveloper.Resources.UI.Controls
{
    /// <summary>
    /// Кастомный контрол для ввода ЛСА, повторяющий часть правил парсера ЛСА.
    /// </summary>
    public partial class LASInputRichTextBox : RichTextBox
    {
        /// <summary>
        /// Автоматически переключать раскладку на английскую при фокусе.
        /// </summary>
        [Category("Поведение")]
        [Description("Автоматически переключать раскладку на английскую при фокусе.")]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SwitchToEnglishOnFocus { get; set; } = true;

        private Control? _attachedParent;

        public LASInputRichTextBox()
        {
            // Включаем DnD, но будем принимать только текст
            this.AllowDrop = true;
            // Отключаем встроенные шорткаты, чтобы перехватывать Ctrl+V/Shift+Insert сами
            this.ShortcutsEnabled = false;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            LASInputHandle.HandleEnter(SwitchToEnglishOnFocus);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AttachToParent(this.Parent);
            UpdateClippedRegion();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            AttachToParent(null);
            base.OnHandleDestroyed(e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            AttachToParent(this.Parent);
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





        [Category("Внешний вид")]
        [Description("Текст-подсказка, отображаемый при пустом поле.")]
        [DefaultValue("")]
        public string PlaceholderText { get; set; } = string.Empty;

        [Category("Внешний вид")]
        [Description("Цвет текста-подсказки.")]
        [DefaultValue(typeof(Color), "GrayText")]
        public Color PlaceholderColor { get; set; } = SystemColors.GrayText;

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
            this.Parent?.Invalidate(this.Bounds);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
            this.Parent?.Invalidate(this.Bounds);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
            this.Parent?.Invalidate(this.Bounds);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateClippedRegion();
            Invalidate();
            this.Parent?.Invalidate(this.Bounds);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            if (e.Data != null && (e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            if (e.Data == null) return;

            string? text = null;
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
                text = e.Data.GetData(DataFormats.UnicodeText) as string;
            else if (e.Data.GetDataPresent(DataFormats.Text))
                text = e.Data.GetData(DataFormats.Text) as string;

            if (!string.IsNullOrEmpty(text))
                InsertPlainTextWithStyle(text);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_PASTE = 0x0302;
            const int WM_PAINT = 0x000F;
        
            if (m.Msg == WM_PASTE)
            {
                HandleSafePaste();
                return;
            }
        
            base.WndProc(ref m);
        
            if (m.Msg == WM_PAINT)
            {
                DrawOverlay();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Перехватываем стандартные комбинации вставки и выполняем безопасную вставку
            if (keyData == (Keys.Control | Keys.V) || keyData == (Keys.Shift | Keys.Insert))
            {
                HandleSafePaste();
                return true; // полностью подавляем стандартную вставку RichTextBox
            }
            // Разрешаем выделение всего текста даже при ShortcutsEnabled=false
            if (keyData == (Keys.Control | Keys.A))
            {
                this.SelectAll();
                return true;
            }
            // Копирование выделенного текста как Unicode (без форматирования)
            if (keyData == (Keys.Control | Keys.C))
            {
                SafeCopySelection();
                return true;
            }
            // Вырезание: копируем Unicode и удаляем выделение
            if (keyData == (Keys.Control | Keys.X))
            {
                SafeCutSelection();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void DrawOverlay()
        {
            using (var g = this.CreateGraphics())
            {
                // Рисуем PlaceholderText
                if (!this.Focused && string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(this.PlaceholderText))
                {
                    var rect = this.ClientRectangle;
                    rect.Inflate(2, -1);
                    TextRenderer.DrawText(g, this.PlaceholderText, this.Font, rect, this.PlaceholderColor, TextFormatFlags.Left);
                }
            }
        }

        private void Parent_Paint(object? sender, PaintEventArgs e)
        {
            if (this.BorderStyle == BorderStyle.None && this.Visible && this.Width > 0 && this.Height > 0)
            {
                var rect = new Rectangle(this.Left, this.Top, this.Width, this.Height);
                ControlPaint.DrawBorder(e.Graphics, rect,
                    Color.FromArgb(100, 100, 100), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(100, 100, 100), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(100, 100, 100), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(100, 100, 100), 1, ButtonBorderStyle.Solid);
            }
        }

        private void AttachToParent(Control? parent)
        {
            if (_attachedParent != null)
            {
                _attachedParent.Paint -= Parent_Paint;
            }
            _attachedParent = parent;
            if (_attachedParent != null)
            {
                _attachedParent.Paint -= Parent_Paint; // защита от дублей
                _attachedParent.Paint += Parent_Paint;
            }
        }

        private void UpdateClippedRegion()
        {
            if (this.BorderStyle == BorderStyle.None && this.Width > 2 && this.Height > 2)
            {
                this.Region = new Region(new Rectangle(1, 1, this.Width - 2, this.Height - 2));
            }
            else
            {
                this.Region = null;
            }
        }

        private string? TryGetPlainTextFromClipboard()
        {
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                {
                    var t = Clipboard.GetText(TextDataFormat.UnicodeText);
                    if (!string.IsNullOrEmpty(t)) return t;
                }
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    var t = Clipboard.GetText(TextDataFormat.Text);
                    if (!string.IsNullOrEmpty(t)) return t;
                }
                // RTF как текст (часто доступен из Office/Word)
                if (Clipboard.ContainsText(TextDataFormat.Rtf))
                {
                    var rtf = Clipboard.GetText(TextDataFormat.Rtf);
                    if (!string.IsNullOrEmpty(rtf))
                    {
                        using var tmp = new RichTextBox();
                        try
                        {
                            tmp.Rtf = rtf;
                            var t = tmp.Text;
                            if (!string.IsNullOrEmpty(t)) return t;
                        }
                        catch { /* Если RTF некорректен — игнорируем */ }
                    }
                }
                // RTF как произвольные данные (может быть Stream)
                if (Clipboard.ContainsData(DataFormats.Rtf))
                {
                    var rtfObj = Clipboard.GetData(DataFormats.Rtf);
                    string? rtf = rtfObj as string;
                    if (string.IsNullOrEmpty(rtf) && rtfObj is System.IO.Stream s)
                    {
                        try
                        {
                            using var reader = new System.IO.StreamReader(s, Encoding.UTF8, true);
                            rtf = reader.ReadToEnd();
                        }
                        catch { }
                    }
                    if (!string.IsNullOrEmpty(rtf))
                    {
                        using var tmp = new RichTextBox();
                        try
                        {
                            tmp.Rtf = rtf;
                            var t = tmp.Text;
                            if (!string.IsNullOrEmpty(t)) return t;
                        }
                        catch { /* Если RTF некорректен — игнорируем */ }
                    }
                }
                if (Clipboard.ContainsText(TextDataFormat.Html))
                {
                    var html = Clipboard.GetText(TextDataFormat.Html);
                    if (!string.IsNullOrEmpty(html))
                    {
                        // Простейшая очистка HTML до текста
                        var t = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", string.Empty);
                        t = System.Net.WebUtility.HtmlDecode(t);
                        if (!string.IsNullOrEmpty(t)) return t;
                    }
                }
            }
            catch { }
            return null;
        }

        private void HandleSafePaste()
        {
            try
            {
                var text = TryGetPlainTextFromClipboard();
                if (!string.IsNullOrEmpty(text))
                {
                    InsertPlainTextWithStyle(text);
                }
            }
            catch
            {
                // Игнорируем ошибки доступа к буферу обмена или вставки
            }
        }

        private void InsertPlainTextWithStyle(string text)
        {
            int start = this.SelectionStart;
            int beforeLen = this.TextLength;

            // Устанавливаем стиль вставки и вставляем только простой текст
            this.SelectionFont = this.Font;
            this.SelectionColor = this.ForeColor;
            this.SelectedText = text ?? string.Empty;

            int insertedLen = Math.Max(0, this.TextLength - beforeLen);

            // Нормализуем формат именно вставленного диапазона
            if (insertedLen > 0)
            {
                this.Select(start, insertedLen);
                this.SelectionFont = this.Font;
                this.SelectionColor = this.ForeColor;

                // Если вдруг остались смешанные стили (Clipboard навязал формат), сбросим весь текст
                bool mixed = this.SelectionFont == null || this.SelectionColor.ToArgb() != this.ForeColor.ToArgb();
                int caret = start + insertedLen;

                if (mixed)
                {
                    this.Select(0, this.TextLength);
                    this.SelectionFont = this.Font;
                    this.SelectionColor = this.ForeColor;
                    this.Select(caret, 0);
                }
                else
                {
                    this.Select(caret, 0);
                }
            }
        }
        private void SafeCopySelection()
        {
            try
            {
                var text = this.SelectedText;
                if (!string.IsNullOrEmpty(text))
                {
                    Clipboard.SetText(text, TextDataFormat.UnicodeText);
                }
            }
            catch
            {
                // Игнорируем ошибки доступа к буферу обмена
            }
        }

        private void SafeCutSelection()
        {
            bool copied = false;
            try
            {
                var text = this.SelectedText;
                if (!string.IsNullOrEmpty(text))
                {
                    Clipboard.SetText(text, TextDataFormat.UnicodeText);
                    copied = true;
                }
            }
            catch
            {
                copied = false; // если не удалось скопировать, не удаляем текст
            }

            if (copied && this.SelectionLength > 0)
            {
                this.SelectedText = string.Empty;
            }
        }
    }
}
