using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Codify.Extensions;

namespace Codify.Controls
{
	public class PanelExtension
	{
		#region Private Members

		private Panel _panel;
		private readonly ObservableCollection<object> _delayItemsQueue;

		#endregion


		#region Constructors

		public PanelExtension()
		{
			_delayItemsQueue = new ObservableCollection<object>();
			_delayItemsQueue.CollectionChanged += OnDelayItemsQueueCollectionChanged;
		}

		#endregion


		#region Private Property

		private DispatcherTimer Timer
		{
			get
			{
				if (_timer == null)
				{
					var delay = GetDelay(_panel);
					_timer = new DispatcherTimer { Interval = delay };
					_timer.Tick += OnTimerTick;
				}
				return _timer;
			}
		}

		private DispatcherTimer _timer;

		#endregion


		#region Attached Properties


		#region Items Source

		public static INotifyCollectionChanged GetItemsSource(DependencyObject obj)
		{
			return (INotifyCollectionChanged)obj.GetValue(ItemsSourceProperty);
		}

		public static void SetItemsSource(DependencyObject obj, INotifyCollectionChanged value)
		{
			obj.SetValue(ItemsSourceProperty, value);
		}

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached(
			"ItemsSource",
			typeof(INotifyCollectionChanged),
			typeof(PanelExtension),
			new PropertyMetadata(default(INotifyCollectionChanged), OnItemsSourceChanged));

		#endregion


		#region Extension

		private static PanelExtension GetExtension(DependencyObject obj)
		{
			return (PanelExtension)obj.GetValue(_extensionProperty);
		}

		private static void SetExtension(DependencyObject obj, PanelExtension value)
		{
			obj.SetValue(_extensionProperty, value);
		}

		private static readonly DependencyProperty _extensionProperty = DependencyProperty.RegisterAttached(
			"Extension",
			typeof(PanelExtension),
			typeof(PanelExtension),
			new PropertyMetadata(default(PanelExtension), OnExtensionChanged));

		#endregion


		#region ItemTemplate

		public static DataTemplate GetItemTemplate(DependencyObject obj)
		{
			return (DataTemplate)obj.GetValue(ItemTemplateProperty);
		}

		public static void SetItemTemplate(DependencyObject obj, DataTemplate value)
		{
			obj.SetValue(ItemTemplateProperty, value);
		}

		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.RegisterAttached(
			"ItemTemplate",
			typeof(DataTemplate),
			typeof(PanelExtension),
			null);

		#endregion


		#region Delay

		public static TimeSpan GetDelay(DependencyObject obj)
		{
			return (TimeSpan)obj.GetValue(DelayProperty);
		}

		public static void SetDelay(DependencyObject obj, TimeSpan value)
		{
			obj.SetValue(DelayProperty, value);
		}

		public static readonly DependencyProperty DelayProperty = DependencyProperty.RegisterAttached(
			"Delay",
			typeof(TimeSpan),
			typeof(PanelExtension),
			new PropertyMetadata(default(TimeSpan), OnDelayChanged));

		#endregion


		#endregion


		#region Event Handling

		private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var panel = sender as Panel;
			if (panel == null)
			{
				throw new InvalidOperationException("Codify.Controls.PanelExtension.ItemsSource property can only be attached to a System.Windows.Controls.Panel.");
			}

			var ext = GetExtension(panel);
			if (ext == null)
			{
				SetExtension(panel, ext = new PanelExtension());
			}

			var oldSource = e.OldValue as INotifyCollectionChanged;
			var newSource = e.NewValue as INotifyCollectionChanged;

			if (oldSource != null)
			{
				oldSource.CollectionChanged -= ext.OnItemsSourceCollectionChanged;
				panel.Children.Clear();
				ext._delayItemsQueue.Clear();
			}
			if (newSource != null)
			{
				newSource.CollectionChanged += ext.OnItemsSourceCollectionChanged;
				ext.AddItems(newSource as IEnumerable);
			}
		}

		private static void OnExtensionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var panel = (Panel)sender;
			var oldExt = (PanelExtension)e.OldValue;
			var newExt = (PanelExtension)e.NewValue;
			if (oldExt != null)
			{
				oldExt._panel = null;
			}
			if (newExt != null)
			{
				newExt._panel = panel;
			}
		}

		private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_panel == null)
			{
				return;
			}
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						var delay = GetDelay(_panel);
						if (delay == default(TimeSpan))
						{
							AddItems(e.NewItems);
						}
						else
						{
							var itemsSource = GetItemsSource(_panel) as IEnumerable;
							if (itemsSource != null)
							{

							}
							_delayItemsQueue.AddRange(e.NewItems);
						}
						break;
					}
				case NotifyCollectionChangedAction.Remove:
					{
						_delayItemsQueue.RemoveRange(e.OldItems);
						var oldItems = e.OldItems;
						var children = _panel.Children;
						var uiChildren = children.Where(elm => elm is FrameworkElement).Cast<FrameworkElement>().ToList();
						if (!uiChildren.Any())
						{
							return;
						}
						foreach (var child in from object item in oldItems from child in uiChildren where child.DataContext == item select child)
						{
							children.Remove(child);
						}
						break;
					}
				case NotifyCollectionChangedAction.Reset:
					_delayItemsQueue.Clear();
					_panel.Children.Clear();
					break;
			}
		}

		private static void OnDelayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var delayCollection = GetExtension(sender);
			if (delayCollection == null)
			{
				return;
			}
			delayCollection.Timer.Interval = (TimeSpan)e.NewValue;
		}

		private void OnTimerTick(object sender, EventArgs eventArgs)
		{
			AddNextItemInQueue();
		}

		private void OnDelayItemsQueueCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var timer = Timer;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					// Do not delay the first item.
					var first = e.NewItems[0];
					if (first == _delayItemsQueue[0])
					{
						AddItems(new[] { first });
					}
					// Start the timer for the rest of the items.
					timer.Start();
					break;
				case NotifyCollectionChangedAction.Remove:
					if (_delayItemsQueue.Count == 0)
					{
						timer.Stop();
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					timer.Stop();
					break;
			}
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Converts the data items to UIElements according to the <see cref="ItemTemplateProperty"/> and add them to the panel.
		/// </summary>
		/// <param name="items">The items.</param>
		private void AddItems(IEnumerable items)
		{
			if (items == null)
			{
				return;
			}
			var template = GetItemTemplate(_panel);
			if (template == null)
			{
				return;
			}
			foreach (var item in items)
			{
				var element = template.LoadContent() as UIElement;
				if (element == null)
				{
					return;
				}
				var frameworkElement = element as FrameworkElement;
				if (frameworkElement != null)
				{
					frameworkElement.DataContext = item;
				}
				_panel.Children.Add(element);
			}
		}

		private void AddNextItemInQueue()
		{
			if (_delayItemsQueue.Count < 1)
			{
				_timer.Stop();
			}
			if (_delayItemsQueue.Count == 0)
			{
				return;
			}
			var item = _delayItemsQueue[0];
			_delayItemsQueue.RemoveAt(0);
			if (_panel.Children.Where(child => child is FrameworkElement).Cast<FrameworkElement>().Any(child => child.DataContext == item))
			{
				AddNextItemInQueue();
			}
			else
			{
				AddItems(new[] { item });
			}
		}

		#endregion
	}
}
