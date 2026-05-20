using PlainCEETimer.Interop;

namespace PlainCEETimer.UI;

public interface IAppMessageFilter
{
    unsafe bool OnMessage(MSG* lpMsg);
}