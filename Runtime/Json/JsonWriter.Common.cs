using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PeroTools2.Serde.Json.Formatter;

namespace PeroTools2.Serde.Json
{
    public ref struct JsonWriterUtf8<TBufferWriter>
        where TBufferWriter : class, IBufferWriter<byte>
    {
        private readonly TBufferWriter m_BufferWriter;

        // C# 10之后可以使用 private ref byte m_BufferRef;
        private Span<byte> m_BufferRef;
        private int m_AdvancedCount;
        private int m_WrittenCount;

        // TODO Options

        public JsonWriterUtf8(TBufferWriter writer)
        {
            m_BufferWriter = writer;
            m_BufferRef = default;
            m_AdvancedCount = 0;
            m_WrittenCount = 0;
        }

        #region Buffer Op

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref byte GetSpanRef(int sizeHint)
        {
            if (m_BufferRef.Length < sizeHint)
            {
                RequestNewBuffer(sizeHint);
            }
            return ref m_BufferRef.GetPinnableReference();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span<byte> GetSpan(int sizeHint)
        {
            if (m_BufferRef.Length < sizeHint)
            {
                RequestNewBuffer(sizeHint);
            }
            return m_BufferRef;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RequestNewBuffer(int sizeHint)
        {
            if (m_AdvancedCount != 0)
            {
                m_BufferWriter.Advance(m_AdvancedCount);
                m_AdvancedCount = 0;
            }
            m_BufferRef = m_BufferWriter.GetSpan(sizeHint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int count)
        {
            if (count == 0) return;
            if (m_BufferRef.Length - count < 0)
            {
                SerdeException.ThrowInvalidAdvance();
            }
            m_BufferRef = m_BufferRef.Slice(count);
            m_AdvancedCount += count;
            m_WrittenCount += count;
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
            m_WrittenCount = 0;
        }

        #endregion

        #region Write Value

        public unsafe void WriteNamedValue<T>(string key, T value, SerdeJsonFormatter<T> formatter, bool writeComma = true)
        {
            if (formatter.isUnsafeFormatter)
            {
                var requiredSize = key.Length + 2 + formatter.GetMaxRequiredSizeUtf8(value) + 2; // 2 是 : 和 , 的大小
                fixed (byte* startPtr = GetSpan(requiredSize))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    destPtr += formatter.SerializeUtf8Unsafe(value, destPtr);
                    if (writeComma)
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                fixed (byte* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                formatter.SerializeUtf8(value, ref this);
                if (writeComma)
                    WriteValueEnding();
            }
        }

        public unsafe void WriteNamedValue<T>(string key, T value, bool writeComma = true)
        {
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredSize = key.Length + 2 + formatter.GetMaxRequiredSizeUtf8(value) + 2; // 2 是 : 和 , 的大小
                fixed (byte* startPtr = GetSpan(requiredSize))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    destPtr += formatter.SerializeUtf8Unsafe(value, destPtr);
                    if (writeComma)
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                fixed (byte* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                formatter.SerializeUtf8(value, ref this);
                if (writeComma)
                    WriteValueEnding();
            }
        }

        public unsafe void WriteValue<T>(T value, SerdeJsonFormatter<T> formatter)
        {
            if (formatter.isUnsafeFormatter)
            {
                var requiredSize = formatter.GetMaxRequiredSizeUtf8(value) + 1;
                fixed (byte* destPtr = GetSpan(requiredSize))
                {
                    var bytesWritten = formatter.SerializeUtf8Unsafe(value, destPtr);
                    Advance(bytesWritten);
                }
            }
            else
            {
                formatter.SerializeUtf8(value, ref this);
            }
        }

        public unsafe void WriteValue<T>(T value)
        {
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredSize = formatter.GetMaxRequiredSizeUtf8(value) + 1;
                fixed (byte* destPtr = GetSpan(requiredSize))
                {
                    var bytesWritten = formatter.SerializeUtf8Unsafe(value, destPtr);
                    Advance(bytesWritten);
                }
            }
            else
            {
                formatter.SerializeUtf8(value, ref this);
            }
        }

        #endregion

        #region Write Array

        public unsafe void WriteArray<T>(T[] value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Length == 0)
            {
                var dest = GetSpan(2);
                dest[0] = (byte) '[';
                dest[1] = (byte) ']';
                Advance(2);
                return;
            }
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredLength = value.Length * (formatter.GetMaxRequiredSizeUtf8(value[0]) + 1) + 2;
                fixed (byte* startPtr = GetSpan(requiredLength))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteBeginArrayUnsafeUtf8(destPtr);
                    for (var i = 0; i < value.Length; i++)
                    {
                        destPtr += formatter.SerializeUtf8Unsafe(value[i], destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndArrayUnsafeUtf8(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                WriteBeginArray();
                for (var i = 0; i < value.Length - 1; i++)
                {
                    formatter.SerializeUtf8(value[i], ref this);
                    WriteValueEnding();
                }
                formatter.SerializeUtf8(value[^1], ref this);
                WriteEndArray();
            }
        }

        public unsafe void WriteArray<T>(IList<T> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = (byte) '[';
                dest[1] = (byte) ']';
                Advance(2);
                return;
            }
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf8(value[0]) + 1) + 2;
                fixed (byte* startPtr = GetSpan(requiredLength))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteBeginArrayUnsafeUtf8(destPtr);
                    for (var i = 0; i < value.Count; i++)
                    {
                        destPtr += formatter.SerializeUtf8Unsafe(value[i], destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndArrayUnsafeUtf8(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                WriteBeginArray();
                for (var i = 0; i < value.Count - 1; i++)
                {
                    formatter.SerializeUtf8(value[i], ref this);
                    WriteValueEnding();
                }
                formatter.SerializeUtf8(value[^1], ref this);
                WriteEndArray();
            }
        }

        public unsafe void WriteArray<T>(IReadOnlyList<T> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = (byte) '[';
                dest[1] = (byte) ']';
                Advance(2);
                return;
            }
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf8(value[0]) + 1) + 2;
                fixed (byte* startPtr = GetSpan(requiredLength))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteBeginArrayUnsafeUtf8(destPtr);
                    for (var i = 0; i < value.Count; i++)
                    {
                        destPtr += formatter.SerializeUtf8Unsafe(value[i], destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndArrayUnsafeUtf8(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                WriteBeginArray();
                for (var i = 0; i < value.Count - 1; i++)
                {
                    formatter.SerializeUtf8(value[i], ref this);
                    WriteValueEnding();
                }
                formatter.SerializeUtf8(value[^1], ref this);
                WriteEndArray();
            }
        }

        #endregion

        #region Write Literal

        /// <summary>写入一个英文冒号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteKeyEnding()
        {
            GetSpanRef(1) = (byte) ':';
            Advance(1);
        }

        /// <summary>写入一个英文逗号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteValueEnding()
        {
            GetSpanRef(1) = (byte) ',';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDoubleQuote()
        {
            GetSpanRef(1) = (byte) '"';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSpace()
        {
            GetSpanRef(1) = (byte) ' ';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBeginObject()
        {
            GetSpanRef(1) = (byte) '{';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEndObject()
        {
            GetSpanRef(1) = (byte) '}';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBeginArray()
        {
            GetSpanRef(1) = (byte) '[';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEndArray()
        {
            GetSpanRef(1) = (byte) ']';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteNull()
        {
            var dest = GetSpan(4);
            dest[0] = (byte) 'n';
            dest[1] = (byte) 'u';
            dest[2] = (byte) 'l';
            dest[3] = (byte) 'l';
            Advance(4);
        }

        #endregion
    }

    public ref struct JsonWriterUtf16<TBufferWriter>
        where TBufferWriter : class, IBufferWriter<char>
    {
        private readonly TBufferWriter m_BufferWriter;

        // C# 10之后可以使用 private ref char m_BufferRef;
        private Span<char> m_BufferRef;
        private int m_AdvancedCount;
        private int m_WrittenCount;

        // TODO Options

        public JsonWriterUtf16(TBufferWriter writer)
        {
            m_BufferWriter = writer;
            m_BufferRef = default;
            m_AdvancedCount = 0;
            m_WrittenCount = 0;
        }

        #region Buffer Op

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref char GetSpanRef(int sizeHint)
        {
            if (m_BufferRef.Length < sizeHint)
            {
                RequestNewBuffer(sizeHint);
            }
            return ref m_BufferRef.GetPinnableReference();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span<char> GetSpan(int sizeHint)
        {
            if (m_BufferRef.Length < sizeHint)
            {
                RequestNewBuffer(sizeHint);
            }
            return m_BufferRef;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RequestNewBuffer(int sizeHint)
        {
            if (m_AdvancedCount != 0)
            {
                m_BufferWriter.Advance(m_AdvancedCount);
                m_AdvancedCount = 0;
            }
            m_BufferRef = m_BufferWriter.GetSpan(sizeHint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int count)
        {
            if (count == 0) return;
            if (m_BufferRef.Length - count < 0)
            {
                SerdeException.ThrowInvalidAdvance();
            }
            m_BufferRef = m_BufferRef.Slice(count);
            m_AdvancedCount += count;
            m_WrittenCount += count;
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
            m_WrittenCount = 0;
        }

        #endregion

        #region Write Value

        public unsafe void WriteNamedValue<T>(string key, T value, SerdeJsonFormatter<T> formatter, bool writeComma = true)
        {
            if (formatter.isUnsafeFormatter)
            {
                var requiredSize = key.Length + formatter.GetMaxRequiredSizeUtf16(value) + 2; // 2 是 : 和 , 的大小
                fixed (char* startPtr = GetSpan(requiredSize))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    destPtr += formatter.SerializeUtf16Unsafe(value, destPtr);
                    if (writeComma)
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                fixed (char* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                formatter.SerializeUtf16(value, ref this);
                if (writeComma)
                    WriteValueEnding();
            }
        }

        public unsafe void WriteNamedValue<T>(string key, T value, bool writeComma = true)
        {
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredSize = key.Length + formatter.GetMaxRequiredSizeUtf16(value) + 2; // 2 是 : 和 , 的大小
                fixed (char* startPtr = GetSpan(requiredSize))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    destPtr += formatter.SerializeUtf16Unsafe(value, destPtr);
                    if (writeComma)
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                fixed (char* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                formatter.SerializeUtf16(value, ref this);
                if (writeComma)
                    WriteValueEnding();
            }
        }

        public unsafe void WriteValue<T>(T value, SerdeJsonFormatter<T> formatter)
        {
            if (formatter.isUnsafeFormatter)
            {
                var requiredSize = formatter.GetMaxRequiredSizeUtf16(value) + 1;
                fixed (char* destPtr = GetSpan(requiredSize))
                {
                    var charsWritten = formatter.SerializeUtf16Unsafe(value, destPtr);
                    Advance(charsWritten);
                }
            }
            else
            {
                formatter.SerializeUtf16(value, ref this);
            }
        }

        public unsafe void WriteValue<T>(T value)
        {
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredSize = formatter.GetMaxRequiredSizeUtf16(value) + 1;
                fixed (char* destPtr = GetSpan(requiredSize))
                {
                    var charsWritten = formatter.SerializeUtf16Unsafe(value, destPtr);
                    Advance(charsWritten);
                }
            }
            else
            {
                formatter.SerializeUtf16(value, ref this);
            }
        }

        #endregion

        #region Write Array

        public unsafe void WriteArray<T>(T[] value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Length == 0)
            {
                var dest = GetSpan(2);
                dest[0] = '[';
                dest[1] = ']';
                Advance(2);
                return;
            }
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredLength = value.Length * (formatter.GetMaxRequiredSizeUtf16(value[0]) + 1) + 2;
                fixed (char* startPtr = GetSpan(requiredLength))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteBeginArrayUnsafeUtf16(destPtr);
                    for (var i = 0; i < value.Length; i++)
                    {
                        destPtr += formatter.SerializeUtf16Unsafe(value[i], destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndArrayUnsafeUtf16(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                WriteBeginArray();
                for (var i = 0; i < value.Length - 1; i++)
                {
                    formatter.SerializeUtf16(value[i], ref this);
                    WriteValueEnding();
                }
                formatter.SerializeUtf16(value[^1], ref this);
                WriteEndArray();
            }
        }

        public unsafe void WriteArray<T>(IList<T> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = '[';
                dest[1] = ']';
                Advance(2);
                return;
            }
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf16(value[0]) + 1) + 2;
                fixed (char* startPtr = GetSpan(requiredLength))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteBeginArrayUnsafeUtf16(destPtr);
                    for (var i = 0; i < value.Count; i++)
                    {
                        destPtr += formatter.SerializeUtf16Unsafe(value[i], destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndArrayUnsafeUtf16(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                WriteBeginArray();
                for (var i = 0; i < value.Count - 1; i++)
                {
                    formatter.SerializeUtf16(value[i], ref this);
                    WriteValueEnding();
                }
                formatter.SerializeUtf16(value[^1], ref this);
                WriteEndArray();
            }
        }

        public unsafe void WriteArray<T>(IReadOnlyList<T> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = '[';
                dest[1] = ']';
                Advance(2);
                return;
            }
            var formatter = SerdeJsonFormatterProvider.GetFormatter<T>();
            if (formatter.isUnsafeFormatter)
            {
                var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf16(value[0]) + 1) + 2;
                fixed (char* startPtr = GetSpan(requiredLength))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteBeginArrayUnsafeUtf16(destPtr);
                    for (var i = 0; i < value.Count; i++)
                    {
                        destPtr += formatter.SerializeUtf16Unsafe(value[i], destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndArrayUnsafeUtf16(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
            }
            else
            {
                WriteBeginArray();
                for (var i = 0; i < value.Count - 1; i++)
                {
                    formatter.SerializeUtf16(value[i], ref this);
                    WriteValueEnding();
                }
                formatter.SerializeUtf16(value[^1], ref this);
                WriteEndArray();
            }
        }

        #endregion

        #region Write Literal

        /// <summary>写入一个英文冒号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteKeyEnding()
        {
            GetSpanRef(1) = ':';
            Advance(1);
        }

        /// <summary>写入一个英文逗号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteValueEnding()
        {
            GetSpanRef(1) = ',';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDoubleQuote()
        {
            GetSpanRef(1) = '"';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSpace()
        {
            GetSpanRef(1) = ' ';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBeginObject()
        {
            GetSpanRef(1) = '{';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEndObject()
        {
            GetSpanRef(1) = '}';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBeginArray()
        {
            GetSpanRef(1) = '[';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEndArray()
        {
            GetSpanRef(1) = ']';
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteNull()
        {
            var dest = GetSpan(4);
            dest[0] = 'n';
            dest[1] = 'u';
            dest[2] = 'l';
            dest[3] = 'l';
            Advance(4);
        }

        #endregion
    }

}