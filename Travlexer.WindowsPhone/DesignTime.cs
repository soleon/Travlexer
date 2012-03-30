﻿using System.ComponentModel;
using System.Device.Location;
using Codify.GoogleMaps;
using Microsoft.Phone.Controls.Maps;
using Travlexer.WindowsPhone.Infrastructure.Models;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone
{
	public class DesignTime
	{
#if DEBUG
		private bool _initialized;

		public MapViewModel MapViewModel
		{
			get
			{
				if (!DesignerProperties.IsInDesignTool)
				{
					return null;
				}
				Initialize();
				return new MapViewModel();
			}
		}

		public LocationCollection SampleLocations
		{
			get
			{
				return new LocationCollection
				{
					new GeoCoordinate(0, 0),
					new GeoCoordinate(0, 60),
					new GeoCoordinate(60, -30),
					new GeoCoordinate(60, 0)
				};
			}
		}

		public ElementColor SampleColor { get; set; }

		public PlaceDetailsViewModel PlaceDetailsViewModel
		{
			get
			{
				if (!DesignerProperties.IsInDesignTool)
				{
					return null;
				}
				Initialize();
				var vm = new PlaceDetailsViewModel { Data = ApplicationContext.Data.Places[0] };
				vm.Data.Notes = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
				return vm;
			}
		}

		private void Initialize()
		{
			if (_initialized)
			{
				return;
			}
			ApplicationContext.Data.GoogleMapsClient = new GoogleMapsClientMock();
			ApplicationContext.Data.AddNewPlace(new Location { Latitude = 9.1540930, Longitude = -1.39166990 });
			_initialized = true;
		}
#endif
	}
}
