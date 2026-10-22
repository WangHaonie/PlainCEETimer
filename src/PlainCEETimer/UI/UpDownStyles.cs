using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public sealed class UpDownStyles : NativeWindow
{
    public static bool CanApply => ThemeManager.IsDarkModeSupported && !SystemVersion.IsWindows11;

    internal bool UseDark;

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WinUser.WM_PAINT && UseDark)
        {
            Win32UI.PnHookThemedPaint();
            base.WndProc(ref m);
            Win32UI.PnUnhookThemedPaint();
            return;
        }

        base.WndProc(ref m);
    }
}
