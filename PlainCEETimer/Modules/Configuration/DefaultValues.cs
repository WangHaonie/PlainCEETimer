using PlainCEETimer.Interop;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PlainCEETimer.Modules.Configuration
{
    public static class DefaultValues
    {
        public static bool AutoDarkCountdown { get; }
        public static int[] ColorDialogColors { get; }
        public static ColorSetObject[] CountdownDefaultColorsDark { get; }
        public static ColorSetObject[] CountdownDefaultColorsLight { get; }
        public static Font CountdownDefaultFont { get; }

        private static readonly string NotoSansSC = "Noto Sans SC";
        private static readonly string MicrosoftYaHei = "Microsoft YaHei";

        static DefaultValues()
        {
            var FontTester = new Font(NotoSansSC, 1);
            CountdownDefaultFont = new(FontTester.Name == NotoSansSC ? NotoSansSC : MicrosoftYaHei, 17.25F, FontStyle.Bold, GraphicsUnit.Point);
            FontTester.Dispose();

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

            ColorDialogColors = [.. Enumerable.Repeat(16777215, 16)];
            AutoDarkCountdown = !File.Exists(App.ConfigFilePath) && ThemeManager.CurrentTheme != SystemTheme.None;
        }
    }
}
