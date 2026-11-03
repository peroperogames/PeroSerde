/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/27-17:33:05
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

/// <summary>
/// Json formatter for Collection
/// </summary>
/// <typeparam name="T">Element type</typeparam>
/// <typeparam name="TCollection">Collection type</typeparam>
[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public sealed class CollectionFormatter<TCollection, T> : ISerdeJsonFormatter<TCollection> where TCollection : ICollection<T>, new()
{
    private static readonly IFixedSerdeJsonFormatter<T> s_FixedFormatter;
    private static readonly ISerdeJsonFormatter<T> s_UnfixedFormatter;
    private static readonly bool s_IsFixed;

    static CollectionFormatter()
    {
        if (!SerdeJsonFormatterProvider.IsRegistered<T>())
        {
            throw SerdeException.ThrowFormatterNotFound(typeof(T));
        }
        var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
        if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
        {
            s_IsFixed = true;
            s_FixedFormatter = fixedFormatter;
        }
        else if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
        {
            s_UnfixedFormatter = unfixedFormatter;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeUtf8(TCollection value, ref JsonWriterUtf8 writer)
    {
        if (s_IsFixed)
        {
            writer.WriteCollection(value, s_FixedFormatter);
        }
        else
        {
            writer.WriteCollection(value, s_UnfixedFormatter);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeUtf16(TCollection value, ref JsonWriterUtf16 writer)
    {
        if (s_IsFixed)
        {
            writer.WriteCollection(value, s_FixedFormatter);
        }
        else
        {
            writer.WriteCollection(value, s_UnfixedFormatter);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TCollection DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
    {
        return s_IsFixed
            ? reader.ReadCollection<TCollection, T>(s_FixedFormatter)
            : reader.ReadCollection<TCollection, T>(s_UnfixedFormatter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TCollection DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
    {
        return s_IsFixed
            ? reader.ReadCollection<TCollection, T>(s_FixedFormatter)
            : reader.ReadCollection<TCollection, T>(s_UnfixedFormatter);
    }
}

/// <summary>
/// Json formatter for ReadOnlyCollection
/// </summary>
/// <typeparam name="T">Element type</typeparam>
/// <typeparam name="TCollection">Collection type</typeparam>
[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public sealed class ReadonlyCollectionFormatter<TCollection, T> : ISerdeJsonFormatter<TCollection> where TCollection : IReadOnlyCollection<T>
{
    private static readonly IFixedSerdeJsonFormatter<T> s_FixedFormatter;
    private static readonly ISerdeJsonFormatter<T> s_UnfixedFormatter;
    private static readonly bool s_IsFixed;
    private static readonly bool s_IsICollection = typeof(ICollection<T>).IsAssignableFrom(typeof(TCollection));
    private static readonly bool s_IsIProducerConsumerCollection = typeof(IProducerConsumerCollection<T>).IsAssignableFrom(typeof(TCollection));

    // .NET Standard library implementations of ICollection<T>
    private static readonly bool s_IsList = typeof(TCollection) == typeof(List<T>);
    private static readonly bool s_IsHashSet = typeof(TCollection) == typeof(HashSet<T>);
    private static readonly bool s_IsSortedSet = typeof(TCollection) == typeof(SortedSet<T>);
    private static readonly bool s_IsLinkedList = typeof(TCollection) == typeof(LinkedList<T>);
    private static readonly bool s_IsCollection = typeof(TCollection) == typeof(Collection<T>);

    // .NET Standard library implementations of IReadOnlyCollection<T>
    private static readonly bool s_IsArray = typeof(TCollection) == typeof(T[]);
    private static readonly bool s_IsQueue = typeof(TCollection) == typeof(Queue<T>);
    private static readonly bool s_IsStack = typeof(TCollection) == typeof(Stack<T>);
    private static readonly bool s_IsConcurrentBag = typeof(TCollection) == typeof(ConcurrentBag<T>);
    private static readonly bool s_IsConcurrentQueue = typeof(TCollection) == typeof(ConcurrentQueue<T>);
    private static readonly bool s_IsConcurrentStack = typeof(TCollection) == typeof(ConcurrentStack<T>);

    // ReadOnlyCollection<T> and interface-typed collections are not creatable through a parameterless constructor
    private static readonly bool s_IsReadOnlyCollection = typeof(TCollection) == typeof(ReadOnlyCollection<T>);
    private static readonly bool s_IsInterface = typeof(TCollection).IsInterface;
    private static readonly ConstructorInfo s_ParameterlessCtor = typeof(TCollection).GetConstructor(Type.EmptyTypes);

    static ReadonlyCollectionFormatter()
    {
        if (!SerdeJsonFormatterProvider.IsRegistered<T>())
        {
            throw SerdeException.ThrowFormatterNotFound(typeof(T));
        }
        var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
        if (formatter == null) throw SerdeException.ThrowFormatterNotFound(typeof(T));
        if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
        {
            s_IsFixed = true;
            s_FixedFormatter = fixedFormatter;
        }
        else if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
        {
            s_UnfixedFormatter = unfixedFormatter;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeUtf8(TCollection value, ref JsonWriterUtf8 writer)
    {
        if (s_IsFixed)
        {
            writer.WriteReadonlyCollection(value, s_FixedFormatter);
        }
        else
        {
            writer.WriteReadonlyCollection(value, s_UnfixedFormatter);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeUtf16(TCollection value, ref JsonWriterUtf16 writer)
    {
        if (s_IsFixed)
        {
            writer.WriteReadonlyCollection(value, s_FixedFormatter);
        }
        else
        {
            writer.WriteReadonlyCollection(value, s_UnfixedFormatter);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TCollection DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
    {
        if (s_IsICollection)
        {
            if (s_IsList)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<List<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<List<T>, T>(s_UnfixedFormatter));
            }
            if (s_IsHashSet)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<HashSet<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<HashSet<T>, T>(s_UnfixedFormatter));
            }
            if (s_IsSortedSet)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<SortedSet<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<SortedSet<T>, T>(s_UnfixedFormatter));
            }
            if (s_IsLinkedList)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<LinkedList<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<LinkedList<T>, T>(s_UnfixedFormatter));
            }
            if (s_IsCollection)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<Collection<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<Collection<T>, T>(s_UnfixedFormatter));
            }
        }
        // Fallback, causes the container to be created twice
        var list = s_IsFixed
            ? reader.ReadCollection<List<T>, T>(s_FixedFormatter)
            : reader.ReadCollection<List<T>, T>(s_UnfixedFormatter);
        // A json null yields no list, and the collection stays null instead of wrapping a missing one
        return list == null ? default : (TCollection) CreateReadOnlyCollection(list);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TCollection DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
    {
        if (s_IsICollection)
        {
            if (s_IsList)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<List<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<List<T>, T>(s_UnfixedFormatter));
            }
            if (s_IsHashSet)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<HashSet<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<HashSet<T>, T>(s_UnfixedFormatter));
            }
            if (s_IsSortedSet)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<SortedSet<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<SortedSet<T>, T>(s_UnfixedFormatter));
            }
            if (s_IsLinkedList)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<LinkedList<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<LinkedList<T>, T>(s_UnfixedFormatter));
            }
            if (s_IsCollection)
            {
                return (TCollection) (object) (s_IsFixed
                    ? reader.ReadCollection<Collection<T>, T>(s_FixedFormatter)
                    : reader.ReadCollection<Collection<T>, T>(s_UnfixedFormatter));
            }
        }
        // Fallback, causes the container to be created twice
        var list = s_IsFixed
            ? reader.ReadCollection<List<T>, T>(s_FixedFormatter)
            : reader.ReadCollection<List<T>, T>(s_UnfixedFormatter);
        // A json null yields no list, and the collection stays null instead of wrapping a missing one
        return list == null ? default : (TCollection) CreateReadOnlyCollection(list);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IReadOnlyCollection<T> CreateReadOnlyCollection(List<T> col)
    {
        if (s_IsArray)
        {
            return col.ToArray();
        }
        if (s_IsQueue)
        {
            return new Queue<T>(col);
        }
        if (s_IsStack)
        {
            return new Stack<T>(col);
        }
        if (s_IsConcurrentBag)
        {
            return new ConcurrentBag<T>(col);
        }
        if (s_IsConcurrentQueue)
        {
            return new ConcurrentQueue<T>(col);
        }
        if (s_IsConcurrentStack)
        {
            return new ConcurrentStack<T>(col);
        }
        // ReadOnlyCollection<T> has no parameterless constructor, so it wraps the materialized list instead
        if (s_IsReadOnlyCollection)
        {
            return new ReadOnlyCollection<T>(col);
        }
        // Interface members that reach this formatter (IReadOnlyCollection<T>, IReadOnlyList<T>, ...) are satisfied by the list itself
        if (s_IsInterface)
        {
            return col;
        }
        if (s_IsICollection)
        {
            var instance = (TCollection) CreateInstance();
            var collection = (ICollection<T>) instance;
            foreach (var item in col)
            {
                collection.Add(item);
            }
            return (TCollection) collection;
        }
        if (s_IsIProducerConsumerCollection)
        {
            var instance = (TCollection) CreateInstance();
            var collection = (IProducerConsumerCollection<T>) instance;
            foreach (var item in col)
            {
                collection.TryAdd(item);
            }
            return (TCollection) collection;
        }
        throw SerdeException.ThrowNotSupportedType(typeof(TCollection));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static object CreateInstance()
    {
        // TCollection is not constrained to new(), so its parameterless constructor is resolved once at type initialization
        if (s_ParameterlessCtor == null)
        {
            throw SerdeException.ThrowNotSupportedType(typeof(TCollection));
        }
        return s_ParameterlessCtor.Invoke(null);
    }
}