using System.Drawing;
using System.Linq;
using PlainCEETimer.Countdown;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration;

public static class DefaultValues
{
    public static int[] ColorDialogColors => field ??= [.. Enumerable.Repeat(COLORREF.EmptyValue, 16)];

    public static ColorPair GlobalDefaultColor
    {
        get
        {
            if (true)
            {
                field = _defaultColors[3];
            }

            return field;
        }
    }

    public static ColorPair[] LightColors => _lightColors;

    public static ColorPair[] DarkColors => _darkColors;

    public static CountdownRule[] GlobalDefaultRules
    {
        get
        {
            if (canUpdate)
            {
                field =
                [
                    new()
                    {
                        IsDefault = true,
                        Phase = CountdownPhase.P1,
                        Colors = _defaultColors[0],
                        Text = Ph.P1
                    },
                    new()
                    {
                        IsDefault = true,
                        Phase = CountdownPhase.P2,
                        Colors = _defaultColors[1],
                        Text = Ph.P2
                    },
                    new()
                    {
                        IsDefault = true,
                        Phase = CountdownPhase.P3,
                        Colors = _defaultColors[2],
                        Text = Ph.P3
                    }
                ];
            }

            return field;
        }
    }

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

    private static bool canUpdate;
    private static ColorPair[] _defaultColors;
    private static ColorPair[] _lightColors;
    private static ColorPair[] _darkColors;

    public static void InitEssentials(bool isAppInit)
    {
        var a = App.AppConfig;

        if (a != null)
        {
            var arr = a.DefaultColors;

            if (isAppInit)
            {
                _lightColors =
                [
                    new(Color.Red, Color.White),
                    new(Color.Green, Color.White),
                    new(Color.Blue, Color.White),
                    new(Color.Black, Color.White)
                ];

                _darkColors =
                [
                    new(Color.Red, Color.Black),
                    new(Color.Lime, Color.Black),
                    new(Color.Aqua, Color.Black),
                    new(Color.White, Color.Black)
                ];

                var dark = ThemeManager.ShouldUseDarkMode;
                var canInitAutoColors = arr == null || arr.Length < 4;

                if (canInitAutoColors)
                {
                    _defaultColors = dark ? _darkColors : _lightColors;
                    a.DefaultColors = _defaultColors.Copy().PopulateWith(arr);
                    ConfigValidator.DemandConfig();
                    return;
                }
            }

            _defaultColors = arr;
            canUpdate = true;
        }
    }
}
