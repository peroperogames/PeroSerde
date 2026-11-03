/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/20
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

public static unsafe partial class JsonReader
{
    internal static ReadOnlySpan<char> ReadPrimitiveUntilEndUtf16(char* src, int maxLength, out int advance)
    {
        advance = 0;
        maxLength += 1;
        var span = new ReadOnlySpan<char>(src, maxLength);
        for (var i = 0; i < maxLength; i++)
        {
            var character = span[i];
            var isWhiteSpace = StringHelper.IsWhiteSpace(character);
            // Excluding the terminator
            if (!isWhiteSpace && character is not ('}' or ']' or ','))
            {
                continue;
            }
            advance = i;
            // Consume the rest
            for (var j = i; j < span.Length; j++)
            {
                if (StringHelper.IsWhiteSpace(span[j]))
                {
                    continue;
                }
                advance = j;
                break;
            }
            return span.Slice(0, i);
        }
        throw SerdeException.ThrowInvalidAdvance();
    }

    /// <summary>Skips one complete value and returns the number of characters consumed (used by cursors).</summary>
    internal static int SkipValueUtf16(ReadOnlySpan<char> span)
    {
        var advance = 0;
        var maxLength = span.Length;
        fixed (char* src = span)
        {
            if (span[0] == '[' || span[0] == '{')
            {
                ReadArrayOrObjectUntilEndUtf16(src, maxLength, out advance);
                return advance;
            }
            // Number or null
            if (span[0] != '"')
            {
                ReadPrimitiveUntilEndUtf16(src, maxLength, out advance);
                return advance;
            }
            // String
            var index = 0;
            var canAllocateOnStack = maxLength < 512;
            var array = canAllocateOnStack ? Array.Empty<char>() : ArrayPool.UTF16.Rent(maxLength);
            var result = canAllocateOnStack ? stackalloc char[maxLength] : array;
            try
            {
                for (var i = 1; i < span.Length; i++)
                {
                    var character = span[i];
                    if (character == '\\')
                    {
                        result[index++] = ReadEscapedCharUtf16(src + i, out var length);
                        i += length - 1; // -1 skips the current character
                    }
                    else if (character == '"')
                    {
                        // Including the double quote
                        advance = i + 1;
                        return advance;
                    }
                    else
                    {
                        result[index++] = character;
                    }
                }
            }
            finally
            {
                if (!canAllocateOnStack) ArrayPool.UTF16.Return(array);
            }
            throw SerdeException.ThrowInvalidAdvance();
        }
    }

    private static ReadOnlySpan<char> ReadArrayOrObjectUntilEndUtf16(char* src, int maxLength, out int advance)
    {
        var arrayCount = 0;
        var objectCount = 0;
        advance = 0;
        maxLength += 1;
        var span = new ReadOnlySpan<char>(src, maxLength);
        var inString = false;
        var escaped = false;
        for (var i = 0; i < maxLength; i++)
        {
            var character = span[i];
            if (inString)
            {
                // Escape state machine, see the UTF-8 twin: "\\" before the closing quote must not keep us in the string.
                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (character is '\\')
                {
                    escaped = true;
                    continue;
                }

                if (character is '"')
                {
                    inString = false;
                }

                continue;
            }

            var isWhiteSpace = StringHelper.IsWhiteSpace(character);
            // Excluding the terminator
            if (isWhiteSpace) continue;
            if (character is '"')
            {
                inString = true;
                continue;
            }
            if (character is '{')
            {
                objectCount++;
            }
            else if (character is '}')
            {
                objectCount--;
            }
            else if (character is '[')
            {
                arrayCount++;
            }
            else if (character is ']')
            {
                arrayCount--;
            }
            else
            {
                continue;
            }
            // Until all brackets are closed
            if (arrayCount > 0 || objectCount > 0)
            {
                continue;
            }
            i++; // Read the last bracket
            advance = i;
            // Consume the rest
            for (var j = i; j < span.Length; j++)
            {
                if (StringHelper.IsWhiteSpace(span[j]))
                {
                    continue;
                }
                advance = j;
                break;
            }
            return span.Slice(0, i);
        }
        throw SerdeException.ThrowInvalidAdvance();
    }

