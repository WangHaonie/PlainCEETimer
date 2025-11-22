using System.Drawing;
using System.Linq;
using PlainCEETimer.Countdown;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration;

public static class DefaultValues
{
    public static int[] ColorDialogColors => field ??= [.. Enumerable.Repeat(COLORREF.EmptyValue, 16)];

    public static CustomRule[] GlobalDefaultRules
    {
        get
        {
            if (field == null)
            {
                var flag = ThemeManager.ShouldUseDarkMode;

                field =
                [
                    new()
                    {
                        IsDefault = true,
                        Phase = CountdownPhase.P1,
                        Colors = flag ? new(Color.Red, Color.Black) : new(Color.Red, Color.White),
                        Text = Ph.P1
                    },
                    new()
                    {
                        IsDefault = true,
                        Phase = CountdownPhase.P2,
                        Colors = flag ? new(Color.Lime, Color.Black) : new(Color.Green, Color.White),
                        Text = Ph.P2
                    },
                    new()
                    {
                        IsDefault = true,
                        Phase = CountdownPhase.P3,
                        Colors = flag ? new(Color.Aqua, Color.Black) : new(Color.Blue, Color.White),
                        Text = Ph.P3
                    }
                ];
            }

            return field.Copy();
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

                using var tester = new Font(NotoSansSC, 1);
                field = new(tester.Name == NotoSansSC ? NotoSansSC : MicrosoftYaHei, 18F, FontStyle.Bold, GraphicsUnit.Point);
            }

            return field;
        }
    }
}
