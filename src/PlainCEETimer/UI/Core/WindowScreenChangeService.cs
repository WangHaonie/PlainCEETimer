using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Core;

public abstract class WindowScreenChangeService : IWindowScreenChangeService
{
    public Screen Current => GetScreen();

    public event EventHandler<ScreenChangedEventArgs> ScreenChanged;

    private Screen lastScreen;
    private readonly Debouncer debouncer;

    protected WindowScreenChangeService()
    {
        debouncer = new(OnScreenChanged);
    }

    public virtual void Dispose()
    {
        debouncer.Destroy();
        GC.SuppressFinalize(this);
    }

    protected abstract Screen GetScreen();

    protected void HandleLocationChanged(object sender, EventArgs e)
    {
        lastScreen ??= GetScreen();
        debouncer.Debounce();
    }

    private void OnScreenChanged()
    {
        var currentScreen = GetScreen();

        if (currentScreen == null)
        {
            return;
        }

        if (lastScreen == null)
        {
            lastScreen = currentScreen;
            return;
        }

        if (currentScreen.Equals(lastScreen))
        {
            var oldScreen = lastScreen;
            lastScreen = currentScreen;
            ScreenChanged?.Invoke(this, new(oldScreen, currentScreen));
        }
    }

    ~WindowScreenChangeService()
    {
        Dispose();
    }
}