    /// <summary>The single parse entry for UTF-16 strings: null returns null.</summary>
    internal static string ReadStringValueUtf16(char* src, int length, out int advance)
    {
        advance = 0;
        if (length > 0 && src[0] == '"')
        {
            if (TryReadStringUtf16(src, length, out var value, out advance))
            {
                return value;
            }
        }

        var result = ReadValueUntilEndUtf16(src, length, out advance);
        if (SpanConverter.IsNull(result))
        {
            return null;
        }

        if (length == 0 || src[0] != '"')
        {
            throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
        }

        return SpanConverter.ToString(result);
    }

    public static ReadOnlySpan<char> ReadValueUntilEndUtf16(char* src, int maxLength, out int advance)
    {
        var span = new Span<char>(src, maxLength);
        // Array/object
        if (span[0] == '[' || span[0] == '{')
        {
            return ReadArrayOrObjectUntilEndUtf16(src, maxLength, out advance);
        }
        // Number or null
        if (span[0] != '"')
        {
            return ReadPrimitiveUntilEndUtf16(src, maxLength, out advance);
        }
        // String
        var index = 0;
        advance = 0;
        var canAllocateOnStack = maxLength < 512;
        var array = canAllocateOnStack ? Array.Empty<char>() : ArrayPool.UTF16.Rent(maxLength);
        // Debug.Log($"arrayLength: {array.Length}, maxLength: {maxLength}");
        var result = canAllocateOnStack ? stackalloc char[maxLength] : array;
        try
        {
            for (var i = 1; i < span.Length; i++)
            {
                var character = span[i];
                if (character == '\\')
                {
                    result[index++] = ReadEscapedCharUtf16(src + i, out var length);
                    i += length - 1; // -1 skips the current character
                }
                else if (character == '"')
                {
                    // Including the double quote
                    advance = i + 1;

                    // Reaching the max length means there was no escape
                    if (index + 2 == advance)
                    {
                        // No need to allocate a new string on the heap; excluding the double quotes
                        return span[1..(advance - 1)];
                    }
                    // Allocate a new string
                    return new string(result[..index]);
                }
                else
                {
                    result[index++] = character;
                }
            }
        }
        finally
        {
            if (!canAllocateOnStack) ArrayPool.UTF16.Return(array);
        }
        throw SerdeException.ThrowInvalidAdvance();
    }

