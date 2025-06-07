using System;
using System.Windows.Forms;
using PlainCEETimer.Controls;

namespace PlainCEETimer.Modules.WinForms
{
    public static class Extensions
    {
        public static void AddControls(this AppForm form, Func<ControlBuilder, Control[]> builder)
        {
            form.Controls.AddRange(builder(new()));
        }

        public static TControl With<TControl>(this TControl control, Action<TControl> additions)
            where TControl : Control
        {
            additions?.Invoke(control);
            return control;
        }
    }
}
