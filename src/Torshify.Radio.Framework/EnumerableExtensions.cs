using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Torshify.Radio.Framework
{
    /// <summary>
    /// 	Extension methods for all kinds of (typed) enumerable data (Array, List, ...)
    /// </summary>
    public static class EnumerableExtensions
    {
        #region Methods

        [Pure]
        public static IEnumerable<TTarget> CountSelect<TSource, TTarget>(this IEnumerable<TSource> source, Func<TSource, int, TTarget> func)
        {
            int i = 0;
            foreach (var item in source)
            {
                yield return func(item, i++);
            }
        }

        /// <summary>
        ///     Returns true if all items in the list are unique using
        ///     <see cref="EqualityComparer{T}.Default">EqualityComparer&lt;T&gt;.Default</see>.
        /// </summary>
        /// <exception cref="ArgumentNullException">if <param name="source"/> is null.</exception>
        [Pure]
        public static bool AllUnique<T>(this IList<T> source)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            return source.TrueForAllPairs((a, b) => !comparer.Equals(a, b));
        }

        /// <summary>
        ///     Returns true if <paramref name="compare"/> returns
        ///     true for every pair of items in <paramref name="source"/>.
        /// </summary>
        [Pure]
        public static bool TrueForAllPairs<T>(this IList<T> source, Func<T, T, bool> compare)
        {
            for (int i = 0; i < source.Count; i++)
            {
                for (int j = i + 1; j < source.Count; j++)
                {
                    if (!compare(source[i], source[j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Returns true if <paramref name="compare"/> returns true of every
        ///     adjacent pair of items in the <paramref name="source"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     If there are n items in the collection, n-1 comparisons are done.
        /// </para>
        /// <para>
        ///     Every valid [i] and [i+1] pair are passed into <paramref name="compare"/>.
        /// </para>
        /// <para>
        ///     If <paramref name="source"/> has 0 or 1 items, true is returned.
        /// </para>
        /// </remarks>
        [Pure]
        public static bool TrueForAllAdjacentPairs<T>(this IEnumerable<T> source, Func<T, T, bool> compare)
        {
            return source.SelectAdjacentPairs().All(t => compare(t.Item1, t.Item2));
        }

        public static IEnumerable<Tuple<T, T>> SelectAdjacentPairs<T>(this IEnumerable<T> source)
        {
            bool hasPrevious = false;
            T previous = default(T);

            foreach (var item in source)
            {
                if (!hasPrevious)
                {
                    previous = item;
                    hasPrevious = true;
                }
                else
                {
                    yield return Tuple.Create(previous, item);
                    previous = item;
                }
            }
        }

        /// <summary>
        ///     Returns true if all of the items in <paramref name="source"/> are not
        ///     null or empty.
        /// </summary>
        /// <exception cref="ArgumentNullException">if <param name="source"/> is null.</exception>
        [Pure]
        public static bool AllNotNullOrEmpty(this IEnumerable<string> source)
        {
            return source.All(item => !string.IsNullOrEmpty(item));
        }

        /// <summary>
        ///     Returns true if all items in <paramref name="source"/> exist
        ///     in <paramref name="set"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">if <param name="source"/> or <param name="set"/> are null.</exception>
        [Pure]
        public static bool AllExistIn<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> set)
        {
            return source.All(set.Contains);
        }

        /// <summary>
        ///     Returns true if <paramref name="source"/> has no items in it; otherwise, false.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     If an <see cref="ICollection{TSource}"/> is provided,
        ///     <see cref="ICollection{TSource}.Count"/> is used.
        /// </para>
        /// <para>
        ///     Yes, this does basically the same thing as the
        ///     <see cref="System.Linq.Enumerable.Any{TSource}(IEnumerable{TSource})"/>
        ///     extention. The differences: 'IsEmpty' is easier to remember and it leverages
        ///     <see cref="ICollection{TSource}.Count">ICollection.Count</see> if it exists.
        /// </para>
        /// </remarks>
        [Pure]
        public static bool IsEmpty<TSource>(this IEnumerable<TSource> source)
        {
            if (source is ICollection<TSource>)
            {
                return ((ICollection<TSource>)source).Count == 0;
            }
            else
            {
                using (IEnumerator<TSource> enumerator = source.GetEnumerator())
                {
                    return !enumerator.MoveNext();
                }
            }
        }

        /// <summary>
        ///     Returns the index of the first item in <paramref name="source"/>
        ///     for which <paramref name="predicate"/> returns true. If none, -1.
        /// </summary>
        /// <param name="source">The source enumerable.</param>
        /// <param name="predicate">The function to evaluate on each element.</param>
        [Pure]
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            int index = 0;
            foreach (TSource item in source)
            {
                if (predicate(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        /// <summary>
        ///     Returns a new <see cref="ReadOnlyCollection{T}"/> using the
        ///     contents of <paramref name="source"/>.
        /// </summary>
        /// <remarks>
        ///     The contents of <paramref name="source"/> are copied to
        ///     an array to ensure the contents of the returned value
        ///     don't mutate.
        /// </remarks>
        public static ReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
        {
            return new ReadOnlyCollection<TSource>(source.ToArray());
        }

        /// <summary>
        ///     Removes the last element from <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The list from which to remove the last element.</param>
        /// <returns>The last element.</returns>
        /// <remarks><paramref name="source"/> must have at least one element and allow changes.</remarks>
        public static TSource RemoveLast<TSource>(this IList<TSource> source)
        {
            TSource item = source[source.Count - 1];
            source.RemoveAt(source.Count - 1);
            return item;
        }

        /// <summary>
        ///     If <paramref name="source"/> is null, return an empty <see cref="IEnumerable{TSource}"/>;
        ///     otherwise, return <paramref name="source"/>.
        /// </summary>
        public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source)
        {
            return source ?? Enumerable.Empty<TSource>();
        }

        /// <summary>
        ///     Recursively projects each nested element to an <see cref="IEnumerable{TSource}"/>
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="recursiveSelector">A transform to apply to each element.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{TSource}"/> whose elements are the
        ///     result of recursively invoking the recursive transform function
        ///     on each element and nested element of the input sequence.
        /// </returns>
        /// <remarks>This is a depth-first traversal. Be careful if you're using this to find something
        /// shallow in a deep tree.</remarks>
        public static IEnumerable<TSource> SelectRecursive<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> recursiveSelector)
        {
            Stack<IEnumerator<TSource>> stack = new Stack<IEnumerator<TSource>>();
            stack.Push(source.GetEnumerator());

            try
            {
                while (stack.Count > 0)
                {
                    if (stack.Peek().MoveNext())
                    {
                        TSource current = stack.Peek().Current;

                        yield return current;

                        stack.Push(recursiveSelector(current).GetEnumerator());
                    }
                    else
                    {
                        stack.Pop().Dispose();
                    }
                }
            }
            finally
            {
                while (stack.Count > 0)
                {
                    stack.Pop().Dispose();
                }
            }
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] items)
        {
            return source.Concat(items.AsEnumerable());
        }

        [Pure]
        public static bool Contains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            return dictionary.Contains(new KeyValuePair<TKey, TValue>(key, value));
        }

        [Pure]
        public static bool CountAtLeast<T>(this IEnumerable<T> source, int count)
        {
            if (source is ICollection<T>)
            {
                return ((ICollection<T>)source).Count >= count;
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    while (count > 0)
                    {
                        if (enumerator.MoveNext())
                        {
                            count--;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public static IEnumerable<TSource> Except<TSource, TOther>(this IEnumerable<TSource> source, IEnumerable<TOther> other, Func<TSource, TOther, bool> comparer)
        {
            return from item in source
                   where !other.Any(x => comparer(item, x))
                   select item;
        }

        public static IEnumerable<TSource> Intersect<TSource, TOther>(this IEnumerable<TSource> source, IEnumerable<TOther> other, Func<TSource, TOther, bool> comparer)
        {
            return from item in source
                   where other.Any(x => comparer(item, x))
                   select item;
        }

        public static INotifyCollectionChanged AsINPC<T>(this ReadOnlyObservableCollection<T> source)
        {
            return (INotifyCollectionChanged)source;
        }

        /// <summary>
        /// Creates an <see cref="ObservableCollection{T}"/> from the <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source elements.</typeparam>
        /// <param name="source">The <see cref="IEnumerable"/> to create the <see cref="ObservableCollection{T}"/> from.</param>
        /// <returns>An <see cref="ObservableCollection{T}"/> that contains elements from the input sequence.</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }

        /// <summary>
        /// 	Performs an action for each item in the enumerable
        /// </summary>
        /// <typeparam name = "T">The enumerable data type</typeparam>
        /// <param name = "values">The data values.</param>
        /// <param name = "action">The action to be performed.</param>
        /// <example>
        /// 	var values = new[] { "1", "2", "3" };
        /// 	values.ConvertList&lt;string, int&gt;().ForEach(Console.WriteLine);
        /// </example>
        /// <remarks>
        /// 	This method was intended to return the passed values to provide method chaining. Howver due to defered execution the compiler would actually never run the entire code at all.
        /// </remarks>
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
                action(value);
        }

        ///<summary>
        ///	Returns enumerable object based on target, which does not contains null references.
        ///	If target is null reference, returns empty enumerable object.
        ///</summary>
        ///<typeparam name = "T">Type of items in target.</typeparam>
        ///<param name = "target">Target enumerable object. Can be null.</param>
        ///<example>
        ///	object[] items = null;
        ///	foreach(var item in items.NotNull()){
        ///	// result of items.NotNull() is empty but not null enumerable
        ///	}
        /// 
        ///	object[] items = new object[]{ null, "Hello World!", null, "Good bye!" };
        ///	foreach(var item in items.NotNull()){
        ///	// result of items.NotNull() is enumerable with two strings
        ///	}
        ///</example>
        ///<remarks>
        ///	Contributed by tencokacistromy, http://www.codeplex.com/site/users/view/tencokacistromy
        ///</remarks>
        public static IEnumerable<T> IgnoreNulls<T>(this IEnumerable<T> target)
        {
            if (ReferenceEquals(target, null))
                yield break;

            foreach (var item in target.Where(item => !ReferenceEquals(item, null)))
                yield return item;
        }

        /// <summary>
        /// 	Returns the maximum item based on a provided selector.
        /// </summary>
        /// <typeparam name = "TItem">The item type</typeparam>
        /// <typeparam name = "TValue">The value item</typeparam>
        /// <param name = "items">The items.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "maxValue">The max value as output parameter.</param>
        /// <returns>The maximum item</returns>
        /// <example>
        /// 	<code>
        /// 		int age;
        /// 		var oldestPerson = persons.MaxItem(p =&gt; p.Age, out age);
        /// 	</code>
        /// </example>
        public static TItem MaxItem<TItem, TValue>(
            this IEnumerable<TItem> items, Func<TItem, TValue> selector, out TValue maxValue)
            where TItem : class
            where TValue : IComparable
        {
            TItem maxItem = null;
            maxValue = default(TValue);

            foreach (var item in items)
            {
                if (item == null)
                    continue;

                var itemValue = selector(item);

                if ((maxItem != null) && (itemValue.CompareTo(maxValue) <= 0))
                    continue;

                maxValue = itemValue;
                maxItem = item;
            }

            return maxItem;
        }

        /// <summary>
        /// 	Returns the maximum item based on a provided selector.
        /// </summary>
        /// <typeparam name = "TItem">The item type</typeparam>
        /// <typeparam name = "TValue">The value item</typeparam>
        /// <param name = "items">The items.</param>
        /// <param name = "selector">The selector.</param>
        /// <returns>The maximum item</returns>
        /// <example>
        /// 	<code>
        /// 		var oldestPerson = persons.MaxItem(p =&gt; p.Age);
        /// 	</code>
        /// </example>
        public static TItem MaxItem<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> selector)
            where TItem : class
            where TValue : IComparable
        {
            TValue maxValue;

            return items.MaxItem(selector, out maxValue);
        }

        /// <summary>
        /// 	Returns the minimum item based on a provided selector.
        /// </summary>
        /// <typeparam name = "TItem">The item type</typeparam>
        /// <typeparam name = "TValue">The value item</typeparam>
        /// <param name = "items">The items.</param>
        /// <param name = "selector">The selector.</param>
        /// <param name = "minValue">The min value as output parameter.</param>
        /// <returns>The minimum item</returns>
        /// <example>
        /// 	<code>
        /// 		int age;
        /// 		var youngestPerson = persons.MinItem(p =&gt; p.Age, out age);
        /// 	</code>
        /// </example>
        public static TItem MinItem<TItem, TValue>(
            this IEnumerable<TItem> items, Func<TItem, TValue> selector, out TValue minValue)
            where TItem : class
            where TValue : IComparable
        {
            TItem minItem = null;
            minValue = default(TValue);

            foreach (var item in items)
            {
                if (item == null)
                    continue;
                var itemValue = selector(item);

                if ((minItem != null) && (itemValue.CompareTo(minValue) >= 0))
                    continue;
                minValue = itemValue;
                minItem = item;
            }

            return minItem;
        }

        /// <summary>
        /// 	Returns the minimum item based on a provided selector.
        /// </summary>
        /// <typeparam name = "TItem">The item type</typeparam>
        /// <typeparam name = "TValue">The value item</typeparam>
        /// <param name = "items">The items.</param>
        /// <param name = "selector">The selector.</param>
        /// <returns>The minimum item</returns>
        /// <example>
        /// 	<code>
        /// 		var youngestPerson = persons.MinItem(p =&gt; p.Age);
        /// 	</code>
        /// </example>
        public static TItem MinItem<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> selector)
            where TItem : class
            where TValue : IComparable
        {
            TValue minValue;

            return items.MinItem(selector, out minValue);
        }

        ///<summary>
        ///	Get Distinct
        ///</summary>
        ///<param name = "source"></param>
        ///<param name = "expression"></param>
        ///<typeparam name = "T"></typeparam>
        ///<typeparam name = "TKey"></typeparam>
        ///<returns></returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> expression)
        {
            return source == null ? Enumerable.Empty<T>() : source.GroupBy(expression).Select(i => i.First());
        }

        /// <summary>
        /// Removes matching items from a sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        /// 
        /// <remarks>
        /// 	Renamed by James Curran, to match corresponding HashSet.RemoveWhere()
        /// 	</remarks>
        public static IEnumerable<T> RemoveWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
                yield break;

            foreach (T t in source)
                if (!predicate(t))
                    yield return t;
        }

        ///<summary>
        ///	Remove item from a list
        ///</summary>
        ///<param name = "source"></param>
        ///<param name = "predicate"></param>
        ///<typeparam name = "T"></typeparam>
        ///<returns></returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        [Obsolete("Use RemoveWhere instead..")]
        public static IEnumerable<T> RemoveAll<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
                return Enumerable.Empty<T>();

            var list = source.ToList();
            list.RemoveAll(predicate);
            return list;
        }

        ///<summary>
        /// Turn the list of objects to a string of Common Seperated Value
        ///</summary>
        ///<param name="source"></param>
        ///<param name="separator"></param>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        /// <example>
        /// 	<code>
        /// 		var values = new[] { 1, 2, 3, 4, 5 };
        ///			string csv = values.ToCSV(';');
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Moses, http://mosesofegypt.net
        /// </remarks>
        public static string ToCSV<T>(this IEnumerable<T> source, char separator)
        {
            if (source == null)
                return string.Empty;

            var csv = new StringBuilder();
            source.ForEach(value => csv.AppendFormat("{0}{1}", value, separator));
            return csv.ToString(0, csv.Length - 1);
        }

        ///<summary>
        /// Turn the list of objects to a string of Common Seperated Value
        ///</summary>
        ///<param name="source"></param>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        /// <example>
        /// 	<code>
        /// 		var values = new[] {1, 2, 3, 4, 5};
        ///			string csv = values.ToCSV();
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Moses, http://mosesofegypt.net
        /// </remarks>
        public static string ToCSV<T>(this IEnumerable<T> source)
        {
            return source.ToCSV(',');
        }

        /// <summary>
        /// Overload the Select to allow null as a return
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="allowNull"></param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> using the selector containing null or non-null results based on <see cref="allowNull"/>.</returns>
        /// <example>
        /// <code>
        /// var list = new List{object}{ new object(), null, null };
        /// var noNulls = list.Select(x => x, false);
        /// </code>
        /// </example>
        /// <remarks>
        /// Contributed by thinktech_coder
        /// </remarks>
        public static IEnumerable<TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector, bool allowNull = true)
        {
            foreach (var item in source)
            {
                var select = selector(item);
                if (allowNull || !Equals(select, default(TSource)))
                    yield return select;
            }
        }

        /// <summary>
        /// Returns true if the <paramref name="source"/> is null or without any items.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return (source == null || !source.Any());
        }

        /// <summary>
        /// Returns true if the <paramref name="source"/> is contains at least one item.
        /// </summary>
        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
        {
            return !source.IsNullOrEmpty();
        }

        /// <summary>
        ///     Appends an element to the end of the current collection and returns the new collection.
        /// </summary>
        /// <typeparam name="T">The enumerable data type</typeparam>
        /// <param name="source">The data values.</param>
        /// <param name="item">The element to append the current collection with.</param>
        /// <returns>
        ///     The modified collection.
        /// </returns>
        /// <example>
        ///     var integers = Enumerable.Range(0, 3);  // 0, 1, 2
        ///     integers = integers.Append(3);          // 0, 1, 2, 3
        /// </example>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item)
        {
            foreach (var i in source)
                yield return i;

            yield return item;
        }

        /// <summary>
        ///     Prepends an element to the start of the current collection and returns the new collection.
        /// </summary>
        /// <typeparam name="T">The enumerable data type</typeparam>
        /// <param name="source">The data values.</param>
        /// <param name="item">The element to prepend the current collection with.</param>
        /// <returns>
        ///     The modified collection.
        /// </returns>
        /// <example>
        ///     var integers = Enumerable.Range(1, 3);  // 1, 2, 3
        ///     integers = integers.Prepend(0);         // 0, 1, 2, 3
        /// </example>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item)
        {
            yield return item;

            foreach (var i in source)
                yield return i;
        }

        /// <summary>
        ///     Creates an Array from an IEnumerable&lt;T&gt; using the specified transform function.
        /// </summary>
        /// <typeparam name="TSource">The source data type</typeparam>
        /// <typeparam name="TResult">The target data type</typeparam>
        /// <param name="source">The source data.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An Array of the target data type</returns>
        /// <example>
        ///     var integers = Enumerable.Range(1, 3);
        ///     var intStrings = values.ToArray(i => i.ToString());
        /// </example>
        /// <remarks>
        ///     This method is a shorthand for the frequently use pattern IEnumerable&lt;T&gt;.Select(Func).ToArray()
        /// </remarks>
        public static TResult[] ToArray<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToArray();
        }

        /// <summary>
        ///     Creates a List&lt;T&gt; from an IEnumerable&lt;T&gt; using the specified transform function.
        /// </summary>
        /// <typeparam name="TSource">The source data type</typeparam>
        /// <typeparam name="TResult">The target data type</typeparam>
        /// <param name="source">The source data.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An IEnumerable&lt;T&gt; of the target data type</returns>
        /// <example>
        ///     var integers = Enumerable.Range(1, 3);
        ///     var intStrings = values.ToList(i => i.ToString());
        /// </example>
        /// <remarks>
        ///     This method is a shorthand for the frequently use pattern IEnumerable&lt;T&gt;.Select(Func).ToList()
        /// </remarks>
        public static List<TResult> ToList<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToList();
        }

        /// <summary>
        /// Computes the sum of a sequence of UInt32 values.
        /// </summary>
        /// <param name="source">A sequence of UInt32 values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        public static uint Sum(this IEnumerable<uint> source)
        {
            return source.Aggregate(0U, (current, number) => current + number);
        }

        /// <summary>
        /// Computes the sum of a sequence of UInt64 values.
        /// </summary>
        /// <param name="source">A sequence of UInt64 values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        public static ulong Sum(this IEnumerable<ulong> source)
        {
            return source.Aggregate(0UL, (current, number) => current + number);
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable UInt32 values.
        /// </summary>
        /// <param name="source">A sequence of nullable UInt32 values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        public static uint? Sum(this IEnumerable<uint?> source)
        {
            return source.Where(nullable => nullable.HasValue).Aggregate(0U,
                                                                         (current, nullable) =>
                                                                         current + nullable.GetValueOrDefault());
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable UInt64 values.
        /// </summary>
        /// <param name="source">A sequence of nullable UInt64 values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        public static ulong? Sum(this IEnumerable<ulong?> source)
        {
            return source.Where(nullable => nullable.HasValue).Aggregate(0UL,
                                                                         (current, nullable) =>
                                                                         current + nullable.GetValueOrDefault());
        }

        /// <summary>
        /// Computes the sum of a sequence of UInt32 values that are obtained by invoking a transformation function on each element of the intput sequence.
        /// </summary>
        /// <param name="source">A sequence of values that are used to calculate a sum.</param>
        /// <param name="selection">A transformation function to apply to each element.</param>
        /// <returns>The sum of the projected values.</returns>
        public static uint Sum<T>(this IEnumerable<T> source, Func<T, uint> selection)
        {
            return source.Select(selection).Sum();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable UInt32 values that are obtained by invoking a transformation function on each element of the intput sequence.
        /// </summary>
        /// <param name="source">A sequence of values that are used to calculate a sum.</param>
        /// <param name="selection">A transformation function to apply to each element.</param>
        /// <returns>The sum of the projected values.</returns>
        public static uint? Sum<T>(this IEnumerable<T> source, Func<T, uint?> selection)
        {
            return source.Select(selection).Sum();
        }

        /// <summary>
        /// Computes the sum of a sequence of UInt64 values that are obtained by invoking a transformation function on each element of the intput sequence.
        /// </summary>
        /// <param name="source">A sequence of values that are used to calculate a sum.</param>
        /// <param name="selector">A transformation function to apply to each element.</param>
        /// <returns>The sum of the projected values.</returns>
        public static ulong Sum<T>(this IEnumerable<T> source, Func<T, ulong> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable UInt64 values that are obtained by invoking a transformation function on each element of the intput sequence.
        /// </summary>
        /// <param name="source">A sequence of values that are used to calculate a sum.</param>
        /// <param name="selector">A transformation function to apply to each element.</param>
        /// <returns>The sum of the projected values.</returns>
        public static ulong? Sum<T>(this IEnumerable<T> source, Func<T, ulong?> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Converts an enumeration of groupings into a Dictionary of those groupings.
        /// </summary>
        /// <typeparam name="TKey">Key type of the grouping and dictionary.</typeparam>
        /// <typeparam name="TValue">Element type of the grouping and dictionary list.</typeparam>
        /// <param name="groupings">The enumeration of groupings from a GroupBy() clause.</param>
        /// <returns>A dictionary of groupings such that the key of the dictionary is TKey type and the value is List of TValue type.</returns>
        public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(
            this IEnumerable<IGrouping<TKey, TValue>> groupings)
        {
            return groupings.ToDictionary(group => group.Key, group => group.ToList());
        }

        #endregion Methods
    }
}