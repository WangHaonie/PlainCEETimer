using System;
using System.Collections.Generic;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs;

public sealed class HotKeyDialog : AppDialog
{
    protected override AppFormParam Params => AppFormParam.AllControl;

    private EventHandler OnUserChanged;
    private PlainLabel LabelHotKey1;
    private PlainLabel LabelHotKey2;
    private PlainLabel LabelHotKey3;
    private PlainHotkeyControl HotkeyCtrl1;
    private PlainHotkeyControl HotkeyCtrl2;
    private PlainHotkeyControl HotkeyCtrl3;
    private PlainHotkeyControl[] HotKeyCtrls;
    private HotKey[] HotKeys;
    private readonly AppConfig AppConfig = App.AppConfig;

    protected override void OnInitializing()
    {
        Text = "快捷键绑定";
        OnUserChanged = (_, _) => UserChanged();
        HotKeys = AppConfig.HotKeys;

        this.AddControls(b =>
        [
            LabelHotKey1 = b.Label(ConfigValidator.GetHokKeyDescription(0)),
            LabelHotKey2 = b.Label(ConfigValidator.GetHokKeyDescription(1)),
            LabelHotKey3 = b.Label(ConfigValidator.GetHokKeyDescription(2)),
            HotkeyCtrl1 = b.HotkeyCtrl(185, OnUserChanged),
            HotkeyCtrl2 = b.HotkeyCtrl(185, OnUserChanged),
            HotkeyCtrl3 = b.HotkeyCtrl(185, OnUserChanged)
        ]);

        HotKeyCtrls = [HotkeyCtrl1, HotkeyCtrl2, HotkeyCtrl3];
        base.OnInitializing();
    }

    protected override void RunLayout(bool isHighDpi)
    {
        ArrangeFirstControl(HotkeyCtrl1);
        ArrangeFirstControl(LabelHotKey1);
        CompactControlX(HotkeyCtrl1, LabelHotKey1, 5);
        CenterControlY(LabelHotKey1, HotkeyCtrl1, -1);
        ArrangeControlYL(LabelHotKey2, LabelHotKey1);
        ArrangeControlYL(LabelHotKey3, LabelHotKey2);
        ArrangeControlYL(HotkeyCtrl2, HotkeyCtrl1, 0, 5);
        ArrangeControlYL(HotkeyCtrl3, HotkeyCtrl2, 0, 5);
        CenterControlY(LabelHotKey2, HotkeyCtrl2, -1);
        CenterControlY(LabelHotKey3, HotkeyCtrl3, -1);
        ArrangeCommonButtonsR(ButtonA, ButtonB, HotkeyCtrl3, 1, 5);
        InitWindowSize(ButtonB, 5, 5);
    }

    protected override void OnLoad()
    {
        if (!HotKeys.IsNullOrEmpty())
        {
            for (int i = 0; i < Math.Min(ConfigValidator.HotKeyCount, HotKeys.Length); i++)
            {
                HotKeyCtrls[i].Hotkey = new(HotKeys[i]);
            }
        }
    }

    protected override bool OnClickButtonA()
    {
        Dictionary<int, HotKey> hkdic = new(ConfigValidator.HotKeyCount);

        for (int i = 0; i < ConfigValidator.HotKeyCount; i++)
        {
            var hk = new HotKey(HotKeyCtrls[i].Hotkey);
            var flag = hkdic.ContainsValue(hk);
            hkdic[i] = hk;

            if (HotKeyService.Test(hk) == HotKeyStatus.Failed || (hk.IsValid && flag))
            {
                MessageX.Error($"无法注册快捷键 \"{ConfigValidator.GetHokKeyDescription(i)}\"，请确保该快捷键未重复且未被其他应用程序注册！");
                return false;
            }
        }

        var arr = new HotKey[ConfigValidator.HotKeyCount];
        hkdic.Values.CopyTo(arr, 0);
        AppConfig.HotKeys = arr;
        ConfigValidator.DemandConfig();
        return base.OnClickButtonA();
    }
}