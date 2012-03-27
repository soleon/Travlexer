using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Codify.WindowsPhone
{
	public static class NavigationService
	{
		#region Private Members

		private static readonly Dictionary<Type, Uri> _viewMap = new Dictionary<Type, Uri>();
		private static readonly PhoneApplicationFrame _phoneApplicationFrame;

		#endregion


		#region Constructors

		static NavigationService()
		{
			_phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
			if (_phoneApplicationFrame != null)
			{
				return;
			}
#if DEBUG
			if (DesignerProperties.IsInDesignTool)
			{
				// Create dummy frame for the designer as the root visual will not be available in designer environment.
				_phoneApplicationFrame = new PhoneApplicationFrame();
				return;
			}
#endif
			throw new InvalidOperationException("The root visual of the current application is either not available or is not a Microsoft.Phone.Controls.PhoneApplicationFrame. NavigationService cannot be used at this time.");
		}

		#endregion


		#region Public Methods

		public static void Register(Type viewModelType, Uri viewUrl)
		{
			_viewMap[viewModelType] = viewUrl;
		}

		public static bool Navigate(Type viewModelType)
		{
			var uri = _viewMap[viewModelType];
			return _phoneApplicationFrame.Navigate(uri);
		}

		public static void GoBack()
		{
			_phoneApplicationFrame.GoBack();
		}

		public static JournalEntry RemoveBackEntry()
		{
			return _phoneApplicationFrame.RemoveBackEntry();
		}

		#endregion


		#region Public Properties

		public static bool CanGoBack
		{
			get { return _phoneApplicationFrame.CanGoBack; }
		}


		public static Uri CurrentSource
		{
			get { return _phoneApplicationFrame.CurrentSource; }
		}

		#endregion


		#region Public Events

		/// <summary>
		/// This event is raised when the hardware Back button is pressed.
		/// </summary>
		public static event EventHandler<CancelEventArgs> BackKeyPress
		{
			add { _phoneApplicationFrame.BackKeyPress += value; }
			remove { _phoneApplicationFrame.BackKeyPress -= value; }
		}

		/// <summary>
		/// This event is raised during a Microsoft.Phone.Controls.PhoneApplicationFrame.RemoveBackEntry operation or during a normal back navigation after the System.Windows.Navigation.NavigationService.Navigated event has been raised.
		/// </summary>
		public static event EventHandler<JournalEntryRemovedEventArgs> JournalEntryRemoved
		{
			add { _phoneApplicationFrame.JournalEntryRemoved += value; }
			remove { _phoneApplicationFrame.JournalEntryRemoved -= value; }
		}

		/// <summary>
		/// This event is raised when the shell chrome is covering the frame.
		/// </summary>
		public static event EventHandler<ObscuredEventArgs> Obscured
		{
			add { _phoneApplicationFrame.Obscured += value; }
			remove { _phoneApplicationFrame.Obscured -= value; }
		}

		/// <summary>
		/// Raised when the Orientation property has changed.
		/// </summary>
		public static event EventHandler<OrientationChangedEventArgs> OrientationChanged
		{
			add { _phoneApplicationFrame.OrientationChanged += value; }
			remove { _phoneApplicationFrame.OrientationChanged -= value; }
		}

		/// <summary>
		/// This event is raised when the shell chrome is no longer covering the frame.
		/// </summary>
		public static event EventHandler Unobscured
		{
			add { _phoneApplicationFrame.Unobscured += value; }
			remove { _phoneApplicationFrame.Unobscured -= value; }
		}

		#endregion
	}
}
