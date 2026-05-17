using System;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI;

public class DpiAwarenessContextScope(DpiAwarenessContext dpiContext) : IDisposable
{
    private readonly DpiAwarenessContext context = DpiHelperEx.SetDpiContext(dpiContext);

    public void Dispose()
    {
        DpiHelperEx.SetDpiContext(context);
        GC.SuppressFinalize(this);
    }
}
