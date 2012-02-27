using System.Globalization;
using System.Linq;

namespace Codify.GoogleMaps
{
	public class Utilities
	{
		private static readonly string[] _supportedLanguageCodes = new[]
		{
			"ar",
			"eu",
			"bg",
			"bn",
			"ca",
			"cs",
			"da",
			"de",
			"el",
			"en",
			"en-AU",
			"en-GB",
			"es",
			"eu",
			"fa",
			"fi",
			"fil",
			"fr",
			"gl",
			"gu",
			"hi",
			"hr",
			"hu",
			"id",
			"it",
			"iw",
			"ja",
			"kn",
			"ko",
			"lt",
			"lv",
			"ml",
			"mr",
			"nl",
			"no",
			"pl",
			"pt",
			"pt-BR",
			"pt-PT",
			"ro",
			"ru",
			"sk",
			"sl",
			"sr",
			"sv",
			"tl",
			"ta",
			"te",
			"th",
			"tr",
			"uk",
			"vi",
			"zh-CN",
			"zh-TW"
		};

		/// <summary>
		/// Gets the supported language code based on the specified culture information.
		/// </summary>
		/// <returns>A language code that supported by Google Maps' APIs, or the two letter ISO language name of the specified culture if no supported language code is found.</returns>
		private static string GetLanguageCode(CultureInfo cultureInfo)
		{
			var culture = cultureInfo;
			var lng = culture.Name;
			return _supportedLanguageCodes.Contains(lng) ? lng : culture.TwoLetterISOLanguageName;
		}

		/// <summary>
		/// Gets the current language code that is supported by Google Maps' APIs, or the two letter ISO language name of the specified culture if no supported language code is found..
		/// </summary>
		public static string CurrentLanguageCode { get { return _currentLanguageCode ?? (_currentLanguageCode = GetLanguageCode(CultureInfo.CurrentCulture)); } }
		private static string _currentLanguageCode;
	}
}