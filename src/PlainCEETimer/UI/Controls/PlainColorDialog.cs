using System;
using System.Drawing;
using System.Linq;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainColorDialog(AppForm owner, Color existing) : PlainCommonDialog(owner, "选取颜色 - 高考倒计时")
{
    public Color Color => color.ToColor();

    private COLORREF color = existing;

    private readonly int[] customColors = DefaultValues.ColorDialogColors
        .Copy().PopulateWith(App.AppConfig.CustomColors);

    protected override bool StartDialog(IntPtr hWndOwner)
    {
        using var lpColors = (LPCUSTCOLORS)customColors;
        var result = Win32UI.RunColorDialog(hWndOwner, HookProc, ref color, lpColors);

        if (result)
        {
            var previous = customColors.Copy();
            lpColors.Populate(customColors);

            if (!customColors.ArrayEquals(previous))
            {
                App.AppConfig.CustomColors =
                [..
                    customColors
                    .ArrayWhere(c => c != COLORREF.EmptyValue)
                    .Distinct()
                ];

                ConfigValidator.DemandConfig();
            }
        }

        return result;
    }
}
