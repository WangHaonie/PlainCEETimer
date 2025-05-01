using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class FontDialogEx : FontDialog, ICommonDialog
    {
        private CommonDialogHelper Helper;

        public FontDialogEx(Font font)
        {
            AllowScriptChange = true;
            AllowVerticalFonts = false;
            Font = font;
            FontMustExist = true;
            MinSize = Validator.MinFontSize;
            MaxSize = Validator.MaxFontSize;
            ScriptsOnly = true;
            ShowColor = false;
            ShowEffects = false;
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            return Helper.HookProc(this, hWnd, msg, wparam, lparam);
        }

        public DialogResult ShowDialog(AppForm owner)
        {
            Helper = new CommonDialogHelper("选择字体 - 高考倒计时", CommonDialogKind.Font, owner);
            return ShowDialog();
        }

        IntPtr ICommonDialog.HookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            return base.HookProc(hWnd, msg, wParam, lParam);
        }
    }
}
