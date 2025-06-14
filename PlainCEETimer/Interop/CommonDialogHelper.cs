﻿using System;
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
    public class CommonDialogHelper
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

        private Rectangle DialogRect;
        private readonly string DialogTitle;
        private readonly CommonDialogKind DialogKind;
        private readonly ICommonDialog Dialog;
        private readonly IntPtr hBrush;
        private readonly AppForm Parent;
        private readonly StringBuilder builder = new(256);
        private static readonly int BackCrColor;
        private static readonly int ForeCrColor;
        private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

        private delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

        public CommonDialogHelper(ICommonDialog dialog, string dialogTitle, CommonDialogKind kind, AppForm owner)
        {
            Dialog = dialog;
            DialogTitle = dialogTitle;
            DialogKind = kind;
            Parent = owner;
            Parent.ReActivate();
            hBrush = CreateSolidBrush(BackCrColor);
        }

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
                    if (DialogTitle != null)
                    {
                        SendMessageW(hWnd, WM_SETTEXT, IntPtr.Zero, Marshal.StringToHGlobalUni(DialogTitle));
                    }

                    if (DialogKind == CommonDialogKind.Font && !((FontDialogEx)Dialog).ShowColor)
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

                    RECT r = new();
                    GetWindowRect(hWnd, ref r);
                    DialogRect = r.ToRectangle();
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
                    return Dialog.HookProc(hWnd, WM_COMMAND, wParam, lParam);
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
            var validArea = Screen.GetWorkingArea(Parent);
            var DialogWidth = DialogRect.Width;
            var DialogHeight = DialogRect.Height;
            var X = Parent.Left + (Parent.Width / 2) - (DialogWidth / 2);
            var Y = Parent.Top + (Parent.Height / 2) - (DialogHeight / 2);
            var l = X;
            var t = Y;
            var r = X + DialogWidth;
            var b = Y + DialogHeight;
            if (l < validArea.X) X = validArea.X;
            if (t < validArea.Y) Y = validArea.Y;
            if (r > validArea.Right) X = validArea.Right - DialogWidth;
            if (b > validArea.Bottom) Y = validArea.Bottom - DialogHeight;
            MoveWindow(hWnd, X, Y, DialogWidth, DialogHeight, false);
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

            public readonly Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }
        }
    }
}
