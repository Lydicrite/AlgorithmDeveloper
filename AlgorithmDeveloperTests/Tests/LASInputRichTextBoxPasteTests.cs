using AlgorithmDeveloper.UI.Elements.Controls;
using System.Collections.Specialized;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AlgorithmDeveloperTests.Tests
{
    [TestClass]
    public class LASInputRichTextBoxPasteTests
    {
        private const int WM_PASTE = 0x0302;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private static void RunInSta(Action action)
        {
            Exception? thrown = null;
            var t = new Thread(() =>
            {
                try { action(); }
                catch (Exception ex) { thrown = ex; }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            if (thrown != null) throw thrown;
        }

        private static (Form, LASInputRichTextBox) CreateHost(Font font, Color color)
        {
            var form = new Form();
            var rtb = new LASInputRichTextBox
            {
                Font = font,
                ForeColor = color,
                BorderStyle = BorderStyle.FixedSingle,
                Width = 300,
                Height = 100
            };
            form.Controls.Add(rtb);
            form.CreateControl();
            rtb.CreateControl();
            return (form, rtb);
        }

        [TestMethod]
        [DoNotParallelize]
        public void PasteTextUsesControlFontAndColor()
        {
            RunInSta(() =>
            {
                using var font = new Font("Consolas", 12f, FontStyle.Regular);
                var color = Color.DarkBlue;
                var (form, rtb) = CreateHost(font, color);
                using (form)
                using (rtb)
                {
                    rtb.Text = string.Empty;
                    rtb.SelectionStart = 0;

                    Clipboard.Clear();
                    Clipboard.SetText("Hello world", TextDataFormat.UnicodeText);

                    // Отправляем WM_PASTE, чтобы пройти через WndProc
                    SendMessage(rtb.Handle, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                    Assert.AreEqual("Hello world", rtb.Text, "Текст должен быть вставлен как Unicode.");
                    rtb.Select(0, rtb.TextLength);

                    var selFont = rtb.SelectionFont;
                    Assert.IsNotNull(selFont, "Шрифт выделения не должен быть null при единообразном стиле.");
                    Assert.AreEqual(rtb.Font.Name, selFont!.Name, "Имя шрифта вставленного текста должно совпадать с контролом.");
                    Assert.AreEqual(rtb.Font.Size, selFont.Size, "Размер шрифта вставленного текста должен совпадать с контролом.");
                    Assert.AreEqual(rtb.ForeColor, rtb.SelectionColor, "Цвет вставленного текста должен совпадать с контролом.");
                }

                Clipboard.Clear();
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void PasteRejectsImageAndLeavesContentEmpty()
        {
            RunInSta(() =>
            {
                using var font = new Font("Segoe Elements", 10f, FontStyle.Regular);
                var color = Color.Black;
                var (form, rtb) = CreateHost(font, color);
                using (form)
                using (rtb)
                {
                    rtb.Text = string.Empty;
                    rtb.SelectionStart = 0;

                    using var bmp = new Bitmap(16, 16);
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Red);
                    }

                    Clipboard.Clear();
                    Clipboard.SetImage(bmp);

                    SendMessage(rtb.Handle, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                    Assert.AreEqual(string.Empty, rtb.Text, "Вставка изображения должна быть заблокирована.");
                }

                Clipboard.Clear();
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void PasteRejectsFileDropList()
        {
            RunInSta(() =>
            {
                using var font = new Font("Segoe Elements", 10f, FontStyle.Regular);
                var color = Color.Black;
                var (form, rtb) = CreateHost(font, color);
                using (form)
                using (rtb)
                {
                    rtb.Text = string.Empty;
                    rtb.SelectionStart = 0;

                    var files = new StringCollection();
                    files.Add(Environment.GetFolderPath(Environment.SpecialFolder.System));
                    Clipboard.Clear();
                    Clipboard.SetFileDropList(files);

                    SendMessage(rtb.Handle, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                    Assert.AreEqual(string.Empty, rtb.Text, "Вставка файлов должна быть заблокирована.");
                }

                Clipboard.Clear();
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void PasteRtfWithColorResetsToControlStyle()
        {
            RunInSta(() =>
            {
                using var font = new Font("Consolas", 12f, FontStyle.Regular);
                var color = Color.DarkBlue;
                var (form, rtb) = CreateHost(font, color);
                using (form)
                using (rtb)
                {
                    rtb.Text = string.Empty;
                    rtb.SelectionStart = 0;

                    string rtf = "{\\rtf1\\ansi\\deff0{\\fonttbl{\\f0 Segoe Elements;}{\\f1 Times New Roman;}}{\\colortbl;\\red255\\green0\\blue0;}\\f1\\fs28\\cf1 Hello \\cf0 world}";
                    Clipboard.Clear();
                    Clipboard.SetData(DataFormats.Rtf, rtf);

                    SendMessage(rtb.Handle, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                    Assert.AreEqual("Hello world", rtb.Text.Replace("\r\n", " ").Trim());
                    rtb.Select(0, rtb.TextLength);
                    var selFont = rtb.SelectionFont;
                    Assert.IsNotNull(selFont, "Шрифт выделения должен быть единообразным.");
                    Assert.AreEqual(rtb.Font.Name, selFont!.Name, "Шрифт должен совпадать с контролом");
                    Assert.AreEqual(rtb.ForeColor.ToArgb(), rtb.SelectionColor.ToArgb(), "Цвет должен совпадать с контролом");
                }

                Clipboard.Clear();
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void PasteHtmlConvertsToPlainTextWithControlStyle()
        {
            RunInSta(() =>
            {
                using var font = new Font("Consolas", 12f, FontStyle.Regular);
                var color = Color.DarkBlue;
                var (form, rtb) = CreateHost(font, color);
                using (form)
                using (rtb)
                {
                    rtb.Text = string.Empty;
                    rtb.SelectionStart = 0;

                    string html = "<div style='color:red;font-family:Arial'>Hello&nbsp;<b>world</b></div>";
                    Clipboard.Clear();
                    Clipboard.SetText(html, TextDataFormat.Html);

                    SendMessage(rtb.Handle, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                    var text = rtb.Text.Replace('\u00A0', ' ').Trim();
                    Assert.AreEqual("Hello world", text, "HTML должен быть очищен до простого текста.");
                    rtb.Select(0, rtb.TextLength);
                    var selFont = rtb.SelectionFont;
                    Assert.IsNotNull(selFont);
                    Assert.AreEqual(rtb.Font.Name, selFont!.Name);
                    Assert.AreEqual(rtb.ForeColor.ToArgb(), rtb.SelectionColor.ToArgb());
                }

                Clipboard.Clear();
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void PasteUnknownFormatIsIgnored()
        {
            RunInSta(() =>
            {
                using var font = new Font("Segoe Elements", 10f, FontStyle.Regular);
                var color = Color.Black;
                var (form, rtb) = CreateHost(font, color);
                using (form)
                using (rtb)
                {
                    rtb.Text = string.Empty;
                    rtb.SelectionStart = 0;

                    var data = new DataObject();
                    data.SetData("MyCustomFormat", new byte[] { 1, 2, 3, 4 });
                    Clipboard.Clear();
                    Clipboard.SetDataObject(data, true);

                    SendMessage(rtb.Handle, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                    Assert.AreEqual(string.Empty, rtb.Text, "Неведомые форматы должны игнорироваться.");
                }

                Clipboard.Clear();
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void PastePrefersUnicodeTextWhenMultipleFormatsPresent()
        {
            RunInSta(() =>
            {
                using var font = new Font("Consolas", 11f, FontStyle.Regular);
                var color = Color.Green;
                var (form, rtb) = CreateHost(font, color);
                using (form)
                using (rtb)
                {
                    rtb.Text = string.Empty;
                    rtb.SelectionStart = 0;

                    using var bmp = new Bitmap(8, 8);
                    var data = new DataObject();
                    data.SetData(DataFormats.UnicodeText, "Plain XYZ");
                    data.SetData(DataFormats.Html, "<b>Styled</b>");
                    data.SetData(DataFormats.Bitmap, bmp);
                    Clipboard.Clear();
                    Clipboard.SetDataObject(data, true);

                    SendMessage(rtb.Handle, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                    Assert.AreEqual("Plain XYZ", rtb.Text, "При наличии нескольких форматов должен побеждать UnicodeText.");
                    rtb.Select(0, rtb.TextLength);
                    var selFont = rtb.SelectionFont;
                    Assert.IsNotNull(selFont);
                    Assert.AreEqual(rtb.Font.Name, selFont!.Name);
                    Assert.AreEqual(rtb.ForeColor.ToArgb(), rtb.SelectionColor.ToArgb());
                }

                Clipboard.Clear();
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void PasteWithEmptyClipboardDoesNothing()
        {
            RunInSta(() =>
            {
                using var font = new Font("Segoe Elements", 10f, FontStyle.Regular);
                var color = Color.Black;
                var (form, rtb) = CreateHost(font, color);
                using (form)
                using (rtb)
                {
                    rtb.Text = "ABC";
                    rtb.SelectionStart = 1;

                    Clipboard.Clear();

                    SendMessage(rtb.Handle, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                    Assert.AreEqual("ABC", rtb.Text, "Пустой буфер обмена не должен менять текст.");
                }

                Clipboard.Clear();
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void PasteReplacesSelectionWithPlainTextAndStyle()
        {
            RunInSta(() =>
            {
                using var font = new Font("Consolas", 12f, FontStyle.Regular);
                var color = Color.DarkBlue;
                var (form, rtb) = CreateHost(font, color);
                using (form)
                using (rtb)
                {
                    rtb.Text = "Start End";
                    rtb.Select(6, 3); // выделяем "End"

                    Clipboard.Clear();
                    Clipboard.SetText("Middle", TextDataFormat.UnicodeText);

                    SendMessage(rtb.Handle, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                    Assert.AreEqual("Start Middle", rtb.Text, "Вставка должна заменять выделение простым текстом.");
                    rtb.Select(0, rtb.TextLength);
                    var selFont = rtb.SelectionFont;
                    Assert.IsNotNull(selFont);
                    Assert.AreEqual(rtb.Font.Name, selFont!.Name);
                    Assert.AreEqual(rtb.ForeColor.ToArgb(), rtb.SelectionColor.ToArgb());
                }

                Clipboard.Clear();
            });
        }
    }
}
