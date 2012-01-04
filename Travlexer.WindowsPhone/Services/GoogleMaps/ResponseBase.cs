using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public abstract class ResponseBase
	{
		[JsonProperty(PropertyName = "html_attributions")]
		public IList<string> HtmlAttributions { get; set; }

		[JsonProperty(PropertyName = "status")]
		public StatusCodes Status { get; set; }
	}
}