using System;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI;

public class DpiAwarenessContextScope(DpiAwarenessContext dpiContext) : IDisposable
{
    private readonly DpiAwarenessContext context = DpiHelper.SetDpiContext(dpiContext);

    public void Dispose()
    {
        DpiHelper.SetDpiContext(context);
        GC.SuppressFinalize(this);
    }
}
