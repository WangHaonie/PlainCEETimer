using System;

namespace PlainCEETimer.Modules
{
    public interface ICommonDialog
    {
        string DialogTitle { get; }

        CommonDialogKind DialogKind { get; }

        IntPtr BaseHookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
