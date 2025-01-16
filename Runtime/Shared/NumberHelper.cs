/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/23
 * Description: NumberHelper
 */

using System.Runtime.CompilerServices;

namespace PeroTools2.Serde
{
    /// <summary>
    /// 数字帮助类
    /// </summary>
    internal static partial class NumberHelper
    {
        /// <summary>float值格式化为字符串后的最大长度</summary>
        internal const int max_float_string_length = 24;

        /// <summary>double值格式化为字符串后的最大长度</summary>
        internal const int max_double_string_length = 32;

        /// <summary>decimal值格式化为字符串后的最大长度</summary>
        internal const int max_decimal_string_length = 32;

        /// <summary>long值格式化为字符串后的最大长度</summary>
        internal const int max_int64_numbers_length = 20; // 0 到 18446744073709551615

        /// <summary>ulong值格式化为字符串后的最大长度</summary>
        internal const int max_uint64_numbers_length = 20; // -9223372036854775808 到 9223372036854775807

        private const char positive_sign = '+';
        private const char negative_sign = '-';
        private const char dot_sign = '.';
        private const char exponent_sign = 'E';

        static NumberHelper()
        {
            InitExponents();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte FromAscii(byte value) => value switch
        {
            48 => 0,
            49 => 1,
            50 => 2,
            51 => 3,
            52 => 4,
            53 => 5,
            54 => 6,
            55 => 7,
            56 => 8,
            57 => 9,
            _ => throw SerdeException.ThrowInvalidParsing()
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte FromAscii(char value) => FromAscii((byte) value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte GetAscii(int value) => value switch
        {
            0 => 48,
            1 => 49,
            2 => 50,
            3 => 51,
            4 => 52,
            5 => 53,
            6 => 54,
            7 => 55,
            8 => 56,
            9 => 57,
            _ => throw SerdeException.ThrowInvalidParsing()
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char GetAsciiChar(int value) => value switch
        {
            0 => '0',
            1 => '1',
            2 => '2',
            3 => '3',
            4 => '4',
            5 => '5',
            6 => '6',
            7 => '7',
            8 => '8',
            9 => '9',
            _ => throw SerdeException.ThrowInvalidParsing()
        };
    }
}