using System;
using System.Windows.Forms;

namespace PlainCEETimer.UI;

public interface IAppWindow : IWin32Window
{
    bool InvokeRequired { get; }

    IDialogService MessageX { get; }

    object Invoke(Delegate method);

    IAsyncResult BeginInvoke(Delegate method);

    void ReActivate();
}