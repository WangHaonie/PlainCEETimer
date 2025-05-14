using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

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

        public DialogResult ShowDialog(AppForm owner)
        {
            Helper = new CommonDialogHelper(this, "选择字体 - 高考倒计时", CommonDialogKind.Font, owner);
            return ShowDialog();
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) => Helper.HookProc(hWnd, msg, wparam, lparam);

        IntPtr ICommonDialog.HookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam) => base.HookProc(hWnd, msg, wParam, lParam);
    }
}
