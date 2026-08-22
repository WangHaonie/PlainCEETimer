using PlainCEETimer.Interop;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsWindowStyles(AppForm form) : IWindowStyles
{
    public bool TopMost
    {
        get => form.TopMost;
        set => form.TopMost = value;
    }

    public bool ShowInTaskbar
    {
        get => form.ShowInTaskbar;
        set => form.ShowInTaskbar = value;
    }

    public bool Visible
    {
        get => form.Visible;
        set => form.Visible = value;
    }

    public double Opacity
    {
        get => form.Opacity;
        set => form.Opacity = value;
    }

    private SystemBackdrop backdrop;

    public void ShowActivated(bool activate)
    {
        if (activate)
        {
            Visible = true;
        }
        else
        {
            Win32UI.ShowWindow(form.Handle, ShowWindowCommand.NoActivate);
        }
    }

    public bool ApplyBackdrop(bool enabled, int type)
    {
        form.TransparentBackground = enabled;

        if (enabled)
        {
            backdrop ??= new(form.Handle);
            backdrop.BackdropType = type;
            return backdrop.Apply();
        }

        backdrop?.Clear();
        return false;
    }
}