using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicMirrorWorker.Models.Weather
{
    public class WeatherResults
    {
        public WeatherCurrent[] CurrentWeathers { get; set; }
        public WeatherForecast[] Forecasts { get; set; }
    }
}
