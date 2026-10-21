using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Input;

internal static class ApplicationCommandsInternals
{
    private static ApplicationCommands_GetUIText s_fnGetUIText;
    private static ApplicationCommands_LoadDefaultGestureFromResource s_fnLoadDefaultGestureFromResource;

    internal static string GetUIText(byte commandId)
    {
        if (s_fnGetUIText == null)
        {
            ReflectionUtils.StaticCreateDelegate(out s_fnGetUIText, typeof(ApplicationCommands), BindingFlags.NonPublic);
        }

        return s_fnGetUIText(commandId);
    }

    internal static InputGestureCollection LoadDefaultGestureFromResource(byte commandId)
    {
        if (s_fnLoadDefaultGestureFromResource == null)
        {
            ReflectionUtils.StaticCreateDelegate(out s_fnLoadDefaultGestureFromResource, typeof(ApplicationCommands), BindingFlags.NonPublic);
        }

        return s_fnLoadDefaultGestureFromResource(commandId);
    }
}
