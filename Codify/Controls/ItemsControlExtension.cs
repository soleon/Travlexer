using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Codify.Extensions;

namespace Codify.Controls
{
    public class ItemsControlExtension
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
                    _timer = new DispatcherTimer {Interval = delay};
                    _timer.Tick += OnTimerTick;
                }
                return _timer;
            }
        }

        private DispatcherTimer _timer;

        #endregion


        #region Constructors

        public ItemsControlExtension()
        {
            _delayItemsQueue = new ObservableCollection<object>();
            _delayItemsQueue.CollectionChanged += OnDelayItemsQueueCollectionChanged;
        }

        #endregion


        #region Attached Properties


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
            typeof (ItemsControlExtension),
            new PropertyMetadata(null, OnItemsSourceChanged));

        #endregion


        #region DelayCollection

        private static ItemsControlExtension GetDelayCollection(DependencyObject obj)
        {
            return (ItemsControlExtension) obj.GetValue(_delayCollectionProperty);
        }

        private static void SetDelayCollection(DependencyObject obj, ItemsControlExtension value)
        {
            obj.SetValue(_delayCollectionProperty, value);
        }

        private static readonly DependencyProperty _delayCollectionProperty = DependencyProperty.RegisterAttached(
            "DelayCollection",
            typeof (ItemsControlExtension),
            typeof (ItemsControlExtension),
            new PropertyMetadata(default(ItemsControlExtension), OnDelayCollectionChanged));

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
            typeof (ItemsControlExtension),
            new PropertyMetadata(default(TimeSpan), OnDelayChanged));

        #endregion


        #endregion


        #region Event Handling

        private static void OnDelayCollectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var oldDelayCollection = e.OldValue as ItemsControlExtension;
            if (oldDelayCollection != null)
            {
                oldDelayCollection.Detach();
            }

            var newDelayCollection = e.NewValue as ItemsControlExtension;
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
                throw new InvalidOperationException("Codify.Controls.ItemsControlExtension.ItemsSource property can only be attached to a System.Windows.Controls.ItemsControl.");
            }

            var delayCollection = GetDelayCollection(control);
            if (delayCollection == null)
            {
                SetDelayCollection(control, delayCollection = new ItemsControlExtension());
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
                    _delayItemsQueue.Clear();
                    _controlItemsSource.Clear();
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
            delayCollection.Timer.Interval = (TimeSpan) e.NewValue;
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
                        _controlItemsSource.Add(first);
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
            }
        }

        private void OnTimerTick(object sender, EventArgs eventArgs)
        {
            AddNextItem();
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

        private void AddNextItem()
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
            if (_controlItemsSource.Contains(item))
            {
                AddNextItem();
            }
            else
            {
                _controlItemsSource.Add(item);
            }
        }

        #endregion
    }
}