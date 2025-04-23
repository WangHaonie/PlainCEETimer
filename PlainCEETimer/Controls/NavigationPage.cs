using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.Controls
{
    public class NavigationPage : Panel
    {
        /// <summary>
        /// 该页面关联的索引。此项不应手动设置。
        /// </summary>
        public int Index { get; set; }

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
