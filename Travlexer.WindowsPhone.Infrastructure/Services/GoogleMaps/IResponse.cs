namespace Travlexer.WindowsPhone.Infrastructure.Services.GoogleMaps
{
	public interface IResponse<T>
	{
		StatusCodes Status { get; set; }

		T Result { get; set; }
	}
}
