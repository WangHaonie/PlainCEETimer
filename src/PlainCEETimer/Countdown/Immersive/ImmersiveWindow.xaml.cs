using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Core;
using PlainCEETimer.WPF.Controls;
using PlainCEETimer.WPF.Extensions;
using PlainCEETimer.WPF.ViewModels;
using WFSize = System.Drawing.Size;

namespace PlainCEETimer.Countdown.Immersive;

[NoConstants]
public sealed partial class ImmersiveWindow : AppWindow
{
    protected override AppWindowStyle Params => AppWindowStyle.Special;

    private double PxPerDip = 1D;
    private double cxSidebar;
    private double cxSidebarUser;
    private bool isActivated = true;
    private ulong lastActivateTick;
    private bool mouseMoved;
    private Point lastMousePos;
    private WFSize szUser;
    private readonly double MinFontSize;
    private readonly double MaxFontSize;
    private readonly ImmersiveViewModel vm;
    private readonly Debouncer LayoutDebouncer;
    private readonly Debouncer IdleDebouncer;
    private readonly ActionInvoker ApplyStyleAction;
    private readonly ActionInvoker IdleAction;
    private readonly ImmersiveObject config;
    private static readonly Duration AnimateDuration;

    private const int IdleTimeoutMs = 5000;
    private const int LayoutDelayMs = 300;
    private const double ContentPadding = 24D;
    private const double ButtonReserve = 48D;
    private const double MinWindowWidth = 630D;
    private const double MinWindowHeight = 210D;
    private const double PNFontRatio = 0.5;
    private const double PNNormalOpacity = 0.4;
    private const double PNDimOpacity = 0.2;
    private const double LineMarginY = 8D;
    private const double TransitionOffset = 14.0;
    private const double FontAnimThreshold = 0.5;
    private const double MouseMoveThreshold = 0.1;

    public ImmersiveWindow()
    {
        ServiceHost.ServiceProvider.CreateViewModel<ImmersiveViewModel>().ApplyTo(this)
            .Import<ICountdownService>((vm, s) => vm.CountdownService = s)
            .Import(new WPFWindowStyles(this), (vm, s) => vm.WindowStyles = s)
            .Import(new WPFWindowInitializer(this), (vm, s) => vm.WindowInitializer = s)
            .Import(MessageX, (vm, s) => vm.DialogService = s)
            .Build(out vm);

        MinWidth = MinWindowWidth;
        MinHeight = MinWindowHeight;
        config = App.Current.AppConfig.Immersive;
        InitializeComponent();
        MinFontSize = ((double)ConfigValidator.MinFontSize).Pt2Dip();
        MaxFontSize = ((double)ConfigValidator.MaxFontSize).Pt2Dip();
        LayoutDebouncer = new(LayoutDelayMs);
        ApplyStyleAction = new(ApplyStyle);

        IdleDebouncer = new(IdleTimeoutMs);
        IdleAction = new(OnIdle);

        cxSidebarUser = vm.SidebarWidth;
        cxSidebar = cxSidebarUser;
        ApplySavedWindowState();

        vm.PropertyChanged += ViewModel_PropertyChanged;
        vm.ExamSwitched += ViewModel_ExamSwitched;
    }

    static ImmersiveWindow()
    {
        AnimateDuration = new(TimeSpan.FromMilliseconds(200));
    }

    public void ReloadConfig()
    {
        vm.LoadConfig();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        UpdateMetrics();
        ApplySidebarWidth(ClampSidebarWidth(cxSidebarUser));
        UpdateStyle();

        lastActivateTick = DateTime.TickCount;
        isActivated = true;
        UpdateControls();
        IdleDebouncer.Debounce(IdleAction);

        if (config.FullScreen)
        {
            vm.SetFullScreen(true);
        }
    }

    protected override void OnDpiChanged()
    {
        UpdateMetrics();
        UpdateStyle();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        UpdateStyle();
        base.OnRenderSizeChanged(sizeInfo);

        if (vm.IsSidebarExpanded)
        {
            SidebarPanel.BeginAnimation(WidthProperty, null);
            SidebarPanel.Width = GetFullSidebarWidth();
        }
        else
        {
            UpdateSidebarWidth();
        }

        if (WindowState != WindowState.Maximized && !vm.IsFullScreen)
        {
            szUser = new WFSize((int)Math.Round(ActualWidth), (int)Math.Round(ActualHeight));
        }
    }

    protected override void OnClosed()
    {
        IdleDebouncer.Destroy();
        SaveWindowState();
        vm.PropertyChanged -= ViewModel_PropertyChanged;
        vm.ExamSwitched -= ViewModel_ExamSwitched;
        LayoutDebouncer.Destroy();
        vm.Dispose();
        base.OnClosed();
    }

