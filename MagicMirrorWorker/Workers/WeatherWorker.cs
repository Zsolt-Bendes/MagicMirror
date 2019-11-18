using MagicMirrorWorker.Models.Weather;
using MagicMirrorWorker.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMirrorWorker.Workers
{
    public class WeatherWorker : BackgroundService
    {
        private readonly ILogger<WeatherWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public WeatherWorker(
            ILogger<WeatherWorker> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var getWeatherInGyor = GetWeatherAsync("Győr");
                    var getWeatherInVienna = GetWeatherAsync("Vienna");

                    _cache.Set(Constants.LATEST_FORECAST_CACHE_KEY, new WeatherResults
                    {
                        Weathers = new OpenWeather[]
                        {
                        await getWeatherInGyor,
                        await getWeatherInVienna
                        }
                    });

                    await Task.Delay(TimeSpan.FromMinutes(60));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error fetching weather data: {ex.Data}");
                } 
            }
        }

        private async Task<OpenWeather> GetWeatherAsync(string cityName)
        {
            var client = _httpClientFactory.CreateClient(Constants.OPEN_WEATHER_CLIENT_NAME);
            client.BaseAddress = new Uri(_configuration["OpenWeather:Url"]);

            var response = await client.GetAsync($"weather?q={cityName}&APPID={_configuration["OpenWeather:AppId"]}&units={_configuration["OpenWeather:Unit"]}&lang=hu");
            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<OpenWeather>(await response.Content.ReadAsStreamAsync());
            }

            throw new Exception("Unexpected error fetching weather data");
        }
    }
}
