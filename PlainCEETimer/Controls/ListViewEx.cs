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

                    foreach (var title in value)
                    {
                        Columns.Add(new ColumnHeader() { Text = title });
                    }
                }

                field = value;
            }
        }

        private readonly bool IsDetails;
        private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

        public ListViewEx()
        {
            View = View.Details;
            FullRowSelect = true;
            GridLines = false;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            HideSelection = false;

            if (UseDark)
            {
                OwnerDraw = true;
                IsDetails = View == View.Details;
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
            foreach (ColumnHeader column in Columns)
            {
                column.Width = -2;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (UseDark)
            {
                ThemeManager.FlushDarkControl(this, NativeStyle.Explorer);
            }

            base.OnHandleCreated(e);
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            if (UseDark && IsDetails)
            {
                using var backBrush = new SolidBrush(ThemeManager.DarkBack);
                var g = e.Graphics;
                var b = e.Bounds;

                g.FillRectangle(backBrush, e.Bounds);
                TextRenderer.DrawText(g, e.Header.Text, Font, b, ThemeManager.DarkFore, TextFormatFlags.LeftAndRightPadding | TextFormatFlags.VerticalCenter);

                if (e.ColumnIndex < Headers.Length - 1)
                {
                    var x = b.Right - 1F;
                    using var p = new SolidBrush(ThemeManager.DarkBorder);
                    g.FillRectangle(p, x, b.Top, 1F, b.Height);
                }
            }
            else
            {
                base.OnDrawColumnHeader(e);
            }
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (!UseDark)
            {
                base.OnDrawItem(e);
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            if (UseDark && IsDetails)
            {
                using var backbrush = new SolidBrush(e.Item.Selected ? ThemeManager.DarkBackSelection : e.SubItem.BackColor);
                var g = e.Graphics;
                var b = e.Bounds;
                g.FillRectangle(backbrush, b);
                b.Offset(4, 0);
                var item = e.SubItem;
                TextRenderer.DrawText(g, item.Text, Font, b, item.ForeColor, TextFormatFlags.VerticalCenter);
            }
            else
            {
                base.OnDrawSubItem(e);
            }
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.NewWidth = Columns[e.ColumnIndex].Width;
            e.Cancel = true;
            base.OnColumnWidthChanging(e);
        }
    }
}
