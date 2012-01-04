using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class Response<T> : ResponseBase
	{
		[JsonProperty(PropertyName = "result")]
		public T Result { get; set; }
	}
}