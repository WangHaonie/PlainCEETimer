using System;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.WPF.Models;

namespace PlainCEETimer.Countdown;

public class CountdownManager
{
    public static CountdownManager Instance => field ??= new();

    public ICountdownService CountdownService => field ??= new DefaultCountdownService() { ShouldDispose = false };

    public FontModel CountdownFont => _font;

    internal event Action<FontModel> CountdownFontChanged;

    private FontModel _font;

    private CountdownManager()
    {
        App.Current.AppExit += CountdownService.Destroy;
    }

    internal void SetCountdownFont(FontModel font)
    {
        if (font != null && !font.Equals(_font))
        {
            _font = font;
            CountdownFontChanged?.Invoke(font);
        }
    }
}
