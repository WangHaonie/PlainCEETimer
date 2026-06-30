using System.Reflection;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.Modules.Reflection;
using WFSize = System.Drawing.Size;

namespace System.Windows.Forms;

[NoConstants]
internal static class DpiHelper
{
    internal static bool isInitialized
    {
        get
        {
            EnsureAccess(ID_isInitialized);
            return (bool)s_fiIsInitialized.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_isInitialized);
            s_fiIsInitialized.SetValue(null, value);
        }
    }

    internal static double deviceDpi
    {
        get
        {
            EnsureAccess(ID_deviceDpi);
            return (double)s_fiDeviceDpi.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_deviceDpi);
            s_fiDeviceDpi.SetValue(null, value);
        }
    }

    internal static double logicalToDeviceUnitsScalingFactor
    {
        get
        {
            EnsureAccess(ID_logicalToDeviceUnitsScalingFactor);
            return (double)s_filogicalToDeviceUnitsScalingFactor.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_logicalToDeviceUnitsScalingFactor);
            s_filogicalToDeviceUnitsScalingFactor.SetValue(null, value);
        }
    }

    internal static bool enableHighDpi
    {
        get
        {
            EnsureAccess(ID_enableHighDpi);
            return (bool)s_fiEnableHighDpi.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_enableHighDpi);
            s_fiEnableHighDpi.SetValue(null, value);
        }
    }

    internal static bool enableDpiChangedMessageHandling
    {
        get
        {
            EnsureAccess(ID_enableDpiChangedMessageHandling);
            return (bool)s_fiEnableDpiChangedMessageHandling.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_enableDpiChangedMessageHandling);
            s_fiEnableDpiChangedMessageHandling.SetValue(null, value);
        }
    }

    private const int ID_isInitialized = 0x001F;
    private const int ID_deviceDpi = 0x00F1;
    private const int ID_enableHighDpi = 0x0010;
    private const int ID_enableDpiChangedMessageHandling = 0x00F0;
    private const int ID_logicalToDeviceUnitsScalingFactor = 0x0002;

    private static FieldInfo s_fiIsInitialized;
    private static FieldInfo s_fiDeviceDpi;
    private static FieldInfo s_fiEnableHighDpi;
    private static FieldInfo s_fiEnableDpiChangedMessageHandling;
    private static FieldInfo s_filogicalToDeviceUnitsScalingFactor;
    private static DpiHelper_LogicalToDeviceUnits1 s_fnLogicalToDeviceUnits1;
    private static DpiHelper_LogicalToDeviceUnits2 s_fnLogicalToDeviceUnits2;
    private static readonly Type s_type;

    static DpiHelper()
    {
        s_type = ReflectionHelper.RevealType(typeof(Control), typeof(DpiHelper));
    }

    public static int LogicalToDeviceUnits(int value, int devicePixels = 0)
    {
        s_fnLogicalToDeviceUnits1 ??= DelegateHelper.StaticCreateDelegate<DpiHelper_LogicalToDeviceUnits1>(s_type, typeof(int), BindingFlags.Public);
        return s_fnLogicalToDeviceUnits1(value, devicePixels);
    }

    public static WFSize LogicalToDeviceUnits(WFSize logicalSize, int deviceDpi = 0)
    {
        s_fnLogicalToDeviceUnits2 ??= DelegateHelper.StaticCreateDelegate<DpiHelper_LogicalToDeviceUnits2>(s_type, typeof(WFSize), BindingFlags.Public);
        return s_fnLogicalToDeviceUnits2(logicalSize, deviceDpi);
    }

    private static void EnsureAccess(int id)
    {
        var flags = BindingFlags.Static | BindingFlags.NonPublic;

        switch (id)
        {
            case ID_isInitialized:
                s_fiIsInitialized ??= s_type.GetField(nameof(isInitialized), flags);
                break;
            case ID_deviceDpi:
                s_fiDeviceDpi ??= s_type.GetField(nameof(deviceDpi), flags);
                break;
            case ID_enableHighDpi:
                s_fiEnableHighDpi ??= s_type.GetField(nameof(enableHighDpi), flags);
                break;
            case ID_enableDpiChangedMessageHandling:
                s_fiEnableDpiChangedMessageHandling ??= s_type.GetField(nameof(enableDpiChangedMessageHandling), flags);
                break;
            case ID_logicalToDeviceUnitsScalingFactor:
                s_filogicalToDeviceUnitsScalingFactor ??= s_type.GetField(nameof(logicalToDeviceUnitsScalingFactor), flags);
                break;
        }
    }
}