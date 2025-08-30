using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
        var previous = customColors.Copy();
        var result = RunColorDialog(hWndOwner, HookProc, ref color, colors);

        if (result)
        {
            colors.ToArray(customColors);

            if (!customColors.SequenceEqual(previous))
            {
                App.AppConfig.CustomColors = customColors;
            }
        }

        return result;
    }

    [DllImport(App.NativesDll, EntryPoint = "#24")]
    private static extern BOOL RunColorDialog(HWND hWndOwner, WNDPROC lpfnHookProc, ref COLORREF lpColor, CUSTCOLORS lpCustomColors);
}
