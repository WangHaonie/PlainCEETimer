using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls
{
    public sealed class FontDialogEx : FontDialog
    {
        private CommonDialogHelper Helper;

        public FontDialogEx(Font font)
        {
            AllowVerticalFonts = false;
            Font = font;
            FontMustExist = true;
            MinSize = Validator.MinFontSize;
            MaxSize = Validator.MaxFontSize;
            ScriptsOnly = true;
            ShowEffects = false;
        }

        public DialogResult ShowDialog(AppForm owner)
        {
            Helper = new CommonDialogHelper(this, owner, "选择字体 - 高考倒计时", base.HookProc);
            return ShowDialog();
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            return Helper.HookProc(hWnd, msg, wparam, lparam);
        }
    }
}
