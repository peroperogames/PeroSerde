/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/20
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    public ref struct JsonReaderUtf8
    {
        private int m_Advanced;
        private readonly ReadOnlySpan<byte> m_Json;
        public readonly ReadOnlySpan<byte> json => m_Json;

        public readonly ReadOnlySpan<byte> remains
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Json.Slice(m_Advanced, m_Json.Length - m_Advanced);
        }

        public JsonReaderUtf8(ReadOnlySpan<byte> json)
        {
            m_Advanced = 0;
            m_Json = json;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int count) => m_Advanced += count;

        #region Read Value

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> ReadValue(bool readComma = false)
        {
            var result = ReadUntilEnd();
            if (readComma) ReadComma();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T ReadValue<T>(IFixedSerdeJsonFormatter<T> formatter, bool readComma = false)
        {
            T result;
            var span = remains;
            fixed (byte* ptr = span)
            {
                result = formatter.DeserializeUtf8Unsafe(ptr, out var advance);
                Advance(advance);
            }

            if (readComma) ReadComma();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ReadValue<T>(ISerdeJsonFormatter<T> formatter, bool readComma = false)
        {
            var span = remains;
            var result = formatter.DeserializeUtf8(ref span, ref this);
            if (readComma) ReadComma();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ReadValue<T>(bool readComma = false)
        {
            var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
            if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
                return ReadValue(fixedFormatter, readComma);
            if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
                return ReadValue(unfixedFormatter, readComma);
            throw SerdeException.ThrowFormatterNotFound(typeof(T));
        }

        #endregion

        #region Read Collection

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TCollection ReadCollection<TCollection, T>(ISerdeJsonFormatter<T> formatter, bool readComma = false) where TCollection : ICollection<T>, new()
        {
            if (TryReadNull())
            {
                return default;
            }

            var col = new TCollection();
            ReadCollection(col, formatter, readComma);
            return col;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TCollection ReadCollection<TCollection, T>(IFixedSerdeJsonFormatter<T> formatter, bool readComma = false) where TCollection : ICollection<T>, new()
        {
            if (TryReadNull())
            {
                return default;
            }

            var col = new TCollection();
            ReadCollection(col, formatter, readComma);
            return col;
        }

        internal void ReadCollection<T>(ICollection<T> col, IFixedSerdeJsonFormatter<T> formatter, bool readComma)
        {
            ReadBeginArray();
            while (true)
            {
                ReadWhiteSpace();
                col.Add(ReadValue(formatter));
                ReadWhiteSpace();
                if (m_Json[m_Advanced] == (byte) ',')
                {
                    Advance(1);
                }
                else if (m_Json[m_Advanced] == (byte) ']')
                {
                    Advance(1);
                    break;
                }
            }
            if (readComma) ReadComma();
        }

        internal void ReadCollection<T>(ICollection<T> col, ISerdeJsonFormatter<T> formatter, bool readComma)
        {
            ReadBeginArray();
            while (true)
            {
                ReadWhiteSpace();
                col.Add(ReadValue(formatter));
                ReadWhiteSpace();
                if (m_Json[m_Advanced] == (byte) ',')
                {
                    Advance(1);
                }
                else if (m_Json[m_Advanced] == (byte) ']')
                {
                    Advance(1);
                    break;
                }
            }
            if (readComma) ReadComma();
        }

        #endregion

        #region Read Literal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<byte> ReadUntilEnd()
        {
            var span = remains;
            fixed (byte* src = remains)
            {
                var result = JsonReader.ReadValueUntilEndUtf8(src, span.Length - 1, out var advance);
                Advance(advance);
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadWhiteSpace()
        {
            for (var i = m_Advanced; i < m_Json.Length; i++)
            {
                if (StringHelper.IsWhiteSpace(m_Json[i])) continue;
                Advance(i - m_Advanced);
                return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadBeginObject() => ReadChar('{');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadEndObject() => ReadChar('}');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadBeginArray() => ReadChar('[');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadEndArray() => ReadChar(']');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadKeyBegin() => ReadChar(':');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadComma() => ReadChar(',');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadBeginObject() => TryReadChar('{');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadEndObject() => TryReadChar('}');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadBeginArray() => TryReadChar('[');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadEndArray() => TryReadChar(']');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadKeyBegin() => TryReadChar(':');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadComma() => TryReadChar(',');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadNull()
        {
            var length = m_Json.Length - m_Advanced;
            var span = m_Json.Slice(m_Advanced, length);
            // 因为最大要4个字符，所以长度为Length - 3
            for (var i = 0; i < span.Length - 3; i++)
            {
                if (StringHelper.IsWhiteSpace(span[i])) continue;
                // 找到连续的null
                if (span[i] == (byte) 'n' && span[i + 1] == (byte) 'u' && span[i + 2] == (byte) 'l' && span[i + 3] == (byte) 'l')
                {
                    Advance(i + 4);
                    return true;
                }
                return false;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> ReadKey()
        {
            ReadWhiteSpace();
            var key = ReadString();
            ReadWhiteSpace();
            ReadKeyBegin();
            ReadWhiteSpace();
            return key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadKey(out ReadOnlySpan<byte> key)
        {
            ReadWhiteSpace();
            if (remains[0] != (byte) '"')
            {
                key = default;
                return false;
            }
            key = ReadString();
            ReadWhiteSpace();
            ReadKeyBegin();
            ReadWhiteSpace();
            return true;
        }

        private ReadOnlySpan<byte> ReadString()
        {
            if (m_Json[m_Advanced] != (byte) '"')
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            for (var i = m_Advanced + 1; i < m_Json.Length; i++)
            {
                if (m_Json[i].Equals((byte) '"') && m_Json[i - 1] != (byte) '\\')
                {
                    var result = m_Json.Slice(m_Advanced + 1, i - m_Advanced - 1);
                    Advance(i + 1 - m_Advanced);
                    return result;
                }
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        /// <summary>
        /// 持续读取字符，直到遇到第一个目标字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadChar(char c)
        {
            var length = m_Json.Length - m_Advanced;
            var span = m_Json.Slice(m_Advanced, length);
            if (!TryGetIndexFromSpan(ref span, length, (byte) c, out var index))
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            Advance(index + 1);
        }

        /// <summary>
        /// 持续读取字符，直到遇到第一个目标字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryReadChar(char c)
        {
            var length = m_Json.Length - m_Advanced;
            var span = m_Json.Slice(m_Advanced, length);
            for (var i = 0; i < span.Length; i++)
            {
                if (StringHelper.IsWhiteSpace(span[i])) continue;
                if (span[i] == (byte) c)
                {
                    Advance(i + 1);
                    return true;
                }
                return false;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool TryGetIndexFromSpan(ref ReadOnlySpan<byte> span, int length, byte character, out int index)
        {
            fixed (byte* ptr = span)
            {
                index = StringHelper.IndexOf(ptr, length, character);
            }
            return index != -1;
        }
        #endregion
    }
    public ref struct JsonReaderUtf16
    {
        private int m_Advanced;
        private readonly ReadOnlySpan<char> m_Json;
        public readonly ReadOnlySpan<char> json => m_Json;

        public readonly ReadOnlySpan<char> remains
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Json.Slice(m_Advanced, m_Json.Length - m_Advanced);
        }

        public JsonReaderUtf16(ReadOnlySpan<char> json)
        {
            m_Advanced = 0;
            m_Json = json;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int count) => m_Advanced += count;

        #region Read Value

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadValue(bool readComma = false)
        {
            var result = ReadUntilEnd();
            if (readComma) ReadComma();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T ReadValue<T>(IFixedSerdeJsonFormatter<T> formatter, bool readComma = false)
        {
            T result;
            var span = remains;
            fixed (char* ptr = span)
            {
                result = formatter.DeserializeUtf16Unsafe(ptr, out var advance);
                Advance(advance);
            }

            if (readComma) ReadComma();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ReadValue<T>(ISerdeJsonFormatter<T> formatter, bool readComma = false)
        {
            var span = remains;
            var result = formatter.DeserializeUtf16(ref span, ref this);
            if (readComma) ReadComma();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ReadValue<T>(bool readComma = false)
        {
            var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
            if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
                return ReadValue(fixedFormatter, readComma);
            if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
                return ReadValue(unfixedFormatter, readComma);
            throw SerdeException.ThrowFormatterNotFound(typeof(T));
        }

        #endregion

        #region Read Collection

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TCollection ReadCollection<TCollection, T>(ISerdeJsonFormatter<T> formatter, bool readComma = false) where TCollection : ICollection<T>, new()
        {
            if (TryReadNull())
            {
                return default;
            }

            var col = new TCollection();
            ReadCollection(col, formatter, readComma);
            return col;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TCollection ReadCollection<TCollection, T>(IFixedSerdeJsonFormatter<T> formatter, bool readComma = false) where TCollection : ICollection<T>, new()
        {
            if (TryReadNull())
            {
                return default;
            }

            var col = new TCollection();
            ReadCollection(col, formatter, readComma);
            return col;
        }

        internal void ReadCollection<T>(ICollection<T> col, IFixedSerdeJsonFormatter<T> formatter, bool readComma)
        {
            ReadBeginArray();
            while (true)
            {
                ReadWhiteSpace();
                col.Add(ReadValue(formatter));
                ReadWhiteSpace();
                if (m_Json[m_Advanced] == ',')
                {
                    Advance(1);
                }
                else if (m_Json[m_Advanced] == ']')
                {
                    Advance(1);
                    break;
                }
            }
            if (readComma) ReadComma();
        }

        internal void ReadCollection<T>(ICollection<T> col, ISerdeJsonFormatter<T> formatter, bool readComma)
        {
            ReadBeginArray();
            while (true)
            {
                ReadWhiteSpace();
                col.Add(ReadValue(formatter));
                ReadWhiteSpace();
                if (m_Json[m_Advanced] == ',')
                {
                    Advance(1);
                }
                else if (m_Json[m_Advanced] == ']')
                {
                    Advance(1);
                    break;
                }
            }
            if (readComma) ReadComma();
        }

        #endregion

        #region Read Literal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<char> ReadUntilEnd()
        {
            var span = remains;
            fixed (char* src = remains)
            {
                var result = JsonReader.ReadValueUntilEndUtf16(src, span.Length - 1, out var advance);
                Advance(advance);
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadWhiteSpace()
        {
            for (var i = m_Advanced; i < m_Json.Length; i++)
            {
                if (StringHelper.IsWhiteSpace(m_Json[i])) continue;
                Advance(i - m_Advanced);
                return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadBeginObject() => ReadChar('{');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadEndObject() => ReadChar('}');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadBeginArray() => ReadChar('[');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadEndArray() => ReadChar(']');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadKeyBegin() => ReadChar(':');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadComma() => ReadChar(',');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadBeginObject() => TryReadChar('{');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadEndObject() => TryReadChar('}');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadBeginArray() => TryReadChar('[');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadEndArray() => TryReadChar(']');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadKeyBegin() => TryReadChar(':');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadComma() => TryReadChar(',');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadNull()
        {
            var length = m_Json.Length - m_Advanced;
            var span = m_Json.Slice(m_Advanced, length);
            // 因为最大要4个字符，所以长度为Length - 3
            for (var i = 0; i < span.Length - 3; i++)
            {
                if (StringHelper.IsWhiteSpace(span[i])) continue;
                // 找到连续的null
                if (span[i] == 'n' && span[i + 1] == 'u' && span[i + 2] == 'l' && span[i + 3] == 'l')
                {
                    Advance(i + 4);
                    return true;
                }
                return false;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadKey()
        {
            ReadWhiteSpace();
            var key = ReadString();
            ReadWhiteSpace();
            ReadKeyBegin();
            ReadWhiteSpace();
            return key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadKey(out ReadOnlySpan<char> key)
        {
            ReadWhiteSpace();
            if (remains[0] != '"')
            {
                key = default;
                return false;
            }
            key = ReadString();
            ReadWhiteSpace();
            ReadKeyBegin();
            ReadWhiteSpace();
            return true;
        }

        private ReadOnlySpan<char> ReadString()
        {
            if (m_Json[m_Advanced] != '"')
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            for (var i = m_Advanced + 1; i < m_Json.Length; i++)
            {
                if (m_Json[i].Equals('"') && m_Json[i - 1] != '\\')
                {
                    var result = m_Json.Slice(m_Advanced + 1, i - m_Advanced - 1);
                    Advance(i + 1 - m_Advanced);
                    return result;
                }
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        /// <summary>
        /// 持续读取字符，直到遇到第一个目标字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadChar(char c)
        {
            var length = m_Json.Length - m_Advanced;
            var span = m_Json.Slice(m_Advanced, length);
            if (!TryGetIndexFromSpan(ref span, length, c, out var index))
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            Advance(index + 1);
        }

        /// <summary>
        /// 持续读取字符，直到遇到第一个目标字符
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryReadChar(char c)
        {
            var length = m_Json.Length - m_Advanced;
            var span = m_Json.Slice(m_Advanced, length);
            for (var i = 0; i < span.Length; i++)
            {
                if (StringHelper.IsWhiteSpace(span[i])) continue;
                if (span[i] == c)
                {
                    Advance(i + 1);
                    return true;
                }
                return false;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool TryGetIndexFromSpan(ref ReadOnlySpan<char> span, int length, char character, out int index)
        {
            fixed (char* ptr = span)
            {
                index = StringHelper.IndexOf(ptr, length, character);
            }
            return index != -1;
        }
        #endregion
    }
}