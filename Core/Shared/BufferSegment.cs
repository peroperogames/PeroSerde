/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/18
 * Description: BufferSegment
 */

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde;

/// <summary>
/// An immutable buffer rented from <see cref="ArrayPool{T}"/>.
/// </summary>
internal struct BufferSegment<T>(int size, ArrayPool<T> pool)
    where T : unmanaged
{
    private T[] m_Buffer = pool.Rent(size);
    private int m_Written = 0;
    public ArrayPool<T> pool = pool;

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