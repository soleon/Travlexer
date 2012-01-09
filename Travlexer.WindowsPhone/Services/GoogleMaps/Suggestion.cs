using Newtonsoft.Json;
using Travlexer.WindowsPhone.Models;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class Suggestion
	{
		#region Public Properties

		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }

		[JsonProperty(PropertyName = "reference")]
		public string Reference { get; set; }

		#endregion


		#region Operators

		public static implicit operator SearchSuggestion(Suggestion suggestion)
		{
			if (suggestion == null)
			{
				return null;
			}
			return new SearchSuggestion
			{
				Description = suggestion.Description,
				Reference = suggestion.Reference
			};
		}

		#endregion
	}
}
