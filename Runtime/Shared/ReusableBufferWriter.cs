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

namespace PeroTools2.Serde
{
    public sealed class ReusableBufferWriter<T> : IBufferWriter<T> where T : unmanaged
    {
        public static class Pool
        {
            private static readonly ConcurrentQueue<ReusableBufferWriter<T>> s_Queue = new();

            public static ReusableBufferWriter<T> Rent()
            {
                if (s_Queue.TryDequeue(out var writer))
                {
                    return writer;
                }
                return new ReusableBufferWriter<T>();
            }

            public static void Return(ReusableBufferWriter<T> writer)
            {
                writer.Reset();
                s_Queue.Enqueue(writer);
            }
        }

        /// <summary>Buffer的默认大小. Buffer之后每次增长, Size变为两倍</summary>
        private const int initial_buffer_size = 262144; // 256KB

        /// <summary>当前可用的Buffer, 会在第一次GetSpan时创建, 并在增长后重新分配</summary>
        private BufferSegment<T> m_CurrentBuffer;

        /// <summary>下次CurrentBuffer增长的默认大小</summary>
        private int? m_NextBufferSize;

        public int totalWritten { get; private set; }

        public Span<T> GetSpan(int sizeHint = 0)
        {
            // 如果CurrentBuffer够用, 返回CurrentBuffer
            if (!m_CurrentBuffer.isNull)
            {
                var buffer = m_CurrentBuffer.freeBuffer;
                if (buffer.Length > sizeHint)
                {
                    return buffer;
                }
            }
            // CurrentBuffer不够用, 申请新Buffer
            BufferSegment<T> next;
            m_NextBufferSize ??= initial_buffer_size;
            if (sizeHint <= m_NextBufferSize)
            {
                next = new BufferSegment<T>(m_NextBufferSize.Value);
                m_NextBufferSize = MathUtil.NewArrayCapacity(m_NextBufferSize.Value);
            }
            else
            {
                next = new BufferSegment<T>(sizeHint);
            }
            // 使用NextBuffer替换CurrentBuffer
            // 旧的CurrentBuffer, 如果已经写入了一些数据, 则需要拷贝到新Buffer
            if (m_CurrentBuffer.writtenCount != 0)
            {
                m_CurrentBuffer.writtenBuffer.CopyTo(next.freeBuffer);
                next.Advance(m_CurrentBuffer.writtenCount);
            }
            m_CurrentBuffer.Free();
            m_CurrentBuffer = next;
            return next.freeBuffer;
        }

        Memory<T> IBufferWriter<T>.GetMemory(int sizeHint)
            => throw new NotSupportedException(); // 不使用GetMemory

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            m_CurrentBuffer.Advance(count);
            totalWritten += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> GetWrittenBuffer() => m_CurrentBuffer.isNull ? ReadOnlySpan<T>.Empty : m_CurrentBuffer.writtenBuffer;

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

        // public async ValueTask WriteToStreamAndResetAsync(Stream stream, CancellationToken ct)
        // {
        //     if (totalWritten == 0) return;
        //     if (m_OldBuffers.Count != 0)
        //     {
        //         foreach (var buffer in m_OldBuffers)
        //         {
        //             await stream.WriteAsync(MemoryMarshal.Cast<T, byte>(buffer.writtenMemory), ct).ConfigureAwait(false);
        //         }
        //     }
        //     if (!m_CurrentBuffer.isNull)
        //     {
        //         await stream.WriteAsync(m_CurrentBuffer.writtenMemory, ct).ConfigureAwait(false);
        //     }
        //     Reset();
        // }

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
}