using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
    public class Place
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "vicinity")]
        public string Vicinity { get; set; }

        [JsonProperty(PropertyName = "types")]
        public IList<string> Types { get; set; }

        [JsonProperty(PropertyName = "geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public string Reference { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "formatted_phone_number")]
        public string FormattedPhoneNumber { get; set; }

        [JsonProperty(PropertyName = "international_phone_number")]
        public string InternationalPhoneNumber { get; set; }

        [JsonProperty(PropertyName = "formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty(PropertyName = "address_components")]
        public IList<AddressComponent> AddressComponents { get; set; }

        [JsonProperty(PropertyName = "rating")]
        public string Raiting { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "utc_offset")]
        public int UtcOffset { get; set; }

        [JsonProperty(PropertyName = "website")]
        public string WebSite { get; set; }
    }
}