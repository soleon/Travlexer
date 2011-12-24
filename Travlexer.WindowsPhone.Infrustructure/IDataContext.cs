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
		void AddNewPin(PlaceIcon icon, Location location);

		/// <summary>
		/// Removes the existing user pin.
		/// </summary>
		/// <param name="pin">The pin.</param>
		void RemovePin(UserPin pin);

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
}
