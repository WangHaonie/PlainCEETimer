using System;
using System.Collections;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class ListViewEx : ListView
    {
        public IComparer Sorter
        {
            get => ListViewItemSorter;
            set
            {
                ListViewItemSorter = value;
                AutoAdjustColumnWidth();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        private void AutoAdjustColumnWidth()
        {
            foreach (ColumnHeader column in Columns)
            {
                column.Width = -2;
            }
        }
    }
}
