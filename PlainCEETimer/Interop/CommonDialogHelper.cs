using PlainCEETimer.Controls;
using PlainCEETimer.Modules;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PlainCEETimer.Interop
{
    public sealed class CommonDialogHelper
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
        private const int stc4 = 0x0443;
        private const int cmb4 = 0x0473;

        private RECT DialogRect;
        private readonly int BackCrColor;
        private readonly IntPtr hBrush;
        private readonly AppForm Parent;
        private readonly StringBuilder builder = new(256);
        private delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);
        private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

        public CommonDialogHelper(AppForm owner)
        {
            Parent = owner;
            Parent.ReActivate();
            BackCrColor = ColorTranslator.ToWin32(ThemeManager.DarkBack);
            hBrush = CreateSolidBrush(BackCrColor);
        }

        public IntPtr HookProc(ICommonDialog Dialog, IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam)
        {
            #region 来自网络

            /*
            
            获取非托管 CommonDialog 的消息钩子并更改其窗口样式 灵感来自：

            systeminformer/SystemInformer/options.c at master · winsiderss/systeminformer
            https://github.com/winsiderss/systeminformer/blob/master/SystemInformer/options.c#L3337

            CommonDialog.cs
            https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/CommonDialog.cs,146

            */

            switch (Msg)
            {
                case WM_SETFOCUS:
                    SetFocus(wParam);
                    break;
                case WM_INITDIALOG:
                    if (Dialog.DialogTitle != null)
                    {
                        SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, Dialog.DialogTitle);
                    }

                    if (Dialog.DialogKind == CommonDialogKind.Font && !((FontDialogEx)Dialog).ShowColor)
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
                    return Dialog.BaseHookProc(hWnd, WM_COMMAND, wParam, lParam);
            }
            #endregion

            if (UseDark)
            {
                return DarkProc(hWnd, Msg, wParam);
            }

            return IntPtr.Zero;
        }

        private IntPtr DarkProc(IntPtr hWnd, int Msg, IntPtr wParam)
        {
            switch (Msg)
            {
                case WM_INITDIALOG:
                    ThemeManager.FlushDarkWindow(hWnd);
                    FlushDark(hWnd);
                    break;
                case WM_CTLCOLORDLG:
                case WM_CTLCOLOREDIT:
                case WM_CTLCOLORSTATIC:
                case WM_CTLCOLORLISTBOX:
                    SetBkMode(wParam, BM_TRANSPARENT);
                    SetTextColor(wParam, ColorTranslator.ToWin32(ThemeManager.DarkFore));
                    SetBkColor(wParam, BackCrColor);
                    return hBrush;
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
                switch (GetNativeControl(child))
                {
                    case NativeControl.Button:
                    case NativeControl.ComboLBox:
                        ThemeManager.FlushDarkControl(child, NativeStyle.Explorer);
                        break;
                    case NativeControl.Label:
                    case NativeControl.TextBox:
                    case NativeControl.ComboBox:
                        ThemeManager.FlushDarkControl(child, NativeStyle.CFD);
                        break;
                }
                return true;

            }, IntPtr.Zero);
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

        private NativeControl GetNativeControl(IntPtr hWnd)
        {
            GetClassName(hWnd, builder, 256);
            return builder.ToString() switch
            {
                "Button" => NativeControl.Button,
                "ComboBox" => NativeControl.ComboBox,
                "ComboLBox" => NativeControl.ComboLBox,
                "Edit" => NativeControl.TextBox,
                _ => NativeControl.Label
            };
        }

        [DllImport(App.User32Dll, CharSet = CharSet.Auto)]
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
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

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

        [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport(App.User32Dll)]
        private static extern IntPtr PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

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
