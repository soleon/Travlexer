using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using Travlexer.WindowsPhone.Core.Extensions;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public interface IGoogleMapsClient
	{
		/// <summary>
		/// Gets a list of <see cref="PlaceDetails"/> that can be found at the specified <see cref="LatLng"/>.
		/// </summary>
		void GetPlaces(LatLng location, Action<RestResponse<EnumerableResponse<PlaceDetails>>> callback);

		/// <summary>
		/// Gets the suggestions based on the input.
		/// </summary>
		/// <param name="center">The geo-coordinate of where the suggestions will be based on.</param>
		/// <param name="input">The input to populate the suggestions.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		void GetSuggestions(LatLng center, string input, Action<RestResponse<AutoCompleteResponse>> callback = null);

		/// <summary>
		/// Searches for places that matches the input.
		/// </summary>
		/// <param name="baseLocation">The geo-coordinate around which to retrieve place information.</param>
		/// <param name="input">The input to search places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		void Search(LatLng baseLocation, string input, Action<RestResponse<EnumerableResponse<Place>>> callback = null);
	}

	public class GoogleMapsClient : IGoogleMapsClient
	{
		#region Private Members

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


		private const string UnitImperial = "imperial";
		private const string UnitMetric = "metric";
		private const string BaseApiUrl = "https://maps.googleapis.com/maps/api";
		private const string ApiKey = "AIzaSyBEPZ46IhE3WI-7mBtSOkixMr6GSwRgrws";
		private readonly string _language;
		private readonly string _region;
		private readonly string _basePlacesSearchUrl;
		private readonly string _basePlaceDetailsUrl;
		private readonly string _baseGeocodingUrl;
		private readonly string _baseDirectionsUrl;
		private readonly string _baseAutoCompleteUrl;
		private readonly string _baseStaticMapUrl;

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GoogleMapsClient"/> class.
		/// </summary>
		public GoogleMapsClient()
		{
			_language = GetLanguageCode(CultureInfo.CurrentCulture);

			_region = RegionInfo.CurrentRegion.TwoLetterISORegionName;
			_basePlacesSearchUrl = "place/search/json?sensor=true&radius=5000&key=" + ApiKey + "&language=" + _language;
			_basePlaceDetailsUrl = "place/details/json?sensor=true&key=" + ApiKey + "&language=" + _language;
			_baseGeocodingUrl = "geocode/json?sensor=true&language=" + _language + "&region=" + _region;
			_baseDirectionsUrl = "directions/json?sensor=true&language=" + _language + "&region=" + _region;
			_baseAutoCompleteUrl = "place/autocomplete/json?sensor=true&key=" + ApiKey + "&language=" + _language;
			_baseStaticMapUrl = "staticmap?sensor=true";
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Gets a list of <see cref="PlaceDetails"/> that can be found at the specified <see cref="LatLng"/>.
		/// </summary>
		public void GetPlaces(LatLng location, Action<RestResponse<EnumerableResponse<PlaceDetails>>> callback = null)
		{
			var c = new RestClient(BaseApiUrl);
			c.ExecuteAsync<EnumerableResponse<PlaceDetails>>(
				new RestRequest(_baseGeocodingUrl + "&latlng=" + location),
				response =>
				{
					if (response != null && response.StatusCode == HttpStatusCode.OK && response.Data == null)
					{
						try
						{
							response.Data = JsonConvert.DeserializeObject<EnumerableResponse<PlaceDetails>>(response.Content);
						}
						catch { }
					}
					callback.ExecuteIfNotNull(response);
				});
		}

		/// <summary>
		/// Gets the suggestions based on the input.
		/// </summary>
		/// <param name="center">The geo-coordinate of where the suggestions will be based on.</param>
		/// <param name="input">The input to populate the suggestions.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		public void GetSuggestions(LatLng center, string input, Action<RestResponse<AutoCompleteResponse>> callback = null)
		{
			var c = new RestClient(BaseApiUrl);
			c.ExecuteAsync<AutoCompleteResponse>(
				new RestRequest(_baseAutoCompleteUrl + "&location=" + center + "&input=" + input),
				response =>
				{
					if (response != null && response.StatusCode == HttpStatusCode.OK && response.Data == null)
					{
						try
						{
							response.Data = JsonConvert.DeserializeObject<AutoCompleteResponse>(response.Content);
						}
						catch { }
					}

					callback.ExecuteIfNotNull(response);
				});
		}

		/// <summary>
		/// Searches for places that matches the input.
		/// </summary>
		/// <param name="baseLocation">The geo-coordinate around which to retrieve place information.</param>
		/// <param name="input">The input to search places.</param>
		/// <param name="callback">The callback to execute after the process is finished.</param>
		public void Search(LatLng baseLocation, string input, Action<RestResponse<EnumerableResponse<Place>>> callback = null)
		{
			var c = new RestClient(BaseApiUrl);
			c.ExecuteAsync<EnumerableResponse<Place>>(
				new RestRequest(_basePlacesSearchUrl + "&location=" + baseLocation + (input == null ? null : "&keyword=" + HttpUtility.UrlEncode(input))),
				response =>
				{
					if (response != null && response.StatusCode == HttpStatusCode.OK && response.Data == null)
					{
						try
						{
							response.Data = JsonConvert.DeserializeObject<EnumerableResponse<Place>>(response.Content);
						}
						catch { }
					}
					callback.ExecuteIfNotNull(response);
				});
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Gets the supported language code based on the specified culture information.
		/// </summary>
		/// <returns>A language code that supported by Google Maps' APIs, or the two letter ISO language name of the specified culture if no supported language code is found.</returns>
		public string GetLanguageCode(CultureInfo cultureInfo)
		{
			var culture = CultureInfo.CurrentCulture;
			var lng = culture.Name;
			return _supportedLanguageCodes.Contains(lng) ? lng : culture.TwoLetterISOLanguageName;
		}

		#endregion
	}

	public class GoogleMapsClientMock : IGoogleMapsClient
	{
		#region Constants

		private const string GeocodeResponse = @"
{
   'results' : [
	  {
		 'address_components' : [
			{
			   'long_name' : 'Fryfusu-Damongo Rd',
			   'short_name' : 'Fryfusu-Damongo Rd',
			   'types' : [ 'route' ]
			},
			{
			   'long_name' : 'Central Gonja',
			   'short_name' : 'Central Gonja',
			   'types' : [ 'administrative_area_level_2', 'political' ]
			},
			{
			   'long_name' : 'Northern',
			   'short_name' : 'Northern',
			   'types' : [ 'administrative_area_level_1', 'political' ]
			},
			{
			   'long_name' : 'Ghana',
			   'short_name' : 'GH',
			   'types' : [ 'country', 'political' ]
			}
		 ],
		 'formatted_address' : 'Fryfusu-Damongo Rd, Ghana',
		 'geometry' : {
			'bounds' : {
			   'northeast' : {
				  'lat' : 9.1540930,
				  'lng' : -1.39166990
			   },
			   'southwest' : {
				  'lat' : 9.151451399999999,
				  'lng' : -1.39836190
			   }
			},
			'location' : {
			   'lat' : 9.152476699999999,
			   'lng' : -1.39513080
			},
			'location_type' : 'APPROXIMATE',
			'viewport' : {
			   'northeast' : {
				  'lat' : 9.154121180291501,
				  'lng' : -1.39166990
			   },
			   'southwest' : {
				  'lat' : 9.151423219708498,
				  'lng' : -1.39836190
			   }
			}
		 },
		 'types' : [ 'route' ]
	  },
	  {
		 'address_components' : [
			{
			   'long_name' : 'Central Gonja',
			   'short_name' : 'Central Gonja',
			   'types' : [ 'administrative_area_level_2', 'political' ]
			},
			{
			   'long_name' : 'Northern',
			   'short_name' : 'Northern',
			   'types' : [ 'administrative_area_level_1', 'political' ]
			},
			{
			   'long_name' : 'Ghana',
			   'short_name' : 'GH',
			   'types' : [ 'country', 'political' ]
			}
		 ],
		 'formatted_address' : 'Central Gonja, Ghana',
		 'geometry' : {
			'bounds' : {
			   'northeast' : {
				  'lat' : 9.331186499999999,
				  'lng' : -0.68870540
			   },
			   'southwest' : {
				  'lat' : 8.54469520,
				  'lng' : -2.0977020
			   }
			},
			'location' : {
			   'lat' : 9.01137950,
			   'lng' : -1.05861350
			},
			'location_type' : 'APPROXIMATE',
			'viewport' : {
			   'northeast' : {
				  'lat' : 9.331186499999999,
				  'lng' : -0.68870540
			   },
			   'southwest' : {
				  'lat' : 8.54469520,
				  'lng' : -2.0977020
			   }
			}
		 },
		 'types' : [ 'administrative_area_level_2', 'political' ]
	  },
	  {
		 'address_components' : [
			{
			   'long_name' : 'Northern',
			   'short_name' : 'Northern',
			   'types' : [ 'administrative_area_level_1', 'political' ]
			},
			{
			   'long_name' : 'Ghana',
			   'short_name' : 'GH',
			   'types' : [ 'country', 'political' ]
			}
		 ],
		 'formatted_address' : 'Northern, Ghana',
		 'geometry' : {
			'bounds' : {
			   'northeast' : {
				  'lat' : 10.7145930,
				  'lng' : 0.563380
			   },
			   'southwest' : {
				  'lat' : 7.954847000000001,
				  'lng' : -2.7796660
			   }
			},
			'location' : {
			   'lat' : 9.543926899999999,
			   'lng' : -0.90566230
			},
			'location_type' : 'APPROXIMATE',
			'viewport' : {
			   'northeast' : {
				  'lat' : 10.7145930,
				  'lng' : 0.563380
			   },
			   'southwest' : {
				  'lat' : 7.954847000000001,
				  'lng' : -2.7796660
			   }
			}
		 },
		 'types' : [ 'administrative_area_level_1', 'political' ]
	  },
	  {
		 'address_components' : [
			{
			   'long_name' : 'Ghana',
			   'short_name' : 'GH',
			   'types' : [ 'country', 'political' ]
			}
		 ],
		 'formatted_address' : 'Ghana',
		 'geometry' : {
			'bounds' : {
			   'northeast' : {
				  'lat' : 11.16666750,
				  'lng' : 1.1995540
			   },
			   'southwest' : {
				  'lat' : 4.73929610,
				  'lng' : -3.24916690
			   }
			},
			'location' : {
			   'lat' : 7.9465270,
			   'lng' : -1.0231940
			},
			'location_type' : 'APPROXIMATE',
			'viewport' : {
			   'northeast' : {
				  'lat' : 11.16666750,
				  'lng' : 1.1995540
			   },
			   'southwest' : {
				  'lat' : 4.73929610,
				  'lng' : -3.24916690
			   }
			}
		 },
		 'types' : [ 'country', 'political' ]
	  }
   ],
   'status' : 'OK'
}
";

		#endregion


		#region Public Methods

		public void GetPlaces(LatLng location, Action<RestResponse<EnumerableResponse<PlaceDetails>>> callback)
		{
			var result = new RestResponse<EnumerableResponse<PlaceDetails>>
			{
				StatusCode = HttpStatusCode.OK,
				Data = JsonConvert.DeserializeObject<EnumerableResponse<PlaceDetails>>(GeocodeResponse)
			};
			callback(result);
		}

		public void GetSuggestions(LatLng center, string input, Action<RestResponse<AutoCompleteResponse>> callback = null)
		{
			throw new NotImplementedException();
		}

		public void Search(LatLng baseLocation, string input, Action<RestResponse<EnumerableResponse<Place>>> callback = null)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
