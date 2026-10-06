using System.Text.Json;
using System.Text.Json.Serialization;

namespace TeleVisionGuide.Json
{
    public class T4aChannel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("channelId")]
        public string ChannelId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
        [JsonPropertyName("order")]
        public long Order { get; set; }
    }

    public class T4aChannels
    {
        [JsonPropertyName("channels")]
        public List<T4aChannel> Channels { get; set; }

        public static T4aChannels FromJson(string json) => JsonSerializer.Deserialize<T4aChannels>(json);
    }
}
