using System.Text.Json.Serialization;

namespace TeleVisionGuide.Json
{
    public class T4aLong
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("lang")]
        public T4aLang Lang { get; set; }

        [JsonPropertyName("type")]
        public T4aTypeEnum Type { get; set; }

        [JsonPropertyName("year")]
        [JsonConverter(typeof(T4aParseStringConverter))]
        public long? Year { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
