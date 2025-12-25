using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls;

/*

.NET 封装 msctls_hotkey32 参考：

c# - Wrapping msctls_hotkey32 in .NET Windows Forms - Stack Overflow
https://stackoverflow.com/a/3274871/21094697


msctls_hotkey32 深色模式 参考：

Please consider adding darkmode for msctls_hotkey32. · Issue #9 · ozone10/darkmodelib
https://github.com/ozone10/darkmodelib/issues/9#issuecomment-3448256063

*/

[DebuggerDisplay("{HotKey}")]
public class PlainHotkeyControl : Control
{
    private sealed class ParentNativeWindow : NativeWindow
    {
        private readonly PlainHotkeyControl Ctrl;

        public ParentNativeWindow(PlainHotkeyControl ctrl)
        {
            AssignHandle(ctrl.Parent.Handle);
            Ctrl = ctrl;
        }

        protected override void WndProc(ref Message m)
        {
            const int EN_CHANGE = 0x0300;
            const int WM_COMMAND = 0x0111;

            if (m.Msg == WM_COMMAND && m.LParam == Ctrl.Handle && m.WParam.ToInt32().HiWord == EN_CHANGE)
            {
                Ctrl.OnHotKeyChanged();
            }

            base.WndProc(ref m);
        }
    }

    public event EventHandler HotKeyChanged;

    public Hotkey Hotkey
    {
        get
        {
            if (IsHandleCreated)
            {
                const int HKM_GETHOTKEY = 0x0400 + 2;
                hotkey = new((ushort)Win32UI.SendMessage(Handle, HKM_GETHOTKEY, 0, 0).ToInt32());
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
            const int WS_BORDER = 0x00800000;
            var cp = base.CreateParams;
            cp.ClassName = "msctls_hotkey32";
            cp.Style |= WS_BORDER;
            return cp;
        }
    }

    protected override Size DefaultMinimumSize => new(100, 21);

    private Hotkey hotkey;
    private readonly bool UseDark = ThemeManager.ShouldUseDarkMode;
    private readonly IntPtr hBrush = Win32UI.CreateSolidBrush(Colors.DarkBackText);

    public PlainHotkeyControl()
    {
        SetStyle(ControlStyles.UserPaint, false);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        const int WS_EX_CLIENTEDGE = 0x00000200;
        Win32UI.RemoveWindowExStyle(Handle, WS_EX_CLIENTEDGE);

        const int HKM_SETRULES = 0x0400 + 3;
        const int HKCOMB_NONE = 0x0001;
        const int HKCOMB_S = 0x0002;
        Win32UI.SendMessage(Handle, HKM_SETRULES, HKCOMB_NONE | HKCOMB_S, (int)(HotkeyF.Ctrl | HotkeyF.Alt));

        SetHotKey(hotkey);

        new ParentNativeWindow(this);
    }

    protected override void WndProc(ref Message m)
    {
        if (UseDark)
        {
            const int WM_ERASEBKGND = 0x0014;
            const int WM_PAINT = 0x000F;
            const int WM_DESTROY = 0x0002;

            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    Win32UI.GetClientRect(m.HWnd, out var rc);
                    Win32UI.FillRect(m.WParam, ref rc, hBrush);
                    m.Result = new(1);
                    return;
                case WM_PAINT:
                    Win32UI.ComctlHookSysColor(Colors.DarkForeText, Colors.DarkBackText);
                    base.WndProc(ref m);
                    Win32UI.ComctlUnhookSysColor();
                    return;
                case WM_DESTROY:
                    Win32UI.DeleteObject(hBrush);
                    break;
            }
        }

        base.WndProc(ref m);
    }

    private void SetHotKey(Hotkey hk)
    {
        const int HKM_SETHOTKEY = 0x0400 + 1;
        Win32UI.SendMessage(Handle, HKM_SETHOTKEY, hk, 0);
    }

    private void OnHotKeyChanged()
    {
        HotKeyChanged?.Invoke(this, EventArgs.Empty);
    }
}