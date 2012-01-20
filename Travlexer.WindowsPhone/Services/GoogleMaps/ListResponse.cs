using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class ListResponse<T> : Response<List<T>>
	{
		[JsonProperty("results")]
		public new List<T> Result
		{
			get { return base.Result; }
			set { base.Result = value; }
		}
	}
}
