using System.Windows;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules;

[NoConstants]
public static class SystemSettings
{
    private unsafe sealed class SystemSettingsMessageFilter : IAppMessageFilter
    {
        private readonly Throttler thr = new();

        public bool OnMessage(MSG* lpMsg)
        {
            if (IsRelevantMessage(lpMsg) && thr.Throttle())
            {
                RefreshAll();
            }

            return false;
        }

        private static bool IsRelevantMessage(MSG* lpMsg)
        {
            return lpMsg->message switch
            {
                WinUser.WM_SYSCOLORCHANGE
                    => true,
                WinUser.WM_SETTINGCHANGE or WinUser.WM_SETTINGCHANGE + WinUser.WM_REFLECT
                    => true,
                _
                    => false,
            };
        }
    }

    public static SystemSetting<bool> AppsUseLightTheme { get; }

    public static SystemSetting<bool> EnableTransparency { get; }

    public static SystemSetting<bool> ClientAreaAnimation { get; }

    private static readonly ISystemSetting[] All;

    private static SystemSettingsMessageFilter ssmf;

    private const string PersonalizeRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    static SystemSettings()
    {
        All =
        [
            AppsUseLightTheme = new(() =>
            {
                using var reg = RegistryHelper.Open(PersonalizeRegPath);
                return reg.Check(nameof(AppsUseLightTheme), 1, 1);
            }),

            EnableTransparency = new(() =>
            {
                using var reg = RegistryHelper.Open(PersonalizeRegPath);
                return reg.Check(nameof(EnableTransparency), 1, 0);
            }),

            ClientAreaAnimation = new(() => SystemParameters.ClientAreaAnimation)
        ];
    }

    public static void Initialize()
    {
        if (ssmf == null)
        {
            ssmf = new();
            AppMessageFilter.AddMessageFilter(ssmf);
        }
    }

    private static void RefreshAll()
    {
        foreach (var s in All)
        {
            s.Refresh();
        }
    }
}
