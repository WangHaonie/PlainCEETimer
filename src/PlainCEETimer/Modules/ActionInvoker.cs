using System;

namespace PlainCEETimer.Modules;

public class ActionInvoker(Action action) : IActionInvoker
{
    public void Invoke()
    {
        action();
    }

    public void Invoke(object state)
    {
        Invoke();
    }
}

public class ActionInvoker<T>(Action<T> action) : IActionInvoker<T>
{
    private T m_arg;

    public void Invoke()
    {
        action(m_arg);
    }

    public void Invoke(object state)
    {
        Invoke();
    }

    void IActionInvoker<T>.SetArg(T arg)
    {
        m_arg = arg;
    }
}

public class ActionInvoker<T1, T2>(Action<T1, T2> action) : IActionInvoker<T1, T2>
{
    private T1 m_arg1;
    private T2 m_arg2;

    public void Invoke()
    {
        action(m_arg1, m_arg2);
    }

    public void Invoke(object state)
    {
        Invoke();
    }

    void IActionInvoker<T1, T2>.SetArgs(T1 arg1, T2 arg2)
    {
        m_arg1 = arg1;
        m_arg2 = arg2;
    }
}