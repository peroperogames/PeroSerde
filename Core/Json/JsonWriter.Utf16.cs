/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/05
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

public static unsafe partial class JsonWriter
{
    #region Write Primitive

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static int WriteBoolUtf16(char* dest, bool value)
    {
        const ulong le_eurt = 0x0065007500720074;
        const ulong le_slaf = 0x0073006C00610066;
        const ulong be_eurt = 0x6500750072007400;
        const ulong be_slaf = 0x73006C0061006600;
        if (value)
        {
            Unsafe.WriteUnaligned(dest, BitConverter.IsLittleEndian ? le_eurt : be_eurt);
            return 4;
        }
        Unsafe.WriteUnaligned(dest, BitConverter.IsLittleEndian ? le_slaf : be_slaf);
        dest[4] = 'e';
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteLongUtf16(char* dest, long value)
        => NumberHelper.ToUtf16StringUnsafe(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteUlongUtf16(char* dest, ulong value)
        => NumberHelper.ToUtf16StringUnsafe(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteFloatUtf16(char* dest, float value)
        => NumberHelper.ToUtf16StringUnsafe(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteDoubleUtf16(char* dest, double value)
        => NumberHelper.ToUtf16StringUnsafe(value, dest);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteDecimalUtf16(char* dest, decimal value)
    {
        var destSpan = new Span<char>(dest, DecimalFormatter.max_size);
        value.TryFormat(destSpan, out var charsWritten, default, CultureInfo.InvariantCulture);
        return charsWritten;
    }

    #endregion

    #region Write Char

    public static int WriteCharUtf16(char* dest, char value)
    {
        var start = dest;
        dest += WriteDoubleQuoteUtf16(dest);
        if (value < 0x20 || value is '"' or '\\') // Escape control and special characters
        {
            dest += WriteEscapedCharUtf16(value, dest);
        }
        else
        {
            *dest = value;
            dest += 1;
        }
        dest += WriteDoubleQuoteUtf16(dest);
        return (int) (dest - start);
    }

    /// <summary>Writes an escaped character; supports UTF8 and UTF16</summary>
    // ReSharper disable once CognitiveComplexity
    public static int WriteEscapedCharUtf16(char value, char* dest)
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
        static int WriteSingleEscapedChar(char* dest, char toEscape)
        {
            dest[0] = '\\';
            dest[1] = toEscape;
            return 2;
        }

        // Write a \u00xx-style escape sequence. Such a character takes 6 positions in the text
        static int WriteDoubleEscapedChar(char* dest, char firstToEscape, char secondToEscape)
        {
            // The control characters that need escaping range from \u0000 to \u001F, so the first four characters are always \u00
            dest[0] = '\\';
            dest[1] = 'u';
            dest[2] = '0';
            dest[3] = '0';
            dest[4] = firstToEscape;
            dest[5] = secondToEscape;
            return 6;
        }
    }

    #endregion

    #region Write String

    public static int WriteStringUtf16(char* dest, ReadOnlySpan<char> value)
    {
        if (value == null)
            return WriteNullUtf16(dest);
        var start = dest;
        dest += WriteDoubleQuoteUtf16(dest);
        dest += WriteStringValueUtf16(value, dest);
        dest += WriteDoubleQuoteUtf16(dest);
        return (int) (dest - start);
    }

    /// <summary>Writes a string without considering escape characters</summary>
    /// <return>charsWritten</return>
    /// <remarks>The caller must ensure no out-of-bounds access happens</remarks>
    public static int WriteStringNoEscapeUtf16(char* dest, ReadOnlySpan<char> str)
    {
        if (str == null)
            return WriteNullUtf16(dest);
        var start = dest;
        dest += WriteDoubleQuoteUtf16(dest);
        dest += WriteRawUtf16(dest, str);
        dest += WriteDoubleQuoteUtf16(dest);
        return (int) (dest - start);
    }

    /// <summary>Writes a <see cref="string"/> into a UTF16 buffer.</summary>
    /// <returns>charsWritten</returns>
    /// <remarks>Takes escape characters into account.</remarks>
    public static int WriteStringValueUtf16(ReadOnlySpan<char> str, char* dest)
    {
        // Fast path: when nothing needs escaping, write the whole string directly (the scan is already vectorized)
        if (!StringHelper.ContainsEscapeChar(str))
        {
            return WriteRawUtf16(dest, str);
        }
        return WriteStringValueEscapedUtf16(str, dest);
    }

    /// <summary>Writes a <see cref="string"/> into a UTF16 buffer.</summary>
    /// <param name="str">The value to write</param>
    /// <param name="dest">The destination</param>
    /// <param name="hasEscape">Whether the value contains a character that has to be escaped, as returned by
    /// <see cref="StringHelper.ContainsEscapeChar(ReadOnlySpan{char})"/>. The caller already scans for the reserve
    /// size, so it hands the result over instead of walking the string twice.</param>
    /// <returns>charsWritten</returns>
    public static int WriteStringValueUtf16(ReadOnlySpan<char> str, char* dest, bool hasEscape)
    {
        return hasEscape ? WriteStringValueEscapedUtf16(str, dest) : WriteRawUtf16(dest, str);
    }

    private static int WriteStringValueEscapedUtf16(ReadOnlySpan<char> str, char* dest)
    {

        // Escape characters must be handled, so the string has to be walked to find them and written separately.
        var originalPtr = dest;
        var from = 0;
        for (var i = 0; i < str.Length; i++)
        {
            ref readonly var c = ref str[i];
            if (c < 0x20 || c is '"' or '\\')
            {
                if (i - from > 0)
                {
                    // The string is already UTF16, so it can be written directly regardless of endianness
                    dest += WriteRawUtf16(dest, str.Slice(from, i - from));
                }
                dest += WriteEscapedCharUtf16(c, dest);
                from = i + 1;
            }
        }
        if (from < str.Length)
        {
            dest += WriteRawUtf16(dest, str[from..]);
        }
        return (int) (dest - originalPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteRawUtf16(char* dest, ReadOnlySpan<char> src)
    {
        fixed (char* srcPtr = src)
        {
            Unsafe.CopyBlockUnaligned(dest, srcPtr, (uint) src.Length * sizeof(char));
        }
        return src.Length;
    }

    #endregion

    #region Write Literal

    /// <summary>Writes an ASCII colon</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteKeyEndingUtf16(char* dest)
    {
        *dest = ':';
        return 1;
    }

    /// <summary>Writes an ASCII comma</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteValueEndingUtf16(char* dest)
    {
        *dest = ',';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteDoubleQuoteUtf16(char* dest)
    {
        *dest = '"';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteSpaceUtf16(char* dest)
    {
        *dest = ' ';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeginObjectUtf16(char* dest)
    {
        *dest = '{';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteEndObjectUtf16(char* dest)
    {
        *dest = '}';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeginArrayUtf16(char* dest)
    {
        *dest = '[';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteEndArrayUtf16(char* dest)
    {
        *dest = ']';
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteNullUtf16(char* dest)
    {
        // Little-endian
        const ulong le_llun = 0x006C006C0075006E;
        // Big-endian
        const ulong be_llun = 0x6C006C0075006E00;
        Unsafe.WriteUnaligned(dest, BitConverter.IsLittleEndian ? le_llun : be_llun);
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteChar(char* dest, char character)
    {
        *dest = character;
        return 1;
    }

    #endregion

    #region Write Indent

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteIndent(int count, char* dest)
    {
        for (var i = 0; i < count; i++)
        {
            dest[i] = ' ';
        }
        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteNewLine(char* dest)
    {
        *dest = '\n';
        return 1;
    }

    #endregion
}