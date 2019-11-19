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
		private static readonly string[] _cities = new string[] { "Győr", "Vienna" };
		private readonly Task<OpenWeather>[] _weatherTasks = new Task<OpenWeather>[_cities.Length];

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
						_weatherTasks[i] = GetWeatherAsync(_cities[i]);
					}

					var responseContainer = new WeatherResults
					{
						Weathers = new OpenWeather[_cities.Length]
					};

					await Task.WhenAll(_weatherTasks);

					for (int i = 0; i < _weatherTasks.Length; i++)
					{
						responseContainer.Weathers[i] = _weatherTasks[i].Result;
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
