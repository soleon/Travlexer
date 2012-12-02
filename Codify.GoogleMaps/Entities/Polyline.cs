using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
    public class Polyline
    {
        [JsonProperty(PropertyName = "points")]
        public string Points { get; set; }

        [JsonProperty(PropertyName = "levels")]
        public string Levels { get; set; }
    }
}