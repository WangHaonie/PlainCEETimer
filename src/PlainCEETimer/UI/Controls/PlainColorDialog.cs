using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainColorDialog : CommonDialog
{
    public Color Color => m_Color;

    public int[] CustomColors
    {
        get
        {
            return (int[])m_CustomColors.Clone();
        }
        set
        {
            int num = (value == null) ? 0 : Math.Min(value.Length, 16);

            if (num > 0)
            {
                Array.Copy(value, 0, m_CustomColors, 0, num);
            }

            for (int i = num; i < 16; i++)
            {
                m_CustomColors[i] = 16777215;
            }
        }
    }

    private CommonDialogHelper Helper;
    private int[] m_CustomColors;
    private Color m_Color;

    public PlainColorDialog()
    {
        m_CustomColors = new int[16];
        CustomColors = App.AppConfig.CustomColors;
    }

    public DialogResult ShowDialog(Color existing, AppForm owner)
    {
        m_Color = existing;
        Helper = new(this, owner, "选取颜色 - 高考倒计时", base.HookProc);
        var previous = CustomColors;
        var result = ShowDialog(owner);

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

    protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        return Helper.HookProc(hWnd, msg, wparam, lparam);
    }

    protected override bool RunDialog(IntPtr hwndOwner)
    {
        var ptr = Marshal.AllocCoTaskMem(64);

        try
        {
            var color = (COLORREF)m_Color;
            var colors = new COLORREFS(m_CustomColors, ptr);
            var result = (bool)CommonDialogs.RunColorDialog(hwndOwner, ref color, HookProc, colors);

            if (result)
            {
                m_Color = color.ToColor();
                colors.ToArray(m_CustomColors);
            }

            return result;
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
    }

    public override void Reset()
    {
        m_Color = Color.Black;
        CustomColors = null;
    }
}
