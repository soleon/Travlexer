using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Codify.WindowsPhone.Extensions;

namespace Codify.WindowsPhone.Collections
{
	public class DelayCollection
	{
		#region Private Members

		private ItemsControl _control;
		private ObservableCollection<object> _controlItemsSource; 
		private readonly ObservableCollection<object> _delayItemsQueue;

		#endregion


		#region Private Property

		private DispatcherTimer Timer
		{
			get
			{
				if (_timer == null)
				{
					var delay = GetDelay(_control);
					_timer = new DispatcherTimer { Interval = delay };
					_timer.Tick += OnTimerTick;
				}
				return _timer;
			}
		}

		private DispatcherTimer _timer;

		#endregion


		#region Constructors


		public DelayCollection()
		{
			_delayItemsQueue = new ObservableCollection<object>();
			_delayItemsQueue.CollectionChanged += OnDelayItemsQueueCollectionChanged;
		}


		#endregion


		#region Public Properties


		#region ItemsSource

		public static INotifyCollectionChanged GetItemsSource(DependencyObject obj)
		{
			return (INotifyCollectionChanged) obj.GetValue(ItemsSourceProperty);
		}

		public static void SetItemsSource(DependencyObject obj, INotifyCollectionChanged value)
		{
			obj.SetValue(ItemsSourceProperty, value);
		}

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached(
			"ItemsSource",
			typeof (INotifyCollectionChanged),
			typeof (DelayCollection),
			new PropertyMetadata(null, OnItemsSourceChanged));

		#endregion


		#region DelayCollection

		private static DelayCollection GetDelayCollection(DependencyObject obj)
		{
			return (DelayCollection) obj.GetValue(_delayCollectionProperty);
		}

		private static void SetDelayCollection(DependencyObject obj, DelayCollection value)
		{
			obj.SetValue(_delayCollectionProperty, value);
		}

		private static readonly DependencyProperty _delayCollectionProperty = DependencyProperty.RegisterAttached(
			"DelayCollection",
			typeof (DelayCollection),
			typeof (DelayCollection),
			new PropertyMetadata(default(DelayCollection), OnDelayCollectionChanged));

		#endregion


		#region Delay

		public static TimeSpan GetDelay(DependencyObject obj)
		{
			return (TimeSpan) obj.GetValue(DelayProperty);
		}

		public static void SetDelay(DependencyObject obj, TimeSpan value)
		{
			obj.SetValue(DelayProperty, value);
		}

		public static readonly DependencyProperty DelayProperty = DependencyProperty.RegisterAttached(
			"Delay",
			typeof (TimeSpan),
			typeof (DelayCollection),
			new PropertyMetadata(default(TimeSpan), OnDelayChanged));

		#endregion


		#endregion


		#region Event Handling

		private static void OnDelayCollectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var oldDelayCollection = e.OldValue as DelayCollection;
			if (oldDelayCollection != null)
			{
				oldDelayCollection.Detach();
			}

			var newDelayCollection = e.NewValue as DelayCollection;
			if (newDelayCollection != null)
			{
				newDelayCollection.Attach(sender as ItemsControl);
			}
		}

		private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var control = sender as ItemsControl;
			if (control == null)
			{
				return;
			}

			var delayCollection = GetDelayCollection(control);
			if (delayCollection == null)
			{
				delayCollection = new DelayCollection();
				SetDelayCollection(control, delayCollection);
			}
			delayCollection.Refersh();
		}

		private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					_delayItemsQueue.AddRange(e.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove:
					_delayItemsQueue.RemoveRange(e.OldItems);
					_controlItemsSource.RemoveRange(e.OldItems);
					break;
				case NotifyCollectionChangedAction.Replace:
					var oldItems = e.OldItems;
					var newItems = e.NewItems;
					for (var i = 0; i < e.OldItems.Count; i++)
					{
						var oldItem = oldItems[i];
						var newItem = newItems[i];
						var index = _delayItemsQueue.IndexOf(oldItem);
						_delayItemsQueue[index] = newItem;
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					var itemsSource = GetItemsSource(_control) as IEnumerable;
					_delayItemsQueue.Clear();
					_delayItemsQueue.AddRange(itemsSource);
					break;
			}
		}

		private static void OnDelayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var delayCollection = GetDelayCollection(sender);
			if (delayCollection == null)
			{
				return;
			}
			delayCollection.Timer.Interval = (TimeSpan)e.NewValue;
		}

		private void OnDelayItemsQueueCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var timer = Timer;
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					timer.Start();
					break;
				case NotifyCollectionChangedAction.Remove:
					if (_delayItemsQueue.Count == 0)
					{
						timer.Stop();
					}
					break;
			}
		}

		private void OnTimerTick(object sender, EventArgs eventArgs)
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
			_controlItemsSource.Add(item);
		}

		#endregion


		#region Private Methods

		private void Refersh()
		{
			var itemsSource = GetItemsSource(_control) as IEnumerable;
			if (itemsSource == null)
			{
				return;
			}
			_delayItemsQueue.Clear();
			if (_controlItemsSource == null)
			{
				return;
			}
			_controlItemsSource.Clear();
			_controlItemsSource.AddRange(itemsSource);
		}

		private void Detach()
		{
			if (_control == null)
			{
				return;
			}
			var itemsSource = GetItemsSource(_control);
			_controlItemsSource = null;
			_control = null;
			if (itemsSource == null)
			{
				return;
			}
			itemsSource.CollectionChanged -= OnItemsSourceCollectionChanged;
		}

		private void Attach(ItemsControl control)
		{
			if (control == null)
			{
				return;
			}
			_control = control;
			_control.ItemsSource = _controlItemsSource = new ObservableCollection<object>();
			var itemsSource = GetItemsSource(control);
			if (itemsSource == null)
			{
				return;
			}
			itemsSource.CollectionChanged += OnItemsSourceCollectionChanged;
		}

		#endregion
	}
}
