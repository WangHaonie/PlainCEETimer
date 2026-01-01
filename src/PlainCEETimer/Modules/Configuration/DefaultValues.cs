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
                field = m_defaultcolors[3];
            }

            return field;
        }
    }

    public static ColorPair[] LightColors => m_lightcolor;

    public static ColorPair[] DarkColors => m_darkcolor;

    public static CountdownRule[] GlobalDefaultRules
    {
        get
        {
            if (m_update)
            {
                field =
                [
                    new()
                    {
                        IsDefault = true,
                        Phase = CountdownPhase.P1,
                        Colors = m_defaultcolors[0],
                        Text = Ph.P1
                    },
                    new()
                    {
                        IsDefault = true,
                        Phase = CountdownPhase.P2,
                        Colors = m_defaultcolors[1],
                        Text = Ph.P2
                    },
                    new()
                    {
                        IsDefault = true,
                        Phase = CountdownPhase.P3,
                        Colors = m_defaultcolors[2],
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

    private static bool m_update;
    private static ColorPair[] m_defaultcolors;
    private static ColorPair[] m_lightcolor;
    private static ColorPair[] m_darkcolor;

    public static void InitEssentials(bool isAppInit)
    {
        var a = App.AppConfig;
        
        if (a != null)
        {
            var arr = a.DefaultColors;

            if (isAppInit)
            {
                m_lightcolor =
                [
                    new(Color.Red, Color.White),
                    new(Color.Green, Color.White),
                    new(Color.Blue, Color.White),
                    new(Color.Black, Color.White)
                ];

                m_darkcolor =
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
                    m_defaultcolors = dark ? m_darkcolor : m_lightcolor;
                    a.DefaultColors = m_defaultcolors.Copy().PopulateWith(arr);
                    Validator.DemandConfig();
                    return;
                }
            }

            m_defaultcolors = arr;
            m_update = true;
        }
    }
}
