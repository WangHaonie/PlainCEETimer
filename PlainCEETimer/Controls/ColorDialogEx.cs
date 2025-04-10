using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class ColorDialogEx : ColorDialog, ICommDlg
    {
        public CommDlg DlgType => CommDlg.Color;
        public string DialogTitle => "选取颜色 - 高考倒计时";

        private static int[] CustomColorCollection = App.AppConfig.CustomColors;
        private int[] PreviousCustomColors;
        private CommonDialogHelper Helper;

        public ColorDialogEx()
        {
            AllowFullOpen = true;
            FullOpen = true;
            CustomColors = CustomColorCollection;
        }

        public DialogResult ShowDialog(Color Default, AppForm owner)
        {
            Color = Default;
            PreviousCustomColors = CustomColorCollection;
            Helper = new CommonDialogHelper(owner);
            var Result = ShowDialog();

            if (Result == DialogResult.OK)
            {
                CustomColorCollection = CustomColors;
                SaveCustomColors();
            }

            return Result;
        }

        public IntPtr BaseHookProc(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam)
        {
            return base.HookProc(hWnd, Msg, wParam, lParam);
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            return Helper.HookProc(this, hWnd, msg, wparam, lparam);
        }

        private void SaveCustomColors()
        {
            if (CustomColorCollection != null && PreviousCustomColors != null && !CustomColorCollection.SequenceEqual(PreviousCustomColors))
            {
                var ExistingConfig = App.AppConfig;
                ExistingConfig.CustomColors = CustomColorCollection;
                App.AppConfig = ExistingConfig;
            }
        }
    }
}
