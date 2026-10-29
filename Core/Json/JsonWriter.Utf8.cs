/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/05
 */

using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json;

public static unsafe partial class JsonWriter
{
    #region Write String

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteRawUtf8(byte* dest, ReadOnlySpan<byte> src)
    {
        fixed (byte* srcPtr = src)
        {
            Unsafe.CopyBlockUnaligned(dest, srcPtr, (uint) src.Length);
        }
        return src.Length;
    }

    public static int WriteStringUtf8(byte* dest, ReadOnlySpan<char> value)
    {
        if (value == null)
            return WriteNullUtf8(dest);
        var start = dest;
        dest += WriteDoubleQuoteUtf8(dest);
        dest += WriteStringValueUtf8(value, dest);
        dest += WriteDoubleQuoteUtf8(dest);
        return (int) (dest - start);
    }

    public static int WriteStringNoEscapeUtf8(byte* dest, ReadOnlySpan<char> value)
    {
        if (value == null)
            return WriteNullUtf8(dest);
        var start = dest;
        dest += WriteDoubleQuoteUtf8(dest);
        dest += WriteStringValueNoEscapeUtf8(value, dest);
        dest += WriteDoubleQuoteUtf8(dest);
        return (int) (dest - start);
    }

    public static int WriteStringValueUtf8(ReadOnlySpan<char> str, byte* dest)
    {
        // Fast path: when nothing needs escaping, write the whole string directly (the scan is already vectorized)
        if (!StringHelper.ContainsEscapeChar(str))
        {
            return WriteStringValueNoEscapeUtf8(str, dest);
        }

        // Escape characters must be handled, so the string has to be walked to find them and written separately.
        var from = 0;
        var start = dest;
        for (var i = 0; i < str.Length; i++)
        {
            ref readonly var c = ref str[i];
            if (c < 0x20 || c is '"' or '\\')
            {
                if (i - from > 0)
                {
                    dest += WriteStringValueNoEscapeUtf8(str.Slice(from, i - from), dest);
                }
                dest += WriteEscapedCharUtf8(c, dest);
                from = i + 1;
            }
        }
        if (from < str.Length)
        {
            dest += WriteStringValueNoEscapeUtf8(str[from..], dest);
        }
        return (int) (dest - start);
    }

    /// <summary>
    /// Writes a UTF8 sequence without considering escape characters.
    /// 
    /// </summary>
    /// <remarks>
    /// 1. If the string contains characters that need escaping, the result will be invalid.
    /// 2. The available length at dest must be greater than <c>Encoding.UTF8.GetMaxByteCount(str.Length)</c>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteStringValueNoEscapeUtf8(ReadOnlySpan<char> str, byte* dest)
    {
        // ASCII fast path: JSON keys and most strings are ASCII, so copying byte by byte is enough.
        // On Mono each Encoding.UTF8.GetBytes call costs about 150ns, while copying 10 characters byte by byte costs about 20ns.
        var start = dest;
        for (var i = 0; i < str.Length; i++)
        {
            var c = str[i];
            if (c >= 0x80)
            {
                // Contains non-ASCII characters; fall back to the encoder and rewrite from the start position
                var maxByteCount = Encoding.UTF8.GetMaxByteCount(str.Length);
                return Encoding.UTF8.GetBytes(str, new Span<byte>(start, maxByteCount));
            }
            *dest = (byte) c;
            dest++;
        }
        return (int) (dest - start);
    }

    #endregion

