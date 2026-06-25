using System.Text.Json.Serialization;

namespace BeitKnesetDisplay.Models
{
    public class ZmanimResponse
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("times")]
        public ZmanimTimes? Times { get; set; }
    }

    public class ZmanimTimes
    {
        [JsonPropertyName("sunrise")]
        public string? Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public string? Sunset { get; set; }

        [JsonPropertyName("sofZmanShma")]
        public string? SofZmanShma { get; set; }

        [JsonPropertyName("chatzot")]
        public string? Chatzot { get; set; }

        [JsonPropertyName("minchaGedola")]
        public string? MinchaGedola { get; set; }

        [JsonPropertyName("minchaKetana")]
        public string? MinchaKetana { get; set; }

        [JsonPropertyName("plagHaMincha")]
        public string? PlagHaMincha { get; set; }
    }

    public class ShabbatResponse
    {
        [JsonPropertyName("items")]
        public List<ShabbatItem>? Items { get; set; }
    }

    public class ShabbatItem
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("hebrew")]
        public string? Hebrew { get; set; }
    }
}