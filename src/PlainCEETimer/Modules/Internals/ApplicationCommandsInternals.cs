using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Input;

internal static class ApplicationCommandsInternals
{
    private static ApplicationCommands_GetUIText s_fnGetUIText;
    private static ApplicationCommands_LoadDefaultGestureFromResource s_fnLoadDefaultGestureFromResource;

    internal static string GetUIText(byte commandId)
    {
        ReflectionUtils.StaticCreateDelegate(ref s_fnGetUIText, typeof(ApplicationCommands), BindingFlags.NonPublic);
        return s_fnGetUIText(commandId);
    }

    internal static InputGestureCollection LoadDefaultGestureFromResource(byte commandId)
    {
        ReflectionUtils.StaticCreateDelegate(ref s_fnLoadDefaultGestureFromResource, typeof(ApplicationCommands), BindingFlags.NonPublic);
        return s_fnLoadDefaultGestureFromResource(commandId);
    }
}
