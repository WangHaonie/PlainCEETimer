using System.Drawing;
using System.IO;
using System.Linq;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules.Configuration
{
    public static class DefaultValues
    {
        public static bool AutoDarkTheme { get; }
        public static int[] ColorDialogColors { get; }
        public static string[] GlobalDefaultCustomTexts { get; }
        public static ColorSetObject[] CountdownDefaultColorsDark { get; }
        public static ColorSetObject[] CountdownDefaultColorsLight { get; }
        public static Font CountdownDefaultFont { get; }

        private static readonly string NotoSansSC = "Noto Sans SC";
        private static readonly string MicrosoftYaHei = "Microsoft YaHei";

        static DefaultValues()
        {
            var tester = new Font(NotoSansSC, 1);
            CountdownDefaultFont = new(tester.Name == NotoSansSC ? NotoSansSC : MicrosoftYaHei, 17.25F, FontStyle.Bold, GraphicsUnit.Point);
            tester.Dispose();

            CountdownDefaultColorsLight =
            [
                new(Color.Red, Color.White),
                new(Color.Green, Color.White),
                new(Color.Blue, Color.White),
                new(Color.Black, Color.White)
            ];

            CountdownDefaultColorsDark =
            [
                new(Color.Red, Color.Black),
                new(Color.Lime, Color.Black),
                new(Color.Aqua, Color.Black),
                new(Color.White, Color.Black)
            ];

            GlobalDefaultCustomTexts = [Constants.PH_P1, Constants.PH_P2, Constants.PH_P3];
            ColorDialogColors = [.. Enumerable.Repeat(16777215, 16)];
            var theme = ThemeManager.CurrentTheme;

            if (!File.Exists(App.ConfigFilePath) && theme != SystemTheme.None)
            {
                AutoDarkTheme = theme == SystemTheme.Dark;
            }
        }
    }
}
