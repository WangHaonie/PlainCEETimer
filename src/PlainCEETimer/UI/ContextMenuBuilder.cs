using System;
using System.Windows.Forms;

namespace PlainCEETimer.UI;

public class ContextMenuBuilder
{
    public MenuItem Item(string text)
    {
        return new(text);
    }

    public MenuItem Item(string text, EventHandler onClickHandler)
    {
        return new(text, onClickHandler);
    }

    public MenuItem Menu(string text, MenuItem[] items)
    {
        return new(text, items);
    }

    public MenuItem Separator()
    {
        return new("-");
    }
}
