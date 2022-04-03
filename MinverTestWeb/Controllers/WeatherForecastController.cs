using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MinverTestLib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MinverTestWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherForecastService _weatherForecastService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastService weatherForecastService)
        {
            _logger = logger;
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get() => await _weatherForecastService.GetForecast();
    }
}
