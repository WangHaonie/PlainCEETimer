using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Extensions;

public static class Extensions
{
    public static TControl With<TControl>(this TControl control, Action<TControl> additions)
        where TControl : Control
    {
        additions(control);
        return control;
    }

    public static TControl AttachContextMenu<TControl>(this TControl control, MenuItemBuilder builder, out ContextMenu instance)
        where TControl : Control
    {
        return AttachContextMenu(control, builder, null, out instance);
    }

    public static TControl AttachContextMenu<TControl>(this TControl control, MenuItemBuilder builder, EventHandler onPopup, out ContextMenu instance)
        where TControl : Control
    {
        var menu = new ContextMenu(builder(new()));
        menu.Popup += onPopup;
        control.ContextMenu = menu;
        instance = menu;
        return control;
    }

    public static TControl Disable<TControl>(this TControl control)
        where TControl : Control
    {
        control.Enabled = false;
        return control;
    }

    public static TControl Tag<TControl, TData>(this TControl control, TData data)
        where TControl : Control
    {
        control.Tag = data;
        return control;
    }

    public static TControl AsFocus<TControl>(this TControl control, AppForm parent)
        where TControl : Control
    {
        if (control is not Form)
        {
            parent.FocusControl = control;
        }

        return control;
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

    public static MenuItem Default(this MenuItem item)
    {
        item.DefaultItem = true;
        return item;
    }

    public static PlainLinkLabel Link(this PlainLinkLabel label, string link, out LinkLabel.Link instance)
    {
        instance = label.Links.Add(0, label.Text.Length, link);
        return label;
    }

    public static PlainLinkLabel Link(this PlainLinkLabel label, int start, int length, string link, out LinkLabel.Link instance)
    {
        instance = label.Links.Add(start, length, link);
        return label;
    }

    public static void Delete(this Control control)
    {
        if (control is not Form)
        {
            control.Parent.Controls.Remove(control);
        }
    }

    public static void AddControls(this Control control, ControlsBuilder builder)
    {
        var ctrls = builder(new());
        var collection = control.Controls;

        foreach (var ctrl in ctrls)
        {
            collection.Add(ctrl);
        }
    }

    public static AppForm FindParentForm(this Control control)
    {
        return (AppForm)control.FindForm();
    }

    public static Point LocationToScreen(this Control control, int xOffset = 0, int yOffset = 0)
    {
        var top = control.TopLevelControl;
        var p = control.Location;

        if (control is not Form)
        {
            p = top.PointToScreen(p);
        }

        return new(p.X + xOffset, p.Y + yOffset);
    }

    public static void DoRadioCheck(this MenuItem menu, int item, out MenuItem menuItem)
    {
        menuItem = null;

        if (menu != null)
        {
            var hmenu = menu.Handle;
            Win32UI.CheckMenuRadioItem(hmenu, 0, Win32UI.GetMenuItemCount(hmenu) - 1, item, MenuFlag.ByPosition);
            menuItem = menu.MenuItems[item];
        }
    }

    public static void Uncheck(this MenuItem item)
    {
        if (item != null)
        {
            Win32UI.MenuUncheckItem(item.Parent.Handle, item.Index, true);
        }
    }
}
