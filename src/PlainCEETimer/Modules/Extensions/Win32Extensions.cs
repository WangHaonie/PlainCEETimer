using System;
using System.Drawing;

namespace PlainCEETimer.Modules.Extensions;

public static class Win32Extensions
{
    public static int ToWin32(this Color color)
        => ColorTranslator.ToWin32(color);

    public static int ToWin32(this bool b)
        => Convert.ToInt32(b);

    public static bool ToBool(this int i)
        => Convert.ToBoolean(i);
}