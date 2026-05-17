using System.Drawing;

namespace PlainCEETimer.Countdown;

public class CountdownBasicInfo(string content, Color fore, Color back)
{
    public string Content => content;

    public Color ForeColor => fore;

    public Color BackColor => back;
}
