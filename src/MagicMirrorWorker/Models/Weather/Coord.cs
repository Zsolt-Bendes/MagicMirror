using System.Text.Json.Serialization;

namespace MagicMirrorWorker.Models.Weather
{
    public class Coord
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }
}
