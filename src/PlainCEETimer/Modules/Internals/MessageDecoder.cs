using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Forms;

internal static class MessageDecoder
{
    private static MessageDecoder_MsgToString m_fnMsgToString;

    internal static string MsgToString(int msg)
    {
        m_fnMsgToString ??= DelegateHelper.StaticCreateDelegate<MessageDecoder_MsgToString>(typeof(Control), typeof(MessageDecoder), BindingFlags.Static | BindingFlags.NonPublic);
        return m_fnMsgToString(msg);
    }
}