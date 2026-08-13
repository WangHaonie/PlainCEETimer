using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules.Configuration;

public class AppParamsInfo
{
    public bool Debug { get; set; }

    [JsonProperty(AppParams.DisableWFPMv2_Key)]
    public bool DisableWFPMv2 { get; set; }

    [JsonProperty(AppParams.EnableCommDlgPMv2_Key)]
    public bool EnableCommDlgPMv2 { get; set; }

    [JsonProperty(AppParams.UseClassicTSP_Key)]
    public bool UseClassicTSP { get; set; }

    [JsonProperty(AppParams.TSFormat_Key)]
    public string TSFormat
    {
        get;
        set
        {
            value = value.Clean();

            if (ConfigValidator.ValidateNeeded
                && (string.IsNullOrWhiteSpace(value) || !TimeSpanFormat.ValidateFormat(value)))
            {
                value = null;
            }

            field = value;
        }
    }
}
