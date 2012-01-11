using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class AutoCompleteResponse : ListResponse<Suggestion>
	{
		[JsonProperty(PropertyName = "predictions")]
		new public List<Suggestion> Results
		{
			get { return base.Results; }
			set { base.Results = value; }
		}
	}
}