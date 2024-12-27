/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/23
 * Description: NumberHelper_Integer
 */

namespace PeroTools2.Serde
{
    static partial class NumberHelper
    {
        /*
         * 以下这些方法在Mono + .NetStandard2.1下比value.TryFormat快3-5倍,
         * 但在CoreCLR + .Net8下实测比value.TryFormat()慢一丢丢,
         * 所以如果之后升级到CoreCLR, 可以直接使用TryFormat代替.
         */

        public static unsafe int ToUtf16StringUnsafe(ulong value, char* dest)
        {
            if (value < 10)
            {
                *dest = (char) ('0' + value);
                return 1;
            }
            var digits = GetIntegerLength(value);
            for (var i = digits; i > 0; i--)
            {
                var temp = '0' + value;
                value /= 10;
                dest[i - 1] = (char) (temp - value * 10);
            }
            return digits;
        }

        public static unsafe int ToUtf16StringUnsafe(long value, char* dest)
        {
            if (value < 0)
            {
                *dest = '-';
                value = unchecked(-value);
                return 1 + ToUtf16StringUnsafe((ulong) value, dest + 1);
            }
            return ToUtf16StringUnsafe((ulong) value, dest);
        }

        public static unsafe int ToUtf8StringUnsafe(ulong value, byte* dest)
        {
            if (value < 10)
            {
                *dest = (byte) ('0' + value);
                return 1;
            }
            var digits = GetIntegerLength(value);
            for (var i = digits; i > 0; i--)
            {
                var temp = '0' + value;
                value /= 10;
                dest[i - 1] = (byte) (temp - value * 10);
            }
            return digits;
        }

        public static unsafe int ToUtf8StringUnsafe(long value, byte* dest)
        {
            if (value < 0)
            {
                *dest = (byte) '-';
                value = unchecked(-value);
                return 1 + ToUtf8StringUnsafe((ulong) value, dest + 1);
            }
            return ToUtf8StringUnsafe((ulong) value, dest);
        }

        /// <summary>获取十进制整数的长度</summary>
        /// <remarks>使用二分法加速</remarks>
        public static int GetIntegerLength(ulong value)
        {
            if (value >= 100000)
            {
                if (value >= 10000000000)
                    if (value >= 1000000000000000)
                        if (value >= 100000000000000000)
                            if (value >= 10000000000000000000) return 20;
                            else if (value >= 1000000000000000000) return 19;
                            else return 18;
                        else if (value >= 10000000000000000) return 17;
                        else return 16;
                    else if (value >= 1000000000000)
                        if (value >= 100000000000000) return 15;
                        else if (value >= 10000000000000) return 14;
                        else return 13;
                    else if (value >= 100000000000) return 12;
                    else return 11;
                if (value >= 10000000)
                    if (value >= 1000000000) return 10;
                    else if (value >= 100000000) return 9;
                    else return 8;
                if (value >= 1000000) return 7;
                return 6;
            }
            if (value >= 100)
                if (value >= 10000) return 5;
                else if (value >= 1000) return 4;
                else return 3;
            if (value >= 10) return 2;
            return 1;
        }
    }
}