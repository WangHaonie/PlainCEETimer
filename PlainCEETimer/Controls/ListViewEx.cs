using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Drawing;
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

            if (ThemeManager.ShouldUseDarkMode)
            {
                OwnerDraw = true;
            }
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
            foreach (ColumnHeader column in Columns)
            {
                column.Width = -2;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            if (ThemeManager.ShouldUseDarkMode)
            {
                ThemeManager.FlushDarkControl(this, DarkControlType.Explorer);
                ForeColor = ThemeManager.DarkFore;
                BackColor = ThemeManager.DarkBack;
            }

            base.OnHandleCreated(e);
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                using var backBrush = new SolidBrush(ThemeManager.DarkBack);
                using var foreBrush = new SolidBrush(ThemeManager.DarkFore);
                using var sf = new StringFormat();
                var bounds = e.Bounds;
                sf.Alignment = StringAlignment.Near;
                e.Graphics.FillRectangle(backBrush, bounds);
                e.Graphics.DrawString(e.Header.Text, Font, foreBrush, new Rectangle(bounds.X + 3, bounds.Y + 3, bounds.Width, bounds.Height), sf);
            }

            base.OnDrawColumnHeader(e);
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawItem(e);
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawSubItem(e);
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = Columns[e.ColumnIndex].Width;
            e.Cancel = true;
            base.OnColumnWidthChanging(e);
        }
    }
}
