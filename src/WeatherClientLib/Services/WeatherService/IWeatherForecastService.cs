using MagicMirrorWorker.Protos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherClientLib.Services.WeatherService
{
    public interface IWeatherForecastService
    {
        IAsyncEnumerable<WeatherResponse> GetStreamingWeather(CancellationToken token);
        Task<WeatherResponse> GetWeather();
    }
}