    private void ApplySavedWindowState()
    {
        if (config.Size != WFSize.Empty)
        {
            Width = config.Size.Width;
            Height = config.Size.Height;
            szUser = config.Size;
        }

        if (config.Maximize)
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void SaveWindowState()
    {
        var m = WindowState == WindowState.Maximized;
        var fs = vm.IsFullScreen;
        var sz = szUser.Width > 0 && szUser.Height > 0 ? szUser : WFSize.Empty;

        if (config.Maximize == m && config.FullScreen == fs && config.Size == sz)
        {
            return;
        }

        config.Maximize = m;
        config.FullScreen = fs;
        config.Size = sz;
        ConfigValidator.DemandConfig();
    }

    private void UpdateStyle()
    {
        if (IsLoaded)
        {
            LayoutDebouncer.Debounce(ApplyStyleAction);
        }
    }

    private void UpdateMetrics()
    {
        PxPerDip = DpiScale.PixelsPerDip;
    }

    private void ApplyStyle()
    {
        if (!vm.IsSidebarExpanded)
        {
            var lines = new[] { vm.PrevContent, vm.Content, vm.NextContent };

            if (!HasAnyText(lines))
            {
                return;
            }

            var sz = GetContentArea();
            var cx = sz.Width;
            var cy = sz.Height;
            double size;

            if (NoWrapBlockFits(lines, MaxFontSize, cx, cy))
            {
                size = MaxFontSize;
            }
            else if (NoWrapBlockFits(lines, MinFontSize, cx, cy))
            {
                size = BinarySearchCore(MinFontSize, MaxFontSize, f => NoWrapBlockFits(lines, f, cx, cy));
            }
            else
            {
                EnsureBlockFits(lines, ref cx, ref cy);
                size = BinarySearchCore(MinFontSize, MaxFontSize, f => WrapBlockFits(lines, f, cx, cy));
            }

            AnimateFont(size);
            UpdateMinHeight(lines, cx);
        }
    }

    private bool NoWrapBlockFits(string[] lines, double size, double cx, double cy)
    {
        var text = lines[1];

        if (!string.IsNullOrEmpty(text) && MeasureNoWrap(text, size).Width > cx)
        {
            return false;
        }

        return NoWrapBlockHeight(lines, size) <= cy;
    }

    private bool WrapBlockFits(string[] lines, double size, double cx, double cy)
    {
        return WrapBlockHeight(lines, size, cx) <= cy;
    }

    private double NoWrapBlockHeight(string[] lines, double size)
    {
        double height = 0D;

        for (int i = 0; i < lines.Length; i++)
        {
            var text = lines[i];

            if (!string.IsNullOrEmpty(text))
            {
                height += MeasureNoWrap(text, LineFontSize(size, i == 1)).Height + LineMarginY * 2D;
            }
        }

        return height;
    }

    private double WrapBlockHeight(string[] lines, double size, double availableWidth)
    {
        double height = 0D;

        for (int i = 0; i < lines.Length; i++)
        {
            var text = lines[i];

            if (string.IsNullOrEmpty(text))
            {
                continue;
            }

            var sz = LineFontSize(size, i == 1);
            var measured = i == 1 ? MeasureWrap(text, sz, availableWidth) : MeasureNoWrap(text, sz);
            height += measured.Height + LineMarginY * 2D;
        }

        return height;
    }

    private Size MeasureNoWrap(string text, double fontSize)
    {
        var ft = CreateText(text, fontSize);
        return new Size(ft.Width, ft.Height);
    }

    private Size MeasureWrap(string text, double fontSize, double maxWidth)
    {
        var ft = CreateText(text, fontSize);
        ft.MaxTextWidth = maxWidth;
        return new Size(ft.Width, ft.Height);
    }

    private FormattedText CreateText(string text, double fontSize)
    {
        return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            new(CurrentText.FontFamily, CurrentText.FontStyle,
                CurrentText.FontWeight, CurrentText.FontStretch),
            fontSize, Brushes.Black, PxPerDip);
    }

