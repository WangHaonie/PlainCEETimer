using System;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public class ContextMenuBuilder
    {
        public MenuItem AddItem(string Text)
            => new(Text);

        public MenuItem AddItem(string Text, EventHandler OnClickHandler)
            => new(Text, OnClickHandler);

        public MenuItem AddSubMenu(string Text, MenuItem[] Items)
            => new(Text, Items);

        public MenuItem AddSeparator()
            => new("-");

        public static ContextMenu Build(Func<ContextMenuBuilder, MenuItem[]> Builder)
            => new(Builder(new()));

        public static ContextMenu Merge(ContextMenu Target, ContextMenu Reference)
        {
            Target.MergeMenu(Reference);
            return Target;
        }
    }
}
