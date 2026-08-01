using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

/*

.NET 封装 msctls_hotkey32 参考：

c# - Wrapping msctls_hotkey32 in .NET Windows Forms - Stack Overflow
https://stackoverflow.com/a/3274871/21094697


msctls_hotkey32 深色模式 参考：

Please consider adding darkmode for msctls_hotkey32. · Issue #9 · ozone10/darkmodelib
https://github.com/ozone10/darkmodelib/issues/9#issuecomment-3448256063

*/

[DebuggerDisplay("{Hotkey}")]
public class PlainHotkeyControl : Control, IThemeAware
{
    public event EventHandler HotKeyChanged;

    public Hotkey Hotkey
    {
        get
        {
            if (IsHandleCreated)
            {
                hotkey = new((ushort)Win32UI.SendMessage(Handle, CommCtrl.HKM_GETHOTKEY, 0, 0));
            }

            return hotkey;
        }
        set
        {
            if (IsHandleCreated)
            {
                SetHotKey(value);
            }

            hotkey = value;
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ClassName = "msctls_hotkey32";
            cp.Style |= WinUser.WS_BORDER;
            return cp;
        }
    }

    protected override Size DefaultMinimumSize => new(100, 21);

    private bool UseDark;
    private Hotkey hotkey;
    private readonly ThemeHelper themeHelper;
    private readonly IntPtr hBrush = Win32UI.CreateSolidBrush(Colors.DarkBackText);

    public PlainHotkeyControl()
    {
        SetStyle(ControlStyles.UserPaint, false);
        themeHelper = new(this);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Win32UI.RemoveWindowExStyle(Handle, WinUser.WS_EX_CLIENTEDGE);
        Win32UI.SendMessage(Handle, CommCtrl.HKM_SETRULES, CommCtrl.HKCOMB_NONE | CommCtrl.HKCOMB_S, (int)(HotkeyF.Ctrl | HotkeyF.Alt));
        SetHotKey(hotkey);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case WinUser.WM_REFLECT + WinUser.WM_COMMAND:
                if (m.WParam.HiWord == WinUser.EN_CHANGE)
                    OnHotKeyChanged();
                return;
            case WinUser.WM_ERASEBKGND when UseDark:
                Win32UI.GetClientRect(m.HWnd, out var rc);
                Win32UI.FillRect(m.WParam, ref rc, hBrush);
                m.Result = new(1);
                return;
            case WinUser.WM_PAINT when UseDark:
                Win32UI.ComctlHookSysColor(Colors.DarkForeText, Colors.DarkBackText);
                base.WndProc(ref m);
                Win32UI.ComctlUnhookSysColor();
                return;
            case WinUser.WM_DESTROY when UseDark:
                Win32UI.DeleteObject(hBrush);
                break;
        }

        base.WndProc(ref m);
    }

    private void SetHotKey(Hotkey hk)
    {
        Win32UI.SendMessage(Handle, CommCtrl.HKM_SETHOTKEY, hk, 0);
    }

    private void OnHotKeyChanged()
    {
        HotKeyChanged?.Invoke(this, EventArgs.Empty);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;

        if (!init)
        {
            Invalidate();
        }
    }
}