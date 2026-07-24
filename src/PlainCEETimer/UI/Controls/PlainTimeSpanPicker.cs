using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

[NoConstants]
public sealed class PlainTimeSpanPicker : UpDownBase, IThemeAware
{
    public unsafe int MaxDays
    {
        get
        {
            if (IsHandleCreated)
            {
                fixed (int* ptr = &m_maxdays)
                {
                    Win32UI.SendMessage(Handle, PTSPM_GETDAYSMAX, 0, ptr);
                }
            }

            return m_maxdays;
        }

        set
        {
            m_maxdays = value;

            if (IsHandleCreated)
            {
                SetMaxDays();
            }
        }
    }

    public unsafe TimeSpan Value
    {
        get
        {
            if (IsHandleCreated)
            {
                fixed (long* ptr = &m_ticks)
                {
                    Win32UI.SendMessage(Handle, PTSPM_GETVALUE, 0, ptr);
                }
            }

            return new(m_ticks);
        }

        set
        {
            m_ticks = value.Ticks;

            if (IsHandleCreated)
            {
                SetValue();
            }
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ClassName = Win32Controls.WC_PLAINTIMESPANPICK;
            return cp;
        }
    }

    protected override Size DefaultMinimumSize => new(50, 23);

    public event EventHandler ValueChanged;

    private long m_ticks;
    private int m_maxdays = 65535;
    private ThemeHelper themeHelper;
    private readonly Debouncer debouncer;
    private readonly ActionInvoker OnValueChangedAction;

    private const int PTSPM_GETVALUE = WM.USER + 0x0010;
    private const int PTSPM_SETVALUE = WM.USER + 0x0011;
    private const int PTSPM_GETDAYSMAX = WM.USER + 0x0012;
    private const int PTSPM_SETDAYSMAX = WM.USER + 0x0013;
    private const int PTSPM_OVERRIDECOLORS = WM.USER + 0x0014;
    private const int PTSPM_INCREASE = WM.USER + 0x0015;
    private const int PTSPN_VALUECHANGE = 1;
    private const int PTSPCOLOR_BACKTEXT = 0;
    private const int PTSPCOLOR_FORETEXT = 1;
    private const int PTSPCOLOR_FORETEXTDISABLED = 2;
    private const int PTSPCOLOR_RESTORE = 0xFF;

    public PlainTimeSpanPicker()
    {
        SetStyle(ControlStyles.UserPaint, false);
        OnValueChangedAction = new(OnValueChangedImpl);
        debouncer = new(new ControlDebounceHelper(this));
    }

    static PlainTimeSpanPicker()
    {
        Win32Controls.PlainTimeSpanPick_RegisterWC();
    }

    public override void DownButton()
    {
        Increase(-1);
    }

    public override void UpButton()
    {
        Increase(1);
    }

    protected override void UpdateEditText()
    {
        return;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        Controls[1].Visible = false;
        base.OnHandleCreated(e);
        SetMaxDays();
        SetValue();
        themeHelper = new(this);
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
            case WM.REFLECT + WM.COMMAND:
                if (m.WParam.ToInt32().HiWord == PTSPN_VALUECHANGE)
                    OnValueChanged();
                return;
            default:
                break;
        }

        base.WndProc(ref m);
    }

    private unsafe void SetMaxDays()
    {
        fixed (int* ptr = &m_maxdays)
        {
            Win32UI.SendMessage(Handle, PTSPM_SETDAYSMAX, 0, ptr);
        }
    }

    private unsafe void SetValue()
    {
        fixed (long* ptr = &m_ticks)
        {
            Win32UI.SendMessage(Handle, PTSPM_SETVALUE, 0, ptr);
        }
    }

    private void Increase(int i)
    {
        if (IsHandleCreated)
        {
            Win32UI.SendMessage(Handle, PTSPM_INCREASE, i, 0);
        }
    }

    private void OnValueChanged()
    {
        debouncer.Debounce(OnValueChangedAction);
    }

    private void OnValueChangedImpl()
    {
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        if (IsHandleCreated)
        {
            var state = (!init).ToWin32();
            var hwnd = Handle;

            if (useDark)
            {
                BackColor = Colors.DarkBackText;
                Win32UI.SendMessage(hwnd, PTSPM_OVERRIDECOLORS,
                    state, int.MakeLong24(Colors.DarkBackText.ToWin32(), PTSPCOLOR_BACKTEXT));
                Win32UI.SendMessage(hwnd, PTSPM_OVERRIDECOLORS,
                    state, int.MakeLong24(Colors.DarkForeText.ToWin32(), PTSPCOLOR_FORETEXT));
                Win32UI.SendMessage(hwnd, PTSPM_OVERRIDECOLORS,
                    state, int.MakeLong24(Colors.DarkForeTextDisabled.ToWin32(), PTSPCOLOR_FORETEXTDISABLED));
                ThemeManager.EnableDarkModeForControl(Controls[0], SystemStyle.ExplorerDark);
            }
            else
            {
                BackColor = SystemColors.Window;
                Win32UI.SendMessage(hwnd, PTSPM_OVERRIDECOLORS,
                    state, int.MakeLong24(0, PTSPCOLOR_RESTORE));
                ThemeManager.EnableDarkModeForControl(Controls[0], SystemStyle.Explorer);
            }
        }
    }
}