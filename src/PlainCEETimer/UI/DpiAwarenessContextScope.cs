using System;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI;

public class DpiAwarenessContextScope(DpiAwarenessContext dpiContext) : IDisposable
{
    private readonly DpiAwarenessContext context = DpiManager.SetDpiContext(dpiContext);

    public void Dispose()
    {
        DpiManager.SetDpiContext(context);
        GC.SuppressFinalize(this);
    }
}
