using System;
using System.Windows.Forms;
using MS.Internal;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Forms;
using PlainCEETimer.WPF;
using PlainCEETimer.WPF.Controls;
using PlainCEETimer.WPF.Views;

namespace PlainCEETimer.UI;

public class WindowManager
{
    public static WindowManager Current { get; } = new();

    public bool TopMost => m_topmost;

    public event EventHandler<TopMostStateChangedEventArgs> TopMostChanged;

    public event EventHandler ActivateRequested;

    private bool m_topmost = true;
    private static bool m_form;

    internal void OnTopMostChanged(bool topmost, bool bAsDefault = true)
    {
        if (bAsDefault)
        {
            m_topmost = topmost;
        }

        TopMostChanged?.Invoke(this, new(topmost));
    }

    internal void OnActivateRequested()
    {
        Win32UI.ActivateUnmanagedWindows();
        ActivateRequested?.Invoke(this, EventArgs.Empty);
    }

    internal static void RunMainUI(bool isWpf)
    {
        InitConfig();

        if (isWpf)
        {
            RunUI(new MainWindow());
        }
        else
        {
            RunUI(new MainForm());
        }
    }

    internal static void RunUI(AppForm form)
    {
        m_form = App.Current.MainWindow == null && true;
        Application.Run(form);
    }

    internal static void RunUI(AppWindow window)
    {
        if (System.Windows.Application.Current == null)
        {
            m_form = App.Current.MainWindow == null && false;
            new WPFApp().Run(window);
        }
    }

    internal static void EndUI()
    {
        System.Windows.Application.Current?.Shutdown();

        if (m_form)
        {
            Application.ExitThread();
        }
    }

    private static void InitConfig()
    {
        FrameworkAppContextSwitches._useAdornerForTextboxSelectionRendering = -1;
    }

    ~WindowManager()
    {
        TopMostChanged = null;
        ActivateRequested = null;
    }
}
