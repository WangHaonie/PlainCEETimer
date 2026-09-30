using System;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.SourceGenerators;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.WPF.Models;

namespace PlainCEETimer.Countdown;

public partial class CountdownManager
{
    public ICountdownService CountdownService => CountdownServiceImpl;

    [BackingField("_font")]
    public partial FontModel CountdownFont { get; }

    public static CountdownManager Instance => field ??= new();

    internal event Action<FontModel> CountdownFontChanged;

    private readonly DefaultCountdownService CountdownServiceImpl;

    private CountdownManager()
    {
        CountdownServiceImpl = new() { ShouldDispose = false };
        App.Current.AppExit += CountdownServiceImpl.Destroy;
    }

    internal void SetCountdownFont(FontModel font)
    {
        if (font != null && !font.Equals(_font))
        {
            _font = font;
            CountdownFontChanged?.Invoke(font);
        }
    }

    internal static void EnsureService()
    {
        Instance.CountdownServiceImpl.Start(CountdownStartInfo.FromConfig(App.Current.AppConfig));
    }
}
