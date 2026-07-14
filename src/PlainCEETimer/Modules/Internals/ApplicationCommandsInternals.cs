using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Input;

internal static class ApplicationCommandsInternals
{
    private static ApplicationCommands_GetUIText m_fnGetUIText;
    private static ApplicationCommands_LoadDefaultGestureFromResource m_fnLoadDefaultGestureFromResource;

    internal static string GetUIText(byte commandId)
    {
        m_fnGetUIText ??= DelegateHelper.StaticCreateDelegate<ApplicationCommands_GetUIText>(typeof(ApplicationCommands), BindingFlags.NonPublic);
        return m_fnGetUIText(commandId);
    }

    internal static InputGestureCollection LoadDefaultGestureFromResource(byte commandId)
    {
        m_fnLoadDefaultGestureFromResource ??= DelegateHelper.StaticCreateDelegate<ApplicationCommands_LoadDefaultGestureFromResource>(typeof(ApplicationCommands), BindingFlags.NonPublic);
        return m_fnLoadDefaultGestureFromResource(commandId);
    }
}
