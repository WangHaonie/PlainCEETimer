using System;

namespace PlainCEETimer.Interop.Extensions;

public static class IntPtrExtensions
{
    public static DpiAwarenessContextHandle AsDpiAwarenessContextHandle(this IntPtr ptr)
    {
        return new(ptr);
    }
}