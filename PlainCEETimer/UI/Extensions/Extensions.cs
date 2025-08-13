using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Extensions
{
    public static class Extensions
    {
        public static void AddControls(this Control ctrl, Func<ControlBuilder, Control[]> builder)
        {
            var ctrls = builder(new());
            var collection = ctrl.Controls;

            for (int i = 0; i < ctrls.Length; i++)
            {
                collection.Add(ctrls[i]);
            }
        }

        public static TControl With<TControl>(this TControl control, Action<TControl> additions)
            where TControl : Control
        {
            additions(control);
            return control;
        }

        public static TControl AsFocus<TControl>(this TControl control, AppForm parent)
            where TControl : Control
        {
            if (control is not Form)
            {
                parent.FocusControl = control;
            }

            return control;
        }

        public static Point LocationToScreen(this Control control, int xOffset = 0, int yOffset = 0)
        {
            var top = control.TopLevelControl;
            var p = control.Location;

            if (control is not Form)
            {
                p = top.PointToScreen(p);
            }

            return new(p.X + xOffset, p.Y + yOffset);
        }
    }
}
