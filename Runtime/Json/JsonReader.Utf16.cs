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
        private static ReadOnlySpan<char> ReadPrimitiveUntilEndUtf16(char* src, int maxLength, out int advance)
        {
            advance = 0;
            maxLength += 1;
            var span = new ReadOnlySpan<char>(src, maxLength);
            for (var i = 0; i < maxLength; i++)
            {
                var character = span[i];
                var isWhiteSpace = StringHelper.IsWhiteSpace(character);
                // 不包括结尾
                if (!isWhiteSpace && character is not ('}' or ']' or ','))
                {
                    continue;
                }
                advance = i;
                // 把剩下的读完
                for (var j = i; j < span.Length; j++)
                {
                    if (StringHelper.IsWhiteSpace(span[j]))
                    {
                        continue;
                    }
                    advance = j;
                    break;
                }
                return span.Slice(0, i);
            }
            throw SerdeException.ThrowInvalidAdvance();
        }

        private static ReadOnlySpan<char> ReadArrayOrObjectUntilEndUtf16(char* src, int maxLength, out int advance)
        {
            var arrayCount = 0;
            var objectCount = 0;
            advance = 0;
            maxLength += 1;
            var span = new ReadOnlySpan<char>(src, maxLength);
            var inString = false;
            for (var i = 0; i < maxLength; i++)
            {
                var character = span[i];
                var isWhiteSpace = StringHelper.IsWhiteSpace(character);
                // 不包括结尾
                if (isWhiteSpace) continue;
                if (inString)
                {
                    if (character is '"' && span[i - 1] != '\\')
                    {
                        inString = false;
                    }
                    continue;
                }
                if (character is '"')
                {
                    inString = true;
                    continue;
                }
                if (character is '{')
                {
                    objectCount++;
                }
                else if (character is '}')
                {
                    objectCount--;
                }
                else if (character is '[')
                {
                    arrayCount++;
                }
                else if (character is ']')
                {
                    arrayCount--;
                }
                else
                {
                    continue;
                }
                // 直到所有括号关闭
                if (arrayCount > 0 || objectCount > 0)
                {
                    continue;
                }
                i++; //读最后一个括号
                advance = i;
                // 把剩下的读完
                for (var j = i; j < span.Length; j++)
                {
                    if (StringHelper.IsWhiteSpace(span[j]))
                    {
                        continue;
                    }
                    advance = j;
                    break;
                }
                return span.Slice(0, i);
            }
            throw SerdeException.ThrowInvalidAdvance();
        }

        public static ReadOnlySpan<char> ReadValueUntilEndUtf16(char* src, int maxLength, out int advance)
        {
            var span = new ReadOnlySpan<char>(src, maxLength);
            // 数组/对象
            if (span[0] == '[' || span[0] == '{')
            {
                return ReadArrayOrObjectUntilEndUtf16(src, maxLength, out advance);
            }
            // 数字 或 null
            if (span[0] != '"')
            {
                return ReadPrimitiveUntilEndUtf16(src, maxLength, out advance);
            }
            // 字符串
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
        public static sbyte? ReadNullableSbyteUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 5, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte? ReadNullableByteUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 4, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal? ReadNullableDecimalUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_decimal_string_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double? ReadNullableDoubleUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_double_string_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float? ReadNullableFloatUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_float_string_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short? ReadNullableShortUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 6, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? ReadNullableIntUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 11, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long? ReadNullableLongUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_int64_numbers_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort? ReadNullableUshortUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 6, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint? ReadNullableUintUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 10, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong? ReadNullableUlongUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_uint64_numbers_length, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUlong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char? ReadNullableCharUtf16(char* src, out int advance)
        {
            var first = src[0];
            var span = ReadValueUntilEndUtf16(src, 8, out advance);
            if (SpanConverter.IsNull(span)) return null;
            if (first != '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
            return SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool? ReadNullableBoolUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 5, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToBool(in span);
        }

        #endregion

        #region Read Primitive

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByteUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 3, out advance);
            return SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSbyteUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 4, out advance);
            return SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimalUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_decimal_string_length, out advance);
            return SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDoubleUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_double_string_length, out advance);
            return SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloatUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_float_string_length, out advance);
            return SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 5, out advance);
            return SpanConverter.ToBool(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadCharUtf16(char* src, out int advance)
        {
            if (src[0] != '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
            var span = ReadValueUntilEndUtf16(src, 8, out advance);
            return SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShortUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 6, out advance);
            return SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadIntUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 11, out advance);
            return SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLongUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_int64_numbers_length, out advance);
            return SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUshortUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 5, out advance);
            return SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUintUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, 10, out advance);
            return SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUlongUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NumberHelper.max_uint64_numbers_length, out advance);
            return SpanConverter.ToUlong(in span);
        }
    }

    #endregion
}