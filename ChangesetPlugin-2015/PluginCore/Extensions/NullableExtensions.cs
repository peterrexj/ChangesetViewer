using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginCore.Extensions
{
    public static class NullableEx
    {
        /// <summary>
        /// (IS) returns singleton sequence of the nullable's value if it has a value, otherwise empty sequence
        /// </summary>
        /// <typeparam name="T">nullable value type</typeparam>
        /// <param name="value">nullable</param>
        /// <returns>singleton sequence of the nullable's value if it has a value, otherwise empty sequence</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T? value) where T : struct
        {
            return value.HasValue ? EnumerableExtensions.Singleton(value.Value) : Enumerable.Empty<T>();
        }
        /// <summary>
        /// (IS) returns true if this nullable does not have a value, or its value is set to default(TValue); false otherwise
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="value">nullable value</param>
        /// <returns>true if this nullable does not have a value, or its value is set to default(TValue); false otherwise</returns>
        public static bool IsNullOrDefault<TValue>(this TValue? value)
            where TValue : struct
        {
            return !value.HasValue || value.Value.Equals(default(TValue));
        }
        /// <summary>
        /// creates nullable of the given value
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="value">value</param>
        /// <returns>nullable of the given value</returns>
        public static T? OfValue<T>(T value)
            where T : struct
        {
            return value;
        }
        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        public static object Wrap(Type nullableType, object value)
        {
            return value == null ? null : Activator.CreateInstance(nullableType, value);
        }
        /// <summary>
        /// (IS) returns the value converted to nullable
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="value">the value to convert to nullable</param>
        /// <returns>the value converted to nullable</returns>
        public static T? AsNullable<T>(this T value) where T : struct
        {
            return value;
        }
        /// <summary>
        /// (IS) calls the action on nullable's value if it has a value, otherwise does nothing
        /// </summary>
        /// <param name="value">nullable to invoke the action on if it has a value</param>
        /// <param name="action">action to invoke on the value if it has a value</param>
        public static void Iter<T>(this Nullable<T> value, Action<T> action) where T : struct
        {
            if (value.HasValue)
                action(value.Value);
        }
    }
}
