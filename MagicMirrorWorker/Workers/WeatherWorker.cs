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
		private static readonly string[] _cities = new string[] { "Gyor", "Vienna" };
		private readonly Task<WeatherCurrent>[] _currentWeatherTasks = new Task<WeatherCurrent>[_cities.Length];
		private readonly Task<WeatherForecast>[] _forecastWeatherTasks = new Task<WeatherForecast>[_cities.Length];

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
					for (int i = 0; i < _cities.Length; i++)
					{
						_currentWeatherTasks[i] = GetWeatherCurrentAsync(_cities[i]);
						_forecastWeatherTasks[i] = GetWeatherForcastAsync(_cities[i]);
					}

					var responseContainer = new WeatherResults
					{
						CurrentWeathers = new WeatherCurrent[_cities.Length],
						Forecasts = new WeatherForecast[_cities.Length]
					};

					await Task.WhenAll(_currentWeatherTasks);

					for (int i = 0; i < _currentWeatherTasks.Length; i++)
					{
						responseContainer.CurrentWeathers[i] = _currentWeatherTasks[i].Result;
					}

					_cache.Set(Constants.LATEST_FORECAST_CACHE_KEY, responseContainer);

					await Task.Delay(TimeSpan.FromMinutes(5));
				}
				catch (Exception ex)
				{
					_logger.LogError($"Unexpected error fetching weather data: {ex.Data}");
				}
			}
		}

		private async Task<WeatherCurrent> GetWeatherCurrentAsync(string cityName)
		{
			var client = _httpClientFactory.CreateClient(Constants.OPEN_WEATHER_CLIENT_NAME);
			client.BaseAddress = new Uri(_configuration["OpenWeather:Url"]);

			var response = await client.GetAsync($"weather?q={cityName}&APPID={_configuration["OpenWeather:AppId"]}&units={_configuration["OpenWeather:Unit"]}&lang={WeatherApiLanguage.hu}");
			if (response.IsSuccessStatusCode)
			{
				return await JsonSerializer.DeserializeAsync<WeatherCurrent>(await response.Content.ReadAsStreamAsync());
			}

			throw new Exception("Unexpected error fetching weather data");
		}

		private async Task<WeatherForecast> GetWeatherForcastAsync(string cityName)
		{
			var client = _httpClientFactory.CreateClient(Constants.OPEN_WEATHER_CLIENT_NAME);
			client.BaseAddress = new Uri(_configuration["OpenWeather:Url"]);

			var response = await client.GetAsync($"forecast?q={cityName}&APPID={_configuration["OpenWeather:AppId"]}&units={_configuration["OpenWeather:Unit"]}&lang={WeatherApiLanguage.hu}");
			if (response.IsSuccessStatusCode)
			{
				return await JsonSerializer.DeserializeAsync<WeatherForecast>(await response.Content.ReadAsStreamAsync());
			}

			throw new Exception("Unexpected error fetching weather forecast data");
		}
	}
}
