using System;
using System.Net;
using Codify.GoogleMaps.Entities;
using Newtonsoft.Json;
using RestSharp;

namespace Codify.GoogleMaps
{
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
		 'international_phone_number' : '+61404490341',
		 'rating' : '3.5',
		 'website' : 'http://www.google.com',
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

        private const string RoutesResponse = @"
{
   'routes' : [
      {
         'bounds' : {
            'northeast' : {
               'lat' : 47.705710,
               'lng' : -122.124120
            },
            'southwest' : {
               'lat' : 47.669550,
               'lng' : -122.176090
            }
         },
         'copyrights' : 'Map data ©2012 Google',
         'legs' : [
            {
               'distance' : {
                  'text' : '7.4 km',
                  'value' : 7373
               },
               'duration' : {
                  'text' : '13 mins',
                  'value' : 760
               },
               'end_address' : '11747 124th Ave NE, Kirkland, WA 98034, USA',
               'end_location' : {
                  'lat' : 47.705710,
                  'lng' : -122.175730
               },
               'start_address' : 'Bear Creek Pkwy, Redmond, WA 98052, USA',
               'start_location' : {
                  'lat' : 47.669550,
                  'lng' : -122.124120
               },
               'steps' : [
                  {
                     'distance' : {
                        'text' : '0.8 km',
                        'value' : 795
                     },
                     'duration' : {
                        'text' : '2 mins',
                        'value' : 116
                     },
                     'end_location' : {
                        'lat' : 47.674120,
                        'lng' : -122.130330
                     },
                     'html_instructions' : 'Head \u003cb\u003enorth\u003c/b\u003e on \u003cb\u003eBear Creek Pkwy\u003c/b\u003e toward \u003cb\u003eNE 74th St\u003c/b\u003e',
                     'polyline' : {
                        'points' : 'um}aHvjkhVQ@MBm@B_@@gBBYDYD[Hq@LKBKBIBODSFUHe@Nq@RSFKDQJSJMJMLKLKNINKPK\\KXI`@GZEZEVCTCRANA?AJ?LALATAT?TAN?LAJ?LAH?JC`@ALAJALAJALCNANCLAJCLCNEXEZi@pDMp@EVAJCHCFCFCDCFCBABCBYR'
                     },
                     'start_location' : {
                        'lat' : 47.669550,
                        'lng' : -122.124120
                     },
                     'travel_mode' : 'DRIVING'
                  },
                  {
                     'distance' : {
                        'text' : '2.7 km',
                        'value' : 2736
                     },
                     'duration' : {
                        'text' : '5 mins',
                        'value' : 316
                     },
                     'end_location' : {
                        'lat' : 47.679560,
                        'lng' : -122.163930
                     },
                     'html_instructions' : 'Turn \u003cb\u003eleft\u003c/b\u003e onto \u003cb\u003eRedmond Way\u003c/b\u003e',
                     'polyline' : {
                        'points' : 'gj~aHpqlhVXpA`@jBNl@FX@H@F@DDZBN@B@HBXBXBV?D?@@J@^@V?Z?\\?PAN?P?FC^?HAHAZEZMp@Ol@Qn@Qb@Uj@m@hAu@rAOXO\\Qj@KVIZIZMx@Gb@Ed@C\\AZA^CxE@dI?ZG`H?b@?bA?T@bA?XA|AEbEDxBJ|An@xED\\ZzBLdABZBj@?b@?t@Cj@I`AIp@GXSt@GTA@SPuBhFqAvCKb@Qb@Yx@AFY~@GVQz@Kf@[vAm@pCQX_@|AgJla@o@lCQx@OdAIx@Ej@Cj@Af@Az@?v@'
                     },
                     'start_location' : {
                        'lat' : 47.674120,
                        'lng' : -122.130330
                     },
                     'travel_mode' : 'DRIVING'
                  },
                  {
                     'distance' : {
                        'text' : '0.9 km',
                        'value' : 906
                     },
                     'duration' : {
                        'text' : '1 min',
                        'value' : 68
                     },
                     'end_location' : {
                        'lat' : 47.679320,
                        'lng' : -122.1760
                     },
                     'html_instructions' : 'Continue onto \u003cb\u003eNE 85th St\u003c/b\u003e',
                     'polyline' : {
                        'points' : 'gl_bHpcshV@rDHN@bCLfR@|C@jEDhM@lB@xBBpD?^?`@@lD'
                     },
                     'start_location' : {
                        'lat' : 47.679560,
                        'lng' : -122.163930
                     },
                     'travel_mode' : 'DRIVING'
                  },
                  {
                     'distance' : {
                        'text' : '2.9 km',
                        'value' : 2936
                     },
                     'duration' : {
                        'text' : '4 mins',
                        'value' : 260
                     },
                     'end_location' : {
                        'lat' : 47.705710,
                        'lng' : -122.175730
                     },
                     'html_instructions' : 'Turn \u003cb\u003eright\u003c/b\u003e onto \u003cb\u003e124th Ave NE\u003c/b\u003e\u003cdiv style=\'font-size:0.9em\'\u003eDestination will be on the left\u003c/div\u003e',
                     'polyline' : {
                        'points' : 'wj_bH~nuhViB?_FDQ@gCBaDD{@?uDKaAAwEIiACgCE{DIUAyACwFK[CuAC{C?{C?sA?cE@mCA_E?mC@k@?aA?_@?cD@sA?o@?yB@uA@{@?iC@mCAgC@mC?gC?qD@Q?eFAe@?oHK'
                     },
                     'start_location' : {
                        'lat' : 47.679320,
                        'lng' : -122.1760
                     },
                     'travel_mode' : 'DRIVING'
                  }
               ],
               'via_waypoint' : []
            }
         ],
         'overview_polyline' : {
            'points' : 'um}aHvjkhV_@DmADgBBYDu@NsAX_Bf@eAZ]Pa@VYZU^Wn@Uz@Mv@OpACJAZEpACp@GfAIv@eAnHY~AO\\c@^rAdGVjBFx@D~AApAEz@Gv@]~Ac@rAcAtBeAlBa@hAUr@WtAMhAEx@ExF@`JG~K@|AG`HDxBJ|At@vFh@`EFfA?xAMlBQjA[jAURgE`Ky@hCa@vA]bBiAhFQX_@|AwKze@a@~BOdBErAArB@rDHNNjVBhJLbZ?`A@lDiB?qFFiHHqFKyGKcLUqIOqBGoQ@mIAyD@yI@{HBmP@qP?uIK'
         },
         'summary' : 'Redmond Way and 124th Ave NE',
         'warnings' : [],
         'waypoint_order' : []
      }
   ],
   'status' : 'OK'
}
";

