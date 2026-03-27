using System;

namespace PlainCEETimer.UI;

public interface IDialogService
{
    IAppWindow Owner { get; }

    bool? Info(string message, MessageButtons buttons = MessageButtons.OK, bool autoClose = false);

    bool? Warn(string message, MessageButtons buttons = MessageButtons.OK, bool autoClose = false);

    bool? Error(string message, Exception ex = null, MessageButtons buttons = MessageButtons.OK, bool autoClose = false);

    int Popup(int uType, string message);
}