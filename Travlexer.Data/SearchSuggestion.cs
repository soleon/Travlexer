namespace Travlexer.Data
{
	public class SearchSuggestion
	{
		public string Description { get; set; }

		public string Reference { get; set; }

        //public static implicit operator SearchSuggestion(Suggestion suggestion)
        //{
        //    if (suggestion == null)
        //    {
        //        return null;
        //    }
        //    return new SearchSuggestion
        //    {
        //        Description = suggestion.Description,
        //        Reference = suggestion.Reference
        //    };
        //}
	}
}