        #endregion


        #region Public Methods

        public void GetPlaces(LatLng location, Action<IRestResponse<ListResponse<Place>>> callback)
        {
            var result = new RestResponse<ListResponse<Place>>
            {
                StatusCode = HttpStatusCode.OK,
                Data = JsonConvert.DeserializeObject<ListResponse<Place>>(GeocodeResponse)
            };
            callback(result);
        }

        public void GetPlaces(string address, Action<IRestResponse<ListResponse<Place>>> callback = null)
        {
            throw new NotImplementedException();
        }

        public void GetPlaceDetails(string reference, Action<IRestResponse<Response<Place>>> callback = null)
        {
            throw new NotImplementedException();
        }

        public void GetSuggestions(LatLng center, string input, Action<IRestResponse<AutoCompleteResponse>> callback = null)
        {
            throw new NotImplementedException();
        }

        public void Search(LatLng center, string input, Action<IRestResponse<ListResponse<Place>>> callback = null)
        {
            throw new NotImplementedException();
        }

        public void CancelGetSuggestions() {}

        public void GetDirections(string origin, string destination, TravelMode mode, RouteMethod method, Units unit, Action<IRestResponse<RoutesResponse>> callback)
        {
            var result = new RestResponse<RoutesResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Data = JsonConvert.DeserializeObject<RoutesResponse>(RoutesResponse)
            };
            callback(result);
        }

        #endregion
    }
}