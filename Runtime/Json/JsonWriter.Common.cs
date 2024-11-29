using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PeroTools2.Serde.Json.Formatter;

namespace PeroTools2.Serde.Json
{
    public ref struct JsonWriterUtf8<TBufferWriter> where TBufferWriter : class, IBufferWriter<byte>
    {
        private readonly TBufferWriter m_BufferWriter;

        private Span<byte> m_BufferRef;
        private int m_AdvancedCount;
        private int m_WrittenCount;

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
                var requiredSize = key.Length + formatter.GetMaxRequiredSizeUtf8() + 2; // 2 是 : 和 , 的大小
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
                var requiredSize = key.Length + formatter.GetMaxRequiredSizeUtf8() + 2; // 2 是 : 和 , 的大小
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
                var requiredSize = formatter.GetMaxRequiredSizeUtf8() + 1;
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
                var requiredSize = formatter.GetMaxRequiredSizeUtf8() + 1;
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

        #region Write List

        public unsafe void WriteNamedList<T>(string key, IList<T> value)
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
                // + 1 是 逗号, + 6 是双引号和 : 和 [ 和 ] 的大小
                var requiredSize = key.Length + value.Count * (formatter.GetMaxRequiredSizeUtf8() + 1) + 6;
                fixed (byte* startPtr = GetSpan(requiredSize))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
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
                // 4 是双引号和 : 和 , 的大小
                fixed (byte* startPtr = GetSpan(key.Length + 4))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                WriteBeginArray();
                for (var i = 0; i < value.Count - 1; i++)
                {
                    formatter.SerializeUtf8(value[i], ref this);
                    WriteValueEnding();
                }
                // 覆盖最后一个逗号
                formatter.SerializeUtf8(value[^1], ref this);
                WriteEndArray();
            }
        }

        public unsafe void WriteList<T>(IList<T> value)
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
                // + 1 是 逗号, + 2 是 [ 和 ]
                var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf8() + 1) + 2;
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

        public unsafe void WriteNamedReadonlyList<T>(string key, IReadOnlyList<T> value)
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
                // + 1 是 逗号, + 6 是双引号和 : 和 [ 和 ] 的大小
                var requiredSize = key.Length + value.Count * (formatter.GetMaxRequiredSizeUtf8() + 1) + 6;
                fixed (byte* startPtr = GetSpan(requiredSize))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
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
                // 4 是双引号和 : 和 , 的大小
                fixed (byte* startPtr = GetSpan(key.Length + 4))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                WriteBeginArray();
                for (var i = 0; i < value.Count - 1; i++)
                {
                    formatter.SerializeUtf8(value[i], ref this);
                    WriteValueEnding();
                }
                // 覆盖最后一个逗号
                formatter.SerializeUtf8(value[^1], ref this);
                WriteEndArray();
            }
        }

        public unsafe void WriteReadonlyList<T>(IReadOnlyList<T> value)
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
                // + 1 是 逗号, + 2 是 [ 和 ]
                var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf8() + 1) + 2;
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

        #region Write Dictionary

        public unsafe void WriteNamedDictionary<TKey, TValue>(string key, IDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = (byte) '{';
                dest[1] = (byte) '}';
                Advance(2);
                return;
            }

            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            if (keyFormatter.isUnsafeFormatter && valueFormatter.isUnsafeFormatter)
            {
                // 第一个 + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                // 第二个 + 4 { } 和 Key 的双引号的长度
                var length = value.Count * (keyFormatter.GetMaxRequiredSizeUtf8() + valueFormatter.GetMaxRequiredSizeUtf8() + 4) + key.Length + 4;
                fixed (byte* startPtr = GetSpan(length))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    destPtr += JsonWriter.WriteBeginObjectUnsafeUtf8(destPtr);
                    foreach (var kv in value)
                    {
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, kv.Key.ToString());
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        destPtr += valueFormatter.SerializeUtf8Unsafe(kv.Value, destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndObjectUnsafeUtf8(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
                WriteEndObject();
            }
            else if (valueFormatter.isUnsafeFormatter)
            {
                var index = 0;
                // 写入 Key
                fixed (byte* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                foreach (var kv in value)
                {
                    var curKey = kv.Key.ToString();
                    // + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                    var length = curKey.Length + valueFormatter.GetMaxRequiredSizeUtf8() + 4;
                    // 每一次迭代都要确保可分配的内存是足够的
                    fixed (byte* startPtr = GetSpan(length))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, curKey);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        destPtr += valueFormatter.SerializeUtf8Unsafe(kv.Value, destPtr);
                        // 不写入最后一个逗号
                        if (index++ != value.Count - 1)
                        {
                            destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                        }
                        Advance((int) (destPtr - startPtr));
                    }
                }
            }
            else
            {
                WriteBeginObject();
                var index = 0;
                // 写入 Key
                fixed (byte* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                foreach (var kv in value)
                {
                    // Key
                    var curKey = kv.Key.ToString();
                    fixed (byte* startPtr = GetSpan(curKey.Length + 2))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, curKey);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        Advance((int) (destPtr - startPtr));
                    }

                    // Value
                    valueFormatter.SerializeUtf8(kv.Value, ref this);
                    // 不写入最后一个逗号
                    if (index++ != value.Count - 1)
                    {
                        WriteValueEnding();
                    }
                }
                WriteEndObject();
            }
        }

        public unsafe void WriteDictionary<TKey, TValue>(IDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = (byte) '{';
                dest[1] = (byte) '}';
                Advance(2);
                return;
            }

            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            if (keyFormatter.isUnsafeFormatter && valueFormatter.isUnsafeFormatter)
            {
                // 第一个 + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                // + 2 { } 的长度
                var length = value.Count * (keyFormatter.GetMaxRequiredSizeUtf8() + valueFormatter.GetMaxRequiredSizeUtf8() + 4) + 2;
                fixed (byte* startPtr = GetSpan(length))
                {
                    var destPtr = startPtr;
                    JsonWriter.WriteBeginObjectUnsafeUtf8(destPtr);
                    foreach (var kv in value)
                    {
                        // Key
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, kv.Key.ToString());
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        // Value
                        destPtr += valueFormatter.SerializeUtf8Unsafe(kv.Value, destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndObjectUnsafeUtf8(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
                WriteEndObject();
            }
            else if (valueFormatter.isUnsafeFormatter)
            {
                var index = 0;
                foreach (var kv in value)
                {
                    var key = kv.Key.ToString();
                    // + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的长度
                    var length = key.Length + valueFormatter.GetMaxRequiredSizeUtf8() + 4;
                    // 每一次迭代都要确保可分配的内存是足够的
                    fixed (byte* startPtr = GetSpan(length))
                    {
                        var destPtr = startPtr;
                        // Key
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        // Value
                        destPtr += valueFormatter.SerializeUtf8Unsafe(kv.Value, destPtr);
                        // 不写入最后一个逗号
                        if (index++ != value.Count - 1)
                        {
                            destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                        }
                        Advance((int) (destPtr - startPtr));
                    }
                }
            }
            else
            {
                WriteBeginObject();
                var index = 0;
                foreach (var kv in value)
                {
                    // Key

                    var key = kv.Key.ToString();
                    fixed (byte* startPtr = GetSpan(key.Length + 2))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        Advance((int) (destPtr - startPtr));
                    }

                    // Value
                    valueFormatter.SerializeUtf8(kv.Value, ref this);
                    // 不写入最后一个逗号
                    if (index++ != value.Count - 1)
                    {
                        WriteValueEnding();
                    }
                }
                WriteEndObject();
            }
        }

        public unsafe void WriteNamedReadonlyDictionary<TKey, TValue>(string key, IReadOnlyDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = (byte) '{';
                dest[1] = (byte) '}';
                Advance(2);
                return;
            }

            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            if (keyFormatter.isUnsafeFormatter && valueFormatter.isUnsafeFormatter)
            {
                // 第一个 + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                // 第二个 + 4 { } 和 Key 的双引号的长度
                var length = value.Count * (keyFormatter.GetMaxRequiredSizeUtf8() + valueFormatter.GetMaxRequiredSizeUtf8() + 4) + key.Length + 4;
                fixed (byte* startPtr = GetSpan(length))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    destPtr += JsonWriter.WriteBeginObjectUnsafeUtf8(destPtr);
                    foreach (var kv in value)
                    {
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, kv.Key.ToString());
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        destPtr += valueFormatter.SerializeUtf8Unsafe(kv.Value, destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndObjectUnsafeUtf8(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
                WriteEndObject();
            }
            else if (valueFormatter.isUnsafeFormatter)
            {
                var index = 0;
                // 写入 Key
                fixed (byte* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                foreach (var kv in value)
                {
                    var curKey = kv.Key.ToString();
                    // + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                    var length = curKey.Length + valueFormatter.GetMaxRequiredSizeUtf8() + 4;
                    // 每一次迭代都要确保可分配的内存是足够的
                    fixed (byte* startPtr = GetSpan(length))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, curKey);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        destPtr += valueFormatter.SerializeUtf8Unsafe(kv.Value, destPtr);
                        // 不写入最后一个逗号
                        if (index++ != value.Count - 1)
                        {
                            destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                        }
                        Advance((int) (destPtr - startPtr));
                    }
                }
            }
            else
            {
                WriteBeginObject();
                var index = 0;
                // 写入 Key
                fixed (byte* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                foreach (var kv in value)
                {
                    // Key
                    var curKey = kv.Key.ToString();
                    fixed (byte* startPtr = GetSpan(curKey.Length + 2))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, curKey);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        Advance((int) (destPtr - startPtr));
                    }

                    // Value
                    valueFormatter.SerializeUtf8(kv.Value, ref this);
                    // 不写入最后一个逗号
                    if (index++ != value.Count - 1)
                    {
                        WriteValueEnding();
                    }
                }
                WriteEndObject();
            }
        }

        public unsafe void WriteReadonlyDictionary<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = (byte) '{';
                dest[1] = (byte) '}';
                Advance(2);
                return;
            }

            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            if (keyFormatter.isUnsafeFormatter && valueFormatter.isUnsafeFormatter)
            {
                // 第一个 + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                // + 2 { } 的长度
                var length = value.Count * (keyFormatter.GetMaxRequiredSizeUtf8() + valueFormatter.GetMaxRequiredSizeUtf8() + 4) + 2;
                fixed (byte* startPtr = GetSpan(length))
                {
                    var destPtr = startPtr;
                    JsonWriter.WriteBeginObjectUnsafeUtf8(destPtr);
                    foreach (var kv in value)
                    {
                        // Key
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, kv.Key.ToString());
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        // Value
                        destPtr += valueFormatter.SerializeUtf8Unsafe(kv.Value, destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndObjectUnsafeUtf8(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
                WriteEndObject();
            }
            else if (valueFormatter.isUnsafeFormatter)
            {
                var index = 0;
                foreach (var kv in value)
                {
                    var key = kv.Key.ToString();
                    // + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的长度
                    var length = key.Length + valueFormatter.GetMaxRequiredSizeUtf8() + 4;
                    // 每一次迭代都要确保可分配的内存是足够的
                    fixed (byte* startPtr = GetSpan(length))
                    {
                        var destPtr = startPtr;
                        // Key
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        // Value
                        destPtr += valueFormatter.SerializeUtf8Unsafe(kv.Value, destPtr);
                        // 不写入最后一个逗号
                        if (index++ != value.Count - 1)
                        {
                            destPtr += JsonWriter.WriteValueEndingUnsafeUtf8(destPtr);
                        }
                        Advance((int) (destPtr - startPtr));
                    }
                }
            }
            else
            {
                WriteBeginObject();
                var index = 0;
                foreach (var kv in value)
                {
                    // Key

                    var key = kv.Key.ToString();
                    fixed (byte* startPtr = GetSpan(key.Length + 2))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf8(destPtr, key);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf8(destPtr);
                        Advance((int) (destPtr - startPtr));
                    }

                    // Value
                    valueFormatter.SerializeUtf8(kv.Value, ref this);
                    // 不写入最后一个逗号
                    if (index++ != value.Count - 1)
                    {
                        WriteValueEnding();
                    }
                }
                WriteEndObject();
            }
        }

        #endregion

        #region Write Literal

        /// <summary>写入一个字符串</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteString(string value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            var length = value.Length + 2;
            fixed (byte* desPtr = GetSpan(length))
            {
                JsonWriter.WriteDoubleQuoteUnsafeUtf8(desPtr);
                JsonWriter.WriteStringValueUnsafeUtf8(value.AsSpan(), desPtr);
                JsonWriter.WriteDoubleQuoteUnsafeUtf8(desPtr);
                Advance(length);
            }
        }

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

    public ref struct JsonWriterUtf16<TBufferWriter> where TBufferWriter : class, IBufferWriter<char>
    {
        private readonly TBufferWriter m_BufferWriter;

        private Span<char> m_BufferRef;
        private int m_AdvancedCount;
        private int m_WrittenCount;

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
                var requiredSize = key.Length + formatter.GetMaxRequiredSizeUtf16() + 2; // 2 是 : 和 , 的大小
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
                var requiredSize = key.Length + formatter.GetMaxRequiredSizeUtf16() + 2; // 2 是 : 和 , 的大小
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
                var requiredSize = formatter.GetMaxRequiredSizeUtf16() + 1;
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
                var requiredSize = formatter.GetMaxRequiredSizeUtf16() + 1;
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

        #region Write List

        public unsafe void WriteNamedList<T>(string key, IList<T> value)
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
                // + 1 是 逗号, + 6 是双引号和 : 和 [ 和 ] 的大小
                var requiredSize = key.Length + value.Count * (formatter.GetMaxRequiredSizeUtf16() + 1) + 6;
                fixed (char* startPtr = GetSpan(requiredSize))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
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
                // 4 是双引号和 : 和 , 的大小
                fixed (char* startPtr = GetSpan(key.Length + 4))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                WriteBeginArray();
                for (var i = 0; i < value.Count - 1; i++)
                {
                    formatter.SerializeUtf16(value[i], ref this);
                    WriteValueEnding();
                }
                // 覆盖最后一个逗号
                formatter.SerializeUtf16(value[^1], ref this);
                WriteEndArray();
            }
        }

        public unsafe void WriteList<T>(IList<T> value)
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
                // + 1 是 逗号, + 2 是 [ 和 ]
                var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf16() + 1) + 2;
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

        public unsafe void WriteNamedReadonlyList<T>(string key, IReadOnlyList<T> value)
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
                // + 1 是 逗号, + 6 是双引号和 : 和 [ 和 ] 的大小
                var requiredSize = key.Length + value.Count * (formatter.GetMaxRequiredSizeUtf16() + 1) + 6;
                fixed (char* startPtr = GetSpan(requiredSize))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
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
                // 4 是双引号和 : 和 , 的大小
                fixed (char* startPtr = GetSpan(key.Length + 4))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                WriteBeginArray();
                for (var i = 0; i < value.Count - 1; i++)
                {
                    formatter.SerializeUtf16(value[i], ref this);
                    WriteValueEnding();
                }
                // 覆盖最后一个逗号
                formatter.SerializeUtf16(value[^1], ref this);
                WriteEndArray();
            }
        }

        public unsafe void WriteReadonlyList<T>(IReadOnlyList<T> value)
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
                // + 1 是 逗号, + 2 是 [ 和 ]
                var requiredLength = value.Count * (formatter.GetMaxRequiredSizeUtf16() + 1) + 2;
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

        #region Write Dictionary

        public unsafe void WriteNamedDictionary<TKey, TValue>(string key, IDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = '{';
                dest[1] = '}';
                Advance(2);
                return;
            }

            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            if (keyFormatter.isUnsafeFormatter && valueFormatter.isUnsafeFormatter)
            {
                // 第一个 + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                // 第二个 + 4 { } 和 Key 的双引号的长度
                var length = value.Count * (keyFormatter.GetMaxRequiredSizeUtf16() + valueFormatter.GetMaxRequiredSizeUtf16() + 4) + key.Length + 4;
                fixed (char* startPtr = GetSpan(length))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    destPtr += JsonWriter.WriteBeginObjectUnsafeUtf16(destPtr);
                    foreach (var kv in value)
                    {
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, kv.Key.ToString());
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        destPtr += valueFormatter.SerializeUtf16Unsafe(kv.Value, destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndObjectUnsafeUtf16(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
                WriteEndObject();
            }
            else if (valueFormatter.isUnsafeFormatter)
            {
                var index = 0;
                // 写入 Key
                fixed (char* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                foreach (var kv in value)
                {
                    var curKey = kv.Key.ToString();
                    // + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                    var length = curKey.Length + valueFormatter.GetMaxRequiredSizeUtf16() + 4;
                    // 每一次迭代都要确保可分配的内存是足够的
                    fixed (char* startPtr = GetSpan(length))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, curKey);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        destPtr += valueFormatter.SerializeUtf16Unsafe(kv.Value, destPtr);
                        // 不写入最后一个逗号
                        if (index++ != value.Count - 1)
                        {
                            destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                        }
                        Advance((int) (destPtr - startPtr));
                    }
                }
            }
            else
            {
                WriteBeginObject();
                var index = 0;
                // 写入 Key
                fixed (char* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                foreach (var kv in value)
                {
                    // Key
                    var curKey = kv.Key.ToString();
                    fixed (char* startPtr = GetSpan(curKey.Length + 2))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, curKey);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        Advance((int) (destPtr - startPtr));
                    }

                    // Value
                    valueFormatter.SerializeUtf16(kv.Value, ref this);
                    // 不写入最后一个逗号
                    if (index++ != value.Count - 1)
                    {
                        WriteValueEnding();
                    }
                }
                WriteEndObject();
            }
        }

        public unsafe void WriteDictionary<TKey, TValue>(IDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = '{';
                dest[1] = '}';
                Advance(2);
                return;
            }

            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            if (keyFormatter.isUnsafeFormatter && valueFormatter.isUnsafeFormatter)
            {
                // 第一个 + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                // + 2 { } 的长度
                var length = value.Count * (keyFormatter.GetMaxRequiredSizeUtf16() + valueFormatter.GetMaxRequiredSizeUtf16() + 4) + 2;
                fixed (char* startPtr = GetSpan(length))
                {
                    var destPtr = startPtr;
                    JsonWriter.WriteBeginObjectUnsafeUtf16(destPtr);
                    foreach (var kv in value)
                    {
                        // Key
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, kv.Key.ToString());
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        // Value
                        destPtr += valueFormatter.SerializeUtf16Unsafe(kv.Value, destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndObjectUnsafeUtf16(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
                WriteEndObject();
            }
            else if (valueFormatter.isUnsafeFormatter)
            {
                var index = 0;
                foreach (var kv in value)
                {
                    var key = kv.Key.ToString();
                    // + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的长度
                    var length = key.Length + valueFormatter.GetMaxRequiredSizeUtf16() + 4;
                    // 每一次迭代都要确保可分配的内存是足够的
                    fixed (char* startPtr = GetSpan(length))
                    {
                        var destPtr = startPtr;
                        // Key
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        // Value
                        destPtr += valueFormatter.SerializeUtf16Unsafe(kv.Value, destPtr);
                        // 不写入最后一个逗号
                        if (index++ != value.Count - 1)
                        {
                            destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                        }
                        Advance((int) (destPtr - startPtr));
                    }
                }
            }
            else
            {
                WriteBeginObject();
                var index = 0;
                foreach (var kv in value)
                {
                    // Key

                    var key = kv.Key.ToString();
                    fixed (char* startPtr = GetSpan(key.Length + 2))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        Advance((int) (destPtr - startPtr));
                    }

                    // Value
                    valueFormatter.SerializeUtf16(kv.Value, ref this);
                    // 不写入最后一个逗号
                    if (index++ != value.Count - 1)
                    {
                        WriteValueEnding();
                    }
                }
                WriteEndObject();
            }
        }

        public unsafe void WriteNamedReadonlyDictionary<TKey, TValue>(string key, IReadOnlyDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = '{';
                dest[1] = '}';
                Advance(2);
                return;
            }

            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            if (keyFormatter.isUnsafeFormatter && valueFormatter.isUnsafeFormatter)
            {
                // 第一个 + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                // 第二个 + 4 { } 和 Key 的双引号的长度
                var length = value.Count * (keyFormatter.GetMaxRequiredSizeUtf16() + valueFormatter.GetMaxRequiredSizeUtf16() + 4) + key.Length + 4;
                fixed (char* startPtr = GetSpan(length))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    destPtr += JsonWriter.WriteBeginObjectUnsafeUtf16(destPtr);
                    foreach (var kv in value)
                    {
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, kv.Key.ToString());
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        destPtr += valueFormatter.SerializeUtf16Unsafe(kv.Value, destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndObjectUnsafeUtf16(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
                WriteEndObject();
            }
            else if (valueFormatter.isUnsafeFormatter)
            {
                var index = 0;
                // 写入 Key
                fixed (char* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                foreach (var kv in value)
                {
                    var curKey = kv.Key.ToString();
                    // + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                    var length = curKey.Length + valueFormatter.GetMaxRequiredSizeUtf16() + 4;
                    // 每一次迭代都要确保可分配的内存是足够的
                    fixed (char* startPtr = GetSpan(length))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, curKey);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        destPtr += valueFormatter.SerializeUtf16Unsafe(kv.Value, destPtr);
                        // 不写入最后一个逗号
                        if (index++ != value.Count - 1)
                        {
                            destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                        }
                        Advance((int) (destPtr - startPtr));
                    }
                }
            }
            else
            {
                WriteBeginObject();
                var index = 0;
                // 写入 Key
                fixed (char* startPtr = GetSpan(key.Length + 2))
                {
                    var destPtr = startPtr;
                    destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                    destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                    Advance((int) (destPtr - startPtr));
                }
                foreach (var kv in value)
                {
                    // Key
                    var curKey = kv.Key.ToString();
                    fixed (char* startPtr = GetSpan(curKey.Length + 2))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, curKey);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        Advance((int) (destPtr - startPtr));
                    }

                    // Value
                    valueFormatter.SerializeUtf16(kv.Value, ref this);
                    // 不写入最后一个逗号
                    if (index++ != value.Count - 1)
                    {
                        WriteValueEnding();
                    }
                }
                WriteEndObject();
            }
        }

        public unsafe void WriteReadonlyDictionary<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            if (value.Count == 0)
            {
                var dest = GetSpan(2);
                dest[0] = '{';
                dest[1] = '}';
                Advance(2);
                return;
            }

            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            if (keyFormatter.isUnsafeFormatter && valueFormatter.isUnsafeFormatter)
            {
                // 第一个 + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的保险长度
                // + 2 { } 的长度
                var length = value.Count * (keyFormatter.GetMaxRequiredSizeUtf16() + valueFormatter.GetMaxRequiredSizeUtf16() + 4) + 2;
                fixed (char* startPtr = GetSpan(length))
                {
                    var destPtr = startPtr;
                    JsonWriter.WriteBeginObjectUnsafeUtf16(destPtr);
                    foreach (var kv in value)
                    {
                        // Key
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, kv.Key.ToString());
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        // Value
                        destPtr += valueFormatter.SerializeUtf16Unsafe(kv.Value, destPtr);
                        destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                    }
                    // 覆盖最后一个逗号
                    JsonWriter.WriteEndObjectUnsafeUtf16(destPtr - 1);
                    Advance((int) (destPtr - startPtr));
                }
                WriteEndObject();
            }
            else if (valueFormatter.isUnsafeFormatter)
            {
                var index = 0;
                foreach (var kv in value)
                {
                    var key = kv.Key.ToString();
                    // + 4 是 Key 作为非字符串时 "x2 :x1 ,x1 的长度
                    var length = key.Length + valueFormatter.GetMaxRequiredSizeUtf16() + 4;
                    // 每一次迭代都要确保可分配的内存是足够的
                    fixed (char* startPtr = GetSpan(length))
                    {
                        var destPtr = startPtr;
                        // Key
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        // Value
                        destPtr += valueFormatter.SerializeUtf16Unsafe(kv.Value, destPtr);
                        // 不写入最后一个逗号
                        if (index++ != value.Count - 1)
                        {
                            destPtr += JsonWriter.WriteValueEndingUnsafeUtf16(destPtr);
                        }
                        Advance((int) (destPtr - startPtr));
                    }
                }
            }
            else
            {
                WriteBeginObject();
                var index = 0;
                foreach (var kv in value)
                {
                    // Key

                    var key = kv.Key.ToString();
                    fixed (char* startPtr = GetSpan(key.Length + 2))
                    {
                        var destPtr = startPtr;
                        destPtr += JsonWriter.WriteStringNoEscapeUnsafeUtf16(destPtr, key);
                        destPtr += JsonWriter.WriteKeyEndingUnsafeUtf16(destPtr);
                        Advance((int) (destPtr - startPtr));
                    }

                    // Value
                    valueFormatter.SerializeUtf16(kv.Value, ref this);
                    // 不写入最后一个逗号
                    if (index++ != value.Count - 1)
                    {
                        WriteValueEnding();
                    }
                }
                WriteEndObject();
            }
        }

        #endregion

        #region Write Literal

        /// <summary>写入一个字符串</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteString(string value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }
            var length = value.Length + 2;
            fixed (char* desPtr = GetSpan(length))
            {
                JsonWriter.WriteDoubleQuoteUnsafeUtf16(desPtr);
                JsonWriter.WriteStringValueUnsafeUtf16(value.AsSpan(), desPtr);
                JsonWriter.WriteDoubleQuoteUnsafeUtf16(desPtr);
                Advance(length);
            }
        }

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