/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/20
 */

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace PeroTools2.Serde.Json;

public static unsafe partial class JsonReader
{
    internal static ReadOnlySpan<byte> ReadPrimitiveUntilEndUtf8(byte* src, int maxLength, out int advance)
    {
        maxLength += 1;
        advance = 0;
        var span = new ReadOnlySpan<byte>(src, maxLength);
        for (var i = 0; i < maxLength; i++)
        {
            var character = (char) span[i];
            var isWhiteSpace = StringHelper.IsWhiteSpace(character);
            // Excluding the terminator
            if (!isWhiteSpace && character is not ('}' or ']' or ','))
            {
                continue;
            }
            advance = i;
            // Consume the remaining whitespace
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

    private static ReadOnlySpan<byte> ReadArrayOrObjectUntilEndUtf8(byte* src, int maxLength, out int advance)
    {
        var arrayCount = 0;
        var objectCount = 0;
        advance = 0;
        maxLength += 1;
        var span = new ReadOnlySpan<byte>(src, maxLength);
        var inString = false;
        var escaped = false;
        for (var i = 0; i < maxLength; i++)
        {
            var character = (char) span[i];
            if (inString)
            {
                // Escape state machine: a backslash escapes the next character, so a string ending
                // with an escaped backslash still terminates on the following quote.
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ZeroByte(uint v) => (v - 0x0101_0101u) & ~v & 0x8080_8080u;

    /// <summary>Finds the next <c>"</c> or <c>\</c> at or after <paramref name="start"/>, or -1 when there is none.</summary>
    /// <remarks>
    /// Keys and values both scan for the closing quote, so the scan is delegated to the vectorised
    /// <see cref="MemoryExtensions.IndexOfAny{T}(ReadOnlySpan{T},T,T)"/> instead of walking bytes one by one.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfQuoteOrBackslashUtf8(ReadOnlySpan<byte> span, int start)
    {
        if ((uint) start >= (uint) span.Length)
        {
            return -1;
        }

        var offset = span.Slice(start).IndexOfAny((byte) '"', (byte) '\\');
        return offset < 0 ? -1 : start + offset;
    }

    /// <summary>The single parse entry for strings: ASCII fast path plus general decoding; null returns null.</summary>
    internal static string ReadStringValueUtf8(ReadOnlySpan<byte> span, out int advance)
    {
        advance = 0;
        if (span.Length > 0 && span[0] == (byte) '"')
        {
            if (TryReadAsciiStringUtf8(span, out var value, out advance))
            {
                return value;
            }
        }

        fixed (byte* src = span)
        {
            return ReadStringValueUtf8(src, span.Length, out advance);
        }
    }

    internal static string ReadStringValueUtf8(byte* src, int length, out int advance)
    {
        advance = 0;
        if (length > 0 && src[0] == (byte) '"')
        {
            if (TryReadAsciiStringUtf8(src, length, out var asciiValue, out advance))
            {
                return asciiValue;
            }
        }

        var result = ReadValueUntilEndUtf8(src, length, out advance);
        if (SpanConverter.IsNull(result))
        {
            return null;
        }

        if (length == 0 || src[0] != (byte) '"')
        {
            throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
        }

        return Encoding.UTF8.GetString(result);
    }

    public static ReadOnlySpan<byte> ReadValueUntilEndUtf8(byte* src, int maxLength, out int advance)
    {
        var span = new Span<byte>(src, maxLength);
        // Array/object
        if (span[0] == '[' || span[0] == '{')
        {
            return ReadArrayOrObjectUntilEndUtf8(src, maxLength, out advance);
        }
        // Number or null
        if (span[0] != '"')
        {
            return ReadPrimitiveUntilEndUtf8(src, maxLength, out advance);
        }
        // String
        var index = 0;
        advance = 0;
        var canAllocateOnStack = maxLength < 1024;
        var array = canAllocateOnStack ? Array.Empty<byte>() : ArrayPool.UTF8.Rent(maxLength);
        var result = canAllocateOnStack ? stackalloc byte[maxLength] : array;
        try
        {
            for (var i = 1; i < span.Length; i++)
            {
                var character = span[i];
                if (character == '\\')
                {
                    var escapedChar = ReadEscapedCharUtf8(src + i, out var length);
                    i += length - 1; // -1 skips the current character
                    if (escapedChar < 0x80)
                    {
                        result[index++] = (byte) escapedChar;
                    }
                    else if (escapedChar < 0x800)
                    {
                        result[index++] = (byte) (0xC0 | (escapedChar >> 6));
                        result[index++] = (byte) (0x80 | (escapedChar & 0x3F));
                    }
                    else if (char.IsHighSurrogate(escapedChar)
                             && i + 6 < span.Length
                             && span[i + 1] == '\\'
                             && span[i + 2] == 'u')
                    {
                        // A surrogate pair must be merged into one 4-byte sequence, otherwise invalid UTF-8 would be written
                        var low = ReadEscapedCharUtf8(src + i + 1, out var lowLength);
                        if (char.IsLowSurrogate(low))
                        {
                            i += lowLength;
                            var codePoint = char.ConvertToUtf32(escapedChar, low);
                            result[index++] = (byte) (0xF0 | (codePoint >> 18));
                            result[index++] = (byte) (0x80 | ((codePoint >> 12) & 0x3F));
                            result[index++] = (byte) (0x80 | ((codePoint >> 6) & 0x3F));
                            result[index++] = (byte) (0x80 | (codePoint & 0x3F));
                        }
                        else
                        {
                            result[index++] = (byte) '?';
                        }
                    }
                    else if (char.IsSurrogate(escapedChar))
                    {
                        result[index++] = (byte) '?';
                    }
                    else
                    {
                        result[index++] = (byte) (0xE0 | (escapedChar >> 12));
                        result[index++] = (byte) (0x80 | ((escapedChar >> 6) & 0x3F));
                        result[index++] = (byte) (0x80 | (escapedChar & 0x3F));
                    }
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
                    return result[..index].ToArray();
                }
                else
                {
                    result[index++] = character;
                }
            }
        }
        finally
        {
            if (!canAllocateOnStack) ArrayPool.UTF8.Return(array);
        }
        throw SerdeException.ThrowInvalidAdvance();
    }

    /// <summary>
    /// Reads a pure ASCII JSON string value (ASCII escapes allowed) with a single heap allocation.
    /// </summary>
    /// <remarks>
    /// The general path must first unescape into a byte[] and then convert it to a string (two allocations);
    /// in the ASCII case the output length can be computed up front and allocated directly as a string in one pass.
    /// Returns false on a non-ASCII byte or non-ASCII escape, and the caller takes the general path.
    /// </remarks>
    internal static bool TryReadAsciiStringUtf8(byte* src, int maxLength, out string result, out int advance)
        => TryReadAsciiStringUtf8(new ReadOnlySpan<byte>(src, maxLength), out result, out advance);

    internal static bool TryReadAsciiStringUtf8(ReadOnlySpan<byte> span, out string result, out int advance)
    {
        result = null;
        advance = 0;

        // Pass 1: locate the closing quote while confirming the whole token is ASCII with intact escape sequences.
        // Pure ASCII without escapes is the most common case, so first skip 16 bytes at a time with 4x uint.
        var hasEscape = false;
        var end = -1;
        var pos = 1;
        {
            var quote = '"' * 0x0101_0101u;
            var slash = '\\' * 0x0101_0101u;
            var vectorLimit = span.Length - 16;
            while (pos <= vectorLimit)
            {
                ref var head = ref MemoryMarshal.GetReference(span);
                ref var p = ref Unsafe.Add(ref head, pos);
                var v0 = Unsafe.ReadUnaligned<uint>(ref p);
                var v1 = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref p, 4));
                var v2 = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref p, 8));
                var v3 = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref p, 12));
                if (((v0 | v1 | v2 | v3) & 0x8080_8080u) != 0
                    || (ZeroByte(v0 ^ quote) | ZeroByte(v1 ^ quote) | ZeroByte(v2 ^ quote) | ZeroByte(v3 ^ quote)) != 0
                    || (ZeroByte(v0 ^ slash) | ZeroByte(v1 ^ slash) | ZeroByte(v2 ^ slash) | ZeroByte(v3 ^ slash)) != 0)
                {
                    // This block contains special bytes; let the scalar loop below handle it byte by byte (it handles escape skipping)
                    break;
                }

                pos += 16;
            }
        }

