using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginCore.Extensions
{
    public static class EnumerableExtensions
    {
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<TItem> DistinctBy<TItem, TDistinctValue>(this IEnumerable<TItem> items, Func<TItem, TDistinctValue> map, IEqualityComparer<TDistinctValue> distinctValueComparer = null)
        {
            if (distinctValueComparer == null)
                distinctValueComparer = EqualityComparer<TDistinctValue>.Default;

            var comparer = EqualityComparerEx.Create<TItem>((a, b) => distinctValueComparer.Equals(map.Invoke(a), map.Invoke(b)), a => distinctValueComparer.GetHashCode(map.Invoke(a)));
            return items.EmptyIfNull().Distinct(comparer);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> Empty<T>()
        {
            return Enumerable.Empty<T>();
        }

        
        [System.Diagnostics.DebuggerStepThrough]
        public static void Iterate<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            items.IterateI((item, _) => action(item));
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void IterateI<TItem>(this IEnumerable<TItem> items, Action<TItem, int> action)
        {
            var index = 0;
            foreach (var item in items.EmptyIfNull())
                action(item, index++);
        }
        
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.EmptyIfNull().Any();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static string Concatenate(this IEnumerable<string> strings)
        {
            return string.Concat(strings.EmptyIfNull());
        }
    }
}
