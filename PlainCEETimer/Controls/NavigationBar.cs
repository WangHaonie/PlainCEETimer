using PlainCEETimer.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class NavigationBar : TreeView
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            NativeInterop.SetWindowTheme(Handle, "Explorer", null);
            base.OnHandleCreated(e);
        }
    }
}
