using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.Interop
{
    public class CommonDialogHelper(CommonDialog dialog, AppForm owner, string dialogTitle, HOOKPROC DefHookProc)
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
                        HDC hdc;
                        HWND hWnd = Handle;

                        if (hdc = GetWindowDC(hWnd))
                        {
                            using var g = Graphics.FromHdc(hdc.ToIntPtr());
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
            private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
            private static extern int GetWindowTextLength(HWND hWnd);

            [DllImport(App.User32Dll)]
            private static extern int ReleaseDC(HWND hWnd, HDC hDC);

            [DllImport(App.User32Dll)]
            private static extern BOOL GetClientRect(HWND hWnd, out RECT lpRect);

            [DllImport(App.User32Dll)]
            private static extern HDC GetWindowDC(HWND hWnd);
        }

        private const int WM_DESTROY = 0x0002;
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

        private static int[] UnusedCtrls;
        private readonly IntPtr hBrush = CreateSolidBrush(BackCrColor);
        private readonly StringBuilder builder = new(256);
        private static readonly COLORREF BackCrColor = new(ThemeManager.DarkBack);
        private static readonly COLORREF ForeCrColor = new(ThemeManager.DarkFore);
        private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

        public IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_INITDIALOG:
                    return WmInitDialog(hWnd);
                case WM_CTLCOLORDLG:
                case WM_CTLCOLOREDIT:
                case WM_CTLCOLORSTATIC:
                case WM_CTLCOLORLISTBOX:
                case WM_CTLCOLORBTN:
                    return WmCtlColor(new(wParam));
                case WM_COMMAND:
                    return DefHookProc(hWnd, WM_COMMAND, wParam, lParam);
                case WM_DESTROY:
                    DeleteObject(hBrush);
                    break;
            }

            return IntPtr.Zero;
        }

        private BOOL WmInitDialog(HWND hWnd)
        {
            owner.ReActivate();

            if (dialogTitle != null)
            {
                SetWindowText(hWnd, dialogTitle);
            }

            if (dialog is PlainFontDialog f)
            {
                HWND hCtrl;

                if (UseDark && (hCtrl = GetDlgItem(hWnd, grp2)))
                {
                    new GroupBoxNativeWindow(hCtrl.ToIntPtr());
                }

                if (UnusedCtrls == null)
                {
                    UnusedCtrls = new int[8];
                    UnusedCtrls[0] = stc6;

                    if (!f.ShowColor)
                    {
                        UnusedCtrls[1] = stc4;
                        UnusedCtrls[2] = cmb4;
                    }

                    if (!f.ShowApply)
                    {
                        UnusedCtrls[3] = psh3;
                    }

                    if (!f.ShowHelp)
                    {
                        UnusedCtrls[4] = psh15;
                    }

                    if (!f.ShowEffects)
                    {
                        UnusedCtrls[5] = grp1;
                        UnusedCtrls[6] = chx1;
                        UnusedCtrls[7] = chx2;
                    }
                }


                foreach (var ctrl in UnusedCtrls)
                {
                    if (hCtrl = GetDlgItem(hWnd, ctrl))
                    {
                        DestroyWindow(hCtrl);
                    }
                }
            }

            if (UseDark)
            {
                ThemeManager.FlushWindow(hWnd);

                EnumChildWindows(hWnd, (child, _) =>
                {
                    ThemeManager.FlushControl(child, GetNativeStyle(child));
                    return BOOL.TRUE;
                }, IntPtr.Zero);
            }

            GetWindowRect(hWnd, out RECT rect);

            Rectangle bounds = rect;
            var screen = Screen.GetWorkingArea(owner);

            var w = bounds.Width;
            var h = bounds.Height;
            var x = owner.Left + (owner.Width / 2) - (w / 2);
            var y = owner.Top + (owner.Height / 2) - (h / 2);

            var l = x;
            var t = y;
            var r = x + w;
            var b = y + h;
            if (l < screen.X) x = screen.X;
            if (t < screen.Y) y = screen.Y;
            if (r > screen.Right) x = screen.Right - w;
            if (b > screen.Bottom) y = screen.Bottom - h;

            MoveWindow(hWnd, x, y, w, h, BOOL.FALSE);
            return BOOL.TRUE;
        }

        private IntPtr WmCtlColor(HDC hDC)
        {
            if (UseDark)
            {
                SetBkMode(hDC, TRANSPARENT);
                SetBkColor(hDC, BackCrColor);
                Natives.SetTextColor(hDC, ForeCrColor);
                return hBrush;
            }

            return IntPtr.Zero;
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
        private static extern int SetBkMode(HDC hdc, int mode);

        [DllImport(App.User32Dll)]
        private static extern void MoveWindow(HWND hWnd, int X, int Y, int nWidth, int nHeight, BOOL bRepaint);

        [DllImport(App.Gdi32Dll)]
        private static extern BOOL DeleteObject(IntPtr hObject);

        [DllImport(App.User32Dll)]
        private static extern BOOL EnumChildWindows(HWND hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate BOOL EnumChildProc(IntPtr hWnd, IntPtr lParam);

        [DllImport(App.User32Dll)]
        private static extern BOOL GetWindowRect(HWND hWnd, out RECT lpRect);

        [DllImport(App.User32Dll)]
        private static extern BOOL DestroyWindow(HWND hWnd);

        [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
        private static extern BOOL SetWindowText(HWND hWnd, string lpString);

        [DllImport(App.Gdi32Dll)]
        private static extern COLORREF SetBkColor(HDC hdc, COLORREF color);

        [DllImport(App.Gdi32Dll)]
        private static extern IntPtr CreateSolidBrush(COLORREF color);

        [DllImport(App.User32Dll)]
        private static extern HWND GetDlgItem(HWND hDlg, int nIDDlgItem);
    }
}
