using System;
using System.Windows.Forms;
using PlainCEETimer.UI.Core;

namespace PlainCEETimer.UI.Extensions;

public static class ContextMenuExtensions
{
    public static TControl AttachContextMenu<TControl>(this TControl control, MenuItemBuilder builder, out ContextMenu instance)
        where TControl : Control
    {
        return AttachContextMenu(control, builder, null, out instance);
    }

    public static TControl AttachContextMenu<TControl>(this TControl control, MenuItemBuilder builder, EventHandler onPopup, out ContextMenu instance)
        where TControl : Control
    {
        return AttachContextMenuEx(new IHasContextMenuWrapper<TControl>(control), builder, onPopup, out instance).Target;
    }

    public static T AttachContextMenuEx<T>(this T obj, MenuItemBuilder builder, out ContextMenu instance)
        where T : IHasContextMenu
    {
        return AttachContextMenuEx(obj, builder, null, out instance);
    }

    public static T AttachContextMenuEx<T>(this T obj, MenuItemBuilder builder, EventHandler onPopup, out ContextMenu instance)
        where T : IHasContextMenu
    {
        var menu = Build(builder);
        menu.Popup += onPopup;
        obj.ContextMenu = menu;
        instance = menu;
        return obj;
    }

    public static ContextMenu Build(this MenuItemBuilder builder)
    {
        return new(builder(new()));
    }

    public static ContextMenu AddItems(this ContextMenu menu, MenuItemBuilder builder, int index = -1)
    {
        var items = menu.MenuItems;
        var count = items.Count;
        var newItems = builder(new());

        if (index > 0 && index < count - 1)
        {
            for (int i = 0; i < newItems.Length; i++)
            {
                items.Add(index + i, newItems[i]);
            }

            return menu;
        }

        items.AddRange(newItems);
        return menu;
    }

    public static ContextMenu RemoveRange(this ContextMenu menu, int start, int length)
    {
        var items = menu.MenuItems;

        for (int i = 0; i < length; i++)
        {
            items.RemoveAt(start);
        }

        return menu;
    }

    public static MenuItem AsDefault(this MenuItem item)
    {
        item.DefaultItem = true;
        return item;
    }

    public static MenuItem Disable(this MenuItem item)
    {
        item.Enabled = false;
        return item;
    }
}