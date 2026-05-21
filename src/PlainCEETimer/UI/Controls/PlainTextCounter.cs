using System;
using System.Drawing;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainTextCounter : PlainLabel
{
    private bool UseDark;
    private int m_length;
    private int m_maxLength;
    private PlainTextBox m_owner;
    private Predicate<int> m_IsValidLength;

    public PlainTextCounter()
    {
        Text = "0/0";
    }

    public void AttachTo(PlainTextBox owner, Predicate<int> IsValidLength)
    {
        m_owner?.TextChanged -= TextBox_TextChanged;
        m_owner = owner ?? throw new ArgumentNullException(nameof(owner));
        m_IsValidLength = IsValidLength ?? throw new ArgumentNullException(nameof(IsValidLength));
        m_maxLength = m_owner.MaxLength;
        m_owner.TextChanged += TextBox_TextChanged;
        RefreshCounter();
    }

    public void RefreshCounter()
    {
        m_length = m_owner == null ? 0 : GetTextLength(m_owner.Text);
        Text = $"{m_length}/{m_maxLength}";
        UpdateCounterColor();
    }

    protected override void Dispose(bool disposing)
    {
        m_owner?.TextChanged -= TextBox_TextChanged;
        base.Dispose(disposing);
    }

    protected override void UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;
        base.UpdateTheme(useDark, init);
        UpdateCounterColor();
    }

    private void TextBox_TextChanged(object sender, EventArgs e)
    {
        RefreshCounter();
    }

    private void UpdateCounterColor()
    {
        ForeColor = m_IsValidLength(m_length) ? (UseDark ? Colors.DarkForeText : Color.Black) : Color.Red;
    }

    private static int GetTextLength(string text)
    {
        if (text == null)
        {
            return 0;
        }

        return text.Clean().Length;
    }
}
