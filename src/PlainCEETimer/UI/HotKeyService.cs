using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI;

[DebuggerDisplay("{hid}")]
public class HotKeyService(HotKey hk, EventHandler<HotKeyPressEventArgs> onHotKeyPress)
{
    /*
    
    注册全局热键 参考：

    .net - Set global hotkeys using C# - Stack Overflow
    https://stackoverflow.com/a/27309185/21094697

    WM_HOTKEY message (Winuser.h) - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-hotkey

    */

    private sealed class HotKeyMessageWindow : NativeWindow, IDisposable
    {
        public HotKeyMessageWindow()
        {
            const int HWND_MESSAGE = -3;
            CreateHandle(new() { Parent = new(HWND_MESSAGE) });
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY && hksvcs != null)
            {
                var count = hksvcs.Count;

                for (int i = 0; i < count; i++)
                {
                    if (hksvcs[i].WmHotKey(ref m))
                    {
                        break;
                    }
                }
            }

            base.WndProc(ref m);
        }

        public void Dispose()
        {
            DestroyHandle();
            GC.SuppressFinalize(this);
        }

        ~HotKeyMessageWindow()
        {
            Dispose();
        }
    }

    private int m_id;
    private bool registered;
    private IntPtr hid;
    private static IntPtr hhkmw;
    private static RandomUID hkids;
    private static List<HotKeyService> hksvcs;
    private static Dictionary<int, HotKey> hks;
    private static HotKeyMessageWindow hkmw;

    static HotKeyService()
    {
        App.AppExit += () =>
        {
            if (hksvcs != null)
            {
                var length = hksvcs.Count;

                for (int i = 0; i < length; i++)
                {
                    hksvcs[i].Unregister();
                }

                hksvcs.Clear();
            }

            hkmw.Destory();
            hkmw = null;
            hks?.Clear();
        };
    }

    public bool Register()
    {
        if (onHotKeyPress == null || !hk.IsValid)
        {
            return true;
        }

        hks ??= [];

        if (hks.ContainsValue(hk))
        {
            return false;
        }

        hkids ??= new(0x0001, 0xBFFF);
        hkmw ??= new();
        hhkmw = hkmw.Handle;
        const ushort MOD_NOREPEAT = 0x4000;

        if (TestCore(hk) && Win32UI.RegisterHotKey(hkmw.Handle, m_id = GetId(), MOD_NOREPEAT | (uint)hk.Modifiers, hk.Key))
        {
            hksvcs ??= [];
            hid = new(m_id);
            registered = true;
            hksvcs.Add(this);
            return true;
        }

        return false;
    }

    public bool Unregister()
    {
        if (hks != null && (!registered || Win32UI.UnregisterHotKey(hhkmw, m_id)))
        {
            hks.Remove(m_id);
            hkids.Remove(m_id);
            return true;
        }

        return false;
    }

    private int GetId()
    {
        var id = hkids.Next();
        hks[id] = hk;
        return id;
    }

    private bool WmHotKey(ref Message m)
    {
        if (m.WParam == hid)
        {
            onHotKeyPress(this, new(m.WParam, m.LParam));
            return true;
        }

        return false;
    }

    public static HotKeyStatus Test(HotKey hk)
    {
        if (!hk.IsValid)
        {
            return HotKeyStatus.Invalid;
        }

        if (hks != null && hks.ContainsValue(hk))
        {
            return HotKeyStatus.Ready;
        }

        if (TestCore(hk))
        {
            return HotKeyStatus.Success;
        }

        return HotKeyStatus.Failed;
    }

    private static bool TestCore(HotKey hk)
    {
        if (Win32UI.RegisterHotKey(IntPtr.Zero, 1, (uint)hk.Modifiers, hk.Key))
        {
            Win32UI.UnregisterHotKey(IntPtr.Zero, 1);
            return true;
        }

        return false;
    }
}