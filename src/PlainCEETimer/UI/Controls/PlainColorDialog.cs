using System;
using System.Drawing;
using System.Linq;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Extensions.Linq;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainColorDialog : PlainCommonDialog
{
    public Color Color => color.ToColor();

    private COLORREF color;
    private readonly int[] customColors;

    public PlainColorDialog(AppForm owner, Color existing) : base(owner, "选取颜色 - 高考倒计时")
    {
        color = existing;
        customColors = DefaultValues.ColorDialogColors.Copy();
        customColors.PopulateWith(App.AppConfig.CustomColors);
    }

    protected override bool StartDialog(IntPtr hWndOwner)
    {
        using LPCUSTCOLORS lpColors = new(customColors);
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

                Validator.DemandConfig();
            }
        }

        return result;
    }
}
