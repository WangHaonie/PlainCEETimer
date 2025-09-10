using System;
using System.Drawing;

namespace PlainCEETimer.Countdown;

public class CountdownUpdatedEventArgs(string content, Color fore, Color back) : EventArgs
{
    public string Content => content;

    public Color ForeColor => fore;

    public Color BackColor => back;
}

public delegate void CountdownUpdatedEventHandler(object sender, CountdownUpdatedEventArgs e);
