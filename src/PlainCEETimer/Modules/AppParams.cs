using System;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Annotations.SourceGenerators;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.Modules;

[NoConstants]
internal static partial class AppParams
{
    [BackingField("m_DebugMode")]
    public static partial bool DebugMode { get; }

    [BackingField("m_DisableWFPMv2")]
    public static partial bool DisableWFPMv2 { get; }

    [BackingField("m_EnableCommDlgPMv2")]
    public static partial bool EnableCommDlgPMv2 { get; }

    [BackingField("m_UseClassicTSP")]
    public static partial bool UseClassicTSP { get; }

    [BackingField("m_MainBackdropAcrylic")]
    public static partial bool MainBackdropAcrylic { get; }

    [BackingField("m_ImmersiveCountdown")]
    public static partial bool ImmersiveCountdown { get; }

    [BackingField("m_TSFormat")]
    public static partial string TSFormat { get; }

    [BackingField("m_TSMax")]
    public static partial TimeSpan TSMax { get; }

    public const string DisableWFPMv2_Key = "BBFB";
    public const string EnableCommDlgPMv2_Key = "ACB1";
    public const string UseClassicTSP_Key = "9AA6";
    public const string MainBackdropAcrylic_Key = "A5C8";
    public const string ImmersiveCountdown_Key = "43A6";
    public const string TSFormat_Key = "4BD0";
    public const string TSMax_Key = "A421";

    public static void LoadConfig(AppParamsInfo info = null)
    {
        info ??= App.Current.AppConfig?.Params;

        if (info != null)
        {
            var dbg = info.Debug;
            m_DebugMode = dbg;
            m_DisableWFPMv2 = dbg && info.DisableWFPMv2;
            m_EnableCommDlgPMv2 = dbg && info.EnableCommDlgPMv2;
            m_UseClassicTSP = dbg && info.UseClassicTSP;
            m_MainBackdropAcrylic = dbg && info.MainBackdropAcrylic && SystemVersion.IsWindows11;
            m_ImmersiveCountdown = dbg && info.ImmersiveCountdown;
            m_TSFormat = dbg ? info.TSFormat : null;
            m_TSMax = dbg && !m_UseClassicTSP ? info.TSMax : ConfigValidator.MaxTick;
        }
    }
}
