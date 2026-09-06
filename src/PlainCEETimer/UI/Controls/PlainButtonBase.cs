using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

internal class PlainButtonBase : IThemeAware
{
    internal bool AutoDarkTheme = true;
    internal bool ShouldHookPaint;

    private ThemeHelper th;
    private readonly ButtonBase button;

    public PlainButtonBase(ButtonBase b)
    {
        b.FlatStyle = FlatStyle.System;
        button = b;
    }

    internal void Attach()
    {
        th ??= new(this);
    }

    internal void Detach()
    {
        th.Destroy();
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        ShouldHookPaint = useDark && !ThemeManager.NewThemeAvailable;
        ThemeManager.ApplyControlTheme(button, useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer, AutoDarkTheme);
    }
}
