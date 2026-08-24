using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainDateTimePicker : DateTimePicker, IThemeAware
{
    private sealed class DropDownAndSysMonthCal32NativeWindow(PlainDateTimePicker owner) : NativeWindow
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinUser.WM_PAINT && owner.UseDark)
            {
                Win32UI.ComctlHookThemeBackground();
                base.WndProc(ref m);
                Win32UI.ComctlUnhookThemeBackground();
                return;
            }

            base.WndProc(ref m);
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
        m_smcnw ??= new(this);
        m_ddnw ??= new(this);
        var hmc = Win32UI.SendMessage(Handle, CommCtrl.DTM_GETMONTHCAL, 0, 0);
        var hdd = Win32UI.GetParent(hmc);
        m_smcnw.AssignHandle(hmc);
        m_ddnw.AssignHandle(hdd);
        base.OnDropDown(eventargs);
    }

    protected override void OnCloseUp(EventArgs eventargs)
    {
        m_ddnw?.ReleaseHandle();
        m_smcnw?.ReleaseHandle();
        base.OnCloseUp(eventargs);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WinUser.WM_PAINT && UseDark)
        {
            Win32UI.ComctlHookThemeBackground();
            base.WndProc(ref m);
            Win32UI.ComctlUnhookThemeBackground();
            return;
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
