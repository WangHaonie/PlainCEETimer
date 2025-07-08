using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls
{
    public sealed class ListViewEx : ListView
    {
        public int SelectedItemsCount => SelectedItems.Count;

        public ListViewItem SelectedItem => SelectedItems[0];

        public string[] Headers
        {
            get;
            set
            {
                var length = value.Length;

                if (value != null && length != 0)
                {
                    Columns.Clear();

                    foreach (var title in value)
                    {
                        Columns.Add(new ColumnHeader() { Text = title });
                    }

                    LastColumn = Columns[length - 1];
                }

                field = value;
            }
        }

        private ColumnHeader LastColumn;
        private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

        public ListViewEx()
        {
            View = View.Details;
            FullRowSelect = true;
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

        public void SelectAll(BOOL selected)
        {
            if (Items.Count != 0)
            {
                ListViewHelper.SelectAllItems(Handle, selected);
            }
        }

        public void Suspend(Action action)
        {
            BeginUpdate();
            action();
            EndUpdate();
            Focus();
        }

        public void AutoAdjustColumnWidth()
        {
            foreach (ColumnHeader column in Columns)
            {
                column.Width = -2;
            }

            if (HasScrollBar(BOOL.TRUE) && HasScrollBar(BOOL.FALSE))
            {
                int w = LastColumn.Width;
                LastColumn.Width -= ThemeManager.VerticalScrollBarWidth;

                if (HasScrollBar(BOOL.FALSE))
                {
                    LastColumn.Width = w;
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            ListViewHelper.FlushTheme(Handle, new(ThemeManager.DarkFore), new(UseDark));
            base.OnHandleCreated(e);
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = Columns[e.ColumnIndex].Width;
            e.Cancel = true;
            base.OnColumnWidthChanging(e);
        }

        private bool HasScrollBar(BOOL isVertical)
        {
            return ListViewHelper.HasScrollBar(Handle, isVertical);
        }
    }
}
