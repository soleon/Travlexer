﻿using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Text.RegularExpressions;
using Codify.Extensions;
using Codify.GoogleMaps.Entities;
using Microsoft.Phone.Controls.Maps;
using Travlexer.Data;
using Place = Travlexer.Data.Place;
using Route = Travlexer.Data.Route;
using RouteMethod = Codify.GoogleMaps.Entities.RouteMethod;
using TravelMode = Codify.GoogleMaps.Entities.TravelMode;
using ViewPort = Travlexer.Data.ViewPort;

namespace Travlexer.WindowsPhone.Infrastructure
{
    public static class DataExtensions
    {
        private static readonly Regex XmlTagRegex = new Regex("<[^>]+>");
        private static readonly Regex DivTagRegex = new Regex("<div[^>]*>");

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

        public static Location ToLocalLocation(this LatLng latLng)
        {
            return latLng == null ? null : new Location {Latitude = latLng.Lat, Longitude = latLng.Lng};
        }

        public static Location ToLocalLocation(this GeoCoordinate coordinate)
        {
            return coordinate == null ? null : new Location {Latitude = coordinate.Latitude, Longitude = coordinate.Longitude};
        }

        public static GeoCoordinate ToGeoCoordinate(this Location location)
        {
            return location == null ? null : new GeoCoordinate(location.Latitude, location.Longitude);
        }

        public static LatLng ToGoogleLocation(this Location location)
        {
            return location == null ? null : new LatLng {Lat = location.Latitude, Lng = location.Longitude};
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

        public static SearchSuggestion ToLocalSearchSuggestion(this Suggestion suggestion)
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

        public static TravelMode ToGoogleTravelMode(this Data.TravelMode mode)
        {
            return (TravelMode) mode;
        }

        public static RouteMethod ToGoogleRouteMethod(this Data.RouteMethod method)
        {
            return (RouteMethod) method;
        }

        public static Units ToGoogleUnits(this UnitSystems unit)
        {
            return (Units) unit;
        }

        public static Route ToLocalRoute(this Codify.GoogleMaps.Entities.Route route)
        {
            if (route == null) throw new ArgumentNullException("route", "Cannot convert null Codify.GoogleMaps.Entities.Route to local Travlexer.Data.Route.");
            var newRoute = new Route();

            var legs = route.Legs;
            if (!legs.IsNullOrEmpty())
            {
                var leg = legs[0];
                newRoute.Distance = leg.Distance.Value;
                newRoute.Duration = leg.Duration.Value;

                var steps = leg.Steps;
                if (!steps.IsNullOrEmpty())
                {
                    var points = new Collection<Location>();
                    var routeSteps = new Collection<RouteStep>();
                    foreach (var step in steps)
                    {
                        points.AddRange(Utilities.DecodePolylinePoints(step.Polyline.Points));
                        routeSteps.Add(new RouteStep
                        {
                            Distance = step.Distance.Value,
                            Duration = step.Duration.Value,
                            Instruction = XmlTagRegex.Replace(DivTagRegex.Replace(step.HtmlInstructions, Environment.NewLine), string.Empty),
                            StartLocation = step.StartLocation.ToLocalLocation(),
                            EndLocation = step.EndLocation.ToLocalLocation()
                        });
                    }
                    newRoute.Points = points;
                    newRoute.Steps = routeSteps;
                }
            }

            return newRoute;
        }
    }
}