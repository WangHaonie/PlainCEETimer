using System;

namespace PlainCEETimer.Modules.Countdown;

public class ExamSwitchedEventArgs(int index) : EventArgs
{
    public int Index => index;
}
