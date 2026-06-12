namespace PlainCEETimer.Modules.Extensions;

public static class ActionInvokerExtensions
{
    public static T WithArgs<T, T1>(this T invoker, T1 arg)
        where T : IActionInvoker<T1>
    {
        invoker.SetArg(arg);
        return invoker;
    }

    public static T WithArgs<T, T1, T2>(this T invoker, T1 arg1, T2 arg2)
        where T : IActionInvoker<T1, T2>
    {
        invoker.SetArgs(arg1, arg2);
        return invoker;
    }
}