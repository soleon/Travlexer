using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Services;

namespace Travlexer.WindowsPhone
{
	public class DataContext : IDataContext
	{
		#region Private Fields

		private Services.GoogleMaps.IGoogleMapsClient _googleMapsClient;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataContext"/> class.
		/// </summary>
		public DataContext(Services.GoogleMaps.IGoogleMapsClient googleMapsClient = null)
		{
			UserPins = new ReadOnlyObservableCollection<UserPin>(_userPins);
			SearchResults = new ReadOnlyObservableCollection<SearchResult>(_searchResults);

			_googleMapsClient = googleMapsClient ?? new Services.GoogleMaps.GoogleMapsClient();
		}

		#endregion


		#region Public Properties


		/// <summary>
		/// Gets the collection that contains all user pins.
		/// </summary>
		public ReadOnlyObservableCollection<UserPin> UserPins { get; private set; }

		private readonly ObservableCollection<UserPin> _userPins = new ObservableCollection<UserPin>();

		/// <summary>
		/// Gets the collection that contains all search results.
		/// </summary>
		public ReadOnlyObservableCollection<SearchResult> SearchResults { get; private set; }

		private readonly ObservableCollection<SearchResult> _searchResults = new ObservableCollection<SearchResult>();

		#endregion


		#region Public Methods

		/// <summary>
		/// Adds a new user pin.
		/// </summary>
		/// <param name="icon">The icon of the user pin.</param>
		/// <param name="location">The location of the user pin.</param>
		public void AddNewUserPin(Location location, PlaceIcon icon = default(PlaceIcon))
		{
			var pin = new UserPin(location) { Icon = icon };
			_userPins.Add(pin);
			GetPlaceDetails(pin);
		}

		/// <summary>
		/// Removes the existing user pin.
		/// </summary>
		/// <param name="userPin">The pin.</param>
		public void RemovePin(UserPin userPin)
		{
			_userPins.Remove(userPin);
		}

		/// <summary>
		/// Removes all user pins.
		/// </summary>
		public void ClearUserPins()
		{
			_userPins.Clear();
		}

		/// <summary>
		/// Removes the search result.
		/// </summary>
		/// <param name="result">The result to be removed.</param>
		public void RemoveSearchResult(SearchResult result)
		{
			_searchResults.Remove(result);
		}

		/// <summary>
		/// Removes all search results.
		/// </summary>
		public void ClearSearchResults()
		{
			_searchResults.Clear();
		}

		/// <summary>
		/// Gets a list of <see cref="PlaceDetails"/> for the specified location.
		/// </summary>
		/// <param name="place"></param>
		/// <param name="callback">The callback to be executed after this process is finished.</param>
		public void GetPlaceDetails(Place place, Action<CallbackEventArgs> callback = null)
		{
			_googleMapsClient.GetPlaceDetails(place.Location, args =>
			{
				// TODO: handle error properly. Check the HTTP status.
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
			});
		}

		#endregion

		#region Private Methods

		

		#endregion
	}
}