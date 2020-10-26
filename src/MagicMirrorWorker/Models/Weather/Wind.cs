using System.Text.Json.Serialization;

namespace MagicMirrorWorker.Models.Weather
{
    public class Wind
    {
        [JsonPropertyName("speed")]
        public float Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Deg { get; set; }
    }
}
