/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde;

/// <summary>
/// Pool of List objects
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class ListPool<T>
{
    public static readonly ListPool<T> shared = new();
    private static readonly ConcurrentQueue<List<T>> s_Queue = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<T> Rent() => s_Queue.TryDequeue(out var collection) ? collection : new List<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Return(List<T> collection)
    {
        collection.Clear();
        s_Queue.Enqueue(collection);
    }
}