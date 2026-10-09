using System;
using System.ComponentModel;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI;

public class ThemeHelper : IDisposable
{
    private readonly WeakReference<IThemeAware> m_obj;
    private static readonly bool themeChangeSupported = ThemeManager.IsDarkModeSupported;

    private ThemeHelper(IThemeAware obj)
    {
        if (obj != null)
        {
            m_obj = new(obj);
            Initialize(obj);
        }
    }

    private ThemeHelper(IThemeAwareEx obj)
    {
        if (obj != null)
        {
            m_obj = new(obj);
            obj.Disposed += Target_Disposed;
            Initialize(obj);
        }
    }

    public void Update()
    {
        SafeFireThemeChanged(ThemeManager.ShouldUseDarkMode, false);
    }

    public void Dispose()
    {
        if (themeChangeSupported) ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
        GC.SuppressFinalize(this);
    }

    public static void Attach(IThemeAwareEx obj)
    {
        if (obj != null)
        {
            _ = new ThemeHelper(obj);
        }
    }

    public static void Attach(IThemeAware obj, ref ThemeHelper th)
    {
        if (th != null)
        {
            Detach(ref th);
        }

        th = new(obj);
    }

    public static void Detach(ref ThemeHelper th)
    {
        th.Destroy();
        th = null;
    }

    private void Initialize(IThemeAware obj)
    {
        if (themeChangeSupported) ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        obj.UpdateTheme(ThemeManager.ShouldUseDarkMode, true);
    }

    private void SafeFireThemeChanged(bool useDark, bool init)
    {
        if (m_obj.TryGetTarget(out var obj))
        {
            obj.UpdateTheme(useDark, init);
        }
    }

    private void Target_Disposed(object sender, EventArgs e)
    {
        ((IComponent)sender).Disposed -= Target_Disposed;
        Dispose();
    }

    private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
    {
        SafeFireThemeChanged(e.UseDark, false);
    }

    ~ThemeHelper()
    {
        Dispose();
    }
}