using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainFontDialog(Font existing) : CommonDialog
{
    public Font Font => m_Font;

    private CommonDialogHelper Helper;
    private Font m_Font = existing;

    public DialogResult ShowDialog(AppForm owner)
    {
        try
        {
            Helper = new(this, owner, "选择字体 - 高考倒计时", base.HookProc);
            return base.ShowDialog(owner);
        }
        catch
        {
            return DialogResult.Cancel;
        }
    }

    protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        return Helper.HookProc(hWnd, msg, wparam, lparam);
    }

    protected override bool RunDialog(IntPtr hwndOwner)
    {
        var lf = LOGFONT.FromFont(m_Font);
        var result = (bool)CommonDialogs.RunFontDialog(hwndOwner, ref lf, HookProc, Validator.MinFontSize, Validator.MaxFontSize);

        if (result)
        {
            var font = lf.ToFont();
            m_Font = new(font.FontFamily, font.SizeInPoints, font.Style, GraphicsUnit.Point, font.GdiCharSet, font.GdiVerticalFont);
        }

        return result;
    }

    public override void Reset()
    {
        m_Font = default;
    }
}
