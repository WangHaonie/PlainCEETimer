namespace PlainCEETimer.Modules;

public interface IDebounceState
{
    void Invoke(object state);
}

public interface IDebounceState<T> : IDebounceState
{
    void Update(T arg);
}
