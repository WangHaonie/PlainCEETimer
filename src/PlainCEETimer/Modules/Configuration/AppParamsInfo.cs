using Newtonsoft.Json;

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
}
