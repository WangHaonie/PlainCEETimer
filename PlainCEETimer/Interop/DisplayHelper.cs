using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class DisplayHelper
    {
        public readonly struct Monitor(int index, string name, string path, RECT rect)
        {
            public int Index { get; } = index;
            public string Name { get; } = name;
            public string Path { get; } = path;
            public Rectangle Bounds { get; } = rect;

            public override readonly string ToString()
            {
                return $"{Index + 1}. {Name} ({Path}) ({Bounds.Width}x{Bounds.Height})";
            }
        }

        public static void EnumSystemDisplays(Action<int, Monitor> callback, int expectCount)
        {
            int i = 0;

            EnumSystemDisplays((r, d, p) =>
            {
                if (i < expectCount)
                {
                    callback(i, new(i, d, p, r));
                    i++;
                }
            });
        }

        [DllImport(App.NativesDll, EntryPoint = "#15", CharSet = CharSet.Unicode)]
        private static extern void EnumSystemDisplays(EnumDisplayProc lpfnEnum);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void EnumDisplayProc(RECT lprcMonitor,
            [MarshalAs(UnmanagedType.LPWStr)] string device,
            [MarshalAs(UnmanagedType.LPWStr)] string path);
    }
}
