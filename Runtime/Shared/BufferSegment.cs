/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/18
 * Description: BufferSegment
 */

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde
{
    /// <summary>
    /// 从<see cref="ArrayPool{T}"/>中借用的一个不可变Buffer.
    /// </summary>
    internal struct BufferSegment<T> where T : unmanaged
    {
        private T[] m_Buffer;
        private int m_Written;
        public ArrayPool<T> pool;

        public readonly bool isNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Buffer == null;
        }

        public readonly int writtenCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Written;
        }

        public readonly Span<T> writtenBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Buffer.AsSpan(0, m_Written);
        }

        public readonly Memory<T> writtenMemory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Buffer.AsMemory(0, m_Written);
        }

        public readonly Span<T> freeBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Buffer.AsSpan(m_Written);
        }

        public BufferSegment(int size, ArrayPool<T> pool)
        {
            this.pool = pool;
            m_Buffer = pool.Rent(size);
            m_Written = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count) => m_Written += count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free()
        {
            if (m_Buffer != null)
            {
                pool.Return(m_Buffer);
            }
            m_Buffer = null;
            m_Written = 0;
        }
    }
}