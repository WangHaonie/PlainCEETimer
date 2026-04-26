using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Forms;
using PlainCEETimer.WPF;
using PlainCEETimer.WPF.Views;

namespace PlainCEETimer.UI;

public class WindowManager
{
    public static WindowManager Current { get; } = new();

    public bool TopMost => _topmost;

    public event EventHandler<TopMostStateChangedEventArgs> TopMostChanged;

    public event EventHandler ActivateRequested;

    private bool _topmost = true;

    internal static void RunMainUI(bool isWpf)
    {
        if (isWpf)
        {
            new WPFApp().Run(new MainWindow());
        }
        else
        {
            Application.Run(new MainForm());
        }
    }

    internal void OnTopMostChanged(bool topmost, bool fAsDefault = true)
    {
        if (fAsDefault)
        {
            _topmost = topmost;
        }

        TopMostChanged?.Invoke(this, new(topmost));
    }

    internal void OnActivateRequested()
    {
        Win32UI.ActivateUnmanagedWindows();
        ActivateRequested?.Invoke(this, EventArgs.Empty);
    }

    internal static void TryExitUI()
    {
        System.Windows.Application.Current?.Shutdown();
        150.AsDelay(_ => Application.ExitThread());
        Application.Exit();
    }

    ~WindowManager()
    {
        TopMostChanged = null;
        ActivateRequested = null;
    }
}