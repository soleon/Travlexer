using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Codify.Collections
{
    public class AdaptedObservableCollection<TSource, TTarget> : ObservableCollection<TTarget>
    {
        #region Private Members

        /// <summary>
        ///   A reference to the function used to wrap source items.
        /// </summary>
        private readonly Func<TSource, TTarget> _converterFunction;

        /// <summary>
        ///   A reference to the function used to determine if the source item passes the condition.
        /// </summary>
        private readonly Func<TSource, bool> _conditionFunction;

        /// <summary>
        ///   An internal list that keep track of the exact source items that maps 1 to 1 to target items in this collection.
        /// </summary>
        private readonly List<TSource> _sourceItems = new List<TSource>();

        #endregion


        #region Public Properties

        /// <summary>
        ///   Gets or sets the source collection to bind to.
        /// </summary>
        public INotifyCollectionChanged Source
        {
            get { return _source; }
            set
            {
                if (_source == value) return;

                var newValue = value as IList<TSource>;

                if (value != null && newValue == null)
                    throw new InvalidOperationException("Source collection must be an observable IList<TSource>. Use ObservableCollection<TSource> or ReadOnlyObservableCollection<TSource>.");

                var oldValue = _source;
                _source = value;
                OnSourceChanged(oldValue, value);
            }
        }

        private INotifyCollectionChanged _source;

        #endregion


        #region Constructors

        /// <summary>
        ///   Creates a new instance of <see cref="AdaptedObservableCollection{TSource, TTarget}" />.
        /// </summary>
        /// <param name="converterFunction"> The function used to convert <see cref="TSource" /> items to <see cref="TTarget" /> items. </param>
        /// <param name="source"> The observable collection of <see cref="TSource" /> items. </param>
        public AdaptedObservableCollection(Func<TSource, TTarget> converterFunction, INotifyCollectionChanged source = null)
            : this(converterFunction, null, source) { }

        /// <summary>
        ///   Creates a new instance of <see cref="AdaptedObservableCollection{TSource, TTarget}" />.
        /// </summary>
        /// <param name="converterFunction"> The function used to convert <see cref="TSource" /> items to <see cref="TTarget" /> items. </param>
        /// <param name="conditionFunction"> The function used to filter <see cref="TSource" /> items. </param>
        /// <param name="source"> The observable collection of <see cref="TSource" /> items. </param>
        public AdaptedObservableCollection(Func<TSource, TTarget> converterFunction, Func<TSource, bool> conditionFunction, INotifyCollectionChanged source = null)
        {
            _converterFunction = converterFunction;
            _conditionFunction = conditionFunction;
            Source = source;
        }

        #endregion


        #region Private Methods

        /// <summary>
        ///   Determines if the given <see cref="TSource" /> item is part of the adapted set.
        /// </summary>
        /// <param name="sourceItem"> </param>
        /// <returns> True if filter passes or does not exist, otherwise false. </returns>
        private bool IsValidItem(TSource sourceItem)
        {
            return _conditionFunction == null || _conditionFunction(sourceItem);
        }

        #endregion


        #region Event Handling

        /// <summary>
        ///   Called when the source is changed.
        /// </summary>
        /// <param name="oldSource"> The old source value. </param>
        /// <param name="newSource"> The new source value. </param>
        private void OnSourceChanged(INotifyCollectionChanged oldSource, INotifyCollectionChanged newSource)
        {
            // Unbind old source
            if (oldSource != null) oldSource.CollectionChanged -= OnSourceItemCollectionChanged;

            // Notify a source collection reset
            OnSourceReset();

            // Don't do anything if new source is null
            if (newSource == null) return;

            // Sync immediately
            OnSourceItemsAdded((IList)newSource);

            // Bind new source collection
            newSource.CollectionChanged += OnSourceItemCollectionChanged;
        }

        /// <summary>
        ///   Called when the source items have changed
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void OnSourceItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnSourceItemsAdded(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    OnSourceItemsRemoved(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    OnSourceItemReplaced((TSource)e.OldItems[0], (TSource)e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    OnSourceReset();
                    break;
            }
        }

        /// <summary>
        ///   Called when a collection of items is added in the source collection.
        /// </summary>
        /// <param name="items"> The new items. </param>
        private void OnSourceItemsAdded(IEnumerable items)
        {
            foreach (TSource sourceItem in items)
            {
                if (!IsValidItem(sourceItem)) continue;

                var index = DetermineIndex(sourceItem);
                var targetItem = _converterFunction(sourceItem);
                if (index >= Count)
                {
                    _sourceItems.Add(sourceItem);
                    Add(targetItem);
                }
                else
                {
                    _sourceItems.Insert(index, sourceItem);
                    InsertItem(index, targetItem);
                }
            }
        }


        /// <summary>
        ///   Determines the index of a new source collection item in this adapted collection.
        /// </summary>
        private int DetermineIndex(TSource newSourceItem)
        {
            // Cast source collection to IList<TSource>, because the IndexOf method is required.
            var source = (IList<TSource>)_source;

            // Get the index of the source item in the source collection.
            var sourceIndex = source.IndexOf(newSourceItem);

            // If there is no condition function or source item is either the first item or it does not exist in source collection,
            // just return the source index.
            if (_conditionFunction == null || sourceIndex <= 0) return sourceIndex;

            var previousItem = default(TSource);

            // Try to find the previous matching item in the source collection.
            for (var i = sourceIndex - 1; i >= 0; i--)
            {
                var item = source[i];
                if (!_conditionFunction(item)) continue;
                previousItem = item;
                break;
            }

            // If no previous matching item can be found, then the new source item should be the first item in this collection.
            if (Equals(previousItem, default(TSource))) return 0;

            // If a previous matching item is found, then the new source item should be the next item of this previous item.
            return _sourceItems.IndexOf(previousItem) + 1;
        }

        /// <summary>
        ///   Called when a collection of items is removed from the source collection.
        /// </summary>
        /// <param name="items"> The old items. </param>
        private void OnSourceItemsRemoved(IEnumerable items)
        {
            foreach (var index in from TSource sourceItem in items where IsValidItem(sourceItem) select _sourceItems.IndexOf(sourceItem))
            {
                RemoveItem(index);
                _sourceItems.RemoveAt(index);
            }
        }

        /// <summary>
        ///   Called when an item is replaced in the source collection.
        /// </summary>
        /// <param name="oldItem"> The old source item. </param>
        /// <param name="newItem"> The new source item. </param>
        private void OnSourceItemReplaced(TSource oldItem, TSource newItem)
        {
            var index = -_sourceItems.IndexOf(oldItem);
            if (IsValidItem(newItem))
            {
                var targetItem = _converterFunction(newItem);
                _sourceItems[index] = newItem;
                this[index] = targetItem;
            }
            else
            {
                _sourceItems.RemoveAt(index);
                RemoveItem(index);
            }
        }

        /// <summary>
        ///   Called when the source collection is reset.
        /// </summary>
        private void OnSourceReset()
        {
            Clear();
        }

        #endregion
    }
}