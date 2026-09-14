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

    private double PxPerDip = 1D;
    private readonly ImmersiveViewModel vm;
    private readonly Debouncer debouncer;
    private readonly ActionInvoker ApplyStyleAction;
    private readonly double MinFontSize;
    private readonly double MaxFontSize;

    private const double ContentPadding = 24D;
    private const double MinWindowWidth = 160D;
    private const double MinWindowHeight = 80D;
    private const int LayoutDelayMs = 300;
    private const double FontAnimThreshold = 0.5;
    private static readonly Duration AnimateDuration;

    public ImmersiveWindow(ICountdownService countdown)
    {
        vm = new(countdown);
        DataContext = vm;
        MinWidth = MinWindowWidth;
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
        var source = PresentationSource.FromVisual(this);
        if (source != null) PxPerDip = source.CompositionTarget.TransformToDevice.M11;
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

    private void ApplyStyle()
    {
        var text = vm.Content;

        if (!string.IsNullOrEmpty(text))
        {
            var cx = Math.Max(1D, ActualWidth - ContentPadding);
            var cy = Math.Max(1D, ActualHeight - ContentPadding);
            var size = FindMaxNoWrapFontSize(text, cx);

            if (size >= MinFontSize)
            {
                AnimateFont(size);
                UpdateMinHeight(text, MinFontSize);
                return;
            }

            size = FindMaxWrapFontSize(text, cx, cy);

            if (size >= MinFontSize)
            {
                AnimateFont(size);
                UpdateMinHeight(text, MinFontSize);
                return;
            }

            EnsureFitsMin(text, ref cx, ref cy);
            AnimateFont(FindMaxWrapFontSize(text, cx, cy));
            UpdateMinHeight(text, MinFontSize);
        }
    }

    private double FindMaxNoWrapFontSize(string text, double cxConstraint)
    {
        var maxSize = MeasureNoWrap(text, MaxFontSize);

        if (maxSize.Width <= cxConstraint)
        {
            return MaxFontSize;
        }

        var expected = MaxFontSize * (cxConstraint / maxSize.Width);
        expected = Math.Max(MinFontSize, Math.Min(MaxFontSize, expected));
        return BinarySearchDown(text, expected, MaxFontSize, cxConstraint);
    }

    private double FindMaxWrapFontSize(string text, double cxConstraint, double cyConstraint)
    {
        if (IsWrapFit(text, MaxFontSize, cxConstraint, cyConstraint))
        {
            return MaxFontSize;
        }

        var szMax = MeasureWrap(text, MaxFontSize, cxConstraint);
        var cx = MaxFontSize * (cxConstraint / Math.Max(1, szMax.Width));
        var cy = MaxFontSize * (cyConstraint / Math.Max(1, szMax.Height));
        var expect = Math.Min(cx, cy);
        expect = Math.Max(MinFontSize, Math.Min(MaxFontSize, expect));
        return BinarySearchWrapDown(text, expect, MaxFontSize, cxConstraint, cyConstraint);
    }

    private double BinarySearchDown(string text, double lo, double hi, double cxConstraint)
    {
        bool _Measure(double f) => MeasureNoWrap(text, f).Width <= cxConstraint;

        if (_Measure(lo))
        {
            return BinarySearchCore(lo, hi, _Measure);
        }

        return BinarySearchCore(MinFontSize, lo, _Measure);
    }

    private double BinarySearchWrapDown(string text, double lo, double hi, double cxConstraint, double cyConstraint)
    {
        bool _Fits(double f) => IsWrapFit(text, f, cxConstraint, cyConstraint);

        if (_Fits(lo))
        {
            return BinarySearchCore(lo, hi, _Fits);
        }

        return BinarySearchCore(MinFontSize, lo, _Fits);
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
        var sizeWrapped = MeasureWrap(text, MinFontSize, cx);
        var cyMax = Px2DipY(ScreenService.WorkingArea.Height);
        var cyNeeded = Math.Ceiling(sizeWrapped.Height + ContentPadding);

        if (cyNeeded > ActualHeight && cyNeeded <= cyMax)
        {
            Height = cyNeeded;
            cy = Math.Max(1D, ActualHeight - ContentPadding);
        }

        sizeWrapped = MeasureWrap(text, MinFontSize, cx);

        if (sizeWrapped.Height > cy)
        {
            var cxMax = Px2DipX(ScreenService.WorkingArea.Width);
            var cxTarget = FindMinWidthForHeight(text, MinFontSize, cx, cxMax, cy);

            if (cxTarget > cx)
            {
                Width = cxTarget + ContentPadding;
                cx = Math.Max(1D, ActualWidth - ContentPadding);
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

    private void UpdateMinHeight(string text, double fontSize)
    {
        var size = MeasureWrap(text, fontSize, Math.Max(1D, ActualWidth - ContentPadding));
        var required = Math.Ceiling(size.Height + ContentPadding);
        MinHeight = Math.Max(MinWindowHeight, required);
        if (required > ActualHeight) Height = required;
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
