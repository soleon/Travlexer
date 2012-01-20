namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class Response<T> : IResponse<T>
	{
		public StatusCodes Status { get; set; }

		public T Result { get; set; }
	}
}
