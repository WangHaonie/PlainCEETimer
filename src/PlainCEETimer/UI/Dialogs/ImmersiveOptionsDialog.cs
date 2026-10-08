using System;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs;

public sealed class ImmersiveOptionsDialog : AppDialog
{
    protected override AppWindowStyle Params => AppWindowStyle.AllControl;

    private PlainTabControl TabControlMain;
    private PlainCheckBox CheckBoxShowPrevNext;
    private PlainCheckBox CheckBoxSkipExams;
    private PlainCheckBox CheckBoxShowLoop;
    private PlainCheckBox CheckBoxNoAnimate;
    private PlainCheckBox CheckBoxUnTopMost;
    private EventHandler OnUserChanged;

    protected override void OnInitializing()
    {
        Text = "沉浸模式选项 - 高考倒计时";
        OnUserChanged = (_, _) => UserChanged();

        this.AddControls(b =>
        [
            TabControlMain = b.TabCtrl(180, 100, false,
            [
                b.TabPage("首页",
                [
                    CheckBoxShowPrevNext = b.CheckBox("预览前后考试的倒计时(&P)", (_, _) =>
                    {
                        var c = CheckBoxShowPrevNext.Checked;
                        CheckBoxShowLoop.Enabled = c;
                        CheckBoxSkipExams.Enabled = c;
                        UserChanged();
                    }),

                    CheckBoxSkipExams = b.CheckBox("跳过预览无倒计时的考试(&S)", OnUserChanged).Disable(),
                    CheckBoxShowLoop = b.CheckBox("循环显示前后考试的倒计时(&L)", OnUserChanged).Disable()
                ]),

                b.TabPage("动画",
                [
                    CheckBoxNoAnimate = b.CheckBox("关闭所有动画效果(&N)", OnUserChanged)
                ]),

                b.TabPage("行为",
                [
                    CheckBoxUnTopMost = b.CheckBox("禁止在全屏模式下设置顶置(&U)", OnUserChanged)
                ])
            ])
        ]);

        base.OnInitializing();
    }

    protected override void RunLayout(bool init, bool isHighDpi)
    {
        ArrangeFirstControl(TabControlMain, 6);

        ArrangeFirstControl(CheckBoxShowPrevNext, 6);
        ArrangeControlYL(CheckBoxSkipExams, CheckBoxShowPrevNext, 0, 3);
        ArrangeControlYL(CheckBoxShowLoop, CheckBoxSkipExams, 0, 3);

        ArrangeFirstControl(CheckBoxNoAnimate, 6);
        ArrangeFirstControl(CheckBoxUnTopMost, 6);

        ArrangeCommonButtonsR(ButtonA, ButtonB, TabControlMain, 0, 3);
        InitWindowSize(ButtonB, 6, 3);
    }

    protected override void OnLoad()
    {
        var c = App.Current.AppConfig.Immersive;

        if (c != null)
        {
            var pn = c.PrevNext;
            CheckBoxShowPrevNext.Checked = pn;
            CheckBoxShowLoop.Checked = pn && c.Loop;
            CheckBoxSkipExams.Checked = pn && c.SkipExams;
            CheckBoxNoAnimate.Checked = c.NoAnimate;
            CheckBoxUnTopMost.Checked = c.UnTopMost;
        }
    }

    protected override bool OnClickButtonA()
    {
        var a = App.Current.AppConfig;
        var i = a.Immersive;
        i = i == null ? new() : i.CreateCopy();

        var pn = CheckBoxShowPrevNext.Checked;
        i.PrevNext = pn;
        i.Loop = pn && CheckBoxShowLoop.Checked;
        i.SkipExams = pn && CheckBoxSkipExams.Checked;
        i.NoAnimate = CheckBoxNoAnimate.Checked;
        i.UnTopMost = CheckBoxUnTopMost.Checked;

        a.Immersive = i;
        return base.OnClickButtonA();
    }
}