    private void EnsureBlockFits(string[] lines, ref double cx, ref double cy)
    {
        var cyContent = GetContentHeight();
        var cxContent = GetContentWidth();
        var cyNeeded = Math.Ceiling(WrapBlockHeight(lines, MinFontSize, cx) + ContentPadding + ButtonReserve);
        var cyMax = Px2DipY(ScreenService.WorkingArea.Height) - cyContent;

        if (cyNeeded > cy + ContentPadding + ButtonReserve && cyNeeded <= cyMax)
        {
            Height = cyNeeded + cyContent;
            cy = cyNeeded - ContentPadding - ButtonReserve;
        }

        if (cyNeeded > cy + ContentPadding + ButtonReserve)
        {
            var cxMax = Px2DipX(ScreenService.WorkingArea.Width) - cxContent;
            var cxTarget = FindMinWidthForBlockHeight(lines, MinFontSize, cx, cxMax, cy);

            if (cxTarget > cx)
            {
                Width = ActualWidth + (cxTarget - cx);
                cx = cxTarget;
            }
        }
    }

    private double FindMinWidthForBlockHeight(string[] lines, double size, double lo, double hi, double maxHeight)
    {
        if (WrapBlockHeight(lines, size, hi) <= maxHeight)
        {
            while (hi - lo > 1D)
            {
                var m = (lo + hi) / 2D;

                if (WrapBlockHeight(lines, size, m) <= maxHeight)
                    hi = m;
                else
                    lo = m;
            }
        }

        return hi;
    }

    private void UpdateMinHeight(string[] lines, double cx)
    {
        var required = Math.Ceiling(WrapBlockHeight(lines, MinFontSize, cx) + ContentPadding + ButtonReserve);
        var outer = required + GetContentHeight();
        MinHeight = Math.Max(MinWindowHeight, outer);

        if (outer > ActualHeight)
        {
            Height = outer;
        }
    }

    private void AnimateFont(double size)
    {
        AnimateLine(PrevText, LineFontSize(size, false));
        AnimateLine(CurrentText, size);
        AnimateLine(NextText, LineFontSize(size, false));
    }

