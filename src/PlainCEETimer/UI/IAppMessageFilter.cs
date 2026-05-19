using System;

namespace PlainCEETimer.UI;

public interface IAppMessageFilter
{
    bool OnMessage(IntPtr lpMsg);
}