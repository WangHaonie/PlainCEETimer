using System.Drawing;
using System.IO;
using System.Linq;
using PlainCEETimer.Countdown;
using PlainCEETimer.Interop;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration;

public static class DefaultValues
{
    public static int[] ColorDialogColors => field ??= [.. Enumerable.Repeat(COLORREF.EmptyValue, 16)];

    public static string[] GlobalDefaultCustomTexts => field ??= [Ph.P1, Ph.P2, Ph.P3];

    public static ColorPair[] CountdownDefaultColors
    {
        get
        {
            if (field == null)
            {
                var theme = ThemeManager.CurrentTheme;
                var dark = false;

                if (!File.Exists(App.ConfigFilePath) && theme != SystemTheme.None)
                {
                    dark = theme == SystemTheme.Dark;
                }

                field = dark ? CountdownDefaultColorsDark : CountdownDefaultColorsLight;
            }

            return field;
        }
    }

    public static ColorPair[] CountdownDefaultColorsDark => field ??=
    [
        new(Color.Red, Color.Black),
        new(Color.Lime, Color.Black),
        new(Color.Aqua, Color.Black),
        new(Color.White, Color.Black)
    ];

    public static ColorPair[] CountdownDefaultColorsLight => field ??=
    [
        new(Color.Red, Color.White),
        new(Color.Green, Color.White),
        new(Color.Blue, Color.White),
        new(Color.Black, Color.White)
    ];

    public static Font CountdownDefaultFont
    {
        get
        {
            if (field == null)
            {
                const string NotoSansSC = "Noto Sans SC";
                const string MicrosoftYaHei = "Microsoft YaHei";

                using var tester = new Font(NotoSansSC, 1);
                field = new(tester.Name == NotoSansSC ? NotoSansSC : MicrosoftYaHei, 18F, FontStyle.Bold, GraphicsUnit.Point);
            }

            return field;
        }
    }
}
