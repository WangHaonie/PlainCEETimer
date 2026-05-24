using System.Windows.Forms;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public sealed class WinFormsWindowScreenChangeService : WindowScreenChangeService
{
    private readonly AppForm form;

    public WinFormsWindowScreenChangeService(AppForm appForm)
    {
        form = appForm;
        form.LocationChanged += HandleLocationChanged;
    }

    public override void Dispose()
    {
        form.LocationChanged -= HandleLocationChanged;
        base.Dispose();
    }

    protected override Screen GetScreen()
    {
        return Screen.FromHandle(form.Handle);
    }
}