    #region ReadNullable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte? ReadNullableSbyteUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableSbyteFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToSbyte(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte? ReadNullableByteUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableByteFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToByte(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal? ReadNullableDecimalUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableDecimalFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToDecimal(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? ReadNullableDoubleUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableDoubleFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToDouble(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float? ReadNullableFloatUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableFloatFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToFloat(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short? ReadNullableShortUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableShortFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToShort(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int? ReadNullableIntUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableIntFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToInt(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long? ReadNullableLongUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableLongFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToLong(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort? ReadNullableUshortUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableUshortFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToUshort(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint? ReadNullableUintUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableUintFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToUint(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong? ReadNullableUlongUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableUlongFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToUlong(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char? ReadNullableCharUtf16(char* src, out int advance)
    {
        var first = src[0];
        var span = ReadValueUntilEndUtf16(src, NullableCharFormatter.max_size, out advance);
        if (SpanConverter.IsNull(span)) return null;
        if (first != '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
        return SpanConverter.ToChar(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool? ReadNullableBoolUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, NullableBoolFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToBool(in span);
    }

    #endregion

    /// <summary>
    /// Reads a JSON string value (escapes allowed) with a single heap allocation.
    /// </summary>
    /// <remarks>
    /// Scans the whole buffer up to <paramref name="maxLength"/>, so a string ending at the end of the buffer still parses correctly;
    /// Returns false on an illegal escape, and the caller takes the general path to throw the original exception.
    /// </remarks>
    internal static bool TryReadStringUtf16(char* src, int maxLength, out string result, out int advance)
    {
        result = null;
        advance = 0;
        var span = new ReadOnlySpan<char>(src, maxLength);

        var hasEscape = false;
        var end = -1;
        for (var i = 1; i < span.Length; i++)
        {
            var c = span[i];
            if (c == '"')
            {
                end = i;
                break;
            }
            if (c != '\\')
            {
                continue;
            }
            hasEscape = true;
            if (i + 1 >= span.Length)
            {
                return false;
            }
            i += span[i + 1] == 'u' ? 5 : 1;
            if (i >= span.Length)
            {
                return false;
            }
        }

        if (end < 0)
        {
            return false;
        }

        var token = span.Slice(1, end - 1);
        advance = end + 1;
        if (!hasEscape)
        {
            result = new string(token);
            return true;
        }

        // First count the output characters
        var length = 0;
        for (var i = 0; i < token.Length; i++)
        {
            if (token[i] == '\\')
            {
                if (token[i + 1] == 'u')
                {
                    if (Hex(token[i + 2]) < 0 || Hex(token[i + 3]) < 0 || Hex(token[i + 4]) < 0 || Hex(token[i + 5]) < 0)
                    {
                        return false;
                    }
                    i += 5;
                }
                else
                {
                    i += 1;
                }
            }
            length++;
        }

        // Then write them into the string in one pass
        var chars = new string('\0', length);
        var index = 0;
        fixed (char* dest = chars)
        {
            for (var i = 0; i < token.Length; i++)
            {
                if (token[i] != '\\')
                {
                    dest[index++] = token[i];
                    continue;
                }
                dest[index++] = ReadEscapedCharUtf16(src + 1 + i, out var escapedLength);
                i += escapedLength - 1;
            }
        }

        result = chars;
        return true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Hex(char c)
        {
            if (c is >= '0' and <= '9') return c - '0';
            if (c is >= 'a' and <= 'f') return c - 'a' + 10;
            if (c is >= 'A' and <= 'F') return c - 'A' + 10;
            return -1;
        }
    }

    #region Read Primitive

    /// <summary>Reads a nested [SerdeJsonSerializable] object: goes through the reader and syncs the consumed length back to the cursor.</summary>
    public static T ReadNested<T>(ref JsonCursorUtf16 cursor)
    {
        var span = new ReadOnlySpan<char>(cursor.p, (int) (cursor.end - cursor.p));
        var reader = new JsonReaderUtf16(span, cursor.p);
        var value = reader.ReadValue<T>();
        cursor.p = cursor.end - reader.remains.Length;
        return value;
    }

    /// <summary>Reads with the formatter specified on the member (types with an explicit [SerdeJsonFormatter] are not in the provider registry).</summary>
    public static T ReadNested<T>(ref JsonCursorUtf16 cursor, object formatter)
    {
        var span = new ReadOnlySpan<char>(cursor.p, (int) (cursor.end - cursor.p));
        var reader = new JsonReaderUtf16(span, cursor.p);
        T value;
        if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
        {
            value = reader.ReadValue(fixedFormatter);
        }
        else if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
        {
            value = reader.ReadValue(unfixedFormatter);
        }
        else
        {
            throw SerdeException.ThrowFormatterNotFound(typeof(T));
        }

        cursor.p = cursor.end - reader.remains.Length;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt32(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadIntUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadInt64(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadLongUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadUInt32(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadUintUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReadUInt64(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadUlongUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadInt16(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadShortUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUInt16(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadUshortUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadByte(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadByteUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ReadSByte(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadSbyteUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadSingle(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadFloatUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReadDouble(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadDoubleUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ReadDecimal(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadDecimalUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ReadBoolean(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadBoolUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ReadChar(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadCharUtf16(cursor.p, out var advance);
        cursor.p += advance;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadString(ref JsonCursorUtf16 cursor)
    {
        BeginPrimitive(ref cursor);
        var value = ReadStringValueUtf16(cursor.p, (int) (cursor.end - cursor.p), out var advance);
        cursor.p += advance;
        return value;
    }

    /// <summary>Skips whitespace and makes sure there is still content to read.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void BeginPrimitive(ref JsonCursorUtf16 cursor)
    {
        cursor.SkipWhiteSpace();
        if (cursor.p >= cursor.end)
        {
            throw SerdeException.ThrowInvalidJson("Invalid json format.");
        }
    }

    /// <summary>Reads an escaped character; supports UTF8 and UTF16</summary>
    // ReSharper disable once CognitiveComplexity
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ReadEscapedCharUtf16(char* src, out int advance)
    {
        // Expect starting backslash
        if (src[0] != '\\') throw SerdeException.ThrowInvalidEscapedChar(src[0]);

        var esc = src[1];
        switch (esc)
        {
            case '"':
                advance = 2;
                return '"';
            case '\\':
                advance = 2;
                return '\\';
            case '/':
                advance = 2;
                return '/';
            case 'b':
                advance = 2;
                return '\b';
            case 't':
                advance = 2;
                return '\t';
            case 'n':
                advance = 2;
                return '\n';
            case 'f':
                advance = 2;
                return '\f';
            case 'r':
                advance = 2;
                return '\r';
            case 'u':
            {
                // Expect \uXXXX (4 hex digits)
                // src[0] == '\\', src[1] == 'u', hex digits at src[2..5]
                var h1 = Hex(src[2]);
                var h2 = Hex(src[3]);
                var h3 = Hex(src[4]);
                var h4 = Hex(src[5]);
                if (h1 < 0 || h2 < 0 || h3 < 0 || h4 < 0)
                    throw SerdeException.ThrowInvalidEscapedChar(esc);
                var code = (h1 << 12) | (h2 << 8) | (h3 << 4) | h4;
                advance = 6;
                return (char) code;
            }
            default:
                throw SerdeException.ThrowInvalidEscapedChar(esc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Hex(char c)
        {
            if (c is >= '0' and <= '9') return c - '0';
            if (c is >= 'A' and <= 'F') return c - 'A' + 10;
            if (c is >= 'a' and <= 'f') return c - 'a' + 10;
            return -1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadByteUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, ByteFormatter.max_size, out advance);
        return SpanConverter.ToByte(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ReadSbyteUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, SbyteFormatter.max_size, out advance);
        return SpanConverter.ToSbyte(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ReadDecimalUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, DecimalFormatter.max_size, out advance);
        return SpanConverter.ToDecimal(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReadDoubleUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, DoubleFormatter.max_size, out advance);
        return SpanConverter.ToDouble(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadFloatUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, FloatFormatter.max_size, out advance);
        return SpanConverter.ToFloat(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ReadBoolUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, BoolFormatter.max_size, out advance);
        return SpanConverter.ToBool(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ReadCharUtf16(char* src, out int advance)
    {
        if (src[0] != '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
        var span = ReadValueUntilEndUtf16(src, CharFormatter.max_size, out advance);
        return SpanConverter.ToChar(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadShortUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, ShortFormatter.max_size, out advance);
        return SpanConverter.ToShort(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadIntUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, IntFormatter.max_size, out advance);
        return SpanConverter.ToInt(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadLongUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, LongFormatter.max_size, out advance);
        return SpanConverter.ToLong(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUshortUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, UshortFormatter.max_size, out advance);
        return SpanConverter.ToUshort(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadUintUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, UintFormatter.max_size, out advance);
        return SpanConverter.ToUint(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReadUlongUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, UlongFormatter.max_size, out advance);
        return SpanConverter.ToUlong(in span);
    }
}

#endregion