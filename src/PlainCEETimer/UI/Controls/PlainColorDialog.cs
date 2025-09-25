using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;

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

    protected override BOOL RunDialog(HWND hWndOwner)
    {
        using var colors = new CUSTCOLORS(customColors);
        var result = RunColorDialog(hWndOwner, HookProc, ref color, colors);

        if (result)
        {
            var previous = customColors.Copy();
            colors.ToArray(customColors);

            if (!customColors.SequenceEqual(previous))
            {
                App.AppConfig.CustomColors = RemoveEmptyColors(customColors);
                Validator.DemandConfig();
            }
        }

        return result;
    }

    private int[] RemoveEmptyColors(int[] colors)
    {
        List<int> tmp = [];

        foreach (var c in colors)
        {
            if (c != COLORREF.EmptyValue)
            {
                tmp.Add(c);
            }
        }

        return [.. tmp];
    }

    [DllImport(App.NativesDll, EntryPoint = "#10")]
    private static extern BOOL RunColorDialog(HWND hWndOwner, WNDPROC lpfnHookProc, ref COLORREF lpColor, CUSTCOLORS lpCustomColors);
}
