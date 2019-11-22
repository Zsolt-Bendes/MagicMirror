using System.Collections.Generic;

namespace MagicMirrorWorker.Models.Weather
{
    public class WeatherResults
    {
        public List<WeatherCurrent> CurrentWeathers { get; set; }
        public List<WeatherForecast> Forecasts { get; set; }

        public WeatherResults() { }

        public WeatherResults(int capacity)
        {
            CurrentWeathers = new List<WeatherCurrent>(capacity);
            Forecasts = new List<WeatherForecast>(capacity);
        }
    }
}
