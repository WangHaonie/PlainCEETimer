using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop;

public static class CStd
{
    [DllImport("ucrtbase.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int system(string _Command);
}