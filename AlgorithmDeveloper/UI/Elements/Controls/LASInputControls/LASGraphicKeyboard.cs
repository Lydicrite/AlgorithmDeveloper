using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgorithmDeveloper.UI.Elements.Controls.LASInputControls
{
    public partial class LASGraphicKeyboard : UserControl
    {
        private readonly List<TextBoxBase> _boundTargets = new();
        private TextBoxBase? _activeTarget;
        private bool _autoBoundAttempted = false;

        public LASGraphicKeyboard()
        {
            InitializeComponent();
            WireButtonHandlers();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                try { LASInputHandle.SymbolApplied -= LASInputHandle_SymbolApplied; } catch { }
                UnbindAll();
            }
            base.Dispose(disposing);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            // Автоматически пытаемся привязаться к контейнеру при добавлении на форму
            if (!_autoBoundAttempted && this.Parent != null)
            {
                _autoBoundAttempted = true;
                try { BindToContainer(this.Parent); } catch { }
                // Если активная цель не указана, берём первую привязанную
                if (_activeTarget == null)
                {
                    _activeTarget = _boundTargets.FirstOrDefault();
                }
            }
        }

        [Category("Behavior")]
        [Description("Активная цель для ввода с графической клавиатуры.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextBoxBase? ActiveTarget
        {
            get => _activeTarget;
            set
            {
                _activeTarget = value;
                // При явном указании активной цели — убеждаемся, что она привязана
                if (value != null && !_boundTargets.Contains(value))
                {
                    BindTargets(value);
                }
            }
        }

        public void BindTargets(params TextBoxBase[] targets)
        {
            if (targets == null || targets.Length == 0) return;
            foreach (var t in targets)
            {
                if (t == null) continue;
                if (_boundTargets.Contains(t)) continue;
                _boundTargets.Add(t);
                t.Enter += Target_Enter;
                t.Disposed += Target_Disposed;
            }
        }

        public void BindToContainer(Control container)
        {
            if (container == null) return;
            var targets = FindTextBoxes(container).ToArray();
            BindTargets(targets);
        }

        public void UnbindAll()
        {
            foreach (var t in _boundTargets.ToList())
            {
                t.Enter -= Target_Enter;
                t.Disposed -= Target_Disposed;
            }
            _boundTargets.Clear();
            _activeTarget = null;
        }





        private IEnumerable<TextBoxBase> FindTextBoxes(Control root)
        {
            if (root is TextBoxBase tbRoot && (tbRoot is LASInputTextBox || tbRoot is AlgorithmDeveloper.UI.Elements.Controls.LASInputRichTextBox))
                yield return tbRoot;
            foreach (Control c in root.Controls)
            {
                foreach (var d in FindTextBoxes(c))
                    yield return d;
            }
        }

        private void Target_Enter(object? sender, EventArgs e)
        {
            if (sender is TextBoxBase tb)
            {
                _activeTarget = tb;
            }
        }

        private void Target_Disposed(object? sender, EventArgs e)
        {
            if (sender is TextBoxBase tb)
            {
                tb.Enter -= Target_Enter;
                tb.Disposed -= Target_Disposed;
                _boundTargets.Remove(tb);
                if (ReferenceEquals(_activeTarget, tb)) _activeTarget = null;
            }
        }

        private void WireButtonHandlers()
        {
            // digits
            _btn0.Click += (s, e) => ApplyKeyPress('0', _btn0);
            _btn1.Click += (s, e) => ApplyKeyPress('1', _btn1);
            _btn2.Click += (s, e) => ApplyKeyPress('2', _btn2);
            _btn3.Click += (s, e) => ApplyKeyPress('3', _btn3);
            _btn4.Click += (s, e) => ApplyKeyPress('4', _btn4);
            _btn5.Click += (s, e) => ApplyKeyPress('5', _btn5);
            _btn6.Click += (s, e) => ApplyKeyPress('6', _btn6);
            _btn7.Click += (s, e) => ApplyKeyPress('7', _btn7);
            _btn8.Click += (s, e) => ApplyKeyPress('8', _btn8);
            _btn9.Click += (s, e) => ApplyKeyPress('9', _btn9);
        
            // tokens
            _btnX.Click += (s, e) => ApplyKeyPress('X', _btnX);
            _btnP.Click += (s, e) => ApplyKeyPress('P', _btnP);
            _btnY.Click += (s, e) => ApplyKeyPress('Y', _btnY);
            _btnCTO.Click += (s, e) => ApplyKeyPress('↑', _btnCTO);
            _btnTP.Click += (s, e) => ApplyKeyPress('↓', _btnTP);
            _btnSpace.Click += (s, e) => ApplyKeyPress(' ', _btnSpace);
            _btnBackspace.Click += (s, e) => ApplyBackspace(_btnBackspace);
        
            // special Yн and Yк
            _btnYn.Click += (s, e) => ApplyYn(_btnYn);
            _btnYk.Click += (s, e) => ApplyYk(_btnYk);
        
            // w↑ pair
            _btnUnCTO.Click += (s, e) => ApplyWUpArrow(_btnUnCTO);
        }

        private TextBoxBase? ResolveTarget()
        {
            if (_activeTarget != null && !_activeTarget.IsDisposed) return _activeTarget;
            var focused = _boundTargets.FirstOrDefault(tb => tb.Focused);
            if (focused != null) return focused;
            return _boundTargets.FirstOrDefault();
        }

        private void ApplyKeyPress(char ch, Button sourceButton)
        {
            var target = ResolveTarget();
            if (target == null) return;
            try { target.Focus(); } catch { }
            var e = new KeyPressEventArgs(ch);
            LASInputHandle.HandleKeyPress(target, e);
            // Мигание отключено
        }

        private void ApplyBackspace(Button sourceButton)
        {
            var target = ResolveTarget();
            if (target == null) return;
            try { target.Focus(); } catch { }

            int pos = target.SelectionStart;
            string t = target.Text;

            if (pos > 0 && t.Length > 0)
            {
                // Стирание токенов "Yн " и "Yк " целиком, если курсор сразу после них
                if (pos >= 3)
                {
                    string prev3 = t.Substring(pos - 3, 3);
                    if (prev3 == "Yн " || prev3 == "Yк ")
                    {
                        target.Text = t.Remove(pos - 3, 3);
                        target.SelectionStart = pos - 3;
                        return;
                    }
                }
                // Особый случай: удаляем пару "w↑" целиком
                if (t[pos - 1] == '↑' && pos - 2 >= 0 && t[pos - 2] == 'w')
                {
                    target.Text = t.Remove(pos - 2, 2);
                    target.SelectionStart = pos - 2;
                    return;
                }
                // Обычное удаление одного символа слева
                target.Text = t.Remove(pos - 1, 1);
                target.SelectionStart = pos - 1;
                return;
            }
            // Если нечего удалять — ничего не делаем
        }

        private void ApplyYk(Button sourceButton)
        {
            var target = ResolveTarget();
            if (target == null) return;
            try { target.Focus(); } catch { }
        
            int pos = target.SelectionStart;
            string t = target.Text;
            char prev = pos > 0 ? t[pos - 1] : '\0';
        
            if (pos == 0)
            {
                // Начало строки: сразу формируем токен
                target.Text = t.Insert(pos, "Yк ");
                target.SelectionStart = pos + 3;
            }
            else if (prev == 'Y')
            {
                // Курсор стоит сразу после 'Y': добавляем маленькую 'к' и пробел, завершая токен
                target.Text = t.Insert(pos, "к ");
                target.SelectionStart = pos + 2;
            }
            else if (prev == ' ')
            {
                // После пробела допустимо вставить токен целиком
                target.Text = t.Insert(pos, "Yк ");
                target.SelectionStart = pos + 3;
            }
            else
            {
                // В середине слова: отделяем пробелом и вставляем токен целиком
                target.Text = t.Insert(pos, " Yк ");
                target.SelectionStart = pos + 4;
            }
            // Мигание отключено
        }

        private void ApplyYn(Button sourceButton)
        {
            var target = ResolveTarget();
            if (target == null) return;
            try { target.Focus(); } catch { }

            if (target.TextLength == 0)
            {
                LASInputHandle.HandleKeyPress(target, new KeyPressEventArgs('Y'));
            }
            else
            {
                LASInputHandle.HandleKeyPress(target, new KeyPressEventArgs('Н'));
            }
            // Мигание отключено
        }

        private void ApplyWUpArrow(Button sourceButton)
        {
            var target = ResolveTarget();
            if (target == null) return;
            try { target.Focus(); } catch { }
            var e = new KeyEventArgs(Keys.W);
            LASInputHandle.HandleKeyDown(target, e);
            // Мигание отключено
        }

        private void LASInputHandle_SymbolApplied(object? sender, LASInputHandle.SymbolAppliedEventArgs e)
        {
        }
    }
}
