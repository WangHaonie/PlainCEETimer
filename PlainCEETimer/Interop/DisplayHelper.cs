using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class DisplayHelper
    {
        private class Monitor
        {
            private readonly int Index;
            private readonly string Name;
            private readonly string InternalName;
            private readonly string Path;
            private readonly Rectangle Bounds;

            public Monitor(int index, string name, string path, string did, RECT rect)
            {
                Index = index;
                Name = name;
                Path = path;
                Bounds = rect;

                var dids = did.Split('\\');
                var iname = did;

                if (dids.Length > 2)
                {
                    iname = dids[1];
                }

                InternalName = iname;
            }

            public override string ToString()
            {
                return string.Format("{0}. {1} {2} ({3}) ({4}x{5})", Index + 1, Name, InternalName, Path, Bounds.Width, Bounds.Height);
            }
        }

        public static string[] GetSystemDisplays()
        {
            var count = Screen.AllScreens.Length;
            var tmp = new string[count];
            int i = 0;

            EnumSystemDisplays((r, d, p, id) =>
            {
                if (i < count)
                {
                    tmp[i] = new Monitor(i, d, p, id, r).ToString();
                    i++;

                    return BOOL.TRUE;
                }

                return BOOL.FALSE;
            });

            return tmp;
        }

        [DllImport(App.NativesDll, EntryPoint = "#14", CharSet = CharSet.Unicode)]
        private static extern void EnumSystemDisplays(EnumDisplayProc lpfnEnum);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate BOOL EnumDisplayProc(RECT lprcMonitor, string device, string path, string did);
    }
}
