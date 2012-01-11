using System.Collections.Generic;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public interface IListResponse<T>
	{
		new List<T> Results { get; set; }
	}
}