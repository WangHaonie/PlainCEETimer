using System;
using System.Reflection;
using System.Windows.Forms;
using PlainCEETimer.Modules.Reflection;

namespace PlainCEETimer.UI.Extensions
{
    public static class ScreenExtensions
    {
        private static Func<Screen, IntPtr> fn_get_hmonitor;

        extension(Screen screen)
        {
            public IntPtr hmonitor
            {
                get
                {
                    if (fn_get_hmonitor == null)
                    {
                        typeof(Screen).RevealMember(out FieldInfo fi, "hmonitor");
                        fi.CreateGetter(out fn_get_hmonitor);
                    }

                    return fn_get_hmonitor(screen);
                }
            }
        }
    }
}
