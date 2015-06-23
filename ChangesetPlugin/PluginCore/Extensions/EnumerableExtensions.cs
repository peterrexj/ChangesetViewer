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
        public static bool ContainsUsingOr<T>(this IEnumerable<T> items, T value)
        {
            return items.EmptyIfNull().Contains(value);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static bool ContainsUsingOr<T>(this IEnumerable<T> items, T value, bool treatValuesAsConstants)
        {
            return items.ContainsUsingOr(value);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static bool VariableSeqContainsUsingOr<T>(this IEnumerable<T> items, T value)
        {
            return items.ContainsUsingOr(value);
        }

        public static IEnumerable<IEnumerable<T>> InBatches<T>(this IEnumerable<T> items, int batchSize)
        {
            using (var enumerator = items.GetEnumerator())
            {
                for (; ; )
                {
                    var batch = new List<T>(batchSize);
                    while (batch.Count < batchSize && enumerator.MoveNext())
                        batch.Add(enumerator.Current);

                    if (batch.Count == 0)
                        yield break;

                    yield return batch;
                }
            }
        }
    }
}
