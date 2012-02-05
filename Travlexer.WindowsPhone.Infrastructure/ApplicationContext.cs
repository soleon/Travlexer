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
			IsBusy.ValueChanging += OnIsLoadingChanging;
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
		}

		/// <summary>
		/// Loads the data context from the storage provided by <see cref="StorageProvider"/>.
		/// </summary>
		public static void LoadContext()
		{
			// Load toolbar state.
			ExpansionStates toolbarState;
			if (StorageProvider.TryGetSetting(ToolbarStateProperty, out toolbarState))
			{
				ToolbarState.Value = toolbarState;
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