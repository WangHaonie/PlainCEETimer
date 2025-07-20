using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.Interop
{
    public class CommonDialogHelper(CommonDialog dialog, AppForm owner, string dialogTitle, HOOKPROC hook)
    {
        private sealed class GroupBoxNativeWindow : NativeWindow
        {
            private const int WM_PAINT = 0x000F;
            private const int WM_GETFONT = 0x0031;

            private bool Handled;

            public GroupBoxNativeWindow(IntPtr hGroupBox)
            {
                AssignHandle(hGroupBox);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_PAINT)
                {
                    base.WndProc(ref m);

                    if (!Handled)
                    {
                        IntPtr hdc;
                        IntPtr hWnd = Handle;

                        if ((hdc = GetWindowDC(hWnd)) != IntPtr.Zero)
                        {
                            using var g = Graphics.FromHdc(hdc);
                            using var font = Font.FromHfont(Natives.SendMessage(hWnd, WM_GETFONT, IntPtr.Zero, IntPtr.Zero));
                            using var brush = new SolidBrush(ThemeManager.DarkFore);

                            GetClientRect(hWnd, out RECT rc);
                            rc.Left += 6;
                            Rectangle rect = rc;
                            var sb = new StringBuilder(GetWindowTextLength(hWnd) + 1);
                            GetWindowText(hWnd, sb, sb.Capacity);
                            g.DrawString(sb.ToString(), font, brush, rect);
                            ReleaseDC(hWnd, hdc);
                            Handled = true;
                        }
                    }

                    return;
                }

                base.WndProc(ref m);
            }

            [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
            private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
            private static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport(App.User32Dll)]
            private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport(App.User32Dll)]
            private static extern BOOL GetClientRect(IntPtr hWnd, out RECT lpRect);

            [DllImport(App.User32Dll)]
            private static extern IntPtr GetWindowDC(IntPtr hWnd);
        }

        private const int WM_DESTROY = 0x0002;
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_INITDIALOG = 0x0110;
        private const int WM_COMMAND = 0x0111;
        private const int WM_CTLCOLORDLG = 0x0136;
        private const int WM_CTLCOLOREDIT = 0x0133;
        private const int WM_CTLCOLORSTATIC = 0x0138;
        private const int WM_CTLCOLORLISTBOX = 0x0134;
        private const int WM_CTLCOLORBTN = 0x0135;
        private const int TRANSPARENT = 0x0001;
        private const int chx1 = 0x0410;
        private const int chx2 = 0x0411;
        private const int cmb4 = 0x0473;
        private const int grp1 = 0x0430;
        private const int grp2 = 0x0431;
        private const int psh3 = 0x0402;
        private const int psh15 = 0x040e;
        private const int stc4 = 0x0443;
        private const int stc6 = 0x0445;

        private Rectangle Bounds;
        private readonly IntPtr hBrush = CreateSolidBrush(CrBack);
        private readonly StringBuilder builder = new(256);
        private static readonly COLORREF CrBack = ThemeManager.DarkBack;
        private static readonly COLORREF ForeCrColor = ThemeManager.DarkFore;
        private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;
        private static readonly List<int> FontDialogUnusedCtrls = [];

        public IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
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

                    HandleFontDialog(hWnd);

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
                        SetBkMode(wParam, TRANSPARENT);
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

        private void HandleFontDialog(IntPtr hWnd)
        {
            if (dialog is FontDialogEx f)
            {
                IntPtr hCtrl;

                if (UseDark && (hCtrl = GetDlgItem(hWnd, grp2)) != IntPtr.Zero)
                {
                    new GroupBoxNativeWindow(hCtrl);
                }

                if (FontDialogUnusedCtrls.Count == 0)
                {
                    FontDialogUnusedCtrls.Add(stc6);

                    if (!f.ShowColor)
                    {
                        FontDialogUnusedCtrls.Add(stc4);
                        FontDialogUnusedCtrls.Add(cmb4);
                    }

                    if (!f.ShowApply)
                    {
                        FontDialogUnusedCtrls.Add(psh3);
                    }

                    if (!f.ShowHelp)
                    {
                        FontDialogUnusedCtrls.Add(psh15);
                    }

                    if (!f.ShowEffects)
                    {
                        FontDialogUnusedCtrls.Add(grp1);
                        FontDialogUnusedCtrls.Add(chx1);
                        FontDialogUnusedCtrls.Add(chx2);
                    }
                }

                foreach (var ctrl in FontDialogUnusedCtrls)
                {
                    if ((hCtrl = GetDlgItem(hWnd, ctrl)) != IntPtr.Zero)
                    {
                        DestroyWindow(hCtrl);
                    }
                }
            }
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

            if (className == "ComboBox" || className == "Edit")
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
        private static extern void MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, BOOL bRepaint);

        [DllImport(App.Gdi32Dll)]
        private static extern BOOL DeleteObject(IntPtr hObject);

        [DllImport(App.User32Dll)]
        private static extern BOOL EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate BOOL EnumChildProc(IntPtr hWnd, IntPtr lParam);

        [DllImport(App.User32Dll)]
        private static extern BOOL GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(App.User32Dll)]
        private static extern BOOL DestroyWindow(IntPtr hWnd);

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
