using System;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public static class ToolTipStyles
{
    public static void ApplyStyle(IntPtr htt, bool useDark, bool init)
    {
        ThemeManager.ApplyControlTheme(htt, useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);

        if (init)
        {
            Win32UI.PnSetTopMostWindow(htt);

            if (SystemVersion.IsWindows11)
            {
                Win32UI.PnSetRoundCornerEx(htt, true);
            }
        }
    }
}
