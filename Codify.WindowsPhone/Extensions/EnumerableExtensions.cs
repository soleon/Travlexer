using System.Collections.Generic;
using System.Linq;

namespace Codify.WindowsPhone.Extensions
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Determines whether the specified enumerable is null or empty.
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			return enumerable == null || !enumerable.Any();
		}
	}
}