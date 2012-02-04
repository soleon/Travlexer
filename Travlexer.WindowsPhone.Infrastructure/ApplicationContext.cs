using Codify.Models;

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
			IsBusy = new ObservableValue<bool> { Value = false };
			IsBusy.ValueChanging += OnIsLoadingChanging;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the observable value that indicates whether this data context is doing any loading.
		/// </summary>
		public static ObservableValue<bool> IsBusy { get; private set; }

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