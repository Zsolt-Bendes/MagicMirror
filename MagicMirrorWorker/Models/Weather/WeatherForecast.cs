using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MagicMirrorWorker.Models.Weather
{
    public class WeatherForecast
	{
        [JsonPropertyName("cod")]
        public long Cod { get; set; }

        [JsonPropertyName("message")]
        public long Message { get; set; }

        [JsonPropertyName("cnt")]
        public long Cnt { get; set; }

        [JsonPropertyName("list")]
        public List<List> List { get; set; }

        [JsonPropertyName("city")]
        public City City { get; set; }
    }

    public class City
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("coord")]
        public Coord Coord { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("population")]
        public long Population { get; set; }

        [JsonPropertyName("timezone")]
        public long Timezone { get; set; }

        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; }
    }

    public class List
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("main")]
        public MainClass Main { get; set; }

        [JsonPropertyName("weather")]
        public List<Weather> Weather { get; set; }

        [JsonPropertyName("clouds")]
        public Clouds Clouds { get; set; }

        [JsonPropertyName("wind")]
        public Wind Wind { get; set; }

        [JsonPropertyName("rain")]
        public Rain Rain { get; set; }

        [JsonPropertyName("dt_txt")]
        public DateTimeOffset DtTxt { get; set; }
    }

    public class MainClass
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("temp_min")]
        public double TempMin { get; set; }

        [JsonPropertyName("temp_max")]
        public double TempMax { get; set; }

        [JsonPropertyName("pressure")]
        public long Pressure { get; set; }

        [JsonPropertyName("sea_level")]
        public long SeaLevel { get; set; }

        [JsonPropertyName("grnd_level")]
        public long GrndLevel { get; set; }

        [JsonPropertyName("humidity")]
        public long Humidity { get; set; }

        [JsonPropertyName("temp_kf")]
        public double TempKf { get; set; }
    }
}
