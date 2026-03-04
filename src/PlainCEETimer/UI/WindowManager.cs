using System;

namespace PlainCEETimer.UI;

public class WindowManager
{
    public static WindowManager Current { get; } = new();

    public bool TopMost => _topmost;

    public event EventHandler<TopMostStateChangedEventArgs> TopMostChanged;

    private bool _topmost = true;

    internal void OnTopMostChanged(bool topmost, bool fAsDefault = true)
    {
        if (fAsDefault)
        {
            _topmost = topmost;
        }

        TopMostChanged?.Invoke(this, new(topmost));
    }

    ~WindowManager()
    {
        TopMostChanged = null;
    }
}