using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class AutoCompleteResponse : ListResponse<Suggestion>
	{
		[JsonProperty(PropertyName = "predictions")]
		public new List<Suggestion> Result
		{
			get { return base.Result; }
			set { base.Result = value; }
		}
	}
}
