using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Microsoft.Phone.Controls.Maps;
using Travelexer.WindowsPhone.Core.Extensions;
using Travelexer.WindowsPhone.Core.Services;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Services.GoogleMaps;
using Place = Travlexer.WindowsPhone.Models.Place;
using PlaceDetails = Travlexer.WindowsPhone.Models.PlaceDetails;

namespace Travlexer.WindowsPhone
{
	public class DataContext : IDataContext
	{
		#region Private Fields

		private readonly IGoogleMapsClient _googleMapsClient;

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataContext"/> class.
		/// </summary>
		public DataContext(IGoogleMapsClient googleMapsClient = null)
		{
			Places = new ReadOnlyObservableCollection<Place>(_places);

			_googleMapsClient = googleMapsClient ?? new GoogleMapsClient();
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the collection that contains all user pins.
		/// </summary>
		public ReadOnlyObservableCollection<Place> Places { get; private set; }

		private readonly ObservableCollection<Place> _places = new ObservableCollection<Place>();

		#endregion


		#region Public Methods

		/// <summary>
		/// Adds a new place.
		/// </summary>
		/// <param name="location">The location of the place.</param>
		public Place AddNewPlace(Location location)
		{
			var p = new Place(location);
			_places.Add(p);
			return p;
		}

		/// <summary>
		/// Removes the existing place.
		/// </summary>
		/// <param name="place">The place to be removed.</param>
		public void RemovePlace(Place place)
		{
			_places.Remove(place);
		}

		/// <summary>
		/// Removes all places.
		/// </summary>
		public void ClearPlaces()
		{
			_places.Clear();
		}

		/// <summary>
		/// Removes all search results.
		/// </summary>
		public void ClearUnPinnedPlaces()
		{
			for (var i = _places.Count; i >= 0; i--)
			{
				if (!_places[i].IsSearchResult)
				{
					_places.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Gets information of the specified <see cref="Place"/>.
		/// </summary>
		/// <param name="place">The place to get the information for.</param>
		/// <param name="callback">The callback to be executed after this process is finished.</param>
		public void GetPlaceInformation(Place place, Action<CallbackEventArgs> callback = null)
		{
			if (!Globals.IsNetworkAvailable)
			{
				callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.NetworkUnavailable));
				return;
			}

			_googleMapsClient.GetPlaces(place.Location, args =>
			{
				EnumerableResponse<Services.GoogleMaps.PlaceDetails> data;
				IList<Services.GoogleMaps.PlaceDetails> results;
				if (args.StatusCode != HttpStatusCode.OK || (data = args.Data) == null || (results = data.Results) == null || results.Count == 0)
				{
					var exception = args.ErrorException;
					callback.ExecuteIfNotNull(exception != null ? new CallbackEventArgs(CallbackStatus.ServiceException, exception) : new CallbackEventArgs(CallbackStatus.Unknown));
					return;
				}
				var details = results.First();
				if (place.Details == null)
				{
					place.Details = new PlaceDetails { ContactNumber = details.FormattedPhoneNumber };
				}
				else
				{
					place.Details.ContactNumber = details.FormattedPhoneNumber;
				}
				place.FormattedAddress = details.FormattedAddress;
				callback.ExecuteIfNotNull(new CallbackEventArgs());
			});
		}

		#endregion


		#region Private Methods

		#endregion
	}
}
