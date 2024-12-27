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
        private bool m_IsIn;
        private int m_Advanced;
        private int m_LastIndex;
        private readonly ReadOnlySpan<byte> m_Json;
        public readonly ReadOnlySpan<byte> json => m_Json;

        public ReadOnlySpan<byte> remains
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Json.Slice(m_Advanced, m_Json.Length - m_Advanced);
        }

        public JsonReaderUtf8(ReadOnlySpan<byte> json)
        {
            m_Advanced = 0;
            m_LastIndex = 0;
            m_IsIn = false;
            m_Json = json;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int count) => m_Advanced += count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T ReadValue<T>(bool readComma, ISerdeJsonFormatter<T> formatter)
        {
            T result;
            if (formatter.isFixedFormatter)
            {
                var span = remains;
                fixed (byte* ptr = span)
                {
                    result = formatter.DeserializeUtf8Unsafe(ptr, out var advance);
                    Advance(advance);
                }
            }
            else
            {
                var span = remains;
                result = formatter.DeserializeUtf8(ref span, ref this);
            }
            if (readComma) ReadComma();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ReadArray<T>(bool readComma, ISerdeJsonFormatter<T> formatter)
        {
            var list = ListPool<T>.shared.Rent();
            ReadCollection(list, readComma, formatter, out var isNull);
            var results = isNull ? null : list.ToArray();
            ListPool<T>.shared.Return(list);
            return results;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCollection ReadCollection<TCollection, T>(bool readComma, ISerdeJsonFormatter<T> formatter) where TCollection : ICollection<T>, new()
        {
            var col = new TCollection();
            ReadCollection(col, readComma, formatter, out var isNull);
            return isNull ? default : col;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ReadValue<T>(bool readComma) => ReadValue<T>(readComma, SerdeJsonFormatterProvider.GetFormatter<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ReadArray<T>(bool readComma) => ReadArray<T>(readComma, SerdeJsonFormatterProvider.GetFormatter<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCollection ReadCollection<TCollection, T>(bool readComma) where TCollection : ICollection<T>, new() => ReadCollection<TCollection, T>(readComma, SerdeJsonFormatterProvider.GetFormatter<T>());

        private void ReadCollection<T>(ICollection<T> list, bool readComma, ISerdeJsonFormatter<T> formatter, out bool isNull)
        {
            if (SpanConverter.IsNull(m_Json.Slice(m_Advanced - 1, 4)))
            {
                isNull = true;
                Advance(4);
            }
            else
            {
                ReadBeginArray();
                while (true)
                {
                    list.Add(ReadValue<T>(false, formatter));
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == (byte) ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == (byte) ']')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            if (readComma) ReadComma();
            isNull = false;
        }

        public unsafe TDictionary ReadDictionary<TDictionary, TKey, TValue>(bool readComma) where TDictionary : IDictionary<TKey, TValue>, new()
        {
            var dic = new TDictionary();
            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            ReadBeginObject();
            if (keyFormatter.isFixedFormatter && valueFormatter.isFixedFormatter)
            {
                while (true)
                {
                    TKey key;
                    TValue value;
                    var strKey = ReadKey();
                    fixed (byte* k = strKey)
                    {
                        key = keyFormatter.DeserializeUtf8Unsafe(k, out _);
                    }
                    fixed (byte* v = remains)
                    {
                        value = valueFormatter.DeserializeUtf8Unsafe(v, out var advance);
                        Advance(advance);
                    }
                    dic.Add(key, value);
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == (byte) ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == (byte) '}')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            else if (keyFormatter.isFixedFormatter && !valueFormatter.isFixedFormatter)
            {
                while (true)
                {
                    TKey key;
                    TValue value;
                    var strKey = ReadKey();
                    fixed (byte* k = strKey)
                    {
                        key = keyFormatter.DeserializeUtf8Unsafe(k, out _);
                    }
                    var span = remains;
                    value = valueFormatter.DeserializeUtf8(ref span, ref this);
                    dic.Add(key, value);
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == (byte) ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == (byte) '}')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            else if (!keyFormatter.isFixedFormatter && valueFormatter.isFixedFormatter)
            {
                while (true)
                {
                    TKey key;
                    TValue value;
                    var strKey = ReadKey();
                    key = keyFormatter.DeserializeUtf8(ref strKey, ref this);
                    fixed (byte* v = remains)
                    {
                        value = valueFormatter.DeserializeUtf8Unsafe(v, out var advance);
                        Advance(advance);
                    }
                    dic.Add(key, value);
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == (byte) ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == (byte) '}')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    TKey key;
                    TValue value;
                    var strKey = ReadKey();
                    key = keyFormatter.DeserializeUtf8(ref strKey, ref this);
                    var span = remains;
                    value = valueFormatter.DeserializeUtf8(ref span, ref this);
                    dic.Add(key, value);
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == (byte) ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == (byte) '}')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            return dic;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<byte> ReadUntilEnd()
        {
            var span = remains;
            fixed(byte* src = remains)
            {
                var result = JsonReader.ReadValueUntilEndUtf8(src, span.Length - 1, out var advance);
                Advance(advance);
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadWhiteSpace()
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
        public ReadOnlySpan<byte> ReadKey()
        {
            ReadWhiteSpace();
            var key = ReadString();
            ReadWhiteSpace();
            ReadKeyBegin();
            ReadWhiteSpace();
            return key;
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
                    var result = m_Json.Slice(m_Advanced + 1, i - 2);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool TryGetIndexFromSpan(ref ReadOnlySpan<byte> span, int length, byte character, out int index)
        {
            fixed (byte* ptr = span)
            {
                index = StringHelper.IndexOf(ptr, length, character);
            }
            return index != -1;
        }
    }
    public ref struct JsonReaderUtf16
    {
        private bool m_IsIn;
        private int m_Advanced;
        private int m_LastIndex;
        private readonly ReadOnlySpan<char> m_Json;
        public readonly ReadOnlySpan<char> json => m_Json;

        public ReadOnlySpan<char> remains
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Json.Slice(m_Advanced, m_Json.Length - m_Advanced);
        }

        public JsonReaderUtf16(ReadOnlySpan<char> json)
        {
            m_Advanced = 0;
            m_LastIndex = 0;
            m_IsIn = false;
            m_Json = json;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int count) => m_Advanced += count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T ReadValue<T>(bool readComma, ISerdeJsonFormatter<T> formatter)
        {
            T result;
            if (formatter.isFixedFormatter)
            {
                var span = remains;
                fixed (char* ptr = span)
                {
                    result = formatter.DeserializeUtf16Unsafe(ptr, out var advance);
                    Advance(advance);
                }
            }
            else
            {
                var span = remains;
                result = formatter.DeserializeUtf16(ref span, ref this);
            }
            if (readComma) ReadComma();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ReadArray<T>(bool readComma, ISerdeJsonFormatter<T> formatter)
        {
            var list = ListPool<T>.shared.Rent();
            ReadCollection(list, readComma, formatter, out var isNull);
            var results = isNull ? null : list.ToArray();
            ListPool<T>.shared.Return(list);
            return results;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCollection ReadCollection<TCollection, T>(bool readComma, ISerdeJsonFormatter<T> formatter) where TCollection : ICollection<T>, new()
        {
            var col = new TCollection();
            ReadCollection(col, readComma, formatter, out var isNull);
            return isNull ? default : col;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ReadValue<T>(bool readComma) => ReadValue<T>(readComma, SerdeJsonFormatterProvider.GetFormatter<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ReadArray<T>(bool readComma) => ReadArray<T>(readComma, SerdeJsonFormatterProvider.GetFormatter<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCollection ReadCollection<TCollection, T>(bool readComma) where TCollection : ICollection<T>, new() => ReadCollection<TCollection, T>(readComma, SerdeJsonFormatterProvider.GetFormatter<T>());

        private void ReadCollection<T>(ICollection<T> list, bool readComma, ISerdeJsonFormatter<T> formatter, out bool isNull)
        {
            if (SpanConverter.IsNull(m_Json.Slice(m_Advanced - 1, 4)))
            {
                isNull = true;
                Advance(4);
            }
            else
            {
                ReadBeginArray();
                while (true)
                {
                    list.Add(ReadValue<T>(false, formatter));
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == ']')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            if (readComma) ReadComma();
            isNull = false;
        }

        public unsafe TDictionary ReadDictionary<TDictionary, TKey, TValue>(bool readComma) where TDictionary : IDictionary<TKey, TValue>, new()
        {
            var dic = new TDictionary();
            var keyFormatter = SerdeJsonFormatterProvider.GetFormatter<TKey>();
            var valueFormatter = SerdeJsonFormatterProvider.GetFormatter<TValue>();
            ReadBeginObject();
            if (keyFormatter.isFixedFormatter && valueFormatter.isFixedFormatter)
            {
                while (true)
                {
                    TKey key;
                    TValue value;
                    var strKey = ReadKey();
                    fixed (char* k = strKey)
                    {
                        key = keyFormatter.DeserializeUtf16Unsafe(k, out _);
                    }
                    fixed (char* v = remains)
                    {
                        value = valueFormatter.DeserializeUtf16Unsafe(v, out var advance);
                        Advance(advance);
                    }
                    dic.Add(key, value);
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == '}')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            else if (keyFormatter.isFixedFormatter && !valueFormatter.isFixedFormatter)
            {
                while (true)
                {
                    TKey key;
                    TValue value;
                    var strKey = ReadKey();
                    fixed (char* k = strKey)
                    {
                        key = keyFormatter.DeserializeUtf16Unsafe(k, out _);
                    }
                    var span = remains;
                    value = valueFormatter.DeserializeUtf16(ref span, ref this);
                    dic.Add(key, value);
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == '}')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            else if (!keyFormatter.isFixedFormatter && valueFormatter.isFixedFormatter)
            {
                while (true)
                {
                    TKey key;
                    TValue value;
                    var strKey = ReadKey();
                    key = keyFormatter.DeserializeUtf16(ref strKey, ref this);
                    fixed (char* v = remains)
                    {
                        value = valueFormatter.DeserializeUtf16Unsafe(v, out var advance);
                        Advance(advance);
                    }
                    dic.Add(key, value);
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == '}')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    TKey key;
                    TValue value;
                    var strKey = ReadKey();
                    key = keyFormatter.DeserializeUtf16(ref strKey, ref this);
                    var span = remains;
                    value = valueFormatter.DeserializeUtf16(ref span, ref this);
                    dic.Add(key, value);
                    ReadWhiteSpace();
                    if (m_Json[m_Advanced - 1] == ',')
                    {
                        Advance(1);
                    }
                    else if (m_Json[m_Advanced - 1] == '}')
                    {
                        Advance(1);
                        break;
                    }
                }
            }
            return dic;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ReadOnlySpan<char> ReadUntilEnd()
        {
            var span = remains;
            fixed(char* src = remains)
            {
                var result = JsonReader.ReadValueUntilEndUtf16(src, span.Length - 1, out var advance);
                Advance(advance);
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadWhiteSpace()
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
        public ReadOnlySpan<char> ReadKey()
        {
            ReadWhiteSpace();
            var key = ReadString();
            ReadWhiteSpace();
            ReadKeyBegin();
            ReadWhiteSpace();
            return key;
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
                    var result = m_Json.Slice(m_Advanced + 1, i - 2);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool TryGetIndexFromSpan(ref ReadOnlySpan<char> span, int length, char character, out int index)
        {
            fixed (char* ptr = span)
            {
                index = StringHelper.IndexOf(ptr, length, character);
            }
            return index != -1;
        }
    }
}