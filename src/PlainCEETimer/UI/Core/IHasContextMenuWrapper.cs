using System.Windows.Forms;

namespace PlainCEETimer.UI.Core;

public class IHasContextMenuWrapper<TControl>(TControl control) : IHasContextMenu
    where TControl : Control
{
    public TControl Target => control;

    public ContextMenu ContextMenu
    {
        get => control.ContextMenu;
        set => control.ContextMenu = value;
    }
}