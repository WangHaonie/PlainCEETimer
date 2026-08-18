using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Core;
using PlainCEETimer.WPF.Extensions;
using WFContextMenu = System.Windows.Forms.ContextMenu;
using WFPoint = System.Drawing.Point;
using WFRectagle = System.Drawing.Rectangle;
using WFSize = System.Drawing.Size;

namespace PlainCEETimer.WPF.Controls;

[NoConstants]
public class AppWindow : Window, IAppWindow
{
    private sealed class AppNativeWindow(AppWindow wnd) : NativeWindow
    {
        protected override void WndProc(ref Message m)
        {
            wnd.WndProc(ref m);
        }
    }

    public Point Location
    {
        get => new(Left, Top);
        set
        {
            Left = value.X;
            Top = value.Y;
        }
    }

    public Size Size
    {
        get => new(ActualWidth, ActualHeight);
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    public WFContextMenu LegacyContextMenu { get; set; }

    public IntPtr Handle => EnsureInteropHelper().EnsureHandle();

    public bool InvokeRequired => !Dispatcher.CheckAccess();

    public IDialogService MessageX { get; }

    public double SuggestedMaxWidth
    {
        get => (double)GetValue(SuggestedMaxWidthProperty);
        private set => SetValue(SuggestedMaxWidthProperty, value);
    }

    public bool Modal
    {
        get
        {
            s_fiShowingAsDialog ??= typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic);
            return (bool)s_fiShowingAsDialog.GetValue(this);
        }
    }

    protected IScreenService ScreenService { get; }

    protected virtual AppWindowStyle Params => AppWindowStyle.None;

    protected WindowManager WindowManager { get; } = WindowManager.Current;

