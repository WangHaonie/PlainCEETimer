using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop;

public static class CStd
{
    [DllImport("ucrtbase.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int system(string _Command);
}