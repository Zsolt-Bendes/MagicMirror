using System.Linq;

namespace MagicMirrorWorker.Utilities.Converters
{
    public static class WeatherConverters
    {
        public static Protos.WeatherCurrentModel ConvertToCurrentWeather(this Models.Weather.WeatherCurrent openWeather) => new Protos.WeatherCurrentModel()
        {
            City = openWeather.Name,
            Description = openWeather.Weather.First().Description,
            Humidity = openWeather.Main.Humidity,
            Temp = openWeather.Main.Temp,
            TempMax = openWeather.Main.TempMax,
            TempMin = openWeather.Main.TempMin,
            WeatherId = openWeather.Weather.First().Id,
            Wind = new Protos.Wind
            {
                Deg = openWeather.Wind.Deg,
                Speed = openWeather.Wind.Speed
            },
            Main = openWeather.Weather.First().Main,
            Icon = openWeather.Weather.First().Icon,
            Sunrise = openWeather.Sys.Sunrise,
            Sunset = openWeather.Sys.Sunset
        };

        public static Protos.WeatherForecast ConvertToWeatherForcast(this Models.Weather.WeatherForecast weatherForecast)
        {
            var result = new Protos.WeatherForecast()
            {
                City = new Protos.City()
                {
                    Name = weatherForecast.City.Name,
                    Sunrise = weatherForecast.City.Sunrise,
                    Sunset = weatherForecast.City.Sunset,
                },
            };

            result.Forecasts.AddRange(weatherForecast.List.Select(x => x.ConvertToForecast()));
            return result;
        }

        private static Protos.Forecast ConvertToForecast(this Models.Weather.List list) => new Protos.Forecast()
        {
            Wind = new Protos.Wind()
            {
                Speed = list.Wind.Speed,
                Deg = list.Wind.Deg
            },
            Main = list.Weather.First().Main,
            Description = list.Weather.First().Description,
            Icon = list.Weather.First().Icon,
            Humidity = list.Main.Humidity,
            Temp = list.Main.Temp,
            TempMin = list.Main.TempMin,
            TempMax = list.Main.TempMax,
            WeatherId = list.Weather.First().Id
        };
    }
}
