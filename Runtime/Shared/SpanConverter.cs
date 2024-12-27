/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde
{
    /// <summary>
    /// UTF8/UTF16 基本类型转换器
    /// </summary>
    public static class SpanConverter
    {
        #region Null

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool IsNull(in ReadOnlySpan<byte> span)
        {
            // Utf8 不需要考虑字节序问题
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
            // 小端
            const ulong le_llun = 0x006C006C0075006E;
            // 大端
            const ulong be_llun = 0x6C006C0075006E00;

            if (span.Length != 4)
            {
                return false;
            }
            fixed (char* ptr = span)
            {
                var value = Unsafe.ReadUnaligned<ulong>(ptr);
                return BitConverter.IsLittleEndian ? value == le_llun : value == be_llun;
            }
        }

        #endregion

        #region UTF16

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static sbyte ToSbyte(in ReadOnlySpan<char> span) => sbyte.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte ToByte(in ReadOnlySpan<char> span) => byte.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static DateTime ToDateTime(in ReadOnlySpan<char> span) => DateTime.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ToString(in ReadOnlySpan<char> span) => new string(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static decimal ToDecimal(in ReadOnlySpan<char> span) => decimal.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double ToDouble(in ReadOnlySpan<char> span) => double.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float ToFloat(in ReadOnlySpan<char> span) => float.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool ToBool(in ReadOnlySpan<char> span) => bool.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char ToChar(in ReadOnlySpan<char> span) => span[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static short ToShort(in ReadOnlySpan<char> span) => short.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ushort ToUshort(in ReadOnlySpan<char> span) => ushort.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ToInt(in ReadOnlySpan<char> span) => int.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint ToUint(in ReadOnlySpan<char> span) => ushort.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long ToLong(in ReadOnlySpan<char> span) => long.Parse(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong ToUlong(in ReadOnlySpan<char> span) => ulong.Parse(span);

        #endregion

        #region UTF8

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static sbyte ToSbyte(in ReadOnlySpan<byte> span)
        {
            var isNegative = span[0] == 45;
            if (isNegative)
            {
                return (sbyte) (-span.Length switch
                {
                    2 => span[0],
                    3 => NumberHelper.FromAscii(span[0]) * 10 + NumberHelper.FromAscii(span[1]),
                    4 => NumberHelper.FromAscii(span[0]) * 100 + NumberHelper.FromAscii(span[1]) * 10 + NumberHelper.FromAscii(span[2]),
                    _ => throw SerdeException.ThrowInvalidParsing()
                });
            }
            return (sbyte) (span.Length switch
            {
                1 => span[0],
                2 => NumberHelper.FromAscii(span[0]) * 10 + NumberHelper.FromAscii(span[1]),
                3 => NumberHelper.FromAscii(span[0]) * 100 + NumberHelper.FromAscii(span[1]) * 10 + NumberHelper.FromAscii(span[2]),
                _ => throw SerdeException.ThrowInvalidParsing()
            });
        }

        internal static byte ToByte(in ReadOnlySpan<byte> span) => (byte) (span.Length switch
        {
            1 => span[0],
            2 => NumberHelper.FromAscii(span[0]) * 10 + NumberHelper.FromAscii(span[1]),
            3 => NumberHelper.FromAscii(span[0]) * 100 + NumberHelper.FromAscii(span[1]) * 10 + NumberHelper.FromAscii(span[2]),
            _ => throw SerdeException.ThrowInvalidParsing()
        });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static DateTime ToDateTime(in ReadOnlySpan<byte> span)
        {
            if (!DateTimeHelper.TryParseDateTime(span, out var value))
                throw SerdeException.ThrowInvalidParsing();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string ToString(in ReadOnlySpan<byte> span) => Encoding.UTF8.GetString(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static decimal ToDecimal(in ReadOnlySpan<byte> span)
        {
            if (!Utf8Parser.TryParse(span, out decimal value, out var consumed) || span.Length != consumed)
            {
                throw SerdeException.ThrowInvalidParsing();
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double ToDouble(in ReadOnlySpan<byte> span)
        {
            if (!Utf8Parser.TryParse(span, out double value, out var consumed) || span.Length != consumed)
            {
                throw SerdeException.ThrowInvalidParsing();
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float ToFloat(in ReadOnlySpan<byte> span)
        {
            if (!Utf8Parser.TryParse(span, out float value, out var consumed) || span.Length != consumed)
            {
                throw SerdeException.ThrowInvalidParsing();
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe bool ToBool(in ReadOnlySpan<byte> span)
        {
            // UTF8 不存在字节序问题，因此直接反过来读就行
            const uint eurt = 0x65757274U;
            const uint slaf = 0x736C6166U;

            fixed (byte* ptr = span)
            {
                var value = Unsafe.ReadUnaligned<uint>(ptr);
                // 如果就是 true，那么就不用判断长度了
                if (value == eurt) return true;
                // false 需要额外判断最后一位，即 e，因此要在长度错误的时候直接抛出异常
                if (span.Length == 5 && value == slaf && span[4] == (byte) 'e') return false;
                throw SerdeException.ThrowInvalidParsing();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char ToChar(in ReadOnlySpan<byte> span)
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

        internal static short ToShort(in ReadOnlySpan<byte> span)
        {
#if NET8_0_OR_GREATER
            return short.Parse(span);
#else
            if (span.IsEmpty)
                throw SerdeException.ThrowInvalidParsing();

            var isNegative = span[0] == (byte) '-';
            var startIndex = isNegative ? 1 : 0;
            short result = 0;

            for (var i = startIndex; i < span.Length; i++)
            {
                var digit = (byte) (span[i] - '0');
                if (digit > 9)
                    throw SerdeException.ThrowInvalidParsing();

                result = (short) (result * 10 + digit);
            }

            return isNegative ? (short) -result : result;
#endif
        }

        internal static ushort ToUshort(in ReadOnlySpan<byte> span)
        {
#if NET8_0_OR_GREATER
            return ushort.Parse(span);
#else
            if (span.IsEmpty)
                throw SerdeException.ThrowInvalidParsing();

            ushort result = 0;

            for (var i = 0; i < span.Length; i++)
            {
                var digit = (byte) (span[i] - '0');
                if (digit > 9)
                    throw SerdeException.ThrowInvalidParsing();

                if (result > ushort.MaxValue / 10 || (result == ushort.MaxValue / 10 && digit > ushort.MaxValue % 10))
                    throw SerdeException.ThrowInvalidParsing();

                result = (ushort) (result * 10 + digit);
            }

            return result;
#endif
        }

        internal static int ToInt(in ReadOnlySpan<byte> span)
        {
#if NET8_0_OR_GREATER
            return int.Parse(span);
#else
            var isNegative = (char) span[0] == '-';
            var result = 0;
            if (isNegative)
            {
                for (var i = 1; i < span.Length; i++)
                {
                    result += span[i] - 48;
                    result *= 10;
                }

                // 除掉多乘的一位数
                return -(result / 10);
            }

            for (var i = 0; i < span.Length; i++)
            {
                result += span[i] - 48;
                result *= 10;
            }
            // 除掉多乘的一位数
            return result / 10;
#endif
        }

        internal static uint ToUint(in ReadOnlySpan<byte> span)
        {
#if NET8_0_OR_GREATER
            return uint.Parse(span);
#else
            var result = 0U;
            for (var i = 0; i < span.Length; i++)
            {
                result += span[i] - 48U;
                result *= 10;
            }
            // 除掉多乘的一位数
            return result / 10;
#endif
        }

        internal static long ToLong(in ReadOnlySpan<byte> span)
        {
#if NET8_0_OR_GREATER
            return long.Parse(span);
#else
            var isNegative = (char) span[0] == '-';
            var result = 0L;
            if (isNegative)
            {
                for (var i = 1; i < span.Length; i++)
                {
                    result += span[i] - 48L;
                    result *= 10;
                }

                // 除掉多乘的一位数
                return -(result / 10);
            }

            for (var i = 0; i < span.Length; i++)
            {
                result += span[i] - 48L;
                result *= 10;
            }
            // 除掉多乘的一位数
            return result / 10;
#endif
        }

        internal static ulong ToUlong(in ReadOnlySpan<byte> span)
        {
#if NET8_0_OR_GREATER
            return uint.Parse(span);
#else
            var result = 0U;
            for (var i = 0; i < span.Length; i++)
            {
                result += span[i] - 48U;
                result *= 10;
            }
            // 除掉多乘的一位数
            return result / 10;
#endif
        }

        #endregion
    }
}