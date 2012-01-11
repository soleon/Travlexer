using System.Collections.Generic;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class ListResponse<T> : ResponseBase, IListResponse<T>
	{
		public virtual List<T> Results { get; set; }
	}
}
