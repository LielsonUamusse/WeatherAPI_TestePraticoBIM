using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.Context;
using WeatherAPI.ExternalModel;
using WeatherAPI.Model;

namespace WeatherAPI.Service
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient,AppDbContext context,ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _context = context;
            _logger = logger;
        }

        //Consulta para guardar na DB
        public async Task<Weather> ConsultWeatherAsync(string city,string? country)
        {
            _logger.LogInformation(
                "Início da consulta meteorológica. Cidade: {City}, País: {Country}",
                city,
                country);

            var location = await GetLocationAsync(city, country);

            var weather = await GetWeatherAsync(
                location.Latitude,
                location.Longitude);

            var result = new Weather
            {
                City = location.Name,
                Country = location.Country,
                CountryCode = location.CountryCode,
                Region = location.Region,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                TimeZone = weather.Timezone,
                Temperature = weather.Current!.Temperature,
                TemperatureUnit = weather.CurrentUnits!.TemperatureUnit,
                ApparentTemperature = weather.Current.ApparentTemperature,
                Humidity = weather.Current.Humidity,
                HumidityUnit = weather.CurrentUnits.HumidityUnit,
                WindSpeed = weather.Current.WindSpeed,
                WindSpeedUnit = weather.CurrentUnits.WindSpeedUnit,
                WeatherCode = weather.Current.WeatherCode,
                WeatherTime = weather.Current.Time,
                ConsultedAt = DateTime.UtcNow
            };

            try
            {
                _context.HistoricoTemp.Add(result);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Consulta guardada com sucesso. ID: {Id}",result.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex,"Erro ao guardar a consulta no SQL Server.");

                throw new Exception("Não foi possível guardar a consulta na base de dados.",ex);
            }

            return result;
        }

        //Procurar a localizacao pesquisada
        private async Task<GeocodingResult> GetLocationAsync(string city,string? country)
        {
            var url = "https://geocoding-api.open-meteo.com/v1/search" + $"?name={Uri.EscapeDataString(city)}" + "&count=10" + "&language=pt" +"&format=json";

            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var response = await _httpClient.GetAsync(url);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Geocoding respondeu com HTTP {StatusCode} em {Duration} ms",
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException( $"A API de geocodificação respondeu com HTTP {(int)response.StatusCode}.");
                }

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<GeoCodingResponse>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (data?.Results == null || data.Results.Count == 0)
                {
                    throw new KeyNotFoundException("Cidade não encontrada.");
                }

                IEnumerable<GeocodingResult> locations =
                    data.Results;

                if (!string.IsNullOrWhiteSpace(country))
                {
                    locations = locations.Where(x =>string.Equals( x.Country, country, StringComparison.OrdinalIgnoreCase) || string.Equals( x.CountryCode, country, StringComparison.OrdinalIgnoreCase));
                }

                var location = locations.FirstOrDefault();

                if (location == null)
                {
                    throw new KeyNotFoundException("Cidade não encontrada para o país informado.");
                }

                return location;
            }
            catch (TaskCanceledException ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Timeout na API de geocodificação após {Duration} ms", stopwatch.ElapsedMilliseconds);

                throw new HttpRequestException("Timeout ao consultar a API de geocodificação.", ex);
            }
        }

        private async Task<DocsResponse> GetWeatherAsync(double latitude, double longitude)
        {
            var latitudeText = latitude.ToString(CultureInfo.InvariantCulture);

            var longitudeText =  longitude.ToString(CultureInfo.InvariantCulture);

            var url = "https://api.open-meteo.com/v1/forecast" + $"?latitude={latitudeText}" + $"&longitude={longitudeText}" + "&current=temperature_2m," + "relative_humidity_2m," +
                "apparent_temperature," + "weather_code," + "wind_speed_10m" + "&timezone=auto";

            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var response = await _httpClient.GetAsync(url);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Weather respondeu com HTTP {StatusCode} em {Duration} ms",
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"A API meteorológica respondeu com HTTP {(int)response.StatusCode}.");
                }

                var json = await response.Content.ReadAsStringAsync();

                var weather = JsonSerializer.Deserialize<DocsResponse>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (weather?.Current == null ||
                    weather.CurrentUnits == null)
                {
                    throw new HttpRequestException("A API meteorológica devolveu dados incompletos.");
                }

                return weather;
            }
            catch (TaskCanceledException ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Timeout na API meteorológica após {Duration} ms",
                    stopwatch.ElapsedMilliseconds);

                throw new HttpRequestException("Timeout ao consultar a API meteorológica.",ex);
            }
        }

        //a lista das temperaturas
        public async Task<List<Weather>> GetHistoryAsync(string? city, string? country, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.HistoricoTemp.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(x =>x.City.Contains(city));
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(x =>x.Country.Contains(country));
            }

            if (startDate.HasValue)
            {
                query = query.Where(x =>x.ConsultedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var finalDate = endDate.Value.Date.AddDays(1);

                query = query.Where(x =>x.ConsultedAt < finalDate);
            }

            return await query.OrderByDescending(x => x.ConsultedAt).ToListAsync();
        }
        //Pesquisar por Id
        public async Task<Weather?> GetByIdAsync(Guid id)
        {
            return await _context.HistoricoTemp.FindAsync(id);
        }

        //Apagar
        public async Task<bool> DeleteAsync(Guid id)
        {
            var record = await _context.HistoricoTemp.FindAsync(id);

            if (record is null)
            {
                return false;
            }
            _context.HistoricoTemp.Remove(record);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

