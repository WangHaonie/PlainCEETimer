using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.WPF.Controls;
using PlainCEETimer.WPF.Extensions;
using PlainCEETimer.WPF.ViewModels;

namespace PlainCEETimer.Countdown.Immersive;

public sealed partial class ImmersiveWindow : AppWindow
{
    protected override AppWindowStyle Params => AppWindowStyle.Special;

    private double ContentWidth => Content is FrameworkElement host ? Math.Max(0D, ActualWidth - host.ActualWidth) : 0D;

    private double ContentHeight => Content is FrameworkElement host ? Math.Max(0D, ActualHeight - host.ActualHeight) : 0D;

    private Size ContentArea
    {
        get
        {
            var host = Content as FrameworkElement;
            var width = host?.ActualWidth ?? ActualWidth;
            var height = host?.ActualHeight ?? ActualHeight;
            return new(Math.Max(1D, width - ContentPadding), Math.Max(1D, height - ContentPadding));
        }
    }

    private double PxPerDip = 1D;
    private readonly ImmersiveViewModel vm;
    private readonly Debouncer debouncer;
    private readonly ActionInvoker ApplyStyleAction;
    private readonly double MinFontSize;
    private readonly double MaxFontSize;
    private static readonly Duration AnimateDuration;

    private const double ContentPadding = 24D;
    private const double MinWindowWidth = 160D;
    private const double MinWindowHeight = 80D;
    private const int LayoutDelayMs = 300;
    private const double FontAnimThreshold = 0.5;
    private const double TitleBarChromeWidth = 200D;
    private const bool FitMinWidthToTitleBar = true;

    public ImmersiveWindow(ICountdownService countdown)
    {
        vm = new(countdown);
        DataContext = vm;
        MinHeight = MinWindowHeight;
        InitializeComponent();
        MinFontSize = ((double)ConfigValidator.MinFontSize).Pt2Dip();
        MaxFontSize = ((double)ConfigValidator.MaxFontSize).Pt2Dip();
        debouncer = new(LayoutDelayMs);
        ApplyStyleAction = new(ApplyStyle);

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName
                is nameof(ImmersiveViewModel.Content)
                or nameof(ImmersiveViewModel.Font))
            {
                UpdateStyle();
            }
        };
    }

    static ImmersiveWindow()
    {
        AnimateDuration = new(TimeSpan.FromMilliseconds(200));
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        UpdateMetrics();
        UpdateStyle();
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
    }

    protected override void OnClosed()
    {
        debouncer.Destroy();
        vm.Dispose();
        base.OnClosed();
    }

    private void UpdateStyle()
    {
        if (IsLoaded)
        {
            debouncer.Debounce(ApplyStyleAction);
        }
    }

    private double MeasureTitleBarMinWidth()
    {
        var ft = new FormattedText(Title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            new(FontFamily, FontStyle, FontWeight, FontStretch),
            FontSize, Brushes.Black, PxPerDip);

        return Math.Max(MinWindowWidth, ft.Width + TitleBarChromeWidth);
    }

    private void UpdateMetrics()
    {
        PxPerDip = DpiScale.PixelsPerDip;
        MinWidth = FitMinWidthToTitleBar ? MeasureTitleBarMinWidth() : MinWindowWidth;
    }

    private void ApplyStyle()
    {
        var text = vm.Content;

        if (!string.IsNullOrEmpty(text))
        {
            var cx = ContentArea.Width;
            var cy = ContentArea.Height;

            if (FitsOneLine(text, MaxFontSize, cx, cy))
            {
                AnimateFont(MaxFontSize);
            }
            else if (FitsOneLine(text, MinFontSize, cx, cy))
            {
                AnimateFont(FindMaxNoWrapFontSize(text, cx, cy));
            }
            else
            {
                EnsureFitsMin(text, ref cx, ref cy);
                AnimateFont(FindMaxWrapFontSize(text, cx, cy));
            }

            UpdateMinHeight(text, MinFontSize, cx);
        }
    }

    private bool FitsOneLine(string text, double fontSize, double cxConstraint, double cyConstraint)
    {
        var size = MeasureNoWrap(text, fontSize);
        return size.Width <= cxConstraint && size.Height <= cyConstraint;
    }

    private double FindMaxNoWrapFontSize(string text, double cxConstraint, double cyConstraint)
    {
        return BinarySearchCore(MinFontSize, MaxFontSize, f => FitsOneLine(text, f, cxConstraint, cyConstraint));
    }

    private double FindMaxWrapFontSize(string text, double cxConstraint, double cyConstraint)
    {
        return BinarySearchCore(MinFontSize, MaxFontSize, f => IsWrapFit(text, f, cxConstraint, cyConstraint));
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

    private bool IsWrapFit(string text, double fontSize, double cxConstraint, double cyConstraint)
    {
        var ft = CreateText(text, fontSize);
        ft.MaxTextWidth = cxConstraint;
        return ft.Width <= cxConstraint && ft.Height <= cyConstraint;
    }

    private FormattedText CreateText(string text, double fontSize)
    {
        return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            new(CountdownText.FontFamily, CountdownText.FontStyle,
                CountdownText.FontWeight, CountdownText.FontStretch),
            fontSize, Brushes.Black, PxPerDip);
    }

    private void EnsureFitsMin(string text, ref double cx, ref double cy)
    {
        var cxContent = ContentWidth;
        var cyContent = ContentHeight;
        var cyNeeded = Math.Ceiling(MeasureWrap(text, MinFontSize, cx).Height + ContentPadding);
        var cyMax = Px2DipY(ScreenService.WorkingArea.Height) - cyContent;

        if (cyNeeded > cy + ContentPadding && cyNeeded <= cyMax)
        {
            Height = cyNeeded + cyContent;
            cy = cyNeeded - ContentPadding;
        }

        if (cyNeeded > cy + ContentPadding)
        {
            var cxMax = Px2DipX(ScreenService.WorkingArea.Width) - cxContent;
            var cxTarget = FindMinWidthForHeight(text, MinFontSize, cx, cxMax, cy);

            if (cxTarget > cx)
            {
                Width = cxTarget + ContentPadding + cxContent;
                cx = cxTarget;
            }
        }
    }

    private double FindMinWidthForHeight(string text, double fontSize, double lo, double hi, double maxHeight)
    {
        if (MeasureWrap(text, fontSize, hi).Height <= maxHeight)
        {
            while (hi - lo > 1D)
            {
                var m = (lo + hi) / 2D;

                if (MeasureWrap(text, fontSize, m).Height <= maxHeight)
                    hi = m;
                else
                    lo = m;
            }
        }

        return hi;
    }

    private void UpdateMinHeight(string text, double fontSize, double cx)
    {
        var required = Math.Ceiling(MeasureWrap(text, fontSize, cx).Height + ContentPadding);
        var outer = required + ContentHeight;
        MinHeight = Math.Max(MinWindowHeight, outer);
        if (outer > ActualHeight) Height = outer;
    }

    private void AnimateFont(double target)
    {
        var current = CountdownText.FontSize;

        if (double.IsNaN(current) || Math.Abs(target - current) < FontAnimThreshold)
        {
            CountdownText.FontSize = target;
            return;
        }

        CountdownText.BeginAnimation(FontSizeProperty, null);

        CountdownText.BeginAnimation(FontSizeProperty, new DoubleAnimation(current, target, AnimateDuration)
        {
            EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut }
        });
    }
}
