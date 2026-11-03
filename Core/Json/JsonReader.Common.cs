/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/20
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

public unsafe ref struct JsonReaderUtf8
{
    private int m_Advanced;
    private readonly ReadOnlySpan<byte> m_Json;

    /// <summary>Base address after the entry point pinned the buffer; null means nothing is pinned, so cursors cannot be created.</summary>
    private readonly byte* m_Base;

    private readonly bool m_HasBase;

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
        m_Base = null;
        m_HasBase = false;
    }

    /// <summary>Created by the entry point: <paramref name="basePtr"/> points at the start of <paramref name="json"/> and is already pinned.</summary>
    internal JsonReaderUtf8(ReadOnlySpan<byte> json, byte* basePtr)
    {
        m_Advanced = 0;
        m_Json = json;
        m_Base = basePtr;
        m_HasBase = true;
    }

    /// <summary>Creates a cursor starting at the current read position</summary>
    public JsonCursorUtf8 CreateCursor()
    {
        if (!m_HasBase)
        {
            throw new NotSupportedException(
                "JsonCursorUtf8 requires a pinned buffer. Use JsonConverter.Deserialize instead of calling the formatter directly.");
        }

        return new JsonCursorUtf8(m_Base + m_Advanced, m_Base + m_Json.Length);
    }

    /// <summary>Synchronizes the position consumed by the cursor back to the reader.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SyncCursor(in JsonCursorUtf8 cursor)
    {
        m_Advanced = (int) (cursor.p - m_Base);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Advance(int count) => m_Advanced += count;

    /// <summary>Synchronizes the length consumed through the cursor by generated code back to the reader.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipConsumed(int count) => m_Advanced += count;

    #region Read Value

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadValue(bool readComma = false)
    {
        var result = ReadUntilEnd();
        if (readComma) ReadComma();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadValue<T>(IFixedSerdeJsonFormatter<T> formatter, bool readComma = false)
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
        ReadWhiteSpace();
        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            return;
        }
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
        ReadWhiteSpace();
        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            return;
        }
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
    public ReadOnlySpan<byte> ReadUntilEnd()
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
        // Up to 4 characters are needed, so the length is Length - 3
        for (var i = 0; i < span.Length - 3; i++)
        {
            if (StringHelper.IsWhiteSpace(span[i])) continue;
            // Find consecutive "null"
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
    /// Keeps reading characters until the first target character is encountered
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadChar(char c)
    {
        ReadWhiteSpace();
        if (remains[0] != (byte) c)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }
        Advance(1);
    }

    /// <summary>
    /// Keeps reading characters until the first target character is encountered
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

    #endregion
}

public unsafe ref struct JsonReaderUtf16
{
    private int m_Advanced;
    private readonly ReadOnlySpan<char> m_Json;

    /// <summary>Base address after the entry point pinned the buffer; null means nothing is pinned, so cursors cannot be created.</summary>
    private readonly char* m_Base;

    private readonly bool m_HasBase;

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
        m_Base = null;
        m_HasBase = false;
    }

    /// <summary>Created by the entry point: <paramref name="basePtr"/> points at the start of <paramref name="json"/> and is already pinned.</summary>
    internal JsonReaderUtf16(ReadOnlySpan<char> json, char* basePtr)
    {
        m_Advanced = 0;
        m_Json = json;
        m_Base = basePtr;
        m_HasBase = true;
    }

    /// <summary>Creates a cursor starting at the current read position (the buffer must already be pinned by the entry point).</summary>
    public JsonCursorUtf16 CreateCursor()
    {
        if (!m_HasBase)
        {
            throw new NotSupportedException(
                "JsonCursorUtf16 requires a pinned buffer. Use JsonConverter.Deserialize instead of calling the formatter directly.");
        }

        return new JsonCursorUtf16(m_Base + m_Advanced, m_Base + m_Json.Length);
    }

    /// <summary>Synchronizes the position consumed by the cursor back to the reader.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SyncCursor(in JsonCursorUtf16 cursor)
    {
        m_Advanced = (int) (cursor.p - m_Base);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Advance(int count) => m_Advanced += count;

    /// <summary>Synchronizes the length consumed through the cursor by generated code back to the reader.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipConsumed(int count) => m_Advanced += count;

    #region Read Value

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char> ReadValue(bool readComma = false)
    {
        var result = ReadUntilEnd();
        if (readComma) ReadComma();
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadValue<T>(IFixedSerdeJsonFormatter<T> formatter, bool readComma = false)
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
        ReadWhiteSpace();
        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            return;
        }
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
        ReadWhiteSpace();
        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            return;
        }
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
    public ReadOnlySpan<char> ReadUntilEnd()
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
        // Up to 4 characters are needed, so the length is Length - 3
        for (var i = 0; i < span.Length - 3; i++)
        {
            if (StringHelper.IsWhiteSpace(span[i])) continue;
            // Find consecutive "null"
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
    /// Keeps reading characters until the first target character is encountered
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadChar(char c)
    {
        ReadWhiteSpace();
        if (remains[0] != c)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }
        Advance(1);
    }

    /// <summary>
    /// Keeps reading characters until the first target character is encountered
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
    private static bool TryGetIndexFromSpan(ref ReadOnlySpan<char> span, int length, char character, out int index)
    {
        fixed (char* ptr = span)
        {
            index = StringHelper.IndexOf(ptr, length, character);
        }
        return index != -1;
    }

    #endregion
}