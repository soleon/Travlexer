using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using Codify.Attributes;
using Codify.Entities;
using Codify.ViewModels;
using Microsoft.Phone.Controls;

namespace Codify.WindowsPhone
{
	public class NavigationService : INavigationService
	{
		#region Private Members

		private readonly Dictionary<Type, Uri> _viewMap = new Dictionary<Type, Uri>();
		private readonly PhoneApplicationFrame _phoneApplicationFrame;
        private readonly Func<Type, NotifyableEntity> _viewModelFactory;

		#endregion


		#region Constructors

        public NavigationService(PhoneApplicationFrame phoneApplicationFrame, Func<Type, NotifyableEntity> viewModelFactory)
		{
			_viewModelFactory = viewModelFactory;
			_phoneApplicationFrame = phoneApplicationFrame;
			_phoneApplicationFrame.Navigated += OnPhoneApplicationFrameNavigated;
		}

		#endregion


		#region Public Methods

		public void Register(Type viewModelType, Uri viewUrl)
		{
			_viewMap[viewModelType] = viewUrl;
		}

		public bool Navigate<T>()
		{
			var viewModelType = typeof (T);
			Uri uri;
			if (!_viewMap.ContainsKey(viewModelType))
			{
				var assembly = Assembly.GetCallingAssembly();
				var assemblyName = assembly.FullName;
				assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(",", StringComparison.Ordinal));
				var viewType = assembly.GetTypes()
					.FirstOrDefault(t =>
					{
						var attr = Attribute.GetCustomAttribute(t, typeof (ViewModelTypeAttribute)) as ViewModelTypeAttribute;
						return attr != null && attr.Type == viewModelType;
					});
				if (viewType == null || viewType.FullName == null)
				{
					throw new InvalidOperationException("Navigation failed. There is no view decorated with ViewModelTypeAttribute that maps to " + viewModelType + ".");
				}
				Register(viewModelType, uri = new Uri(viewType.FullName.Replace(assemblyName, null).Replace(".", "/") + ".xaml", UriKind.Relative));
			}
			else
			{
				uri = _viewMap[viewModelType];
			}
			return _phoneApplicationFrame.Navigate(uri);
		}

		public void GoBack()
		{
			_phoneApplicationFrame.GoBack();
		}

		public JournalEntry RemoveBackEntry()
		{
			return _phoneApplicationFrame.RemoveBackEntry();
		}

		#endregion


		#region Public Properties

		public bool CanGoBack
		{
			get { return _phoneApplicationFrame.CanGoBack; }
		}

		public Uri CurrentSource
		{
			get { return _phoneApplicationFrame.CurrentSource; }
		}

		#endregion


		#region Public Events

		/// <summary>
		/// This event is raised when the hardware Back button is pressed.
		/// </summary>
		public event EventHandler<CancelEventArgs> BackKeyPress
		{
			add { _phoneApplicationFrame.BackKeyPress += value; }
			remove { _phoneApplicationFrame.BackKeyPress -= value; }
		}

		/// <summary>
		/// This event is raised during a Microsoft.Phone.Controls.PhoneApplicationFrame.RemoveBackEntry operation or during a normal back navigation after the System.Windows.Navigation.NavigationService.Navigated event has been raised.
		/// </summary>
		public event EventHandler<JournalEntryRemovedEventArgs> JournalEntryRemoved
		{
			add { _phoneApplicationFrame.JournalEntryRemoved += value; }
			remove { _phoneApplicationFrame.JournalEntryRemoved -= value; }
		}

		/// <summary>
		/// This event is raised when the shell chrome is covering the frame.
		/// </summary>
		public event EventHandler<ObscuredEventArgs> Obscured
		{
			add { _phoneApplicationFrame.Obscured += value; }
			remove { _phoneApplicationFrame.Obscured -= value; }
		}

		/// <summary>
		/// Raised when the Orientation property has changed.
		/// </summary>
		public event EventHandler<OrientationChangedEventArgs> OrientationChanged
		{
			add { _phoneApplicationFrame.OrientationChanged += value; }
			remove { _phoneApplicationFrame.OrientationChanged -= value; }
		}

		/// <summary>
		/// This event is raised when the shell chrome is no longer covering the frame.
		/// </summary>
		public event EventHandler Unobscured
		{
			add { _phoneApplicationFrame.Unobscured += value; }
			remove { _phoneApplicationFrame.Unobscured -= value; }
		}

		#endregion


		#region Event Handling

		private void OnPhoneApplicationFrameNavigated(object sender, NavigationEventArgs e)
		{
			var content = e.Content;
			if (content == null)
			{
				return;
			}
			var element = e.Content as FrameworkElement;
			if (element == null)
			{
				throw new NotSupportedException("NavigationService only supports navigating to a Windows.Controls.FrameworkElement.");
			}
			if (element.DataContext != null)
			{
				return;
			}
			var attribute = Attribute.GetCustomAttribute(element.GetType(), typeof (ViewModelTypeAttribute)) as ViewModelTypeAttribute;
			if (attribute == null)
			{
                // Do not attempt to apply view model if it is not specified.
			    return;
			}
			if (element is PhoneApplicationPage)
			{
				((PhoneApplicationPage) element).DataContext = _viewModelFactory(attribute.Type);
			}
			else
			{
				element.DataContext = _viewModelFactory(attribute.Type);
			}
		}

		#endregion
	}
}
