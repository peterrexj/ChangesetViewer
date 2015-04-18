using PluginCore.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
    }
}
