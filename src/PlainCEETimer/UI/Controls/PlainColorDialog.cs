using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainColorDialog : PlainCommonDialog
{
    public int[] CustomColors => (int[])customColors.Clone();
    public Color Color => color.ToColor();

    private COLORREF color;
    private readonly int[] customColors = App.AppConfig.CustomColors;

    public PlainColorDialog(AppForm owner, Color existing)
    {
        color = existing;
        Parent = owner;
        Text = "选取颜色 - 高考倒计时";
    }

    public override DialogResult Show()
    {
        var previous = CustomColors;
        var result = base.Show();

        if (result == DialogResult.OK)
        {
            var tmp = CustomColors;

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
        var result = CommonDialogs.RunColorDialog(hWndOwner, DialogHook, ref color, colors);

        if (result)
        {
            colors.ToArray(customColors);
        }

        return result;
    }
}
