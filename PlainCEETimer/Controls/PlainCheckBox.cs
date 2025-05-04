using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Controls
{
    public sealed class PlainCheckBox : CheckBox
    {
        public PlainCheckBox() => new PlainButtonBase(this);
    }
}
