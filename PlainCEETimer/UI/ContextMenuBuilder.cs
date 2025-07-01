using System;
using System.Windows.Forms;

namespace PlainCEETimer.UI
{
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

        public static ContextMenu Build(Func<ContextMenuBuilder, MenuItem[]> builder)
        {
            return new(builder(new()));
        }

        public static ContextMenu Build(Func<ContextMenuBuilder, MenuItem[]> builder, EventHandler onPopup)
        {
            var menu = Build(builder);
            menu.Popup += onPopup;
            return menu;
        }

        public static ContextMenu Merge(ContextMenu target, ContextMenu reference)
        {
            target.MergeMenu(reference);
            return target;
        }
    }
}
