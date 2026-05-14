/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/20
 */

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

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
            var span = new Span<char>(src, maxLength);
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
            var index = 0;
            advance = 0;
            var canAllocateOnStack = maxLength < 512;
            var array = canAllocateOnStack ? Array.Empty<char>() : ArrayPool.UTF16.Rent(maxLength);
            // Debug.Log($"arrayLength: {array.Length}, maxLength: {maxLength}");
            var result = canAllocateOnStack ? stackalloc char[maxLength] : array;
            try
            {
                for (var i = 1; i < span.Length; i++)
                {
                    var character = span[i];
                    if (character == '\\')
                    {
                        result[index++] = ReadEscapedCharUtf16(src + i, out var length);
                        i += length - 1; // -1 跳过当前字符
                    }
                    else if (character == '"')
                    {
                        // 包括双引号
                        advance = i + 1;

                        // 如果达到最大长度，说明没有转义
                        if (index + 2 == advance)
                        {
                            // 不用在堆上分配新字符串, 不包括双引号
                            return span[1..(advance - 1)];
                        }
                        // 分配一个新的字符串
                        return new string(result[..index]);
                    }
                    else
                    {
                        result[index++] = character;
                    }
                }
            }
            finally
            {
                if (!canAllocateOnStack) ArrayPool.UTF16.Return(array);
            }
            throw SerdeException.ThrowInvalidAdvance();
        }

        #region ReadNullable

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte? ReadNullableSbyteUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableSbyteFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte? ReadNullableByteUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableByteFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal? ReadNullableDecimalUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableDecimalFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double? ReadNullableDoubleUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableDoubleFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float? ReadNullableFloatUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableFloatFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short? ReadNullableShortUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableShortFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? ReadNullableIntUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableIntFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long? ReadNullableLongUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableLongFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort? ReadNullableUshortUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableUshortFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint? ReadNullableUintUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableUintFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong? ReadNullableUlongUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableUlongFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToUlong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char? ReadNullableCharUtf16(char* src, out int advance)
        {
            var first = src[0];
            var span = ReadValueUntilEndUtf16(src, NullableCharFormatter.max_size, out advance);
            if (SpanConverter.IsNull(span)) return null;
            if (first != '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
            return SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool? ReadNullableBoolUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, NullableBoolFormatter.max_size, out advance);
            return SpanConverter.IsNull(span) ? null : SpanConverter.ToBool(in span);
        }

        #endregion

        #region Read Primitive

        /// <summary>读转义字符, 支持UTF8和UTF16</summary>
        // ReSharper disable once CognitiveComplexity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadEscapedCharUtf16(char* src, out int advance)
        {
            // Expect starting backslash
            if (src[0] != '\\') throw SerdeException.ThrowInvalidEscapedChar(src[0]);

            var esc = src[1];
            switch (esc)
            {
                case '"':
                    advance = 2;
                    return '"';
                case '\\':
                    advance = 2;
                    return '\\';
                case '/':
                    advance = 2;
                    return '/';
                case 'b':
                    advance = 2;
                    return '\b';
                case 't':
                    advance = 2;
                    return '\t';
                case 'n':
                    advance = 2;
                    return '\n';
                case 'f':
                    advance = 2;
                    return '\f';
                case 'r':
                    advance = 2;
                    return '\r';
                case 'u':
                {
                    // Expect \uXXXX (4 hex digits)
                    // src[0] == '\\', src[1] == 'u', hex digits at src[2..5]
                    var h1 = Hex(src[2]);
                    var h2 = Hex(src[3]);
                    var h3 = Hex(src[4]);
                    var h4 = Hex(src[5]);
                    if (h1 < 0 || h2 < 0 || h3 < 0 || h4 < 0)
                        throw SerdeException.ThrowInvalidEscapedChar(esc);
                    var code = (h1 << 12) | (h2 << 8) | (h3 << 4) | h4;
                    advance = 6;
                    return (char) code;
                }
                default:
                    throw SerdeException.ThrowInvalidEscapedChar(esc);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int Hex(char c)
            {
                if (c is >= '0' and <= '9') return c - '0';
                if (c is >= 'A' and <= 'F') return c - 'A' + 10;
                if (c is >= 'a' and <= 'f') return c - 'a' + 10;
                return -1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByteUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, ByteFormatter.max_size, out advance);
            return SpanConverter.ToByte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSbyteUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, SbyteFormatter.max_size, out advance);
            return SpanConverter.ToSbyte(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimalUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, DecimalFormatter.max_size, out advance);
            return SpanConverter.ToDecimal(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDoubleUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, DoubleFormatter.max_size, out advance);
            return SpanConverter.ToDouble(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloatUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, FloatFormatter.max_size, out advance);
            return SpanConverter.ToFloat(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, BoolFormatter.max_size, out advance);
            return SpanConverter.ToBool(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadCharUtf16(char* src, out int advance)
        {
            if (src[0] != '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
            var span = ReadValueUntilEndUtf16(src, CharFormatter.max_size, out advance);
            return SpanConverter.ToChar(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShortUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, ShortFormatter.max_size, out advance);
            return SpanConverter.ToShort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadIntUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, IntFormatter.max_size, out advance);
            return SpanConverter.ToInt(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLongUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, LongFormatter.max_size, out advance);
            return SpanConverter.ToLong(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUshortUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, UshortFormatter.max_size, out advance);
            return SpanConverter.ToUshort(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUintUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, UintFormatter.max_size, out advance);
            return SpanConverter.ToUint(in span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUlongUtf16(char* src, out int advance)
        {
            var span = ReadPrimitiveUntilEndUtf16(src, UlongFormatter.max_size, out advance);
            return SpanConverter.ToUlong(in span);
        }
    }

    #endregion
}