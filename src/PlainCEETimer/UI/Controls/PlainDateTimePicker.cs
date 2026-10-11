using System;
using System.Globalization;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainDateTimePicker : DateTimePicker, IThemeAwareEx
{
    private sealed class DropDownAndSysMonthCal32NativeWindow : NativeWindow
    {
        internal PlainDateTimePicker m_owner;

        private bool dragging;
        private bool canHook;
        private Debouncer debouncer;
        private readonly ActionInvoker UnhookAction;

        public DropDownAndSysMonthCal32NativeWindow()
        {
            UnhookAction = new(DelayUnhook);
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
                        goto proceed;
                    case WinUser.WM_TIMER:
                    case WinUser.WM_KEYDOWN:
                    case WinUser.WM_MOUSEWHEEL:
                    case WinUser.WM_MOUSEMOVE when dragging:
                    case WinUser.WM_MENUSELECT when m.LParam == IntPtr.Zero:
                    proceed:
                        SafeHook();
                        debouncer ??= new(uiCritical: true);
                        debouncer.Debounce(UnhookAction);
                        break;
                    case WinUser.WM_PAINT:
                        HookCore();
                        base.WndProc(ref m);
                        UnhookCore();
                        return;
                    case WinUser.WM_NCDESTROY:
                        debouncer.Destroy();
                        SafeUnhook();
                        break;
                }
            }

            base.WndProc(ref m);
        }

        private void SafeHook()
        {
            if (!canHook)
            {
                HookCore();
                canHook = true;
            }
        }

        private void SafeUnhook()
        {
            if (canHook)
            {
                UnhookCore();
                canHook = false;
            }
        }

        private void DelayUnhook()
        {
            if (!dragging)
            {
                SafeUnhook();
            }
        }

        private static void HookCore()
        {
            Win32UI.PnHookThemedPaint();
        }

        private static void UnhookCore()
        {
            Win32UI.PnUnhookThemedPaint();
        }
    }

    private sealed class RelativeValueDialog(PlainDateTimePicker owner) : AppDialog
    {
        public DateTime SelectedValue => m_current;

        protected override bool GrayButtonA => false;

        private DateTime m_current;
        private DateTime m_value;
        private PlainLabel LabelCurrent;
        private PlainLabel LabelSet;
        private PlainLabel LabelPreview;
        private PlainTimeSpanPicker PtspMain;
        private PlainComboBox ComboBoxType;

        protected override void OnInitializing()
        {
            var option = owner.AccessibilityObject.Name is var optionName && string.IsNullOrEmpty(optionName)
                ? string.Empty : $" ({optionName.Truncate(10, true)})";
            m_current = owner.Value;
            Text = "设置相对值 - DateTimePicker" + option;

            this.AddControls(b =>
            [
                LabelCurrent = b.Label("0"),
                LabelSet = b.Label("设置为: "),
                PtspMain = b.TimeSpanPicker(155, UpdatePreview),
                ComboBoxType = b.ComboBox(55, UpdatePreview, ["之后", "之前"]),
                LabelPreview = b.Label("0")
            ]);

            LabelCurrent.Text = $"当前值: {m_current.ToString(ConfigValidator.DTPFormat)}";
            UpdatePreview(this, EventArgs.Empty);
            base.OnInitializing();
        }

        protected override void RunLayout(bool init, bool isHighDpi)
        {
            ArrangeFirstControl(LabelCurrent, 6);
            CompactControlY(PtspMain, LabelCurrent, 3);
            ArrangeControlYL(LabelSet, LabelCurrent);
            CenterControlY(LabelSet, PtspMain);
            CompactControlX(PtspMain, LabelSet);
            ArrangeControlXT(ComboBoxType, PtspMain, 6);
            ArrangeControlYL(LabelPreview, LabelSet);
            CompactControlY(LabelPreview, PtspMain, 1);
            ArrangeCommonButtonsR(ButtonA, ButtonB, ComboBoxType, LabelPreview, 3, 3);
            InitWindowSize(ButtonB, 6, 6);
        }

        protected override void OnLoad()
        {
            var g = App.Current.AppConfig.General;
            PtspMain.Value = g.PreferredTS;
            ComboBoxType.SelectedIndex = g.PreferMinusTS.ToWin32();
        }

        protected override bool OnClickButtonA()
        {
            var g = App.Current.AppConfig.General;
            var v = PtspMain.Value;
            g.PreferredTS = v;
            g.PreferMinusTS = ComboBoxType.SelectedIndex.ToBoolean();
            m_current = m_value;
            ConfigValidator.DemandConfig();
            return base.OnClickButtonA();
        }

        private void UpdatePreview(object sender, EventArgs e)
        {
            var offset = PtspMain.Value;
            var negative = ComboBoxType.SelectedIndex.ToBoolean();
            offset = negative ? new(-offset.Ticks) : offset;
            var value = SafeAdd(m_current, offset);
            LabelPreview.Text = "目标值: " + value.ToString(ConfigValidator.DTPFormat);
            m_value = value;
        }

        private DateTime SafeAdd(DateTime value, TimeSpan offset)
        {
            var min = owner.MinDate.Ticks;
            var max = owner.MaxDate.Ticks;

            DateTime result;

            try
            {
                result = value.Add(offset);
            }
            catch (ArgumentOutOfRangeException)
            {
                result = offset.Ticks < 0L ? DateTime.MinValue : DateTime.MaxValue;
            }
            catch
            {
                result = value;
            }

            var ticks = result.Ticks.Clamp(min, max);
            return new(ticks);
        }
    }

    private bool UseDark;
    private AppForm parentForm;
    private RelativeValueDialog dialog;
    private DropDownAndSysMonthCal32NativeWindow m_ddnw;
    private DropDownAndSysMonthCal32NativeWindow m_smcnw;

    static PlainDateTimePicker()
    {
        Win32Controls.DateTimePick_InitClass();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        parentForm = this.FindParentForm();
        ThemeHelper.Attach(this);
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
        switch (m.Msg)
        {
            case WinUser.WM_COMMAND:

                switch (m.WParam.LoWord)
                {
                    case Natives.IDM_DPM_COPY:
                        Copy();
                        break;
                    case Natives.IDM_DPM_PASTE:
                        Paste();
                        break;
                    case Natives.IDM_DPM_RELATIVE:
                        OpenDialog();
                        break;
                }

                break;

            case WinUser.WM_LBUTTONDBLCLK:
                Paste();
                return;

            case WinUser.WM_PAINT when UseDark:
                Win32UI.PnHookThemedPaint();
                base.WndProc(ref m);
                Win32UI.PnUnhookThemedPaint();
                return;
        }

        base.WndProc(ref m);
    }

    private void Copy()
    {
        Clipboard.SetText(Text);
    }

    private void Paste()
    {
        if (Clipboard.ContainsText())
        {
            var text = Clipboard.GetText();

            if (DateTime.TryParseExact(text, ConfigValidator.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
                || DateTime.TryParse(text, out result))
            {
                Value = result;
            }
        }
    }

    private void OpenDialog()
    {
        if (dialog == null || dialog.IsDisposed)
        {
            dialog = new(this);
        }

        if (dialog.ShowDialog(parentForm) == true)
        {
            Value = dialog.SelectedValue;
        }
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;
    }
}
