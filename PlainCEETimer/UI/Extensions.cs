using System;
using System.Windows.Forms;

namespace PlainCEETimer.UI
{
    public static class Extensions
    {
        public static void AddControls(this Control ctrl, Func<ControlBuilder, Control[]> builder)
        {
            ctrl.Controls.AddRange(builder(new()));
        }

        public static TControl With<TControl>(this TControl control, Action<TControl> additions)
            where TControl : Control
        {
            additions?.Invoke(control);
            return control;
        }
    }
}
