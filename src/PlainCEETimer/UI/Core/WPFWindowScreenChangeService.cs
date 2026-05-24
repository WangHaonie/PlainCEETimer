using System.Windows.Forms;
using PlainCEETimer.WPF.Controls;

namespace PlainCEETimer.UI.Core;

public sealed class WPFWindowScreenChangeService : WindowScreenChangeService
{
    private readonly AppWindow window;

    public WPFWindowScreenChangeService(AppWindow appWindow)
    {
        window = appWindow;
        window.LocationChanged += HandleLocationChanged;
    }

    public override void Dispose()
    {
        window.LocationChanged -= HandleLocationChanged;
        base.Dispose();
    }

    protected override Screen GetScreen()
    {
        return Screen.FromHandle(window.Handle);
    }
}
