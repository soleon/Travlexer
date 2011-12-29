using System;
using System.Net.NetworkInformation;

namespace Travlexer.WindowsPhone
{
	public static class Globals
	{
		#region Constructors

		/// <summary>
		/// Initializes the <see cref="Globals"/> class.
		/// </summary>
		static Globals()
		{
			// Initializes the network avalability flag and listens to network changes.
			IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
			NetworkChange.NetworkAddressChanged += OnNetworkChanged;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets or sets the data context that contains all essential data for this application.
		/// </summary>
		public static IDataContext DataContext
		{
			get { return _dataContext ?? (_dataContext = new DataContext()); }
			set { _dataContext = value; }
		}

		private static IDataContext _dataContext;

		/// <summary>
		/// Gets a value indicating whether the network is available.
		/// </summary>
		public static bool IsNetworkAvailable { get; private set; }

		#endregion


		#region Event Handling

		private static void OnNetworkChanged(object sender, EventArgs e)
		{
			IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
		}

		#endregion
	}
}
