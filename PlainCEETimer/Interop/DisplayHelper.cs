using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class DisplayHelper
    {
        public static string[] GetSystemDisplays()
        {
            var count = Screen.AllScreens.Length;
            var tmp = new string[count];
            int i = 0;

            EnumSystemDisplays((r, d, p, id) =>
            {
                if (i < count)
                {
                    tmp[i] = string.Format("{0}. {1} {2} ({3}) ({4}x{5})", i + 1, d, GetInternalName(id), p, r.Right - r.Left, r.Bottom - r.Top);
                    i++;

                    return BOOL.TRUE;
                }

                return BOOL.FALSE;
            });

            return tmp;
        }

        private static string GetInternalName(string did)
        {
            var dids = did.Split('\\');
            var iname = did;

            if (dids.Length > 2)
            {
                iname = dids[1];
            }

            return iname;
        }

        [DllImport(App.NativesDll, EntryPoint = "#12", CharSet = CharSet.Unicode)]
        private static extern void EnumSystemDisplays(EnumDisplayProc lpfnEnum);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate BOOL EnumDisplayProc(RECT lprcMonitor, string device, string path, string did);
    }
}