    #region Write Primitive

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBoolUtf8(byte* dest, bool value)
    {
        // UTF8 has no endianness concerns
        const uint eurt = 0x65757274U;
        const uint slaf = 0x736C6166U;

        if (value)
        {
            Unsafe.WriteUnaligned(dest, eurt);
            return 4;
        }
        Unsafe.WriteUnaligned(dest, slaf);
        dest[4] = (byte) 'e';
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteLongUtf8(byte* dest, long value) =>
        NumberHelper.ToUtf8StringUnsafe(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteUlongUtf8(byte* dest, ulong value) =>
        NumberHelper.ToUtf8StringUnsafe(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteFloatUtf8(byte* dest, float value) =>
        // TODO Write the formatted value as a string or as the default value?
        NumberHelper.ToUtf8StringUnsafe(value, dest);

    public static int WriteDoubleUtf8(byte* dest, double value) =>
        NumberHelper.ToUtf8StringUnsafe(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteDecimalUtf8(byte* dest, decimal value)
    {
        Utf8Formatter.TryFormat(
            value,
            new Span<byte>(dest, DecimalFormatter.max_size),
            out var bytesWritten
        );
        return bytesWritten;
    }

    public static int WriteCharUtf8(byte* dest, char value)
    {
        var start = dest;
        dest += WriteDoubleQuoteUtf8(dest);
        if (value < 0x20 || value is '"' or '\\') // Escape control and special characters
        {
            dest += WriteEscapedCharUtf8(value, dest);
        }
        else if (value < 0x80)
        {
            *dest = (byte) value;
            dest += 1;
        }
        // Surrogate pairs are not supported yet
        else if (char.IsSurrogate(value))
        {
            throw SerdeException.ThrowInvalidSurrogateChar(value);
        }
        // 2-byte UTF-8 sequence
        else if (value <= 0x7FF)
        {
            dest[0] = (byte) (0xC0 | (value >> 6));
            dest[1] = (byte) (0x80 | (value & 0x3F));
            dest += 2;
        }
        // 3-byte UTF-8 sequence
        else
        {
            dest[0] = (byte) (0xE0 | (value >> 12));
            dest[1] = (byte) (0x80 | ((value >> 6) & 0x3F));
            dest[2] = (byte) (0x80 | (value & 0x3F));
            dest += 3;
        }
        dest += WriteDoubleQuoteUtf8(dest);
        return (int) (dest - start);
    }

    /// <summary>Writes an escaped character; supports UTF8 and UTF16</summary>
    // ReSharper disable once CognitiveComplexity
    public static int WriteEscapedCharUtf8(char value, byte* dest)
    {
        return value switch
        {
            '"' => WriteSingleEscapedChar(dest, '"'),
            '\\' => WriteSingleEscapedChar(dest, '\\'),
            '\u0000' => WriteDoubleEscapedChar(dest, '0', '0'),
            '\u0001' => WriteDoubleEscapedChar(dest, '0', '1'),
            '\u0002' => WriteDoubleEscapedChar(dest, '0', '2'),
            '\u0003' => WriteDoubleEscapedChar(dest, '0', '3'),
            '\u0004' => WriteDoubleEscapedChar(dest, '0', '4'),
            '\u0005' => WriteDoubleEscapedChar(dest, '0', '5'),
            '\u0006' => WriteDoubleEscapedChar(dest, '0', '6'),
            '\u0007' => WriteDoubleEscapedChar(dest, '0', '7'),
            '\b' => WriteSingleEscapedChar(dest, 'b'),
            '\t' => WriteSingleEscapedChar(dest, 't'),
            '\n' => WriteSingleEscapedChar(dest, 'n'),
            '\u000B' => WriteDoubleEscapedChar(dest, '0', 'B'),
            '\f' => WriteSingleEscapedChar(dest, 'f'),
            '\r' => WriteSingleEscapedChar(dest, 'r'),
            '\u000E' => WriteDoubleEscapedChar(dest, '0', 'E'),
            '\u000F' => WriteDoubleEscapedChar(dest, '0', 'F'),
            '\u0010' => WriteDoubleEscapedChar(dest, '1', '0'),
            '\u0011' => WriteDoubleEscapedChar(dest, '1', '1'),
            '\u0012' => WriteDoubleEscapedChar(dest, '1', '2'),
            '\u0013' => WriteDoubleEscapedChar(dest, '1', '3'),
            '\u0014' => WriteDoubleEscapedChar(dest, '1', '4'),
            '\u0015' => WriteDoubleEscapedChar(dest, '1', '5'),
            '\u0016' => WriteDoubleEscapedChar(dest, '1', '6'),
            '\u0017' => WriteDoubleEscapedChar(dest, '1', '7'),
            '\u0018' => WriteDoubleEscapedChar(dest, '1', '8'),
            '\u0019' => WriteDoubleEscapedChar(dest, '1', '9'),
            '\u001A' => WriteDoubleEscapedChar(dest, '1', 'A'),
            '\u001B' => WriteDoubleEscapedChar(dest, '1', 'B'),
            '\u001C' => WriteDoubleEscapedChar(dest, '1', 'C'),
            '\u001D' => WriteDoubleEscapedChar(dest, '1', 'D'),
            '\u001E' => WriteDoubleEscapedChar(dest, '1', 'E'),
            '\u001F' => WriteDoubleEscapedChar(dest, '1', 'F'),
            _ => throw SerdeException.ThrowInvalidEscapedChar(value)
        };

        // Write an \x-style escape sequence. Such a character takes 2 positions in the text
        static int WriteSingleEscapedChar(byte* dest, char toEscape)
        {
            dest[0] = (byte) '\\';
            dest[1] = (byte) toEscape;
            return 2;
        }

        // Write a \u00xx-style escape sequence. Such a character takes 6 positions in the text
        static int WriteDoubleEscapedChar(byte* dest, char firstToEscape, char secondToEscape)
        {
            // The control characters that need escaping range from \u0000 to \u001F, so the first four characters are always \u00
            dest[0] = (byte) '\\';
            dest[1] = (byte) 'u';
            dest[2] = (byte) '0';
            dest[3] = (byte) '0';
            dest[4] = (byte) firstToEscape;
            dest[5] = (byte) secondToEscape;
            return 6;
        }
    }

    #endregion

    #region Write Literal

    /// <summary>Writes an ASCII colon</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteKeyEndingUtf8(byte* dest)
    {
        *dest = (byte) ':';
        return 1;
    }

    /// <summary>Writes an ASCII comma</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteValueEndingUtf8(byte* dest)
    {
        *dest = (byte) ',';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteDoubleQuoteUtf8(byte* dest)
    {
        *dest = (byte) '"';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteSpaceUtf8(byte* dest)
    {
        *dest = (byte) ' ';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeginObjectUtf8(byte* dest)
    {
        *dest = (byte) '{';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteEndObjectUtf8(byte* dest)
    {
        *dest = (byte) '}';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeginArrayUtf8(byte* dest)
    {
        *dest = (byte) '[';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteEndArrayUtf8(byte* dest)
    {
        *dest = (byte) ']';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteAscii(byte* dest, char ascii)
    {
        *dest = (byte) ascii;
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteNullUtf8(byte* dest)
    {
        // Utf8 has no endianness concerns
        const uint llun = 0x6C6C756EU;
        Unsafe.WriteUnaligned(dest, llun);
        return 4;
    }

    #endregion

    #region Write Indent

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteIndent(int count, byte* dest)
    {
        for (var i = 0; i < count; i++)
        {
            dest[i] = (byte) ' ';
        }
        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteNewLine(byte* dest)
    {
        *dest = (byte) '\n';
        return 1;
    }

    #endregion
}