using System;
using System.Windows.Forms;

namespace PlainCEETimer.Modules.WinForms
{
    public static class Extensions
    {
        public static void AddControls(this Control control, Func<ControlBuilder, Control[]> builder)
        {
            control.Controls.AddRange(builder(new()));
        }

        public static TControl With<TControl>(this TControl control, Action<TControl> additions)
            where TControl : Control
        {
            additions?.Invoke(control);
            return control;
        }
    }
}
