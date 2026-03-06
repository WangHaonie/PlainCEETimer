using System;
using System.Windows.Forms;

namespace PlainCEETimer.UI;

public interface IDialogService
{
    DialogResult Info(string message, MessageButtons buttons = MessageButtons.OK, bool autoClose = false);

    DialogResult Warn(string message, MessageButtons buttons = MessageButtons.OK, bool autoClose = false);

    DialogResult Error(string message, Exception ex = null, MessageButtons buttons = MessageButtons.OK, bool autoClose = false);
}