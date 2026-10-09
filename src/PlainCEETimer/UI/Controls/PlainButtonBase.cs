using System.Windows.Forms;
using PlainCEETimer.Modules;

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
        ThemeHelper.Attach(this, ref th);
    }

    internal void Detach()
    {
        ThemeHelper.Detach(ref th);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        ShouldHookPaint = useDark && !ThemeManager.NewThemeAvailable;
        ThemeManager.ApplyControlTheme(button, useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer, AutoDarkTheme);
    }
}
