using System.Drawing;
using System.Linq;
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

    public PlainColorDialog(AppForm owner, Color existing)
    {
        color = existing;
        Parent = owner;
        Text = "选取颜色 - 高考倒计时";
        customColors = DefaultValues.ColorDialogColors.Copy();
        customColors.PopulateWith(App.AppConfig.CustomColors);
    }

    public override DialogResult Show()
    {
        var previous = customColors.Copy();
        var result = base.Show();

        if (result == DialogResult.OK)
        {
            var tmp = customColors;

            if (!tmp.SequenceEqual(previous))
            {
                App.AppConfig.CustomColors = tmp;
            }
        }

        return result;
    }

    protected override BOOL RunDialog(HWND hWndOwner)
    {
        using var colors = new CUSTCOLORS(customColors);
        var result = CommonDialogs.RunColorDialog(hWndOwner, HookProc, ref color, colors);

        if (result)
        {
            colors.ToArray(customColors);
        }

        return result;
    }
}
