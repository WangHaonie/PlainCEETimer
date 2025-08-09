using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class ListViewHelper
    {
        public sealed class SysHeader32NativeWindow : NativeWindow
        {
            private const int WM_SETCURSOR = 0x0020;

            public SysHeader32NativeWindow(HWND hHeader)
            {
                AssignHandle((IntPtr)hHeader);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_SETCURSOR)
                {
                    m.Result = BOOL.TRUE;
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
        }

        [DllImport(App.NativesDll, EntryPoint = "#3")]
        public static extern void SelectAllItems(IntPtr hLV, BOOL selected);

        [DllImport(App.User32Dll)]
        public static extern BOOL SetWindowPos(HWND hWnd, HWND hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    }
}