    private void ViewModel_ExamSwitched(object sender, EventArgs e)
    {
        CountdownHost.BeginAnimation(OpacityProperty, null);

        if (vm.NoAnimate)
        {
            CountdownHost.Opacity = 1D;
            CountdownHost.RenderTransform = null;
            return;
        }

        var t = new TranslateTransform(0.0, TransitionOffset);
        CountdownHost.RenderTransform = t;
        CountdownHost.BeginAnimation(OpacityProperty, new DoubleAnimation(0.0, 1.0, AnimateDuration));

        t.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(TransitionOffset, 0.0, AnimateDuration)
        {
            EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut }
        });
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ImmersiveViewModel.Content):
            case nameof(ImmersiveViewModel.PrevContent):
            case nameof(ImmersiveViewModel.NextContent):
            case nameof(ImmersiveViewModel.Font):
                UpdateStyle();
                break;
            case nameof(ImmersiveViewModel.IsSidebarOpen):
                OnSidebarOpenChanged();
                break;
            case nameof(ImmersiveViewModel.IsSidebarExpanded):
                OnSidebarExpandedChanged();
                break;
            case nameof(ImmersiveViewModel.SidebarWidth):
                OnSidebarWidthChanged();
                break;
        }
    }

    private void SidebarGrip_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (!vm.IsSidebarExpanded)
        {
            ApplySidebarWidth(ClampSidebarWidth(cxSidebar - e.HorizontalChange));
        }
    }

    private void SidebarGrip_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        cxSidebarUser = cxSidebar;
        vm.SaveSidebarWidth(cxSidebarUser);
    }

    private void CountdownArea_MouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(CountdownArea);

        if (mouseMoved && (position - lastMousePos).Length < MouseMoveThreshold)
        {
            return;
        }

        mouseMoved = true;
        lastMousePos = position;
        lastActivateTick = DateTime.TickCount;
        IdleDebouncer.Debounce(IdleAction);

        if (!isActivated)
        {
            isActivated = true;
            UpdateControls();
        }
    }

    private void OnSidebarOpenChanged()
    {
        if (!vm.IsSidebarOpen)
        {
            lastActivateTick = DateTime.TickCount;
            isActivated = true;
            IdleDebouncer.Debounce(IdleAction);
        }

        UpdateControls();
        AnimateSidebar(GetSidebarTargetWidth());
    }

    private void OnSidebarExpandedChanged()
    {
        SidebarGrip.Visibility = vm.IsSidebarExpanded ? Visibility.Collapsed : Visibility.Visible;

        if (!vm.IsSidebarExpanded)
        {
            cxSidebar = ClampSidebarWidth(cxSidebarUser);
        }

        AnimateSidebar(GetSidebarTargetWidth());
    }

    private void OnSidebarWidthChanged()
    {
        cxSidebarUser = vm.SidebarWidth;
        ApplySidebarWidth(ClampSidebarWidth(cxSidebarUser));
    }

    private void OnIdle()
    {
        if (isActivated && DateTime.TickCount - lastActivateTick >= IdleTimeoutMs)
        {
            isActivated = false;
            UpdateControls();
        }
    }

    private void UpdateControls()
    {
        var toggleVisible = isActivated || vm.IsSidebarOpen;
        AnimateOpacity(SidebarToggle, toggleVisible ? 1D : 0D);
        SidebarToggle.IsHitTestVisible = toggleVisible;

        var pn = isActivated ? PNNormalOpacity : PNDimOpacity;
        AnimateOpacity(PrevText, pn);
        AnimateOpacity(NextText, pn);
    }

    private double GetSidebarTargetWidth()
    {
        return vm.IsSidebarExpanded ? GetFullSidebarWidth() : vm.IsSidebarOpen ? cxSidebar : 0D;
    }

    private double GetContentWidth()
    {
        return Content is FrameworkElement fe ? Math.Max(0D, ActualWidth - fe.ActualWidth) : 0D;
    }

    private double GetContentHeight()
    {
        return Content is FrameworkElement fe ? Math.Max(0D, ActualHeight - fe.ActualHeight) : 0D;
    }

    private double GetFullSidebarWidth()
    {
        return Content is FrameworkElement fe && fe.ActualWidth > 0.5D ? fe.ActualWidth : ActualWidth;
    }

    private Size GetContentArea()
    {
        var width = CountdownArea?.ActualWidth ?? ActualWidth;
        var height = CountdownArea?.ActualHeight ?? ActualHeight;
        return new(Math.Max(1D, width - ContentPadding), Math.Max(1D, height - ContentPadding - ButtonReserve));
    }

    private void AnimateSidebar(double target)
    {
        var from = SidebarPanel.ActualWidth;

        SidebarPanel.BeginAnimation(WidthProperty, null);

        if (vm.NoAnimate)
        {
            SidebarPanel.Width = target;
            UpdateStyle();
            return;
        }

        SidebarPanel.Width = from;
        SidebarPanel.BeginAnimation(WidthProperty, new DoubleAnimation(from, target, AnimateDuration)
        {
            EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut }
        });

        UpdateStyle();
    }

    private void UpdateSidebarWidth()
    {
        var clamped = ClampSidebarWidth(cxSidebarUser);

        if (Math.Abs(clamped - cxSidebar) > 0.5D)
        {
            ApplySidebarWidth(clamped);
        }
    }

    private double ClampSidebarWidth(double value)
    {
        var cxc = Content is FrameworkElement fe && fe.ActualWidth > 0.5D ? fe.ActualWidth : ActualWidth;

        if (cxc <= 0.5D)
        {
            return value;
        }

        var min = Math.Max(ImmersiveObject.MinSidebarWidth, cxc / 4D);
        var max = Math.Max(min, cxc / 2D);
        return value.Clamp(min, max);
    }

    private void ApplySidebarWidth(double width)
    {
        cxSidebar = width;

        if (vm.IsSidebarOpen)
        {
            SidebarPanel.BeginAnimation(WidthProperty, null);
            SidebarPanel.Width = width;
        }

        UpdateStyle();
    }

    private static bool HasAnyText(string[] lines)
    {
        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                return true;
            }
        }

        return false;
    }

    private static double LineFontSize(double size, bool isCurrent)
    {
        return isCurrent ? size : size * PNFontRatio;
    }

    private static double BinarySearchCore(double lo, double hi, Func<double, bool> predicate)
    {
        if (!predicate(lo))
        {
            return lo;
        }

        if (predicate(hi))
        {
            return hi;
        }

        while (hi - lo > 0.25D)
        {
            var m = (lo + hi) / 2D;

            if (predicate(m))
                lo = m;
            else
                hi = m;
        }

        return lo;
    }

    private void AnimateLine(TextBlock target, double fontSize)
    {
        var current = target.FontSize;
        target.BeginAnimation(TextBlock.FontSizeProperty, null);

        if (vm.NoAnimate || double.IsNaN(current) || Math.Abs(fontSize - current) < FontAnimThreshold)
        {
            target.FontSize = fontSize;
            return;
        }

        target.BeginAnimation(TextBlock.FontSizeProperty, new DoubleAnimation(current, fontSize, AnimateDuration)
        {
            EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut }
        });
    }

    private void AnimateOpacity(UIElement target, double value)
    {
        var current = target.Opacity;
        target.BeginAnimation(OpacityProperty, null);

        if (vm.NoAnimate || Math.Abs(current - value) < 0.01D)
        {
            target.Opacity = value;
            return;
        }

        target.BeginAnimation(OpacityProperty, new DoubleAnimation(current, value, AnimateDuration)
        {
            EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut }
        });
    }
}
