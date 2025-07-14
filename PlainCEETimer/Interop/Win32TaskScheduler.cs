using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class Win32TaskScheduler
    {
        static Win32TaskScheduler()
        {
            Initialize();
        }

        [DllImport(App.NativesDll, EntryPoint = "#16")]
        private static extern void Initialize();

        [DllImport(App.NativesDll, EntryPoint = "#17", CharSet = CharSet.Unicode)]
        public static extern void ImportTask(string taskName, [MarshalAs(UnmanagedType.BStr)] string bstrXml);

        [DllImport(App.NativesDll, EntryPoint = "#18", CharSet = CharSet.Unicode)]
        public static extern void ExportTask(string taskName, [MarshalAs(UnmanagedType.BStr)] out string pBstrXml);

        [DllImport(App.NativesDll, EntryPoint = "#19", CharSet = CharSet.Unicode)]
        public static extern void EnableTask(string taskName);

        [DllImport(App.NativesDll, EntryPoint = "#20", CharSet = CharSet.Unicode)]
        public static extern void DeleteTask(string taskName);

        [DllImport(App.NativesDll, EntryPoint = "#21")]
        public static extern void Release();
    }
}
