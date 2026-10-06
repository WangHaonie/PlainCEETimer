using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PlainCEETimer.WPF.ViewModels;

public sealed partial class ImmersiveExamItem(int index, string name) : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = name;

    [ObservableProperty]
    public partial string Content { get; set; }

    [ObservableProperty]
    public partial Brush ForeBrush { get; set; }

    [ObservableProperty]
    public partial Brush BackBrush { get; set; }

    public int Index => index;
}
