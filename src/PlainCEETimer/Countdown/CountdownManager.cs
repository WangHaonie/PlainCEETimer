using System;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.SourceGenerators;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.WPF.Models;

namespace PlainCEETimer.Countdown;

public partial class CountdownManager
{
    public static CountdownManager Instance => field ??= new();

    public ICountdownService CountdownService => field ??= new DefaultCountdownService() { ShouldDispose = false };

    [BackingField("_font")]
    public partial FontModel CountdownFont { get; }

    internal event Action<FontModel> CountdownFontChanged;

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
