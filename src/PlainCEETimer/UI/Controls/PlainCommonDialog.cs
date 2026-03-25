using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public abstract class PlainCommonDialog(AppForm owner, string dialogTitle) : CommonDialog
{
    private sealed class GroupBoxNativeWindow : NativeWindow
    {
        private bool Handled;

        public GroupBoxNativeWindow(IntPtr hGroupBox)
        {
            AssignHandle(hGroupBox);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM.PAINT)
            {
                base.WndProc(ref m);

                if (!Handled)
                {
                    var hWnd = Handle;
                    using var g = Graphics.FromHwnd(hWnd);
                    using var font = Font.FromHfont(Win32UI.SendMessage(hWnd, WM.GETFONT, 0, 0));
                    using var brush = new SolidBrush(Colors.DarkForeText);

                    Win32UI.GetClientRect(hWnd, out var rc);
                    g.DrawString(Win32UI.GetWindowText(hWnd), font, brush, rc.Left + 7, rc.Top);
                    Handled = true;
                }

                return;
            }

            base.WndProc(ref m);
        }
    }

    private IntPtr Handle;
    private IntPtr MsgBoxHandle;
    private HOOKPROC CBTHookProc;
    private readonly IntPtr hBrush = Win32UI.CreateSolidBrush(BackCrColor);
    private static readonly COLORREF BackCrColor = Colors.DarkBackText;
    private static readonly COLORREF ForeCrColor = Colors.DarkForeText;
    private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

    public new bool? ShowDialog()
    {
        return ShowDialog(owner).AsBoolean();
    }

    protected abstract bool StartDialog(IntPtr hWndOwner);

    protected sealed override bool RunDialog(IntPtr hwndOwner)
    {
        try
        {
            return StartDialog(hwndOwner);
        }
        catch
        {
            return false;
        }
    }

    protected sealed override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        return msg switch
        {
            WM.INITDIALOG
                => WmInitDialog(hWnd),
            WM.CTLCOLORDLG
            or WM.CTLCOLOREDIT
            or WM.CTLCOLORSTATIC
            or WM.CTLCOLORLISTBOX
            or WM.CTLCOLORBTN
                => WmCtlColor(wparam),
            WM.DESTROY
                => WmDestroy(),
            _
                => IntPtr.Zero,
        };
    }

    private IntPtr WmInitDialog(IntPtr hWnd)
    {
        Handle = hWnd;
        owner.ReActivate();
        CBTHookProc = CbtHookProc;
        Win32UI.RegisterUnmanagedWindow(hWnd);
        Win32UI.ComdlgHookMessageBox(CBTHookProc);

        if (dialogTitle != null)
        {
            Win32UI.SetWindowText(hWnd, dialogTitle);
        }

        if (UseDark)
        {
            ThemeManager.EnableDarkModeForWindow(Handle);

            Win32UI.EnumChildWindows(hWnd, (child, _) =>
            {
                ThemeManager.EnableDarkModeForControl(child, GetNativeStyle(child, out var up), up);
                return true;
            }, IntPtr.Zero);
        }

        if (this is PlainFontDialog)
        {
            IntPtr hCtrl;

            if (UseDark && ((hCtrl = Win32UI.GetDlgItem(hWnd, NativeConstants.grp2)) != IntPtr.Zero))
            {
                if (ThemeManager.NewThemeAvailable)
                {
                    ThemeManager.EnableDarkModeForControl(hCtrl, SystemStyle.DarkTheme);
                }
                else
                {
                    _ = new GroupBoxNativeWindow(hCtrl);
                }
            }
        }

        Win32UI.GetWindowRect(hWnd, out var rect);
        Win32UI.MakeCenter(rect, owner.Bounds, out var r);
        Win32UI.MoveWindow(hWnd, r.X, r.Y, r.Width, r.Height, false);
        return new(1);
    }

    private IntPtr WmCtlColor(IntPtr hDC)
    {
        if (UseDark)
        {
            Win32UI.SetBkMode(hDC, NativeConstants.TRANSPARENT);
            Win32UI.SetBkColor(hDC, BackCrColor);
            Win32UI.SetTextColor(hDC, ForeCrColor);
            return hBrush;
        }

        return IntPtr.Zero;
    }

    private IntPtr WmDestroy()
    {
        Win32UI.ComdlgUnhookMessageBox();
        Win32UI.DeleteObject(hBrush);
        Win32UI.UnregisterUnmanagedWindow(Handle);
        return IntPtr.Zero;
    }

    private static SystemStyle GetNativeStyle(IntPtr hWnd, out bool up)
    {
        var cn = Win32UI.GetClassName(hWnd);

        if (cn == "ComboBox")
        {
            up = false;
            return SystemStyle.CfdDark;
        }

        if (cn == "Edit")
        {
            up = true;
            return SystemStyle.CfdDark;
        }

        up = true;
        return SystemStyle.ExplorerDark;
    }

    private IntPtr CbtHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        switch (nCode)
        {
            case NativeConstants.HCBT_CREATEWND:
                var lpcs = Marshal.ReadIntPtr(lParam);

                if (Win32UI.IsDialog(lpcs))
                {
                    Win32UI.GetWindowRect(Marshal.ReadIntPtr(lpcs, CREATESTRUCT.hwndParent), out var lprc);

                    Win32UI.MakeCenter
                    (
                        new
                        (
                            Marshal.ReadInt32(lpcs, CREATESTRUCT.x),
                            Marshal.ReadInt32(lpcs, CREATESTRUCT.y),
                            Marshal.ReadInt32(lpcs, CREATESTRUCT.cx),
                            Marshal.ReadInt32(lpcs, CREATESTRUCT.cy)
                        ), lprc, out var r
                    );

                    Marshal.WriteInt32(lpcs, CREATESTRUCT.x, r.X);
                    Marshal.WriteInt32(lpcs, CREATESTRUCT.y, r.Y);
                    Win32UI.RegisterUnmanagedWindow(MsgBoxHandle = wParam);
                }

                break;
            case NativeConstants.HCBT_DESTROYWND:

                if (wParam == MsgBoxHandle)
                {
                    Win32UI.UnregisterUnmanagedWindow(wParam);
                }

                break;
        }

        return new(-1);
    }

    public sealed override void Reset()
    {

    }
}
