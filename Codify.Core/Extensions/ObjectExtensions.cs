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

        public static TResult UseIfNotNull<TTarget, TResult>(this TTarget target, Func<TTarget, TResult> function, TResult defaultResult = default(TResult)) where TTarget : class
        {
            return target == null || function == null ? defaultResult : function(target);
        }
    }
}