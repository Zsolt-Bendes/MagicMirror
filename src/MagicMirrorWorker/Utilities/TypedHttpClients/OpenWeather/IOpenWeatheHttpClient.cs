using MagicMirrorWorker.Models.Weather;
using System.Threading.Tasks;

namespace MagicMirrorWorker.Utilities.TypedHttpClients.OpenWeather
{
    public interface IOpenWeatheHttpClient
    {
        Task<WeatherCurrent> GetCurrentWeatherAsync(string cityName);
        Task<WeatherForecast> GetForecastWeatherAsync(string cityName);
    }
}