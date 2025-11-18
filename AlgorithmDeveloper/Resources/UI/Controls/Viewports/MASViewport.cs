using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgorithmDeveloper.Resources.UI.Controls.Viewports
{
    public partial class MASViewport : UserControl
    {
        public MASViewport()
        {
            InitializeComponent();
        }

        public void FillData(string name, DataTable table)
        {
            if (_dgv == null) throw new ArgumentNullException(nameof(_dgv));
            if (_lbl == null) throw new ArgumentNullException(nameof(_lbl));
            if (table == null) throw new ArgumentNullException(nameof(table));

            _lbl.Text = name;
            _dgv.SuspendLayout();
            _dgv.DataSource = table;

            foreach (DataGridViewColumn column in _dgv.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

            _dgv.AutoResizeColumns();
            _dgv.ResumeLayout();

            // Принудительно обновляем макет перед расчетом размеров
            _dgv.PerformLayout();

            // Автоматическое масштабирование контрола под содержимое
            AutoResizeToContent();

            _dgv.Refresh();
        }

        /// <summary>
        /// Публичный метод для обновления размера контрола в соответствии с текущим содержимым
        /// </summary>
        public void UpdateSize()
        {
            AutoResizeToContent();
        }

        /// <summary>
        /// Автоматически изменяет размер контрола в соответствии с содержимым DataGridView
        /// </summary>
        private void AutoResizeToContent()
        {
            if (_dgv == null || _dgv.DataSource == null) return;

            try
            {
                // Приостанавливаем обновление для более точных расчетов
                SuspendLayout();

                // Вычисляем необходимую ширину DataGridView
                int dgvWidth = _dgv.RowHeadersVisible ? _dgv.RowHeadersWidth : 0;
                foreach (DataGridViewColumn column in _dgv.Columns)
                {
                    if (column.Visible)
                        dgvWidth += column.Width;
                }

                // Вычисляем необходимую высоту DataGridView
                int dgvHeight = _dgv.ColumnHeadersVisible ? _dgv.ColumnHeadersHeight : 0;
                foreach (DataGridViewRow row in _dgv.Rows)
                {
                    if (row.Visible && !row.IsNewRow)
                        dgvHeight += row.Height;
                }

                // Добавляем отступы для границ DataGridView только если они есть
                if (_dgv.BorderStyle == BorderStyle.Fixed3D)
                {
                    dgvWidth += 4;  // 2px с каждой стороны для Fixed3D
                    dgvHeight += 4;
                }
                else if (_dgv.BorderStyle == BorderStyle.FixedSingle)
                {
                    dgvWidth += 2;  // 1px с каждой стороны для FixedSingle
                    dgvHeight += 2;
                }

                // Устанавливаем размер DataGridView точно под содержимое
                _dgv.Size = new Size(dgvWidth, dgvHeight);

                // Вычисляем размер GroupBox на основе реального размера DataGridView
                // Учитываем позицию DataGridView внутри GroupBox и отступы
                int groupBoxWidth = _dgv.Location.X + dgvWidth + _lbl.Padding.Right;
                int groupBoxHeight = _dgv.Location.Y + dgvHeight + _lbl.Padding.Bottom;

                // Устанавливаем размер GroupBox
                _lbl.Size = new Size(groupBoxWidth, groupBoxHeight);

                // Размер всего контрола равен размеру GroupBox
                int totalWidth = groupBoxWidth;
                int totalHeight = groupBoxHeight;

                // Устанавливаем максимальные разумные размеры (можно настроить)
                const int maxWidth = 1000;
                const int maxHeight = 700;

                totalWidth = Math.Min(totalWidth, maxWidth);
                totalHeight = Math.Min(totalHeight, maxHeight);

                // Устанавливаем минимальные размеры
                totalWidth = Math.Max(totalWidth, MinimumSize.Width);
                totalHeight = Math.Max(totalHeight, MinimumSize.Height);

                // Применяем новый размер к контролу
                Size = new Size(totalWidth, totalHeight);
            }
            finally
            {
                ResumeLayout(true);
            }
        }
    }
}
