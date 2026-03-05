using System.Windows;

namespace PlainCEETimer.WPF.Models;

public class FontWeightModel(string name, FontWeight value)
{
    public string Display => name;

    public FontWeight FontWeight => value;
}