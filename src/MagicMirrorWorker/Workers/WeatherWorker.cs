using MagicMirrorWorker.Models.Weather;
using MagicMirrorWorker.Utilities;
using MagicMirrorWorker.Utilities.TypedHttpClients.OpenWeather;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MagicMirrorWorker.Workers
{
    public class WeatherWorker : BackgroundService
    {
        private const int OPEN_WEATHER_REFRESH_INTERVAL = 10;

        private static readonly string[] _cities = new string[] { "Gyor", "Vienna" };
        private readonly List<Task> _tasks = new List<Task>(_cities.Length * 2);

        private readonly ILogger<WeatherWorker> _logger;
        private readonly IOpenWeatheHttpClient _openWeatheHttpClient;
        private readonly IMemoryCache _cache;

        public WeatherWorker(
            ILogger<WeatherWorker> logger,
            IOpenWeatheHttpClient openWeatheHttpClient,
            IMemoryCache cache)
        {
            _logger = logger;
            _openWeatheHttpClient = openWeatheHttpClient;
            _cache = cache;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var item in _cities)
                    {
                        _tasks.Add(_openWeatheHttpClient.GetCurrentWeatherAsync(item));
                        _tasks.Add(_openWeatheHttpClient.GetForecastWeatherAsync(item));
                    }

                    await Task.WhenAll(_tasks.ToArray());

                    _cache.Set(Constants.LATEST_FORECAST_CACHE_KEY, CreateWeatherResultsContainer());
                    await Task.Delay(TimeSpan.FromMinutes(OPEN_WEATHER_REFRESH_INTERVAL));

                }
                catch (TimeoutException timeOutEx)
                {
                    // Log timeout and honoring the expires time
                    _logger.LogWarning($"Open weather API limit reached. Data: {timeOutEx.Data}");
                    await Task.Delay(TimeSpan.FromMinutes(60));
                }
                catch (Exception ex)
                {
                    await Task.Delay(TimeSpan.FromMinutes(60));
                    _logger.LogError($"Unexpected error fetching weather data: {ex.Data}");
                }
                finally
                {
                    _tasks.Clear();
                }
            }

            WeatherResults CreateWeatherResultsContainer()
            {
                var container = new WeatherResults(_cities.Length);

                foreach (var task in _tasks)
                {
                    if (task is Task<WeatherCurrent> currentTask)
                    {
                        container.CurrentWeathers.Add(currentTask.Result);
                    }
                    if (task is Task<WeatherForecast> forecastTask)
                    {
                        container.Forecasts.Add(forecastTask.Result);
                    }
                }

                return container;
            }
        }
    }
}
