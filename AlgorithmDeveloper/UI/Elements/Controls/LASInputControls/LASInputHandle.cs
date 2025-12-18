using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmDeveloper.UI.Elements.Controls.LASInputControls
{
    internal static class LASInputHandle
    {
        public static void HandleEnter(bool switchToEnglishOnFocus)
        {
            if (!switchToEnglishOnFocus) return;
            try
            {
                var ci = new CultureInfo("en-US");
                InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(ci);
            }
            catch
            {
                
            }
        }

        public static void HandleKeyDown(TextBoxBase control, KeyEventArgs e)
        {
            int pos = control.SelectionStart;
            string t = control.Text;
    
            // Удаление выделенного текста по Backspace/Delete
            if ((e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete) && control.SelectionLength > 0)
            {
                int start = control.SelectionStart;
                int len = control.SelectionLength;
                control.Text = t.Remove(start, len);
                control.SelectionStart = start;
                e.Handled = true;
                OnSymbolApplied(control, "<");
                return;
            }
    
            if (e.KeyCode == Keys.Up)
            {
                if (t.Length != 0 && pos > 0 && t[pos - 1] == ' ')
                {
                    InsertSymbol(control, "↑");
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (t.Length != 0 && pos > 0 && t[pos - 1] == ' ')
                {
                    InsertSymbol(control, "↓");
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.W && !e.Control && !e.Alt)
            {
                if (pos == 0)
                {
                    e.Handled = true;
                    return;
                }
    
                if (IsPrevTokenWArrow(t, pos))
                {
                    e.Handled = true;
                    return;
                }
    
                if (pos > 0 && t[pos - 1] != ' ')
                {
                    e.Handled = true;
                    return;
                }
    
                char next = pos < t.Length ? t[pos] : '\0';
                if (next == '↑')
                {
                    e.Handled = true;
                    return;
                }
    
                InsertSymbol(control, "w↑");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (pos > 0 && t.Length > 0)
                {
                    // Удаление пары "w↑" целиком
                    if (t[pos - 1] == '↑' && pos - 2 >= 0 && t[pos - 2] == 'w')
                    {
                        control.Text = t.Remove(pos - 2, 2);
                        control.SelectionStart = pos - 2;
                        e.Handled = true;
                        OnSymbolApplied(control, "<");
                        return;
                    }
                    // Удаление токена "Yн " целиком
                    if (pos >= 3 && t.Substring(pos - 3, 3) == "Yн ")
                    {
                        control.Text = t.Remove(pos - 3, 3);
                        control.SelectionStart = pos - 3;
                        e.Handled = true;
                        OnSymbolApplied(control, "<");
                        return;
                    }
                    // Удаление токена "Yк " целиком
                    if (pos >= 3 && t.Substring(pos - 3, 3) == "Yк ")
                    {
                        control.Text = t.Remove(pos - 3, 3);
                        control.SelectionStart = pos - 3;
                        e.Handled = true;
                        OnSymbolApplied(control, "<");
                        return;
                    }
                    // Обычное удаление одного символа слева
                    control.Text = t.Remove(pos - 1, 1);
                    control.SelectionStart = pos - 1;
                    e.Handled = true;
                    OnSymbolApplied(control, "<");
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                // При отсутствии выделения удаляем символ справа
                if (pos < t.Length)
                {
                    control.Text = t.Remove(pos, 1);
                    control.SelectionStart = pos;
                    e.Handled = true;
                    OnSymbolApplied(control, "<");
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
            }
        }

        public static void HandleKeyPress(TextBoxBase control, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;
            int pos = control.SelectionStart;
            string t = control.Text;

            if (char.IsControl(keyChar))
            {
                return;
            }

            if (!IsValidLASSymbol(keyChar))
            {
                e.Handled = true;
                return;
            }

            string symbol = keyChar.ToString().ToUpper();

            if (t.Length == 0)
            {
                if (symbol == "Y")
                {
                    InsertSymbol(control, "Yн ");
                    OnSymbolApplied(control, "Yн ");
                }
                e.Handled = true;
                return;
            }

            if (symbol == "Н")
            {
                symbol = symbol.ToLower();
                bool startsWithYn = t.StartsWith("Yн ");
                char prev = pos > 0 ? t[pos - 1] : '\0';
                bool containsSmallN = t.IndexOf('н') >= 0;
                if (t.Length != 0 && !startsWithYn && prev == 'Y' && !containsSmallN)
                {
                    InsertSymbol(control, symbol);
                    OnSymbolApplied(control, symbol);
                }
                e.Handled = true;
                return;
            }

            if (symbol == "К")
            {
                symbol = symbol.ToLower();
                char prev = pos > 0 ? t[pos - 1] : '\0';
                bool containsSmallK = t.IndexOf('к') >= 0;
                if (t.Length != 0 && prev == 'Y' && !containsSmallK)
                {
                    InsertSymbol(control, symbol);
                    OnSymbolApplied(control, symbol);
                }
                e.Handled = true;
                return;
            }

            if (symbol == "X" || symbol == "Y" || symbol == "P")
            {
                char prev = pos > 0 ? t[pos - 1] : '\0';
                if (prev == ' ')
                {
                    InsertSymbol(control, symbol);
                    OnSymbolApplied(control, symbol);
                }
                e.Handled = true;
                return;
            }

            if (char.IsDigit(keyChar))
            {
                char prevChar = pos > 0 ? t[pos - 1] : '\0';
                if (!(prevChar == '↑' || prevChar == '↓' || prevChar == 'X' || prevChar == 'Y' || prevChar == 'P' || char.IsDigit(prevChar)))
                {
                    e.Handled = true;
                    return;
                }
                
                InsertSymbol(control, $"{keyChar}");
                OnSymbolApplied(control, $"{keyChar}");
                e.Handled = true;
                return;
            }

            if (char.IsWhiteSpace(keyChar))
            {
                if (pos > 0 && t[pos - 1] != ' ')
                {
                    InsertSymbol(control, " ");
                    OnSymbolApplied(control, " ");
                }
                e.Handled = true;
                return;
            }

            if (symbol == "↑")
            {
                char prev = pos > 0 ? t[pos - 1] : '\0';
                if (t.Length != 0 && prev == ' ')
                {
                    InsertSymbol(control, "↑");
                    OnSymbolApplied(control, "↑");
                }
                e.Handled = true;
                return;
            }

            if (symbol == "↓")
            {
                char prev = pos > 0 ? t[pos - 1] : '\0';
                if (t.Length != 0 && prev == ' ')
                {
                    InsertSymbol(control, "↓");
                    OnSymbolApplied(control, "↓");
                }
                e.Handled = true;
                return;
            }

            if (symbol != "↑")
            {
                InsertSymbol(control, symbol);
                OnSymbolApplied(control, symbol);
            }
            e.Handled = true;
        }

        private static void InsertSymbol(TextBoxBase control, string symbol)
        {
            int pos = control.SelectionStart;
            string t = control.Text;
            control.Text = t.Insert(pos, symbol);
            control.SelectionStart = pos + symbol.Length;
        }

        private static bool IsValidLASSymbol(char c)
        {
            string up = c.ToString().ToUpper();
            string low = c.ToString().ToLower();
            return
                up == "Y" ||
                low == "н" || low == "к" ||
                up == "X" || up == "P" ||
                c == ' ' || c == '(' || c == ')' || c == '|' ||
                char.IsDigit(c) || c == '↑' || c == '↓';
        }

        private static bool IsPrevTokenWArrow(string t, int pos)
        {
            int i = pos - 1;
            while (i >= 0 && t[i] == ' ') i--;
            while (i >= 0 && char.IsDigit(t[i])) i--;
            if (i >= 1 && t[i] == '↑' && t[i - 1] == 'w') return true;
            if (pos >= 2 && t[pos - 2] == 'w' && t[pos - 1] == '↑') return true;
            return false;
        }
        public static event EventHandler<SymbolAppliedEventArgs>? SymbolApplied;
        
        public sealed class SymbolAppliedEventArgs : EventArgs
        {
            public TextBoxBase Control { get; }
            public string Symbol { get; }
            public SymbolAppliedEventArgs(TextBoxBase control, string symbol)
            {
                Control = control;
                Symbol = symbol;
            }
        }
        
        private static void OnSymbolApplied(TextBoxBase control, string symbol)
        {
            try { SymbolApplied?.Invoke(control, new SymbolAppliedEventArgs(control, symbol)); } catch { }
        }
    }
}
