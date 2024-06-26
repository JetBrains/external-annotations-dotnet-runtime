// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Dynamic.Utils
{
    internal static class CollectionExtensions
    {
        public static TrueReadOnlyCollection<T> AddFirst<T>(this ReadOnlyCollection<T> list, T item)
        {
            T[] res = new T[list.Count + 1];
            res[0] = item;
            list.CopyTo(res, 1);
            return new TrueReadOnlyCollection<T>(res);
        }

        public static T[] AddFirst<T>(this T[] array, T item)
        {
            T[] res = new T[array.Length + 1];
            res[0] = item;
            array.CopyTo(res, 1);
            return res;
        }

        public static T[] AddLast<T>(this T[] array, T item)
        {
            T[] res = new T[array.Length + 1];
            array.CopyTo(res, 0);
            res[array.Length] = item;
            return res;
        }

        public static T[] RemoveFirst<T>(this T[] array)
        {
            T[] result = new T[array.Length - 1];
            Array.Copy(array, 1, result, 0, result.Length);
            return result;
        }

        public static T[] RemoveLast<T>(this T[] array)
        {
            T[] result = new T[array.Length - 1];
            Array.Copy(array, result, result.Length);
            return result;
        }

        /// <summary>
        /// Wraps the provided enumerable into a ReadOnlyCollection{T}
        ///
        /// Copies all of the data into a new array, so the data can't be
        /// changed after creation. The exception is if the enumerable is
        /// already a ReadOnlyCollection{T}, in which case we just return it.
        /// </summary>
        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T>? enumerable)
        {
            if (enumerable != null && enumerable != ReadOnlyCollection<T>.Empty)
            {
                if (enumerable is TrueReadOnlyCollection<T> troc)
                {
                    return troc;
                }

                if (enumerable is ReadOnlyCollectionBuilder<T> builder)
                {
                    return builder.ToReadOnlyCollection();
                }

                T[] array = enumerable.ToArray();
                if (array.Length != 0)
                {
                    return new TrueReadOnlyCollection<T>(array);
                }
            }

            return ReadOnlyCollection<T>.Empty;
        }

        // We could probably improve the hashing here
        public static int ListHashCode<[DefaultEqualityUsage] T>(this ReadOnlyCollection<T> list)
        {
            EqualityComparer<T> cmp = EqualityComparer<T>.Default;
            int h = 6551;
            foreach (T t in list)
            {
                if (t != null)
                {
                    h ^= (h << 5) ^ cmp.GetHashCode(t);
                }
            }
            return h;
        }

        public static bool ListEquals<[DefaultEqualityUsage] T>(this ReadOnlyCollection<T> first, ReadOnlyCollection<T> second)
        {
            if (first == second)
            {
                return true;
            }

            int count = first.Count;

            if (count != second.Count)
            {
                return false;
            }

            EqualityComparer<T> cmp = EqualityComparer<T>.Default;
            for (int i = 0; i != count; ++i)
            {
                if (!cmp.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
