using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
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
    protected event Action LocationRefreshed;

    private bool IsLoading = true;
    private bool SetRoundRegion;
    private readonly AppFormParam ParamsInternal;
    private readonly int RoundCornerRadius = 13;
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
        App.TrayMenuShowAllClicked += AppLauncher_TrayMenuShowAllClicked;
        MessageX = new(this);

        if (!Special)
        {
            MainForm.UniTopMostChanged += MainForm_UniTopMostChanged;
            MainForm_UniTopMostChanged();
        }

        SuspendLayout();
        AutoScaleDimensions = new(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Font = AppFont;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.Manual;
        ShowIcon = false;

        if (SetRoundCorner)
        {
            AutoSize = false;
            ControlBox = false;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;

            if (SmallRoundCorner)
            {
                RoundCornerRadius = 7;
            }
        }

        ClientSize = new();
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

    internal void ShowFlyout(AppForm flyout, Func<Point> GetPos)
    {
        LocationChanged += (_, _) =>
        {
            if (!flyout.IsDisposed)
            {
                flyout.Location = GetPos();
            }
        };

        flyout.Show(this);
    }

    protected sealed override void OnLoad(EventArgs e)
    {
        SuspendLayout();
        RunLayout(IsHighDpi);
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
        base.OnClosed(e);
        Dispose(true);

        if (CheckParam(AppFormParam.ModelessDialog))
        {
            DialogEnd?.Invoke(DialogResult);
            DialogEnd = null;
        }
    }

    protected sealed override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;

            if (CheckParam(AppFormParam.CompositedStyle))
            {
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
            }

            return cp;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        if (DpiRatio == 0F)
        {
            var g = CreateGraphics();
            DpiRatio = g.DpiX / 96F;
            IsHighDpi = DpiRatio > 1F;
            g.Dispose();
        }

        if (ThemeManager.ShouldUseDarkMode)
        {
            if (!Special)
            {
                ForeColor = Colors.DarkForeText;
                BackColor = Colors.DarkBackText;
            }

            ThemeManager.EnableDarkMode(Handle);
        }

        if (SetRoundCorner)
        {
            if (SystemVersion.IsWindows11)
            {
                Win32UI.SetRoundCornerEx(Handle, (bool)SmallRoundCorner);
            }
            else
            {
                SetRoundRegion = true;
            }
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
    /// 该方法没有默认实现，可不调用 base.StartLayout(bool);
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

    protected void GroupBoxArrageFirstControl(Control target, int xOffset = 0, int yOffset = 0)
    {
        target.SetBounds(6 + ScaleToDpi(xOffset), CurrentFontHeight + ScaleToDpi(yOffset), 0, 0, BoundsSpecified.Location);
    }

    protected void GroupBoxAutoAdjustHeight(PlainGroupBox groupBox, Control yLast, int yOffset = 0)
    {
        groupBox.Height = yLast.Bottom + ScaleToDpi(yOffset);
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

    /// <summary>
    /// 以父容器宽度为参考使 Label 单行内容达到一定长度时自动换行。
    /// </summary>
    /// <param name="target">目标 Label</param>
    protected void SetLabelAutoWrap(PlainLabel target)
    {
        SetLabelAutoWrap(target, target.Parent.Width - target.Left);
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

    protected void RemoveControls(Control parent, params Control[] controls)
    {
        var ctrls = parent.Controls;

        foreach (var c in controls)
        {
            ctrls.Remove(c);
            c.Dispose();
        }
    }

    protected void KeepOnScreen()
    {
        var screen = GetCurrentScreenRect();
        bool b = false;

        if (Left < screen.Left)
        {
            Left = screen.Left;
            b = true;
        }

        if (Top < screen.Top)
        {
            Top = screen.Top;
            b = true;
        }

        if (Right > screen.Right)
        {
            Left = screen.Right - Width;
            b = true;
        }

        if (Bottom > screen.Bottom)
        {
            Top = screen.Bottom - Height;
            b = true;
        }

        if (Special && b)
        {
            LocationRefreshed?.Invoke();
        }
    }

    private void AppLauncher_TrayMenuShowAllClicked()
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
