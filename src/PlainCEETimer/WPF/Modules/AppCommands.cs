using System.Windows.Documents;
using System.Windows.Input;

namespace PlainCEETimer.WPF.Modules;

public static class AppCommands
{
    public static void Initialize()
    {
        const byte idDelete = 0x05;
        var cmdDelete = RoutedUICommandInternals.AttachTo(EditingCommands.Delete);
        cmdDelete._text = ApplicationCommandsInternals.GetUIText(idDelete);
        cmdDelete._inputGestureCollection = ApplicationCommandsInternals.LoadDefaultGestureFromResource(idDelete);
    }
}
