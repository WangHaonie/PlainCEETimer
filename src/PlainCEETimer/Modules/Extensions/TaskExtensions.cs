using System;
using System.Threading.Tasks;

namespace PlainCEETimer.Modules.Extensions;

public static class TaskExtensions
{
    public static Task Start(this Action start)
        => Task.Run(start);

    public static Task Start(this Action start, Action<Task> after)
        => Task.Run(start).ContinueWith(after);

    public static Task AsDelay(this int delay, Action<Task> after)
        => Task.Delay(delay).ContinueWith(after);

    public static Action<Task> SafeExecute(this Action action)
        => _ => SafeExecutionContext.Post(SafeExecute_, action);

    private static void SafeExecute_(object state)
        => ((Action)state)();
}