using System;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;

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

    [JsonProperty(AppParams.MainBackdropAcrylic_Key)]
    public bool MainBackdropAcrylic { get; set; }

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

    [JsonProperty(AppParams.TSMax_Key)]
    [JsonConverter(typeof(TimeSpanTicksConverter))]
    public TimeSpan TSMax
    {
        get;
        set => field = !ConfigValidator.ValidateNeeded || value.Ticks > TimeSpan.TicksPerSecond ? value : ConfigValidator.MaxTick;
    } = ConfigValidator.MaxTick;
}
