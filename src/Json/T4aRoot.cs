using System.Text.Json;
using System.Text.Json.Serialization;
namespace TeleVisionGuide.Json
{
    public class T4aRoot
    {
        [JsonPropertyName("events")]
        public T4aEvent[] Events { get; set; }

        [JsonPropertyName("cachetime")]
        public string Cachetime { get; set; }

        public static T4aRoot FromJson(string json) => JsonSerializer.Deserialize<T4aRoot>(json, T4aConverter.Settings);
    }
}