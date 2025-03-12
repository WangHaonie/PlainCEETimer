using PlainCEETimer.Interop;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class Hyperlink : LinkLabel
    {
        [Category("Appearance")]
        public string HyperLink { get; set; }

        public Hyperlink()
        {
            ThemeManager.SystemThemeChanged += ThemeManager_SystemThemeChanged;

            if (ThemeManager.AllowDarkMode && ThemeManager.IsDarkMode)
            {
                LinkColor = ThemeManager.DarkLinkColor;
            }
        }

        private void ThemeManager_SystemThemeChanged(object sender, EventArgs e)
        {
            if (ThemeManager.IsDarkMode)
            {
                LinkColor = ThemeManager.DarkLinkColor;
            }
            else
            {
                LinkColor = ThemeManager.LightLinkColor;
            }
        }

        protected sealed override void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(HyperLink);
            base.OnLinkClicked(e);
        }
    }
}
