using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
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
