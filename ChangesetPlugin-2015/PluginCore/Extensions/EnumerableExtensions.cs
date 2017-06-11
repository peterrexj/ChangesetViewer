﻿using System;
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

        [System.Diagnostics.DebuggerStepThrough]
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

        /// <summary>
        ///  Returns a seq which contains single given value.
        /// </summary>
        /// <typeparam name="T">Seq type</typeparam>
        /// <param name="value">Value to include in the seq</param>
        /// <returns>Seq which contains single given value</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> Singleton<T>(T value)
        {
            return new[] { value };
        }

        /// <summary>
        ///  A shortcut to get a sequence of static list of values, as in: Enumerable.OfValues(1, 2, 3) which returns sequence of 1, 2, 3
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="values">list of values</param>
        /// <returns>sequence of the given values</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<TValue> OfValues<TValue>(params TValue[] values)
        {
            return values;
        }

        /// <summary>
        ///  Returns a sequence that contains the elements generated by the given computation. 
        /// </summary>
        /// <typeparam name="TItem">seq item type</typeparam>
        /// <typeparam name="TState">state value type</typeparam>
        /// <param name="state">The initial state value.</param>
        /// <param name="generator">A function that takes the current state and returns an optional tuple of the next element of the sequence and the next state value.</param>
        /// <returns>The result sequence.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<TItem> Unfold<TItem, TState>(TState state, Func<TState, Tuple<TItem, TState>> generator)
        {
            for (var next = generator(state); next != null; next = generator(next.Item2))
                yield return next.Item1;
        }
        /// <summary>
        ///  Returns a sequence that contains the elements of a linked list defined by the nextGenerator function.
        /// </summary>
        /// <typeparam name="TItem">seq item type</typeparam>
        /// <param name="first">first item in the linked list (can be null)</param>
        /// <param name="nextGenerator">function which takes the current item in the linked list, and returns next item, or null.</param>
        /// <returns>The result sequence.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<TItem> UnfoldLinkedList<TItem>(TItem first, Func<TItem, TItem> nextGenerator)
        {
            return Unfold(first, state => state == null ? null : Tuple.Create(state, nextGenerator(state)));
        }

        /// <summary>
        /// Returns empty sequence of the given type
        /// </summary>
        /// <typeparam name="T">Type of the sequence</typeparam>
        /// <returns>Empty sequence of the given type</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> Empty<T>()
        {
            return Enumerable.Empty<T>();
        }

        /// <summary>
        ///  Iterates the sequence and calls the given action with the item
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        public static void Iter<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            items.Iteri((item, _) => action(item));
        }
        /// <summary>
        ///  invokes all actions in the sequence
        /// </summary>
        /// <param name="actions">actions to invoke</param>
        [System.Diagnostics.DebuggerStepThrough]
        public static void Invoke(this IEnumerable<Action> actions)
        {
            actions.Iter(a => a());
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void Iteri<TItem>(this IEnumerable<TItem> items, Action<TItem, int> action)
        {
            var index = 0;
            foreach (var item in items.EmptyIfNull())
                action(item, index++);
        }
        
        #region SUM
        static T? SumEx<T>(this IEnumerable<T?> items, Func<T, T, T> add) where T : struct
        {
            return items.EmptyIfNull().Where(v => v.HasValue).Aggregate((T?)null, (acc, v) => add(acc.GetValueOrDefault(), v.Value));
        }
        /// <summary>
        /// Sums the items in a list. Ignores null items in the list when summing. To be used instead of LinqToObject .Sum() when summing nullable types
        /// </summary>
        /// <param name="items">Items to Sum</param>
        /// <returns>Returns the sum of items in the Enumerable. Returns null if list is empty or items are all null. </returns>
        public static decimal? SumEx(this IEnumerable<decimal?> items) { return items.SumEx((a, b) => a + b); }

        /// <summary>
        /// Sums the items in a list. Ignores null items in the list when summing. To be used instead of LinqToObject .Sum() when summing nullable types
        /// </summary>
        /// <param name="items">Items to Sum</param>
        /// <returns>Returns the sum of items in the Enumerable. Returns null if list is empty or items are all null. </returns>
        public static double? SumEx(this IEnumerable<double?> items) { return items.SumEx((a, b) => a + b); }

        /// <summary>
        /// Sums the items in a list. Ignores null items in the list when summing. To be used instead of LinqToObject .Sum() when summing nullable types
        /// </summary>
        /// <param name="items">Items to Sum</param>
        /// <returns>Returns the sum of items in the Enumerable. Returns null if list is empty or items are all null. </returns>
        public static float? SumEx(this IEnumerable<float?> items) { return items.SumEx((a, b) => a + b); }

        /// <summary>
        /// Sums the items in a list. Ignores null items in the list when summing. To be used instead of LinqToObject .Sum() when summing nullable types
        /// </summary>
        /// <param name="items">Items to Sum</param>
        /// <returns>Returns the sum of items in the Enumerable. Returns null if list is empty or items are all null. </returns>
        public static int? SumEx(this IEnumerable<int?> items) { return items.SumEx((a, b) => a + b); }

        /// <summary>
        /// Sums the items in a list. Ignores null items in the list when summing. To be used instead of LinqToObject .Sum() when summing nullable types
        /// </summary>
        /// <param name="items">Items to Sum</param>
        /// <returns>Returns the sum of items in the Enumerable. Returns null if list is empty or items are all null. </returns>
        public static long? SumEx(this IEnumerable<long?> items) { return items.SumEx((a, b) => a + b); }

        #endregion


        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.EmptyIfNull().Any();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static string Concatenate(this IEnumerable<string> strings, string separator)
        {
            return string.Concat(strings.SeparateWith(separator));
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static string Concatenate(this IEnumerable<string> strings)
        {
            return string.Concat(strings.EmptyIfNull());
        }

        /// <summary>
        /// Yields items separated with the given separator item; the separator item is not yielded after the last item
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="items">items to separate with the given separator</param>
        /// <param name="separator">the seperator to separate the items with</param>
        /// <returns>items separated with the given separator item; the separator item is not yielded after the last item</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> SeparateWith<T>(this IEnumerable<T> items, T separator)
        {
            var first = true;
            foreach (var item in items.EmptyIfNull())
            {
                if (first)
                    first = false;
                else
                    yield return separator;

                yield return item;
            }
        }
    }
}
