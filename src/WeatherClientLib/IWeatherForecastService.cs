using MagicMirrorWorker.Protos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherClientLib
{
	public interface IWeatherForecastService
	{
		IAsyncEnumerable<WeatherResponse> GetStreamingWeather(CancellationToken token);
		Task<WeatherResponse> GetWeather();
	}
}
