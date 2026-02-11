using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.UI.Controls;

public abstract class AppForm : Form
{
    /// <summary>
    /// 获取当前 <see cref="AppForm"/> 的消息框实例。
    /// </summary>
    public AppMessageBox MessageX { get; }

    public Control FocusControl { get; set; }

    protected virtual AppFormParam Params => AppFormParam.None;

    public event Action<DialogResult> DialogEnd;

    private bool IsLoading = true;
    private bool SetRoundRegion;
    private bool canSaveSize;
    private FormWindowState lastState;
    private Size lastSize;
    private readonly AppFormParam ParamsInternal;
    private readonly int RoundCornerRadius = 13;
    private readonly bool IsSizable;
    private readonly bool SetRoundCorner;
    private readonly bool SmallRoundCorner;
    private readonly bool Special;
    private readonly bool OnEscClosing;

    private static float DpiRatio;
    private static bool IsHighDpi;
    private static readonly Font AppFont;
    private static readonly int CurrentFontHeight;

    protected AppForm()
    {
        ParamsInternal = Params;
        Special = CheckParam(AppFormParam.Special);
        OnEscClosing = CheckParam(AppFormParam.OnEscClosing);
        KeyPreview = CheckParam(AppFormParam.KeyPreview);
        SetRoundCorner = CheckParam(AppFormParam.RoundCorner);
        SmallRoundCorner = CheckParam(AppFormParam.RoundCornerSmall);
        IsSizable = CheckParam(AppFormParam.Sizable);
        App.ActivateMain += App_ActivateMain;
        MessageX = new(this);

        if (!Special)
        {
            MainForm.UniTopMostChanged += MainForm_UniTopMostChanged;
            MainForm_UniTopMostChanged();
        }

        SuspendLayout();
        AutoScaleDimensions = new(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = AppFont;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.Manual;
        ShowIcon = false;

        if (IsSizable)
        {
            MaximizeBox = true;
            FormBorderStyle = FormBorderStyle.Sizable;
            SizeGripStyle = SizeGripStyle.Hide;
        }

        if (SetRoundCorner)
        {
            ControlBox = false;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;

            if (SmallRoundCorner)
            {
                RoundCornerRadius = 7;
            }

            ClientSize = default;
        }

        OnInitializing();
        ResumeLayout(true);
    }

    static AppForm()
    {
        AppFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        CurrentFontHeight = AppFont.Height;
    }

    public void ReActivate()
    {
        var tmp = TopMost;
        TopMost = true;
        WindowState = FormWindowState.Normal;
        Show();
        Activate();
        TopMost = tmp;
        KeepOnScreen();
    }

    protected sealed override void OnLoad(EventArgs e)
    {
        SuspendLayout();
        RunLayout(IsHighDpi);
        InitToUserSize();
        ResumeLayout(true);
        OnLoad();
        base.OnLoad(e);

        if (CheckParam(AppFormParam.CenterScreen))
        {
            CenterToScreen();
        }
    }
    protected sealed override void OnShown(EventArgs e)
    {
        IsLoading = false;
        OnShown();
        FocusControl?.Focus();
        base.OnShown(e);
    }

    protected override void OnResizeBegin(EventArgs e)
    {
        if (IsSizable)
        {
            SuspendLayout();
        }

        base.OnResizeBegin(e);
    }

    protected override void OnResizeEnd(EventArgs e)
    {
        if (IsSizable)
        {
            Size = KeepOnScreen(Size, true);
            ResumeLayout();
        }

        base.OnResizeEnd(e);
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        if (SetRoundRegion && SetRoundCorner)
        {
            Win32UI.SetRoundCorner(Handle, Width, Height, ScaleToDpi(RoundCornerRadius));
        }

        base.OnSizeChanged(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (OnEscClosing && e.KeyCode == Keys.Escape)
        {
            Close();
        }

        base.OnKeyDown(e);
    }

    protected sealed override void OnFormClosing(FormClosingEventArgs e)
    {
        var reason = e.CloseReason;
        e.Cancel = reason != CloseReason.WindowsShutDown && OnClosing(reason);
        base.OnFormClosing(e);
    }

    protected sealed override void OnClosed(EventArgs e)
    {
        OnClosed();
        SaveWindowParameters();
        base.OnClosed(e);
        Dispose(true);

        if (CheckParam(AppFormParam.ModelessDialog))
        {
            DialogEnd?.Invoke(DialogResult);
            DialogEnd = null;
        }

        App.ActivateMain -= App_ActivateMain;
    }

    protected sealed override CreateParams CreateParams
    {
        get
        {
            const int WS_EX_COMPOSITED = 0x02000000;
            var cp = base.CreateParams;

            if (CheckParam(AppFormParam.CompositedStyle))
            {
                cp.ExStyle |= WS_EX_COMPOSITED;
            }

            return cp;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        if (DpiRatio == 0F)
        {
            using var g = CreateGraphics();
            DpiRatio = g.DpiX / 96F;
            IsHighDpi = DpiRatio > 1F;
        }

        if (ThemeManager.ShouldUseDarkMode)
        {
            if (!Special)
            {
                ForeColor = Colors.DarkForeText;
                BackColor = Colors.DarkBackText;
            }

            ThemeManager.EnableDarkModeForWindow(Handle);
        }

        if (SetRoundCorner)
        {
            if (SystemVersion.IsWindows11)
            {
                Win32UI.SetRoundCornerEx(Handle, SmallRoundCorner);
            }
            else
            {
                SetRoundRegion = true;
            }
        }

        if (IsSizable)
        {
            SystemMenu.From(this)
                .InsertItem(-2, "默认大小(&D)", (_, _) =>
                {
                    var def = MinimumSize;

                    if (def != Size)
                    {
                        Size = def;
                    }
                })

                .InsertSeparator(-2);
        }

        base.OnHandleCreated(e);
    }

    /// <summary>
    /// 在 <see cref="AppForm"/> 完成设定基本选项时触发，可用于让派生类修改有关窗体的属性，这将覆盖先前设定的选项。
    /// 该方法没有默认实现，可不调用 base.OnInitialize();
    /// </summary>
    protected virtual void OnInitializing() { }

    /// <summary>
    /// 在 <see cref="AppForm"/> OnLoad 之前触发，可用于对控件进行最后的布局。
    /// 该方法没有默认实现，可不调用 base.RunLayout(bool);
    /// </summary>
    protected virtual void RunLayout(bool isHighDpi) { }

    /// <summary>
    /// 在 <see cref="AppForm"/> 加载时触发。该方法没有默认实现，直接派生自 <see cref="AppForm"/> 的类可不调用 base.OnLoad();
    /// </summary>
    protected virtual void OnLoad() { }

    /// <summary>
    /// 在 <see cref="AppForm"/> 已向用户显示时触发。该方法没有默认实现，可不调用 base.OnShown();
    /// </summary>
    protected virtual void OnShown() { }

    /// <summary>
    /// 在 <see cref="AppForm"/> 被关闭时触发。该方法默认返回 <see langword="false"/>，可不调用 base.OnClosing(CloseReason);
    /// </summary>
    /// <returns><see langword="true"/> 则取消关闭窗口, <see langword="false"/> 则允许关闭窗口</returns>
    protected virtual bool OnClosing(CloseReason closeReason) => false;

    /// <summary>
    /// 在 <see cref="AppForm"/> 关闭后触发。该方法没有默认实现，可不调用 base.OnClosed();
    /// </summary>
    protected virtual void OnClosed() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool CheckParam(AppFormParam param)
    {
        return (ParamsInternal & param) == param;
    }

    /// <summary>
    /// 在用户未保存更改并尝试关闭窗体时显示警告。同时防止直接关闭警告时也窗体会随之关闭。
    /// </summary>
    protected bool ShowUnsavedWarning(string msg, Func<bool> SaveChanges, ref bool flagUserChanged)
    {
        switch (MessageX.Warn(msg, MessageButtons.YesNo))
        {
            case DialogResult.Yes:
                return !SaveChanges();
            case DialogResult.No:
                flagUserChanged = false;
                Close();
                return false;
            default:
                return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int ScaleToDpi(int px)
    {
        return (int)(px * DpiRatio);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int UnscaleToDpi(int px)
    {
        return (int)(px / DpiRatio);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected Size ScaleToDpi(Size sz)
    {
        return new(ScaleToDpi(sz.Width), ScaleToDpi(sz.Height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected Size UnscaleToDpi(Size sz)
    {
        return new(UnscaleToDpi(sz.Width), UnscaleToDpi(sz.Height));
    }

    protected bool IsModkeysPressed(Keys keys)
    {
        return (ModifierKeys & keys) == keys;
    }

    protected Rectangle GetCurrentScreenRect()
    {
        return Special ? Screen.GetWorkingArea(this) : Screen.GetWorkingArea(Cursor.Position);
    }

    protected void ArrangeFirstControl(Control control, int x = 3, int y = 3)
    {
        control.SetBounds(x, y, 0, 0, BoundsSpecified.Location);
    }

    /// <summary>
    /// 参考指定控件，在 X 方向上水平排列目标控件，并在 Y 方向上与指定控件的上边缘对齐
    /// </summary>
    protected void ArrangeControlXT(Control target, Control reference, int xOffset = 0, int yOffset = 0)
    {
        target.SetBounds(reference.Right + ScaleToDpi(xOffset), reference.Top + ScaleToDpi(yOffset), 0, 0, BoundsSpecified.Location);
    }

    /// <summary>
    /// 参考指定控件，在 X 方向上水平排列目标控件，与 <paramref name="reference1"/> 左边缘对齐，并在 Y 方向上与 <paramref name="reference2"/> 上边缘对齐。
    /// </summary>
    protected void ArrangeControlXLT(Control target, Control reference1, Control reference2, int xOffset = 0, int yOffset = 0)
    {
        target.SetBounds(reference1.Left + ScaleToDpi(xOffset), reference2.Top + ScaleToDpi(yOffset), 0, 0, BoundsSpecified.Location);
    }

    /// <summary>
    /// 参考指定控件，在 X 方向上水平排列目标控件，与 <paramref name="reference1"/> 右边缘对齐，并在 Y 方向上与 <paramref name="reference2"/> 上边缘对齐。
    /// </summary>
    protected void ArrangeControlXRT(Control target, Control reference1, Control reference2, int xOffset = 0, int yOffset = 0)
    {
        target.SetBounds(reference1.Right + ScaleToDpi(xOffset), reference2.Top + ScaleToDpi(yOffset), 0, 0, BoundsSpecified.Location);
    }

    /// <summary>
    /// 参考指定控件，在 Y 方向上竖直排列目标控件，并在 X 方向上与指定控件的左边缘对齐
    /// </summary>
    protected void ArrangeControlYL(Control target, Control reference, int xOffset = 0, int yOffset = 0)
    {
        target.SetBounds(reference.Left + ScaleToDpi(xOffset), reference.Bottom + ScaleToDpi(yOffset), 0, 0, BoundsSpecified.Location);
    }

    protected void ArrangeCommonButtonsR(PlainButton button1, PlainButton button2, Control reference, int xOffset = 0, int yOffset = 0)
    {
        button2.SetBounds(reference.Right - button2.Width + ScaleToDpi(xOffset), reference.Bottom + ScaleToDpi(yOffset), 0, 0, BoundsSpecified.Location);
        button1?.SetBounds(button2.Left - button1.Width - ScaleToDpi(3), button2.Top, 0, 0, BoundsSpecified.Location);
    }

    /// <summary>
    /// 将目标控件的左边缘在 X 方向上与参考控件的左边缘对齐
    /// </summary>
    protected void AlignControlXL(Control target, Control reference, int xOffset = 0)
    {
        target.Left = reference.Left + ScaleToDpi(xOffset);
    }

    protected void AlignControlXR(Control target, Control reference, int xOffset = 0)
    {
        target.Left = reference.Right - target.Width + ScaleToDpi(xOffset);
    }

    /// <summary>
    /// 将目标控件在 Y 方向上与参考控件居中
    /// </summary>
    protected void CenterControlY(Control target, Control reference, int yOffset = 0)
    {
        target.Top = reference.Top + (reference.Height - target.Height) / 2 + ScaleToDpi(yOffset);
    }

    /// <summary>
    /// 将目标控件在 X 方向上与参考控件保持紧凑
    /// </summary>
    protected void CompactControlX(Control target, Control reference, int xOffset = 0)
    {
        target.Left = reference.Right + ScaleToDpi(xOffset);
    }

    /// <summary>
    /// 将目标控件在 Y 方向上与参考控件保持紧凑
    /// </summary>
    protected void CompactControlY(Control target, Control reference, int yOffset = 0)
    {
        target.Top = reference.Bottom + ScaleToDpi(yOffset);
    }

    protected void GroupBoxArrageControl(Control target, int xOffset = 0, int yOffset = 0)
    {
        target.SetBounds(6 + ScaleToDpi(xOffset), CurrentFontHeight + ScaleToDpi(yOffset), 0, 0, BoundsSpecified.Location);
    }

    protected void GroupBoxAutoAdjustHeight(PlainGroupBox groupBox, Control yLast, int yOffset = 0)
    {
        groupBox.Height = yLast.Bottom + ScaleToDpi(yOffset);
    }

    protected void InitWindowSize(Control xyLast, int xOffset = 0, int yOffset = 0)
    {
        ClientSize = new(xyLast.Right + ScaleToDpi(xOffset), xyLast.Bottom + ScaleToDpi(yOffset));
        MinimumSize = Size;
    }

    protected void EndModelessDialog(bool success, bool close = true)
    {
        DialogResult = success ? DialogResult.OK : DialogResult.None;

        if (close)
        {
            Close();
        }
    }

    /// <summary>
    /// 仅当窗体加载完成再执行指定的代码。
    /// </summary>
    protected void EnsureLoaded(Action action)
    {
        if (!IsLoading)
        {
            action();
        }
    }

    protected void SetLocation(int x, int y)
    {
        SetBounds(x, y, 0, 0, BoundsSpecified.Location);
    }

    protected void SetLabelAutoWrap(PlainLabel target, bool useParent)
    {
        SetLabelAutoWrap(target, useParent ? (target.Parent.Width - target.Left) : (int)(GetCurrentScreenRect().Width * 0.75));
    }

    protected void SetLabelAutoWrap(PlainLabel target, int maxWidth)
    {
        /*

        Label 控件自动换行 参考:

        c# - Word wrap for a label in Windows Forms - Stack Overflow
        https://stackoverflow.com/a/3680595/21094697

        */

        target.MaximumSize = new(maxWidth, 0);
        target.AutoSize = true;
    }

    protected Point KeepOnScreen()
    {
        var screen = GetCurrentScreenRect();
        var x = Left.Clamp(screen.X, screen.Right - Width);
        var y = Top.Clamp(screen.Y, screen.Bottom - Height);
        SetLocation(x, y);
        return new(x, y);
    }

    protected Size KeepOnScreen(Size sz, bool keep)
    {
        var screen = GetCurrentScreenRect();
        var w = sz.Width.Clamp(0, screen.Width);
        var h = sz.Height.Clamp(0, screen.Height);
        sz = new(w, h);

        if (keep && sz != lastSize)
        {
            var x = Left.Clamp(screen.X, screen.Right - Width);
            var y = Top.Clamp(screen.Y, screen.Bottom - Height);
            SetLocation(x, y);
        }

        return sz;
    }

    private void InitToUserSize()
    {
        if (IsSizable)
        {
            var dic = App.AppConfig.Sizes;

            if (dic != null && dic.TryGetValue(Name, out var szobj))
            {
                var sz = ScaleToDpi(szobj.Size);

                if (sz != Size.Empty && sz != ClientSize)
                {
                    ClientSize = KeepOnScreen(sz, false);
                }

                if (szobj.Maximize)
                {
                    WindowState = lastState = FormWindowState.Maximized;
                }

                lastSize = Size;
            }
        }
    }

    private void SaveWindowParameters()
    {
        if (IsSizable)
        {
            var sz = Size;
            var csz = ClientSize;
            var usz = IsHighDpi ? UnscaleToDpi(sz) : sz;
            var ucsz = UnscaleToDpi(csz);
            var st = WindowState == FormWindowState.Maximized;
            var changed = sz != lastSize;
            var isdef = sz == MinimumSize;

            if ((st ^ (lastState == FormWindowState.Maximized)) || (changed && !isdef && !st))
            {
                App.AppConfig.Sizes[Name] = new(st, (st || isdef) ? Size.Empty : ucsz);
                ConfigValidator.DemandConfig();
            }
        }
    }

    private void App_ActivateMain()
    {
        if (!IsDisposed)
        {
            ReActivate();
            KeepOnScreen();
        }
    }

    private void MainForm_UniTopMostChanged()
    {
        TopMost = !IsDisposed && MainForm.UniTopMost;
    }
}
