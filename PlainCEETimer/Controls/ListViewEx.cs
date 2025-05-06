using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

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

                    foreach (var title in value)
                    {
                        Columns.Add(new ColumnHeader() { Text = title });
                    }
                }

                field = value;
            }
        }

        private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

        public ListViewEx()
        {
            View = View.Details;
            FullRowSelect = true;
            GridLines = false;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            HideSelection = false;
            ShowItemToolTips = true;

            if (UseDark)
            {
                ForeColor = ThemeManager.DarkFore;
                BackColor = ThemeManager.DarkBack;
            }

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void SelectAll(bool IsSelected)
        {
            if (ItemsCount != 0)
            {
                ListViewHelper.SelectAllItems(Handle, IsSelected ? 1 : 0);
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
            var count = Columns.Count;
            var w = 0;

            for (int i = 0; i < Columns.Count; i++)
            {
                var col = Columns[i];

                if (i == count - 1)
                {
                    col.Width = Width - w - ThemeManager.VScrollBarWidth - 8;
                }
                else
                {
                    col.Width = -2;
                    w += col.Width;
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            ListViewHelper.FlushTheme(Handle, UseDark);
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
