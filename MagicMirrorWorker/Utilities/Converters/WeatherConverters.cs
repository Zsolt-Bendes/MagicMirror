using System.Linq;

namespace MagicMirrorWorker.Utilities.Converters
{
	public static class WeatherConverters
	{
		public static Protos.WeatherModel ConvertToWeatherRespone(this Models.Weather.WeatherCurrent openWeather)
		{
			return new Protos.WeatherModel()
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
				Icon = openWeather.Weather.First().Icon
			};
		}
	}
}
