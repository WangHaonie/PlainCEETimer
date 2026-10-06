using System.Windows;
using PlainCEETimer.Modules;

namespace PlainCEETimer.WPF.Modules;

public static class ScrollBarAdditions
{
    public static readonly DependencyProperty ShowArrowsProperty =
        DependencyProperty.RegisterAttached(MemberNames.ShowArrows, typeof(bool), typeof(ScrollBarAdditions),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    public static bool GetShowArrows(DependencyObject obj)
    {
        return (bool)obj.GetValue(ShowArrowsProperty);
    }

    public static void SetShowArrows(DependencyObject obj, bool value)
    {
        obj.SetValue(ShowArrowsProperty, value);
    }
}
