using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlainCEETimer.WPF.Modules;

public static class TextBoxHelper
{
    public static readonly DependencyProperty HasErrorProperty =
        DependencyProperty.RegisterAttached("HasError", typeof(bool), typeof(TextBoxHelper),
            new PropertyMetadata(false, OnHasErrorChanged));

    public static readonly DependencyProperty ErrorActionProperty =
        DependencyProperty.RegisterAttached("ErrorAction", typeof(TextBoxErrorAction), typeof(TextBoxHelper),
            new PropertyMetadata(TextBoxErrorAction.Highlight));

    public static readonly DependencyProperty HighlightBrushProperty =
        DependencyProperty.RegisterAttached("HighlightBrush", typeof(Brush), typeof(TextBoxHelper),
            new PropertyMetadata(Brushes.Red));

    public static void SetHasError(DependencyObject obj, bool value)
    {
        obj.SetValue(HasErrorProperty, value);
    }

    public static bool GetHasError(DependencyObject obj)
    {
        return (bool)obj.GetValue(HasErrorProperty);
    }

    public static void SetErrorAction(DependencyObject obj, TextBoxErrorAction value)
    {
        obj.SetValue(ErrorActionProperty, value);
    }

    public static TextBoxErrorAction GetErrorAction(DependencyObject obj)
    {
        return (TextBoxErrorAction)obj.GetValue(ErrorActionProperty);
    }

    public static Brush GetHighlightBrush(DependencyObject obj)
    {
        return (Brush)obj.GetValue(HighlightBrushProperty);
    }

    public static void SetHighlightBrush(DependencyObject obj, Brush value)
    {
        obj.SetValue(HighlightBrushProperty, value);
    }

    private static void OnHasErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox tb)
        {
            var value = (bool)e.NewValue;

            switch (GetErrorAction(tb))
            {
                case TextBoxErrorAction.Highlight:
                    if (value)
                        tb.BorderBrush = GetHighlightBrush(tb);
                    else
                        tb.ClearValue(Control.BorderBrushProperty);
                    break;
                case TextBoxErrorAction.Hidden:
                    if (value)
                        tb.Visibility = Visibility.Hidden;
                    else
                        tb.ClearValue(UIElement.VisibilityProperty);
                    break;
            }
        }
    }
}