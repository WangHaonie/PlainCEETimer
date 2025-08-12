using System.Drawing;

namespace PlainCEETimer.UI
{
    internal readonly struct Colors
    {
        public static readonly Color DarkForeText = Color.White;
        public static readonly Color DarkForeConsole = Color.FromArgb(204, 204, 204);
        public static readonly Color DarkForeLinkNormal = Color.FromArgb(153, 235, 255);
        public static readonly Color DarkForeLinkOnClick = Color.FromArgb(76, 194, 255);
        public static readonly Color DarkForeLinkDisabled = DarkForeConsole;
        public static readonly Color DarkForeListViewHeader = Color.FromArgb(222, 222, 222);
        public static readonly Color DarkBackText = Color.FromArgb(32, 32, 32);
        public static readonly Color DarkBorder = Color.FromArgb(60, 60, 60);
        public static readonly Color LightForeLinkNormal = Color.FromArgb(0, 62, 146);
        public static readonly Color LightForeLinkOnClick = Color.FromArgb(0, 103, 192);
        public static readonly Color LightForeLinkDisabled = Color.FromArgb(109, 109, 109);
        public static readonly Color LightForeListViewHeader = Color.FromArgb(76, 96, 122);
    }
}
