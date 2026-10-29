/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/18
 * Description: ReusableBufferWriter
 */

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde;

/// <summary>
/// Variable-length buffer writer
/// </summary>
/// <typeparam name="T">Buffer element type</typeparam>
public sealed class ReusableBufferWriter<T> : IBufferWriter<T> where T : unmanaged
{
    internal static ArrayPool<T> cachePool = ArrayPool<T>.Shared;

    public static class Pool
    {
        private static readonly ConcurrentQueue<ReusableBufferWriter<T>> s_Queue = new();

        /// <summary>
        /// Rents an idle Buffer writer from the pool
        /// </summary>
        public static ReusableBufferWriter<T> Rent()
        {
            if (s_Queue.TryDequeue(out var writer))
            {
                return writer;
            }
            return new ReusableBufferWriter<T>();
        }

        /// <summary>
        /// Returns and resets the Buffer writer
        /// </summary>
        public static void Return(ReusableBufferWriter<T> writer)
        {
            writer.Reset();
            s_Queue.Enqueue(writer);
        }
    }

    /// <summary>Default Buffer size. Each subsequent growth doubles the size.</summary>
    private const int initial_buffer_size = 262144; // 256KB

    /// <summary>Currently available Buffer, created on the first GetSpan call and reallocated after growth</summary>
    private BufferSegment<T> m_CurrentBuffer;

    /// <summary>Default size for the next CurrentBuffer growth</summary>
    private int? m_NextBufferSize;

    /// <summary>
    /// Total written length
    /// </summary>
    public int totalWritten
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    /// <summary>
    /// Gets an available free memory region
    /// </summary>
    /// <param name="sizeHint">Memory length</param>
    public Span<T> GetSpan(int sizeHint = 0)
    {
        // If CurrentBuffer is large enough, return CurrentBuffer
        if (!m_CurrentBuffer.isNull)
        {
            var buffer = m_CurrentBuffer.freeBuffer;
            if (buffer.Length > sizeHint)
            {
                return buffer;
            }
        }
        // CurrentBuffer is not large enough, allocate a new Buffer
        BufferSegment<T> next;
        m_NextBufferSize ??= initial_buffer_size;
        if (sizeHint <= m_NextBufferSize)
        {
            next = new BufferSegment<T>(m_NextBufferSize.Value, cachePool);
            m_NextBufferSize = MathUtil.NewArrayCapacity(m_NextBufferSize.Value);
        }
        else
        {
            next = new BufferSegment<T>(sizeHint, cachePool);
        }
        // Replace CurrentBuffer with NextBuffer
        // If the old CurrentBuffer already holds data, copy it to the new Buffer
        if (m_CurrentBuffer.writtenCount != 0)
        {
            m_CurrentBuffer.writtenBuffer.CopyTo(next.freeBuffer);
            next.Advance(m_CurrentBuffer.writtenCount);
        }
        m_CurrentBuffer.Free();
        m_CurrentBuffer = next;
        return next.freeBuffer;
    }

    Memory<T> IBufferWriter<T>.GetMemory(int sizeHint) => throw new NotSupportedException(); // GetMemory is not used

    /// <summary>
    /// Advances the writer
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        m_CurrentBuffer.Advance(count);
        totalWritten += count;
    }

    /// <summary>
    /// Gets the already written Buffer
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> GetWrittenBuffer() => m_CurrentBuffer.isNull ? ReadOnlySpan<T>.Empty : m_CurrentBuffer.writtenBuffer;

    /// <summary>
    /// Converts the written part of the Buffer into an array
    /// </summary>
    public T[] CopyToArray()
    {
        if (totalWritten == 0) return Array.Empty<T>();

        var result = new T[totalWritten];
        if (!m_CurrentBuffer.isNull)
        {
            m_CurrentBuffer.writtenBuffer.CopyTo(result.AsSpan());
        }
        return result;
    }

    /// <summary>
    /// Clears the buffer and releases the Buffer
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        if (totalWritten == 0) return;
        m_CurrentBuffer.Free();
        m_CurrentBuffer = default;
        totalWritten = 0;
        m_NextBufferSize = initial_buffer_size;
    }
}