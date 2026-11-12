using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.WPF.Modules;

[NoConstants]
public static class SmoothScroll
{
    private sealed class ScrollAnimator
    {
        private bool isAnimating;
        private bool enabled;
        private int countBringIntoView;
        private long startsAt;
        private double fromV;
        private double fromH;
        private double toV;
        private double toH;
        private bool m_Vanimate;
        private bool m_Hanimate;
        private bool m_SuppressBringIntoView;
        private double m_duration;
        private bool m_scrollUnitSet;
        private ItemsControl m_itemsControl;
        private UIElement m_content;
        private FrameworkElement m_target;
        private readonly ScrollViewer m_sv;
        private readonly MouseWheelEventHandler m_OnPreviewMouseWheelHandler;
        private readonly MouseButtonEventHandler m_OnPreviewMouseDownHandler;
        private readonly KeyEventHandler m_OnPreviewKeyDownHandler;
        private readonly ScrollChangedEventHandler m_OnScrollChangedHandler;
        private readonly RoutedEventHandler m_OnLoadedHandler;
        private readonly RoutedEventHandler m_OnUnloadedHandler;
        private readonly RequestBringIntoViewEventHandler m_OnBringIntoViewHandler;

        public ScrollAnimator(ScrollViewer scrollViewer)
        {
            m_sv = scrollViewer;
            m_OnPreviewMouseWheelHandler = OnPreviewMouseWheel;
            m_OnPreviewMouseDownHandler = OnPreviewMouseDown;
            m_OnPreviewKeyDownHandler = OnPreviewKeyDown;
            m_OnScrollChangedHandler = OnScrollChanged;
            m_OnLoadedHandler = OnLoaded;
            m_OnUnloadedHandler = OnUnloaded;
            m_OnBringIntoViewHandler = OnBringIntoView;
        }

        public void Enable()
        {
            if (!enabled)
            {
                enabled = true;

                ApplyPixelScrollUnit();
                m_sv.AddHandler(Mouse.PreviewMouseWheelEvent, m_OnPreviewMouseWheelHandler, true);
                m_sv.AddHandler(Mouse.PreviewMouseDownEvent, m_OnPreviewMouseDownHandler, true);
                m_sv.AddHandler(Keyboard.PreviewKeyDownEvent, m_OnPreviewKeyDownHandler, true);
                m_sv.ScrollChanged += m_OnScrollChangedHandler;
                m_sv.Loaded += m_OnLoadedHandler;
                m_sv.Unloaded += m_OnUnloadedHandler;

                if (m_sv.IsLoaded)
                {
                    AttachBringIntoView();
                }
            }
        }

        public void Disable()
        {
            if (enabled)
            {
                enabled = false;

                StopAnimation();
                DetachBringIntoView();
                RestoreScrollUnit();
                m_sv.RemoveHandler(Mouse.PreviewMouseWheelEvent, m_OnPreviewMouseWheelHandler);
                m_sv.RemoveHandler(Mouse.PreviewMouseDownEvent, m_OnPreviewMouseDownHandler);
                m_sv.RemoveHandler(Keyboard.PreviewKeyDownEvent, m_OnPreviewKeyDownHandler);
                m_sv.ScrollChanged -= m_OnScrollChangedHandler;
                m_sv.Loaded -= m_OnLoadedHandler;
                m_sv.Unloaded -= m_OnUnloadedHandler;
                m_itemsControl = null;
            }
        }

        private void ApplyPixelScrollUnit()
        {
            if (m_itemsControl == null && m_sv.TemplatedParent is ItemsControl ic)
            {
                m_itemsControl = ic;

                if (VirtualizingPanel.GetScrollUnit(ic) != ScrollUnit.Pixel)
                {
                    VirtualizingPanel.SetScrollUnit(ic, ScrollUnit.Pixel);
                    m_scrollUnitSet = true;
                }
            }
        }

        private void RestoreScrollUnit()
        {
            if (m_scrollUnitSet && m_itemsControl != null)
            {
                VirtualizingPanel.SetScrollUnit(m_itemsControl, ScrollUnit.Item);
            }

            m_scrollUnitSet = false;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ApplyPixelScrollUnit();
            AttachBringIntoView();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            StopAnimation();
            DetachBringIntoView();
            m_itemsControl = null;
        }

