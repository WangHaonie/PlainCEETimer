using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Dialogs;

public sealed class ImmersiveOptionsDialog : AppDialog
{
    protected override AppWindowStyle Params => AppWindowStyle.AllControl;

    private PlainTabControl TabControlMain;

    protected override void OnInitializing()
    {
        Text = "沉浸模式选项 - 高考倒计时";

        this.AddControls(b =>
        [
            TabControlMain = b.TabCtrl(300, 150, false,
            [
                b.TabPage("常规",
                [

                ])
            ])
        ]);

        base.OnInitializing();
    }

    protected override void RunLayout(bool init, bool isHighDpi)
    {
        ArrangeFirstControl(TabControlMain);
        ArrangeCommonButtonsR(ButtonA, ButtonB, TabControlMain, 0, 3);
        InitWindowSize(ButtonB, 3, 3);
    }
}
