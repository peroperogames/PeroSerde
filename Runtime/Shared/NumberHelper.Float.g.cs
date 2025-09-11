using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde
{
    static partial class NumberHelper {
        /// <summary>将一个float值格式化为utf16字符串.</summary>
        /// <remarks>
        /// 必须确保dest的空间足够, 最大需要<see cref="max_float_string_length"/>个字节
        /// </remarks>
        internal static unsafe int ToUtf16StringUnsafe(float value, char* dest)
        {
            // TODO 特殊值的处理？需要提供几种方式
            if (IsSpecialValue(value))
            {
                throw SerdeException.ThrowInvalidFloatPoint();
            }
            var start = dest;
            // 按需写入负号
            if (value < 0)
            {
                *dest = negative_sign;
                dest++;
                value = -value;
            }
            // 获取浮点数的指数部分
            var exponent = GetExponent(value);
            // 整数部分的长度. 如果整数位数超过最大位数或为负数, 需要使用科学计数法, 此时整数长度为1
            var lengthOfInteger = exponent >= 0 && exponent < s_MaxFloatDigitalLength ? exponent + 1 : 1;
            // 小数部分的长度 + 1(用于四舍五入)
            var lengthOfFractional = s_MaxFloatDigitalLength - lengthOfInteger;
            // exponent此时代表科学计数法的指数部分. 如果为0代表不需要使用科学计数法.
            if (exponent < s_MaxFloatDigitalLength && exponent > -max_floating_zero_length)
            {
                exponent = 0;
            }
            // 将浮点数的有效部分转换为整数来处理: 123.45678 => 12345678
            var mantissa = (ulong) Pow(value, lengthOfFractional - exponent);

            {
                ulong temp;
                // 对最后一位进行四舍五入
                if (lengthOfFractional >= 1)
                {
                    temp = mantissa / 10;
                    if (mantissa - temp * 10 >= 5)
                        mantissa = temp + 1;
                    else
                        mantissa = temp;
                    lengthOfFractional -= 1;
                }
                // 去掉尾随0
                while (lengthOfFractional >= 4 && mantissa == (temp = mantissa / 10000) * 10000)
                {
                    mantissa = temp;
                    lengthOfFractional -= 4;
                }
                while (lengthOfFractional > 0 && mantissa == (temp = mantissa / 10) * 10)
                {
                    mantissa = temp;
                    lengthOfFractional -= 1;
                }
            }
            // 写入数字
            if (lengthOfFractional == 0) // 没有小数位的情况, 直接按照整数写入.
            {
                var charsWritten = ToUtf16StringUnsafe(mantissa, dest);
                if (charsWritten > lengthOfInteger && exponent != 0)
                {
                    exponent += 1;
                    charsWritten -= 1;
                }
                dest += charsWritten;
            }
            // 如果整数部分是 0，则先写入整数部分
            else if (value < 1)
            {
                *dest = '0';
                *(dest + 1) = dot_sign;
                dest += 2;
                // 整数是零，所以不需要再移动小数点，直接写入小数部分
                var fractionalExponent = GetExponent(mantissa);
                var firstNumIndex = lengthOfFractional - (fractionalExponent + 1);
                for (int i = 0; i < firstNumIndex; i++)
                {
                    dest[i] = '0';
                }
                dest += firstNumIndex;
                ToUtf16StringUnsafe(mantissa, dest);
                dest += fractionalExponent + 1;
            }
            else
            {
                // 有小数位的部分, 先按照整数写入, 再写入小数点
                ToUtf16StringUnsafe(mantissa, dest);
                dest += lengthOfInteger;
                // 小数部分整体向后错一位, 来放小数点
                for (var i = lengthOfFractional; i > 0;)
                {
                    dest[i] = dest[--i];
                }
                *dest = dot_sign;
                dest += lengthOfFractional + 1;
            }
            // 如果需要科学计数法, 写入科学计数法部分
            if (exponent != 0)
            {
                dest[0] = exponent_sign;
                if (exponent > 0) {
                    dest[1] = positive_sign;
                    dest += 2;
                    dest += ToUtf16StringUnsafe((ulong) exponent, dest);
                } else {
                    dest[1] = negative_sign;
                    dest += 2;
                    dest += ToUtf16StringUnsafe((ulong) (-exponent), dest);
                }
            }
            return (int) (dest - start);
        }
        /// <summary>将一个float值格式化为utf8字符串.</summary>
        /// <remarks>
        /// 必须确保dest的空间足够, 最大需要<see cref="max_float_string_length"/>个字节
        /// </remarks>
        internal static unsafe int ToUtf8StringUnsafe(float value, byte* dest)
        {
            // TODO 特殊值的处理？需要提供几种方式
            if (IsSpecialValue(value))
            {
                throw SerdeException.ThrowInvalidFloatPoint();
            }
            var start = dest;
            // 按需写入负号
            if (value < 0)
            {
                *dest = (byte) negative_sign;
                dest++;
                value = -value;
            }
            // 获取浮点数的指数部分
            var exponent = GetExponent(value);
            // 整数部分的长度. 如果整数位数超过最大位数或为负数, 需要使用科学计数法, 此时整数长度为1
            var lengthOfInteger = exponent >= 0 && exponent < s_MaxFloatDigitalLength ? exponent + 1 : 1;
            // 小数部分的长度 + 1(用于四舍五入)
            var lengthOfFractional = s_MaxFloatDigitalLength - lengthOfInteger;
            // exponent此时代表科学计数法的指数部分. 如果为0代表不需要使用科学计数法.
            if (exponent < s_MaxFloatDigitalLength && exponent > -max_floating_zero_length)
            {
                exponent = 0;
            }
            // 将浮点数的有效部分转换为整数来处理: 123.45678 => 12345678
            var mantissa = (ulong) Pow(value, lengthOfFractional - exponent);

            {
                ulong temp;
                // 对最后一位进行四舍五入
                if (lengthOfFractional >= 1)
                {
                    temp = mantissa / 10;
                    if (mantissa - temp * 10 >= 5)
                        mantissa = temp + 1;
                    else
                        mantissa = temp;
                    lengthOfFractional -= 1;
                }
                // 去掉尾随0
                while (lengthOfFractional >= 4 && mantissa == (temp = mantissa / 10000) * 10000)
                {
                    mantissa = temp;
                    lengthOfFractional -= 4;
                }
                while (lengthOfFractional > 0 && mantissa == (temp = mantissa / 10) * 10)
                {
                    mantissa = temp;
                    lengthOfFractional -= 1;
                }
            }
            // 写入数字
            if (lengthOfFractional == 0) // 没有小数位的情况, 直接按照整数写入.
            {
                var bytesWritten = ToUtf8StringUnsafe(mantissa, dest);
                if (bytesWritten > lengthOfInteger && exponent != 0)
                {
                    exponent += 1;
                    bytesWritten -= 1;
                }
                dest += bytesWritten;
            }
            // 如果整数部分是 0，则先写入整数部分
            else if (value < 1)
            {
                *dest = (byte) '0';
                *(dest + 1) = (byte) dot_sign;
                dest += 2;
                // 整数是零，所以不需要再移动小数点，直接写入小数部分
                var fractionalExponent = GetExponent(mantissa);
                var firstNumIndex = lengthOfFractional - (fractionalExponent + 1);
                for (int i = 0; i < firstNumIndex; i++)
                {
                    dest[i] = (byte) '0';
                }
                dest += firstNumIndex;
                ToUtf8StringUnsafe(mantissa, dest);
                dest += fractionalExponent + 1;
            }
            else
            {
                // 有小数位的部分, 先按照整数写入, 再写入小数点
                ToUtf8StringUnsafe(mantissa, dest);
                dest += lengthOfInteger;
                // 小数部分整体向后错一位, 来放小数点
                for (var i = lengthOfFractional; i > 0;)
                {
                    dest[i] = dest[--i];
                }
                *dest = (byte) dot_sign;
                dest += lengthOfFractional + 1;
            }
            // 如果需要科学计数法, 写入科学计数法部分
            if (exponent != 0)
            {
                dest[0] = (byte) exponent_sign;
                if (exponent > 0) {
                    dest[1] = (byte) positive_sign;
                    dest += 2;
                    dest += ToUtf8StringUnsafe((ulong) exponent, dest);
                } else {
                    dest[1] = (byte) negative_sign;
                    dest += 2;
                    dest += ToUtf8StringUnsafe((ulong) (-exponent), dest);
                }
            }
            return (int) (dest - start);
        }
        /// <summary>将一个double值格式化为utf16字符串.</summary>
        /// <remarks>
        /// 必须确保dest的空间足够, 最大需要<see cref="max_double_string_length"/>个字节
        /// </remarks>
        internal static unsafe int ToUtf16StringUnsafe(double value, char* dest)
        {
            // TODO 特殊值的处理？需要提供几种方式
            if (IsSpecialValue(value))
            {
                throw SerdeException.ThrowInvalidFloatPoint();
            }
            var start = dest;
            // 按需写入负号
            if (value < 0)
            {
                *dest = negative_sign;
                dest++;
                value = -value;
            }
            // 获取浮点数的指数部分
            var exponent = GetExponent(value);
            // 整数部分的长度. 如果整数位数超过最大位数或为负数, 需要使用科学计数法, 此时整数长度为1
            var lengthOfInteger = exponent >= 0 && exponent < s_MaxDoubleDigitalLength ? exponent + 1 : 1;
            // 小数部分的长度 + 1(用于四舍五入)
            var lengthOfFractional = s_MaxDoubleDigitalLength - lengthOfInteger;
            // exponent此时代表科学计数法的指数部分. 如果为0代表不需要使用科学计数法.
            if (exponent < s_MaxDoubleDigitalLength && exponent > -max_floating_zero_length)
            {
                exponent = 0;
            }
            // 将浮点数的有效部分转换为整数来处理: 123.45678 => 12345678
            var mantissa = (ulong) Pow(value, lengthOfFractional - exponent);

            {
                ulong temp;
                // 对最后一位进行四舍五入
                if (lengthOfFractional >= 1)
                {
                    temp = mantissa / 10;
                    if (mantissa - temp * 10 >= 5)
                        mantissa = temp + 1;
                    else
                        mantissa = temp;
                    lengthOfFractional -= 1;
                }
                // 去掉尾随0
                while (lengthOfFractional >= 4 && mantissa == (temp = mantissa / 10000) * 10000)
                {
                    mantissa = temp;
                    lengthOfFractional -= 4;
                }
                while (lengthOfFractional > 0 && mantissa == (temp = mantissa / 10) * 10)
                {
                    mantissa = temp;
                    lengthOfFractional -= 1;
                }
            }
            // 写入数字
            if (lengthOfFractional == 0) // 没有小数位的情况, 直接按照整数写入.
            {
                var charsWritten = ToUtf16StringUnsafe(mantissa, dest);
                if (charsWritten > lengthOfInteger && exponent != 0)
                {
                    exponent += 1;
                    charsWritten -= 1;
                }
                dest += charsWritten;
            }
            // 如果整数部分是 0，则先写入整数部分
            else if (value < 1)
            {
                *dest = '0';
                *(dest + 1) = dot_sign;
                dest += 2;
                // 整数是零，所以不需要再移动小数点，直接写入小数部分
                var fractionalExponent = GetExponent(mantissa);
                var firstNumIndex = lengthOfFractional - (fractionalExponent + 1);
                for (int i = 0; i < firstNumIndex; i++)
                {
                    dest[i] = '0';
                }
                dest += firstNumIndex;
                ToUtf16StringUnsafe(mantissa, dest);
                dest += fractionalExponent + 1;
            }
            else
            {
                // 有小数位的部分, 先按照整数写入, 再写入小数点
                ToUtf16StringUnsafe(mantissa, dest);
                dest += lengthOfInteger;
                // 小数部分整体向后错一位, 来放小数点
                for (var i = lengthOfFractional; i > 0;)
                {
                    dest[i] = dest[--i];
                }
                *dest = dot_sign;
                dest += lengthOfFractional + 1;
            }
            // 如果需要科学计数法, 写入科学计数法部分
            if (exponent != 0)
            {
                dest[0] = exponent_sign;
                if (exponent > 0) {
                    dest[1] = positive_sign;
                    dest += 2;
                    dest += ToUtf16StringUnsafe((ulong) exponent, dest);
                } else {
                    dest[1] = negative_sign;
                    dest += 2;
                    dest += ToUtf16StringUnsafe((ulong) (-exponent), dest);
                }
            }
            return (int) (dest - start);
        }
        /// <summary>将一个double值格式化为utf8字符串.</summary>
        /// <remarks>
        /// 必须确保dest的空间足够, 最大需要<see cref="max_double_string_length"/>个字节
        /// </remarks>
        internal static unsafe int ToUtf8StringUnsafe(double value, byte* dest)
        {
            // TODO 特殊值的处理？需要提供几种方式
            if (IsSpecialValue(value))
            {
                throw SerdeException.ThrowInvalidFloatPoint();
            }
            var start = dest;
            // 按需写入负号
            if (value < 0)
            {
                *dest = (byte) negative_sign;
                dest++;
                value = -value;
            }
            // 获取浮点数的指数部分
            var exponent = GetExponent(value);
            // 整数部分的长度. 如果整数位数超过最大位数或为负数, 需要使用科学计数法, 此时整数长度为1
            var lengthOfInteger = exponent >= 0 && exponent < s_MaxDoubleDigitalLength ? exponent + 1 : 1;
            // 小数部分的长度 + 1(用于四舍五入)
            var lengthOfFractional = s_MaxDoubleDigitalLength - lengthOfInteger;
            // exponent此时代表科学计数法的指数部分. 如果为0代表不需要使用科学计数法.
            if (exponent < s_MaxDoubleDigitalLength && exponent > -max_floating_zero_length)
            {
                exponent = 0;
            }
            // 将浮点数的有效部分转换为整数来处理: 123.45678 => 12345678
            var mantissa = (ulong) Pow(value, lengthOfFractional - exponent);

            {
                ulong temp;
                // 对最后一位进行四舍五入
                if (lengthOfFractional >= 1)
                {
                    temp = mantissa / 10;
                    if (mantissa - temp * 10 >= 5)
                        mantissa = temp + 1;
                    else
                        mantissa = temp;
                    lengthOfFractional -= 1;
                }
                // 去掉尾随0
                while (lengthOfFractional >= 4 && mantissa == (temp = mantissa / 10000) * 10000)
                {
                    mantissa = temp;
                    lengthOfFractional -= 4;
                }
                while (lengthOfFractional > 0 && mantissa == (temp = mantissa / 10) * 10)
                {
                    mantissa = temp;
                    lengthOfFractional -= 1;
                }
            }
            // 写入数字
            if (lengthOfFractional == 0) // 没有小数位的情况, 直接按照整数写入.
            {
                var bytesWritten = ToUtf8StringUnsafe(mantissa, dest);
                if (bytesWritten > lengthOfInteger && exponent != 0)
                {
                    exponent += 1;
                    bytesWritten -= 1;
                }
                dest += bytesWritten;
            }
            // 如果整数部分是 0，则先写入整数部分
            else if (value < 1)
            {
                *dest = (byte) '0';
                *(dest + 1) = (byte) dot_sign;
                dest += 2;
                // 整数是零，所以不需要再移动小数点，直接写入小数部分
                var fractionalExponent = GetExponent(mantissa);
                var firstNumIndex = lengthOfFractional - (fractionalExponent + 1);
                for (int i = 0; i < firstNumIndex; i++)
                {
                    dest[i] = (byte) '0';
                }
                dest += firstNumIndex;
                ToUtf8StringUnsafe(mantissa, dest);
                dest += fractionalExponent + 1;
            }
            else
            {
                // 有小数位的部分, 先按照整数写入, 再写入小数点
                ToUtf8StringUnsafe(mantissa, dest);
                dest += lengthOfInteger;
                // 小数部分整体向后错一位, 来放小数点
                for (var i = lengthOfFractional; i > 0;)
                {
                    dest[i] = dest[--i];
                }
                *dest = (byte) dot_sign;
                dest += lengthOfFractional + 1;
            }
            // 如果需要科学计数法, 写入科学计数法部分
            if (exponent != 0)
            {
                dest[0] = (byte) exponent_sign;
                if (exponent > 0) {
                    dest[1] = (byte) positive_sign;
                    dest += 2;
                    dest += ToUtf8StringUnsafe((ulong) exponent, dest);
                } else {
                    dest[1] = (byte) negative_sign;
                    dest += 2;
                    dest += ToUtf8StringUnsafe((ulong) (-exponent), dest);
                }
            }
            return (int) (dest - start);
        }
    }
}