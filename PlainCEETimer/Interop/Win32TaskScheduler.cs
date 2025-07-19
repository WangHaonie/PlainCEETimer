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

        [DllImport(App.NativesDll, EntryPoint = "#15")]
        private static extern void Initialize();

        [DllImport(App.NativesDll, EntryPoint = "#16", CharSet = CharSet.Unicode)]
        public static extern void Import(string taskName, [MarshalAs(UnmanagedType.BStr)] string bstrXml);

        [DllImport(App.NativesDll, EntryPoint = "#17", CharSet = CharSet.Unicode)]
        public static extern void Export(string taskName, [MarshalAs(UnmanagedType.BStr)] out string pbstrXml);

        [DllImport(App.NativesDll, EntryPoint = "#18", CharSet = CharSet.Unicode)]
        public static extern void Enable(string taskName);

        [DllImport(App.NativesDll, EntryPoint = "#19", CharSet = CharSet.Unicode)]
        public static extern void Delete(string taskName);

        [DllImport(App.NativesDll, EntryPoint = "#20")]
        public static extern void Release();
    }
}
