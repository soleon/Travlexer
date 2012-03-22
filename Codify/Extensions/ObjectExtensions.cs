using System;

namespace Codify.Extensions
{
	public static class ObjectExtensions
	{
		public static void UseIfNotNull<T>(this T obj, Action<T> action) where T : class
		{
			if (obj == null || action == null)
			{
				return;
			}
			action(obj);
		}
	}
}