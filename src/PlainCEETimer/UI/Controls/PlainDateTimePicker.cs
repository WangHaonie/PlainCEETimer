using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainDateTimePicker : DateTimePicker, IThemeAware
{
    private sealed class DropDownAndSysMonthCal32NativeWindow : NativeWindow
    {
        internal PlainDateTimePicker m_owner;

        private bool dragging;
        private bool canHook = true;
        private Debouncer debouncer;
        private readonly ActionInvoker<bool> UnhookAction;

        public DropDownAndSysMonthCal32NativeWindow()
        {
            UnhookAction = new(Unhook);
        }

        protected override void WndProc(ref Message m)
        {
            if (m_owner.UseDark)
            {
                switch (m.Msg)
                {
                    case WinUser.WM_LBUTTONDOWN:
                        dragging = true;
                        goto proceed;
                    case WinUser.WM_LBUTTONUP:
                        dragging = false;
                        break;
                    case WinUser.WM_TIMER:
                    case WinUser.WM_KEYDOWN:
                    case WinUser.WM_MOUSEWHEEL:
                    case WinUser.WM_MOUSEMOVE when dragging:
                    proceed:
                        if (canHook) Hook(true);
                        debouncer ??= new();
                        debouncer.Debounce(UnhookAction.WithArgs(true));
                        break;

                    case WinUser.WM_PAINT:
                        Hook(false);
                        base.WndProc(ref m);
                        Unhook(false);
                        return;
                }
            }

            base.WndProc(ref m);
        }

        private void Hook(bool flag)
        {
            Win32UI.PnHookThemedPaint();
            if (flag) canHook = false;
        }

        private void Unhook(bool flag)
        {
            Win32UI.PnUnhookThemedPaint();
            if (flag) canHook = true;
        }
    }

    private bool UseDark;
    private ThemeHelper themeHelper;
    private DropDownAndSysMonthCal32NativeWindow m_ddnw;
    private DropDownAndSysMonthCal32NativeWindow m_smcnw;

    protected override void OnHandleCreated(EventArgs e)
    {
        themeHelper ??= new(this);
        base.OnHandleCreated(e);
    }

    protected override void OnDropDown(EventArgs eventargs)
    {
        var hmc = Win32UI.SendMessage(Handle, CommCtrl.DTM_GETMONTHCAL, 0, 0);
        NativeWindowHelper.Attach(hmc, ref m_smcnw);
        NativeWindowHelper.Attach(Win32UI.GetParent(hmc), ref m_ddnw);
        m_smcnw.m_owner = this;
        m_ddnw.m_owner = this;
        base.OnDropDown(eventargs);
    }

    protected override void OnCloseUp(EventArgs eventargs)
    {
        NativeWindowHelper.Detach(m_ddnw);
        NativeWindowHelper.Detach(m_smcnw);
        base.OnCloseUp(eventargs);
    }

    protected override void WndProc(ref Message m)
    {
        if (UseDark)
        {
            switch (m.Msg)
            {
                case WinUser.WM_PAINT:
                    Win32UI.PnHookThemedPaint();
                    base.WndProc(ref m);
                    Win32UI.PnUnhookThemedPaint();
                    return;
            }
        }

        base.WndProc(ref m);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;
    }
}
