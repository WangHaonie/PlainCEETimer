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
    public Color Color => (Color)color;

    private readonly COLORREF color = existing;

    private readonly COLORREF[] customColors = DefaultValues.ColorDialogColors
        .Copy().PopulateWith(App.AppConfig.CustomColors);

    protected unsafe override bool StartDialog(IntPtr hWndOwner)
    {
        fixed (COLORREF* lpColor = &color)
        fixed (COLORREF* lpColors = customColors)
        {
            var result = Win32UI.RunColorDialog(hWndOwner, HookProc, lpColor, lpColors);

            if (result)
            {
                if (!customColors.AsSpan().SequenceEqual(new ReadOnlySpan<COLORREF>(lpColor, COLORREF.LPCUSTCOLORS_Length)))
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
}
