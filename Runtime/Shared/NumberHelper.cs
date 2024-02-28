/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/23
 * Description: NumberHelper
 */

namespace PeroTools2.Serde
{
    public static partial class NumberHelper
    {
        /// <summary>float值格式化为字符串后的最大长度</summary>
        public const int max_float_string_length = 24;

        /// <summary>double值格式化为字符串后的最大长度</summary>
        public const int max_double_string_length = 32;

        /// <summary>decimal值格式化为字符串后的最大长度</summary>
        public const int max_decimal_string_length = 32;

        /// <summary>long值格式化为字符串后的最大长度</summary>
        public const int max_int64_numbers_length = 20; // 0 到 18446744073709551615

        /// <summary>ulong值格式化为字符串后的最大长度</summary>
        public const int max_uint64_numbers_length = 20; // -9223372036854775808 到 9223372036854775807

        public const char positive_sign = '+';
        public const char negative_sign = '-';
        public const char dot_sign = '.';
        public const char exponent_sign = 'E';

        static NumberHelper()
        {
            InitExponents();
        }
    }
}