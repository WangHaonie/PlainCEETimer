using PlainCEETimer.Interop;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class NavigationBar : TreeView
    {
        private readonly int ItemsCount;
        private readonly NavigationPage[] Pages;

        public NavigationBar(string[] navItems, NavigationPage[] pages, ScrollableControl pagePresenter)
        {
            BorderStyle = BorderStyle.None;
            Dock = DockStyle.Fill;
            FullRowSelect = true;
            HideSelection = false;
            HotTracking = true;
            ShowLines = false;
            ItemsCount = navItems.Length;

            if (pages.Length == ItemsCount)
            {
                var collection = pagePresenter.Controls;

                for (int i = 0; i < ItemsCount; i++)
                {
                    var page = pages[i];
                    page.Dock = DockStyle.Fill;
                    page.Index = i;
                    Nodes.Add(navItems[i]);
                    collection.Add(page);
                }

                Pages = pages;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            NativeInterop.SetWindowTheme(Handle, "Explorer", null);
            base.OnHandleCreated(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            SwitchTo(e.Node.Index);
            base.OnAfterSelect(e);
        }

        public void SwitchTo(NavigationPage page)
        {
            SwitchTo(page.Index);
        }

        private void SwitchTo(int pageIndex)
        {
            for (int i = 0; i < ItemsCount; i++)
            {
                Pages[i].Visible = i == pageIndex;
            }
        }
    }
}
