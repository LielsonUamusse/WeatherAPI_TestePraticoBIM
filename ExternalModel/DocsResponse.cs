using System.Text.Json.Serialization;

namespace WeatherAPI.ExternalModel
{
    public class DocsResponse
    {
        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }

        [JsonPropertyName("current_units")]
        public CurrentUnits? CurrentUnits { get; set; }
    }
    public class CurrentWeather
    {
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int Humidity { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public double ApparentTemperature { get; set; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }
    }

    public class CurrentUnits
    {
        [JsonPropertyName("temperature_2m")]
        public string TemperatureUnit { get; set; } = string.Empty;

        [JsonPropertyName("relative_humidity_2m")]
        public string HumidityUnit { get; set; } = string.Empty;

        [JsonPropertyName("wind_speed_10m")]
        public string WindSpeedUnit { get; set; } = string.Empty;
    }
}
