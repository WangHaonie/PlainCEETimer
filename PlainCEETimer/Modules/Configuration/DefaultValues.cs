using System.Drawing;
using System.IO;
using System.Linq;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Modules.Configuration
{
    public static class DefaultValues
    {
        public static int[] ColorDialogColors { get; }
        public static string[] GlobalDefaultCustomTexts { get; }
        public static ColorSetObject[] CountdownDefaultColors { get; }
        public static ColorSetObject[] CountdownDefaultColorsDark { get; }
        public static ColorSetObject[] CountdownDefaultColorsLight { get; }
        public static Font CountdownDefaultFont { get; }

        private const string NotoSansSC = "Noto Sans SC";
        private const string MicrosoftYaHei = "Microsoft YaHei";

        static DefaultValues()
        {
            GlobalDefaultCustomTexts = [.. Constants.PhAllPhases];
            ColorDialogColors = [.. Enumerable.Repeat(16777215, 16)];

            var tester = new Font(NotoSansSC, 1);
            CountdownDefaultFont = new(tester.Name == NotoSansSC ? NotoSansSC : MicrosoftYaHei, 18F, FontStyle.Bold, GraphicsUnit.Point);
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

            var theme = ThemeManager.CurrentTheme;
            var dark = false;

            if (!File.Exists(App.ConfigFilePath) && theme != SystemTheme.None)
            {
                dark = theme == SystemTheme.Dark;
            }

            CountdownDefaultColors = dark ? CountdownDefaultColorsDark : CountdownDefaultColorsLight;
        }
    }
}
