namespace PlainCEETimer.UI;

public interface IHasParentWindow
{
    IAppWindow Owner { get; }
}