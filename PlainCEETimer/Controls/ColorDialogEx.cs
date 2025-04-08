using PlainCEETimer.Modules;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class ColorDialogEx : ColorDialog
    {
        private static int[] CustomColorCollection = App.AppConfig.CustomColors;
        private int[] PreviousCustomColors;

        public ColorDialogEx()
        {
            AllowFullOpen = true;
            FullOpen = true;
            CustomColors = CustomColorCollection;
        }

        public DialogResult ShowDialog(Color Default)
        {
            Color = Default;
            PreviousCustomColors = CustomColorCollection;
            var Result = ShowDialog();

            if (Result == DialogResult.OK)
            {
                CustomColorCollection = CustomColors;
                SaveCustomColors();
            }

            return Result;
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