        for (; pos < span.Length; pos++)
        {
            var b = span[pos];
            if (b == (byte) '"')
            {
                end = pos;
                break;
            }
            if (b >= 0x80)
            {
                return false;
            }
            if (b != (byte) '\\')
            {
                continue;
            }
            hasEscape = true;
            if (pos + 1 >= span.Length)
            {
                return false;
            }
            pos += span[pos + 1] == (byte) 'u' ? 5 : 1;
            if (pos >= span.Length)
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
            result = SpanConverter.ToString(token);
            return true;
        }

        // Pass 2: count the output characters
        var length = 0;
        for (var i = 0; i < token.Length; i++)
        {
            if (token[i] == (byte) '\\')
            {
                // Incomplete escape sequence (truncated exactly at the end of the buffer): let the general path handle it
                if (i + 1 >= token.Length)
                {
                    return false;
                }

                if (token[i + 1] == (byte) 'u')
                {
                    if (i + 5 >= token.Length)
                    {
                        return false;
                    }

                    var h1 = Hex(token[i + 2]);
                    var h2 = Hex(token[i + 3]);
                    var h3 = Hex(token[i + 4]);
                    var h4 = Hex(token[i + 5]);
                    if (h1 < 0 || h2 < 0 || h3 < 0 || h4 < 0)
                    {
                        return false;
                    }
                    var code = (h1 << 12) | (h2 << 8) | (h3 << 4) | h4;
                    if (code > 0x7F)
                    {
                        // Non-ASCII escape goes to the general path (which writes back byte by byte)
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

        // Pass 3: write into the string in one pass
        var chars = new string('\0', length);
        var index = 0;
        fixed (char* dest = chars)
        {
            for (var i = 0; i < token.Length; i++)
            {
                if (token[i] != (byte) '\\')
                {
                    dest[index++] = (char) token[i];
                    continue;
                }
                // Reuse the existing decode logic so behavior matches the general path (pass 1 already guarantees no exception in range)
                int escapedLength;
                fixed (byte* source = span)
                {
                    dest[index++] = ReadEscapedCharUtf8(source + 1 + i, out escapedLength);
                }
                i += escapedLength - 1;
            }
        }

        result = chars;
        return true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Hex(byte c)
        {
            if (c is >= (byte) '0' and <= (byte) '9') return c - '0';
            if (c is >= (byte) 'a' and <= (byte) 'f') return c - 'a' + 10;
            if (c is >= (byte) 'A' and <= (byte) 'F') return c - 'A' + 10;
            return -1;
        }
    }

    #region Read Nullable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte? ReadNullableSbyteUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableSbyteFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToSbyte(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte? ReadNullableByteUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableByteFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToByte(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal? ReadNullableDecimalUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableDecimalFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToDecimal(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? ReadNullableDoubleUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableDoubleFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToDouble(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float? ReadNullableFloatUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableFloatFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToFloat(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short? ReadNullableShortUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableShortFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToShort(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int? ReadNullableIntUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableIntFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToInt(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long? ReadNullableLongUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableLongFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToLong(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort? ReadNullableUshortUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableUshortFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToUshort(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint? ReadNullableUintUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableUintFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToUint(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong? ReadNullableUlongUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableUlongFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToUlong(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char? ReadNullableCharUtf8(byte* src, out int advance)
    {
        var first = src[0];
        var span = ReadPrimitiveUntilEndUtf8(src, NullableCharFormatter.max_size, out advance);
        if (SpanConverter.IsNull(span)) return null;
        if (first != (byte) '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
        return SpanConverter.ToChar(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool? ReadNullableBoolUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, NullableBoolFormatter.max_size, out advance);
        return SpanConverter.IsNull(span) ? null : SpanConverter.ToBool(in span);
    }

    #endregion

    #region Read Primitive

    /// <summary>Reads an escaped character; supports UTF8 and UTF16</summary>
    // ReSharper disable once CognitiveComplexity
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ReadEscapedCharUtf8(byte* src, out int advance)
    {
        // Expect starting backslash
        if (src[0] != '\\') throw SerdeException.ThrowInvalidEscapedChar(src[0]);

        var esc = (char) src[1];
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
        static int Hex(byte c)
        {
            if (c is >= (byte) '0' and <= (byte) '9') return c - '0';
            if (c is >= (byte) 'A' and <= (byte) 'F') return c - 'A' + 10;
            if (c is >= (byte) 'a' and <= (byte) 'f') return c - 'a' + 10;
            return -1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadByteUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, ByteFormatter.max_size, out advance);
        return SpanConverter.ToByte(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ReadSbyteUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, SbyteFormatter.max_size, out advance);
        return SpanConverter.ToSbyte(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ReadDecimalUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, DecimalFormatter.max_size, out advance);
        return SpanConverter.ToDecimal(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReadDoubleUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, DoubleFormatter.max_size, out advance);
        return SpanConverter.ToDouble(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadFloatUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, FloatFormatter.max_size, out advance);
        return SpanConverter.ToFloat(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ReadBoolUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, BoolFormatter.max_size, out advance);
        return SpanConverter.ToBool(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ReadCharUtf8(byte* src, out int advance)
    {
        if ((char) src[0] != '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
        var span = ReadValueUntilEndUtf8(src, CharFormatter.max_size, out advance);
        return SpanConverter.ToChar(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadShortUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, ShortFormatter.max_size, out advance);
        return SpanConverter.ToShort(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadIntUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, IntFormatter.max_size, out advance);
        return SpanConverter.ToInt(in span);
    }

    /// <summary>Parses a small integer in one pass: the digits are read and converted at the same time.</summary>
    /// <remarks>
    /// <see cref="ReadIntUtf8"/> has to find the end of the token before it can parse it, which walks the digits twice.
    /// A cursor-free reader pays that twice per value, so the common case (sign plus at most nine digits) is parsed
    /// straight from the buffer; anything else falls back to the general path that also reports the JSON errors.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadIntFastUtf8(byte* src, out int advance)
    {
        var negative = src[0] == (byte) '-';
        var index = negative ? 1 : 0;
        var start = index;
        var value = 0;
        while (true)
        {
            var digit = src[index] - (byte) '0';
            if ((uint) digit > 9)
            {
                break;
            }
            value = (value * 10) + digit;
            index++;
        }

        // Ten digit values overflow an int, and a token that is not followed by a delimiter is not a number
        if (index == start || index - start > 9 || (src[index] != (byte) ',' && src[index] != (byte) ']' && src[index] != (byte) '}'))
        {
            return ReadIntUtf8(src, out advance);
        }

        advance = index;
        while (StringHelper.IsWhiteSpace(src[advance]))
        {
            advance++;
        }

        return negative ? -value : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadLongUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, LongFormatter.max_size, out advance);
        return SpanConverter.ToLong(in span);
    }

    /// <summary>Parses a small 64-bit integer in one pass, see <see cref="ReadIntFastUtf8"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadLongFastUtf8(byte* src, out int advance)
    {
        var negative = src[0] == (byte) '-';
        var index = negative ? 1 : 0;
        var start = index;
        long value = 0;
        while (true)
        {
            var digit = src[index] - (byte) '0';
            if ((uint) digit > 9)
            {
                break;
            }
            value = (value * 10) + digit;
            index++;
        }

        // Nineteen digit values overflow a long, and a token that is not followed by a delimiter is not a number
        if (index == start || index - start > 18 || (src[index] != (byte) ',' && src[index] != (byte) ']' && src[index] != (byte) '}'))
        {
            return ReadLongUtf8(src, out advance);
        }

        advance = index;
        while (StringHelper.IsWhiteSpace(src[advance]))
        {
            advance++;
        }

        return negative ? -value : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUshortUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, UshortFormatter.max_size, out advance);
        return SpanConverter.ToUshort(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadUintUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, UintFormatter.max_size, out advance);
        return SpanConverter.ToUint(in span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReadUlongUtf8(byte* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf8(src, UlongFormatter.max_size, out advance);
        return SpanConverter.ToUlong(in span);
    }
    #region Span Parsing

    /// <summary>Finds the end of the current primitive token and skips the whitespace after it.</summary>
    internal static ReadOnlySpan<byte> ReadPrimitiveUntilEndUtf8(ReadOnlySpan<byte> span, out int advance)
    {
        for (var i = 0; i < span.Length; i++)
        {
            var character = span[i];
            if (!StringHelper.IsWhiteSpace(character) && character is not ((byte) '}' or (byte) ']' or (byte) ','))
            {
                continue;
            }

            advance = i;
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

    /// <summary>Skips the whitespace that follows a token that ended at <paramref name="index"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SkipWhiteSpaceUtf8(ReadOnlySpan<byte> span, int index)
    {
        while (index < span.Length && StringHelper.IsWhiteSpace(span[index]))
        {
            index++;
        }

        return index;
    }

    /// <summary>Parses a small integer in one pass: the digits are read and accumulated at the same time.</summary>
    /// <remarks>
    /// The general path has to find the end of the token before it can parse it, which walks the digits twice. A
    /// cursor-free reader pays that twice per value, so the common case is parsed straight from the span; anything else
    /// falls back to the general path that also reports the JSON errors.
    /// </remarks>
    internal static int ReadIntFastUtf8(ReadOnlySpan<byte> span, out int advance)
    {
        ref var start = ref MemoryMarshal.GetReference(span);
        var negative = span[0] == (byte) '-';
        var index = negative ? 1 : 0;
        var first = index;
        var value = 0;
        while (index < span.Length)
        {
            var digit = Unsafe.Add(ref start, index) - '0';
            if ((uint) digit > 9)
            {
                break;
            }

            value = (value * 10) + digit;
            index++;
        }

        // Ten digit values overflow an int, and a token that is not followed by a delimiter is not a number
        if (index == first || index - first > 9 || index >= span.Length
            || (span[index] != (byte) ',' && span[index] != (byte) ']' && span[index] != (byte) '}'))
        {
            var token = ReadPrimitiveUntilEndUtf8(span, out advance);
            return SpanConverter.ToInt(in token);
        }

        advance = SkipWhiteSpaceUtf8(span, index);
        return negative ? -value : value;
    }

    /// <summary>Parses a small 64-bit integer in one pass, see <see cref="ReadIntFastUtf8(ReadOnlySpan{char}, out int)"/>.</summary>
    internal static long ReadLongFastUtf8(ReadOnlySpan<byte> span, out int advance)
    {
        ref var start = ref MemoryMarshal.GetReference(span);
        var negative = span[0] == (byte) '-';
        var index = negative ? 1 : 0;
        var first = index;
        long value = 0;
        while (index < span.Length)
        {
            var digit = Unsafe.Add(ref start, index) - '0';
            if ((uint) digit > 9)
            {
                break;
            }

            value = (value * 10) + digit;
            index++;
        }

        if (index == first || index - first > 18 || index >= span.Length
            || (span[index] != (byte) ',' && span[index] != (byte) ']' && span[index] != (byte) '}'))
        {
            var token = ReadPrimitiveUntilEndUtf8(span, out advance);
            return SpanConverter.ToLong(in token);
        }

        advance = SkipWhiteSpaceUtf8(span, index);
        return negative ? -value : value;
    }

    /// <summary>
    /// Reads <c>[-]digits[.digits]</c> into a mantissa and a fraction digit count. Returns false for exponents, long
    /// mantissas and anything the general parser has to handle.
    /// </summary>
    private static bool TryParseRealFastUtf8(ReadOnlySpan<byte> span, out ulong mantissa, out bool negative, out int fractionDigits, out int end)
    {
        mantissa = 0;
        negative = false;
        fractionDigits = 0;
        end = 0;
        if (span.IsEmpty)
        {
            return false;
        }

        ref var start = ref MemoryMarshal.GetReference(span);
        var index = 0;
        if (Unsafe.Add(ref start, 0) == (byte) '-')
        {
            negative = true;
            index = 1;
        }

        var digits = 0;
        var seenDot = false;
        while (index < span.Length)
        {
            var c = Unsafe.Add(ref start, index);
            var digit = c - '0';
            if ((uint) digit <= 9)
            {
                if (mantissa > 900_000_000)
                {
                    // More precision than the fast path can hold, the framework parser rounds it correctly
                    return false;
                }

                mantissa = (mantissa * 10) + (ulong) digit;
                digits++;
                if (seenDot)
                {
                    fractionDigits++;
                }

                index++;
                continue;
            }

            if (c == (byte) '.' && !seenDot)
            {
                seenDot = true;
                index++;
                continue;
            }

            break;
        }

        if (digits == 0 || digits > 15 || fractionDigits > 22 || index >= span.Length
            || (span[index] != (byte) ',' && span[index] != (byte) ']' && span[index] != (byte) '}'))
        {
            return false;
        }

        end = index;
        return true;
    }

    /// <summary>Parses a real number in one pass, see <see cref="TryParseRealFastUtf8"/> for the fallback.</summary>
    internal static float ReadFloatFastUtf8(ReadOnlySpan<byte> span, out int advance)
    {
        if (!TryParseRealFastUtf8(span, out var mantissa, out var negative, out var fractionDigits, out var end))
        {
            var token = ReadPrimitiveUntilEndUtf8(span, out advance);
            return SpanConverter.ToFloat(in token);
        }

        advance = SkipWhiteSpaceUtf8(span, end);
        double result = fractionDigits == 0 ? mantissa : mantissa / SpanConverter.doublePowersOfTen[fractionDigits];
        var value = (float) result;
        return negative ? -value : value;
    }

    /// <summary>Parses a real number in one pass, see <see cref="TryParseRealFastUtf8"/> for the fallback.</summary>
    internal static double ReadDoubleFastUtf8(ReadOnlySpan<byte> span, out int advance)
    {
        if (!TryParseRealFastUtf8(span, out var mantissa, out var negative, out var fractionDigits, out var end))
        {
            var token = ReadPrimitiveUntilEndUtf8(span, out advance);
            return SpanConverter.ToDouble(in token);
        }

        advance = SkipWhiteSpaceUtf8(span, end);
        var result = fractionDigits == 0 ? (double) mantissa : mantissa / SpanConverter.doublePowersOfTen[fractionDigits];
        return negative ? -result : result;
    }

    /// <summary>Parses a literal directly; anything that is not <c>true</c> or <c>false</c> takes the general path.</summary>
    internal static bool ReadBoolFastUtf8(ReadOnlySpan<byte> span, out int advance)
    {
        ref var start = ref MemoryMarshal.GetReference(span);
        if (span.Length >= 4
            && Unsafe.Add(ref start, 0) == (byte) 't' && Unsafe.Add(ref start, 1) == (byte) 'r' && Unsafe.Add(ref start, 2) == (byte) 'u' && Unsafe.Add(ref start, 3) == (byte) 'e')
        {
            advance = 4;
            return true;
        }

        if (span.Length >= 5
            && Unsafe.Add(ref start, 0) == (byte) 'f' && Unsafe.Add(ref start, 1) == (byte) 'a' && Unsafe.Add(ref start, 2) == (byte) 'l' && Unsafe.Add(ref start, 3) == (byte) 's' && Unsafe.Add(ref start, 4) == (byte) 'e')
        {
            advance = 5;
            return false;
        }

        var token = ReadPrimitiveUntilEndUtf8(span, out advance);
            return SpanConverter.ToBool(in token);
    }

    /// <summary>Reads a single character string directly; escaped characters take the general path.</summary>
    internal static char ReadCharFastUtf8(ReadOnlySpan<byte> span, out int advance)
    {
        ref var start = ref MemoryMarshal.GetReference(span);
        if (span.Length >= 3 && Unsafe.Add(ref start, 0) == (byte) '"' && Unsafe.Add(ref start, 2) == (byte) '"')
        {
            advance = 3;
            return (char) Unsafe.Add(ref start, 1);
        }

        // The general path has to strip the quotes, so it stays on the pointer overload
        fixed (byte* src = span)
        {
            return ReadCharUtf8(src, out advance);
        }
    }

    #endregion
}

#endregion