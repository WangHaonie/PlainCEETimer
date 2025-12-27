using System;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI;

public class PagedContextMenu
{
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
            }
        }
    }

    public int CountPerPage
    {
        get => ppCount;
        set => ppCount = value.Clamp(10, 300);
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

    public MenuItem Parent
    {
        get => m_parent;
        set
        {
            if (value != null)
            {
                m_parent = value;
            }
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
    private bool isPaged;
    private bool isLoaded;
    private string[] m_items;
    private MenuItem m_parent;
    private MenuItem m_defitem;
    private MenuItem m_lastchecked;
    private MenuItem[] m_pages;
    private Menu.MenuItemCollection m_target;
    private readonly int radioOffset = 2;
    private readonly MenuItem m_settings = new("设置每页最大项数");
    private readonly MenuItem m_separator = new("-");

    public void Build()
    {
        if (m_parent != null && m_items != null)
        {
            isLoaded = false;
            m_target = m_parent.MenuItems;
            m_target.Clear();
            m_defitem = null;
            m_lastchecked = null;
            CreateNewInstance();
            isLoaded = true;
            DoRadioCheck(absoluteIndex);
        }
    }

    private void CreateNewInstance()
    {
        m_target.AddRange([m_settings, m_separator]);

        if (totalCount == 0)
        {
            m_target.Add(m_defitem = new MenuItem(DefaultText) { Enabled = false });
            isPaged = false;
            return;
        }

        if (totalCount <= ppCount)
        {
            for (int i = 0; i < totalCount; i++)
            {
                var item = new MenuItem(m_items[i]);
                item.Click += Item_Click;
                m_target.Add(item);
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
            m_target.Add(pageHeader);
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
                m_parent.DoRadioCheck(index + radioOffset, out m_lastchecked);
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