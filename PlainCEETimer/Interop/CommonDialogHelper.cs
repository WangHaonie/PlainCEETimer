using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.Interop
{
    public class CommonDialogHelper(ICommonDialog dialog, string dialogTitle, CommonDialogKind kind, AppForm owner)
    {
        private const int SW_HIDE = 0x0000;
        private const int BM_TRANSPARENT = 0x0001;
        private const int WM_DESTROY = 0x0002;
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_SETTEXT = 0x000C;
        private const int WM_INITDIALOG = 0x0110;
        private const int WM_COMMAND = 0x0111;
        private const int WM_CTLCOLORDLG = 0x0136;
        private const int WM_CTLCOLOREDIT = 0x0133;
        private const int WM_CTLCOLORSTATIC = 0x0138;
        private const int WM_CTLCOLORLISTBOX = 0x0134;
        private const int WM_CTLCOLORBTN = 0x0135;
        private const int stc4 = 0x0443;
        private const int cmb4 = 0x0473;

        private Rectangle Bounds;
        private readonly IntPtr hBrush = CreateSolidBrush(BackCrColor);
        private readonly StringBuilder builder = new(256);
        private static readonly int BackCrColor;
        private static readonly int ForeCrColor;
        private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

        private delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

        static CommonDialogHelper()
        {
            BackCrColor = ThemeManager.DarkBack.ToWin32();
            ForeCrColor = ThemeManager.DarkFore.ToWin32();
        }

        public IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
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
                    SetFocus(wParam);
                    break;
                case WM_INITDIALOG:
                    owner.ReActivate();

                    if (dialogTitle != null)
                    {
                        SendMessageW(hWnd, WM_SETTEXT, IntPtr.Zero, Marshal.StringToHGlobalUni(dialogTitle));
                    }

                    if (kind == CommonDialogKind.Font && !((FontDialogEx)dialog).ShowColor)
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

                    if (UseDark)
                    {
                        ThemeManager.FlushWindow(hWnd);
                        FlushDark(hWnd);
                    }

                    GetWindowRect(hWnd, out RECT r);
                    Bounds = r;
                    KeepOnScreen(hWnd);
                    PostMessage(hWnd, WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
                    break;
                case WM_CTLCOLORDLG:
                case WM_CTLCOLOREDIT:
                case WM_CTLCOLORSTATIC:
                case WM_CTLCOLORLISTBOX:
                case WM_CTLCOLORBTN:
                    if (UseDark)
                    {
                        SetBkMode(wParam, BM_TRANSPARENT);
                        SetTextColor(wParam, ForeCrColor);
                        SetBkColor(wParam, BackCrColor);
                        return hBrush;
                    }
                    break;
                case WM_COMMAND:
                    return dialog.HookProc(hWnd, WM_COMMAND, wParam, lParam);
                case WM_DESTROY:
                    DeleteObject(hBrush);
                    break;
            }

            return IntPtr.Zero;
        }

        private void FlushDark(IntPtr hWnd)
        {
            EnumChildWindows(hWnd, (child, _) =>
            {
                ThemeManager.FlushControl(child, GetNativeStyle(child));
                return true;
            }, IntPtr.Zero);
        }

        private void KeepOnScreen(IntPtr hWnd)
        {
            var validArea = Screen.GetWorkingArea(owner);
            var W = Bounds.Width;
            var H = Bounds.Height;
            var X = owner.Left + (owner.Width / 2) - (W / 2);
            var Y = owner.Top + (owner.Height / 2) - (H / 2);
            var l = X;
            var t = Y;
            var r = X + W;
            var b = Y + H;
            if (l < validArea.X) X = validArea.X;
            if (t < validArea.Y) Y = validArea.Y;
            if (r > validArea.Right) X = validArea.Right - W;
            if (b > validArea.Bottom) Y = validArea.Bottom - H;
            MoveWindow(hWnd, X, Y, W, H, false);
        }

        private NativeStyle GetNativeStyle(IntPtr hWnd)
        {
            GetClassName(hWnd, builder, 256);
            var className = builder.ToString();

            if (className.Has("ComboBox") || className.Has("Edit"))
            {
                return NativeStyle.CFD;
            }

            return NativeStyle.Explorer;
        }

        [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport(App.Gdi32Dll)]
        private static extern int SetBkMode(IntPtr hdc, int mode);

        [DllImport(App.Gdi32Dll)]
        private static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport(App.Gdi32Dll)]
        private static extern int SetTextColor(IntPtr hdc, int crColor);

        [DllImport(App.Gdi32Dll)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport(App.User32Dll)]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        [DllImport(App.User32Dll)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(App.User32Dll)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(App.User32Dll)]
        private static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport(App.Gdi32Dll)]
        private static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport(App.User32Dll)]
        private static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

        [DllImport(App.User32Dll)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport(App.User32Dll)]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(App.User32Dll)]
        private static extern IntPtr PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public static implicit operator Rectangle(RECT r)
            {
                return Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
            }
        }
    }
}
