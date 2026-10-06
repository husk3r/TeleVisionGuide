using System.Text.Json;
using System.Text.Json.Serialization;

namespace TeleVisionGuide.Json
{
    public class T4aGenre
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }

    public class T4aGenres
    {
        [JsonPropertyName("genres")]
        public List<T4aGenre> Genres { get; set; }

        public static T4aGenres FromJson(string json)
        {
            string wellFormatted = json;

            for (int i = 1; i < 500; i++)
            {
                wellFormatted = wellFormatted.Replace($"\"{i}\":", "", StringComparison.CurrentCultureIgnoreCase);
            }

            return JsonSerializer.Deserialize<T4aGenres>(wellFormatted);
        }
    }
}
