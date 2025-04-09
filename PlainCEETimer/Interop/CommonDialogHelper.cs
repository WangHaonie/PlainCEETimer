using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PlainCEETimer.Interop
{
    public class CommonDialogHelper
    {
        private RECT DialogRect;
        private readonly AppForm Parent;

        public CommonDialogHelper(AppForm owner)
        {
            Parent = owner;
            Parent.ReActivate();
        }

        public IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            switch (msg)
            {
                case 0x0007: // WM_SETFOCUS
                    SetFocus(wparam);
                    break;
                case 0x0110: // WM_INITDIALOG
                    if (ThemeManager.ShouldUseDarkMode) ThemeManager.FlushDarkWindow(hWnd);
                    GetWindowRect(hWnd, ref DialogRect);
                    KeepOnScreen(hWnd);
                    PostMessage(hWnd, 0x0007, 0, 0);
                    break;
            }

            return IntPtr.Zero;
        }

        private void KeepOnScreen(IntPtr hWnd)
        {
            var validArea = Screen.FromControl(Parent).WorkingArea;
            var DialogWidth = DialogRect.Right - DialogRect.Left;
            var DialogHeight = DialogRect.Bottom - DialogRect.Top;
            var X = Parent.Left + (Parent.Width / 2) - (DialogWidth / 2);
            var Y = Parent.Top + (Parent.Height / 2) - (DialogHeight / 2);
            var l = X;
            var t = Y;
            var r = X + DialogWidth;
            var b = Y + DialogHeight;
            if (l < validArea.Left) X = validArea.Left;
            if (t < validArea.Top) Y = validArea.Top;
            if (r > validArea.Right) X = validArea.Right - DialogWidth;
            if (b > validArea.Bottom) Y = validArea.Bottom - DialogHeight;
            MoveWindow(hWnd, X, Y, DialogWidth, DialogHeight, false);
        }

        [DllImport(App.User32Dll, CallingConvention = CallingConvention.StdCall)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport(App.User32Dll, CallingConvention = CallingConvention.StdCall)]
        private static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport(App.User32Dll, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport(App.User32Dll, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetFocus(IntPtr hWnd);


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
