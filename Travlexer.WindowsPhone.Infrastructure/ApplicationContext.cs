using Codify;
using Codify.Models;
using Codify.Storage;

namespace Travlexer.WindowsPhone.Infrastructure
{
	public static class ApplicationContext
	{
		#region Private Members

		/// <summary>
		/// Stores the number of busy requests.
		/// </summary>
		private static ushort _busyRequestCount;

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes the <see cref="ApplicationContext"/> class.
		/// </summary>
		static ApplicationContext()
		{
			ToolbarState = new ObservableValue<ExpansionStates>();
			IsBusy = new ObservableValue<bool> { Value = false };
			IsTrackingCurrentLocation = new ObservableValue<bool>();
			IsOnline = new ObservableValue<bool>(true);
			IsBusy.ValueChanging += OnIsLoadingChanging;
			IsFirstRun = true;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets or sets the storage provider for saving and loading data.
		/// </summary>
		public static IStorage StorageProvider
		{
			get { return _storageProvider ?? (_storageProvider = new IsolatedStorage()); }
			set { _storageProvider = value; }
		}

		private static IStorage _storageProvider;

		/// <summary>
		/// Gets the observable value that indicates whether this data context is doing any loading.
		/// </summary>
		public static ObservableValue<bool> IsBusy { get; private set; }

		/// <summary>
		/// Gets the state of the toolbar.
		/// </summary>
		public static ObservableValue<ExpansionStates> ToolbarState { get; private set; }

		private const string ToolbarStateProperty = "ToolbarState";


		/// <summary>
		/// Gets a value indicating whether this instance is the first run.
		/// </summary>
		public static bool IsFirstRun { get; private set; }

		private const string IsFirstRunProperty = "IsFirstRun";

		/// <summary>
		/// Gets or sets a value indicating whether this instance is tracking current location.
		/// </summary>
		public static ObservableValue<bool> IsTrackingCurrentLocation { get; set; }

		private const string IsTrackingCurrentLocationProperty = "IsTrackingCurrentLocation";

		/// <summary>
		/// Gets or sets a value indicating whether this instance is offline.
		/// </summary>
		public static ObservableValue<bool> IsOnline { get; private set; }

		private const string IsOnlineProperty = "IsOnline";

		#endregion


		#region Public Methods

		/// <summary>
		/// Toggles the state of the toolbar.
		/// </summary>
		public static void ToggleToolbarState()
		{
			ToolbarState.Value = ToolbarState.Value == ExpansionStates.Collapsed ? ExpansionStates.Expanded : ExpansionStates.Collapsed;
		}

		/// <summary>
		/// Saves the data context to the storage provided by <see cref="StorageProvider"/>.
		/// </summary>
		public static void SaveContext()
		{
			// Save toolbar state.
			StorageProvider.SaveSetting(ToolbarStateProperty, ToolbarState.Value);

			// Save first run flag.
			// Always save false, otherwise it's not called the "first run" flag isn't it.
			StorageProvider.SaveSetting(IsFirstRunProperty, false);

			// Save current location tracking flag.
			StorageProvider.SaveSetting(IsTrackingCurrentLocationProperty, IsTrackingCurrentLocation.Value);

			// Save offline flag.
			StorageProvider.SaveSetting(IsOnlineProperty, IsOnline.Value);
		}

		/// <summary>
		/// Loads the data context from the storage provided by <see cref="StorageProvider"/>.
		/// </summary>
		public static void LoadContext()
		{
			// Load first run flag.
			bool isFirstRun;
			if (StorageProvider.TryGetSetting(IsFirstRunProperty, out isFirstRun))
			{
				IsFirstRun = isFirstRun;
			}
			// Load toolbar state.
			ExpansionStates toolbarState;
			if (StorageProvider.TryGetSetting(ToolbarStateProperty, out toolbarState))
			{
				ToolbarState.Value = toolbarState;
			}

			// Load current location tracking flag.
			bool isTrackingCurrnetLocation;
			if (StorageProvider.TryGetSetting(IsTrackingCurrentLocationProperty, out isTrackingCurrnetLocation))
			{
				IsTrackingCurrentLocation.Value = isTrackingCurrnetLocation;
			}

			// Load offline flag.
			bool isOffline;
			if (StorageProvider.TryGetSetting(IsOnlineProperty, out isOffline))
			{
				IsOnline.Value = isOffline;
			}
		}

		#endregion


		#region Event Handling

		/// <summary>
		/// Called before the value of <see cref="IsBusy"/> is changed.
		/// </summary>
		/// <param name="oldValue">The existing value of <see cref="IsBusy"/>.</param>
		/// <param name="newValue">The desired new value.</param>
		/// <returns>The value to be set.</returns>
		private static bool OnIsLoadingChanging(bool oldValue, bool newValue)
		{
			if (newValue)
			{
				_busyRequestCount++;
			}
			else
			{
				_busyRequestCount--;
			}
			return _busyRequestCount > 0;
		}

		#endregion
	}
}
