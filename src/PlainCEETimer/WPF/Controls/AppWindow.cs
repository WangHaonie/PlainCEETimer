using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Internal;
using PlainCEETimer.UI;
using WFContextMenu = System.Windows.Forms.ContextMenu;
using WFCursor = System.Windows.Forms.Cursor;
using WFPoint = System.Drawing.Point;
using WFRectagle = System.Drawing.Rectangle;

namespace PlainCEETimer.WPF.Controls;

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

    public Form ParentForm
    {
        get;
        set
        {
            if (value != null)
            {
                EnsureInteropHelper().Owner = value.Handle;
            }

            field = value;
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

    public WFContextMenu NativeContextMenu { get; set; }

    public IntPtr Handle => EnsureInteropHelper().EnsureHandle();

    public bool InvokeRequired => !Dispatcher.CheckAccess();

    public AppMessageBox MessageX { get; }

    protected virtual AppWindowStyle Params => AppWindowStyle.None;

    private double DpiScaleX;
    private double DpiScaleY;
    private AppNativeWindow window;
    private WindowInteropHelper wih;
    private readonly int RoundCornerRadius = 8;
    private readonly bool SetRoundCorner;
    private readonly bool Special;
    private readonly bool SystemRoundCorner;
    private readonly AppWindowStyle ParamsInternal;

    public AppWindow()
    {
        var wm = WindowManager.Current;
        ParamsInternal = Params;
        Special = CheckParam(AppWindowStyle.Special);
        SetRoundCorner = CheckParam(AppWindowStyle.RoundCorner);
        wm.ActivateRequested += App_ActivateRequested;

        if (!Special)
        {
            wm = WindowManager.Current;
            wm.TopMostChanged += AppWindow_TopMostChanged;
            Topmost = wm.TopMost;
        }

        if (SetRoundCorner)
        {
            WindowStyle = WindowStyle.None;

            if (SystemVersion.IsWindows11)
            {
                SystemRoundCorner = true;
            }
            else
            {
                AllowsTransparency = true;
                Background = Brushes.Transparent;

                WindowChrome.SetWindowChrome(this, new()
                {
                    CaptionHeight = 0,
                    CornerRadius = new(RoundCornerRadius),
                    GlassFrameThickness = new(0)
                });
            }
        }

        MessageX = new(this);
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

    protected override void OnSourceInitialized(EventArgs e)
    {
        window = new(this);

        if (ThemeManager.ShouldUseDarkMode)
        {
            ThemeManager.EnableDarkModeForWindow(Handle);
        }

        if (SystemRoundCorner)
        {
            Win32UI.SetRoundCornerEx(Handle, false);
        }

        base.OnSourceInitialized(e);
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        UpdateDpiScale(newDpi);
        base.OnDpiChanged(oldDpi, newDpi);
    }

    protected sealed override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        e.Cancel = !WPFApp.IsSystemClosing && OnClosing();
    }

    protected virtual bool OnClosing()
    {
        return false;
    }

    protected virtual void WndProc(ref Message m)
    {
        const int WM_CONTEXTMENU = 0x007B;
        const int WM_COMMAND = 0x0111;

        switch (m.Msg)
        {
            case WM_CONTEXTMENU:
                WmContextMenu(ref m);
                return;
            case WM_COMMAND:
                WmCommand(ref m);
                return;
        }

        DefWndProc(ref m);
    }

    private void AppWindow_TopMostChanged(object sender, TopMostStateChangedEventArgs e)
    {
        Topmost = e.IsTopMost;
    }

    private void App_ActivateRequested(object sender, EventArgs e)
    {
        ReActivate();
        KeepOnScreen();
    }

    private void DefWndProc(ref Message m)
    {
        window.DefWndProc(ref m);
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

    protected Point KeepOnScreen()
    {
        var screen = GetCurrentScreenRect();
        var x = Dip2PxX(Left).Clamp(screen.X, screen.Right - Dip2PxY(Width));
        var y = Dip2PxY(Top).Clamp(screen.Y, screen.Bottom - Dip2PxY(Height));
        SetLocation(x, y);
        return Location;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected double Px2DipX(int px)
    {
        return px / DpiScaleX;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected double Px2DipY(int px)
    {
        return px / DpiScaleY;
    }

    protected Point Px2Dip(WFPoint p)
    {
        return new(Px2DipX(p.X), Px2DipY(p.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int Dip2PxX(double dip)
    {
        return (int)(dip * DpiScaleX);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int Dip2PxY(double dip)
    {
        return (int)(dip * DpiScaleY);
    }

    protected WFRectagle GetCurrentScreenRect()
    {
        return Special ? Screen.FromHandle(Handle).WorkingArea : Screen.GetWorkingArea(WFCursor.Position);
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
}