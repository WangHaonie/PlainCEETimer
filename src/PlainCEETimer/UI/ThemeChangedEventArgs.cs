using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public class ThemeChangedEventArgs(SystemTheme theme)
{
    public SystemTheme Theme => theme;

    public bool UseDark { get; } = theme == SystemTheme.Dark;
}