        private void AttachBringIntoView()
        {
            if (m_sv.Content is UIElement e && e != m_content)
            {
                DetachBringIntoView();

                if (e is ItemsPresenter)
                {
                    m_content = e;
                    e.AddHandler(FrameworkElement.RequestBringIntoViewEvent, m_OnBringIntoViewHandler, false);
                }
            }
        }

        private void DetachBringIntoView()
        {
            m_content?.RemoveHandler(FrameworkElement.RequestBringIntoViewEvent, m_OnBringIntoViewHandler);
            m_content = null;
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (NoAnimation || e.Handled || e.OriginalSource is not DependencyObject src
                || FindScrollableScrollViewer(src, e.Delta) != m_sv)
            {
                return;
            }

            var lines = SystemParameters.WheelScrollLines;

            if (lines != 0)
            {
                var d = lines == -1 ? m_sv.ViewportHeight : Math.Abs(lines) * WheelLineHeight;
                var delta = -e.Delta / 120D * d;
                e.Handled = true;
                ScrollBy(delta);
            }
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            StopAnimation();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            StopAnimation();
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (m_sv.Content != m_content)
            {
                AttachBringIntoView();
            }
        }

        private void OnBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (NoAnimation || m_SuppressBringIntoView || e.Handled
                || e.TargetObject is not FrameworkElement target)
            {
                return;
            }

            if (FindScrollViewer(target) != m_sv)
            {
                return;
            }

            if (m_sv.ScrollableHeight <= 0D && m_sv.ScrollableWidth <= 0D)
            {
                return;
            }

            if (!target.IsArrangeValid)
            {
                e.Handled = true;

                m_sv.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
                {
                    if (enabled && m_sv.IsLoaded)
                    {
                        m_sv.UpdateLayout();

                        if (target.IsArrangeValid
                            && TryGetBringIntoViewOffsets(target, Rect.Empty, out var targetVertical, out var targetHorizontal))
                        {
                            AnimateBringIntoView(target, targetVertical, targetHorizontal);
                            return;
                        }

                        m_SuppressBringIntoView = true;

                        try
                        {
                            target.BringIntoView();
                        }
                        finally
                        {
                            m_SuppressBringIntoView = false;
                        }
                    }
                });

                return;
            }

