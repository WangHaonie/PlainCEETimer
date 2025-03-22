using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class ListViewEx : ListView
    {
        public int ItemsCount => Items.Count;

        public int SelectedItemsCount => SelectedItems.Count;

        public string[] Headers
        {
            get;
            set
            {
                if (value?.Length != 0)
                {
                    Columns.Clear();

                    foreach (var Title in value)
                    {
                        Columns.Add(new ColumnHeader() { Text = Title });
                    }
                }

                field = value;
            }
        }

        public ListViewEx()
        {
            View = View.Details;
            FullRowSelect = true;
            GridLines = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            HideSelection = false;
        }

        public void SelectAll(bool IsSelected)
        {
            if (ItemsCount != 0)
            {
                foreach (ListViewItem Item in Items)
                {
                    Item.Selected = IsSelected;
                }
            }
        }

        public void Suspend(Action Method)
        {
            BeginUpdate();
            Method();
            EndUpdate();
            Focus();
        }

        public void AutoAdjustColumnWidth()
        {
            foreach (ColumnHeader column in Columns)
            {
                column.Width = -2;
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
