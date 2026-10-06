using System.Text.Json.Serialization;

namespace TeleVisionGuide.Json
{
    public class T4aTextsClass
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("VeryShort")]
        public T4aLong? VeryShort { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("Long")]
        public T4aLong? Long { get; set; }
    }
}
