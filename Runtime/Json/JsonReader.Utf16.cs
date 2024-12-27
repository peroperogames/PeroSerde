/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/20
 */

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    [PublicAPI]
    public static unsafe partial class JsonReader
    {
        private static ReadOnlySpan<char> ReadNotStringUntilEndUtf16(char* src, int maxLength, out int advance)
        {
            advance = 0;
            maxLength += 1;
            var span = new ReadOnlySpan<char>(src, maxLength);
            for (var i = 0; i < maxLength; i++)
            {
                var character = span[i];
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
                        if (StringHelper.IsWhiteSpace(character))
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

        internal static ReadOnlySpan<char> ReadValueUntilEndUtf16(char* src, int maxLength, out int advance)
        {
            var span = new ReadOnlySpan<char>(src, maxLength);
            if (span[0] != '"')
            {
                return ReadNotStringUntilEndUtf16(src, maxLength, out advance);
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = span[i];
                if (character is '"' && span[i - 1] != '\\')
                {
                    // 包括双引号
                    advance = i + 1;
                    // 不包括双引号
                    return span.Slice(1, i - 1);
                }
            }
            throw SerdeException.ThrowInvalidAdvance();
        }

        #region ReadNullable

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static sbyte? ReadNullableSbyteUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 5, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte? ReadNullableByteUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 4, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static decimal? ReadNullableDecimalUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_decimal_string_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double? ReadNullableDoubleUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_double_string_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float? ReadNullableFloatUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_float_string_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static short? ReadNullableShortUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 6, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int? ReadNullableIntUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 11, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long? ReadNullableLongUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_int64_numbers_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ushort? ReadNullableUshortUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 6, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint? ReadNullableUintUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 10, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong? ReadNullableUlongUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_uint64_numbers_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUlong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char? ReadNullableCharUtf16(char* src, out int advance)
        {
            var span = ReadValueUntilEndUtf16(src, 8, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool? ReadNullableBoolUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 5, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToBool(in span);
        }

        #endregion

        #region Read Primitive

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte ReadByteUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 3, out advance);
            return SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static sbyte ReadSbyteUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 4, out advance);
            return SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static decimal ReadDecimalUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_decimal_string_length, out advance);
            return SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double ReadDoubleUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_double_string_length, out advance);
            return SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float ReadFloatUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_float_string_length, out advance);
            return SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool ReadBoolUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 5, out advance);
            return SpanConverter.ToBool(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static char ReadCharUtf16(char* src, out int advance)
        {
            var span = ReadValueUntilEndUtf16(src, 8, out advance);
            return SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static short ReadShortUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 6, out advance);
            return SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ReadIntUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 11, out advance);
            return SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long ReadLongUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_int64_numbers_length, out advance);
            return SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ushort ReadUshortUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 5, out advance);
            return SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint ReadUintUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, 10, out advance);
            return SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong ReadUlongUtf16(char* src, out int advance)
        {
            var span = ReadNotStringUntilEndUtf16(src, NumberHelper.max_uint64_numbers_length, out advance);
            return SpanConverter.ToUlong(in span);
        }
    }

    #endregion
}