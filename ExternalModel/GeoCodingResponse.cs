using System.Text.Json.Serialization;

namespace WeatherAPI.ExternalModel
{
    public class GeoCodingResponse
    {
        [JsonPropertyName("results")]
        public List<GeocodingResult>? Results { get; set; }
    }
    public class GeocodingResult
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; } = string.Empty;

        [JsonPropertyName("admin1")]
        public string? Region { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [JsonPropertyName("population")]
        public long? Population { get; set; }
    }
}
