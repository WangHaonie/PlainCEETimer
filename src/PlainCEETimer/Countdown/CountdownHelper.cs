using System;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.WPF.Models;

namespace PlainCEETimer.Countdown;

public class CountdownHelper
{
    public static CountdownHelper Instance => field ??= new();

    private CountdownHelper()
    {
        App.Current.AppExit += CountdownService.Destroy;
    }

    public ICountdownService CountdownService { get; } = new DefaultCountdownService();

    public FontModel CountdownFont { get; private set; }

    internal event Action<FontModel> CountdownFontChanged;

    internal void SetCountdownFont(FontModel font)
    {
        if (font != null)
        {
            CountdownFont = font;
            CountdownFontChanged?.Invoke(font);
        }
    }
}
