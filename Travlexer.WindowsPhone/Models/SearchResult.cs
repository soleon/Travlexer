namespace Travlexer.WindowsPhone.Models
{
	/// <summary>
	/// Represents a search result pin on the map.
	/// </summary>
	public class SearchResult : Place
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResult"/> class.
		/// </summary>
		/// <param name="location">The geographical location of this search result.</param>
		public SearchResult(Location location) : base(location) { }
	}
}