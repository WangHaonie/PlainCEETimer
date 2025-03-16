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

                foreach (ColumnHeader column in Columns)
                {
                    column.Width = -2;
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
            base.OnHandleCreated(e);
        }
    }
}
