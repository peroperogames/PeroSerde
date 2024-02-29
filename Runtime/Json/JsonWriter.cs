/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/26
 * Description: JsonWriter
 */

#pragma warning disable CA2252 // Opt in to preview features before using them

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    public ref partial struct JsonWriter<TSymbol, TBufferWriter>
        where TSymbol : unmanaged // char or byte
        where TBufferWriter : class, IBufferWriter<TSymbol>
    {
        private readonly TBufferWriter m_BufferWriter;

        // C# 10之后可以使用 private ref TSymbol m_BufferRef;
        private Span<TSymbol> m_BufferRef;
        private int m_AdvancedCount;
        private int m_WrittenCount;
        private readonly bool m_WriteUtf8;

        // TODO Options

        public JsonWriter(TBufferWriter writer)
        {
            m_BufferWriter = writer;
            m_BufferRef = default;
            m_AdvancedCount = 0;
            m_WrittenCount = 0;
            m_WriteUtf8 = typeof(TSymbol) == typeof(byte);
        }

        #region Buffer Op

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref T GetSpanRef<T>(int sizeHint) where T : unmanaged
        {
            if (m_BufferRef.Length < sizeHint)
            {
                RequestNewBuffer(sizeHint);
            }
            return ref Unsafe.As<TSymbol, T>(ref m_BufferRef.GetPinnableReference());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe T* GetSpanPtr<T>(int sizeHint) where T : unmanaged
        {
            if (m_BufferRef.Length < sizeHint)
            {
                RequestNewBuffer(sizeHint);
            }
            return (T*) Unsafe.AsPointer(ref m_BufferRef.GetPinnableReference());
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

        public void WriteValue<T>(string key, T value, bool writeComma = true)
        {
            if (m_WriteUtf8)
            {
                WriteValueUtf8(key, value, writeComma);
            }
            else
            {
                WriteValueUtf16(key, value, writeComma);
            }
        }

        #region Write Literal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBeginObject() => WriteSingleChar('{');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEndObject() => WriteSingleChar('}');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBeginArray() => WriteSingleChar('[');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEndArray() => WriteSingleChar(']');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteSingleChar(char c)
        {
            if (m_WriteUtf8)
                GetSpanRef<byte>(1) = (byte) c;
            else
                GetSpanRef<char>(1) = c;
            Advance(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteNull()
        {
            // 性能测试表明, 少量字节拷贝的速度远不如直接一个一个赋值..
            if (m_WriteUtf8)
            {
                var s = GetSpanPtr<byte>(4);
                s[0] = (byte) 'n';
                s[1] = (byte) 'u';
                s[2] = (byte) 'l';
                s[3] = (byte) 'l';
            }
            else
            {
                var s = GetSpanPtr<char>(4);
                s[0] = 'n';
                s[1] = 'u';
                s[2] = 'l';
                s[3] = 'l';
            }
            Advance(4);
        }

        #endregion

        #region Write Indent

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int WriteIndentUnsafe(int count, char* dest)
        {
            for (var i = 0; i < count; i++)
            {
                dest[i] = ' ';
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int WriteNewLineUnsafe(char* dest)
        {
            *dest = '\n';
            return 1;
        }

        #endregion
    }
}