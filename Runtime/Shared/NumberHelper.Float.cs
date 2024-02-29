/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/02/22
 * Description: NumberHelper
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde
{
    public static partial class NumberHelper
    {
        // 1, 10, 100, 1000, 1e4, 1e5, 1e6,... 1e308
        private static double[] s_PositiveExponents;

        // 1, 0.1, 0.01, 0.001, 1e-4, 1e-5, 1e-6,... 1e-324
        private static double[] s_NegativeExponents;

        // float值格式化后的最大字符长度: 7 + 1(小数点)
        private static readonly int s_MaxFloatDigitalLength = GetIntegerLength(0x7FFFFF) + 1;

        // double值格式化后的最大字符长度: 15 + 1(小数点)
        private static readonly int s_MaxDoubleDigitalLength = GetIntegerLength(0xFFFFFFFFFFFFF) + 1;

        private const int max_floating_zero_length = 5;

        private const double max_positive_double_value = double.MaxValue;
        private const double min_positive_double_value = 4.94065645841246544E-324;

        /// <summary>判断一个float值是否是特殊值: Nan, Inf, -Inf</summary>
        public static bool IsSpecialValue(float value)
        {
            const uint specialValue = 0x7F800000;
            return (Unsafe.As<float, uint>(ref value) & specialValue) == specialValue;
        }

        /// <summary>判断一个double值是否是特殊值: Nan, Inf, -Inf</summary>
        public static bool IsSpecialValue(double value)
        {
            const ulong specialValue = 0x7FF0000000000000;
            return (Unsafe.As<double, ulong>(ref value) & specialValue) == specialValue;
        }

        /// <summary>求浮点数x的y次幂</summary>
        public static double Pow(double x, int y)
        {
            if (y >= 0 && y < s_PositiveExponents.Length)
            {
                return x * s_PositiveExponents[y];
            }
            if (y < 0 && -y < s_PositiveExponents.Length)
            {
                return x / s_PositiveExponents[-y];
            }
            // 对于y极大的情况, 将y减半, 分两次计算
            return Pow(Pow(x, y / 2), y - y / 2);
        }

        private static void InitExponents()
        {
            // 初始化positiveExponents
            s_PositiveExponents = new double[SlowGetLength(max_positive_double_value)];
            for (var i = 0; i < s_PositiveExponents.Length; i++)
            {
                s_PositiveExponents[i] = Math.Pow(10, i);
            }
            // 初始化negativeExponents
            s_NegativeExponents = new double[SlowGetFractionalLength(min_positive_double_value)];
            for (var i = 0; i < s_NegativeExponents.Length; i++)
            {
                s_NegativeExponents[i] = Math.Pow(10, -i);
            }
        }

        /// <summary>获取正浮点数的指数部分</summary>
        /// <remarks>无法处理: 负数, Nan, Inf, -Inf</remarks>
        private static int GetExponent(double value)
        {
            return value switch
            {
                >= 1 => GetPositiveExponent(value),
                > 0 and < 1 => GetNegativeExponent(value),
                0 => 0,
                // 负数, Nan, Inf, -Inf无法处理
                _ => throw new NotSupportedException($"Invalid Float Point Value: {value}")
            };

            // 获取大于1的浮点数的整数部分位数, 也是科学计数法的指数部分
            static int GetPositiveExponent(double v)
            {
                var exponents = s_PositiveExponents;
                var i = 0;
                while (i + 128 < exponents.Length && v >= exponents[i + 128])
                {
                    i += 128;
                }
                while (i + 16 < exponents.Length && v >= exponents[i + 16])
                {
                    i += 16;
                }
                while (i < exponents.Length && v >= exponents[i])
                {
                    i += 1;
                }
                return i - 1;
            }

            // 获取大于0小于1的浮点数的指数部分, 就是 1e-2的-2
            static int GetNegativeExponent(double v)
            {
                var i = 0;
                var exponents = s_NegativeExponents;
                while (i + 128 < exponents.Length && v < exponents[i + 128])
                {
                    i += 128;
                }
                while (i + 16 < exponents.Length && v < exponents[i + 16])
                {
                    i += 16;
                }
                while (i < exponents.Length && v < exponents[i])
                {
                    i += 1;
                }
                return -i;
            }
        }

        private static int SlowGetLength(double value)
        {
            var result = 0;
            while (value >= 1e16)
            {
                value /= 1e16;
                result += 16;
            }
            while (value >= 1)
            {
                value /= 10;
                result += 1;
            }
            return result;
        }

        private static int SlowGetFractionalLength(double value)
        {
            var result = 0;
            while (value < 1e-16)
            {
                value *= 1e16;
                result += 16;
            }
            while (value < 1)
            {
                value *= 10;
                result += 1;
            }
            return result;
        }
    }
}