// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections.Generic
{
    /// <summary>
    /// Generic collection that guarantees the uniqueness of its elements, as defined
    /// by some comparer. It also supports basic set operations such as Union, Intersection,
    /// Complement and Exclusive Complement.
    /// </summary>
    public interface ISet<T> : ICollection<T>
    {
        //Add ITEM to the set, return true if added, false if duplicate
        [CollectionAccess(CollectionAccessType.UpdatedContent | CollectionAccessType.Read)]
        new bool Add(T item);

        //Transform this set into its union with the IEnumerable<T> other
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        void UnionWith(IEnumerable<T> other);

        //Transform this set into its intersection with the IEnumerable<T> other
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        void IntersectWith(IEnumerable<T> other);

        //Transform this set so it contains no elements that are also in other
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        void ExceptWith(IEnumerable<T> other);

        //Transform this set so it contains elements initially in this or in other, but not both
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        void SymmetricExceptWith(IEnumerable<T> other);

        //Check if this set is a subset of other
        [CollectionAccess(CollectionAccessType.Read)]
        bool IsSubsetOf(IEnumerable<T> other);

        //Check if this set is a superset of other
        [CollectionAccess(CollectionAccessType.Read)]
        bool IsSupersetOf(IEnumerable<T> other);

        //Check if this set is a subset of other, but not the same as it
        [CollectionAccess(CollectionAccessType.Read)]
        bool IsProperSupersetOf(IEnumerable<T> other);

        //Check if this set is a superset of other, but not the same as it
        [CollectionAccess(CollectionAccessType.Read)]
        bool IsProperSubsetOf(IEnumerable<T> other);

        //Check if this set has any elements in common with other
        [CollectionAccess(CollectionAccessType.Read)]
        bool Overlaps(IEnumerable<T> other);

        //Check if this set contains the same and only the same elements as other
        [CollectionAccess(CollectionAccessType.Read)]
        bool SetEquals(IEnumerable<T> other);
    }
}
