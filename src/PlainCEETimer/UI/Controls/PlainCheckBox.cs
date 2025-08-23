using System.Windows.Forms;

namespace PlainCEETimer.UI.Controls
{
    public sealed class PlainCheckBox : CheckBox
    {
        public PlainCheckBox()
        {
            new PlainButtonBase(this);
        }
    }
}
