/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/20
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    public static unsafe partial class JsonReader
    {
        private static ReadOnlySpan<byte> ReadNotStringUntilEndUtf8(byte* src, int maxLength, out int advance)
        {
            maxLength += 1;
            advance = 0;
            var span = new ReadOnlySpan<byte>(src, maxLength);
            for (var i = 0; i < maxLength; i++)
            {
                var character = (char) span[i];
                var isWiteSpace = StringHelper.IsWhiteSpace(character);
                // 不包括结尾
                if (!isWiteSpace && character is not ('{' or '}' or '[' or ']' or ','))
                {
                    continue;
                }
                if (isWiteSpace)
                {
                    // 把剩下的读完
                    for (var j = i; i < span.Length; j++)
                    {
                        if (character is not ('{' or '}' or '[' or ']' or ','))
                        {
                            continue;
                        }
                        advance = j;
                        break;
                    }
                }
                else
                {
                    advance = i;
                }
                return span.Slice(0, i);
            }
            throw SerdeException.ThrowInvalidAdvance();
        }

        public static ReadOnlySpan<byte> ReadValueUntilEndUtf8(byte* src, int maxLength, out int advance)
        {
            var span = new ReadOnlySpan<byte>(src, maxLength);
            // The first character is "
            if (span[0] != '"')
            {
                return ReadNotStringUntilEndUtf8(src, maxLength, out advance);
            }
            for (var i = 1; i < maxLength + 1; i++)
            {
                var character = (char) span[i];
                if (character is '"' && (char) span[i - 1] != '\\')
                {
                    // 包括双引号
                    advance = i + 1;
                    // 不包括双引号
                    return span.Slice(1, i - 1);
                }
            }
            throw SerdeException.ThrowInvalidAdvance();
        }

        #region Read Nullable

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte? ReadNullableSbyteUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 5, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte? ReadNullableByteUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 4, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal? ReadNullableDecimalUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_decimal_string_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double? ReadNullableDoubleUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_double_string_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float? ReadNullableFloatUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_float_string_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short? ReadNullableShortUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 6, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? ReadNullableIntUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 11, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long? ReadNullableLongUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_int64_numbers_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort? ReadNullableUshortUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 6, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint? ReadNullableUintUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 10, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong? ReadNullableUlongUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_uint64_numbers_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUlong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char? ReadNullableCharUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 8, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool? ReadNullableBoolUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 5, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToBool(in span);
        }

        #endregion

        #region Read Primitive

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByteUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 3, out advance);
            return SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSbyteUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 4, out advance);
            return SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimalUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_decimal_string_length, out advance);
            return SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDoubleUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_double_string_length, out advance);
            return SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloatUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_float_string_length, out advance);
            return SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 5, out advance);
            return SpanConverter.ToBool(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadCharUtf8(byte* src, out int advance)
        {
            var span = ReadValueUntilEndUtf8(src, 8, out advance);
            return SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShortUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 6, out advance);
            return SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadIntUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 11, out advance);
            return SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLongUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_int64_numbers_length, out advance);
            return SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUshortUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 5, out advance);
            return SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUintUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, 10, out advance);
            return SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUlongUtf8(byte* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf8(src, NumberHelper.max_uint64_numbers_length, out advance);
            return SpanConverter.ToUlong(in span);
        }
    }

    #endregion
}