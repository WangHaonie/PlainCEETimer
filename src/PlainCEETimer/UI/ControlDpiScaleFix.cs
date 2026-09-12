using System.Drawing;

namespace PlainCEETimer.UI;

public class ControlDpiScaleFix
{
    private SizeF m_lastScale;

    public bool CanScale(SizeF newScale)
    {
        if (newScale == m_lastScale)
        {
            return false;
        }

        m_lastScale = newScale;
        return true;
    }
}
