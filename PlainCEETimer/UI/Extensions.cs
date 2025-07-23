using System;
using System.Windows.Forms;

namespace PlainCEETimer.UI
{
    public static class Extensions
    {
        public static void AddControls(this Control ctrl, Func<ControlBuilder, Control[]> builder)
        {
            var ctrls = builder?.Invoke(new());
            var collection = ctrl.Controls;

            for (int i = 0; i < ctrls.Length; i++)
            {
                collection.Add(ctrls[i]);
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
