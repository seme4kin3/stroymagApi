using Infrastructure.Import;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("excel")]
        public async Task<IActionResult> UploadExcel(IFormFile file, [FromServices] IExcelImportService import, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не передан");

            using var stream = file.OpenReadStream();
            var result = await import.ImportAsync(stream, ct);

            // Если остановились из-за пустого SKU — это «частичный успех», но не ошибка 500/400.
            return Ok(result);
        }
    }
}
