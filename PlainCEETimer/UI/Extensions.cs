using System;
using System.Windows.Forms;

namespace PlainCEETimer.UI
{
    public static class Extensions
    {
        public static void AddControls(this Control ctrl, Func<ControlBuilder, Control[]> builder)
        {
            var ctrls = builder?.Invoke(new());
            var length = ctrls.Length;

            for (int i = 0; i < length; i++)
            {
                ctrl.Controls.Add(ctrls[i]);
            }
        }

        public static TControl With<TControl>(this TControl control, Action<TControl> additions)
            where TControl : Control
        {
            additions?.Invoke(control);
            return control;
        }
    }
}
