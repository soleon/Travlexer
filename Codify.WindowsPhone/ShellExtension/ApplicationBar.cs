using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Codify.Extensions;
using Microsoft.Phone.Shell;

namespace Codify.WindowsPhone.ShellExtension
{
	/// <summary>
	/// A dependency wrapper to Microsoft.Phone.Shell.ApplicationBar, enabling several dependency object behaviors to the application bar.
	/// </summary>
	public class ApplicationBar : DependencyObject
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


		#region Dependency Properties


		#region IsVisible

		public bool IsVisible
		{
			get { return (bool) GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(
			"IsVisible",
			typeof (bool),
			typeof (ApplicationBar),
			new PropertyMetadata(true, (s, e) => ((ApplicationBar) s)._applicationBar.IsVisible = (bool) e.NewValue));

		#endregion


		#region Opacity

		public double Opacity
		{
			get { return (double) GetValue(OpacityProperty); }
			set { SetValue(OpacityProperty, value); }
		}

		public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(
			"Opacity",
			typeof (double),
			typeof (ApplicationBar),
			new PropertyMetadata(1D, (s, e) => ((ApplicationBar) s)._applicationBar.Opacity = (double) e.NewValue));

		#endregion


		#region IsMenuEnabled

		public bool IsMenuEnabled
		{
			get { return (bool) GetValue(IsMenuEnabledProperty); }
			set { SetValue(IsMenuEnabledProperty, value); }
		}

		public static readonly DependencyProperty IsMenuEnabledProperty = DependencyProperty.Register(
			"IsMenuEnabled",
			typeof (bool),
			typeof (ApplicationBar),
			new PropertyMetadata(true, (s, e) => ((ApplicationBar) s)._applicationBar.IsMenuEnabled = (bool) e.NewValue));

		#endregion


		#region BackgroundColor

		public Color BackgroundColor
		{
			get { return (Color) GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}

		public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
			"BackgroundColor",
			typeof (Color),
			typeof (ApplicationBar),
			new PropertyMetadata(default(Color), (s, e) => ((ApplicationBar) s)._applicationBar.BackgroundColor = (Color) e.NewValue));

		#endregion


		#region ForegroundColor

		public Color ForegroundColor
		{
			get { return (Color) GetValue(ForegroundColorProperty); }
			set { SetValue(ForegroundColorProperty, value); }
		}

		public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.Register(
			"ForegroundColor",
			typeof (Color),
			typeof (ApplicationBar),
			new PropertyMetadata(default(Color), (s, e) => ((ApplicationBar) s)._applicationBar.ForegroundColor = (Color) e.NewValue));

		#endregion


		#region Mode

		public ApplicationBarMode Mode
		{
			get { return (ApplicationBarMode) GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
			"Mode",
			typeof (ApplicationBarMode),
			typeof (ApplicationBar),
			new PropertyMetadata(default(ApplicationBarMode), (s, e) => ((ApplicationBar) s)._applicationBar.Mode = (ApplicationBarMode) e.NewValue));

		#endregion


		#region ButtonItemsSource

		public IEnumerable ButtonItemsSource
		{
			get { return (IEnumerable) GetValue(ButtonItemsSourceProperty); }
			set { SetValue(ButtonItemsSourceProperty, value); }
		}

		/// <summary>
		/// Defines the <see cref="ButtonItemsSource"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ButtonItemsSourceProperty = DependencyProperty.Register(
			"ButtonItemsSource",
			typeof (IEnumerable),
			typeof (ApplicationBar),
			new PropertyMetadata(default(IEnumerable), (s, e) => ((ApplicationBar) s).OnButtonItemsSourceChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue)));

		#endregion


		#region ButtonIconUriMemberPath

		public string ButtonIconUriMemberPath
		{
			get { return (string) GetValue(ButtonIconUriMemberPathProperty); }
			set { SetValue(ButtonIconUriMemberPathProperty, value); }
		}

		public static readonly DependencyProperty ButtonIconUriMemberPathProperty = DependencyProperty.Register(
			"ButtonIconUriMemberPath",
			typeof (string),
			typeof (ApplicationBar),
			null);

		#endregion


		#region ButtonIsEnabledMemberPath

		public string ButtonIsEnabledMemberPath
		{
			get { return (string) GetValue(ButtonIsEnabledMemberPathProperty); }
			set { SetValue(ButtonIsEnabledMemberPathProperty, value); }
		}

		public static readonly DependencyProperty ButtonIsEnabledMemberPathProperty = DependencyProperty.Register(
			"ButtonIsEnabledMemberPath",
			typeof (string),
			typeof (ApplicationBar),
			null);

		#endregion


		#region ButtonTextMemberPath

		public string ButtonTextMemberPath
		{
			get { return (string) GetValue(ButtonTextMemberPathProperty); }
			set { SetValue(ButtonTextMemberPathProperty, value); }
		}

		public static readonly DependencyProperty ButtonTextMemberPathProperty = DependencyProperty.Register(
			"ButtonTextMemberPath",
			typeof (string),
			typeof (ApplicationBar),
			null);

		#endregion


		#region ButtonCommandMemberPath

		public string ButtonCommandMemberPath
		{
			get { return (string) GetValue(ButtonCommandMemberPathProperty); }
			set { SetValue(ButtonCommandMemberPathProperty, value); }
		}

		public static readonly DependencyProperty ButtonCommandMemberPathProperty = DependencyProperty.Register(
			"ButtonCommandMemberPath",
			typeof (string),
			typeof (ApplicationBar),
			null);

		#endregion


		#region ButtonCommandParameterMemberPath

		public string ButtonCommandParameterMemberPath
		{
			get { return (string) GetValue(ButtonCommandParameterMemberPathProperty); }
			set { SetValue(ButtonCommandParameterMemberPathProperty, value); }
		}

		public static readonly DependencyProperty ButtonCommandParameterMemberPathProperty = DependencyProperty.Register(
			"ButtonCommandParameterMemberPath",
			typeof (string),
			typeof (ApplicationBar),
			null);

		#endregion


		#region MenuItemIsEnabledMemberPath

		public string MenuItemIsEnabledMemberPath
		{
			get { return (string) GetValue(MenuItemIsEnabledMemberPathProperty); }
			set { SetValue(MenuItemIsEnabledMemberPathProperty, value); }
		}

		public static readonly DependencyProperty MenuItemIsEnabledMemberPathProperty = DependencyProperty.Register(
			"MenuItemIsEnabledMemberPath",
			typeof (string),
			typeof (ApplicationBar),
			null);

		#endregion


		#region MenuItemTextMemberPath

		public string MenuItemTextMemberPath
		{
			get { return (string) GetValue(MenuItemTextMemberPathProperty); }
			set { SetValue(MenuItemTextMemberPathProperty, value); }
		}

		public static readonly DependencyProperty MenuItemTextMemberPathProperty = DependencyProperty.Register(
			"MenuItemTextMemberPath",
			typeof (string),
			typeof (ApplicationBar),
			null);

		#endregion


		#region MenuItemCommandMemberPath

		public string MenuItemCommandMemberPath
		{
			get { return (string) GetValue(MenuItemCommandMemberPathProperty); }
			set { SetValue(MenuItemCommandMemberPathProperty, value); }
		}

		public static readonly DependencyProperty MenuItemCommandMemberPathProperty = DependencyProperty.Register(
			"MenuItemCommandMemberPath",
			typeof (string),
			typeof (ApplicationBar),
			null);

		#endregion


		#region MenuItemCommandParameterMemberPath

		public string MenuItemCommandParameterMemberPath
		{
			get { return (string) GetValue(MenuItemCommandParameterMemberPathProperty); }
			set { SetValue(MenuItemCommandParameterMemberPathProperty, value); }
		}

		public static readonly DependencyProperty MenuItemCommandParameterMemberPathProperty = DependencyProperty.Register(
			"MenuItemCommandParameterMemberPath",
			typeof (string),
			typeof (ApplicationBar),
			null);

		#endregion


		#region MenuItemsSource

		public IEnumerable MenuItemsSource
		{
			get { return (IEnumerable) GetValue(MenuItemsSourceProperty); }
			set { SetValue(MenuItemsSourceProperty, value); }
		}

		/// <summary>
		/// Defines the <see cref="MenuItemsSource"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty MenuItemsSourceProperty = DependencyProperty.Register(
			"MenuItemsSource",
			typeof (IEnumerable),
			typeof (ApplicationBar),
			new PropertyMetadata(default(IEnumerable), (s, e) => ((ApplicationBar) s).OnMenuItemsSourceChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue)));

		#endregion


		#endregion


		#region Public Properties

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
			typeof (object),
			typeof (ApplicationBar),
			null);

		private const string DataContextPropertyName = "DataContext";

		#endregion


		#region Attached Properties

		public static ApplicationBar GetApplicationBar(DependencyObject obj)
		{
			return (ApplicationBar) obj.GetValue(ApplicationBarProperty);
		}

		public static void SetApplicationBar(DependencyObject obj, ApplicationBar value)
		{
			obj.SetValue(ApplicationBarProperty, value);
		}

		public static readonly DependencyProperty ApplicationBarProperty = DependencyProperty.RegisterAttached(
			"ApplicationBar",
			typeof (ApplicationBar),
			typeof (ApplicationBar),
			new PropertyMetadata(default(ApplicationBar), OnApplicationBarChanged));

		private static void OnApplicationBarChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var page = sender as Microsoft.Phone.Controls.PhoneApplicationPage;
			if (page == null)
			{
				throw new InvalidOperationException("Codify.WindowsPhone.Shell.ApplicationBar.ApplicationBar property can only be attached to a Microsoft.Phone.Controls.PhoneApplicationPage.");
			}
			var oldAppBar = (ApplicationBar) e.OldValue;
			if (oldAppBar != null)
			{
				oldAppBar.Detach();
			}
			var newAppBar = (ApplicationBar) e.NewValue;
			if (newAppBar != null)
			{
				newAppBar.Attach(page);
			}
		}

		#endregion


		#region Event Handling

		/// <summary>
		/// Called when items in the <see cref="Buttons"/> collection have changed.
		/// </summary>
		private void OnButtonsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var buttons = _applicationBar.Buttons;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					buttons.AddRange(e.NewItems.Cast<ApplicationBarIconButton>().Select(b =>
					{
						if (ButtonItemsSource == null)
						{
							BindingOperations.SetBinding(b, FrameworkElement.DataContextProperty, new Binding(DataContextPropertyName) { Source = this });
						}
						return b.Item;
					}));
					break;
				case NotifyCollectionChangedAction.Remove:
					buttons.RemoveRange(e.NewItems.Cast<ApplicationBarIconButton>().Select(b => b.Item));
					break;
				case NotifyCollectionChangedAction.Reset:
					buttons.Clear();
					break;
			}
		}

		/// <summary>
		/// Called when <see cref="ButtonItemsSource"/> changes.
		/// </summary>
		private void OnButtonItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			var source = oldValue as INotifyCollectionChanged;
			if (source != null)
			{
				source.CollectionChanged -= OnButtonItemsSourceCollectionChanged;
			}
			source = newValue as INotifyCollectionChanged;
			if (source != null)
			{
				source.CollectionChanged += OnButtonItemsSourceCollectionChanged;
			}
			_buttons.Clear();
			AddButtons(newValue);
		}

		/// <summary>
		/// Called when items in the <see cref="ButtonItemsSource"/> is changed.
		/// </summary>
		private void OnButtonItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					AddButtons(e.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (var item in e.OldItems)
					{
						for (var i = _buttons.Count - 1; i >= 0; i--)
						{
							if (_buttons[i].DataContext == item)
							{
								_buttons.RemoveAt(i);
							}
						}
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					var oldItems = e.OldItems;
					var newItems = e.NewItems;
					for (var i = 0; i < oldItems.Count; i++)
					{
						var oldItem = oldItems[i];
						var newItem = newItems[i];
						foreach (var button in _buttons.Where(b => b.DataContext == oldItem))
						{
							button.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = newItem });
							break;
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					_buttons.Clear();
					break;
			}
		}

		/// <summary>
		/// Called when items in the <see cref="MenuItems"/> collection have changed.
		/// </summary>
		private void OnMenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var items = _applicationBar.MenuItems;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					items.AddRange(e.NewItems.Cast<ApplicationBarMenuItem>().Select(i =>
					{
						if (MenuItemsSource == null)
						{
							BindingOperations.SetBinding(i, FrameworkElement.DataContextProperty, new Binding(DataContextPropertyName) { Source = this });
						}
						return i.Item;
					}));
					break;
				case NotifyCollectionChangedAction.Remove:
					items.RemoveRange(e.NewItems.Cast<ApplicationBarMenuItem>().Select(b => b.Item));
					break;
				case NotifyCollectionChangedAction.Reset:
					items.Clear();
					break;
			}
		}

		/// <summary>
		/// Called when <see cref="MenuItemsSource"/> changes.
		/// </summary>
		private void OnMenuItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			var source = oldValue as INotifyCollectionChanged;
			if (source != null)
			{
				source.CollectionChanged -= OnMenuItemsSourceCollectionChanged;
			}
			source = newValue as INotifyCollectionChanged;
			if (source != null)
			{
				source.CollectionChanged += OnMenuItemsSourceCollectionChanged;
			}
			_menuItems.Clear();
			AddMenuItems(newValue);
		}

		/// <summary>
		/// Called when items in the <see cref="MenuItemsSource"/> is changed.
		/// </summary>
		private void OnMenuItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					AddMenuItems(e.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (var item in e.OldItems)
					{
						for (var i = _menuItems.Count - 1; i >= 0; i--)
						{
							if (_menuItems[i].DataContext == item)
							{
								_menuItems.RemoveAt(i);
							}
						}
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					var oldItems = e.OldItems;
					var newItems = e.NewItems;
					for (var i = 0; i < oldItems.Count; i++)
					{
						var oldItem = oldItems[i];
						var newItem = newItems[i];
						foreach (var menuItem in _menuItems.Where(item => item.DataContext == oldItem))
						{
							menuItem.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = newItem });
							break;
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					_menuItems.Clear();
					break;
			}
		}

		#endregion


		#region Private Methods

		private void Attach(Microsoft.Phone.Controls.PhoneApplicationPage page)
		{
			// Binds the data context of the page to this application bar, so that the data context can be passed onto the buttons and menu items.
			BindingOperations.SetBinding(this, _dataContextProperty, new Binding(DataContextPropertyName) { Source = page });
			page.ApplicationBar = _applicationBar;
		}

		private void Detach()
		{
			_applicationBar = null;
		}

		/// <summary>
		/// Adds new application bar buttons from the specified items source.
		/// </summary>
		private void AddButtons(IEnumerable itemsSource)
		{
			if (itemsSource == null)
			{
				return;
			}
			foreach (var item in itemsSource)
			{
				var button = new ApplicationBarIconButton();
				button.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = item });
				if (ButtonIconUriMemberPath != null)
				{
					button.SetBinding(ApplicationBarIconButton.IconUriProperty, new Binding(ButtonIconUriMemberPath));
				}
				if (ButtonTextMemberPath != null)
				{
					button.SetBinding(ApplicationBarIconButton.TextProperty, new Binding(ButtonTextMemberPath));
				}
				if (ButtonIsEnabledMemberPath != null)
				{
					button.SetBinding(ApplicationBarIconButton.IsEnabledProperty, new Binding(ButtonIsEnabledMemberPath));
				}
				if (ButtonCommandMemberPath != null)
				{
					button.SetBinding(ApplicationBarIconButton.CommandProperty, new Binding(ButtonCommandMemberPath));
				}
				if (ButtonCommandParameterMemberPath != null)
				{
					button.SetBinding(ApplicationBarIconButton.CommandParameterProperty, new Binding(ButtonCommandParameterMemberPath));
				}
				_buttons.Add(button);
			}
		}

		/// <summary>
		/// Adds new application bar menu items from the specified items source.
		/// </summary>
		private void AddMenuItems(IEnumerable itemsSource)
		{
			if (itemsSource == null)
			{
				return;
			}
			foreach (var item in itemsSource)
			{
				var menuItem = new ApplicationBarMenuItem();
				menuItem.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = item });
				if (MenuItemTextMemberPath != null)
				{
					menuItem.SetBinding(ApplicationBarMenuItem.TextProperty, new Binding(MenuItemTextMemberPath));
				}
				if (MenuItemIsEnabledMemberPath != null)
				{
					menuItem.SetBinding(ApplicationBarMenuItem.IsEnabledProperty, new Binding(MenuItemIsEnabledMemberPath));
				}
				if (MenuItemCommandMemberPath != null)
				{
					menuItem.SetBinding(ApplicationBarMenuItem.CommandProperty, new Binding(MenuItemCommandMemberPath));
				}
				if (MenuItemCommandParameterMemberPath != null)
				{
					menuItem.SetBinding(ApplicationBarMenuItem.CommandParameterProperty, new Binding(MenuItemCommandParameterMemberPath));
				}
				_menuItems.Add(menuItem);
			}
		}

		#endregion
	}
}
