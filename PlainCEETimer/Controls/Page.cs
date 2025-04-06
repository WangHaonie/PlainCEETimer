using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class Page : Panel
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

        public Page()
        {
            BackColor = SystemColors.Window;
        }
    }
}
