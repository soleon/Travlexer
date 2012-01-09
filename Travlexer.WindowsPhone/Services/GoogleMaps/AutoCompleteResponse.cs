using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class AutoCompleteResponse
	{
		[JsonProperty(PropertyName = "predictions")]
		public List<Suggestion> Suggestions { get; set; }
	}
}