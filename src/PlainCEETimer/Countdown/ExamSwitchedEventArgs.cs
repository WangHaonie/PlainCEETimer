using System;

namespace PlainCEETimer.Countdown;

public class ExamSwitchedEventArgs(int index) : EventArgs
{
    public int Index => index;
}

public delegate void ExamSwitchedEventHandler(object sender, ExamSwitchedEventArgs e);
