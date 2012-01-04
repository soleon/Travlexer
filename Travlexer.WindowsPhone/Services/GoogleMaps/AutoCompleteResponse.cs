using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class AutoCompleteResponse
	{
		[JsonProperty(PropertyName = "predictions")]
		public IList<Suggestion> Suggestions { get; set; }
	}
}