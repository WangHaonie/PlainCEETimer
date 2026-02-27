using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainToolTip : ToolTip
{
    public void InitStyle()
    {
        var hWnd = this.GetHandle();

        if (SystemVersion.IsWindows11)
        {
            Win32UI.SetRoundCornerEx(hWnd, true);
        }

        if (ThemeManager.ShouldUseDarkMode)
        {
            ThemeManager.EnableDarkModeForControl(hWnd, NativeStyle.ExplorerDark);
        }
    }
}
