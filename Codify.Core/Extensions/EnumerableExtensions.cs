using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Codify.Extensions
{
	public static class EnumerationExtensions
	{
		/// <summary>
		/// Determines whether the specified enumerable is null or empty.
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			return enumerable == null || !enumerable.Any();
		}

		/// <summary>
		/// Performs the specified action on each element of the <see cref="enumerable"/>.
		/// </summary>
		/// <typeparam name="T">Type of the elements in the enumerable.</typeparam>
		/// <param name="enumerable">The enuerable to perform the action on.</param>
		/// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="enumerable"/>.</param>
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var i in enumerable)
			{
				action(i);
			}
		}

		/// <summary>
		/// Adds multiple items to a list.
		/// </summary>
		/// <param name="list">The list to add the items.</param>
		/// <param name="items">The items to be added.</param>
		public static void AddRange(this IList list, IEnumerable items)
		{
			if (items == null)
			{
				return;
			}
			foreach (var i in items)
			{
				list.Add(i);
			}
		}

		/// <summary>
		/// Removes multiple items from a list.
		/// </summary>
		/// <param name="list">The list to remove the items.</param>
		/// <param name="items">The items to be removed.</param>
		public static void RemoveRange(this IList list, IEnumerable items)
		{
			if (items == null)
			{
				return;
			}
			foreach (var i in items)
			{
				list.Remove(i);
			}
		}
	}
}
