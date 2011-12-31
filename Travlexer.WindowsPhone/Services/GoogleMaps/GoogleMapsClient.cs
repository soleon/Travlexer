using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.Phone.Controls.Maps;
using Newtonsoft.Json;
using RestSharp;
using Travlexer.WindowsPhone.Models;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public interface IGoogleMapsClient
	{
		/// <summary>
		/// Gets a list of <see cref="PlaceDetails"/> that can be found at the specified <see cref="LatLng"/>.
		/// </summary>
		void GetPlaces(LatLng location, ViewPort bounds, Action<RestResponse<EnumerableResponse<PlaceDetails>>> callback);
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
			_basePlacesSearchUrl = "place/search/json?sensor=true&radius=500&key=" + ApiKey + "&language=" + _language;
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
		public void GetPlaces(LatLng location, ViewPort bounds, Action<RestResponse<EnumerableResponse<PlaceDetails>>> callback)
		{
			var c = new RestClient(BaseApiUrl);
			c.ExecuteAsync<EnumerableResponse<PlaceDetails>>(
				new RestRequest(_baseGeocodingUrl + "&latlng=" + location + (bounds == null ? null : "&bounds=" + bounds)),
				response =>
				{
					try
					{
						response.Data = JsonConvert.DeserializeObject<EnumerableResponse<PlaceDetails>>(response.Content);
					}
					catch {}
					callback(response);
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

		public void GetPlaces(LatLng location, ViewPort bounds, Action<RestResponse<EnumerableResponse<PlaceDetails>>> callback)
		{
			var result = new RestResponse<EnumerableResponse<PlaceDetails>>
			{
				StatusCode = HttpStatusCode.OK,
				Data = JsonConvert.DeserializeObject<EnumerableResponse<PlaceDetails>>(GeocodeResponse)
			};
			callback(result);
		}

		#endregion
	}

	public class AddressComponent
	{
		[JsonProperty(PropertyName = "long_name")]
		public string LongName { get; set; }

		[JsonProperty(PropertyName = "short_name")]
		public string ShortName { get; set; }

		[JsonProperty(PropertyName = "types")]
		public IEnumerable<string> Types { get; set; }
	}

	public class AutoCompleteResponse
	{
		[JsonProperty(PropertyName = "predictions")]
		public IEnumerable<Suggestion> Suggestions { get; set; }
	}

	public struct Distance
	{
		[JsonProperty(PropertyName = "text")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "value")]
		public int Value { get; set; }
	}

	public struct Duration
	{
		[JsonProperty(PropertyName = "text")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "value")]
		public int Value { get; set; }
	}

	public class EnumerableResponse<T> : ResponseBase<T>
	{
		[JsonProperty(PropertyName = "results")]
		public virtual IEnumerable<T> Results { get; set; }
	}

	public class RoutesResponse : EnumerableResponse<Route>
	{
		[JsonProperty(PropertyName = "routes")]
		public override IEnumerable<Route> Results
		{
			get { return base.Results; }
			set { base.Results = value; }
		}
	}

	public class Geometry
	{
		public enum LocationTypes
		{
			APPROXIMATE,
			GEOMETRIC_CENTER,
			RANGE_INTERPOLATED,
			ROOFTOP
		}

		[JsonProperty(PropertyName = "location")]
		public LatLng Location { get; set; }

		[JsonProperty(PropertyName = "viewport")]
		public ViewPort ViewPort { get; set; }

		[JsonProperty(PropertyName = "location_type")]
		public LocationTypes LocationType { get; set; }
	}

	public class Leg
	{
		[JsonProperty(PropertyName = "distance")]
		public Distance Distance { get; set; }

		[JsonProperty(PropertyName = "duration")]
		public Duration Duration { get; set; }

		[JsonProperty(PropertyName = "end_address")]
		public string EndAddress { get; set; }

		[JsonProperty(PropertyName = "end_location")]
		public LatLng EndLocation { get; set; }

		[JsonProperty(PropertyName = "start_address")]
		public string StartAddress { get; set; }

		[JsonProperty(PropertyName = "start_location")]
		public LatLng StartLocation { get; set; }

		[JsonProperty(PropertyName = "steps")]
		public IEnumerable<Step> Steps { get; set; }
	}

	public class Place
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "icon")]
		public string Icon { get; set; }

		[JsonProperty(PropertyName = "vicinity")]
		public string Vicinity { get; set; }

		[JsonProperty(PropertyName = "types")]
		public IEnumerable<string> Types { get; set; }

		[JsonProperty(PropertyName = "geometry")]
		public Geometry Geometry { get; set; }

		[JsonProperty(PropertyName = "reference")]
		public string Reference { get; set; }

		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
	}

	public class PlaceDetails : Place
	{
		[JsonProperty(PropertyName = "formatted_phone_number")]
		public string FormattedPhoneNumber { get; set; }

		[JsonProperty(PropertyName = "formatted_address")]
		public string FormattedAddress { get; set; }

		[JsonProperty(PropertyName = "address_components")]
		public IEnumerable<AddressComponent> AddressComponents { get; set; }
	}

	public enum PlaceType : byte
	{
		premise,
		subpremise,
		street_number,
		route,
		locality,
		sublocality,
		political,
		administrative_area_level_1,
		administrative_area_level_2,
		administrative_area_level_3,
		country,
		postal_code,
		intersection,
		colloquial_area,
		neighborhood,
		natural_feature,
		airport,
		park,
		post_box,
		floor,
		room,
		geocode,
		bus_station,
		transit_station,
		establishment
	}

	public class Polyline
	{
		[JsonProperty(PropertyName = "points")]
		public string Points { get; set; }

		[JsonProperty(PropertyName = "levels")]
		public string Levels { get; set; }
	}

	public class Response<T> : ResponseBase<T>
	{
		[JsonProperty(PropertyName = "result")]
		public T Result { get; set; }
	}

	public abstract class ResponseBase<T>
	{
		[JsonProperty(PropertyName = "html_attributions")]
		public IEnumerable<string> HtmlAttributions { get; set; }

		[JsonProperty(PropertyName = "status")]
		public StatusCodes Status { get; set; }
	}

	public class Route
	{
		[JsonProperty(PropertyName = "copyrights")]
		public string Copyrights { get; set; }

		[JsonProperty(PropertyName = "summary")]
		public string Summary { get; set; }

		[JsonProperty(PropertyName = "bounds")]
		public ViewPort Bounds { get; set; }

		[JsonProperty(PropertyName = "warnings")]
		public IEnumerable<string> Warnings { get; set; }

		[JsonProperty(PropertyName = "waypoint_order")]
		public IEnumerable<int> WaypointOrder { get; set; }

		[JsonProperty(PropertyName = "legs")]
		public IEnumerable<Leg> Legs { get; set; }

		[JsonProperty(PropertyName = "overview_polyline")]
		public Polyline OverviewPolyline { get; set; }
	}

	public struct Size
	{
		public int Width { get; set; }
		public int Height { get; set; }

		public override string ToString()
		{
			return Width + "x" + Height;
		}
	}

	public enum StatusCodes
	{
		OK,
		UNKNOWN_ERROR,
		ZERO_RESULTS,
		OVER_QUERY_LIMIT,
		REQUEST_DENIED,
		INVALID_REQUEST,
		MAX_WAYPOINTS_EXCEEDED,
		NOT_FOUND
	}

	public class Step
	{
		[JsonProperty(PropertyName = "distance")]
		public Distance Distance { get; set; }

		[JsonProperty(PropertyName = "duration")]
		public Duration Duration { get; set; }

		[JsonProperty(PropertyName = "end_location")]
		public LatLng EndLocation { get; set; }

		[JsonProperty(PropertyName = "start_location")]
		public LatLng StartLocation { get; set; }

		[JsonProperty(PropertyName = "travel_mode")]
		public TravelMode TravelMode { get; set; }

		[JsonProperty(PropertyName = "html_instructions")]
		public string HtmlInstructions { get; set; }

		[JsonProperty(PropertyName = "polyline")]
		public Polyline Polyline { get; set; }
	}

	public class Suggestion
	{
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }

		[JsonProperty(PropertyName = "reference")]
		public string Reference { get; set; }

		[JsonProperty(PropertyName = "types")]
		public IEnumerable<PlaceType> Types { get; set; }
	}

	public class ViewPort
	{
		private const string Delimiter = "|";


		#region Public Properties

		[JsonProperty(PropertyName = "northeast")]
		public LatLng Northeast { get; set; }

		[JsonProperty(PropertyName = "southwest")]
		public LatLng Southwest { get; set; }

		#endregion


		#region Operators

		public static implicit operator LocationRect(ViewPort viewPort)
		{
			return new LocationRect
			{
				Northeast = viewPort.Northeast,
				Southwest = viewPort.Southwest
			};
		}

		public static implicit operator ViewPort(LocationRect viewPort)
		{
			return new ViewPort
			{
				Northeast = viewPort.Northeast,
				Southwest = viewPort.Southwest
			};
		}

		#endregion


		#region Public Methods

		public override string ToString()
		{
			return Southwest + Delimiter + Northeast;
		}

		#endregion
	}

	public class LatLng
	{
		#region Constants

		private const string Delimiter = ",";

		#endregion


		#region Public Properties

		[JsonProperty(PropertyName = "lat")]
		public double Lat { get; set; }

		[JsonProperty(PropertyName = "lng")]
		public double Lng { get; set; }

		#endregion


		#region Operators

		public static implicit operator Location(LatLng latLng)
		{
			return new Location(latLng.Lat, latLng.Lng);
		}

		public static implicit operator LatLng(Location location)
		{
			return new LatLng { Lat = location.Latitude, Lng = location.Longitude };
		}

		public static implicit operator GeoCoordinate(LatLng latLng)
		{
			return new GeoCoordinate(latLng.Lat, latLng.Lng);
		}

		public static implicit operator LatLng(GeoCoordinate coordinate)
		{
			return new LatLng { Lat = coordinate.Latitude, Lng = coordinate.Longitude };
		}

		#endregion


		#region Public Methods

		public override string ToString()
		{
			return Lat.ToString(NumberFormatInfo.InvariantInfo) + Delimiter + Lng.ToString(NumberFormatInfo.InvariantInfo);
		}

		#endregion
	}

	public enum TravelMode
	{
		Driving,
		Bicycling,
		Walking
	}
}
