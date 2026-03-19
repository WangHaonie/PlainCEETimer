using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Core;

public class AppTrayIconLoader : ITrayIconLoader
{
    public string Text
    {
        get => trayIcon.Text;
        set => trayIcon.Text = value;
    }

    public Icon Icon
    {
        get => trayIcon.Icon;
        set => trayIcon.Icon = value;
    }

    public ContextMenu ContextMenu
    {
        get => trayIcon.ContextMenu;
        set => trayIcon.ContextMenu = value;
    }

    public bool Visible
    {
        get => trayIcon.Visible;
        set => trayIcon.Visible = value;
    }

    private NotifyIcon trayIcon;

    public AppTrayIconLoader()
    {
        trayIcon = new();

        trayIcon.MouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                WindowManager.Current.OnActivateRequested();
            }
        };
    }

    public void Dispose()
    {
        trayIcon.Destory();
        trayIcon = null;
        GC.SuppressFinalize(this);
    }

    ~AppTrayIconLoader()
    {
        Dispose();
    }
}