using System;
using System.Net.NetworkInformation;
using Codify.ViewModels;
using Codify.WindowsPhone;
using Microsoft.Phone.Controls;
using Ninject;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone
{
	public static class ApplicationContext
	{
		#region Public Properties

		/// <summary>
		/// Gets a value indicating whether the network is available.
		/// </summary>
		public static bool IsNetworkAvailable { get; private set; }

		public static INavigationService NavigationService
		{
			get;
			private set;
		}

		public static IDataContext Data { get; private set; }

		public static IConfigurationContext Configuration { get; private set; }

		#endregion


		#region Event Handling

		private static void OnNetworkChanged(object sender, EventArgs e)
		{
			IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Initializes the application context for use through out the entire application life time.
		/// </summary>
		/// <param name="frame">The <see cref="T:Microsoft.Phone.Controls.PhoneApplicationFrame"/> of the current application.</param>
		public static void Initialize(PhoneApplicationFrame frame)
		{
			// Initialize Ioc container.
			var kernel = new StandardKernel();
			kernel.Bind<Func<Type, IViewModel>>().ToMethod(context => t => context.Kernel.Get(t) as IViewModel);
			kernel.Bind<PhoneApplicationFrame>().ToConstant(frame);
			kernel.Bind<INavigationService>().To(typeof(NavigationService)).InSingletonScope();
			kernel.Bind<IDataContext>().To<DataContext>();
			kernel.Bind<IConfigurationContext>().To(typeof(ConfigurationContext)).InSingletonScope();

			// Initialize contexts.
			Data = kernel.Get<IDataContext>();
			Configuration = kernel.Get<IConfigurationContext>();
			NavigationService = kernel.Get<INavigationService>();

			// Initializes the network avalability flag and listens to network changes.
			IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
			NetworkChange.NetworkAddressChanged += OnNetworkChanged;
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
