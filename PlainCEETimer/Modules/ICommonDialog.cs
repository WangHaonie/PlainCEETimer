using System;

namespace PlainCEETimer.Modules
{
    public interface ICommonDialog
    {
        IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
