using Codify;
using Codify.Entities;
using Codify.Storage;

namespace Travlexer.WindowsPhone.Infrastructure
{
	public class ConfigurationContext : IConfigurationContext
	{
		#region Private Members

		/// <summary>
		/// Stores the number of busy requests.
		/// </summary>
		private ushort _busyRequestCount;

        private readonly IStorage _storageProvider;

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes the <see cref="ConfigurationContext"/> class.
		/// </summary>
		public ConfigurationContext(IStorage storageProvider)
		{
            _storageProvider = storageProvider;

			ToolbarState = new ObservableValue<ExpansionStates>();
			IsBusy = new ObservableValue<bool>(false, true);
			IsTrackingCurrentLocation = new ObservableValue<bool>();
			IsOnline = new ObservableValue<bool>(true);
			IsBusy.ValueChanging += OnIsLoadingChanging;
			IsFirstRun = true;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the observable value that indicates whether this data context is doing any loading.
		/// </summary>
		public ObservableValue<bool> IsBusy { get; private set; }

		/// <summary>
		/// Gets the state of the toolbar.
		/// </summary>
		public ObservableValue<ExpansionStates> ToolbarState { get; private set; }

		private const string ToolbarStateProperty = "ToolbarState";


		/// <summary>
		/// Gets a value indicating whether this instance is the first run.
		/// </summary>
		public bool IsFirstRun { get; private set; }

		private const string IsFirstRunProperty = "IsFirstRun";

		/// <summary>
		/// Gets or sets a value indicating whether this instance is tracking current location.
		/// </summary>
		public ObservableValue<bool> IsTrackingCurrentLocation { get; set; }

		private const string IsTrackingCurrentLocationProperty = "IsTrackingCurrentLocation";

		/// <summary>
		/// Gets or sets a value indicating whether this instance is offline.
		/// </summary>
		public ObservableValue<bool> IsOnline { get; private set; }

		private const string IsOnlineProperty = "IsOnline";

		#endregion


		#region Public Methods

		/// <summary>
		/// Toggles the state of the toolbar.
		/// </summary>
		public void ToggleToolbarState()
		{
			ToolbarState.Value = ToolbarState.Value == ExpansionStates.Collapsed ? ExpansionStates.Expanded : ExpansionStates.Collapsed;
		}

		/// <summary>
        /// Saves the data context to the storage provided by storage provider implements <see cref="T:Codify.Storage.IStorage"/>.
		/// </summary>
		public void SaveContext()
		{
			// Save toolbar state.
			_storageProvider.SaveSetting(ToolbarStateProperty, ToolbarState.Value);

			// Save first run flag.
			// Always save false, otherwise it's not called the "first run" flag isn't it.
            _storageProvider.SaveSetting(IsFirstRunProperty, false);

			// Save current location tracking flag.
            _storageProvider.SaveSetting(IsTrackingCurrentLocationProperty, IsTrackingCurrentLocation.Value);

			// Save offline flag.
            _storageProvider.SaveSetting(IsOnlineProperty, IsOnline.Value);
		}

		/// <summary>
		/// Loads the data context from the storage provided by a storage provider implements <see cref="T:Codify.Storage.IStorage"/>.
		/// </summary>
		public void LoadContext()
		{
			// Load first run flag.
			bool isFirstRun;
            if (_storageProvider.TryGetSetting(IsFirstRunProperty, out isFirstRun))
			{
				IsFirstRun = isFirstRun;
			}
			// Load toolbar state.
			ExpansionStates toolbarState;
            if (_storageProvider.TryGetSetting(ToolbarStateProperty, out toolbarState))
			{
				ToolbarState.Value = toolbarState;
			}

			// Load current location tracking flag.
			bool isTrackingCurrnetLocation;
            if (_storageProvider.TryGetSetting(IsTrackingCurrentLocationProperty, out isTrackingCurrnetLocation))
			{
				IsTrackingCurrentLocation.Value = isTrackingCurrnetLocation;
			}

			// Load offline flag.
			bool isOffline;
            if (_storageProvider.TryGetSetting(IsOnlineProperty, out isOffline))
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
		private bool OnIsLoadingChanging(bool oldValue, bool newValue)
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
