namespace PlainCEETimer.Modules;

public interface IDebounceState
{
    void Invoke();
}

public interface IDebounceState<T> : IDebounceState
{
    void Update(T arg);
}