using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public abstract class PlainCommonDialog(AppForm owner, string dialogTitle) : CommonDialog
{
    private sealed class GroupBoxNativeWindow : NativeWindow
    {
        private const int WM_PAINT = 0x000F;
        private const int WM_GETFONT = 0x0031;

        private bool Handled;

        public GroupBoxNativeWindow(HWND hGroupBox)
        {
            AssignHandle((IntPtr)hGroupBox);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PAINT)
            {
                base.WndProc(ref m);

                if (!Handled)
                {
                    HWND hWnd = Handle;

                    using var g = Graphics.FromHwnd((IntPtr)hWnd);
                    using var font = Font.FromHfont(Natives.SendMessage(hWnd, WM_GETFONT, IntPtr.Zero, IntPtr.Zero));
                    using var brush = new SolidBrush(Colors.DarkForeText);

                    GetClientRect(hWnd, out var rc);
                    rc.Left += 6;
                    Rectangle rect = rc;
                    g.DrawString(Win32UI.GetWindowTextEx(hWnd), font, brush, rect);
                    Handled = true;
                }

                return;
            }

            base.WndProc(ref m);
        }

        [DllImport(App.User32Dll)]
        private static extern BOOL GetClientRect(HWND hWnd, out RECT lpRect);
    }

    private const int WM_DESTROY = 0x0002;
    private const int WM_INITDIALOG = 0x0110;
    private const int WM_CTLCOLORDLG = 0x0136;
    private const int WM_CTLCOLOREDIT = 0x0133;
    private const int WM_CTLCOLORSTATIC = 0x0138;
    private const int WM_CTLCOLORLISTBOX = 0x0134;
    private const int WM_CTLCOLORBTN = 0x0135;
    private const int TRANSPARENT = 0x0001;
    private const int grp2 = 0x0431;

    private readonly IntPtr hBrush = CreateSolidBrush(BackCrColor);
    private readonly StringBuilder builder = new(256);
    private static readonly COLORREF BackCrColor = Colors.DarkBackText;
    private static readonly COLORREF ForeCrColor = Colors.DarkForeText;
    private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

    public new DialogResult ShowDialog()
    {
        return ShowDialog(owner);
    }

    protected abstract BOOL RunDialog(HWND hWndOwner);

    protected sealed override bool RunDialog(IntPtr hwndOwner)
    {
        try
        {
            return RunDialog((HWND)hwndOwner);
        }
        catch
        {
            return false;
        }
    }

    protected sealed override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
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
                return WmCtlColor(wparam);
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

        if (this is PlainFontDialog f)
        {
            HWND hCtrl;

            if (UseDark && (hCtrl = GetDlgItem(hWnd, grp2)))
            {
                new GroupBoxNativeWindow(hCtrl);
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

        GetWindowRect(hWnd, out var rect);

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

    private NativeStyle GetNativeStyle(HWND hWnd)
    {
        builder.Clear();
        GetClassName(hWnd, builder, 256);
        var className = builder.ToString();

        if (className == "ComboBox" || className == "Edit")
        {
            return NativeStyle.CfdDark;
        }

        return NativeStyle.ExplorerDark;
    }

    public sealed override void Reset()
    {

    }

    [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
    private static extern int GetClassName(HWND hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport(App.Gdi32Dll)]
    private static extern int SetBkMode(HDC hdc, int mode);

    [DllImport(App.User32Dll)]
    private static extern void MoveWindow(HWND hWnd, int X, int Y, int nWidth, int nHeight, BOOL bRepaint);

    [DllImport(App.Gdi32Dll)]
    private static extern BOOL DeleteObject(IntPtr hObject);

    [DllImport(App.User32Dll)]
    private static extern BOOL EnumChildWindows(HWND hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

    private delegate BOOL EnumChildProc(HWND hWnd, IntPtr lParam);

    [DllImport(App.User32Dll)]
    private static extern BOOL GetWindowRect(HWND hWnd, out RECT lpRect);

    [DllImport(App.User32Dll, CharSet = CharSet.Unicode)]
    private static extern BOOL SetWindowText(HWND hWnd, string lpString);

    [DllImport(App.Gdi32Dll)]
    private static extern COLORREF SetBkColor(HDC hdc, COLORREF color);

    [DllImport(App.Gdi32Dll)]
    private static extern IntPtr CreateSolidBrush(COLORREF color);

    [DllImport(App.User32Dll)]
    private static extern HWND GetDlgItem(HWND hDlg, int nIDDlgItem);
}
