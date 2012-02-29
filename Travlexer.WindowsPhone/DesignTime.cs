using System.ComponentModel;
using System.Device.Location;
using Codify.GoogleMaps;
using Microsoft.Phone.Controls.Maps;
using Travlexer.WindowsPhone.Infrastructure;
using Travlexer.WindowsPhone.Infrastructure.Models;
using Travlexer.WindowsPhone.ViewModels;
using Place = Travlexer.WindowsPhone.Infrastructure.Models.Place;

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
				DataContext.GoogleMapsClient = new GoogleMapsClientMock();
				return _userPin ?? (_userPin = DataContext.AddNewPlace(new Location { Latitude = 9.1540930, Longitude = -1.39166990 }));
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
					DataContext.GoogleMapsClient = new GoogleMapsClientMock();
					DataContext.AddNewPlace(new Location { Latitude = 9.1540930, Longitude = -1.39166990 });
					_mapViewModel = new MapViewModel();
				}
				return _mapViewModel;
			}
		}

		private MapViewModel _mapViewModel;

		public LocationCollection SampleLocations
		{
			get
			{
				return new LocationCollection
				{
					new GeoCoordinate(0,0),
					new GeoCoordinate(0, 60),
					new GeoCoordinate(60,-30),
					new GeoCoordinate(60,0)
				};
			}
		}

		public ElementColor SampleColor
		{
			get;
			set;
		}
#endif
	}
}
