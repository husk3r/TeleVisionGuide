using System.Text.Json.Serialization;

namespace TeleVisionGuide.Json
{
    public class T4aContent
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("subtitle")]
        public string? Subtitle { get; set; }

        [JsonPropertyName("contentId")]
        public long ContentId { get; set; }

        [JsonPropertyName("baseContentId")]
        public long? BaseContentId { get; set; }

        [JsonPropertyName("originalTitle")]
        public string? OriginalTitle { get; set; }

        [JsonPropertyName("titleLang")]
        public string TitleLang { get; set; }

        [JsonPropertyName("numerationType")]
        public T4aNumerationType? NumerationType { get; set; }

        [JsonPropertyName("numerationSource")]
        public T4aNumerationSource? NumerationSource { get; set; }

        [JsonPropertyName("episodeNumber")]
        public long? EpisodeNumber { get; set; }

        [JsonPropertyName("seasonNumber")]
        public long? SeasonNumber { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("year")]
        public string? Year { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("category")]
        public long? Category { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("genre")]
        public long? Genre { get; set; }

        [JsonPropertyName("texts")]
        public T4aTextsUnion Texts { get; set; }
    }
}
