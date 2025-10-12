using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public abstract class PlainCommonDialog(AppForm owner, string dialogTitle) : CommonDialog
{
    private sealed class GroupBoxNativeWindow : NativeWindow
    {
        private bool Handled;

        public GroupBoxNativeWindow(HWND hGroupBox)
        {
            AssignHandle((IntPtr)hGroupBox);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_PAINT = 0x000F;
            const int WM_GETFONT = 0x0031;

            if (m.Msg == WM_PAINT)
            {
                base.WndProc(ref m);

                if (!Handled)
                {
                    HWND hWnd = Handle;

                    using var g = Graphics.FromHwnd((IntPtr)hWnd);
                    using var font = Font.FromHfont(Win32UI.SendMessage(hWnd, WM_GETFONT, 0, 0));
                    using var brush = new SolidBrush(Colors.DarkForeText);

                    Win32UI.GetClientRect(hWnd, out var rc);
                    rc.Left += 6;
                    Rectangle rect = rc;
                    g.DrawString(Win32UI.GetWindowText(hWnd), font, brush, rect);
                    Handled = true;
                }

                return;
            }

            base.WndProc(ref m);
        }
    }

    private readonly IntPtr hBrush = Win32UI.CreateSolidBrush(BackCrColor);
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
        const int WM_DESTROY = 0x0002;
        const int WM_INITDIALOG = 0x0110;
        const int WM_CTLCOLORDLG = 0x0136;
        const int WM_CTLCOLOREDIT = 0x0133;
        const int WM_CTLCOLORSTATIC = 0x0138;
        const int WM_CTLCOLORLISTBOX = 0x0134;
        const int WM_CTLCOLORBTN = 0x0135;

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
                Win32UI.DeleteObject(hBrush);
                break;
        }

        return IntPtr.Zero;
    }

    private BOOL WmInitDialog(HWND hWnd)
    {
        owner.ReActivate();

        if (dialogTitle != null)
        {
            Win32UI.SetWindowText(hWnd, dialogTitle);
        }

        if (UseDark)
        {
            ThemeManager.FlushWindow(hWnd);

            Win32UI.EnumChildWindows(hWnd, (child, _) =>
            {
                ThemeManager.FlushControl(child, GetNativeStyle(child, out var up), up);
                return BOOL.TRUE;
            }, IntPtr.Zero);
        }

        if (this is PlainFontDialog f)
        {
            HWND hCtrl;
            const int grp2 = 0x0431;

            if (UseDark && (hCtrl = Win32UI.GetDlgItem(hWnd, grp2)))
            {
                if (ThemeManager.NewThemeAvailable)
                {
                    ThemeManager.FlushControl(hCtrl, NativeStyle.DarkTheme);
                }
                else
                {
                    new GroupBoxNativeWindow(hCtrl);
                }
            }
        }

        Win32UI.GetWindowRect(hWnd, out var rect);

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

        Win32UI.MoveWindow(hWnd, x, y, w, h, BOOL.FALSE);
        return BOOL.TRUE;
    }

    private IntPtr WmCtlColor(HDC hDC)
    {
        if (UseDark)
        {
            const int TRANSPARENT = 1;

            Win32UI.SetBkMode(hDC, TRANSPARENT);
            Win32UI.SetBkColor(hDC, BackCrColor);
            Win32UI.SetTextColor(hDC, ForeCrColor);
            return hBrush;
        }

        return IntPtr.Zero;
    }

    private NativeStyle GetNativeStyle(HWND hWnd, out bool up)
    {
        var cn = Win32UI.GetClassName(hWnd);

        if (cn == "ComboBox")
        {
            up = false;
            return NativeStyle.CfdDark;
        }

        if (cn == "Edit")
        {
            up = true;
            return NativeStyle.CfdDark;
        }

        up = true;
        return NativeStyle.ExplorerDark;
    }

    public sealed override void Reset()
    {

    }
}
