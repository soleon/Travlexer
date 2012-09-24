using System;
using System.ComponentModel;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Codify.WindowsPhone
{
	public interface INavigationService {
		void Register(Type viewModelType, Uri viewUrl);
		bool Navigate<T>();
		void GoBack();
		JournalEntry RemoveBackEntry();
		bool CanGoBack { get; }
		Uri CurrentSource { get; }

		/// <summary>
		/// This event is raised when the hardware Back button is pressed.
		/// </summary>
		event EventHandler<CancelEventArgs> BackKeyPress;

		/// <summary>
		/// This event is raised during a Microsoft.Phone.Controls.PhoneApplicationFrame.RemoveBackEntry operation or during a normal back navigation after the System.Windows.Navigation.NavigationService.Navigated event has been raised.
		/// </summary>
		event EventHandler<JournalEntryRemovedEventArgs> JournalEntryRemoved;

		/// <summary>
		/// This event is raised when the shell chrome is covering the frame.
		/// </summary>
		event EventHandler<ObscuredEventArgs> Obscured;

		/// <summary>
		/// Raised when the Orientation property has changed.
		/// </summary>
		event EventHandler<OrientationChangedEventArgs> OrientationChanged;

		/// <summary>
		/// This event is raised when the shell chrome is no longer covering the frame.
		/// </summary>
		event EventHandler Unobscured;

	    /// <summary>
	    /// Occurs when a new navigation is requested.
	    /// </summary>
	    event NavigatingCancelEventHandler Navigating;
	}
}