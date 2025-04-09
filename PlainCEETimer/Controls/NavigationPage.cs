using PlainCEETimer.Interop;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class NavigationPage : Panel
    {
        /// <summary>
        /// 该页面关联的索引。此项不应手动设置。
        /// </summary>
        public int Index
        {
            get;
            set
            {
                if (value > -1)
                {
                    field = value;
                }
            }
        }

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
