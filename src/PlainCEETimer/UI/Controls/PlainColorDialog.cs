using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainColorDialog : PlainCommonDialog
{
    public int[] CustomColors => (int[])customColors.Clone();
    public Color Color => color;

    private Color color;
    private readonly int[] customColors = App.AppConfig.CustomColors;

    public PlainColorDialog(AppForm owner, Color existing)
    {
        Parent = owner;
        color = existing;
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
        var ptr = Marshal.AllocCoTaskMem(64);

        try
        {
            var color = (COLORREF)this.color;
            var colors = new COLORREFS(customColors, ptr);
            var result = CommonDialogs.RunColorDialog(hWndOwner, DialogHook, ref color, colors);

            if (result)
            {
                this.color = color.ToColor();
                colors.ToArray(customColors);
            }

            return result;
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
    }
}
