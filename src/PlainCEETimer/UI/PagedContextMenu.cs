using System;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI;

public class PagedContextMenu
{
    public string Text
    {
        get;
        set
        {
            if (field != value)
            {
                m_menu?.Text = value;
                field = value;
            }
        }
    }

    public string DefaultText
    {
        get;
        set
        {
            if (field != value)
            {
                m_defitem?.Text = value;
                field = value;
            }
        }
    }

    public string[] Items
    {
        get => m_items;
        set
        {
            if (value != null && m_items != value)
            {
                m_items = value;
                totalCount = value.Length;
                absoluteIndex = absoluteIndex.Clamp(0, totalCount - 1);
                canRebuild = true;
            }
        }
    }

    public int CountPerPage
    {
        get => ppCount;
        set
        {
            if (!canRebuild)
            {
                canRebuild = ppCount != value;
            }

            ppCount = value.Clamp(10, 300);
        }
    }

    public int SelectedIndex
    {
        get => absoluteIndex;
        set
        {
            if (absoluteIndex != value)
            {
                absoluteIndex = value;
                DoRadioCheck(value);
            }
        }
    }

    public MenuItem MenuItem
    {
        get
        {
            if (canRebuild)
            {
                isLoaded = false;
                m_menu.Destory();
                m_menu = null;
                m_defitem = null;
                CreateNewInstance();
                isLoaded = true;
                DoRadioCheck(absoluteIndex);
                canRebuild = false;
            }

            return m_menu;
        }
    }

    public event EventHandler ItemClick;

    public event EventHandler SettingsRequest
    {
        add
        {
            m_settings.Click += value;
        }
        remove
        {
            m_settings.Click -= value;
        }
    }

    private int ppCount = 30;
    private int totalCount;
    private int pageCount;
    private int absoluteIndex;
    private bool canRebuild;
    private bool isPaged;
    private bool isLoaded;
    private string[] m_items;
    private MenuItem m_menu;
    private MenuItem m_defitem;
    private MenuItem m_lastchecked;
    private MenuItem[] m_pages;
    private readonly int radioOffset = 2;
    private readonly MenuItem m_settings = new("设置每页最大项数");
    private readonly MenuItem m_separator = new("-");

    private void CreateNewInstance()
    {
        m_menu = new MenuItem(Text);
        var items = m_menu.MenuItems;
        items.AddRange([m_settings, m_separator]);

        if (totalCount == 0)
        {
            items.Add(m_defitem = new MenuItem(DefaultText) { Enabled = false });
            isPaged = false;
            return;
        }

        if (totalCount <= ppCount)
        {
            for (int i = 0; i < totalCount; i++)
            {
                var item = new MenuItem(m_items[i]);
                item.Click += Item_Click;
                items.Add(item);
            }

            isPaged = false;
            return;
        }

        pageCount = (int)Math.Ceiling((double)totalCount / ppCount);
        m_pages = new MenuItem[pageCount];

        for (int i = 0; i < pageCount; i++)
        {
            var ipStart = i * ppCount;
            var ipEnd = Math.Min(ipStart + ppCount, totalCount) - 1;
            var pageHeader = new MenuItem($"{ipStart + 1}~{ipEnd + 1}") { Tag = i };
            items.Add(pageHeader);
            m_pages[i] = pageHeader;
            var pItems = pageHeader.MenuItems;

            for (int j = ipStart; j <= ipEnd; j++)
            {
                var item = new MenuItem(m_items[j]);
                item.Click += PagedItem_Click;
                pItems.Add(item);
            }
        }

        isPaged = true;
    }

    private void DoRadioCheck(int index, int pageIndex = -1)
    {
        if (isLoaded)
        {
            if (totalCount == 0 || index > totalCount)
            {
                return;
            }

            m_lastchecked.Uncheck();

            if (isPaged)
            {
                if (pageIndex < 0)
                {
                    index = PagedIndexAbsoluteToRelative(index, out pageIndex);
                }

                for (int i = 0; i <= pageIndex; i++)
                {
                    var item = m_pages[i];

                    if (i == pageIndex)
                    {
                        item.DefaultItem = true;
                        item.DoRadioCheck(index, out m_lastchecked);
                    }
                    else
                    {
                        item.DefaultItem = false;
                    }
                }
            }
            else
            {
                m_menu.DoRadioCheck(index + radioOffset, out m_lastchecked);
            }
        }
    }

    private void Item_Click(object sender, EventArgs e)
    {
        var item = (MenuItem)sender;

        if (item != m_lastchecked)
        {
            var index = item.Index;
            absoluteIndex = index - radioOffset;
            DoRadioCheck(absoluteIndex);
            OnItemClick(e);
        }
    }

    private void PagedItem_Click(object sender, EventArgs e)
    {
        var item = (MenuItem)sender;

        if (item != m_lastchecked)
        {
            var index = item.Index;
            var pageIndex = (int)item.Parent.Tag;
            absoluteIndex = PagedIndexRelativeToAbsolute(index, pageIndex);
            DoRadioCheck(index, pageIndex);
            OnItemClick(e);
        }
    }

    private void OnItemClick(EventArgs e)
    {
        ItemClick?.Invoke(this, e);
    }

    private int PagedIndexRelativeToAbsolute(int index, int pageIndex)
    {
        return index + ppCount * pageIndex;
    }

    private int PagedIndexAbsoluteToRelative(int index, out int pageIndex)
    {
        pageIndex = index / ppCount;
        return index % ppCount;
    }
}