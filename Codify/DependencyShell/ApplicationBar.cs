using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;
using Codify.Extensions;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Codify.DependencyShell
{
	public class ApplicationBar : Behavior<PhoneApplicationPage>
	{
		#region Private Members

		/// <summary>
		/// Reference to the real application bar that is displayed in the associated page.
		/// </summary>
		private Microsoft.Phone.Shell.ApplicationBar _applicationBar;

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationBar"/> class.
		/// </summary>
		public ApplicationBar()
		{
			_applicationBar = new Microsoft.Phone.Shell.ApplicationBar();			
			_buttons = new ObservableCollection<ApplicationBarIconButton>();
			_buttons.CollectionChanged += OnButtonsCollectionChanged;
			_menuItems = new ObservableCollection<ApplicationBarMenuItem>();
			_menuItems.CollectionChanged += OnMenuItemsCollectionChanged;
		}


		#endregion


		#region Public Properties

		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(
			"IsVisible",
			typeof(bool),
			typeof(ApplicationBar),
			new PropertyMetadata(true, (s, e) => ((ApplicationBar)s)._applicationBar.IsVisible = (bool)e.NewValue));

		public double Opacity
		{
			get { return (double)GetValue(OpacityProperty); }
			set { SetValue(OpacityProperty, value); }
		}

		public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(
			"Opacity",
			typeof(double),
			typeof(ApplicationBar),
			new PropertyMetadata(1D, (s, e) => ((ApplicationBar)s)._applicationBar.Opacity = (double)e.NewValue));

		public bool IsMenuEnabled
		{
			get { return (bool)GetValue(IsMenuEnabledProperty); }
			set { SetValue(IsMenuEnabledProperty, value); }
		}

		public static readonly DependencyProperty IsMenuEnabledProperty = DependencyProperty.Register(
			"IsMenuEnabled",
			typeof(bool),
			typeof(ApplicationBar),
			new PropertyMetadata(true, (s, e) => ((ApplicationBar)s)._applicationBar.IsMenuEnabled = (bool)e.NewValue));

		public Color BackgroundColor
		{
			get { return (Color)GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}

		public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
			"BackgroundColor",
			typeof(Color),
			typeof(ApplicationBar),
			new PropertyMetadata(default(Color), (s, e) => ((ApplicationBar)s)._applicationBar.BackgroundColor = (Color)e.NewValue));

		public Color ForegroundColor
		{
			get { return (Color)GetValue(ForegroundColorProperty); }
			set { SetValue(ForegroundColorProperty, value); }
		}

		public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.Register(
			"ForegroundColor",
			typeof(Color),
			typeof(ApplicationBar),
			new PropertyMetadata(default(Color), (s, e) => ((ApplicationBar)s)._applicationBar.ForegroundColor = (Color)e.NewValue));

		public ApplicationBarMode Mode
		{
			get { return (ApplicationBarMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
			"Mode",
			typeof(ApplicationBarMode),
			typeof(ApplicationBar),
			new PropertyMetadata(default(ApplicationBarMode), (s, e) => ((ApplicationBar)s)._applicationBar.Mode = (ApplicationBarMode)e.NewValue));

		public IList Buttons
		{
			get { return _buttons; }
		}

		private readonly ObservableCollection<ApplicationBarIconButton> _buttons;

		public IList MenuItems
		{
			get { return _menuItems; }
		}

		private readonly ObservableCollection<ApplicationBarMenuItem> _menuItems;

		private static readonly DependencyProperty _dataContextProperty = DependencyProperty.Register(
			DataContextPropertyName,
			typeof(object),
			typeof(ApplicationBar),
			null);
		private const string DataContextPropertyName = "DataContext";

		#endregion


		#region Event Handling

		/// <summary>
		/// Called after the behavior is attached to an AssociatedObject.
		/// </summary>
		protected override void OnAttached()
		{
			// Binds the data context of the associated object to this behavior, so that the data context can be passed onto the buttons and menu items.
			BindingOperations.SetBinding(this, _dataContextProperty, new Binding(DataContextPropertyName) { Source = AssociatedObject });
			AssociatedObject.ApplicationBar = _applicationBar;
			base.OnAttached();
		}

		/// <summary>
		/// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
		/// </summary>
		protected override void OnDetaching()
		{
			_applicationBar = null;
			base.OnDetaching();
		}

		/// <summary>
		/// Called when items in the <see cref="Buttons"/> collection have changed.
		/// </summary>
		private void OnButtonsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
#if DEBUG
			if (DesignerProperties.IsInDesignTool)
			{
				return;
			}
#endif
			var buttons = _applicationBar.Buttons;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					buttons.AddRange(e.NewItems.Cast<ApplicationBarIconButton>().Select(b =>
					{
						BindingOperations.SetBinding(b, FrameworkElement.DataContextProperty, new Binding(DataContextPropertyName) { Source = this });
						return b.Item;
					}));
					break;
				case NotifyCollectionChangedAction.Remove:
					buttons.RemoveRange(e.NewItems.Cast<ApplicationBarIconButton>().Select(b => b.Item));
					break;
				case NotifyCollectionChangedAction.Replace:
					throw new NotImplementedException();
				case NotifyCollectionChangedAction.Reset:
					buttons.Clear();
					break;
			}
		}

		/// <summary>
		/// Called when items in the <see cref="MenuItems"/> collection have changed.
		/// </summary>
		private void OnMenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
#if DEBUG
			if (DesignerProperties.IsInDesignTool)
			{
				return;
			}
#endif
			var items = _applicationBar.MenuItems;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					items.AddRange(e.NewItems.Cast<ApplicationBarMenuItem>().Select(i =>
					{
						BindingOperations.SetBinding(i, FrameworkElement.DataContextProperty, new Binding(DataContextPropertyName) { Source = this });
						return i.Item;
					}));
					break;
				case NotifyCollectionChangedAction.Remove:
					items.RemoveRange(e.NewItems.Cast<ApplicationBarMenuItem>().Select(b => b.Item));
					break;
				case NotifyCollectionChangedAction.Replace:
					throw new NotImplementedException();
				case NotifyCollectionChangedAction.Reset:
					items.Clear();
					break;
			}
		}

		#endregion
	}
}
