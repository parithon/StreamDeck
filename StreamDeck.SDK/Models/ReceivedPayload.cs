using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK.Models
{
    internal enum ReceivedEventType
    {
        didReceiveSettings,
        didReceiveGlobalSettings,
        keyDown,
        keyUp,
        willAppear,
        willDisappear,
        titleParametersDidChange,
        deviceDidConnect,
        deviceDidDisconnect,
        applicationDidLaunch,
        applicationDidTerminate,
        systesmDidWakeUp,
        propertyInspectorDidAppear,
        propertyInspectorDidDisappear,
        sendToPlugin,
        sendToPropertyInspector,
        unknown = 9999
    }

    internal class ReceivedPayload
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

        [JsonProperty("event")]
        public string EventString { get; set; }

        [JsonIgnore]
        public ReceivedEventType Event => !string.IsNullOrEmpty(EventString) ? Enum.Parse<ReceivedEventType>(EventString) : ReceivedEventType.unknown;

        [JsonProperty("device")]
        public string Device { get; set; }

        [JsonProperty("deviceInfo")]
        public Device DeviceInfo { get; set; }

        [JsonProperty("payload")]
        public ActionPayload Payload { get; set; }
    }
}
