using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class NavigationBar : TreeView
{
    private readonly int ItemsCount;
    private readonly NavigationPage[] Pages;

    public NavigationBar(string[] navItems, NavigationPage[] pages)
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
            for (int i = 0; i < ItemsCount; i++)
            {
                pages[i].Index = i;
                Nodes.Add(navItems[i]);
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
        SwitchTo(Nodes[page.Index]);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        if (ThemeManager.ShouldUseDarkMode)
        {
            ForeColor = Colors.DarkForeText;
            BackColor = Colors.DarkBackText;
            ThemeManager.EnableDarkModeForControl(this, NativeStyle.ExplorerDark);
        }
        else
        {
            ThemeManager.EnableDarkModeForControl(this, NativeStyle.Explorer);
        }

        base.OnHandleCreated(e);
    }

    protected override void OnAfterSelect(TreeViewEventArgs e)
    {
        SwitchTo(e.Node);
        base.OnAfterSelect(e);
    }

    private void SwitchTo(TreeNode navItem)
    {
        var index = navItem.Index;
        SelectedNode = navItem;

        for (int i = 0; i < ItemsCount; i++)
        {
            Pages[i].Visible = i == index;
        }
    }
}
