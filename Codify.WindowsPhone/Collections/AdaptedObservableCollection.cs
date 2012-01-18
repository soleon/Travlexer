using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Codify.WindowsPhone.Collections
{
	public class AdaptedObservableCollection<TSource, TTarget> : ObservableCollection<TTarget>
	{
		#region Private Members

		/// <summary>
		/// A reference to the function used to wrap source items.
		/// </summary>
		private readonly Func<TSource, TTarget> _converterFunction;

		/// <summary>
		/// A reference to the function used to determine if the source item passes the condition.
		/// </summary>
		private readonly Func<TSource, bool> _conditionFunction;

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets or sets the source collection to bind to.
		/// </summary>
		public INotifyCollectionChanged Source
		{
			get { return _source; }
			set
			{
				if (_source == value)
				{
					return;
				}

				var newValue = value as IList<TSource>;

				if (value != null && newValue == null)
				{
					throw new InvalidOperationException("Source collection must be an observable IList<TSource>. Use ObservableCollection<TSource> or ReadOnlyObservableCollection<TSource>.");
				}

				var oldValue = _source;
				_source = value;
				OnSourceChanged(oldValue, value);
			}
		}

		private INotifyCollectionChanged _source;

		#endregion


		#region Constructors

		/// <summary>
		/// Creates a new instance of <see cref="AdaptedObservableCollection{TSource, TTarget}"/>.
		/// </summary>
		/// <param name="converterFunction">The function used to convert <see cref="TSource"/> items to <see cref="TTarget"/> items.</param>
		/// <param name="source">The observable collection of <see cref="TSource"/> items.</param>
		public AdaptedObservableCollection(Func<TSource, TTarget> converterFunction, INotifyCollectionChanged source = null)
			: this(converterFunction, null, source) { }

		/// <summary>
		/// Creates a new instance of <see cref="AdaptedObservableCollection{TSource, TTarget}"/>.
		/// </summary>
		/// <param name="converterFunction">The function used to convert <see cref="TSource"/> items to <see cref="TTarget"/> items.</param>
		/// <param name="conditionFunction">The function used to filter <see cref="TSource"/> items.</param>
		/// <param name="source">The observable collection of <see cref="TSource"/> items.</param>
		public AdaptedObservableCollection(Func<TSource, TTarget> converterFunction, Func<TSource, bool> conditionFunction, INotifyCollectionChanged source = null)
		{
			_converterFunction = converterFunction;
			_conditionFunction = conditionFunction;
			Source = source;
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Determines if the given <see cref="TSource"/> item is part of the adapted set.
		/// </summary>
		/// <param name="item"></param>
		/// <returns>True if filter passes or does not exist, otherwise false.</returns>
		private bool CheckSourceItem(TSource item)
		{
			return _conditionFunction == null || _conditionFunction(item);
		}

		#endregion


		#region Event Handling

		/// <summary>
		/// Called when the source is changed.
		/// </summary>
		/// <param name="oldSource">The old source value.</param>
		/// <param name="newSource">The new source value.</param>
		private void OnSourceChanged(INotifyCollectionChanged oldSource, INotifyCollectionChanged newSource)
		{
			// Unbind old source
			if (oldSource != null)
			{
				oldSource.CollectionChanged -= OnSourceItemCollectionChanged;
			}


			// Notify a source collection reset
			OnSourceReset();


			// Don't do anything if new source is null
			if (newSource == null)
			{
				return;
			}


			// Sync immediately
			OnSourceItemsAdded(0, (IList)newSource);


			// Bind new source collection
			newSource.CollectionChanged += OnSourceItemCollectionChanged;
		}

		/// <summary>
		/// Called when the source items have changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSourceItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					OnSourceItemsAdded(e.NewStartingIndex, e.NewItems);
					break;

				case NotifyCollectionChangedAction.Remove:
					OnSourceItemsRemoved(e.OldStartingIndex, e.OldItems);
					break;

				case NotifyCollectionChangedAction.Replace:
					OnSourceItemReplaced(e.NewStartingIndex, (TSource)e.NewItems[0]);
					break;

				case NotifyCollectionChangedAction.Reset:
					OnSourceReset();
					break;
			}
		}

		/// <summary>
		/// Called when a collection of items is added in the source collection.
		/// </summary>
		/// <param name="startingIndex">The index to insert the collection.</param>
		/// <param name="items">The new items.</param>
		private void OnSourceItemsAdded(int startingIndex, IList items)
		{
			var len = items.Count;
			var currentIndex = startingIndex;

			for (var i = 0; i < len; i++)
			{
				var sourceItem = (TSource)items[i];

				if (!CheckSourceItem(sourceItem))
				{
					continue;
				}

				var targetItem = _converterFunction((TSource)items[i]);
				Insert(currentIndex, targetItem);
				currentIndex++;
			}
		}

		/// <summary>
		/// Called when a collection of items is removed from the source collection.
		/// </summary>
		/// <param name="startingIndex">The index of the first item.</param>
		/// <param name="items">The old items.</param>
		private void OnSourceItemsRemoved(int startingIndex, IList items)
		{
			var len = items.Count;

			for (var i = 0; i < len; i++)
			{
				var sourceItem = (TSource)items[i];

				if (!CheckSourceItem(sourceItem))
				{
					continue;
				}

				RemoveAt(startingIndex);
			}
		}

		/// <summary>
		/// Called when an item is replaced in the source collection.
		/// </summary>
		/// <param name="index">The index of the item.</param>
		/// <param name="newItem">The new source item.</param>
		private void OnSourceItemReplaced(int index, TSource newItem)
		{
			if (CheckSourceItem(newItem))
			{
				var targetItem = _converterFunction(newItem);
				this[index] = targetItem;
			}
			else
			{
				RemoveAt(index);
			}
		}

		/// <summary>
		/// Called when the source collection is reset.
		/// </summary>
		private void OnSourceReset()
		{
			Clear();
		}

		#endregion
	}
}
