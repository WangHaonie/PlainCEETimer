using System;
using System.Drawing;

namespace PlainCEETimer.Interop.Extensions;

public static class IntPtrExtensions
{
    public static Point AsMenuLocation(this IntPtr lParam)
    {
        var dw = lParam.ToInt32();

        if (dw == -1)
        {
            dw = 0;
        }

        return new(dw);
    }

    public static DpiAwarenessContextHandle AsDpiAwarenessContextHandle(this IntPtr ptr)
    {
        return new(ptr);
    }
}