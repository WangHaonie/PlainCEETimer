using System;
using Microsoft.Win32;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;

namespace PlainCEETimer.Modules;

[NoConstants]
public class SystemVersion
{
    public static bool BeforeNT10 => isBeforeNT10 ??= !Current.AtLeast(WindowsVersions.NT10);

    public static bool IsWindows11 => isWindows11 ??= Current.AtLeast(WindowsVersions.Windows11_21H2);

    public static readonly SystemVersion Current = new();

    private readonly Version m_version;

    private static bool? isBeforeNT10;
    private static bool? isWindows11;
    private const string CurrentVersionRegistryPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

    private SystemVersion()
    {
        var v = Environment.OSVersion.Version;
        m_version = new(v.Major, v.Minor, v.Build, UBR());
    }

    public bool AtLeast(Version version)
    {
        var v1 = m_version;
        var v2 = version;

        if (v1.Major.AtLeast(v2.Major))
        {
            return true;
        }

        if (v1.Minor.AtLeast(v2.Minor))
        {
            return true;
        }

        if (v1.Build.AtLeast(v2.Build))
        {
            return true;
        }

        return v1.Revision.AtLeast(v2.Revision);
    }

    private static int UBR()
    {
        using var reg = RegistryHelper.Open(CurrentVersionRegistryPath, rootKey: RegistryHive.LocalMachine);
        return reg.Get(nameof(UBR), 0);
    }
}
