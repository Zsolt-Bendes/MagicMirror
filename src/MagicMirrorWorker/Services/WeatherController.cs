﻿using MagicMirrorWorker.Protos;
using MagicMirrorWorker.Utilities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MagicMirrorWorker.Services
{
    [ApiController]
    public class WeatherController : Controller
    {
        private readonly IMemoryCache _cache;

        public WeatherController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [EnableCors()]
        [HttpGet("/json")]
        public ActionResult<WeatherResponse> GetWeatherForecast()
        {
            var forecast = _cache.Get<Models.Weather.WeatherResults>(Constants.LATEST_FORECAST_CACHE_KEY);
            return WeatherService.ConvertToCurrentWeatherResponse(forecast);
        }
    }
}
