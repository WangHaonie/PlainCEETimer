using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.WPF.Extensions;

namespace PlainCEETimer.WPF.Controls;

[NoConstants]
[TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
[TemplatePart(Name = PART_ListBox, Type = typeof(ListBox))]
public sealed class FontFamilyInputBox : TextBox
{
    private bool IsOpen
    {
        get => m_popup?.IsOpen ?? false;
        set => m_popup?.IsOpen = value;
    }

    private string lastText;
    private Window parentWindow;
    private Popup m_popup;
    private ListBox m_listbox;
    private HwndSource hwndSource;
    private HwndSourceHook m_hook;
    private readonly Throttler throttler;
    private readonly Action ClosePopupAction;
    private readonly ReadOnlyCollection<string> systemFonts;

    private const string PART_Popup = nameof(PART_Popup);
    private const string PART_ListBox = nameof(PART_ListBox);

    static FontFamilyInputBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FontFamilyInputBox), new FrameworkPropertyMetadata(typeof(FontFamilyInputBox)));
    }

    public FontFamilyInputBox()
    {
        lastText = string.Empty;

        systemFonts = Fonts.SystemFontFamilies
            .Select(ff => ff.Source)
            .OrderBy(ff => ff, StringComparer.OrdinalIgnoreCase)
            .ToList().AsReadOnly();

        Loaded += FontFamilyInputBox_Loaded;
        Unloaded += FontFamilyInputBox_Unloaded;
        throttler = new(300);
        ClosePopupAction = () => IsOpen = false;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ClearListBoxEvents();

        m_popup = GetTemplateChild(PART_Popup) as Popup;

        if (GetTemplateChild(PART_ListBox) is ListBox lb)
        {
            m_listbox = lb;
            lb.ItemsSource = systemFonts;
            InitListBoxEvents();
        }
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        base.OnTextChanged(e);

        var text = Text;

        if (text != lastText)
        {
            if (IsOpen)
            {
                ScrollToMatch();
            }
            else
            {
                var initialInput = text.Length == 1 || lastText.Length == 0;
                var addedComma = text.Count(IsComma) > lastText.Count(IsComma);

                if (initialInput || addedComma)
                {
                    IsOpen = true;
                    ScrollToMatch();
                }
            }

            lastText = text;
        }
    }

    private void ScrollToMatch()
    {
        var front = Text.Substring(0, CaretIndex);
        var lastComma = front.LastIndexOf(',');
        var token = lastComma >= 0 ? front.Substring(lastComma + 1).Compact() : front.Compact();

        ScrollToMatchCore(token);
    }

    private void ScrollToMatchCore(string token)
    {
        if (m_listbox != null)
        {
            var items = m_listbox.Items;
            var count = items.Count;

            if (count > 0)
            {
                int index = -1;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    var length = systemFonts.Count;

                    for (int i = 0; i < length; i++)
                    {
                        if (systemFonts[i].StartsWith(token, StringComparison.OrdinalIgnoreCase))
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index < 0)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            if (systemFonts[i].Contains(token, StringComparison.OrdinalIgnoreCase))
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                }

                m_listbox.SelectedIndex = index;
                m_listbox.ScrollIntoView(items[index.Clamp(0, count)]);
            }
        }
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        var flag = false;

        if (!IsOpen)
        {
            flag = true;
        }

        HandleCommonKeys(e);

        if (!flag && !e.Handled)
        {
            flag = true;
        }

        if (flag)
        {
            base.OnPreviewKeyDown(e);
        }
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        if (m_listbox?.IsKeyboardFocusWithin == true)
        {
            return;
        }

        IsOpen = false;
    }


    private void FontFamilyInputBox_Loaded(object sender, RoutedEventArgs e)
    {
        parentWindow = Window.GetWindow(this);

        if (parentWindow != null)
        {
            parentWindow.PreviewMouseDown += ParentWindow_PreviewMouseDown;
            parentWindow.Deactivated += ParentWindow_Deactivated;
            hwndSource = PresentationSource.FromVisual(parentWindow) as HwndSource;

            if (hwndSource != null)
            {
                m_hook ??= WndProc;
                hwndSource.AddHook(m_hook);
            }
        }
    }

    private void FontFamilyInputBox_Unloaded(object sender, RoutedEventArgs e)
    {
        if (parentWindow != null)
        {
            parentWindow.PreviewMouseDown -= ParentWindow_PreviewMouseDown;
            parentWindow.Deactivated -= ParentWindow_Deactivated;
            parentWindow = null;
        }

        if (hwndSource != null)
        {
            if (m_hook != null)
            {
                hwndSource.RemoveHook(m_hook);
                m_hook = null;
            }

            hwndSource = null;
        }

        ClearListBoxEvents();
        m_popup = null;
        m_listbox = null;
    }

    private void ParentWindow_Deactivated(object sender, EventArgs e)
    {
        IsOpen = false;
    }

    private void ParentWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsOpen)
        {
            return;
        }

        var source = e.OriginalSource as DependencyObject;

        if (!source.IsChildOf(this) && !source.IsChildOf(m_listbox) && m_popup?.IsMouseOver == false)
        {
            IsOpen = false;
        }
    }

    private void ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        HandleCommonKeys(e);
    }

    private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (m_listbox != null
            && (e.OriginalSource as DependencyObject).GetParent(out ListBoxItem lbi)
            && lbi.Content is string s)
        {
            ApplySelection(s);
            e.Handled = true;
        }
    }

    private void HandleCommonKeys(KeyEventArgs e)
    {
        if (m_listbox == null)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Up:
            case Key.Down:
                if (IsOpen && m_listbox != null && !m_listbox.IsKeyboardFocusWithin)
                {
                    m_listbox.Focus();

                    if (m_listbox.SelectedIndex < 0)
                    {
                        m_listbox.SelectedIndex = 0;
                    }

                    (m_listbox.ItemContainerGenerator.ContainerFromIndex(m_listbox.SelectedIndex) as ListBoxItem)?.Focus();
                    e.Handled = true;
                }
                break;

            case Key.Tab:
            case Key.Enter:
                if (IsOpen && m_listbox.SelectedItem is string s)
                {
                    ApplySelection(s);
                    e.Handled = true;
                }
                break;
            case Key.Escape:
                if (IsOpen)
                {
                    IsOpen = false;
                    Focus();
                    e.Handled = true;
                }
                break;
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM.NCLBUTTONDOWN:
            case WM.NCRBUTTONDOWN:
            case WM.NCMBUTTONDOWN:
            case WM.NCXBUTTONDOWN:
            case WM.NCLBUTTONDBLCLK:
            case WM.NCRBUTTONDBLCLK:
            case WM.NCMBUTTONDBLCLK:
            case WM.NCXBUTTONDBLCLK:
            case WM.MOVE:
            case WM.MOVING:
            case WM.SIZE:
            case WM.SIZING:
            case WM.ACTIVATE when wParam == IntPtr.Zero:
                throttler.Throttle(ClosePopupAction);
                break;
            default:
                break;
        }

        return IntPtr.Zero;
    }

    private void ApplySelection(string fontName)
    {
        var text = Text;
        var pos = CaretIndex;
        int lastComma = pos > 0 ? text.LastIndexOf(',', pos - 1) : -1;
        int nextComma = text.IndexOf(',', pos);
        int start = lastComma >= 0 ? lastComma + 1 : 0;
        int end = nextComma >= 0 ? nextComma : text.Length;

        while (start < end && char.IsWhiteSpace(text[start]))
        {
            start++;
        }

        while (end > start && char.IsWhiteSpace(text[end - 1]))
        {
            end--;
        }

        var front = lastComma >= 0 ? string.Concat(text.AsSpan(0, lastComma + 1), " ") : text.AsSpan(0, start);
        var rear = text.Substring(end);

        var newText = string.Concat(front, fontName, rear);
        var newPos = front.Length + fontName.Length;

        IsOpen = false;
        lastText = newText;
        Text = newText;
        CaretIndex = newPos;
        Focus();
    }

    private void InitListBoxEvents()
    {
        if (m_listbox != null)
        {
            m_listbox.PreviewKeyDown += ListBox_PreviewKeyDown;
            m_listbox.PreviewMouseLeftButtonDown += ListBox_PreviewMouseLeftButtonDown;
        }
    }

    private void ClearListBoxEvents()
    {
        if (m_listbox != null)
        {
            m_listbox.PreviewKeyDown -= ListBox_PreviewKeyDown;
            m_listbox.PreviewMouseLeftButtonDown -= ListBox_PreviewMouseLeftButtonDown;
        }
    }

    private static bool IsComma(char value)
    {
        return value == ',';
    }
}