using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.WPF.Extensions;

namespace PlainCEETimer.WPF.Modules;

[NoConstants]
public static partial class SmoothScroll
{
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.RegisterAttached(MemberNames.Enabled, typeof(bool), typeof(SmoothScroll),
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

    private static ScrollViewer FindScrollViewer(DependencyObject obj)
    {
        for (var current = obj; current != null; current = current.GetParent())
        {
            if (current is ScrollViewer sv)
            {
                return sv;
            }
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
        for (var current = obj; current != null; current = current.GetParent())
        {
            if (current is ScrollViewer sv && CanScroll(sv, delta))
            {
                return sv;
            }
        }

        return null;
    }
}
