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
            return true;
        });

        if (tmp.Count == 0)
        {
            tmp.Add("<未知>");
        }

        return [.. tmp];
    }

    [DllImport(App.NativesDll, EntryPoint = "#3")]
    private static extern bool EnumSystemDisplays(EnumDisplayProc lpfnEnum);
}
