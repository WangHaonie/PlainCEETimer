using PlainCEETimer.Interop;
using System;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class FontDialogEx : FontDialog
    {
        private CommonDialogHelper Helper;

        public DialogResult ShowDialog(AppForm owner)
        {
            Helper = new CommonDialogHelper(owner);
            return ShowDialog();
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            return Helper.HookProc(hWnd, msg, wparam, lparam);
        }
    }
}
