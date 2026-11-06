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
    internal void Advance(int count) => m_Advanced += count;

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
        // The per-call resolution is cached per registration, so the hot path pays one version compare instead of the
        // three generic static reads and interface casts GetFormatterInternal performs on every call.
        SerdeJsonFormatterProvider.Resolve<T>(out var fixedFormatter, out var unfixedFormatter);
        if (fixedFormatter != null)
            return ReadValue(fixedFormatter, readComma);
        if (unfixedFormatter != null)
            return ReadValue(unfixedFormatter, readComma);
        throw SerdeException.ThrowFormatterNotFound(typeof(T));
    }

    #endregion

    #region Read Value Directly

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32Value(bool readComma = false)
    {
        var value = JsonReader.ReadIntFastUtf8(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64Value(bool readComma = false)
    {
        var value = JsonReader.ReadLongFastUtf8(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16Value(bool readComma = false)
    {
        var span = remains;
        short value;
        fixed (byte* src = span)
        {
            value = JsonReader.ReadShortUtf8(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16Value(bool readComma = false)
    {
        var span = remains;
        ushort value;
        fixed (byte* src = span)
        {
            value = JsonReader.ReadUshortUtf8(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByteValue(bool readComma = false)
    {
        var span = remains;
        byte value;
        fixed (byte* src = span)
        {
            value = JsonReader.ReadByteUtf8(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByteValue(bool readComma = false)
    {
        var span = remains;
        sbyte value;
        fixed (byte* src = span)
        {
            value = JsonReader.ReadSbyteUtf8(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32Value(bool readComma = false)
    {
        var span = remains;
        uint value;
        fixed (byte* src = span)
        {
            value = JsonReader.ReadUintUtf8(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64Value(bool readComma = false)
    {
        var span = remains;
        ulong value;
        fixed (byte* src = span)
        {
            value = JsonReader.ReadUlongUtf8(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadSingleValue(bool readComma = false)
    {
        var value = JsonReader.ReadFloatFastUtf8(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDoubleValue(bool readComma = false)
    {
        var value = JsonReader.ReadDoubleFastUtf8(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal ReadDecimalValue(bool readComma = false)
    {
        var span = remains;
        decimal value;
        fixed (byte* src = span)
        {
            value = JsonReader.ReadDecimalUtf8(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBooleanValue(bool readComma = false)
    {
        var value = JsonReader.ReadBoolFastUtf8(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char ReadCharValue(bool readComma = false)
    {
        var value = JsonReader.ReadCharFastUtf8(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a string value through the single-allocation fast path, without the formatter indirection.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadStringValue(bool readComma = false)
    {
        var value = JsonReader.ReadStringValueUtf8(remains, out var advance);
        Advance(advance);
        if (readComma) ReadComma();
        return value;
    }

    #endregion

    #region Read Collection Directly

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public int[] ReadInt32Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<int>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<int>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadInt32Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<int>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<int> ReadInt32List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<int>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadInt32Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public long[] ReadInt64Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<long>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<long>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadInt64Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<long>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<long> ReadInt64List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<long>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadInt64Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public short[] ReadInt16Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<short>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<short>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadInt16Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<short>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<short> ReadInt16List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<short>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadInt16Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public ushort[] ReadUInt16Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<ushort>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<ushort>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadUInt16Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<ushort>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<ushort> ReadUInt16List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<ushort>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadUInt16Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public byte[] ReadByteArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<byte>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<byte>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadByteValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<byte>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<byte> ReadByteList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<byte>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadByteValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public sbyte[] ReadSByteArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<sbyte>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<sbyte>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadSByteValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<sbyte>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<sbyte> ReadSByteList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<sbyte>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadSByteValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public uint[] ReadUInt32Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<uint>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<uint>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadUInt32Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<uint>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<uint> ReadUInt32List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<uint>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadUInt32Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public ulong[] ReadUInt64Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<ulong>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<ulong>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadUInt64Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<ulong>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<ulong> ReadUInt64List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<ulong>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadUInt64Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public float[] ReadSingleArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<float>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<float>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadSingleValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<float>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<float> ReadSingleList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<float>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadSingleValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public double[] ReadDoubleArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<double>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<double>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadDoubleValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<double>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<double> ReadDoubleList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<double>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadDoubleValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public decimal[] ReadDecimalArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<decimal>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<decimal>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadDecimalValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<decimal>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<decimal> ReadDecimalList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<decimal>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadDecimalValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public bool[] ReadBooleanArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<bool>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<bool>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadBooleanValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<bool>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<bool> ReadBooleanList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<bool>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadBooleanValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public char[] ReadCharArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<char>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<char>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadCharValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<char>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<char> ReadCharList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<char>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadCharValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public string[] ReadStringArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<string>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == (byte) ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<string>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadStringValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == (byte) ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == (byte) ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<string>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<string> ReadStringList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<string>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadStringValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == (byte) ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == (byte) ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
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
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if(m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            return;
        }
        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

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
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if(m_Json[m_Advanced] == (byte) ']')
        {
            Advance(1);
            return;
        }
        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

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
    public bool TryReadComma()
    {
        var json = m_Json;
        var index = m_Advanced;
        while (index < json.Length && StringHelper.IsWhiteSpace(json[index])) index++;
        m_Advanced = index;
        if (index >= json.Length || json[index] != (byte) ',')
        {
            return false;
        }

        // Consume the comma only: the next key read skips the whitespace in front of the next key
        m_Advanced = index + 1;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadNull()
    {
        var length = m_Json.Length - m_Advanced;
        var span = m_Json.Slice(m_Advanced, length);
        // Up to 4 characters are needed, hence the length of Length - 3
        for (var i = 0; i < span.Length - 3; i++)
        {
            if (StringHelper.IsWhiteSpace(span[i])) continue;
            // Find consecutive nulls
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

    /// <summary>Reads the next object key and consumes its colon.</summary>
    /// <remarks>
    /// This runs once per member, so it walks the buffer directly instead of going through the whitespace, quote and
    /// character helpers: one whitespace skip, one vectorised quote search and one whitespace skip before the colon.
    /// Keys that contain escapes keep the general path.
    /// </remarks>
    public bool TryReadKey(out ReadOnlySpan<byte> key)
    {
        var json = m_Json;
        var index = m_Advanced;
        while (index < json.Length && StringHelper.IsWhiteSpace(json[index])) index++;
        if (index >= json.Length || json[index] != (byte) '"')
        {
            m_Advanced = index;
            key = default;
            return false;
        }

        var start = index + 1;
        var end = JsonReader.IndexOfQuoteOrBackslashUtf8(json, start);
        if (end < 0)
        {
            throw SerdeException.ThrowInvalidJson(json.ToString());
        }

        if (json[end] == (byte) '\\')
        {
            // Escaped key: fall back to the general string reader, which has to restart from the opening quote
            m_Advanced = index;
            key = ReadString();
            ReadWhiteSpace();
            ReadKeyBegin();
            ReadWhiteSpace();
            return true;
        }

        key = json.Slice(start, end - start);
        index = end + 1;
        while (index < json.Length && StringHelper.IsWhiteSpace(json[index])) index++;
        if (index >= json.Length || json[index] != (byte) ':')
        {
            throw SerdeException.ThrowInvalidJson(json.ToString());
        }

        // Skip the whitespace after the colon so the value reader starts on the value itself
        index++;
        while (index < json.Length && StringHelper.IsWhiteSpace(json[index])) index++;
        m_Advanced = index;
        return true;
    }

    private ReadOnlySpan<byte> ReadString()
    {
        if (m_Json[m_Advanced] != (byte) '"')
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        // Track escapes by skipping the escaped character: outside a string a JSON text holds only quotes and
        // backslashes, so one vectorised search per special character keeps the common case at a single scan.
        var position = m_Advanced + 1;
        while (true)
        {
            var found = JsonReader.IndexOfQuoteOrBackslashUtf8(m_Json, position);
            if (found < 0)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[found] == (byte) '"')
            {
                var result = m_Json.Slice(m_Advanced + 1, found - m_Advanced - 1);
                Advance(found + 1 - m_Advanced);
                return result;
            }

            position = found + 2;
            if (position >= m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
        }
    }

    /// <summary>
    /// Reads characters until the first target character is encountered
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
    /// Reads characters until the first target character is encountered
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
    internal void Advance(int count) => m_Advanced += count;

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
        // The per-call resolution is cached per registration, so the hot path pays one version compare instead of the
        // three generic static reads and interface casts GetFormatterInternal performs on every call.
        SerdeJsonFormatterProvider.Resolve<T>(out var fixedFormatter, out var unfixedFormatter);
        if (fixedFormatter != null)
            return ReadValue(fixedFormatter, readComma);
        if (unfixedFormatter != null)
            return ReadValue(unfixedFormatter, readComma);
        throw SerdeException.ThrowFormatterNotFound(typeof(T));
    }

    #endregion

    #region Read Value Directly

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32Value(bool readComma = false)
    {
        var value = JsonReader.ReadIntFastUtf16(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64Value(bool readComma = false)
    {
        var value = JsonReader.ReadLongFastUtf16(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16Value(bool readComma = false)
    {
        var span = remains;
        short value;
        fixed (char* src = span)
        {
            value = JsonReader.ReadShortUtf16(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16Value(bool readComma = false)
    {
        var span = remains;
        ushort value;
        fixed (char* src = span)
        {
            value = JsonReader.ReadUshortUtf16(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByteValue(bool readComma = false)
    {
        var span = remains;
        byte value;
        fixed (char* src = span)
        {
            value = JsonReader.ReadByteUtf16(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByteValue(bool readComma = false)
    {
        var span = remains;
        sbyte value;
        fixed (char* src = span)
        {
            value = JsonReader.ReadSbyteUtf16(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32Value(bool readComma = false)
    {
        var span = remains;
        uint value;
        fixed (char* src = span)
        {
            value = JsonReader.ReadUintUtf16(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64Value(bool readComma = false)
    {
        var span = remains;
        ulong value;
        fixed (char* src = span)
        {
            value = JsonReader.ReadUlongUtf16(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadSingleValue(bool readComma = false)
    {
        var value = JsonReader.ReadFloatFastUtf16(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDoubleValue(bool readComma = false)
    {
        var value = JsonReader.ReadDoubleFastUtf16(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal ReadDecimalValue(bool readComma = false)
    {
        var span = remains;
        decimal value;
        fixed (char* src = span)
        {
            value = JsonReader.ReadDecimalUtf16(src, out var advance);
            Advance(advance);
        }

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBooleanValue(bool readComma = false)
    {
        var value = JsonReader.ReadBoolFastUtf16(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a primitive with the same parse as its formatter, without the formatter lookup and interface call.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char ReadCharValue(bool readComma = false)
    {
        var value = JsonReader.ReadCharFastUtf16(remains, out var advance);
        Advance(advance);

        if (readComma) ReadComma();
        return value;
    }

    /// <summary>Reads a string value through the single-allocation fast path, without the formatter indirection.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadStringValue(bool readComma = false)
    {
        var value = JsonReader.ReadStringValueUtf16(remains, out var advance);
        Advance(advance);
        if (readComma) ReadComma();
        return value;
    }

    #endregion

    #region Read Collection Directly

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public int[] ReadInt32Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<int>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<int>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadInt32Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<int>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<int> ReadInt32List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<int>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadInt32Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public long[] ReadInt64Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<long>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<long>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadInt64Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<long>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<long> ReadInt64List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<long>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadInt64Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public short[] ReadInt16Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<short>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<short>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadInt16Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<short>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<short> ReadInt16List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<short>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadInt16Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public ushort[] ReadUInt16Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<ushort>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<ushort>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadUInt16Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<ushort>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<ushort> ReadUInt16List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<ushort>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadUInt16Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public byte[] ReadByteArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<byte>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<byte>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadByteValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<byte>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<byte> ReadByteList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<byte>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadByteValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public sbyte[] ReadSByteArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<sbyte>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<sbyte>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadSByteValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<sbyte>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<sbyte> ReadSByteList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<sbyte>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadSByteValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public uint[] ReadUInt32Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<uint>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<uint>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadUInt32Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<uint>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<uint> ReadUInt32List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<uint>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadUInt32Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public ulong[] ReadUInt64Array(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<ulong>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<ulong>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadUInt64Value());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<ulong>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<ulong> ReadUInt64List(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<ulong>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadUInt64Value());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public float[] ReadSingleArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<float>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<float>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadSingleValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<float>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<float> ReadSingleList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<float>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadSingleValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public double[] ReadDoubleArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<double>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<double>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadDoubleValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<double>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<double> ReadDoubleList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<double>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadDoubleValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public decimal[] ReadDecimalArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<decimal>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<decimal>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadDecimalValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<decimal>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<decimal> ReadDecimalList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<decimal>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadDecimalValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public bool[] ReadBooleanArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<bool>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<bool>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadBooleanValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<bool>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<bool> ReadBooleanList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<bool>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadBooleanValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public char[] ReadCharArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<char>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<char>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadCharValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<char>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<char> ReadCharList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<char>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadCharValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
    }

    /// <summary>Reads an array of primitive values, parsing every element without the formatter indirection.</summary>
    public string[] ReadStringArray(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var buffer = ListPool<string>.shared.Rent();
        try
        {
            ReadBeginArray();
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[m_Advanced] == ']')
            {
                Advance(1);
                if (readComma) ReadComma();
                return Array.Empty<string>();
            }

            while (true)
            {
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }

                buffer.Add(ReadStringValue());
                // The value parse leaves us on the delimiter (numbers skip their trailing whitespace, strings do not),
                // so one whitespace skip and two character compares replace the generic TryReadChar helper here.
                ReadWhiteSpace();
                if ((uint) m_Advanced >= (uint) m_Json.Length)
                {
                    throw SerdeException.ThrowInvalidJson(m_Json.ToString());
                }
                var delimiter = m_Json[m_Advanced];
                if (delimiter == ',')
                {
                    Advance(1);
                    continue;
                }
                if (delimiter == ']')
                {
                    Advance(1);
                    break;
                }
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (readComma) ReadComma();
            return buffer.ToArray();
        }
        finally
        {
            ListPool<string>.shared.Return(buffer);
        }
    }

    /// <summary>Reads a list of primitive values, parsing every element without the formatter indirection.</summary>
    public List<string> ReadStringList(bool readComma = false)
    {
        if (TryReadNull())
        {
            return null;
        }

        var list = new List<string>();
        ReadBeginArray();
        ReadWhiteSpace();
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (m_Json[m_Advanced] == ']')
        {
            Advance(1);
            if (readComma) ReadComma();
            return list;
        }

        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            list.Add(ReadStringValue());
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
            var delimiter = m_Json[m_Advanced];
            if (delimiter == ',')
            {
                Advance(1);
                continue;
            }
            if (delimiter == ']')
            {
                Advance(1);
                break;
            }
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if (readComma) ReadComma();
        return list;
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
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if(m_Json[m_Advanced] == ']')
        {
            Advance(1);
            return;
        }
        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

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
        if ((uint) m_Advanced >= (uint) m_Json.Length)
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        if(m_Json[m_Advanced] == ']')
        {
            Advance(1);
            return;
        }
        while (true)
        {
            ReadWhiteSpace();
            if ((uint) m_Advanced >= (uint) m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

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
    public bool TryReadComma()
    {
        var json = m_Json;
        var index = m_Advanced;
        while (index < json.Length && StringHelper.IsWhiteSpace(json[index])) index++;
        m_Advanced = index;
        if (index >= json.Length || json[index] != ',')
        {
            return false;
        }

        // Consume the comma only: the next key read skips the whitespace in front of the next key
        m_Advanced = index + 1;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadNull()
    {
        var length = m_Json.Length - m_Advanced;
        var span = m_Json.Slice(m_Advanced, length);
        // Up to 4 characters are needed, hence the length of Length - 3
        for (var i = 0; i < span.Length - 3; i++)
        {
            if (StringHelper.IsWhiteSpace(span[i])) continue;
            // Find consecutive nulls
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

    /// <summary>Reads the next object key and consumes its colon.</summary>
    /// <remarks>
    /// This runs once per member, so it walks the buffer directly instead of going through the whitespace, quote and
    /// character helpers: one whitespace skip, one vectorised quote search and one whitespace skip before the colon.
    /// Keys that contain escapes keep the general path.
    /// </remarks>
    public bool TryReadKey(out ReadOnlySpan<char> key)
    {
        var json = m_Json;
        var index = m_Advanced;
        while (index < json.Length && StringHelper.IsWhiteSpace(json[index])) index++;
        if (index >= json.Length || json[index] != '"')
        {
            m_Advanced = index;
            key = default;
            return false;
        }

        var start = index + 1;
        var end = JsonReader.IndexOfQuoteOrBackslashUtf16(json, start);
        if (end < 0)
        {
            throw SerdeException.ThrowInvalidJson(json.ToString());
        }

        if (json[end] == '\\')
        {
            // Escaped key: fall back to the general string reader, which has to restart from the opening quote
            m_Advanced = index;
            key = ReadString();
            ReadWhiteSpace();
            ReadKeyBegin();
            ReadWhiteSpace();
            return true;
        }

        key = json.Slice(start, end - start);
        index = end + 1;
        while (index < json.Length && StringHelper.IsWhiteSpace(json[index])) index++;
        if (index >= json.Length || json[index] != ':')
        {
            throw SerdeException.ThrowInvalidJson(json.ToString());
        }

        // Skip the whitespace after the colon so the value reader starts on the value itself
        index++;
        while (index < json.Length && StringHelper.IsWhiteSpace(json[index])) index++;
        m_Advanced = index;
        return true;
    }

    private ReadOnlySpan<char> ReadString()
    {
        if (m_Json[m_Advanced] != '"')
        {
            throw SerdeException.ThrowInvalidJson(m_Json.ToString());
        }

        // Track escapes by skipping the escaped character: outside a string a JSON text holds only quotes and
        // backslashes, so one vectorised search per special character keeps the common case at a single scan.
        var position = m_Advanced + 1;
        while (true)
        {
            var found = JsonReader.IndexOfQuoteOrBackslashUtf16(m_Json, position);
            if (found < 0)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }

            if (m_Json[found] == '"')
            {
                var result = m_Json.Slice(m_Advanced + 1, found - m_Advanced - 1);
                Advance(found + 1 - m_Advanced);
                return result;
            }

            position = found + 2;
            if (position >= m_Json.Length)
            {
                throw SerdeException.ThrowInvalidJson(m_Json.ToString());
            }
        }
    }

    /// <summary>
    /// Reads characters until the first target character is encountered
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
    /// Reads characters until the first target character is encountered
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

    #endregion
}
