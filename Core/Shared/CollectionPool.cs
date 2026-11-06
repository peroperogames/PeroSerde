/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde;

/// <summary>
/// Pool of List objects
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// The cache is per thread. A concurrent queue costs a CAS loop plus a cache line transfer on every rent and return,
/// which is more than the pooled list saves on a five element array, so the pool has to stay off the shared path to be
/// worth having at all.
/// </remarks>
internal sealed class ListPool<T>
{
    private const int max_depth = 8;

    /// <summary>Lists grown past this many elements are dropped instead of pooled.</summary>
    private const int max_capacity = 16 * 1024;

    public static readonly ListPool<T> shared = new();

    // ReSharper disable once StaticMemberInGenericType
    [ThreadStatic] private static List<T>[] s_Items;

    // ReSharper disable once StaticMemberInGenericType
    [ThreadStatic] private static int s_Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<T> Rent()
    {
        var items = s_Items;
        if (items != null && s_Count > 0)
        {
            var index = --s_Count;
            var list = items[index];
            items[index] = null;
            return list;
        }
        return new List<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Return(List<T> collection)
    {
        // Clear keeps the internal array, so a single huge array would stay on this thread for the rest of the process
        if (collection.Capacity > max_capacity)
        {
            return;
        }

        collection.Clear();
        var items = s_Items ??= new List<T>[max_depth];
        if (s_Count >= items.Length)
        {
            // Nesting deeper than the cache holds drops the list instead of growing the cache
            return;
        }
        items[s_Count++] = collection;
    }
}
