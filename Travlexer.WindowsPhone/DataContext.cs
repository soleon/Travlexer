using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using Travlexer.WindowsPhone.Core.Extensions;
using Travlexer.WindowsPhone.Core.Services;
using Travlexer.WindowsPhone.Core.Threading;
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
			for (var i = _places.Count - 1; i >= 0; i--)
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

			_googleMapsClient.GetPlaces(place.Location, response =>
			{
				EnumerableResponse<Services.GoogleMaps.PlaceDetails> data;
				IList<Services.GoogleMaps.PlaceDetails> results;
				if (response.StatusCode != HttpStatusCode.OK || (data = response.Data) == null || (results = data.Results) == null || results.Count == 0)
				{
					var exception = response.ErrorException;
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

		/// <summary>
		/// Searches for places that matches the input.
		/// </summary>
		/// <param name="baseLocation">The geo-coordinate around which to retrieve place information.</param>
		/// <param name="input">The input to search places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		public void Search(Location baseLocation, string input, Action<CallbackEventArgs<IList<Place>>> callback = null)
		{
			if (!Globals.IsNetworkAvailable)
			{
				callback.ExecuteIfNotNull(new CallbackEventArgs<IList<Place>>(CallbackStatus.NetworkUnavailable));
				return;
			}

			_googleMapsClient.Search(baseLocation, input, response =>
			{
				EnumerableResponse<Services.GoogleMaps.Place> data;
				IList<Services.GoogleMaps.Place> results;
				if (response.StatusCode != HttpStatusCode.OK || (data = response.Data) == null || (results = data.Results) == null || results.Count == 0)
				{
					var exception = response.ErrorException;
					callback.ExecuteIfNotNull(exception != null ? new CallbackEventArgs<IList<Place>>(CallbackStatus.ServiceException, exception) : new CallbackEventArgs<IList<Place>>(CallbackStatus.Unknown));
					return;
				}


				ClearUnPinnedPlaces();
				var places = results.Take(10).Select(p => (Place)p).ToList();

				UIThread.RunWorker(() =>
				{
					var lastIndex = places.Count - 1;
					for (var i = 0; i <= lastIndex; i++)
					{
						var place = places[i];
						UIThread.InvokeAsync(() => _places.Add(place));
						if (i == lastIndex)
						{
							break;
						}
						Thread.Sleep(100);
					}
				});
				callback.ExecuteIfNotNull(new CallbackEventArgs<IList<Place>>(places));
			});
		}

		#endregion


		#region Private Methods

		#endregion
	}
}
