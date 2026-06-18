using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI;

public class ControlDebounceHelper(Control control) : IDebounceState
{
    private AppForm parentForm;

    public bool ShouldDebounce => (parentForm ??= control.FindParentForm())?.Loaded == true;
}
