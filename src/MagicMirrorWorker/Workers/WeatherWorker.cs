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
        private const int _open_weather_refresh_interval = 10;

        private static readonly string[] _cities = { "Gyor", "Vienna" };
        private readonly List<Task> _tasks = new(_cities.Length * 2);

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

                    await Task.WhenAll(_tasks);

                    _cache.Set(Constants.LATEST_FORECAST_CACHE_KEY, CreateWeatherResultsContainer(_tasks));
                    await Task.Delay(TimeSpan.FromMinutes(_open_weather_refresh_interval), stoppingToken);

                }
                catch (TimeoutException timeOutEx)
                {
                    // Log timeout and honoring the expires time
                    _logger.LogWarning($"Open weather API limit reached. Data: {timeOutEx.Data}");
                    await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
                }
                catch (Exception ex)
                {
                    await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
                    _logger.LogError($"Unexpected error fetching weather data: {ex.Data}");
                }
                finally
                {
                    _tasks.Clear();
                }
            }

            static WeatherResults CreateWeatherResultsContainer(List<Task> tasks)
            {
                WeatherResults container = new(_cities.Length);

                foreach (var task in tasks)
                {
                    switch (task)
                    {
                        case Task<WeatherCurrent> currentTask:
                            container.CurrentWeathers.Add(currentTask.Result);
                            break;
                        case Task<WeatherForecast> forecastTask:
                            container.Forecasts.Add(forecastTask.Result);
                            break;
                    }
                }

                return container;
            }
        }
    }
}
