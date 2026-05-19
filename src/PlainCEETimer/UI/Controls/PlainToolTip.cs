using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainToolTip : ToolTip, IThemeAware
{
    private IntPtr Handle;
    private ThemeHelper themeHelper;

    public void InitStyle()
    {
        Handle = this.GetHandle();

        if (SystemVersion.IsWindows11)
        {
            Win32UI.SetRoundCornerEx(Handle, true);
        }

        themeHelper ??= new(this);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        ThemeManager.EnableDarkModeForControl(Handle, useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);
    }
}
