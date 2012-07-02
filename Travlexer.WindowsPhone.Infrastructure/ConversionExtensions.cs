using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Codify.Extensions;
using Microsoft.Phone.Controls.Maps;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.Infrastructure
{
    public static class ConversionExtensions
    {
        public static Place ToPlace(this Codify.GoogleMaps.Entities.Place googlePlace)
        {
            return new Place(googlePlace.Geometry.Location.ToLocalLocation(), name: googlePlace.Name)
            {
                ContactNumber = googlePlace.InternationalPhoneNumber ?? googlePlace.FormattedPhoneNumber,
                Address = googlePlace.FormattedAddress,
                ViewPort = googlePlace.Geometry.ViewPort.ToLocalViewPort(),
                Reference = googlePlace.Reference,
                WebSite = googlePlace.WebSite,
                Rating = googlePlace.Raiting
            };
        }

        public static Location ToLocalLocation(this Codify.GoogleMaps.Entities.LatLng latLng)
        {
            return latLng == null ? null : new Location { Latitude = latLng.Lat, Longitude = latLng.Lng };
        }

        public static Location ToLocalLocation(this GeoCoordinate coordinate)
        {
            return coordinate == null ? null : new Location { Latitude = coordinate.Latitude, Longitude = coordinate.Longitude };
        }

        public static GeoCoordinate ToGeoCoordinate(this Location location)
        {
            return location == null ? null : new GeoCoordinate(location.Latitude, location.Longitude);
        }

        public static Codify.GoogleMaps.Entities.LatLng ToGoogleLocation(this Location location)
        {
            return location == null ? null : new Codify.GoogleMaps.Entities.LatLng { Lat = location.Latitude, Lng = location.Longitude };
        }

        public static ViewPort ToLocalViewPort(this Codify.GoogleMaps.Entities.ViewPort viewPort)
        {
            if (viewPort == null)
            {
                return null;
            }
            return new ViewPort
            {
                Northeast = viewPort.Northeast.ToLocalLocation(),
                Southwest = viewPort.Southwest.ToLocalLocation()
            };
        }

        public static ViewPort ToLocalViewPort(this LocationRect rect)
        {
            if (rect == null)
            {
                return null;
            }
            return new ViewPort
            {
                Northeast = rect.Northeast.ToLocalLocation(),
                Southwest = rect.Southwest.ToLocalLocation()
            };
        }

        public static LocationRect ToLocationRect(this ViewPort viewPort)
        {
            if (viewPort == null)
            {
                return null;
            }
            return new LocationRect
            {
                Northeast = viewPort.Northeast.ToGeoCoordinate(),
                Southwest = viewPort.Southwest.ToGeoCoordinate()
            };
        }

        public static Codify.GoogleMaps.Entities.ViewPort ToGoogleViewPort(this ViewPort viewPort)
        {
            if (viewPort == null)
            {
                return null;
            }
            return new Codify.GoogleMaps.Entities.ViewPort
            {
                Northeast = viewPort.Northeast.ToGoogleLocation(),
                Southwest = viewPort.Southwest.ToGoogleLocation()
            };
        }

        public static SearchSuggestion ToLocalSearchSuggestion(this Codify.GoogleMaps.Entities.Suggestion suggestion)
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

        public static Codify.GoogleMaps.Entities.TravelMode ToGoogleTravelMode(this TravelMode mode)
        {
            return (Codify.GoogleMaps.Entities.TravelMode)mode;
        }

        public static Codify.GoogleMaps.Entities.RouteMethod ToGoogleRouteMethod(this RouteMethod method)
        {
            return (Codify.GoogleMaps.Entities.RouteMethod)method;
        }

        public static Codify.GoogleMaps.Entities.Units ToGoogleUnits(this Units unit)
        {
            return (Codify.GoogleMaps.Entities.Units)unit;
        }

        public static Route ToLocalRoute(this Codify.GoogleMaps.Entities.Route route)
        {
            if (route == null)
            {
                return null;
            }
            var newRoute = new Route();

            var legs = route.Legs;
            if (legs.Any())
            {
                var steps = legs[0].Steps;
                if (steps.Any())
                {
                    var points = new Collection<Location>();
                    foreach (var step in steps)
                    {
                        points.AddRange(Utilities.DecodePolylinePoints(step.Polyline.Points));
                    }
                    newRoute.Points = points;
                }
            }

            return newRoute;
        }
    }
}
