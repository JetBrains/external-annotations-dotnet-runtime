// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace System.Collections.Immutable
{
    /// <summary>
    /// A set of initialization methods for instances of <see cref="ImmutableSortedSet{T}"/>.
    /// </summary>
    public static class ImmutableSortedSet
    {
        /// <summary>
        /// Returns an empty collection.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <returns>The immutable collection.</returns>
        public static ImmutableSortedSet<T> Create<T>()
        {
            return ImmutableSortedSet<T>.Empty;
        }

        /// <summary>
        /// Returns an empty collection.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        /// The immutable collection.
        /// </returns>
        public static ImmutableSortedSet<T> Create<T>(IComparer<T>? comparer)
        {
            return ImmutableSortedSet<T>.Empty.WithComparer(comparer);
        }

        /// <summary>
        /// Creates a new immutable collection prefilled with the specified item.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="item">The item to prepopulate.</param>
        /// <returns>The new immutable collection.</returns>
        public static ImmutableSortedSet<T> Create<T>(T item)
        {
            return ImmutableSortedSet<T>.Empty.Add(item);
        }

        /// <summary>
        /// Creates a new immutable collection prefilled with the specified item.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <param name="item">The item to prepopulate.</param>
        /// <returns>The new immutable collection.</returns>
        public static ImmutableSortedSet<T> Create<T>(IComparer<T>? comparer, T item)
        {
            return ImmutableSortedSet<T>.Empty.WithComparer(comparer).Add(item);
        }

        /// <summary>
        /// Creates a new immutable collection prefilled with the specified items.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="items">The items to prepopulate.</param>
        /// <returns>The new immutable collection.</returns>
        public static ImmutableSortedSet<T> CreateRange<T>(IEnumerable<T> items)
        {
            return ImmutableSortedSet<T>.Empty.Union(items);
        }

        /// <summary>
        /// Creates a new immutable collection prefilled with the specified items.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <param name="items">The items to prepopulate.</param>
        /// <returns>The new immutable collection.</returns>
        public static ImmutableSortedSet<T> CreateRange<T>(IComparer<T>? comparer, IEnumerable<T> items)
        {
            return ImmutableSortedSet<T>.Empty.WithComparer(comparer).Union(items);
        }

        /// <summary>
        /// Creates a new immutable collection prefilled with the specified items.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="items">The items to prepopulate.</param>
        /// <returns>The new immutable collection.</returns>
        public static ImmutableSortedSet<T> Create<T>(params T[] items)
        {
            Requires.NotNull(items, nameof(items));
            return Create((ReadOnlySpan<T>)items);
        }

        /// <summary>
        /// Creates a new immutable sorted set that contains the specified array of items.
        /// </summary>
        /// <typeparam name="T">The type of items in the immutable set.</typeparam>
        /// <param name="items">A span that contains the items to prepopulate the set with.</param>
        /// <returns>A new immutable set that contains the specified items.</returns>
        public static ImmutableSortedSet<T> Create<T>(params ReadOnlySpan<T> items)
        {
            return ImmutableSortedSet<T>.Empty.Union(items);
        }

        /// <summary>
        /// Creates a new immutable collection prefilled with the specified items.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <param name="items">The items to prepopulate.</param>
        /// <returns>The new immutable collection.</returns>
        public static ImmutableSortedSet<T> Create<T>(IComparer<T>? comparer, params T[] items)
        {
            Requires.NotNull(items, nameof(items));

            return Create(comparer, (ReadOnlySpan<T>)items);
        }

        /// <summary>
        /// Creates a new immutable collection prefilled with the specified items.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <param name="items">The items to prepopulate.</param>
        /// <returns>The new immutable collection.</returns>
        public static ImmutableSortedSet<T> Create<T>(IComparer<T>? comparer, params ReadOnlySpan<T> items)
        {
            return ImmutableSortedSet<T>.Empty.WithComparer(comparer).Union(items);
        }

        /// <summary>
        /// Returns an empty collection.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <returns>The immutable collection.</returns>
        [return: CollectionAccess(CollectionAccessType.None)]
        public static ImmutableSortedSet<T>.Builder CreateBuilder<T>()
        {
            return Create<T>().ToBuilder();
        }

        /// <summary>
        /// Returns an empty collection.
        /// </summary>
        /// <typeparam name="T">The type of items stored by the collection.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        /// The immutable collection.
        /// </returns>
        [return: CollectionAccess(CollectionAccessType.None)]
        public static ImmutableSortedSet<T>.Builder CreateBuilder<T>(IComparer<T>? comparer)
        {
            return Create<T>(comparer).ToBuilder();
        }

        /// <summary>
        /// Enumerates a sequence exactly once and produces an immutable set of its contents.
        /// </summary>
        /// <typeparam name="TSource">The type of element in the sequence.</typeparam>
        /// <param name="source">The sequence to enumerate.</param>
        /// <param name="comparer">The comparer to use for initializing and adding members to the sorted set.</param>
        /// <returns>An immutable set.</returns>
        public static ImmutableSortedSet<TSource> ToImmutableSortedSet<TSource>(this IEnumerable<TSource> source, IComparer<TSource>? comparer)
        {
            if (source is ImmutableSortedSet<TSource> existingSet)
            {
                return existingSet.WithComparer(comparer);
            }

            return ImmutableSortedSet<TSource>.Empty.WithComparer(comparer).Union(source);
        }

        /// <summary>
        /// Enumerates a sequence exactly once and produces an immutable set of its contents.
        /// </summary>
        /// <typeparam name="TSource">The type of element in the sequence.</typeparam>
        /// <param name="source">The sequence to enumerate.</param>
        /// <returns>An immutable set.</returns>
        public static ImmutableSortedSet<TSource> ToImmutableSortedSet<TSource>(this IEnumerable<TSource> source)
        {
            return ToImmutableSortedSet(source, null);
        }

        /// <summary>
        /// Returns an immutable copy of the current contents of the builder's collection.
        /// </summary>
        /// <param name="builder">The builder to create the immutable set from.</param>
        /// <returns>An immutable set.</returns>
        public static ImmutableSortedSet<TSource> ToImmutableSortedSet<TSource>(this ImmutableSortedSet<TSource>.Builder builder)
        {
            Requires.NotNull(builder, nameof(builder));

            return builder.ToImmutable();
        }
    }
}
