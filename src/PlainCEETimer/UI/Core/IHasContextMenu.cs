using System.Windows.Forms;

namespace PlainCEETimer.UI.Core;

public interface IHasContextMenu
{
    ContextMenu ContextMenu { get; set; }
}