namespace PlainCEETimer.UI.Core;

public class WindowMessageService(WndProcCallback WndProc) : IWindowMessageService
{
    public WndProcCallback DefWndProc => WndProc;
}