using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
    public struct Distance
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
    }
}