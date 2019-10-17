using Newtonsoft.Json;
using System.Collections.Generic;

namespace StreamDeck.SDK.Models
{
    internal class ActionPayload
    {
        [JsonProperty("settings")]
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        [JsonProperty("coordinates")]
        public Coordinates Coordinates { get; set; }

        [JsonProperty("state")]
        public int State { get; set; }

        [JsonProperty("userDesiredState")]
        public int UserDesiredState { get; set; }

        [JsonProperty("isInMultiAction")]
        public bool IsInMultiAction { get; set; }
    }
}
