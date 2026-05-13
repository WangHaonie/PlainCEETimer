using System;

namespace PlainCEETimer.Modules;

public static class WindowsVersions
{
    public static Version NT6 => field ??= new(6, 0);

    public static Version Windows81 => field ??= new(6, 3, 9600);

    public static Version NT10 => field ??= new(10, 0);

    public static Version Windows10_1607 => field ??= new(10, 0, 14393);

    public static Version Windows10_1809 => field ??= new(10, 0, 17763);

    public static Version Windows10_1703 => field ??= new(10, 0, 15063);

    public static Version Windows10_1903 => field ??= new(10, 0, 18362);

    public static Version Windows10_20H1 => field ??= new(10, 0, 18985);

    public static Version Windows11_21H2 => field ??= new(10, 0, 22000);

    public static Version Windows11_24H2_WIP_NewDarkTheme => field ??= new(10, 0, 26120, 6682);
}
