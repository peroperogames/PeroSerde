/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/18
 * Description: MemoryPackWriter
 */

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace PeroTools2.Serde.MemoryPack
{
    public ref partial struct MemoryPackWriter<TBufferWriter>
        where TBufferWriter : class, IBufferWriter<byte>
    {
        private const int depth_limit = 1000;

        private readonly TBufferWriter m_BufferWriter;
        private Span<byte> m_BufferRef;
        private int m_AdvancedCount;
        private int m_Depth;
        private readonly bool m_SerializeStringAsUtf8;

        public int writtenCount { get; private set; }
        public int bufferLength { get; private set; }
        public MemoryPackWriterOptionalState optionalState { get; private set; }
        public readonly MemoryPackSerializerOptions options => optionalState.options;

        public MemoryPackWriter(TBufferWriter writer, MemoryPackWriterOptionalState optionalState)
        {
            m_BufferWriter = writer;
            m_BufferRef = default;
            bufferLength = 0;
            m_AdvancedCount = 0;
            writtenCount = 0;
            m_Depth = 0;
            m_SerializeStringAsUtf8 = optionalState.options.stringEncoding == StringEncoding.UTF8;
            this.optionalState = optionalState;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte GetSpanReference(int sizeHint)
        {
            if (bufferLength < sizeHint)
            {
                RequestNewBuffer(sizeHint);
            }
            return ref MemoryMarshal.GetReference(m_BufferRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RequestNewBuffer(int sizeHint)
        {
            if (m_AdvancedCount != 0)
            {
                m_BufferWriter.Advance(m_AdvancedCount);
                m_AdvancedCount = 0;
            }
            var span = m_BufferWriter.GetSpan(sizeHint);
            m_BufferRef = span;
            bufferLength = span.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            if (count == 0) return;
            var rest = bufferLength - count;
            if (rest < 0)
            {
                SerdeException.ThrowInvalidAdvance();
            }
            bufferLength = rest;
            m_BufferRef = m_BufferRef[count..];
            m_AdvancedCount += count;
            writtenCount += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            if (m_AdvancedCount != 0)
            {
                m_BufferWriter.Advance(m_AdvancedCount);
                m_AdvancedCount = 0;
            }
            m_BufferRef = default;
            bufferLength = 0;
            writtenCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetStringWriteLength(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 4;
            return m_SerializeStringAsUtf8
                ? 8 + Encoding.UTF8.GetByteCount(value)
                : 4 + checked(value.Length * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetUnmanagedArrayWriteLength<T>(T[] value) where T : unmanaged =>
            value != null && value.Length != 0
                ? 4 + Unsafe.SizeOf<T>() * value.Length
                : 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMemoryPackFormatter<T> GetFormatter<T>() => MemoryPackFormatterProvider.GetFormatter<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMemoryPackFormatter GetFormatter(Type type) => MemoryPackFormatterProvider.GetFormatter(type);

        #region Write Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteObjectHeader(byte memberCount)
        {
            if (memberCount >= MemoryPackCode.reserved1)
            {
                MemoryPackSerializationException.ThrowWriteInvalidMemberCount(memberCount);
            }
            GetSpanReference(1) = memberCount;
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteNullObjectHeader()
        {
            GetSpanReference(1) = MemoryPackCode.null_object;
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteObjectReferenceId(uint referenceId)
        {
            GetSpanReference(1) = MemoryPackCode.reference_id;
            Advance(1);
            WriteVarInt(referenceId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnionHeader(ushort tag)
        {
            if (tag < MemoryPackCode.wide_tag)
            {
                GetSpanReference(1) = (byte) tag;
                Advance(1);
            }
            else
            {
                ref var spanRef = ref GetSpanReference(3);
                Unsafe.WriteUnaligned(ref spanRef, MemoryPackCode.wide_tag);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref spanRef, 1), tag);
                Advance(3);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteCollectionHeader(int length)
        {
            Unsafe.WriteUnaligned(ref GetSpanReference(4), length);
            Advance(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteNullCollectionHeader()
        {
            Unsafe.WriteUnaligned(ref GetSpanReference(4), MemoryPackCode.null_collection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value)
        {
            if (m_SerializeStringAsUtf8)
                WriteUtf8(value);
            else
                WriteUtf16(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUtf16(string value)
        {
            if (value == null)
            {
                WriteNullCollectionHeader();
                return;
            }
            if (value.Length == 0)
            {
                WriteCollectionHeader(0);
                return;
            }
            // C#的string采用utf16编码, 每个字符长度为2或4,
            // 但C#会始终保证内存中的字节长度为string.Length的二倍.
            // 这是因为需要4字节的文字的Length为2, 例如: "🐂".Length = 2
            var copyByteCount = checked(value.Length * 2);
            ref var dest = ref GetSpanReference(copyByteCount + 4);
            Unsafe.WriteUnaligned(ref dest, value.Length);
            var srcSpan = MemoryMarshal.AsBytes(value.AsSpan());
            var destSpan = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref dest, 4), copyByteCount);
            srcSpan.CopyTo(destSpan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUtf8(string value)
        {
            if (value == null)
            {
                WriteNullCollectionHeader();
                return;
            }
            if (value.Length == 0)
            {
                WriteCollectionHeader(0);
                return;
            }
            var src = value.AsSpan();
            // Encoding.UTF8.GetMaxByteCount()的算法
            var maxByteCount = (src.Length + 1) * 3;
            // write: (int ~utf8_byte_count, int utf16_length, utf8_bytes)
            ref var dest = ref GetSpanReference(maxByteCount + 8); // + header
            // Write UTF16-Length
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref dest, 4), src.Length);
            var destSpan = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref dest, 8), maxByteCount);
            var bytesWritten = Encoding.UTF8.GetBytes(src, destSpan);
            Unsafe.WriteUnaligned(ref dest, ~bytesWritten);
            Advance(bytesWritten + 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePackable<T>(in T value) where T : IMemoryPackable<T> => WriteValue(value);

        public void WriteValue<T>(in T value)
        {
            m_Depth++;
            if (m_Depth == depth_limit)
                MemoryPackSerializationException.ThrowReachedDepthLimit(value.GetType());
            GetFormatter<T>().Serialize(ref this, ref Unsafe.AsRef(value));
            m_Depth--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteValue(Type type, object value)
        {
            m_Depth++;
            if (m_Depth == depth_limit)
                MemoryPackSerializationException.ThrowReachedDepthLimit(type);
            GetFormatter(type).Serialize(ref this, ref value);
            m_Depth--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteValueWithFormatter<TFormatter, T>(TFormatter formatter, in T value)
            where TFormatter : IMemoryPackFormatter<T>
        {
            m_Depth++;
            if (m_Depth == depth_limit)
                MemoryPackSerializationException.ThrowReachedDepthLimit(value.GetType());
            formatter.Serialize(ref this, ref Unsafe.AsRef(value));
            m_Depth--;
        }

        #endregion

        #region Write Array/Span

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray<T>(T[] array)
        {
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                DangerousWriteUnmanagedArray(array);
                return;
            }
            if (array == null)
            {
                WriteNullCollectionHeader();
                return;
            }
            WriteCollectionHeader(array.Length);
            var formatter = GetFormatter<T>();
            for (var i = 0; i < array.Length; i++)
            {
                formatter.Serialize(ref this, ref array[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSpan<T>(Span<T> span)
        {
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                DangerousWriteUnmanagedSpan<T>(span);
                return;
            }
            WriteCollectionHeader(span.Length);
            var formatter = GetFormatter<T>();
            for (var i = 0; i < span.Length; i++)
            {
                formatter.Serialize(ref this, ref span[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSpan<T>(ReadOnlySpan<T> span)
        {
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                DangerousWriteUnmanagedSpan(span);
                return;
            }
            WriteCollectionHeader(span.Length);
            var formatter = GetFormatter<T>();
            for (var i = 0; i < span.Length; i++)
            {
                formatter.Serialize(ref this, ref Unsafe.AsRef(span[i]));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePackableArray<T>(T[] array) where T : IMemoryPackable<T>
        {
            WriteArray(array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePackableSpan<T>(Span<T> span) where T : IMemoryPackable<T>
        {
            WriteSpan(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePackableSpan<T>(ReadOnlySpan<T> span) where T : IMemoryPackable<T>
        {
            WriteSpan(span);
        }

        public void WriteSpanWithoutLengthHeader<T>(ReadOnlySpan<T> span)
        {
            if (span.Length == 0) return;
            if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                var srcLength = Unsafe.SizeOf<T>() * span.Length;
                ref var dest = ref GetSpanReference(srcLength);
                ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span));
                Unsafe.CopyBlockUnaligned(ref dest, ref src, (uint) srcLength);
                Advance(srcLength);
            }
            else
            {
                var formatter = GetFormatter<T>();
                for (var i = 0; i < span.Length; i++)
                {
                    formatter.Serialize(ref this, ref Unsafe.AsRef(span[i]));
                }
            }
        }

        #endregion

        #region Write Unmanaged Array/Span

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanagedArray<T>(T[] array) where T : unmanaged
        {
            DangerousWriteUnmanagedArray(array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnmanagedSpan<T>(ReadOnlySpan<T> span) where T : unmanaged
        {
            DangerousWriteUnmanagedSpan(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanagedArray<T>(T[] array)
        {
            if (array == null)
            {
                WriteNullCollectionHeader();
                return;
            }
            DangerousWriteUnmanagedSpan<T>(array.AsSpan());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DangerousWriteUnmanagedSpan<T>(ReadOnlySpan<T> span)
        {
            if (span.Length == 0)
            {
                WriteCollectionHeader(0);
                return;
            }
            var srcLength = Unsafe.SizeOf<T>() * span.Length;
            var allocSize = srcLength + 4;

            ref var dest = ref GetSpanReference(allocSize);
            ref var src = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span));

            Unsafe.WriteUnaligned(ref dest, span.Length);
            Unsafe.CopyBlockUnaligned(ref Unsafe.Add(ref dest, 4), ref src, (uint) srcLength);
            Advance(allocSize);
        }

        #endregion
    }
}