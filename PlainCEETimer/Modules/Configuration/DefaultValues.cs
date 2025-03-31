using PlainCEETimer.Interop;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PlainCEETimer.Modules.Configuration
{
    public static class DefaultValues
    {
        public static bool IsDarkModeSupported { get; }
        public static int[] ColorDialogColors { get; }
        public static ColorSetObject[] CountdownDefaultColorsDark { get; }
        public static ColorSetObject[] CountdownDefaultColorsLight { get; }
        public static Font CountdownDefaultFont { get; }

        public static readonly int Initialize;
        private static readonly string NotoSansSC = "Noto Sans SC";
        private static readonly string MicrosoftYaHei = "Microsoft YaHei";

        static DefaultValues()
        {
            var FontTester = new Font(NotoSansSC, 1);
            bool NotoAvailable = false;

            if (FontTester.Name == NotoSansSC)
            {
                NotoAvailable = true;
            }

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

            CountdownDefaultFont = new(NotoAvailable ? NotoSansSC : MicrosoftYaHei, 17.25F, FontStyle.Bold, GraphicsUnit.Point);
            ColorDialogColors = [.. Enumerable.Repeat(16777215, 16)];
            IsDarkModeSupported = !File.Exists(App.ConfigFilePath)
                && App.OSBuild >= WindowsBuilds.Windows10_1903
                && NativeInterop.ShouldAppsUseDarkMode();
        }
    }
}
