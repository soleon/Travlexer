using System.Collections.ObjectModel;
using Travlexer.WindowsPhone.Infrustructure.Entities;

namespace Travlexer.WindowsPhone.Infrustructure
{
	public class DataContext : IDataContext
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataContext"/> class.
		/// </summary>
		public DataContext()
		{
			Pins = new ReadOnlyObservableCollection<UserPin>(_pins);
			SearchResults = new ReadOnlyObservableCollection<SearchResult>(_searchResults);
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the user pins.
		/// </summary>
		public ReadOnlyObservableCollection<UserPin> Pins { get; private set; }

		private readonly ObservableCollection<UserPin> _pins = new ObservableCollection<UserPin>();

		/// <summary>
		/// Gets the search results.
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
		public void AddNewPin(PlaceIcon icon, Location location)
		{
			_pins.Add(new UserPin(location));
		}

		/// <summary>
		/// Removes the existing user pin.
		/// </summary>
		/// <param name="UserPin">The pin.</param>
		public void RemovePin(UserPin UserPin)
		{
			_pins.Remove(UserPin);
		}

		/// <summary>
		/// Removes all user pins.
		/// </summary>
		public void ClearUserPins()
		{
			_pins.Clear();
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

		#endregion
	}
}