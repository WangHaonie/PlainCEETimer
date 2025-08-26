using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class DisplayHelper
{
    public static string[] GetSystemDisplays()
    {
        List<string> tmp = [];

        EnumSystemDisplays(d =>
        {
            tmp.Add(d.ToString());
            return BOOL.TRUE;
        });

        return [.. tmp];
    }

    [DllImport(App.NativesDll, EntryPoint = "#12")]
    private static extern void EnumSystemDisplays(EnumDisplayProc lpfnEnum);

    private delegate BOOL EnumDisplayProc(SystemDisplay info);
}
