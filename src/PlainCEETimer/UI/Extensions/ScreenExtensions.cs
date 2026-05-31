using System;
using System.Reflection;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Extensions
{
    public static class ScreenExtensions
    {
        extension(Screen screen)
        {
            public IntPtr hmonitor
            {
                get
                {
                    return (IntPtr)typeof(Screen)
                        .GetField("hmonitor", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(screen);
                }
            }
        }
    }
}
