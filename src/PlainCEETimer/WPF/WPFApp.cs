using System.Windows;

namespace PlainCEETimer.WPF;

public sealed class WPFApp : Application
{
    public static bool IsSystemClosing { get; private set; }

    public WPFApp()
    {
        ShutdownMode = ShutdownMode.OnMainWindowClose;
    }

    protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
        IsSystemClosing = true;
        base.OnSessionEnding(e);
    }
}