            if (TryGetBringIntoViewOffsets(target, e.TargetRect, out var v, out var h))
            {
                e.Handled = true;
                AnimateBringIntoView(target, v, h);
            }
        }

        private void AnimateBringIntoView(FrameworkElement target, double vertical, double horizontal)
        {
            m_target = target;
            countBringIntoView = 0;
            AnimateTo(vertical, horizontal,
                GetBringIntoViewDuration(vertical - m_sv.VerticalOffset, horizontal - m_sv.HorizontalOffset));

            if (!isAnimating)
            {
                m_target = null;
            }
        }

        private bool TryGetBringIntoViewOffsets(FrameworkElement target, Rect rc, out double v, out double h)
        {
            v = m_sv.VerticalOffset;
            h = m_sv.HorizontalOffset;

            Point p;
            double cx;
            double cy;

            try
            {
                var transform = target.TransformToAncestor(m_sv);

                if (rc.IsEmpty)
                {
                    p = transform.Transform(default);
                    cx = target.ActualWidth;
                    cy = target.ActualHeight;
                }
                else
                {
                    p = transform.Transform(rc.TopLeft);
                    cx = rc.Width;
                    cy = rc.Height;
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return false;
            }

            var r = p.X + cx;
            var b = p.Y + cy;
            var cxvp = m_sv.ViewportWidth;
            var cyvp = m_sv.ViewportHeight;
            var vChanged = false;
            var hChanged = false;

            if (p.Y < 0D)
            {
                v += p.Y;
                vChanged = true;
            }
            else if (b > cyvp)
            {
                v += b - cyvp;
                vChanged = true;
            }

            if (p.X < 0D)
            {
                h += p.X;
                hChanged = true;
            }
            else if (r > cxvp)
            {
                h += r - cxvp;
                hChanged = true;
            }

            v = CoerceVertical(v);
            h = CoerceHorizontal(h);
            return vChanged || hChanged;
        }

        private void ScrollBy(double delta)
        {
            m_target = null;
            var current = isAnimating ? toV : m_sv.VerticalOffset;
            AnimateTo(CoerceVertical(current + delta), m_sv.HorizontalOffset, GetWheelDuration(delta));
        }

        private void AnimateTo(double vertical, double horizontal, double duration)
        {
            if (NoAnimation)
            {
                m_sv.ScrollToVerticalOffset(CoerceVertical(vertical));
                m_sv.ScrollToHorizontalOffset(CoerceHorizontal(horizontal));
                return;
            }

            var vNew = m_sv.VerticalOffset;
            var hNew = m_sv.HorizontalOffset;

            if (!isAnimating
                && Math.Abs(vertical - vNew) <= AxisTolerance
                && Math.Abs(horizontal - hNew) <= AxisTolerance)
            {
                return;
            }

            if (isAnimating
                && Math.Abs(vertical - toV) <= AxisTolerance
                && Math.Abs(horizontal - toH) <= AxisTolerance)
            {
                return;
            }

            fromV = vNew;
            fromH = hNew;
            toV = vertical;
            toH = horizontal;
            m_Vanimate = Math.Abs(vertical - vNew) > AxisTolerance;
            m_Hanimate = Math.Abs(horizontal - hNew) > AxisTolerance;
            m_duration = Math.Max(1D, duration);
            startsAt = Stopwatch.GetTimestamp();

            if (!isAnimating)
            {
                isAnimating = true;
                AttachRendering(this);
            }
        }

        public bool Tick()
        {
            if (isAnimating)
            {
                if (NoAnimation)
                {
                    if (m_Vanimate)
                    {
                        m_sv.ScrollToVerticalOffset(toV);
                    }

                    if (m_Hanimate)
                    {
                        m_sv.ScrollToHorizontalOffset(toH);
                    }

                    isAnimating = false;
                    return true;
                }

                var elapsed = (Stopwatch.GetTimestamp() - startsAt) * 1000D / Stopwatch.Frequency;
                var progress = Math.Min(1D, elapsed / m_duration);
                var remaining = 1D - progress;
                var eased = 1D - remaining * remaining * remaining;

                if (m_Vanimate)
                {
                    m_sv.ScrollToVerticalOffset(fromV + (toV - fromV) * eased);
                }

                if (m_Hanimate)
                {
                    m_sv.ScrollToHorizontalOffset(fromH + (toH - fromH) * eased);
                }

                if (progress >= 1D)
                {
                    isAnimating = false;
                    return true;
                }

                return false;
            }

            return true;
        }

        public void OnAnimationCompleted()
        {
            if (NoAnimation)
            {
                m_target = null;
                return;
            }

            if (m_target is not FrameworkElement fe)
            {
                return;
            }

            if (!enabled || !m_sv.IsLoaded)
            {
                m_target = null;
                return;
            }

            if (++countBringIntoView > MaxBringIntoViewRetry)
            {
                m_target = null;
                return;
            }

            m_sv.UpdateLayout();

            if (fe.IsArrangeValid && TryGetBringIntoViewOffsets(fe, Rect.Empty, out var v, out var h))
            {
                AnimateTo(v, h, Math.Min(BringIntoViewDuration,
                    GetBringIntoViewDuration(v - m_sv.VerticalOffset, h - m_sv.HorizontalOffset)));

                if (isAnimating)
                {
                    return;
                }
            }

            m_target = null;
        }

        public void StopAnimation()
        {
            m_target = null;
            countBringIntoView = 0;

            if (isAnimating)
            {
                isAnimating = false;
                DetachRendering(this);
            }
        }

        private static double GetWheelDuration(double delta)
        {
            var duration = WheelDuration * Math.Sqrt(Math.Abs(delta) / WheelReferenceDistance);
            return Math.Min(MaxWheelDuration, Math.Max(MinWheelDuration, duration));
        }

        private double GetBringIntoViewDuration(double deltaV, double deltaH)
        {
            var distance = Math.Sqrt(deltaV * deltaV + deltaH * deltaH);
            var duration = BringIntoViewDuration * Math.Sqrt(distance / Math.Max(1D, m_sv.ViewportHeight));
            return Math.Min(MaxBringIntoViewDuration, Math.Max(MinBringIntoViewDuration, duration));
        }

        private double CoerceVertical(double value)
        {
            return Math.Max(0D, Math.Min(value, m_sv.ScrollableHeight));
        }

        private double CoerceHorizontal(double value)
        {
            return Math.Max(0D, Math.Min(value, m_sv.ScrollableWidth));
        }
    }

    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(SmoothScroll),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnEnabledChanged));

    private static bool s_renderingAttached;
    private static readonly List<ScrollAnimator> s_currentAnimators = [];
    private static readonly ConditionalWeakTable<ScrollViewer, ScrollAnimator> s_animators = new();

    private const double WheelLineHeight = 16D;
    private const double WheelReferenceDistance = 48D;
    private const double WheelDuration = 200D;
    private const double MinWheelDuration = 120D;
    private const double MaxWheelDuration = 300D;
    private const double BringIntoViewDuration = 300D;
    private const double MinBringIntoViewDuration = 200D;
    private const double MaxBringIntoViewDuration = 600D;
    private const double AxisTolerance = 0.05D;
    private const int MaxBringIntoViewRetry = 3;

    private static bool NoAnimation => !SystemSettings.ClientAreaAnimation.Value;

    public static bool GetEnabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(EnabledProperty);
    }

    public static void SetEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(EnabledProperty, value);
    }

    private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ScrollViewer sv)
        {
            if ((bool)e.NewValue)
            {
                s_animators.GetValue(sv, static v => new(v)).Enable();
            }
            else if (s_animators.TryGetValue(sv, out var a))
            {
                a.Disable();
                s_animators.Remove(sv);
            }
        }
    }

    private static void AttachRendering(ScrollAnimator a)
    {
        if (!s_currentAnimators.Contains(a))
        {
            s_currentAnimators.Add(a);
        }

        if (!s_renderingAttached)
        {
            s_renderingAttached = true;
            CompositionTarget.Rendering += OnRendering;
        }
    }

    private static void DetachRendering(ScrollAnimator a)
    {
        s_currentAnimators.Remove(a);

        if (s_currentAnimators.Count == 0 && s_renderingAttached)
        {
            s_renderingAttached = false;
            CompositionTarget.Rendering -= OnRendering;
        }
    }

    private static void OnRendering(object sender, EventArgs e)
    {
        for (var i = s_currentAnimators.Count - 1; i >= 0; i--)
        {
            if (i >= s_currentAnimators.Count)
            {
                continue;
            }

            var a = s_currentAnimators[i];
            bool complete;

            try
            {
                complete = a.Tick();
            }
            catch
            {
                a.StopAnimation();
                continue;
            }

            if (complete)
            {
                s_currentAnimators.RemoveAt(i);

                try
                {
                    a.OnAnimationCompleted();
                }
                catch { }
            }
        }

        if (s_currentAnimators.Count == 0 && s_renderingAttached)
        {
            s_renderingAttached = false;
            CompositionTarget.Rendering -= OnRendering;
        }
    }

    private static DependencyObject GetParent(DependencyObject obj)
    {
        if (obj is Visual || obj is Visual3D)
        {
            return VisualTreeHelper.GetParent(obj);
        }

        if (obj is FrameworkContentElement fce)
        {
            return fce.Parent;
        }

        return LogicalTreeHelper.GetParent(obj);
    }

    private static ScrollViewer FindScrollViewer(DependencyObject obj)
    {
        while (obj != null)
        {
            if (obj is ScrollViewer sv)
            {
                return sv;
            }

            obj = GetParent(obj);
        }

        return null;
    }

    private static bool CanScroll(ScrollViewer sv, int delta)
    {
        if (sv.ScrollableHeight <= 0D)
        {
            return false;
        }

        return delta > 0
            ? sv.VerticalOffset > 0.1D
            : sv.VerticalOffset < sv.ScrollableHeight - 0.1D;
    }

    private static ScrollViewer FindScrollableScrollViewer(DependencyObject obj, int delta)
    {
        while (obj != null)
        {
            if (obj is ScrollViewer sv && CanScroll(sv, delta))
            {
                return sv;
            }

            obj = GetParent(obj);
        }

        return null;
    }
}
