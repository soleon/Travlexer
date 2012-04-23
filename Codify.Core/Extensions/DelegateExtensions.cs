using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Codify.Extensions
{
    public static class DelegateExtensions
    {
        public static void ExecuteIfNotNull(this ICommand command, object parameter = null)
        {
            command.UseIfNotNull(c => c.Execute(parameter));
        }

        public static void ExecuteIfNotNull(this PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs e)
        {
            handler.UseIfNotNull(h => h(sender, e));
        }

        public static void ExecuteIfNotNull(this Action action)
        {
            action.UseIfNotNull(a => a());
        }

        public static void ExecuteIfNotNull(this EventHandler handler, object sender, EventArgs e)
        {
            handler.UseIfNotNull(h => h(sender, e));
        }

        public static void ExecuteIfNotNull<T>(this Action<T> action, T parameter)
        {
            action.UseIfNotNull(a => a(parameter));
        }

        public static void ExecuteIfNotNull<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
        {
            action.UseIfNotNull(a => a(param1, param2));
        }

        public static TResult ExecuteIfNotNull<TResult>(this Func<TResult> function, TResult defaultResult = default(TResult))
        {
            return function.UseIfNotNull(f => f(), defaultResult);
        }

        public static TResult ExecuteIfNotNull<T, TResult>(this Func<T, TResult> function, T param, TResult defaultResult = default(TResult))
        {
            return function.UseIfNotNull(f => f(param), defaultResult);
        }

        public static TResult ExecuteIfNotNull<T1, T2, TResult>(this Func<T1, T2, TResult> function, T1 param1, T2 param2, TResult defaultResult = default(TResult))
        {
            return function.UseIfNotNull(f => f(param1, param2), defaultResult);
        }

        public static TResult ExecuteIfNotNull<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, TResult defaultResult = default(TResult))
        {
            return function.UseIfNotNull(f => f(param1, param2, param3, param4), defaultResult);
        }
    }
}
