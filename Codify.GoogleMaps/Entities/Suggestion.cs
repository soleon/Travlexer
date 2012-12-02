using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
    public class Suggestion
    {
        #region Public Properties

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        #endregion
    }
}