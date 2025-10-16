using System;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace PlainCEETimer.Modules;

public readonly struct SystemVersion
{
    public static readonly bool IsWindows11;
    public static readonly SystemVersion Current;

    public readonly int Major;
    public readonly int Minor;
    public readonly int Build;
    public readonly int UBR;

    public SystemVersion()
    {
        using var reg = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", rootKey: RegistryHive.LocalMachine);
        UBR = reg.Get(nameof(UBR), 0);
        var ver = Environment.OSVersion.Version;
        Major = ver.Major;
        Minor = ver.Minor;
        Build = ver.Build;
    }

    static SystemVersion()
    {
        Current = new();
        IsWindows11 = Current.Build >= WindowsBuilds.Windows11_21H2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(SystemVersion sv)
    {
        return sv.Build;
    }
}