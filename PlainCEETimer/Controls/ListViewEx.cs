using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class ListViewEx : ListView
    {
        public int ItemsCount => Items.Count;

        public ListViewItem SelectedItem => SelectedItems[0];

        public int SelectedItemsCount => SelectedItems.Count;

        public void RemoveSelectedItems()
        {
            foreach (ListViewItem Item in SelectedItems)
            {
                Items.Remove(Item);
            }
        }

        public void Suspend(Action Method)
        {
            BeginUpdate();
            Method();
            EndUpdate();
        }

        public void ClearAll()
        {
            Items.Clear();
        }

        public void AutoAdjustColumnWidth()
        {
            if (ItemsCount != 0)
            {
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

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = Columns[e.ColumnIndex].Width;
            e.Cancel = true;
            base.OnColumnWidthChanging(e);
        }
    }
}
