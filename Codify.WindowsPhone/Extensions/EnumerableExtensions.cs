using System;
using System.Collections.Generic;
using System.Linq;

namespace Codify.WindowsPhone.Extensions
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
		/// Performs the specified action on each element of the <see cref="array"/>.
		/// </summary>
		/// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="array"/>.</param>
		public static void ForEach<T>(this T[] array, Action<T> action)
		{
			foreach (var t in array) {
				action(t);
			}
		}
	}
}