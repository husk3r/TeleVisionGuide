using System.Text.Json.Serialization;

namespace TeleVisionGuide.Json
{
    public class T4aEvent
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("eventId")]
        public string EventId { get; set; }

        [JsonPropertyName("airDate")]
        public DateTimeOffset AirDate { get; set; }

        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; set; }

        [JsonPropertyName("linkUrl")]
        public object LinkUrl { get; set; }

        [JsonPropertyName("duration")]
        public long Duration { get; set; }

        [JsonPropertyName("subtitled")]
        public long Subtitled { get; set; }

        [JsonPropertyName("teletextSubtitled")]
        public long TeletextSubtitled { get; set; }

        [JsonPropertyName("visualDescription")]
        public long VisualDescription { get; set; }

        [JsonPropertyName("channel")]
        public long Channel { get; set; }

        [JsonPropertyName("content")]
        public T4aContent Content { get; set; }

        [JsonPropertyName("highlight")]
        public bool? Highlight { get; set; }

        [JsonPropertyName("mediathek")]
        public Uri? Mediathek { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("photo")]
        public T4aPhoto? Photo { get; set; }
    }
}
