using System.Text.Json.Serialization;

namespace MagicMirrorWorker.Models.Weather
{
	public class Clouds
    {
        [JsonPropertyName("all")]
        public long All { get; set; }
    }
}
