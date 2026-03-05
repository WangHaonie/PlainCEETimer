using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using PlainCEETimer.Modules;
using PlainCEETimer.WPF.Models;

namespace PlainCEETimer.WPF.ViewModels;

public sealed partial class FontDialogViewModel : ObservableObject
{
    public ObservableCollection<FontWeightModel> FontWeightCollection => field ??=
    [
        /*
         
        字体粗细由小到大 参考：

        FontWeights Class (System.Windows) | Microsoft Learn
        https://learn.microsoft.com/en-us/dotnet/api/system.windows.fontweights

         */

        new("极细", FontWeights.Thin),
        new("特细", FontWeights.ExtraLight),
        new("细体", FontWeights.Light),
        new("常规", FontWeights.Regular),
        new("中等", FontWeights.Medium),
        new("半粗", FontWeights.SemiBold),
        new("粗体", FontWeights.Bold),
        new("特粗", FontWeights.ExtraBold),
        new("黑体", FontWeights.Black),
        new("特黑", FontWeights.ExtraBlack)
    ];

    public ObservableCollection<FontSizeModel> FontSizeCollection => field ??=
        new(FontSizeModel.Yield(10.0, 36.0, 2.0));

    public FontDialogViewModel(FontModel font)
    {
        font ??= FontModel.FromGdiFont(App.AppConfig.Font);
    }
}