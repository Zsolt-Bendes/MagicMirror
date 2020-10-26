using MagicMirrorWorker.Protos;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WeatherClientLib.Models;

namespace WeatherClientLib.Services.WeatherService
{
    public class HttpWeatherForecastService : IWeatherForecastService
    {
        private readonly HttpClient _httpClient;
        private readonly string _weatherUri;

        public HttpWeatherForecastService(HttpClient httpClient, IOptionsMonitor<HttpWeatherForecastServiceOptions> options)
        {
            _httpClient = httpClient;
            _weatherUri = options.CurrentValue?.Address?.ToString() ?? "json";
        }

        public async IAsyncEnumerable<WeatherResponse> GetStreamingWeather([EnumeratorCancellation] CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var weather = await _httpClient.GetJsonAsync<WeatherResponse>(_weatherUri);
                yield return weather;
                await Task.Delay(5000); // poll every 5 sec
            }
        }

        public Task<WeatherResponse> GetWeather() => _httpClient.GetJsonAsync<WeatherResponse>(_weatherUri);
    }
}
