namespace WeatherAPI.Model
{
    public class Weather
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string? Region { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TimeZone { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public string TemperatureUnit { get; set; } = string.Empty;
        public double ApparentTemperature { get; set; }
        public int Humidity { get; set; }
        public string HumidityUnit { get; set; } = string.Empty;
        public double WindSpeed { get; set; }
        public string WindSpeedUnit { get; set; } = string.Empty;
        public int WeatherCode { get; set; }
        public DateTime WeatherTime { get; set; }
        public DateTime ConsultedAt { get; set; } = DateTime.UtcNow;
    }
}
