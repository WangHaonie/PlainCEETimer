using System;

namespace PlainCEETimer.UI;

public class HotKeyPressEventArgs(IntPtr wParam, IntPtr lParam)
{
    public int Id { get; } = wParam.ToInt32();

    public HotKey HotKey { get; } = new(lParam);
}
