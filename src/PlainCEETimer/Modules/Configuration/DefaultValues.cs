using System.Drawing;
using System.Linq;
using PlainCEETimer.Countdown;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration;

[NoConstants]
public static class DefaultValues
{
    private const int DefaultColorsCount = 4;

    public static COLORREF[] ColorDialogColors => field ??= [.. Enumerable.Repeat((COLORREF)COLORREF.EmptyValue, COLORREF.LPCUSTCOLORS_Length)];

    public static ColorPair GlobalDefaultColor => globalDefaultColor;

    public static ColorPair[] LightColors => lightColors;

    public static ColorPair[] DarkColors => darkColors;

    public static CountdownRule[] GlobalDefaultRules => globalDefaultRules;

    public static Font CountdownDefaultFont
    {
        get
        {
            if (field == null)
            {
                const string NotoSansSC = "Noto Sans SC";
                const string MicrosoftYaHei = "Microsoft YaHei";

                /*
                
                测试指定字体是否安装 参考：

                .net - Test if a Font is installed - Stack Overflow
                https://stackoverflow.com/a/114003/21094697

                */

                using var tester = new Font(NotoSansSC, 1);
                field = new(tester.Name == NotoSansSC ? NotoSansSC : MicrosoftYaHei, 18F, FontStyle.Bold, GraphicsUnit.Point);
            }

            return field;
        }
    }

    private static ColorPair globalDefaultColor;
    private static CountdownRule[] globalDefaultRules;

    private static readonly ColorPair[] lightColors =
    [
        new(Color.Red, Color.White),
        new(Color.Green, Color.White),
        new(Color.Blue, Color.White),
        new(Color.Black, Color.White)
    ];

    private static readonly ColorPair[] darkColors =
    [
        new(Color.Red, Color.Black),
        new(Color.Lime, Color.Black),
        new(Color.Aqua, Color.Black),
        new(Color.White, Color.Black)
    ];

    public static void InitEssentials()
    {
        var config = App.Current.AppConfig;

        if (config == null)
        {
            return;
        }

        var colors = config.DefaultColors;

        if (colors == null || colors.Length < DefaultColorsCount)
        {
            config.DefaultColors = CreateDefaultColors(colors);
            return;
        }

        ApplyDefaultColors(colors);
    }

    private static ColorPair[] CreateDefaultColors(ColorPair[] colors)
    {
        var defaults = ThemeManager.ShouldUseDarkMode ? darkColors : lightColors;
        return defaults.Copy().PopulateWith(colors);
    }

    private static void ApplyDefaultColors(ColorPair[] colors)
    {
        globalDefaultColor = colors[DefaultColorsCount - 1];

        globalDefaultRules =
        [
            new()
            {
                Default = true,
                Phase = CountdownPhase.P1,
                Colors = colors[0],
                Text = Ph.P1
            },
            new()
            {
                Default = true,
                Phase = CountdownPhase.P2,
                Colors = colors[1],
                Text = Ph.P2
            },
            new()
            {
                Default = true,
                Phase = CountdownPhase.P3,
                Colors = colors[2],
                Text = Ph.P3
            }
        ];
    }
}
