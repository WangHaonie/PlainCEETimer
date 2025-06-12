using System;

namespace PlainCEETimer.UI
{
    public interface ICommonDialog
    {
        IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
