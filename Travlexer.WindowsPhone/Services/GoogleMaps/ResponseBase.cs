namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public abstract class ResponseBase : IResponse
	{
		public StatusCodes Status { get; set; }
	}
}
