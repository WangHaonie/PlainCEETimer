using System;
using PlainCEETimer.Modules.Annotations.SourceGenerators;

namespace PlainCEETimer.UI.Core;

public sealed partial class WindowVisibilityController(IWindowStyles styles, Action onVisibilityChanged)
{
    [BackingField("_hidden")]
    public partial bool Hidden { get; }

    public void Show(bool activate)
    {
        _hidden = false;
        styles.ShowActivated(activate);
        onVisibilityChanged();
    }

    public void Hide()
    {
        _hidden = true;
        styles.Visible = false;
        onVisibilityChanged();
    }

    public void Toggle()
    {
        if (_hidden)
            Show(true);
        else
            Hide();
    }
}
