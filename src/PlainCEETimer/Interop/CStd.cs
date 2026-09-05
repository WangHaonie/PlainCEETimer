using System.Runtime.InteropServices;
using System.Security;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

[SuppressUnmanagedCodeSecurity]
internal static class CStd
{
    [DllImport(App.CrtDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, ExactSpelling = true)]
    public static extern int system(string _Command);
}