    public static readonly DependencyProperty SuggestedMaxWidthProperty =
        DependencyProperty.Register(nameof(SuggestedMaxWidth), typeof(double), typeof(AppWindow),
            new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure));

    private bool IsClosed;
    private bool IsClosing;
    private double DpiScaleX;
    private double DpiScaleY;
    private IAppWindow _owner;
    private ThemeHelper themeHelper;
    private AppNativeWindow window;
    private WindowInteropHelper wih;
    private static FieldInfo s_fiShowingAsDialog;
    private readonly bool SetRoundCorner;
    private readonly bool Special;
    private readonly bool suggestMaxWidth;
    private readonly bool NativeRoundCorner;
    private readonly AppWindowStyle ParamsInternal;

    private const double AutoWrapMargin = 10D;

    public AppWindow()
    {
        SetResourceReference(StyleProperty, typeof(Window));
        ParamsInternal = Params;
        Special = CheckParam(AppWindowStyle.Special);
        SetRoundCorner = CheckParam(AppWindowStyle.RoundCorner);
        suggestMaxWidth = CheckParam(AppWindowStyle.SuggestMaxWidth);
        InitEvents();
        FontFamily = new("Segoe UI, Microsoft YaHei");
        FontSize = 9D.Pt2Dip();
        ScreenService = new ScreenHelper(Special ? this : null);

        if (SetRoundCorner)
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;

            if (!SystemVersion.IsWindows11)
            {
                WindowChrome.SetWindowChrome(this, new()
                {
                    CaptionHeight = 0,
                    CornerRadius = (TryFindResource("WindowBorderCornerRadius") is CornerRadius cr) ? cr : default,
                    GlassFrameThickness = new(0)
                });
            }
            else
            {
                NativeRoundCorner = true;
            }
        }

        MessageX = new AppMessageBox(this);
        UpdateDpiScale(VisualTreeHelper.GetDpi(this));
    }

    public object Invoke(Delegate method, params object[] args)
    {
        return Dispatcher.Invoke(method, args);
    }

    public IAsyncResult BeginInvoke(Delegate method, params object[] args)
    {
        return Dispatcher.BeginInvoke(method, args).Task;
    }

    public void ReActivate()
    {
        if (IsClosed)
        {
            return;
        }

        var tmp = Topmost;
        WindowState = WindowState.Normal;
        Topmost = true;
        Show();
        Activate();
        Topmost = tmp;
        KeepOnScreen();
    }

    public bool? ShowDialog(IAppWindow owner)
    {
        SetOwner(owner);
        return ShowDialog();
    }

    public void Show(IAppWindow owner)
    {
        SetOwner(owner);
        Show();
    }

    public new void Close()
    {
        if (!IsClosing)
        {
            base.Close();
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        var hwnd = Handle;
        window = new(this);
        window.AssignHandle(hwnd);

        if (_owner != null)
        {
            EnsureInteropHelper().Owner = _owner.Handle;
        }

        if (NativeRoundCorner)
        {
            Win32UI.SetRoundCornerEx(hwnd, false);
        }

        if (WindowStyle != WindowStyle.None)
        {
            Win32UI.RemoveWindowIcon(hwnd);
        }

        var canResize = ResizeMode is ResizeMode.CanResize or ResizeMode.CanResizeWithGrip;
        var canMinimize = ResizeMode != ResizeMode.NoResize;

        SystemMenu.FromHwnd(hwnd)
            .SetEnabled(WinUser.SC_RESTORE, !Modal)
            .SetEnabled(WinUser.SC_SIZE, canResize)
            .SetEnabled(WinUser.SC_MINIMIZE, canMinimize)
            .SetEnabled(WinUser.SC_MAXIMIZE, canResize);

        themeHelper ??= new(this);
        RefreshSuggestedMaxWidth();
        base.OnSourceInitialized(e);
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        UpdateDpiScale(newDpi);
        RefreshSuggestedMaxWidth();
        base.OnDpiChanged(oldDpi, newDpi);
        DpiHelperEx.GlobalUpdateDeviceDpi();
    }

    protected sealed override void OnClosed(EventArgs e)
    {
        OnClosed();
        ClearEvents();
        base.OnClosed(e);
        IsClosed = true;
    }

    protected virtual bool OnClosing()
    {
        return false;
    }

    protected virtual void OnClosed()
    {
        return;
    }

    protected virtual void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case WinUser.WM_CLOSE:
                WmClose(ref m);
                return;
            case WinUser.WM_CONTEXTMENU:
                WmContextMenu(ref m);
                return;
            case WinUser.WM_COMMAND:
                WmCommand(ref m);
                return;
            case WinUser.WM_SYSCOMMAND:
                WmSysCommand(ref m);
                return;
        }

        DefWndProc(ref m);
    }

    protected virtual void DefWndProc(ref Message m)
    {
        window.DefWndProc(ref m);
    }

    protected void SetLocation(int x, int y)
    {
        Left = Px2DipX(x);
        Top = Px2DipY(y);
    }

    internal protected Point KeepOnScreen()
    {
        var screen = GetCurrentScreenRect();
        var x = Dip2PxX(Left).ClampSafe(screen.X, screen.Right - Dip2PxY(ActualWidth));
        var y = Dip2PxY(Top).ClampSafe(screen.Y, screen.Bottom - Dip2PxY(ActualHeight));
        SetLocation(x, y);
        return Location;
    }

    internal protected void RefreshSuggestedMaxWidth()
    {
        if (suggestMaxWidth)
        {
            SuggestedMaxWidth = Math.Max(0D,
                Px2DipX(ScreenService.WorkingArea.Width)
                - AutoWrapMargin
                - SystemParameters.WindowResizeBorderThickness.Left * 2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal protected double Px2DipX(int px)
    {
        return px / DpiScaleX;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal protected double Px2DipY(int px)
    {
        return px / DpiScaleY;
    }

    internal protected Point Px2Dip(WFPoint p)
    {
        return new(Px2DipX(p.X), Px2DipY(p.Y));
    }

    internal protected WFPoint Dip2Px(Point p)
    {
        return new(Dip2PxX(p.X), Dip2PxY(p.Y));
    }

    internal protected Size Px2Dip(WFSize p)
    {
        return new(Px2DipX(p.Width), Px2DipY(p.Height));
    }

    internal protected WFSize Dip2Px(Size p)
    {
        return new(Dip2PxX(p.Width), Dip2PxY(p.Width));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal protected int Dip2PxX(double dip)
    {
        return (int)(dip * DpiScaleX);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal protected int Dip2PxY(double dip)
    {
        return (int)(dip * DpiScaleY);
    }

    protected WFRectagle GetCurrentScreenRect()
    {
        return ScreenService.WorkingArea;
    }

    private void InitEvents()
    {
        WindowManager.ActivateRequested += WindowManager_ActivateRequested;

        if (!Special)
        {
            WindowManager.TopMostChanged += WindowManager_TopMostChanged;
            Topmost = WindowManager.TopMost;
        }
    }

    private void ClearEvents()
    {
        WindowManager.ActivateRequested -= WindowManager_ActivateRequested;

        if (!Special)
        {
            WindowManager.TopMostChanged -= WindowManager_TopMostChanged;
        }

        themeHelper.Destroy();
    }

    private void WindowManager_TopMostChanged(object sender, TopMostStateChangedEventArgs e)
    {
        Topmost = e.IsTopMost;
    }

    private void WindowManager_ActivateRequested(object sender, EventArgs e)
    {
        ReActivate();
    }

    private void WmClose(ref Message m)
    {
        if (FireOnClosing())
        {
            return;
        }

        DefWndProc(ref m);
    }

    private void WmContextMenu(ref Message m)
    {
        var cm = LegacyContextMenu;

        if (cm != null)
        {
            var pos = m.LParam.AsMenuLocation();
            Win32UI.TrackPopupMenuEx(cm.Handle, TrackPopupMenu.Default, pos.X, pos.Y, m.HWnd, IntPtr.Zero);
            return;
        }

        DefWndProc(ref m);
    }

    private void WmCommand(ref Message m)
    {
        if (m.LParam == IntPtr.Zero && Command.DispatchID(m.WParam.LoWord))
        {
            return;
        }

        DefWndProc(ref m);
    }

    private void WmSysCommand(ref Message m)
    {
        if ((((nint)m.WParam) & 0xFFF0) == WinUser.SC_CLOSE && FireOnClosing())
        {
            return;
        }

        DefWndProc(ref m);
    }

    private bool FireOnClosing()
    {
        IsClosing = true;
        var result = !WPFApp.IsSystemClosing && OnClosing();
        IsClosing = false;
        return result;
    }

    private void SetOwner(IAppWindow owner)
    {
        if (owner is AppWindow wnd)
        {
            Owner = wnd;
        }
        else
        {
            _owner = owner;
        }
    }

    private WindowInteropHelper EnsureInteropHelper()
    {
        wih ??= new(this);
        return wih;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckParam(AppWindowStyle param)
    {
        return (ParamsInternal & param) == param;
    }

    private void UpdateDpiScale(DpiScale dpiScale)
    {
        DpiScaleX = dpiScale.DpiScaleX;
        DpiScaleY = dpiScale.DpiScaleY;
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        ThemeManager.EnableDarkModeForWindow(Handle, useDark);
    }

    WFContextMenu IHasContextMenu.ContextMenu
    {
        get => LegacyContextMenu;
        set => LegacyContextMenu = value;
    }
}
