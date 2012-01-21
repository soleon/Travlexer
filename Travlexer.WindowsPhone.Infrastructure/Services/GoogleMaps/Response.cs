namespace Travlexer.WindowsPhone.Infrastructure.Services.GoogleMaps
{
	public class Response<T> : IResponse<T>
	{
		public StatusCodes Status { get; set; }

		public T Result { get; set; }
	}
}
