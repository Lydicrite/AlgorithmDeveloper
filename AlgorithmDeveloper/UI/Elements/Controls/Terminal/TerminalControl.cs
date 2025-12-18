using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace AlgorithmDeveloper.UI.Elements.Controls.Terminal
{
    public partial class TerminalControl : UserControl
    {
        public event EventHandler<string>? InputReceived;

        private bool _binaryInputMode = false;
        private int _binaryInputLimit = 1;
        private bool _showTimestamp = true;

        /// <summary>
        /// Если true, разрешает ввод только '0' или '1' и ограничивает длину до BinaryInputLimit.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public bool BinaryInputMode
        {
            get => _binaryInputMode;
            set
            {
                _binaryInputMode = value;
                // Запрещаем вставку из буфера обмена (контекстное меню и горячие клавиши) если включен бинарный режим
                _inputTB.ShortcutsEnabled = !_binaryInputMode;
                
                if (_binaryInputMode)
                {
                    // Проверка текущего текста на соответствие
                    if (!IsValidBinaryString(_inputTB.Text))
                    {
                        _inputTB.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Максимальное количество символов при включенном BinaryInputMode.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(1)]
        public int BinaryInputLimit
        {
            get => _binaryInputLimit;
            set
            {
                _binaryInputLimit = Math.Max(1, value);
                if (_binaryInputMode && _inputTB.Text.Length > _binaryInputLimit)
                {
                    _inputTB.Text = _inputTB.Text.Substring(0, _binaryInputLimit);
                }
            }
        }

        /// <summary>
        /// Если true, в начале каждой строки лога выводится временная метка.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        public bool ShowTimestamp
        {
            get => _showTimestamp;
            set => _showTimestamp = value;
        }

        /// <summary>
        /// Управляет доступностью и видимостью поля ввода.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InputEnabled
        {
            get => _inputPanel.Enabled;
            set
            {
                _inputPanel.Enabled = value;
                _inputPanel.Visible = value;
                // При включении автоматически пытаемся сфокусировать
                if (value)
                {
                    FocusInput();
                }
            }
        }

        public TerminalControl()
        {
            InitializeComponent();
            _inputTB.KeyDown += InputTB_KeyDown;
            _inputTB.KeyPress += InputTB_KeyPress;
            // По умолчанию поле ввода скрыто/неактивно, если не задано иное (можно изменить при необходимости)
            InputEnabled = false; 
        }

        public void Clear()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Clear));
                return;
            }
            _outputRTB.Clear();
        }

        public void AppendText(string text, Color color, int fontSize = 10, bool bold = false)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendText(text, color, fontSize, bold)));
                return;
            }

            _outputRTB.SelectionStart = _outputRTB.TextLength;
            _outputRTB.SelectionLength = 0;
            _outputRTB.SelectionColor = color;
            _outputRTB.SelectionFont = new Font(_outputRTB.Font.FontFamily, fontSize, bold ? FontStyle.Bold : FontStyle.Regular);
            _outputRTB.AppendText(text);
            _outputRTB.SelectionColor = _outputRTB.ForeColor;
            _outputRTB.ScrollToCaret();
        }

        public void LogMessage(string message, Color tsColor)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => LogMessage(message, tsColor)));
                return;
            }

            int fs = (int)Math.Round(_outputRTB.Font.Size);
            if (_outputRTB.TextLength > 0)
                AppendText("\n", _outputRTB.ForeColor, fs, false);

            if (_showTimestamp)
                AppendText($"[{DateTime.Now:dd.MM.yyyy, HH:mm}] -> ", tsColor, fs, false);
            else
                AppendText("", tsColor, fs, false);

            AppendText(message, _outputRTB.ForeColor, fs, false);
        }

        public void LogSuccess(string message)
        {
            LogMessage(message, Color.DarkSeaGreen);
        }

        public void LogError(string message)
        {
            LogMessage(message, Color.IndianRed);
        }

        public void LogInfo(string message)
        {
            LogMessage(message, _outputRTB.ForeColor);
        }

        private void InputTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Убираем звук и перевод строки
                string text = _inputTB.Text.Trim();

                if (_binaryInputMode)
                {
                    if (!IsValidBinaryString(text) || text.Length != _binaryInputLimit)
                    {
                        LogError($"Ввод должен состоять из {_binaryInputLimit} символов '0' или '1'.");
                        _inputTB.SelectAll();
                        return;
                    }
                }

                _inputTB.Clear();
                
                // Эхо в терминал
                int fs = (int)Math.Round(_outputRTB.Font.Size);
                if (_outputRTB.TextLength > 0)
                    AppendText("\n", _outputRTB.ForeColor, fs, false);
                AppendText($"> {text}", Color.Gray, fs, false);

                InputReceived?.Invoke(this, text);
            }
        }

        private void InputTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_binaryInputMode)
            {
                // Разрешаем только 0, 1 и Backspace
                if (e.KeyChar != '0' && e.KeyChar != '1' && e.KeyChar != '\b')
                {
                    e.Handled = true;
                }

                // Ограничиваем длину BinaryInputLimit символами (если не выделен текст и не Backspace)
                if (_inputTB.Text.Length >= _binaryInputLimit && e.KeyChar != '\b' && _inputTB.SelectionLength == 0)
                {
                    e.Handled = true;
                }
            }
        }

        public void FocusInput()
        {
            if (_inputTB.CanFocus)
                _inputTB.Focus();
        }

        private bool IsValidBinaryString(string s)
        {
            foreach (char c in s)
            {
                if (c != '0' && c != '1') return false;
            }
            return true;
        }
    }
}