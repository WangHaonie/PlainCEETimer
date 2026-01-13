using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlainCEETimer.Modules.Extensions;

public static class TaskExtensions
{
    public static Task Start(this Action start)
        => Task.Run(start);

    public static Task Start(this Action start, Action<Task> after)
        => Task.Run(start).ContinueWith(after);

    public static Task AsDelay(this int ms, Action<Task> after)
        => Task.Delay(ms).ContinueWith(after);

    public static Action<Task> WithUI(this Action action, Control ui)
        => _ => ui.BeginInvoke(action);
}