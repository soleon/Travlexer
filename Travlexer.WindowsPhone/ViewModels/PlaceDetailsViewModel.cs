using Codify.ViewModels;
using Travlexer.WindowsPhone.Infrastructure;
using Travlexer.WindowsPhone.Infrastructure.Models;

namespace Travlexer.WindowsPhone.ViewModels
{
	public class PlaceDetailsViewModel : DataViewModel<Place>
	{
		#region Constructor

		public PlaceDetailsViewModel()
		{
			Data = ApplicationContext.Data.SelectedPlace;
		}

		#endregion
	}
}
