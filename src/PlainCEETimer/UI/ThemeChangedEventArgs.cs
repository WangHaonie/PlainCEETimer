using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public class ThemeChangedEventArgs(SystemTheme theme)
{
    public SystemTheme Theme => theme;
}