using Newtonsoft.Json;

namespace StreamDeck.SDK.Models
{
    internal class ActionPayload
    {
        [JsonProperty("settings")]
        public dynamic Settings { get; set; }

        [JsonProperty("coordinates")]
        public Size Coordinates { get; set; }

        [JsonProperty("state")]
        public int State { get; set; }

        [JsonProperty("userDesiredState")]
        public bool UserDesiredState { get; set; }

        [JsonProperty("isInMultiAction")]
        public bool IsInMultiAction { get; set; }
    }
}
