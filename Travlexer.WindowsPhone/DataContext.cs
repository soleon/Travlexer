using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Travelexer.WindowsPhone.Core.Extensions;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Services;

namespace Travlexer.WindowsPhone
{
	public class DataContext : IDataContext
	{
		#region Private Fields

		private readonly Services.GoogleMaps.IGoogleMapsClient _googleMapsClient;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataContext"/> class.
		/// </summary>
		public DataContext(Services.GoogleMaps.IGoogleMapsClient googleMapsClient = null)
		{
			Places = new ReadOnlyObservableCollection<Place>(_places);

			_googleMapsClient = googleMapsClient ?? new Services.GoogleMaps.GoogleMapsClient();
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
		/// <param name="icon">The icon of the place.</param>
		/// <param name="callback">The action to execute after the process is finished.</param>
		public Place AddNewPlace(Location location, PlaceIcon icon = default(PlaceIcon), Action<CallbackEventArgs> callback = null)
		{
			var p = new Place(location) { Icon = icon };
			_places.Add(p);
			GetPlaceDetails(p, callback);
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
		/// Gets a list of <see cref="PlaceDetails"/> for the specified location.
		/// </summary>
		/// <param name="place"></param>
		/// <param name="callback">The callback to be executed after this process is finished.</param>
		public void GetPlaceDetails(Place place, Action<CallbackEventArgs> callback = null)
		{
			if (!Globals.IsNetworkAvailable)
			{
				callback.ExecuteIfNotNull(new CallbackEventArgs(CallbackStatus.NetworkUnavailable));
				return;
			}

			_googleMapsClient.GetPlaceDetails(place.Location, args =>
			{
				if (args.StatusCode != HttpStatusCode.OK)
				{
					var exception = args.ErrorException;
					callback.ExecuteIfNotNull(exception != null ? new CallbackEventArgs(CallbackStatus.ServiceException, exception) : new CallbackEventArgs(CallbackStatus.Unknown));
					return;
				}

				var details = args.Data.Results.First();
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