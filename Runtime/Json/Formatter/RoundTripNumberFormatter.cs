/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/10/10
 */

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 浮点数往返写出的公共实现。
    /// </summary>
    internal static class RoundTripNumberHelper
    {
        // "R" 格式的最大字符数。
        internal const int max_float_size = 16;
        internal const int max_double_size = 25;

        internal static unsafe int Format(float value, char* dest)
        {
            if (NumberHelper.IsSpecialValue(value))
            {
                throw SerdeException.ThrowInvalidFloatPoint();
            }
            // 负零自己写: 不依赖运行时 "R" 格式化对负零的处理 (默认写法用 value < 0 判符号, -0.0 会漏)
            if (Unsafe.As<float, uint>(ref value) == 0x80000000U)
            {
                dest[0] = '-';
                dest[1] = '0';
                return 2;
            }
            if (!value.TryFormat(new Span<char>(dest, max_float_size), out var written, "R", CultureInfo.InvariantCulture))
            {
                throw SerdeException.ThrowInvalidFloatPoint();
            }
            return written;
        }

        internal static unsafe int Format(double value, char* dest)
        {
            if (NumberHelper.IsSpecialValue(value))
            {
                throw SerdeException.ThrowInvalidFloatPoint();
            }
            // 负零自己写: 不依赖运行时 "R" 格式化对负零的处理 (默认写法用 value < 0 判符号, -0.0 会漏)
            if (Unsafe.As<double, ulong>(ref value) == 0x8000000000000000UL)
            {
                dest[0] = '-';
                dest[1] = '0';
                return 2;
            }
            if (!value.TryFormat(new Span<char>(dest, max_double_size), out var written, "R", CultureInfo.InvariantCulture))
            {
                throw SerdeException.ThrowInvalidFloatPoint();
            }
            return written;
        }
    }

    /// <summary>float 的往返格式化器</summary>
    public sealed class RoundTripFloatFormatter : IFixedSerdeJsonFormatter<float>
    {
        public const int max_size = RoundTripNumberHelper.max_float_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(float value, byte* dest) => WriteUtf8(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(float value, char* dest) => RoundTripNumberHelper.Format(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadFloatUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadFloatUtf16(src, out advance);

        // "R" 的输出只含 ASCII 字符, 逐字符截断成字节即可
        internal static unsafe int WriteUtf8(float value, byte* dest)
        {
            var buffer = stackalloc char[max_size];
            var written = RoundTripNumberHelper.Format(value, buffer);
            for (var i = 0; i < written; i++)
            {
                dest[i] = (byte) buffer[i];
            }
            return written;
        }
    }

    /// <summary>double 的往返格式化器</summary>
    public sealed class RoundTripDoubleFormatter : IFixedSerdeJsonFormatter<double>
    {
        public const int max_size = RoundTripNumberHelper.max_double_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(double value, byte* dest) => WriteUtf8(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(double value, char* dest) => RoundTripNumberHelper.Format(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadDoubleUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadDoubleUtf16(src, out advance);

        internal static unsafe int WriteUtf8(double value, byte* dest)
        {
            var buffer = stackalloc char[max_size];
            var written = RoundTripNumberHelper.Format(value, buffer);
            for (var i = 0; i < written; i++)
            {
                dest[i] = (byte) buffer[i];
            }
            return written;
        }
    }

    /// <summary>float? 的往返格式化器</summary>
    public sealed class NullableRoundTripFloatFormatter : IFixedSerdeJsonFormatter<float?>
    {
        public const int max_size = RoundTripNumberHelper.max_float_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(float? value, byte* dest) =>
            value == null ? JsonWriter.WriteNullUtf8(dest) : RoundTripFloatFormatter.WriteUtf8(value.Value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(float? value, char* dest) =>
            value == null ? JsonWriter.WriteNullUtf16(dest) : RoundTripNumberHelper.Format(value.Value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableFloatUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableFloatUtf16(src, out advance);
    }

    /// <summary>double? 的往返格式化器</summary>
    public sealed class NullableRoundTripDoubleFormatter : IFixedSerdeJsonFormatter<double?>
    {
        public const int max_size = RoundTripNumberHelper.max_double_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(double? value, byte* dest) =>
            value == null ? JsonWriter.WriteNullUtf8(dest) : RoundTripDoubleFormatter.WriteUtf8(value.Value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(double? value, char* dest) =>
            value == null ? JsonWriter.WriteNullUtf16(dest) : RoundTripNumberHelper.Format(value.Value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableDoubleUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableDoubleUtf16(src, out advance);
    }
}