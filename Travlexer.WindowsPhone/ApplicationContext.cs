using System;
using System.Device.Location;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using Codify.Entities;
using Codify.GoogleMaps;
using Codify.Serialization;
using Codify.Storage;
using Codify.WindowsPhone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone
{
    public static class ApplicationContext
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether the network is available.
        /// </summary>
        public static bool IsNetworkAvailable { get; private set; }

        public static INavigationService NavigationService { get; private set; }

        public static IDataContext Data { get; private set; }

        public static IConfigurationContext Configuration { get; private set; }

        public static GeoCoordinateWatcher GeoCoordinateWatcher { get; private set; }

        #endregion


        #region Public Methods

        /// <summary>
        ///     Initializes the application context for use through out the entire application life time.
        /// </summary>
        /// <param name="frame">
        ///     The <see cref="T:Microsoft.Phone.Controls.PhoneApplicationFrame" /> of the current application.
        /// </param>
        public static void Initialize(PhoneApplicationFrame frame)
        {
            // Initialize Ioc container.
            var kernel = new StandardKernel();
            kernel.Bind<Func<Type, NotifyableEntity>>().ToMethod(context => t => context.Kernel.Get(t) as NotifyableEntity);
            kernel.Bind<PhoneApplicationFrame>().ToConstant(frame);
            kernel.Bind<INavigationService>().To<NavigationService>().InSingletonScope();
            kernel.Bind<IStorage>().To<IsolatedStorage>().InSingletonScope();
            kernel.Bind<ISerializer<byte[]>>().To<BinarySerializer>().InSingletonScope();
            kernel.Bind<IGoogleMapsClient>().To<GoogleMapsClient>().InSingletonScope();
            kernel.Bind<IDataContext>().To<DataContext>().InSingletonScope();
            kernel.Bind<IConfigurationContext>().To<ConfigurationContext>().InSingletonScope();
            Initialize(kernel);

            // Initialize event handlers and other properties.
            GeoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High) {MovementThreshold = 10D};

            ((DataContext) Data).AppVersion = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;

            Data.PreventScreenLock.ValueChanged += (old, @new) => { PhoneApplicationService.Current.ApplicationIdleDetectionMode = @new ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled; };

            Data.UseLocationService.ValueChanged += (old, @new) =>
            {
                if (@new) GeoCoordinateWatcher.Start();
                else GeoCoordinateWatcher.Stop();
            };

            IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            NetworkChange.NetworkAddressChanged += (s, e) => IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
        }

        /// <summary>
        ///     Initializes the application context for use through out the entire application life time.
        /// </summary>
        /// <param name="kernel">
        ///     An <see cref="T:Ninject.IKernel" /> that contains necessary configuration to initialize the application context.
        /// </param>
        public static void Initialize(IKernel kernel)
        {
            // Initialize contexts.
            Data = kernel.Get<IDataContext>();
            Configuration = kernel.Get<IConfigurationContext>();
            NavigationService = kernel.Get<INavigationService>();
        }

        public static void LoadContext()
        {
            Data.LoadContext();
            Configuration.LoadContext();
        }

        public static void SaveContext()
        {
            Data.SaveContext();
            Configuration.SaveContext();
        }

        #endregion
    }
}