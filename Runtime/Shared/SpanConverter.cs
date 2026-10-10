/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace PeroTools2.Serde
{
    /// <summary>
    /// UTF8/UTF16 基本类型转换器
    /// </summary>
    [PublicAPI]
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
        public static sbyte ToSbyte(in ReadOnlySpan<char> span) => sbyte.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<sbyte>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByte(in ReadOnlySpan<char> span) => byte.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<byte>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ToDateTime(in ReadOnlySpan<char> span) => DateTime.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<DateTime>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(in ReadOnlySpan<char> span) => new string(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ToDecimal(in ReadOnlySpan<char> span) => decimal.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<decimal>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(in ReadOnlySpan<char> span)
        {
            if (!double.TryParse(span, out var result))
            {
                throw SerdeException.ThrowInvalidParsing<double>();
            }
            // Unity 的 Mono 上 double.TryParse("-0") 会丢掉负零的符号位, 这里补回来(与 CoreCLR 一致)
            return result == 0.0 && span.Length > 0 && span[0] == '-' ? -0.0 : result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(in ReadOnlySpan<char> span)
        {
            if (!float.TryParse(span, out var result))
            {
                throw SerdeException.ThrowInvalidParsing<float>();
            }
            // Unity 的 Mono 上 float.TryParse("-0") 会丢掉负零的符号位, 这里补回来(与 CoreCLR 一致)
            return result == 0f && span.Length > 0 && span[0] == '-' ? -0.0f : result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ToBool(in ReadOnlySpan<char> span) => bool.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<bool>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToChar(in ReadOnlySpan<char> span) => span[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ToShort(in ReadOnlySpan<char> span) => short.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<short>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUshort(in ReadOnlySpan<char> span) => ushort.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<ushort>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(in ReadOnlySpan<char> span) => int.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<int>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUint(in ReadOnlySpan<char> span) => uint.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<uint>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToLong(in ReadOnlySpan<char> span) => long.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<long>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToUlong(in ReadOnlySpan<char> span) => ulong.TryParse(span, out var result) ? result : throw SerdeException.ThrowInvalidParsing<ulong>();

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
                throw SerdeException.ThrowInvalidParsing<bool>();
            }
            // Utf8Parser 在 Mono 上同样会丢掉 "-0" 的符号位, 这里补回来
            return value == 0.0 && span.Length > 0 && span[0] == (byte) '-' ? -0.0 : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(in ReadOnlySpan<byte> span)
        {
            if (!Utf8Parser.TryParse(span, out float value, out var consumed) || span.Length != consumed)
            {
                throw SerdeException.ThrowInvalidParsing<float>();
            }
            // Utf8Parser 在 Mono 上同样会丢掉 "-0" 的符号位, 这里补回来
            return value == 0f && span.Length > 0 && span[0] == (byte) '-' ? -0.0f : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool ToBool(in ReadOnlySpan<byte> span)
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
}