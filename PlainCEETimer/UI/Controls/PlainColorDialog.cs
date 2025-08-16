using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls
{
    public sealed class PlainColorDialog : ColorDialog
    {
        private static int[] CustomColorCollection = App.AppConfig.CustomColors;
        private int[] PreviousCustomColors;
        private CommonDialogHelper Helper;

        public PlainColorDialog()
        {
            AllowFullOpen = true;
            FullOpen = true;
            CustomColors = CustomColorCollection;
        }

        public DialogResult ShowDialog(Color Default, AppForm owner)
        {
            Color = Default;
            PreviousCustomColors = CustomColorCollection;
            Helper = new(this, owner, "选取颜色 - 高考倒计时", base.HookProc);
            var Result = ShowDialog();

            if (Result == DialogResult.OK)
            {
                CustomColorCollection = CustomColors;
                SaveCustomColors();
            }

            return Result;
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            return Helper.HookProc(hWnd, msg, wparam, lparam);
        }

        private void SaveCustomColors()
        {
            if (CustomColorCollection != null && PreviousCustomColors != null && !CustomColorCollection.SequenceEqual(PreviousCustomColors))
            {
                App.AppConfig.CustomColors = CustomColorCollection;
                App.CanSaveConfig = true;
            }
        }
    }
}
