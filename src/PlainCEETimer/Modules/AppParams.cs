using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.Modules;

[NoConstants]
internal static class AppParams
{
    public static bool DebugMode
    {
        get => m_DebugMode;
        set => m_DebugMode = value;
    }

    public static bool DisableWFPMv2 => m_DebugMode && m_DisableWFPMv2;

    public static bool EnableCommDlgPMv2 => m_DebugMode && m_EnableCommDlgPMv2;

    public static bool UseClassicTSP => m_DebugMode && m_UseClassicTSP;

    public static string TSFormat => m_DebugMode ? m_TSFormat : null;

    private static bool m_DebugMode;
    private static bool m_DisableWFPMv2;
    private static bool m_EnableCommDlgPMv2;
    private static bool m_UseClassicTSP;
    private static string m_TSFormat;

    public const string DisableWFPMv2_Key = "BBFB";
    public const string EnableCommDlgPMv2_Key = "ACB1";
    public const string UseClassicTSP_Key = "9AA6";
    public const string TSFormat_Key = "4BD0";

    public static void LoadConfig()
    {
        var info = App.Current.AppConfig.Params;

        if (info != null)
        {
            m_DebugMode = info.Debug;
            m_DisableWFPMv2 = info.DisableWFPMv2;
            m_EnableCommDlgPMv2 = info.EnableCommDlgPMv2;
            m_UseClassicTSP = info.UseClassicTSP;
            m_TSFormat = info.TSFormat;
        }
    }
}
