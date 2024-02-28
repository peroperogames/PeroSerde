/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/18
 * Description: MemoryPackWriterOptionalState
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PeroTools2.Serde.MemoryPack
{
    public static class MemoryPackWriterOptionalStatePool
    {
        private static readonly ConcurrentQueue<MemoryPackWriterOptionalState> m_Queue = new();

        public static MemoryPackWriterOptionalState Rent(MemoryPackSerializerOptions options)
        {
            if (!m_Queue.TryDequeue(out var state))
            {
                state = new MemoryPackWriterOptionalState();
            }
            state.Init(options);
            return state;
        }

        internal static void Return(MemoryPackWriterOptionalState state)
        {
            state.Reset();
            m_Queue.Enqueue(state);
        }
    }

    public sealed class MemoryPackWriterOptionalState : IDisposable
    {
        internal static readonly MemoryPackWriterOptionalState nullState = new(true);

        private uint m_NextId;
        private readonly Dictionary<object, uint> m_ObjToRef;

        public MemoryPackSerializerOptions options { get; private set; }

        internal MemoryPackWriterOptionalState()
        {
            m_ObjToRef = new Dictionary<object, uint>(ReferenceEqualityComparer.instance);
            options = null;
            m_NextId = 0;
        }

        private MemoryPackWriterOptionalState(bool _)
        {
            m_ObjToRef = null;
            options = MemoryPackSerializerOptions.Default;
            m_NextId = 0;
        }

        internal void Init(MemoryPackSerializerOptions op)
        {
            options = op ?? MemoryPackSerializerOptions.Default;
        }

        public (bool existsReference, uint id) GetOrAddReference(object value)
        {
            if (m_ObjToRef.TryGetValue(value, out var id))
                return (true, id);
            id = m_NextId++;
            m_ObjToRef.Add(value, id);
            return (false, id);
        }

        public void Reset()
        {
            m_ObjToRef.Clear();
            options = null;
            m_NextId = 0;
        }

        void IDisposable.Dispose()
        {
            MemoryPackWriterOptionalStatePool.Return(this);
        }
    }
}