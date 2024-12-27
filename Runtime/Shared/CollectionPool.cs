/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde
{
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

    internal sealed class StackPool<T>
    {
        public static readonly StackPool<T> shared = new();
        private static readonly ConcurrentQueue<Stack<T>> s_Queue = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Stack<T> Rent() => s_Queue.TryDequeue(out var collection) ? collection : new Stack<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(Stack<T> collection)
        {
            collection.Clear();
            s_Queue.Enqueue(collection);
        }
    }
}