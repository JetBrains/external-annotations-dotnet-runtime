// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Collections.ObjectModel
{
    [Serializable]
    [DebuggerTypeProxy(typeof(KeyedCollection<,>.KeyedCollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public abstract class KeyedCollection<[DefaultEqualityUsage] TKey, TItem> : Collection<TItem> where TKey : notnull
    {
        private const int DefaultThreshold = 0;

        private readonly IEqualityComparer<TKey> comparer; // Do not rename (binary serialization)
        private Dictionary<TKey, TItem>? dict; // Do not rename (binary serialization)
        private int keyCount; // Do not rename (binary serialization)
        private readonly int threshold; // Do not rename (binary serialization)

        protected KeyedCollection() : this(null, DefaultThreshold)
        {
        }

        protected KeyedCollection(IEqualityComparer<TKey>? comparer) : this(comparer, DefaultThreshold)
        {
        }

        protected KeyedCollection(IEqualityComparer<TKey>? comparer, int dictionaryCreationThreshold)
            : base(new List<TItem>()) // Be explicit about the use of List<T> so we can foreach over
                                      // Items internally without enumerator allocations.
        {
            if (dictionaryCreationThreshold < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dictionaryCreationThreshold), SR.ArgumentOutOfRange_InvalidThreshold);
            }

            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
            threshold = dictionaryCreationThreshold == -1 ? int.MaxValue : dictionaryCreationThreshold;
        }

        /// <summary>
        /// Enables the use of foreach internally without allocations using <see cref="List{T}"/>'s struct enumerator.
        /// </summary>
        private new List<TItem> Items
        {
            get
            {
                Debug.Assert(base.Items is List<TItem>);
                return (List<TItem>)base.Items;
            }
        }

        [CollectionAccess(CollectionAccessType.None)]
        public IEqualityComparer<TKey> Comparer => comparer;

        public TItem this[TKey key]
        {
            [CollectionAccess(CollectionAccessType.Read)]
            get
            {
                TItem item;
                if (TryGetValue(key, out item!))
                {
                    return item;
                }

                throw new KeyNotFoundException(SR.Format(SR.Arg_KeyNotFoundWithKey, key.ToString()));
            }
        }

        [CollectionAccess(CollectionAccessType.Read)]
        public bool Contains(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (dict != null)
            {
                return dict.ContainsKey(key);
            }

            foreach (TItem item in Items)
            {
                if (comparer.Equals(GetKeyForItem(item), key))
                {
                    return true;
                }
            }

            return false;
        }

        [CollectionAccess(CollectionAccessType.Read)]
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TItem item)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (dict != null)
            {
                return dict.TryGetValue(key, out item!);
            }

            foreach (TItem itemInItems in Items)
            {
                TKey keyInItems = GetKeyForItem(itemInItems);
                if (keyInItems != null && comparer.Equals(key, keyInItems))
                {
                    item = itemInItems;
                    return true;
                }
            }

            item = default;
            return false;
        }

        private bool ContainsItem([DefaultEqualityUsage] TItem item)
        {
            TKey key;
            if ((dict == null) || ((key = GetKeyForItem(item)) == null))
            {
                return Items.Contains(item);
            }

            TItem itemInDict;
            if (dict.TryGetValue(key, out itemInDict!))
            {
                return EqualityComparer<TItem>.Default.Equals(itemInDict, item);
            }

            return false;
        }

        // ReSharper disable once InternalAttributeOnPublicApi - can't annotate for now
        [DefaultEqualityUsageInternal(nameof(TItem))]
        [CollectionAccess(CollectionAccessType.ModifyExistingContent | CollectionAccessType.Read)]
        public bool Remove(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (dict != null)
            {
                TItem item;
                return dict.TryGetValue(key, out item!) && Remove(item);
            }

            for (int i = 0; i < Items.Count; i++)
            {
                if (comparer.Equals(GetKeyForItem(Items[i]), key))
                {
                    RemoveItem(i);
                    return true;
                }
            }

            return false;
        }

        protected IDictionary<TKey, TItem>? Dictionary => dict;

        protected void ChangeItemKey([DefaultEqualityUsage] TItem item, TKey newKey)
        {
            if (!ContainsItem(item))
            {
                throw new ArgumentException(SR.Argument_ItemNotExist, nameof(item));
            }

            TKey oldKey = GetKeyForItem(item);
            if (!comparer.Equals(oldKey, newKey))
            {
                if (newKey != null)
                {
                    AddKey(newKey, item);
                }
                if (oldKey != null)
                {
                    RemoveKey(oldKey);
                }
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            dict?.Clear();
            keyCount = 0;
        }

        protected abstract TKey GetKeyForItem(TItem item);

        protected override void InsertItem(int index, TItem item)
        {
            TKey key = GetKeyForItem(item);
            if (key != null)
            {
                AddKey(key, item);
            }

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            TKey key = GetKeyForItem(Items[index]);
            if (key != null)
            {
                RemoveKey(key);
            }

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TItem item)
        {
            TKey newKey = GetKeyForItem(item);
            TKey oldKey = GetKeyForItem(Items[index]);

            if (comparer.Equals(oldKey, newKey))
            {
                if (newKey != null && dict != null)
                {
                    dict[newKey] = item;
                }
            }
            else
            {
                if (newKey != null)
                {
                    AddKey(newKey, item);
                }

                if (oldKey != null)
                {
                    RemoveKey(oldKey);
                }
            }

            base.SetItem(index, item);
        }

        private void AddKey(TKey key, TItem item)
        {
            if (dict != null)
            {
                dict.Add(key, item);
            }
            else if (keyCount == threshold)
            {
                CreateDictionary();
                dict!.Add(key, item);
            }
            else
            {
                if (Contains(key))
                {
                    throw new ArgumentException(SR.Format(SR.Argument_AddingDuplicate, key), nameof(key));
                }

                keyCount++;
            }
        }

        private void CreateDictionary()
        {
            dict = new Dictionary<TKey, TItem>(comparer);
            foreach (TItem item in Items)
            {
                TKey key = GetKeyForItem(item);
                if (key != null)
                {
                    dict.Add(key, item);
                }
            }
        }

        private void RemoveKey(TKey key)
        {
            Debug.Assert(key != null, "key shouldn't be null!");
            if (dict != null)
            {
                dict.Remove(key);
            }
            else
            {
                keyCount--;
            }
        }

        private sealed class KeyedCollectionDebugView
        {
            private readonly KeyedCollection<TKey, TItem> _collection;

            public KeyedCollectionDebugView(KeyedCollection<TKey, TItem> collection)
            {
                ArgumentNullException.ThrowIfNull(collection);
                _collection = collection;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyedCollectionDebugViewItem[] Items
            {
                get
                {
                    var items = new KeyedCollectionDebugViewItem[_collection.Count];
                    for (int i = 0; i < items.Length; i++)
                    {
                        TItem item = _collection[i];
                        items[i] = new KeyedCollectionDebugViewItem(_collection.GetKeyForItem(item), item);
                    }
                    return items;
                }
            }

            [DebuggerDisplay("{Value}", Name = "[{Key}]")]
            internal readonly struct KeyedCollectionDebugViewItem
            {
                public KeyedCollectionDebugViewItem(TKey key, TItem value)
                {
                    Key = key;
                    Value = value;
                }

                [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
                public TKey Key { get; }

                [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
                public TItem Value { get; }
            }
        }
    }
}
