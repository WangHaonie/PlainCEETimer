using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Fody;

namespace PlainCEETimer.UI;

[NoConstants]
internal static class HotKeyManager
{
    /*
    
    注册全局热键 参考：

    .net - Set global hotkeys using C# - Stack Overflow
    https://stackoverflow.com/a/27309185/21094697

    WM.HOTKEY message (Winuser.h) - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-hotkey

    */

    internal sealed class MessageWindow : NativeWindow, IDisposable
    {
        public MessageWindow()
        {
            CreateHandle(new() { Parent = new(NativeConstants.HWND_MESSAGE) });
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM.HOTKEY && hksvcs != null)
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

        ~MessageWindow()
        {
            Dispose();
        }
    }

    internal readonly struct ValidationResult(List<int> failed)
    {
        public bool Failed => FailedCount > 0;

        private readonly ReadOnlyCollection<int> FailedIndices = failed.AsReadOnly();
        private readonly int FailedCount = failed.Count;

        public string GetMessage()
        {
            if (FailedCount == 0)
            {
                return null;
            }

            var sb = new StringBuilder(64);
            sb.AppendLine("以下快捷键注册失败，请确保它们未重复且未被其他应用程序占用！");
            sb.AppendLine();

            foreach (var index in FailedIndices)
            {
                sb.Append("  • ").AppendLine(GetHotKeyDescription(index));
            }

            return sb.ToString();
        }

        public bool PopupIfFailed(IDialogService dialog)
        {
            var errmsg = GetMessage();
            var result = errmsg != null;

            if (result)
            {
                dialog.Warn(errmsg);
            }

            return result;
        }
    }

    public const int HotKeyCount = 3;

    private static readonly List<HotKeyService> hksvcs = [];
    private static readonly HotKeyPressEventHandler[] hkhandlers = new HotKeyPressEventHandler[HotKeyCount];

    static HotKeyManager()
    {
        App.AppExit += UnregisterAll;
    }

    public static HotKey[] EnsureHotKeys(HotKey[] value)
    {
        var arr = new HotKey[HotKeyCount];

        if (value != null)
        {
            int length = value.Length;

            if (length == HotKeyCount)
            {
                return value;
            }

            length = Math.Min(HotKeyCount, length);

            for (int i = 0; i < length; i++)
            {
                arr[i] = value[i];
            }
        }

        return arr;
    }

    public static string GetHotKeyDescription(int index) => index switch
    {
        0 => "隐藏主窗口",
        1 => "上一个考试",
        2 => "下一个考试",
        _ => null
    };

    public static void SetHandlers(params HotKeyPressEventHandler[] handlers)
    {
        var length = Math.Min(handlers.Length, HotKeyCount);

        for (int i = 0; i < length; i++)
        {
            hkhandlers[i] = handlers[i];
        }
    }

    public static ValidationResult ValidateHotKeys(HotKey[] hotKeys)
    {
        var result = Validate(hotKeys);

        if (result.Failed)
        {
            return result;
        }

        UnregisterAll();

        var failed = new List<int>();

        for (int i = 0; i < hotKeys.Length; i++)
        {
            var hk = hotKeys[i];

            if (!hk.IsValid)
            {
                continue;
            }

            var handler = hkhandlers[i];
            var svc = new HotKeyService(hk, handler);

            if (svc.Register())
            {
                hksvcs.Add(svc);
            }
            else
            {
                failed.Add(i);
            }
        }

        return new(failed);
    }

    public static void UnregisterAll()
    {
        var length = hksvcs.Count;

        for (int i = 0; i < length; i++)
        {
            hksvcs[i]?.Unregister();
            hksvcs[i] = null;
        }
    }

    private static ValidationResult Validate(HotKey[] hotKeys)
    {
        var set = new HashSet<HotKey>();
        var failed = new List<int>();

        for (int i = 0; i < hotKeys.Length; i++)
        {
            var hk = hotKeys[i];

            if (!hk.IsValid)
            {
                continue;
            }

            if (!set.Add(hk))
            {
                failed.Add(i);
                continue;
            }

            if (HotKeyService.Test(hk) == HotKeyStatus.Failed)
            {
                failed.Add(i);
            }
        }

        return new(failed);
    }
}