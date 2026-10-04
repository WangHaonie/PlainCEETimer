using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using PlainCEETimer.Modules;

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
        var current = start;
        menu = null;

        while (current != null)
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

            var parent = VisualTreeHelper.GetParent(current);

            if (parent == null && current is FrameworkElement fe)
            {
                parent = fe.Parent;
            }

            current = parent;
        }

        return false;
    }
}
