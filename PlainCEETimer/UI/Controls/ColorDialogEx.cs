using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls
{
    public sealed class ColorDialogEx : ColorDialog, ICommonDialog
    {
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
            Helper = new CommonDialogHelper(this, "选取颜色 - 高考倒计时", CommonDialogKind.Color, owner);
            var Result = ShowDialog();

            if (Result == DialogResult.OK)
            {
                CustomColorCollection = CustomColors;
                SaveCustomColors();
            }

            return Result;
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) => Helper.HookProc(hWnd, msg, wparam, lparam);

        IntPtr ICommonDialog.HookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam) => base.HookProc(hWnd, msg, wParam, lParam);

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
