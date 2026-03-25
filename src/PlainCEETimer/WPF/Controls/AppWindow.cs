using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.Modules.Internals;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Core;
using NativeContextMenu = System.Windows.Forms.ContextMenu;
using WFPoint = System.Drawing.Point;
using WFRectagle = System.Drawing.Rectangle;
using WFSize = System.Drawing.Size;

namespace PlainCEETimer.WPF.Controls;

[NoConstants]
public class AppWindow : Window, IAppWindow
{
    private sealed class AppNativeWindow : NativeWindow
    {
        private readonly AppWindow wnd;

        public AppNativeWindow(AppWindow appWindow)
        {
            wnd = appWindow;
            AssignHandle(wnd.Handle);
        }

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

    public NativeContextMenu NativeContextMenu { get; set; }

    public IntPtr Handle => EnsureInteropHelper().EnsureHandle();

    public bool InvokeRequired => !Dispatcher.CheckAccess();

    public IDialogService MessageX { get; }

    protected IScreenService ScreenService { get; }

    protected virtual AppWindowStyle Params => AppWindowStyle.None;

    protected WindowManager WindowManager { get; } = WindowManager.Current;

    private bool IsClosing;
    private double DpiScaleX;
    private double DpiScaleY;
    private IAppWindow _owner;
    private AppNativeWindow window;
    private WindowInteropHelper wih;
    private const double PtToDipRatio = 96.0 / 72.0;
    private readonly bool SetRoundCorner;
    private readonly bool Special;
    private readonly bool NativeRoundCorner;
    private readonly AppWindowStyle ParamsInternal;

    public AppWindow()
    {
        SetResourceReference(StyleProperty, typeof(Window));
        ParamsInternal = Params;
        Special = CheckParam(AppWindowStyle.Special);
        SetRoundCorner = CheckParam(AppWindowStyle.RoundCorner);
        InitEvents();
        FontFamily = new("Segoe UI, Microsoft YaHei");
        FontSize = Pt2Dip(9.0);
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

    public object Invoke(Delegate method)
    {
        return Dispatcher.Invoke(method);
    }

    public IAsyncResult BeginInvoke(Delegate method)
    {
        return Dispatcher.BeginInvoke(method).Task;
    }

    public void ReActivate()
    {
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
        window = new(this);

        if (_owner != null)
        {
            EnsureInteropHelper().Owner = _owner.Handle;
        }

        var hwnd = Handle;

        if (NativeRoundCorner)
        {
            Win32UI.SetRoundCornerEx(hwnd, false);
        }

        if (WindowStyle != WindowStyle.None)
        {
            Win32UI.RemoveWindowIcon(hwnd);
        }

        if (ThemeManager.ShouldUseDarkMode)
        {
            ThemeManager.EnableDarkModeForWindow(hwnd);
        }

        base.OnSourceInitialized(e);
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        UpdateDpiScale(newDpi);
        base.OnDpiChanged(oldDpi, newDpi);
    }

    protected sealed override void OnClosed(EventArgs e)
    {
        OnClosed();
        ClearEvents();
        base.OnClosed(e);
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
            case WM.CLOSE:
                WmClose(ref m);
                return;
            case WM.CONTEXTMENU:
                WmContextMenu(ref m);
                return;
            case WM.COMMAND:
                WmCommand(ref m);
                return;
            case WM.SYSCOMMAND:
                WmSysCommand(ref m);
                return;
        }

        DefWndProc(ref m);
    }

    protected void Hide(bool hide)
    {
        Opacity = hide ? 0D : 1D;
        IsHitTestVisible = !hide;
    }

    protected void SetLocation(int x, int y)
    {
        Left = Px2DipX(x);
        Top = Px2DipY(y);
    }

    internal protected Point KeepOnScreen()
    {
        var screen = GetCurrentScreenRect();
        var x = Dip2PxX(Left).Clamp(screen.X, screen.Right - Dip2PxY(ActualWidth));
        var y = Dip2PxY(Top).Clamp(screen.Y, screen.Bottom - Dip2PxY(ActualHeight));
        SetLocation(x, y);
        return Location;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static double Pt2Dip(double pt)
    {
        return pt * PtToDipRatio;
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
    }

    private void WindowManager_TopMostChanged(object sender, TopMostStateChangedEventArgs e)
    {
        Topmost = e.IsTopMost;
    }

    private void WindowManager_ActivateRequested(object sender, EventArgs e)
    {
        ReActivate();
        KeepOnScreen();
    }

    private void DefWndProc(ref Message m)
    {
        window.DefWndProc(ref m);
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
        var cm = NativeContextMenu;

        if (cm != null)
        {
            var pos = m.LParam.AsPoint();
            Win32UI.TrackPopupMenuEx(cm.Handle, TrackPopupMenu.Default, pos.X, pos.Y, m.HWnd, IntPtr.Zero);
            return;
        }

        DefWndProc(ref m);
    }

    private void WmCommand(ref Message m)
    {
        if (m.LParam == IntPtr.Zero && Command.DispatchID(m.WParam.ToInt32().LoWord))
        {
            return;
        }

        DefWndProc(ref m);
    }

    private void WmSysCommand(ref Message m)
    {
        if ((m.WParam.ToInt32() & 0xFFF0) == NativeConstants.SC_CLOSE && FireOnClosing())
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

    NativeContextMenu IHasContextMenu.ContextMenu
    {
        get => NativeContextMenu;
        set => NativeContextMenu = value;
    }
}