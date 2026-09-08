using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

[SuppressUnmanagedCodeSecurity]
public static class DisplayHelper
{
    public static string[] GetSystemDisplays()
    {
        List<string> tmp = [];

        PnEnumSystemDisplays(d =>
        {
            tmp.Add(d.ToString());
            return true;
        });

        if (tmp.Count == 0)
        {
            tmp.Add("<未知>");
        }

        return [.. tmp];
    }

    [DllImport(App.NativesDll, EntryPoint = "#1")]
    private static extern bool PnEnumSystemDisplays(EnumDisplayProc lpfnEnum);
}
