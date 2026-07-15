using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Service;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather(
            [FromQuery] string city,
            [FromQuery] string? country)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest(new
                {
                    message = "O parâmetro city é obrigatório."
                });
            }

            var result =
                await _weatherService.ConsultWeatherAsync(
                    city.Trim(),
                    country?.Trim());

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] string? city,
            [FromQuery] string? country,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var history =
                await _weatherService.GetHistoryAsync(
                    city,
                    country,
                    startDate,
                    endDate);

            return Ok(history);
        }

        [HttpGet("history/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var record =
                await _weatherService.GetByIdAsync(id);

            if (record == null)
            {
                return NotFound(new
                {
                    message = "Registo não encontrado."
                });
            }

            return Ok(record);
        }

        [HttpDelete("history/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted =
                await _weatherService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Registo não encontrado."
                });
            }

            return NoContent();
        }
    }
}
