using System;

namespace PlainCEETimer.UI;

public class ThemeHelper : IDisposable
{
    private readonly IThemeAware m_obj;

    public ThemeHelper(IThemeAware obj)
    {
        if (obj != null)
        {
            m_obj = obj;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            m_obj.UpdateTheme(ThemeManager.ShouldUseDarkMode, true);
        }
    }

    public void Update()
    {
        m_obj.UpdateTheme(ThemeManager.ShouldUseDarkMode, false);
    }

    public void Dispose()
    {
        ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
        GC.SuppressFinalize(this);
    }

    private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
    {
        m_obj.UpdateTheme(e.UseDark, false);
    }

    ~ThemeHelper()
    {
        Dispose();
    }
}