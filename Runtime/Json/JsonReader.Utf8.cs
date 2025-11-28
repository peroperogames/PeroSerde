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
        private static ReadOnlySpan<byte> ReadPrimitiveUntilEndUtf8(byte* src, int maxLength, out int advance)
        {
            maxLength += 1;
            advance = 0;
            var span = new ReadOnlySpan<byte>(src, maxLength);
            for (var i = 0; i < maxLength; i++)
            {
                var character = (char) span[i];
                var isWhiteSpace = StringHelper.IsWhiteSpace(character);
                // 不包括结尾
                if (!isWhiteSpace && character is not ('}' or ']' or ','))
                {
                    continue;
                }
                advance = i;
                // 把剩下空白的读完
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

        private static ReadOnlySpan<byte> ReadArrayOrObjectUntilEndUtf8(byte* src, int maxLength, out int advance)
        {
            var arrayCount = 0;
            var objectCount = 0;
            advance = 0;
            maxLength += 1;
            var span = new ReadOnlySpan<byte>(src, maxLength);
            var inString = false;
            for (var i = 0; i < maxLength; i++)
            {
                var character = (char) span[i];
                var isWhiteSpace = StringHelper.IsWhiteSpace(character);
                // 不包括结尾
                if (isWhiteSpace) continue;
                if (inString)
                {
                    if (character is '"' && (char) span[i - 1] != '\\')
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

        public static ReadOnlySpan<byte> ReadValueUntilEndUtf8(byte* src, int maxLength, out int advance)
        {
            var span = new ReadOnlySpan<byte>(src, maxLength);
            // 数组/对象
            if (span[0] == '[' || span[0] == '{')
            {
                return ReadArrayOrObjectUntilEndUtf8(src, maxLength, out advance);
            }
            // 数字 或 null
            if (span[0] != '"')
            {
                return ReadPrimitiveUntilEndUtf8(src, maxLength, out advance);
            }
            // 字符串
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
            var span = ReadPrimitiveUntilEndUtf8(src, NullableSbyteFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte? ReadNullableByteUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableByteFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal? ReadNullableDecimalUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableDecimalFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double? ReadNullableDoubleUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableDoubleFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float? ReadNullableFloatUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableFloatFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short? ReadNullableShortUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableShortFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? ReadNullableIntUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableIntFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long? ReadNullableLongUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableLongFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort? ReadNullableUshortUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableUshortFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint? ReadNullableUintUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableUintFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong? ReadNullableUlongUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableUlongFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUlong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char? ReadNullableCharUtf8(byte* src, out int advance)
        {
            var first = src[0];
            var span = ReadPrimitiveUntilEndUtf8(src, NullableCharFormatter.max_size, out advance);
            if (SpanConverter.IsNull(span)) return null;
            if (first != (byte) '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
            return SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool? ReadNullableBoolUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, NullableBoolFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToBool(in span);
        }

        #endregion

        #region Read Primitive

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByteUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, ByteFormatter.max_size, out advance);
            return SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSbyteUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, SbyteFormatter.max_size, out advance);
            return SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimalUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, DecimalFormatter.max_size, out advance);
            return SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDoubleUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, DoubleFormatter.max_size, out advance);
            return SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloatUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, FloatFormatter.max_size, out advance);
            return SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, BoolFormatter.max_size, out advance);
            return SpanConverter.ToBool(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadCharUtf8(byte* src, out int advance)
        {
            if ((char) src[0] != '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
            var span = ReadValueUntilEndUtf8(src, CharFormatter.max_size, out advance);
            return SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShortUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, ShortFormatter.max_size, out advance);
            return SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadIntUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, IntFormatter.max_size, out advance);
            return SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLongUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, LongFormatter.max_size, out advance);
            return SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUshortUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, UshortFormatter.max_size, out advance);
            return SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUintUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, UintFormatter.max_size, out advance);
            return SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUlongUtf8(byte* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf8(src, UlongFormatter.max_size, out advance);
            return SpanConverter.ToUlong(in span);
        }
    }

    #endregion
}