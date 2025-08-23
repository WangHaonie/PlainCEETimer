using System;
using Newtonsoft.Json;

namespace PlainCEETimer.Modules
{
    public class ResponseObject
    {
        [JsonProperty("name")]
        public string Version { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishDate { get; set; }

        [JsonProperty("size")]
        public long UpdateSize { get; set; }

        [JsonProperty("body")]
        public string UpdateLog { get; set; }
    }
}
