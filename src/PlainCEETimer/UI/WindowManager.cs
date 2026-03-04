using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.WPF;
using PlainCEETimer.WPF.Controls;

namespace PlainCEETimer.UI;

public class WindowManager
{
    public static WindowManager Current { get; } = new();

    public bool TopMost => _topmost;

    public event EventHandler<TopMostStateChangedEventArgs> TopMostChanged;

    public event EventHandler ActivateRequested;

    private bool _topmost = true;

    internal static void RunUI(IAppWindow window)
    {
        if (window != null)
        {
            if (window is AppWindow aw)
            {
                new WPFApp().Run(aw);
                return;
            }

            if (window is AppForm af)
            {
                Application.Run(af);
                return;
            }
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

    ~WindowManager()
    {
        TopMostChanged = null;
        ActivateRequested = null;
    }
}