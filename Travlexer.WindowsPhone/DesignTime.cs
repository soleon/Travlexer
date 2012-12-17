using System;
using System.ComponentModel;
using System.Device.Location;
using Codify.Entities;
using Codify.GoogleMaps;
using Codify.Serialization;
using Codify.Storage;
using Codify.WindowsPhone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Ninject;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone
{
    public class DesignTime
    {
#if DEBUG
        private IKernel _kernel;

        public MapViewModel MapViewModel
        {
            get
            {
                if (!DesignerProperties.IsInDesignTool) return null;
                if (_mapViewModel == null)
                {
                    Initialize();
                    _mapViewModel = _kernel.Get<MapViewModel>();
                }
                return _mapViewModel;
            }
        }

        private MapViewModel _mapViewModel;

        public LocationCollection SampleLocations
        {
            get
            {
                if (!DesignerProperties.IsInDesignTool) return null;
                return new LocationCollection
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 60),
                    new GeoCoordinate(60, -30),
                    new GeoCoordinate(60, 0)
                };
            }
        }

        public ElementColor SampleColor { get; set; }

        public PlaceDetailsViewModel PlaceDetailsViewModel
        {
            get
            {
                if (!DesignerProperties.IsInDesignTool) return null;
                Initialize();
                var place = ApplicationContext.Data.Places[0];
                //place.IsSearchResult = true;
                ApplicationContext.Data.SelectedPlace.Value = place;
                var vm = _kernel.Get<PlaceDetailsViewModel>();
                vm.Data = place;
                vm.Data.Notes = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
                return vm;
            }
        }

        public ManageViewModel ManageViewModel
        {
            get
            {
                if (!DesignerProperties.IsInDesignTool) return null;
                if (_manageViewModel == null)
                {
                    Initialize();
                    _manageViewModel = _kernel.Get<ManageViewModel>();
                }
                return _manageViewModel;
            }
        }

        private ManageViewModel _manageViewModel;

        public CheckableViewModel<object> CheckableViewModel
        {
            get
            {
                if (!DesignerProperties.IsInDesignTool) return null;
                return _checkableViewModel ?? (_checkableViewModel = new CheckableViewModel<object>());
            }
        }

        private CheckableViewModel<object> _checkableViewModel;

        public RouteDetailsViewModel RouteDetailsViewModel
        {
            get
            {
                if (!DesignerProperties.IsInDesignTool) return null;
                if (_routeDetailsViewModel == null)
                {
                    Initialize();
                    ApplicationContext.Data.SelectedRoute.Value = ApplicationContext.Data.Routes[0];
                    _routeDetailsViewModel = _kernel.Get<RouteDetailsViewModel>();
                }
                return _routeDetailsViewModel;
            }
        }

        private RouteDetailsViewModel _routeDetailsViewModel;

        public SettingsViewModel Settings
        {
            get { return _settings ?? (_settings = _kernel.Get<SettingsViewModel>()); }
        }

        private SettingsViewModel _settings;

        public AppInfoViewModel AppInfo
        {
            get { return _appInfo ?? (_appInfo = new AppInfoViewModel()); }
        }

        private AppInfoViewModel _appInfo;

        private void Initialize()
        {
            if (_kernel != null) return;
            _kernel = new StandardKernel();
            _kernel.Bind<Func<Type, NotifyableEntity>>().ToMethod(context => t => context.Kernel.Get(t) as NotifyableEntity);
            _kernel.Bind<PhoneApplicationFrame>().ToConstant(new PhoneApplicationFrame());
            _kernel.Bind<INavigationService>().To<NavigationService>().InSingletonScope();
            _kernel.Bind<IStorage>().To<IsolatedStorage>().InSingletonScope();
            _kernel.Bind<ISerializer<byte[]>>().To<BinarySerializer>().InSingletonScope();
            _kernel.Bind<IDataContext>().To<DataContext>().InSingletonScope();
            _kernel.Bind<IGoogleMapsClient>().To<GoogleMapsClientMock>().InSingletonScope();
            _kernel.Bind<IConfigurationContext>().To<ConfigurationContext>().InSingletonScope();

            ApplicationContext.Initialize(_kernel);
            var place1 = ApplicationContext.Data.AddNewPlace(new Location
            {
                Latitude = 9.1540930,
                Longitude = -1.39166990
            });
            var place2 = ApplicationContext.Data.AddNewPlace(new Location
            {
                Latitude = 9.1540930,
                Longitude = -1.39166990
            });
            ApplicationContext.Data.GetRoute(null, null, default(TravelMode), default(RouteMethod), r =>
            {
                r.Result.ArrivalPlaceId = place1.Id;
                r.Result.DeparturePlaceId = place2.Id;
            });
        }
#endif
    }
}