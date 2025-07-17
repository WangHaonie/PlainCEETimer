using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.Interop
{
    public class CommonDialogHelper(CommonDialog dialog, AppForm owner, string dialogTitle, HOOKPROC hook)
    {
        private const int SW_HIDE = 0x0000;
        private const int BM_TRANSPARENT = 0x0001;
        private const int WM_DESTROY = 0x0002;
        private const int WM_SETFOCUS = 0x0007;
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
        private readonly IntPtr hBrush = CreateSolidBrush(CrBack);
        private readonly StringBuilder builder = new(256);
        private static readonly COLORREF CrBack = ThemeManager.DarkBack;
        private static readonly COLORREF ForeCrColor = ThemeManager.DarkFore;
        private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

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
                        SetWindowText(hWnd, dialogTitle);
                    }

                    if (dialog is FontDialogEx f && !f.ShowColor)
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
                        SetBkColor(wParam, CrBack);
                        return hBrush;
                    }
                    break;
                case WM_COMMAND:
                    return hook(hWnd, WM_COMMAND, wParam, lParam);
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
                return BOOL.TRUE;
            }, IntPtr.Zero);
        }

        private void KeepOnScreen(IntPtr hWnd)
        {
            var validArea = Screen.GetWorkingArea(owner);
            var w = Bounds.Width;
            var h = Bounds.Height;
            var x = owner.Left + (owner.Width / 2) - (w / 2);
            var y = owner.Top + (owner.Height / 2) - (h / 2);
            var l = x;
            var t = y;
            var r = x + w;
            var b = y + h;
            if (l < validArea.X) x = validArea.X;
            if (t < validArea.Y) y = validArea.Y;
            if (r > validArea.Right) x = validArea.Right - w;
            if (b > validArea.Bottom) y = validArea.Bottom - h;
            MoveWindow(hWnd, x, y, w, h, BOOL.FALSE);
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

        [DllImport(App.User32Dll)]
        private static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, BOOL bRepaint);

        [DllImport(App.Gdi32Dll)]
        private static extern BOOL DeleteObject(IntPtr hObject);

        [DllImport(App.User32Dll)]
        private static extern BOOL EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate BOOL EnumChildProc(IntPtr hWnd, IntPtr lParam);

        [DllImport(App.User32Dll)]
        private static extern BOOL GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(App.User32Dll)]
        private static extern BOOL ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
        private static extern BOOL SetWindowText(IntPtr hWnd, string lpString);

        [DllImport(App.Gdi32Dll)]
        private static extern COLORREF SetTextColor(IntPtr hdc, COLORREF color);

        [DllImport(App.Gdi32Dll)]
        private static extern COLORREF SetBkColor(IntPtr hdc, COLORREF color);

        [DllImport(App.Gdi32Dll)]
        private static extern IntPtr CreateSolidBrush(COLORREF color);

        [DllImport(App.User32Dll)]
        private static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

        [DllImport(App.User32Dll)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport(App.User32Dll)]
        private static extern IntPtr PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
