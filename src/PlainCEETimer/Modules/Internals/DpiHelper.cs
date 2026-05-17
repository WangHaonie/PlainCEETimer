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
            return (bool)m_fiIsInitialized.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_isInitialized);
            m_fiIsInitialized.SetValue(null, value);
        }
    }

    internal static double deviceDpi
    {
        get
        {
            EnsureAccess(ID_deviceDpi);
            return (double)m_fiDeviceDpi.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_deviceDpi);
            m_fiDeviceDpi.SetValue(null, value);
        }
    }

    internal static double logicalToDeviceUnitsScalingFactor
    {
        get
        {
            EnsureAccess(ID_logicalToDeviceUnitsScalingFactor);
            return (double)m_filogicalToDeviceUnitsScalingFactor.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_logicalToDeviceUnitsScalingFactor);
            m_filogicalToDeviceUnitsScalingFactor.SetValue(null, value);
        }
    }

    internal static bool enableHighDpi
    {
        get
        {
            EnsureAccess(ID_enableHighDpi);
            return (bool)m_fiEnableHighDpi.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_enableHighDpi);
            m_fiEnableHighDpi.SetValue(null, value);
        }
    }

    internal static bool enableDpiChangedMessageHandling
    {
        get
        {
            EnsureAccess(ID_enableDpiChangedMessageHandling);
            return (bool)m_fiEnableDpiChangedMessageHandling.GetValue(null);
        }
        set
        {
            EnsureAccess(ID_enableDpiChangedMessageHandling);
            m_fiEnableDpiChangedMessageHandling.SetValue(null, value);
        }
    }

    private const int ID_isInitialized = 0x001F;
    private const int ID_deviceDpi = 0x00F1;
    private const int ID_enableHighDpi = 0x0010;
    private const int ID_enableDpiChangedMessageHandling = 0x00F0;
    private const int ID_logicalToDeviceUnitsScalingFactor = 0x0002;

    private static FieldInfo m_fiIsInitialized;
    private static FieldInfo m_fiDeviceDpi;
    private static FieldInfo m_fiEnableHighDpi;
    private static FieldInfo m_fiEnableDpiChangedMessageHandling;
    private static FieldInfo m_filogicalToDeviceUnitsScalingFactor;
    private static DpiHelper_LogicalToDeviceUnits1 m_fnLogicalToDeviceUnits1;
    private static DpiHelper_LogicalToDeviceUnits2 m_fnLogicalToDeviceUnits2;
    private static readonly Type m_ref;
    private static readonly Type m_current;
    private static readonly Type m_type;
    private static readonly BindingFlags m_bfAttr = BindingFlags.Static | BindingFlags.NonPublic;

    static DpiHelper()
    {
        m_ref = typeof(Control);
        m_current = typeof(DpiHelper);
        m_type = m_ref.Assembly.GetType(m_current.FullName);
    }

    public static int LogicalToDeviceUnits(int value, int devicePixels = 0)
    {
        m_fnLogicalToDeviceUnits1 ??= DelegateHelper.StaticCreateDelegate<DpiHelper_LogicalToDeviceUnits1>(m_ref, m_current, typeof(int));
        return m_fnLogicalToDeviceUnits1(value, devicePixels);
    }

    public static WFSize LogicalToDeviceUnits(WFSize logicalSize, int deviceDpi = 0)
    {
        m_fnLogicalToDeviceUnits2 ??= DelegateHelper.StaticCreateDelegate<DpiHelper_LogicalToDeviceUnits2>(m_ref, m_current, typeof(WFSize));
        return m_fnLogicalToDeviceUnits2(logicalSize, deviceDpi);
    }

    private static void EnsureAccess(int id)
    {
        switch (id)
        {
            case ID_isInitialized:
                m_fiIsInitialized ??= m_type.GetField(nameof(isInitialized), m_bfAttr);
                break;
            case ID_deviceDpi:
                m_fiDeviceDpi ??= m_type.GetField(nameof(deviceDpi), m_bfAttr);
                break;
            case ID_enableHighDpi:
                m_fiEnableHighDpi ??= m_type.GetField(nameof(enableHighDpi), m_bfAttr);
                break;
            case ID_enableDpiChangedMessageHandling:
                m_fiEnableDpiChangedMessageHandling ??= m_type.GetField(nameof(enableDpiChangedMessageHandling), m_bfAttr);
                break;
            case ID_logicalToDeviceUnitsScalingFactor:
                m_filogicalToDeviceUnitsScalingFactor ??= m_type.GetField(nameof(logicalToDeviceUnitsScalingFactor), m_bfAttr);
                break;
        }
    }
}