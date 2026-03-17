using System.Windows;

namespace PlainCEETimer.WPF.Models;

public class FontWeightItem(string name, FontWeight value)
{
    public string Display => name;

    public FontWeight FontWeight => value;
}