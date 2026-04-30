using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI;

[DebuggerDisplay("{hid}")]
public class HotKeyService(HotKey hk, HotKeyPressEventHandler onHotKeyPress)
{
    private int m_id;
    private bool registered;
    private IntPtr hid;

    private static IntPtr hhkmw;
    private static RandomUID hkids;
    private static Dictionary<int, HotKey> hks;
    private static HotKeyManager.MessageWindow hkmw;

    static HotKeyService()
    {
        App.AppExit += () =>
        {
            hkmw.Destory();
            hkmw = null;
            hks = null;
        };
    }

    public bool Register()
    {
        if (onHotKeyPress == null || !hk.IsValid)
        {
            return false;
        }

        hks ??= [];

        if (hks.ContainsValue(hk))
        {
            return false;
        }

        hkids ??= new(0x0001, 0xBFFF);
        hkmw ??= new();
        hhkmw = hkmw.Handle;

        if (TestCore(hk)
            && Win32UI.RegisterHotKey(hkmw.Handle, m_id = GetId(), NativeConstants.MOD_NOREPEAT | (uint)hk.Modifiers, hk.Key))
        {
            hid = new(m_id);
            registered = true;
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

    internal bool WmHotKey(ref Message m)
    {
        if (m.WParam == hid)
        {
            onHotKeyPress(this, new(m.WParam, m.LParam));
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