namespace PlainCEETimer.UI;

public class DialogEndEventArgs(bool? value)
{
    public bool? Result => value;
}