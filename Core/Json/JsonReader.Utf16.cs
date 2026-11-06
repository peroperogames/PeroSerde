/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/20
 */

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    internal static string ReadStringValueUtf16(ReadOnlySpan<char> span, out int advance)
    {
        advance = 0;
        if (span.Length > 0 && span[0] == '"')
        {
            if (TryReadStringUtf16(span, out var value, out advance))
            {
                return value;
            }
        }

        fixed (char* src = span)
        {
            return ReadStringValueUtf16(src, span.Length, out advance);
        }
    }

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

    /// <summary>Finds the next <c>"</c> or <c>\</c> at or after <paramref name="start"/>, or -1 when there is none.</summary>
    /// <remarks>
    /// The string scan is the hot path of both keys and values, so it is delegated to the vectorised
    /// <see cref="MemoryExtensions.IndexOfAny{T}(ReadOnlySpan{T},T,T)"/> instead of walking characters one by one.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IndexOfQuoteOrBackslashUtf16(ReadOnlySpan<char> span, int start)
    {
        if ((uint) start >= (uint) span.Length)
        {
            return -1;
        }

        var offset = span.Slice(start).IndexOfAny('"', '\\');
        return offset < 0 ? -1 : start + offset;
    }

    /// <summary>
    /// Reads a JSON string value (escapes allowed) with a single heap allocation.
    /// </summary>
    /// <remarks>
    /// Scans the whole buffer up to <paramref name="maxLength"/>, so a string ending at the end of the buffer still parses correctly;
    /// Returns false on an illegal escape, and the caller takes the general path to throw the original exception.
    /// </remarks>
    internal static bool TryReadStringUtf16(char* src, int maxLength, out string result, out int advance)
        => TryReadStringUtf16(new ReadOnlySpan<char>(src, maxLength), out result, out advance);

    internal static bool TryReadStringUtf16(ReadOnlySpan<char> span, out string result, out int advance)
    {
        result = null;
        advance = 0;

        var hasEscape = false;
        var end = -1;
        var position = 1;
        while (true)
        {
            var found = IndexOfQuoteOrBackslashUtf16(span, position);
            if (found < 0)
            {
                return false;
            }

            if (span[found] == '"')
            {
                end = found;
                break;
            }

            hasEscape = true;
            if (found + 1 >= span.Length)
            {
                return false;
            }

            // Skip the escape sequence: \uXXXX is six characters, every other escape is two
            position = found + (span[found + 1] == 'u' ? 6 : 2);
            if (position > span.Length)
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
        fixed (char* source = span)
        {
            for (var i = 0; i < token.Length; i++)
            {
                if (token[i] != '\\')
                {
                    dest[index++] = token[i];
                    continue;
                }
                dest[index++] = ReadEscapedCharUtf16(source + 1 + i, out var escapedLength);
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

    public static long ReadLongUtf16(char* src, out int advance)
    {
        var span = ReadPrimitiveUntilEndUtf16(src, LongFormatter.max_size, out advance);
        return SpanConverter.ToLong(in span);
    }

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
    #region Span Parsing

    /// <summary>Finds the end of the current primitive token and skips the whitespace after it.</summary>
    internal static ReadOnlySpan<char> ReadPrimitiveUntilEndUtf16(ReadOnlySpan<char> span, out int advance)
    {
        for (var i = 0; i < span.Length; i++)
        {
            var character = span[i];
            if (!StringHelper.IsWhiteSpace(character) && character is not ('}' or ']' or ','))
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
    private static int SkipWhiteSpaceUtf16(ReadOnlySpan<char> span, int index)
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
    internal static int ReadIntFastUtf16(ReadOnlySpan<char> span, out int advance)
    {
        ref var start = ref MemoryMarshal.GetReference(span);
        var negative = span[0] == '-';
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
            || (span[index] != ',' && span[index] != ']' && span[index] != '}'))
        {
            var token = ReadPrimitiveUntilEndUtf16(span, out advance);
            return SpanConverter.ToInt(in token);
        }

        advance = SkipWhiteSpaceUtf16(span, index);
        return negative ? -value : value;
    }

    /// <summary>Parses a small 64-bit integer in one pass, see <see cref="ReadIntFastUtf16(ReadOnlySpan{char}, out int)"/>.</summary>
    internal static long ReadLongFastUtf16(ReadOnlySpan<char> span, out int advance)
    {
        ref var start = ref MemoryMarshal.GetReference(span);
        var negative = span[0] == '-';
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
            || (span[index] != ',' && span[index] != ']' && span[index] != '}'))
        {
            var token = ReadPrimitiveUntilEndUtf16(span, out advance);
            return SpanConverter.ToLong(in token);
        }

        advance = SkipWhiteSpaceUtf16(span, index);
        return negative ? -value : value;
    }

    /// <summary>
    /// Reads <c>[-]digits[.digits]</c> into a mantissa and a fraction digit count. Returns false for exponents, long
    /// mantissas and anything the general parser has to handle.
    /// </summary>
    private static bool TryParseRealFastUtf16(ReadOnlySpan<char> span, out ulong mantissa, out bool negative, out int fractionDigits, out int end)
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
        if (Unsafe.Add(ref start, 0) == '-')
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

            if (c == '.' && !seenDot)
            {
                seenDot = true;
                index++;
                continue;
            }

            break;
        }

        if (digits == 0 || digits > 15 || fractionDigits > 22 || index >= span.Length
            || (span[index] != ',' && span[index] != ']' && span[index] != '}'))
        {
            return false;
        }

        end = index;
        return true;
    }

    /// <summary>Parses a real number in one pass, see <see cref="TryParseRealFastUtf16"/> for the fallback.</summary>
    internal static float ReadFloatFastUtf16(ReadOnlySpan<char> span, out int advance)
    {
        if (!TryParseRealFastUtf16(span, out var mantissa, out var negative, out var fractionDigits, out var end))
        {
            var token = ReadPrimitiveUntilEndUtf16(span, out advance);
            return SpanConverter.ToFloat(in token);
        }

        // Mantissa and power of ten are both exact in float only below 2^24 and 1e10; anything else goes through the
        // framework parser, because narrowing a rounded double to float can round twice
        if (mantissa >= 1 << 24 || fractionDigits > 10)
        {
            var token = ReadPrimitiveUntilEndUtf16(span, out advance);
            return SpanConverter.ToFloat(in token);
        }

        advance = SkipWhiteSpaceUtf16(span, end);
        var value = fractionDigits == 0 ? (float) mantissa : (float) mantissa / SpanConverter.floatPowersOfTen[fractionDigits];
        return negative ? -value : value;
    }

    /// <summary>Parses a real number in one pass, see <see cref="TryParseRealFastUtf16"/> for the fallback.</summary>
    internal static double ReadDoubleFastUtf16(ReadOnlySpan<char> span, out int advance)
    {
        if (!TryParseRealFastUtf16(span, out var mantissa, out var negative, out var fractionDigits, out var end))
        {
            var token = ReadPrimitiveUntilEndUtf16(span, out advance);
            return SpanConverter.ToDouble(in token);
        }

        advance = SkipWhiteSpaceUtf16(span, end);
        var result = fractionDigits == 0 ? mantissa : mantissa / SpanConverter.doublePowersOfTen[fractionDigits];
        return negative ? -result : result;
    }

    /// <summary>Parses a literal directly; anything that is not <c>true</c> or <c>false</c> takes the general path.</summary>
    internal static bool ReadBoolFastUtf16(ReadOnlySpan<char> span, out int advance)
    {
        ref var start = ref MemoryMarshal.GetReference(span);
        if (span.Length >= 4
            && Unsafe.Add(ref start, 0) == 't' && Unsafe.Add(ref start, 1) == 'r' && Unsafe.Add(ref start, 2) == 'u' && Unsafe.Add(ref start, 3) == 'e')
        {
            advance = 4;
            return true;
        }

        if (span.Length >= 5
            && Unsafe.Add(ref start, 0) == 'f' && Unsafe.Add(ref start, 1) == 'a' && Unsafe.Add(ref start, 2) == 'l' && Unsafe.Add(ref start, 3) == 's' && Unsafe.Add(ref start, 4) == 'e')
        {
            advance = 5;
            return false;
        }

        var token = ReadPrimitiveUntilEndUtf16(span, out advance);
            return SpanConverter.ToBool(in token);
    }

    /// <summary>Reads a single character string directly; escaped characters take the general path.</summary>
    internal static char ReadCharFastUtf16(ReadOnlySpan<char> span, out int advance)
    {
        ref var start = ref MemoryMarshal.GetReference(span);
        if (span.Length >= 3 && Unsafe.Add(ref start, 0) == '"' && Unsafe.Add(ref start, 2) == '"')
        {
            advance = 3;
            return Unsafe.Add(ref start, 1);
        }

        // The general path has to strip the quotes, so it stays on the pointer overload
        fixed (char* src = span)
        {
            return ReadCharUtf16(src, out advance);
        }
    }

    #endregion
}

#endregion