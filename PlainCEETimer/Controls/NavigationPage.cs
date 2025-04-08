using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class NavigationPage : Panel
    {
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

        public NavigationPage()
        {
            BackColor = SystemColors.Window;
            Visible = false;
        }
    }
}
