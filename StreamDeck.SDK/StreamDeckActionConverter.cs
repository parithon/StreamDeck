using StreamDeck.SDK.Abstractions;
using Newtonsoft.Json;
using StreamDeck.SDK.Models;
using System;

namespace StreamDeck.SDK
{
    internal class StreamDeckActionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IStreamDeckAction);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(StreamDeckAction));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
