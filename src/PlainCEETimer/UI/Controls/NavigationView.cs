using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class NavigationView : Control
{
    private sealed class NavigationBar : TreeView
    {
        internal NavigationBar()
        {
            BorderStyle = BorderStyle.None;
            Dock = DockStyle.Fill;
            FullRowSelect = true;
            HideSelection = false;
            HotTracking = true;
            ShowLines = false;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
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
    }

    public new int Height
    {
        get => m_height;
        set
        {
            m_height = value;
            UpdateView();
        }
    }

    public int NavigationBarWidth
    {
        get => m_barw;
        set
        {
            m_barw = value;
            UpdateView();
        }
    }

    public int NavigationPageWidth
    {
        get => m_pagew;
        set
        {
            m_pagew = value;
            UpdateView();
        }
    }

    public int HeaderIndent
    {
        get => m_hindent;
        set
        {
            m_hindent = value;
            UpdateView();
        }
    }

    public int HeaderHeight
    {
        get => m_hheight;
        set
        {
            m_hheight = value;
            UpdateView();
        }
    }

    public event EventHandler<NavigationViewEventArgs> SelectedPageChanged;

    private int m_height = 150;
    private int m_barw = 40;
    private int m_pagew = 110;
    private int m_hindent = 5;
    private int m_hheight = 25;
    private bool isSwitching;
    private Panel panelNavBar;
    private Panel panelNavPages;
    private NavigationBar navBar;
    private TreeNodeCollection m_headers;
    private ControlCollection m_ctrls;
    private List<NavigationPage> m_pages;

    public NavigationView()
    {
        Initialize();
    }

    public void AddPage(NavigationPage page)
    {
        page.Header = m_headers.Add(page.Text);
        m_ctrls.Add(page);
        m_pages.Add(page);
    }

    public void AddPages(NavigationPage[] pages)
    {
        foreach (var page in pages)
        {
            page.Header = m_headers.Add(page.Text);
        }

        m_ctrls.AddRange(pages);
        m_pages.AddRange(pages);
    }

    public void RemovePage(NavigationPage page)
    {
        m_headers.Remove(page.Header);
        m_ctrls.Remove(page);
        m_pages.Remove(page);
    }

    public void SwitchTo(NavigationPage page)
    {
        var index = page.Header.Index;
        SwitchToPageCore(page, index);
        navBar.SelectedNode = page.Header;
        navBar.Focus();
    }

    private void UpdateView()
    {
        navBar.Indent = m_hindent;
        navBar.ItemHeight = m_hheight;
        panelNavBar.Height = m_height;
        panelNavBar.Width = m_barw;
        panelNavPages.Left = m_barw;
        panelNavPages.Height = m_height;
        panelNavPages.Width = m_pagew;
        Width = m_barw + m_pagew;
        base.Height = m_height;
    }

    private void SwitchToPageCore(NavigationPage page, int index)
    {
        isSwitching = true;
        var length = m_pages.Count;
        var cindex = navBar.SelectedNode.Index;

        for (int i = 0; i < length; i++)
        {
            var current = m_pages[i];
            current.Visible = page == current && i == index;
        }

        if (index != cindex)
        {
            OnSelectedPageChanged(index, page);
        }

        isSwitching = false;
    }

    protected override void OnGotFocus(EventArgs e)
    {
        navBar.Focus();
        base.OnGotFocus(e);
    }

    private void OnAfterSelect(object sender, TreeViewEventArgs e)
    {
        if (!isSwitching)
        {
            var index = e.Node.Index;
            var page = m_pages[index];
            SwitchToPageCore(page, index);
        }
    }

    private void OnSelectedPageChanged(int index, NavigationPage page)
    {
        SelectedPageChanged?.Invoke(this, new(index, page));
    }

    private void Initialize()
    {
        navBar = new();
        navBar.AfterSelect += OnAfterSelect;
        navBar.Dock = DockStyle.Fill;
        m_headers = navBar.Nodes;
        panelNavBar = new();
        panelNavBar.Controls.Add(navBar);
        panelNavPages = new();
        m_ctrls = panelNavPages.Controls;
        m_pages = [];
        UpdateView();
        Controls.AddRange([panelNavBar, panelNavPages]);
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }
}