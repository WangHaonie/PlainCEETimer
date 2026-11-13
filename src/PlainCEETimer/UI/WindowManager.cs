using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
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

    internal static void RunMainUI()
    {
        if (App.Current.MainWindow == null)
        {
            InitConfig();

            if (App.Current.AppConfig.Display.UseWPF)
            {
                RunWindow<MainWindow>();
            }
            else
            {
                RunForm(new MainForm());
            }
        }
    }

    internal static void RunWindow<TWindow>()
        where TWindow : AppWindow, new()
    {
        new WPFApp().Run(new TWindow());
    }

    internal static void RunForm(AppForm form)
    {
        Application.Run(form);
    }

    internal static void ShowFormSafe(AppForm form)
    {
        if (GetMessageLoopStatus())
        {
            form.ReActivate();
        }
        else
        {
            RunForm(form);
        }
    }

    internal static void EndUI()
    {
        if (App.Current.MainWindow is AppForm)
        {
            Application.Exit();
        }

        System.Windows.Application.Current?.Shutdown();
    }

    private static bool GetMessageLoopStatus()
    {
        if (Application.MessageLoop)
        {
            return true;
        }

        if (Dispatcher.FromThread(Thread.CurrentThread) is Dispatcher d)
        {
            return d._frameDepth > 0;
        }

        return false;
    }

    private static void InitConfig()
    {
        FrameworkAppContextSwitches._useAdornerForTextboxSelectionRendering = -1;
    }
}
