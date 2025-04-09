using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class NavigationBar : TreeView
    {
        private readonly int ItemsCount;
        private readonly NavigationPage[] Pages;

        /// <summary>
        /// 初始化新的竖直导航栏实例。
        /// </summary>
        /// <param name="navItems">导航栏的项 (不支持多级)。</param>
        /// <param name="pages">导航栏关联的相关页面。</param>
        /// <param name="pagePresenter">用于显示页面的可滚动的控件</param>
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

        /// <summary>
        /// 切换到对应的页面。
        /// </summary>
        /// <param name="page"></param>
        public void SwitchTo(NavigationPage page)
        {
            SwitchTo(page.Index);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ForeColor = ThemeManager.DarkFore;
                BackColor = ThemeManager.DarkBack;
                ThemeManager.FlushDarkControl(this, DarkControlType.Explorer);
            }
            else
            {
                ThemeManager.FlushDarkControl(this, DarkControlType.ExplorerLight);
            }

            base.OnHandleCreated(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            SwitchTo(e.Node.Index);
            base.OnAfterSelect(e);
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
