using System.Windows;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.WPF.Extensions;

namespace PlainCEETimer.WPF.Modules;

public static class Win32ContextMenu
{
    public static readonly DependencyProperty ContextMenuProperty =
        DependencyProperty.RegisterAttached(MemberNames.ContextMenu, typeof(ContextMenu), typeof(Win32ContextMenu),
            new PropertyMetadata(null));

    public static ContextMenu GetContextMenu(DependencyObject obj)
    {
        return (ContextMenu)obj.GetValue(ContextMenuProperty);
    }

    public static void SetContextMenu(DependencyObject obj, ContextMenu value)
    {
        obj.SetValue(ContextMenuProperty, value);
    }

    public static bool TryFindContextMenu(DependencyObject start, DependencyObject end, out ContextMenu menu)
    {
        menu = null;

        for (var current = start; current != null; current = current.GetParent())
        {
            if (current is FrameworkElement)
            {
                var value = current.ReadLocalValue(ContextMenuProperty);

                if (value != DependencyProperty.UnsetValue)
                {
                    menu = value as ContextMenu;
                    return true;
                }
            }

            if (current == end)
            {
                break;
            }
        }

        return false;
    }
}
