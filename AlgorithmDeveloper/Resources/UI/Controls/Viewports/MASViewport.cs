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
        private ImageList? _rowImageList;
        public MASViewport()
        {
            InitializeComponent();
            _rowImageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(1, TextRenderer.MeasureText("X", Font).Height + 6)
            };
            _rowImageList.Images.Add(new Bitmap(1, 1));
            _lv.SmallImageList = _rowImageList;
        }

        public void FillData(string name, DataTable table)
        {
            if (_lv == null) throw new ArgumentNullException(nameof(_lv));
            if (_lbl == null) throw new ArgumentNullException(nameof(_lbl));
            if (table == null) throw new ArgumentNullException(nameof(table));

            _lbl.Text = name;
            _lv.BeginUpdate();
            _lv.Items.Clear();
            _lv.Columns.Clear();

            for (int ci = 0; ci < table.Columns.Count; ci++)
            {
                var col = table.Columns[ci];
                var header = new ColumnHeader
                {
                    Text = col.ColumnName,
                    TextAlign = ci == 0 ? HorizontalAlignment.Left : HorizontalAlignment.Center
                };
                _lv.Columns.Add(header);
            }

            foreach (DataRow row in table.Rows)
            {
                var item = new ListViewItem(Convert.ToString(row[0]) ?? string.Empty);
                item.ImageIndex = 0;
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    item.SubItems.Add(Convert.ToString(row[i]) ?? string.Empty);
                }
                _lv.Items.Add(item);
            }
            AutoSizeColumnsByText(table);
            AdjustRowHeightByContent(table);
            _lv.EndUpdate();

            AutoResizeToContent();
            _lv.Refresh();
        }

        /// <summary>
        /// Публичный метод для обновления размера контрола в соответствии с текущим содержимым
        /// </summary>
        public void UpdateSize()
        {
            AutoResizeToContent();
        }

        private void AutoResizeToContent()
        {
            if (_lv == null || _lv.Columns.Count == 0) return;

            try
            {
                SuspendLayout();

                int lvWidth = 0;
                foreach (ColumnHeader column in _lv.Columns)
                {
                    lvWidth += column.Width;
                }

                int headerHeight = _lv.Font.Height + 8;
                int itemHeight = _lv.Items.Count > 0 ? _lv.GetItemRect(0).Height : (_lv.Font.Height + 6);
                int lvHeight = headerHeight + (_lv.Items.Count * itemHeight);

                if (_lv.BorderStyle == BorderStyle.Fixed3D)
                {
                    lvWidth += 4;
                    lvHeight += 4;
                }
                else if (_lv.BorderStyle == BorderStyle.FixedSingle)
                {
                    lvWidth += 2;
                    lvHeight += 2;
                }

                _lv.Size = new Size(lvWidth, lvHeight);

                int groupBoxWidth = _lv.Location.X + lvWidth + _lbl.Padding.Right;
                int groupBoxHeight = _lv.Location.Y + lvHeight + _lbl.Padding.Bottom;

                _lbl.Size = new Size(groupBoxWidth, groupBoxHeight);

                int totalWidth = groupBoxWidth;
                int totalHeight = groupBoxHeight;

                const int maxWidth = 1000;
                const int maxHeight = 700;

                totalWidth = Math.Min(totalWidth, maxWidth);
                totalHeight = Math.Min(totalHeight, maxHeight);

                totalWidth = Math.Max(totalWidth, MinimumSize.Width);
                totalHeight = Math.Max(totalHeight, MinimumSize.Height);

                Size = new Size(totalWidth, totalHeight);
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        private void AutoSizeColumnsByText(DataTable table)
        {
            if (_lv.Columns.Count == 0) return;
            using var g = CreateGraphics();
            int padding = 18;
            int maxTotalWidth = 1600;
            for (int ci = 0; ci < _lv.Columns.Count; ci++)
            {
                int maxWidth = TextRenderer.MeasureText(_lv.Columns[ci].Text, _lv.Font).Width + padding;
                for (int ri = 0; ri < table.Rows.Count; ri++)
                {
                    string text = Convert.ToString(table.Rows[ri][ci]) ?? string.Empty;
                    int w = TextRenderer.MeasureText(g, text, _lv.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width + padding;
                    if (w > maxWidth) maxWidth = w;
                }
                _lv.Columns[ci].Width = maxWidth;
            }
            int total = _lv.Columns.Cast<ColumnHeader>().Sum(c => c.Width);
            if (total > maxTotalWidth)
            {
                float scale = (float)maxTotalWidth / total;
                for (int ci = 0; ci < _lv.Columns.Count; ci++)
                {
                    _lv.Columns[ci].Width = Math.Max(60, (int)(_lv.Columns[ci].Width * scale));
                }
            }
        }

        private void AdjustRowHeightByContent(DataTable table)
        {
            if (_rowImageList == null || _lv.Columns.Count == 0) return;
            int baseLine = TextRenderer.MeasureText("X", _lv.Font).Height + 4;
            int maxLines = 1;
            for (int ri = 0; ri < table.Rows.Count; ri++)
            {
                int rowLines = 1;
                for (int ci = 0; ci < _lv.Columns.Count; ci++)
                {
                    string text = Convert.ToString(table.Rows[ri][ci]) ?? string.Empty;
                    int colWidth = _lv.Columns[ci].Width - 8;
                    if (colWidth < 20) colWidth = 20;
                    var size = TextRenderer.MeasureText(text, _lv.Font, new Size(colWidth, int.MaxValue), TextFormatFlags.WordBreak);
                    int lines = Math.Max(1, (int)Math.Ceiling((double)size.Height / baseLine));
                    if (lines > rowLines) rowLines = lines;
                }
                if (rowLines > maxLines) maxLines = rowLines;
            }
            int newHeight = Math.Min(300, baseLine * maxLines + 2);
            _rowImageList.ImageSize = new Size(1, newHeight);
            if (_rowImageList.Images.Count == 0) _rowImageList.Images.Add(new Bitmap(1, 1));
            _lv.SmallImageList = _rowImageList;
            foreach (ListViewItem item in _lv.Items) item.ImageIndex = 0;
        }

        private void _lv_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            using var back = new SolidBrush(Color.FromArgb(46, 46, 46));
            using var gridPen = new Pen(Color.FromArgb(240, 240, 240));
            using var borderPen = new Pen(Color.FromArgb(240, 240, 240));
            var rect = e.Bounds;
            e.Graphics.FillRectangle(back, rect);
            e.Graphics.DrawRectangle(gridPen, rect);

            var textFlags = TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.LeftAndRightPadding;
            if (e.ColumnIndex == 0)
                textFlags |= TextFormatFlags.Left;
            else
                textFlags |= TextFormatFlags.HorizontalCenter;

            TextRenderer.DrawText(e.Graphics, e.Header.Text, _lv.Font, new Rectangle(rect.X, rect.Y + 2, rect.Width, rect.Height), Color.Gainsboro, textFlags);
            if (e.ColumnIndex == 0)
            {
                e.Graphics.DrawLine(borderPen, 0, rect.Top, 0, rect.Bottom);
            }
            if (e.ColumnIndex == _lv.Columns.Count - 1)
            {
                int x = _lv.ClientSize.Width - 1;
                e.Graphics.DrawLine(borderPen, x, rect.Top, x, rect.Bottom);
            }
        }

        private void _lv_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            // Подробная отрисовка выполняется в DrawSubItem
        }

        private void _lv_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            var rect = e.Bounds;
            var isSelected = e.Item.Selected;
            var darkHeader = Color.FromArgb(46, 46, 46);
            using var back = new SolidBrush(e.ColumnIndex == 0 ? darkHeader : (isSelected ? SystemColors.Highlight : _lv.BackColor));
            e.Graphics.FillRectangle(back, rect);

            var flags = TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.LeftAndRightPadding;
            if (e.ColumnIndex == 0)
                flags |= TextFormatFlags.Left;
            else
                flags |= TextFormatFlags.HorizontalCenter;
            var textColor = e.ColumnIndex == 0 ? Color.Gainsboro : (isSelected ? SystemColors.HighlightText : _lv.ForeColor);
            TextRenderer.DrawText(e.Graphics, e.SubItem.Text ?? string.Empty, _lv.Font, new Rectangle(rect.X, rect.Y + 2, rect.Width, rect.Height), textColor, flags);

            using var borderPen = new Pen(Color.FromArgb(240, 240, 240));

            if (e.ColumnIndex == 0)
            {
                e.Graphics.DrawLine(borderPen, 0, rect.Top, 0, rect.Bottom);
            }
            if (e.ColumnIndex > 0 && e.ColumnIndex < _lv.Columns.Count - 1)
            {
                e.Graphics.DrawLine(borderPen, 0, rect.Top, 0, rect.Bottom);
            }
            if (e.ColumnIndex == _lv.Columns.Count - 1)
            {
                int x = _lv.ClientSize.Width - 1;
                e.Graphics.DrawLine(borderPen, x, rect.Top, x, rect.Bottom);
            }

            if (e.ItemIndex == 0)
                e.Graphics.DrawRectangle(borderPen, rect);
            else
                e.Graphics.DrawRectangle(borderPen, new Rectangle(rect.X, rect.Y - 1, rect.Width, rect.Height));
        }

        private void _lv_ColumnWidthChanging(object? sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = _lv.Columns[e.ColumnIndex].Width;
        }
    }
}

// Yн P0 ↑1 Y1 w↑2 ↓1 Y3 w↑3 ↓2 Y2 X1 ↑2 w↑3 ↓3 X0 ↑4 Y5 w↑5 ↓4 Y4 w↑5 ↓5 Yк
// e.Graphics.DrawRectangle(borderPen, new RectangleF(rect.X, rect.Y - 1, rect.Width, rect.Height));