using System.ComponentModel;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Services.GoogleMaps;
using Travlexer.WindowsPhone.ViewModels;
using Place = Travlexer.WindowsPhone.Models.Place;

namespace Travlexer.WindowsPhone
{
	public class DesignTime
	{
#if DEBUG
		public Place UserPin
		{
			get
			{
				if (!DesignerProperties.IsInDesignTool)
				{
					return null;
				}
				var data = Globals.DataContext = new DataContext(new GoogleMapsClientMock());
				return _userPin ?? (_userPin = data.AddNewPlace(new Location(9.1540930, -1.39166990)));
			}
		}

		private Place _userPin;

		public MapViewModel MapViewModel
		{
			get
			{
				if (!DesignerProperties.IsInDesignTool)
				{
					return null;
				}
				if (_mapViewModel == null)
				{
					var data = Globals.DataContext = new DataContext(new GoogleMapsClientMock());
					data.AddNewPlace(new Location(9.1540930, -1.39166990));
					_mapViewModel = new MapViewModel();
				}
				return _mapViewModel;
			}
		}

		private MapViewModel _mapViewModel;
#endif
	}
}
