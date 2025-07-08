using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class DisplayHelper
    {
        public class Monitor
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

        public static void EnumSystemDisplays(Action<int, Monitor> callback, int expectCount)
        {
            int i = 0;

            EnumSystemDisplays((r, d, p, id) =>
            {
                if (i < expectCount)
                {
                    callback(i, new(i, d, p, id, r));
                    i++;
                }
            });
        }

        [DllImport(App.NativesDll, EntryPoint = "#15", CharSet = CharSet.Unicode)]
        private static extern void EnumSystemDisplays(EnumDisplayProc lpfnEnum);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void EnumDisplayProc(RECT lprcMonitor,
            [MarshalAs(UnmanagedType.LPWStr)] string device,
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            [MarshalAs(UnmanagedType.LPWStr)] string did);
    }
}
