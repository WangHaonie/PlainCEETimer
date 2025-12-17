using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules;

public static class HotKeyHelper
{
    /*
    
    注册全局热键 参考：

    .net - Set global hotkeys using C# - Stack Overflow
    https://stackoverflow.com/a/27309185/21094697

    WM_HOTKEY message (Winuser.h) - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-hotkey

     */

    private class HotKeyMessageWindow : NativeWindow
    {
        public HotKeyMessageWindow()
        {
            const int HWND_MESSAGE = -3;
            CreateHandle(new() { Parent = new(HWND_MESSAGE) });
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY)
            {
                OnHotKeyPress(new(m.WParam, m.LParam));
            }

            base.WndProc(ref m);
        }
    }

    public static event EventHandler<HotKeyPressEventArgs> HotKeyPress;

    private static int hkId;
    private static IntPtr hhkmw;
    private static HotKeyMessageWindow hkmw;
    private static Dictionary<HotKey, int> hks;

    /// <summary>
    /// 测试是否可以注册指定的热键。
    /// </summary>
    /// <returns>0 - 可以注册该热键；1 - 已在当前应用程序注册；2 - 热键值无效；3 - 无法注册该热键</returns>
    public static int Test(HotKey hk)
    {
        if (!hk.IsValid)
        {
            return 2;
        }

        if (hks != null && hks.ContainsKey(hk))
        {
            return 1;
        }

        if (Win32UI.RegisterHotKey(IntPtr.Zero, 0xBFFF - 1, (uint)hk.Modifiers, hk.Key))
        {
            Win32UI.UnregisterHotKey(IntPtr.Zero, 0xBFFF - 1);
            return 0;
        }

        return 3;
    }

    /// <summary>
    /// 注册指定的热键。
    /// </summary>
    /// <returns>0 - 可以注册该热键；1 - 已在当前应用程序注册；2 - 热键值无效；3 - 无法注册该热键</returns>
    public static int Register(HotKey hk)
    {
        if (!hk.IsValid)
        {
            return 2;
        }

        if (hkmw == null)
        {
            hkmw = new();
            hhkmw = hkmw.Handle;
        }

        hks ??= [];

        if (hks.ContainsKey(hk))
        {
            return 1;
        }

        const ushort MOD_NOREPEAT = 0x4000;

        if (Win32UI.RegisterHotKey(hhkmw, ++hkId, MOD_NOREPEAT | (uint)hk.Modifiers, hk.Key))
        {
            hks.Add(hk, hkId);
            return 0;
        }

        hkId--;
        return 3;
    }

    public static bool UnRegister(HotKey hk)
    {
        if (UnRegisterCore(hk))
        {
            Remove(hk);
            return true;
        }

        return false;
    }

    public static void UnRegisterAll()
    {
        if (hkId != 0)
        {
            foreach (var hk in hks.Keys)
            {
                UnRegisterCore(hk);
            }

            hks.Clear();
            Remove(default);
            hkId = 0;
        }

        HotKeyPress = null;
    }

    private static bool UnRegisterCore(HotKey hk)
    {
        if (!hks.ContainsKey(hk))
        {
            return false;
        }

        if (Win32UI.UnregisterHotKey(hhkmw, hks[hk]))
        {
            return true;
        }

        return false;
    }

    private static void Remove(HotKey hk)
    {
        hks.Remove(hk);

        if (hks.Count == 0)
        {
            hkmw.DestroyHandle();
            hkmw = null;
            hks = null;
        }
    }

    private static void OnHotKeyPress(HotKeyPressEventArgs e)
    {
        HotKeyPress?.Invoke(null, e);
    }
}

public class HotKeyPressEventArgs(IntPtr wParam, IntPtr lParam)
{
    public int Id { get; } = wParam.ToInt32();

    public HotKey HotKey { get; } = new(lParam);
}
