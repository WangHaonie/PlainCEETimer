using System.ComponentModel;
using System.Runtime.Serialization;

namespace PlainCEETimer.Modules.Configuration;

public class GeneralObject
{
    public bool AutoSwitch { get; set; }

    public int Interval { get; set; }

    [DefaultValue(true)]
    public bool TrayIcon { get; set; } = true;

    public bool TrayText { get; set; }

    public bool MemClean { get; set; }

    [DefaultValue(true)]
    public bool TopMost { get; set; } = true;

    [DefaultValue(true)]
    public bool UniTopMost { get; set; } = true;

    [DefaultValue(ConfigValidator.DefCpp)]
    public int CountPerPage
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, ConfigValidator.MaxCpp, ConfigValidator.MinCpp, ConfigValidator.DefCpp);
    } = ConfigValidator.DefCpp;

    [DefaultValue(ConfigValidator.MaxOpacity)]
    public int Opacity
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, ConfigValidator.MaxOpacity, ConfigValidator.MinOpacity, ConfigValidator.MaxOpacity);
    } = ConfigValidator.MaxOpacity;

    [DefaultValue(ConfigValidator.DefExamNameTruncate)]
    public int Truncate
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, ConfigValidator.MaxExamNameLength, ConfigValidator.MinExamNameLength, ConfigValidator.DefExamNameTruncate);
    } = ConfigValidator.DefExamNameTruncate;

    public BorderColorObject BorderColor { get; set; }

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        TrayText = ConfigValidator.ValidateBoolean(TrayText, TrayIcon);
    }
}
