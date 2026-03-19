using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlainCEETimer.WPF.Modules;

public static class TextBoxErrorActionHelper
{
    public static readonly DependencyProperty HasErrorProperty =
        DependencyProperty.RegisterAttached("HasError", typeof(bool), typeof(TextBoxErrorActionHelper),
            new PropertyMetadata(false, OnHasErrorChanged));

    public static readonly DependencyProperty ErrorActionProperty =
        DependencyProperty.RegisterAttached("ErrorAction", typeof(TextBoxErrorAction), typeof(TextBoxErrorActionHelper),
            new PropertyMetadata(TextBoxErrorAction.Highlight));

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

    private static void OnHasErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox tb)
        {
            ApplyErrorFeedback(tb, (bool)e.NewValue, GetErrorAction(tb));
        }
    }

    private static void ApplyErrorFeedback(TextBox textBox, bool hasError, TextBoxErrorAction mode)
    {
        switch (mode)
        {
            case TextBoxErrorAction.Highlight:
                if (hasError)
                {
                    textBox.BorderBrush = Brushes.Red;
                    return;
                }
                break;
            case TextBoxErrorAction.Hidden:
                if (hasError)
                {
                    textBox.Visibility = Visibility.Hidden;
                    return;
                }
                break;
        }

        ResetStyle(textBox);
    }

    private static void ResetStyle(TextBox textBox)
    {
        textBox.ClearValue(Control.BorderBrushProperty);
        textBox.Visibility = Visibility.Visible;
    }
}