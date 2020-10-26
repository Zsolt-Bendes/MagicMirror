using MagicMirrorWorker.Models.Options;
using MagicMirrorWorker.Models.Weather;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MagicMirrorWorker.Utilities.TypedHttpClients.OpenWeather
{
    public class OpenWeatheHttpClient : IOpenWeatheHttpClient
    {
        private readonly OpenWeatherApiOptions _options;
        private readonly HttpClient _httpClient;

        public OpenWeatheHttpClient(IOptions<OpenWeatherApiOptions> options, HttpClient httpClient)
        {
            _options = options.Value;
            _httpClient = httpClient; 
            _httpClient.BaseAddress = new Uri(_options.Url);
        }

        public async Task<WeatherCurrent> GetCurrentWeatherAsync(string cityName)
        {
            var response = await _httpClient.GetAsync($"weather{BuildWeatherApiQueryParams(cityName)}");
            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<WeatherCurrent>(await response.Content.ReadAsStreamAsync());
            }
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new TimeoutException();
            }

            throw new Exception("Unexpected error fetching weather data");
        }

        public async Task<WeatherForecast> GetForecastWeatherAsync(string cityName)
        {
            var response = await _httpClient.GetAsync($"forecast{BuildWeatherApiQueryParams(cityName)}");
            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<WeatherForecast>(await response.Content.ReadAsStreamAsync());
            }
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new TimeoutException();
            }

            throw new Exception("Unexpected error fetching weather forecast data");
        }

        private string BuildWeatherApiQueryParams(string cityName) => new QueryBuilder(new Dictionary<string, string>
            {
                {"q", cityName },
                {"APPID", _options.AppKey },
                {"units", _options.Unit },
                {"lang", $"{_options.Language}" }
            }).ToQueryString().ToString();
    }
}
