using System.Text.Json.Serialization;

namespace TeleVisionGuide.Json
{
    public class T4aPhoto
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("copyright")]
        public string? Copyright { get; set; }

        [JsonPropertyName("level")]
        public T4aLevel Level { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("orientation")]
        public T4aOrientation Orientation { get; set; }
    }
}
