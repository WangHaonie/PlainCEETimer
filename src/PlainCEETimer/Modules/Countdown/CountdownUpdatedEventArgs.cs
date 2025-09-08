using System;
using System.Drawing;

namespace PlainCEETimer.Modules.Countdown;

public class CountdownUpdatedEventArgs(string content, Color fore, Color back) : EventArgs
{
    public string Content => content;
    public Color ForeColor => fore;
    public Color BackColor => back;
}
