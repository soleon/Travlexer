using System.Collections.ObjectModel;
using Travlexer.WindowsPhone.Infrustructure.Entities;

namespace Travlexer.WindowsPhone.Infrustructure
{
	/// <summary>
	/// Defines the only data contract for external sources to access and manipulate data.
	/// </summary>
	public interface IDataContext
	{
		/// <summary>
		/// Adds a new user pin.
		/// </summary>
		/// <param name="icon">The icon of the user pin.</param>
		/// <param name="location">The location of the user pin.</param>
		void AddNewPin(PinIcon icon, Location location);

		/// <summary>
		/// Removes the existing user pin.
		/// </summary>
		/// <param name="pin">The pin.</param>
		void RemovePin(Pin pin);

		/// <summary>
		/// Removes all user pins.
		/// </summary>
		void ClearUserPins();

		/// <summary>
		/// Removes the search result.
		/// </summary>
		/// <param name="result">The result to be removed.</param>
		void RemoveSearchResult(SearchResult result);

		/// <summary>
		/// Removes all search results.
		/// </summary>
		void ClearSearchResults();
	}

	public class DataContext : IDataContext
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataContext"/> class.
		/// </summary>
		public DataContext()
		{
			Pins = new ReadOnlyObservableCollection<Pin>(_pins);
			SearchResults = new ReadOnlyObservableCollection<SearchResult>(_searchResults);
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the user pins.
		/// </summary>
		public ReadOnlyObservableCollection<Pin> Pins { get; private set; }

		private readonly ObservableCollection<Pin> _pins = new ObservableCollection<Pin>();

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
		public void AddNewPin(PinIcon icon, Location location)
		{
			_pins.Add(new Pin(location));
		}

		/// <summary>
		/// Removes the existing user pin.
		/// </summary>
		/// <param name="pin">The pin.</param>
		public void RemovePin(Pin pin)
		{
			_pins.Remove(pin);
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
