using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PlainCEETimer.Interop
{
    public class CommonDialogHelper
    {
        private const int SW_HIDE = 0x0000;
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_SETTEXT = 0x000C;
        private const int WM_INITDIALOG = 0x0110;
        private const int WM_COMMAND = 0x0111;
        private const int stc4 = 0x0443;
        private const int cmb4 = 0x0473;

        private RECT DialogRect;
        private readonly AppForm Parent;

        public CommonDialogHelper(AppForm owner)
        {
            Parent = owner;
            Parent.ReActivate();
        }

        public IntPtr HookProc(ICommDlg Dlg, IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            #region 来自网络

            /*
            
            获取非托管 CommonDialog 的消息钩子并更改其窗口样式 灵感来自：

            systeminformer/SystemInformer/options.c at master · winsiderss/systeminformer
            https://github.com/winsiderss/systeminformer/blob/master/SystemInformer/options.c#L3337

            CommonDialog.cs
            https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/CommonDialog.cs,146

            */

            switch (msg)
            {
                case WM_SETFOCUS:
                    SetFocus(wparam);
                    break;
                case WM_INITDIALOG:
                    if (Dlg.DialogTitle != null)
                    {
                        SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, Dlg.DialogTitle);
                    }

                    if (ThemeManager.ShouldUseDarkMode)
                    {
                        ThemeManager.FlushDarkWindow(hWnd);
                    }

                    if (Dlg.DlgType == CommDlg.Font && !((FontDialogEx)Dlg).ShowColor)
                    {
                        IntPtr hSelectorStatic;

                        if ((hSelectorStatic = GetDlgItem(hWnd, stc4)) != IntPtr.Zero)
                        {
                            ShowWindow(hSelectorStatic, SW_HIDE);
                        }

                        if ((hSelectorStatic = GetDlgItem(hWnd, cmb4)) != IntPtr.Zero)
                        {
                            ShowWindow(hSelectorStatic, SW_HIDE);
                        }
                    }

                    GetWindowRect(hWnd, ref DialogRect);
                    KeepOnScreen(hWnd);
                    PostMessage(hWnd, WM_SETFOCUS, 0, 0);
                    break;
                case WM_COMMAND:
                    return Dlg.HookProcCallBack(hWnd, WM_COMMAND, wparam, lparam);
            }
            #endregion

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

        [DllImport(App.User32Dll)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport(App.User32Dll)]
        private static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport(App.User32Dll)]
        private static extern IntPtr PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport(App.User32Dll)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport(App.User32Dll)]
        private static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

        [DllImport(App.User32Dll)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

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
