namespace PlainCEETimer.Modules;

public interface IActionInvoker
{
    void Invoke(object state);
}

public interface IActionInvoker<T> : IActionInvoker
{
    void SetArg(T arg);
}

public interface IActionInvoker<T1, T2> : IActionInvoker
{
    void SetArgs(T1 arg1, T2 arg2);
}
