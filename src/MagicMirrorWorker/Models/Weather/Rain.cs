using System.Text.Json.Serialization;

namespace MagicMirrorWorker.Models.Weather
{
    public class Rain
    {
        [JsonPropertyName("3h")]
        public double The3H { get; set; }
    }
}
