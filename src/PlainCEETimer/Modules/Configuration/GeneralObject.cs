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

    [DefaultValue(Validator.DefCpp)]
    public int CountPerPage
    {
        get;
        set => Validator.SetValue(ref field, value, Validator.MaxCpp, Validator.MinCpp, Validator.DefCpp);
    } = Validator.DefCpp;

    [DefaultValue(Validator.MaxOpacity)]
    public int Opacity
    {
        get;
        set => Validator.SetValue(ref field, value, Validator.MaxOpacity, Validator.MinOpacity, Validator.MaxOpacity);
    } = Validator.MaxOpacity;

    public BorderColorObject BorderColor { get; set; }

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        TrayText = Validator.ValidateBoolean(TrayText, TrayIcon);
    }
}
