using System;

namespace PlainCEETimer.UI;

public interface IAppMessageFilter
{
    void OnMessage(IntPtr lpMsg);
}