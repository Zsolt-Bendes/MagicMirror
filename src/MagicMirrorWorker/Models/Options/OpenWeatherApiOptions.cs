namespace MagicMirrorWorker.Models.Options
{
    public class OpenWeatherApiOptions
    {
        public const string Position = "OpenWeather";

        public string Url { get; set; }
        public string AppKey { get; set; }
        public string Unit { get; set; }
        public WeatherApiLanguage Language { get; set; }
    }
}
