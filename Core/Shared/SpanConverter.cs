/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Buffers.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde;

/// <summary>
/// UTF8/UTF16 primitive type converter
/// </summary>
public static class SpanConverter
{
    #region Null

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool IsNull(in ReadOnlySpan<byte> span)
    {
        // Utf8 does not need to consider byte order
        const uint llun = 0x6C6C756EU;
        if (span.Length != 4)
        {
            return false;
        }
        fixed (byte* ptr = span)
        {
            return Unsafe.ReadUnaligned<uint>(ptr) == llun;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool IsNull(in ReadOnlySpan<char> span)
    {
        // Little endian
        const ulong leLlun = 0x006C_006C_0075_006E;
        // Big endian
        const ulong beLlun = 0x6C00_6C00_7500_6E00;

        if (span.Length != 4)
        {
            return false;
        }
        fixed (char* ptr = span)
        {
            var value = Unsafe.ReadUnaligned<ulong>(ptr);
            return BitConverter.IsLittleEndian ? value == leLlun : value == beLlun;
        }
    }

    #endregion

    #region UTF16

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ToSbyte(in ReadOnlySpan<char> span) =>
        TryParseIntegerUtf16(span, out var value) && value >= sbyte.MinValue && value <= sbyte.MaxValue
            ? (sbyte) value
            : sbyte.TryParse(span, out var result)
                ? result
                : throw SerdeException.ThrowInvalidParsing<sbyte>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ToByte(in ReadOnlySpan<char> span) =>
        TryParseIntegerUtf16(span, out var value) && value >= byte.MinValue && value <= byte.MaxValue
            ? (byte) value
            : byte.TryParse(span, out var result)
                ? result
                : throw SerdeException.ThrowInvalidParsing<byte>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime ToDateTime(in ReadOnlySpan<char> span) => DateTime.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<DateTime>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(in ReadOnlySpan<char> span) => new(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ToDecimal(in ReadOnlySpan<char> span) => decimal.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<decimal>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(in ReadOnlySpan<char> span)
    {
        if (!TryParseRealUtf16(span, out var mantissa, out var exponent, out var negative))
        {
            return ParseDoubleSlow(span);
        }
        // Clinger's fast path: the mantissa and the power of ten are both exact in double, so the multiply/divide
        // rounds correctly and casting then rounds once more to float
        var result = exponent >= 0
            ? mantissa * doublePowersOfTen[exponent]
            : mantissa / doublePowersOfTen[-exponent];
        return negative ? -result : result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToFloat(in ReadOnlySpan<char> span)
    {
        if (!TryParseRealUtf16(span, out var mantissa, out var exponent, out var negative))
        {
            return ParseFloatSlow(span);
        }
        var result = exponent >= 0
            ? (float) (mantissa * doublePowersOfTen[exponent])
            : (float) (mantissa / doublePowersOfTen[-exponent]);
#if DEBUG
        // The fast path above rounds through double; fail loudly if that ever disagrees with the framework parser
        var expected = ParseFloatSlow(span);
        if (!result.Equals(expected))
        {
            throw new InvalidOperationException($"float fast path mismatch: {new string(span)} -> {result} != {expected}");
        }
#endif
        return negative ? -result : result;
    }

    /// <summary>Parses a float with the framework parser, without rounding through double first.</summary>
    internal static float ParseFloatFramework(in ReadOnlySpan<char> span)
    {
        if (!float.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            throw SerdeException.ThrowInvalidParsing<float>();
        }

        return result == 0.0f && span.Length > 0 && span[0] == '-' ? -0.0f : result;
    }

    /// <summary>Parses a float with the framework parser; the token of a JSON number is pure ASCII.</summary>
    internal static float ParseFloatFramework(in ReadOnlySpan<byte> span)
    {
        Span<char> chars = span.Length <= 64 ? stackalloc char[span.Length] : new char[span.Length];
        for (var i = 0; i < span.Length; i++)
        {
            chars[i] = (char) span[i];
        }

        if (!float.TryParse(chars, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            throw SerdeException.ThrowInvalidParsing<float>();
        }

        return result == 0.0f && span.Length > 0 && span[0] == (byte) '-' ? -0.0f : result;
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static double ParseDoubleSlow(in ReadOnlySpan<char> span)
    {
        // Json numbers are culture invariant, and the invariant parser is the fast one
        if (!double.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            throw SerdeException.ThrowInvalidParsing<double>();
        }
        // On the Mono runtime used by Unity, double.TryParse("-0") drops the sign bit of negative zero; restore it here (consistent with CoreCLR)
        return result == 0.0 && span.Length > 0 && span[0] == '-' ? -0.0 : result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static float ParseFloatSlow(in ReadOnlySpan<char> span)
    {
        // Json numbers are culture invariant, and the invariant parser is the fast one
        if (!float.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            throw SerdeException.ThrowInvalidParsing<float>();
        }
        // On the Mono runtime used by Unity, float.TryParse("-0") drops the sign bit of negative zero; restore it here (consistent with CoreCLR)
        return result == 0f && span.Length > 0 && span[0] == '-' ? -0.0f : result;
    }

    // Exact powers of ten that are representable in double, the limit for Clinger's fast path
    /// <summary>Powers of ten that are exact in <see cref="float"/>, the limit for the float fast path.</summary>
    internal static readonly float[] floatPowersOfTen =
    {
        1e0f, 1e1f, 1e2f, 1e3f, 1e4f, 1e5f, 1e6f, 1e7f, 1e8f, 1e9f, 1e10f
    };

    internal static readonly double[] doublePowersOfTen =
    {
        1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9, 1e10, 1e11, 1e12, 1e13, 1e14, 1e15, 1e16, 1e17, 1e18,
        1e19, 1e20, 1e21, 1e22
    };

    /// <summary>
    /// Parses <c>[-]digits[.digits][e[+-]digits]</c> into a mantissa and a power of ten.
    /// </summary>
    /// <remarks>
    /// Returns false when the value does not fit the exact fast path (more than 15 significant digits, a mantissa that
    /// does not fit in 2^53, or an exponent outside ±22), which leaves the work to the framework parser.
    /// </remarks>
    private static bool TryParseRealUtf16(in ReadOnlySpan<char> span, out ulong mantissa, out int exponent, out bool negative)
    {
        mantissa = 0;
        exponent = 0;
        negative = false;

        var index = 0;
        if (span.IsEmpty) return false;
        if (span[0] == '-')
        {
            negative = true;
            index = 1;
        }
        else if (span[0] == '+')
        {
            index = 1;
        }

        var digits = 0;
        var seenDigit = false;
        var fraction = 0;
        for (; index < span.Length; index++)
        {
            var c = span[index];
            if ((uint) (c - '0') > 9) break;
            seenDigit = true;
            if (digits < 15)
            {
                // Skip leading zeros, they do not consume precision
                if (mantissa == 0 && c == '0')
                {
                    continue;
                }
                mantissa = mantissa * 10 + (ulong) (c - '0');
                digits++;
            }
            else
            {
                // Too many significant digits for the exact fast path
                return false;
            }
        }

        if (index < span.Length && span[index] == '.')
        {
            index++;
            for (; index < span.Length; index++)
            {
                var c = span[index];
                if ((uint) (c - '0') > 9) break;
                seenDigit = true;
                if (digits < 15)
                {
                    if (mantissa == 0 && c == '0')
                    {
                        fraction--;
                        continue;
                    }
                    mantissa = mantissa * 10 + (ulong) (c - '0');
                    digits++;
                    fraction--;
                }
                else
                {
                    return false;
                }
            }
        }

        if (!seenDigit) return false;
        exponent = fraction;

        if (index < span.Length)
        {
            var c = span[index];
            if (c is not ('e' or 'E')) return false;
            index++;
            var negativeExponent = false;
            if (index < span.Length && (span[index] == '+' || span[index] == '-'))
            {
                negativeExponent = span[index] == '-';
                index++;
            }
            if (index >= span.Length) return false;
            var value = 0;
            for (; index < span.Length; index++)
            {
                var digit = span[index] - '0';
                if ((uint) digit > 9 || value > 1000) return false;
                value = value * 10 + digit;
            }
            exponent += negativeExponent ? -value : value;
        }

        if (exponent is < -22 or > 22) return false;
        if (mantissa > 1UL << 53) return false;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool ToBool(in ReadOnlySpan<char> span)
    {
        const ulong leEurt = 0x0065_0075_0072_0074;
        const ulong leSlaf = 0x0073_006C_0061_0066;
        const ulong beEurt = 0x0074_0072_0075_0065;
        const ulong beSlaf = 0x0066_0061_006C_0073;

        // The reader hands over the exact token, so the two literals are the whole common case
        if (span.Length == 4)
        {
            fixed (char* ptr = span)
            {
                var v = Unsafe.ReadUnaligned<ulong>(ptr);
                if (v == (BitConverter.IsLittleEndian ? leEurt : beEurt)) return true;
            }
        }
        else if (span.Length == 5)
        {
            fixed (char* ptr = span)
            {
                var v = Unsafe.ReadUnaligned<ulong>(ptr);
                if (v == (BitConverter.IsLittleEndian ? leSlaf : beSlaf) && span[4] == 'e') return false;
            }
        }
        throw SerdeException.ThrowInvalidParsing<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ToChar(in ReadOnlySpan<char> span) => span[0];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ToShort(in ReadOnlySpan<char> span) =>
        TryParseIntegerUtf16(span, out var value) && value >= short.MinValue && value <= short.MaxValue
            ? (short) value
            : short.TryParse(span, out var result)
                ? result
                : throw SerdeException.ThrowInvalidParsing<short>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ToUshort(in ReadOnlySpan<char> span) =>
        TryParseIntegerUtf16(span, out var value) && value >= ushort.MinValue && value <= ushort.MaxValue
            ? (ushort) value
            : ushort.TryParse(span, out var result)
                ? result
                : throw SerdeException.ThrowInvalidParsing<ushort>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(in ReadOnlySpan<char> span) =>
        TryParseIntegerUtf16(span, out var value) && value >= int.MinValue && value <= int.MaxValue
            ? (int) value
            : int.TryParse(span, out var result)
                ? result
                : throw SerdeException.ThrowInvalidParsing<int>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUint(in ReadOnlySpan<char> span) =>
        TryParseIntegerUtf16(span, out var value) && value >= uint.MinValue && value <= uint.MaxValue
            ? (uint) value
            : uint.TryParse(span, out var result)
                ? result
                : throw SerdeException.ThrowInvalidParsing<uint>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToLong(in ReadOnlySpan<char> span) =>
        TryParseIntegerUtf16(span, out var value)
            ? value
            : long.TryParse(span, out var result)
                ? result
                : throw SerdeException.ThrowInvalidParsing<long>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUlong(in ReadOnlySpan<char> span) =>
        TryParseIntegerUtf16(span, out var value) && value >= 0
            ? (ulong) value
            : ulong.TryParse(span, out var result)
                ? result
                : throw SerdeException.ThrowInvalidParsing<ulong>();

    /// <summary>Parses <c>[+-]?[0-9]+</c> without going through the culture aware number parser.</summary>
    /// <remarks>Falls back to the framework parser on anything else (whitespace, separators, overflow).</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseIntegerUtf16(in ReadOnlySpan<char> span, out long value)
    {
        value = 0;
        if (span.IsEmpty) return false;

        var index = 0;
        var negative = false;
        if (span[0] == '-')
        {
            negative = true;
            index = 1;
        }
        else if (span[0] == '+')
        {
            index = 1;
        }
        if (index == span.Length) return false;

        for (; index < span.Length; index++)
        {
            var digit = span[index] - '0';
            if ((uint) digit > 9) return false;
            // Leave the overflowing values to the framework parser
            if (value > (long.MaxValue - digit) / 10) return false;
            value = value * 10 + digit;
        }
        if (negative) value = -value;
        return true;
    }

    #endregion

    #region UTF8

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ToSbyte(in ReadOnlySpan<byte> span)
    {
        var isNegative = span[0] == 45;
        if (isNegative)
        {
            return (sbyte) -(span.Length switch
            {
                2 => NumberHelper.FromAscii(span[1]),
                3 => NumberHelper.FromAscii(span[1]) * 10 + NumberHelper.FromAscii(span[2]),
                4 => NumberHelper.FromAscii(span[1]) * 100 + NumberHelper.FromAscii(span[2]) * 10 + NumberHelper.FromAscii(span[3]),
                _ => throw SerdeException.ThrowInvalidParsing<sbyte>()
            });
        }
        return (sbyte) (span.Length switch
        {
            1 => NumberHelper.FromAscii(span[0]),
            2 => NumberHelper.FromAscii(span[0]) * 10 + NumberHelper.FromAscii(span[1]),
            3 => NumberHelper.FromAscii(span[0]) * 100 + NumberHelper.FromAscii(span[1]) * 10 + NumberHelper.FromAscii(span[2]),
            _ => throw SerdeException.ThrowInvalidParsing<sbyte>()
        });
    }

    public static byte ToByte(in ReadOnlySpan<byte> span) => (byte) (span.Length switch
    {
        1 => NumberHelper.FromAscii(span[0]),
        2 => NumberHelper.FromAscii(span[0]) * 10 + NumberHelper.FromAscii(span[1]),
        3 => NumberHelper.FromAscii(span[0]) * 100 + NumberHelper.FromAscii(span[1]) * 10 + NumberHelper.FromAscii(span[2]),
        _ => throw SerdeException.ThrowInvalidParsing<byte>()
    });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime ToDateTime(in ReadOnlySpan<byte> span)
    {
        if (!DateTimeHelper.TryParseDateTime(span, out var value))
            throw SerdeException.ThrowInvalidParsing<DateTime>();
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(in ReadOnlySpan<byte> span) => Encoding.UTF8.GetString(span);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ToDecimal(in ReadOnlySpan<byte> span)
    {
        if (!Utf8Parser.TryParse(span, out decimal value, out var consumed) || span.Length != consumed)
        {
            throw SerdeException.ThrowInvalidParsing<decimal>();
        }
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(in ReadOnlySpan<byte> span)
    {
        if (!Utf8Parser.TryParse(span, out double value, out var consumed) || span.Length != consumed)
        {
            throw SerdeException.ThrowInvalidParsing<double>();
        }
        // Utf8Parser also drops the sign bit of "-0" on Mono; restore it here
        return value == 0.0 && span.Length > 0 && span[0] == (byte) '-' ? -0.0 : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToFloat(in ReadOnlySpan<byte> span)
    {
        if (!Utf8Parser.TryParse(span, out float value, out var consumed) || span.Length != consumed)
        {
            throw SerdeException.ThrowInvalidParsing<float>();
        }
        // Utf8Parser also drops the sign bit of "-0" on Mono; restore it here
        return value == 0f && span.Length > 0 && span[0] == (byte) '-' ? -0.0f : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool ToBool(in ReadOnlySpan<byte> span)
    {
        // UTF8 has no byte order issue, so it can simply be read in reverse
        const uint eurt = 0x6575_7274U;
        const uint slaf = 0x736C_6166U;

        fixed (byte* ptr = span)
        {
            var value = Unsafe.ReadUnaligned<uint>(ptr);
            // If the value is exactly true, there is no need to check the length
            if (value == eurt) return true;
            // false requires an extra check of the last character, e, so throw directly when the length is wrong
            if (span.Length == 5 && value == slaf && span[4] == (byte) 'e') return false;
            throw SerdeException.ThrowInvalidParsing<bool>();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ToChar(in ReadOnlySpan<byte> span)
    {
        if (span.Length == 1)
        {
            return (char) span[0];
        }

        if (span[0] == (byte) '\\')
        {
            switch (span[1])
            {
                case (byte) '"':
                    return '"';
                case (byte) '\\':
                    return '\\';
                case (byte) '/':
                    return '/';
                case (byte) 'b':
                    return '\b';
                case (byte) 'f':
                    return '\f';
                case (byte) 'n':
                    return '\n';
                case (byte) 'r':
                    return '\r';
                case (byte) 't':
                    return '\t';
                case (byte) 'u':
                case (byte) 'x':
                    if (Utf8Parser.TryParse(span.Slice(2, 4), out int value, out _, 'X'))
                    {
                        return (char) value;
                    }
                    throw SerdeException.ThrowInvalidEscapedChar((char) span[1]);
            }
        }

        Span<char> charSpan = stackalloc char[1];
        Encoding.UTF8.GetChars(span, charSpan);
        return charSpan[0];
    }

    public static short ToShort(in ReadOnlySpan<byte> span)
    {
        var isNegative = (char) span[0] == '-';
        short result;
        if (isNegative)
        {
            result = (short) (span[1] - 48);
            for (var i = 2; i < span.Length; i++)
            {
                result *= 10;
                result += (short) (span[i] - 48);
            }

            return (short) -result;
        }

        result = (short) (span[0] - 48);
        for (var i = 1; i < span.Length; i++)
        {
            result *= 10;
            result += (short) (span[i] - 48);
        }
        return result;
    }

    public static ushort ToUshort(in ReadOnlySpan<byte> span)
    {
        var result = (ushort) (span[0] - 48);
        for (var i = 1; i < span.Length; i++)
        {
            result *= 10;
            result += (ushort) (span[i] - 48);
        }
        return result;
    }

    public static int ToInt(in ReadOnlySpan<byte> span)
    {
        var isNegative = (char) span[0] == '-';
        int result;
        if (isNegative)
        {
            result = span[1] - 48;
            for (var i = 2; i < span.Length; i++)
            {
                result *= 10;
                result += span[i] - 48;
            }

            return -result;
        }

        result = span[0] - 48;
        for (var i = 1; i < span.Length; i++)
        {
            result *= 10;
            result += span[i] - 48;
        }
        return result;
    }

    public static uint ToUint(in ReadOnlySpan<byte> span)
    {
        var result = span[0] - 48U;
        for (var i = 1; i < span.Length; i++)
        {
            result *= 10;
            result += span[i] - 48U;
        }
        return result;
    }

    public static long ToLong(in ReadOnlySpan<byte> span)
    {
        var isNegative = (char) span[0] == '-';
        long result;
        if (isNegative)
        {
            result = span[1] - 48L;
            for (var i = 2; i < span.Length; i++)
            {
                result *= 10;
                result += span[i] - 48L;
            }

            return -result;
        }

        result = span[0] - 48L;
        for (var i = 1; i < span.Length; i++)
        {
            result *= 10;
            result += span[i] - 48L;
        }
        return result;
    }

    public static ulong ToUlong(in ReadOnlySpan<byte> span)
    {
        var result = span[0] - 48UL;
        for (var i = 1; i < span.Length; i++)
        {
            result *= 10;
            result += span[i] - 48U;
        }
        return result;
    }

    #endregion
}