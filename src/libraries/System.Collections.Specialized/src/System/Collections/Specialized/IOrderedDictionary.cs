// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections.Specialized
{
    /// <devdoc>
    /// <para>
    /// This interface adds indexing on the IDictionary keyed table concept.  Objects
    /// added or inserted in an IOrderedDictionary must have both a key and an index, and
    /// can be retrieved by either.
    /// This interface is useful when preserving easy IDictionary access semantics via a key is
    /// desired while ordering is necessary.
    /// </para>
    /// </devdoc>
    public interface IOrderedDictionary : IDictionary
    {
        // properties
        /// <devdoc>
        /// Returns the object at the given index
        /// </devdoc>
        object? this[int index]
        {
            [CollectionAccess(CollectionAccessType.Read)] get;
            [CollectionAccess(CollectionAccessType.UpdatedContent)] set;
        }

        // Returns an IDictionaryEnumerator for this dictionary.
        [CollectionAccess(CollectionAccessType.Read)]
        new IDictionaryEnumerator GetEnumerator();

        // methods
        /// <devdoc>
        /// Inserts the given object, with the given key, at the given index
        /// </devdoc>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        void Insert(int index, object key, object? value);

        /// <devdoc>
        /// Removes the object and key at the given index
        /// </devdoc>
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        void RemoveAt(int index);
    }
}
