using PlainCEETimer.Interop;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public class NavigationPage : Panel
    {
        /// <summary>
        /// 初始化新的 <see cref="NavigationPage"/> 实例。
        /// </summary>
        public NavigationPage()
        {
            BackColor = ThemeManager.ShouldUseDarkMode ? ThemeManager.DarkBack : SystemColors.Window;
            Visible = false;
        }
    }
}
