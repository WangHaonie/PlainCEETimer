using System;

namespace PlainCEETimer.Modules;

public class DebounceState(Action action) : IDebounceState
{
    public void Invoke()
    {
        action();
    }
}

public class DebounceState<T>(Action<T> action) : IDebounceState<T>
{
    private T m_arg;

    public void Invoke()
    {
        action(m_arg);
    }

    public void Update(T arg)
    {
        m_arg = arg;
    }
}