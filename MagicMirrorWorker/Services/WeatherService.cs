using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MagicMirrorWorker.Protos;
using MagicMirrorWorker.Utilities.Converters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MagicMirrorWorker.Services
{
	public class WeatherService : Weather.WeatherBase
	{
		private readonly IMemoryCache _cache;

		public WeatherService(IMemoryCache cache)
		{
			_cache = cache;
		}

		public override async Task GetWeatherStream(Empty request, IServerStreamWriter<WeatherResponse> responseStream, ServerCallContext context)
		{
			while (!context.CancellationToken.IsCancellationRequested)
			{
				var cachedForecast = _cache.Get<Models.Weather.WeatherResults>(Constants.LATEST_FORECAST_CACHE_KEY);
				await responseStream.WriteAsync(ConvertToCurrentWeatherResponse(cachedForecast));
				await Task.Delay(TimeSpan.FromMinutes(5));
			}
		}

		public override Task<WeatherResponse> GetWeather(Empty request, ServerCallContext context)
		{
			var forecast = _cache.Get<Models.Weather.WeatherResults>(Constants.LATEST_FORECAST_CACHE_KEY);
			return Task.FromResult(ConvertToCurrentWeatherResponse(forecast));
		}

		public static WeatherResponse ConvertToCurrentWeatherResponse(Models.Weather.WeatherResults weather)
		{
			var response = new WeatherResponse();
			response.Results.AddRange(weather.Weathers.Select(x => x.ConvertToWeatherRespone()));

			return response;
		}
	}
}
