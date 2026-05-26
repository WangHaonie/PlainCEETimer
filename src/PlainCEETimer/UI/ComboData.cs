using System.Runtime.CompilerServices;

namespace PlainCEETimer.UI;

public class ComboData
{
    public string Display => m_display;

    public int Value => m_value;

    private int m_value;
    private readonly string m_display;

    private ComboData(string display)
    {
        m_display = display;
    }

    internal void SetValue(int value)
    {
        m_value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ComboData(string display)
    {
        return new(display);
    }
}
