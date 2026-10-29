/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/20
 */

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json
{
    public static unsafe partial class JsonReader
    {
        internal static ReadOnlySpan<byte> ReadPrimitiveUntilEndUtf8(byte* src, int maxLength, out int advance)
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

        /// <summary>字符串类型的唯一解析入口：ASCII 快路径 + 通用解码，null 返回 null。</summary>
        internal static string ReadStringValueUtf8(byte* src, int length, out int advance)
        {
            advance = 0;
            if (length > 0 && src[0] == (byte) '"')
            {
                if (TryReadAsciiStringUtf8(src, length, out var asciiValue, out advance))
                {
                    return asciiValue;
                }
            }

            var result = ReadValueUntilEndUtf8(src, length, out advance);
            if (SpanConverter.IsNull(result))
            {
                return null;
            }

            if (length == 0 || src[0] != (byte) '"')
            {
                throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
            }

            return Encoding.UTF8.GetString(result);
        }

        public static ReadOnlySpan<byte> ReadValueUntilEndUtf8(byte* src, int maxLength, out int advance)
        {
            var span = new Span<byte>(src, maxLength);
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
            var index = 0;
            advance = 0;
            var canAllocateOnStack = maxLength < 1024;
            var array = canAllocateOnStack ? Array.Empty<byte>() : ArrayPool.UTF8.Rent(maxLength);
            var result = canAllocateOnStack ? stackalloc byte[maxLength] : array;
            try
            {
                for (var i = 1; i < span.Length; i++)
                {
                    var character = span[i];
                    if (character == '\\')
                    {
                        result[index++] = (byte) ReadEscapedCharUtf8(src + i, out var length);
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
                        return result[..index].ToArray();
                    }
                    else
                    {
                        result[index++] = character;
                    }
                }
            }
            finally
            {
                if (!canAllocateOnStack) ArrayPool.UTF8.Return(array);
            }
            throw SerdeException.ThrowInvalidAdvance();
        }

        /// <summary>
        /// 读取一个纯 ASCII 的 JSON 字符串值（允许 ASCII 转义），只做一次堆分配.
        /// </summary>
        /// <remarks>
        /// 通用路径需要先把反转义结果写进 byte[] 再转成 string（两次分配）；
        /// ASCII 场景下可以先算出输出长度，直接一次分配成 string。
        /// 遇到非 ASCII 字节或非 ASCII 转义时返回 false，由调用方走通用路径。
        /// </remarks>
        internal static bool TryReadAsciiStringUtf8(byte* src, int maxLength, out string result, out int advance)
        {
            result = null;
            advance = 0;
            var span = new ReadOnlySpan<byte>(src, maxLength);

            // 第一遍：定位结束引号，同时确认整个 token 都是 ASCII 且转义序列完整。
            // 纯 ASCII 且无转义是最常见的情况，先用 4×uint 一次跳过 16 字节。
            var hasEscape = false;
            var end = -1;
            var pos = 1;
            {
                var quote = (uint) '"' * 0x0101_0101u;
                var slash = (uint) '\\' * 0x0101_0101u;
                var vectorLimit = span.Length - 16;
                while (pos <= vectorLimit)
                {
                    var p = src + pos;
                    var v0 = Unsafe.ReadUnaligned<uint>(p);
                    var v1 = Unsafe.ReadUnaligned<uint>(p + 4);
                    var v2 = Unsafe.ReadUnaligned<uint>(p + 8);
                    var v3 = Unsafe.ReadUnaligned<uint>(p + 12);
                    if (((v0 | v1 | v2 | v3) & 0x8080_8080u) != 0
                        || (ZeroByte(v0 ^ quote) | ZeroByte(v1 ^ quote) | ZeroByte(v2 ^ quote) | ZeroByte(v3 ^ quote)) != 0
                        || (ZeroByte(v0 ^ slash) | ZeroByte(v1 ^ slash) | ZeroByte(v2 ^ slash) | ZeroByte(v3 ^ slash)) != 0)
                    {
                        // 这一块含特殊字节，交给下面的标量循环逐字节处理（它负责转义跳过）
                        break;
                    }

                    pos += 16;
                }
            }

            for (; pos < span.Length; pos++)
            {
                var b = span[pos];
                if (b == (byte) '"')
                {
                    end = pos;
                    break;
                }
                if (b >= 0x80)
                {
                    return false;
                }
                if (b != (byte) '\\')
                {
                    continue;
                }
                hasEscape = true;
                if (pos + 1 >= span.Length)
                {
                    return false;
                }
                pos += span[pos + 1] == (byte) 'u' ? 5 : 1;
                if (pos >= span.Length)
                {
                    return false;
                }
            }

            if (end < 0)
            {
                return false;
            }

            var token = span.Slice(1, end - 1);
            advance = end + 1;
            if (!hasEscape)
            {
                result = SpanConverter.ToString(token);
                return true;
            }

            // 第二遍：算出输出字符数
            var length = 0;
            for (var i = 0; i < token.Length; i++)
            {
                if (token[i] == (byte) '\\')
                {
                    if (token[i + 1] == (byte) 'u')
                    {
                        var h1 = Hex(token[i + 2]);
                        var h2 = Hex(token[i + 3]);
                        var h3 = Hex(token[i + 4]);
                        var h4 = Hex(token[i + 5]);
                        if (h1 < 0 || h2 < 0 || h3 < 0 || h4 < 0)
                        {
                            return false;
                        }
                        var code = (h1 << 12) | (h2 << 8) | (h3 << 4) | h4;
                        if (code > 0x7F)
                        {
                            // 非 ASCII 转义交给通用路径（那里是按字节写回的）
                            return false;
                        }
                        i += 5;
                    }
                    else
                    {
                        i += 1;
                    }
                }
                length++;
            }

            // 第三遍：一次性写进字符串
            var chars = new string('\0', length);
            var index = 0;
            fixed (char* dest = chars)
            {
                for (var i = 0; i < token.Length; i++)
                {
                    if (token[i] != (byte) '\\')
                    {
                        dest[index++] = (char) token[i];
                        continue;
                    }
                    // 复用现有解码逻辑，保证与通用路径行为一致（第一遍已保证范围内不会抛异常）
                    dest[index++] = ReadEscapedCharUtf8(src + 1 + i, out var escapedLength);
                    i += escapedLength - 1;
                }
            }

            result = chars;
            return true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static uint ZeroByte(uint v) => (v - 0x0101_0101u) & ~v & 0x8080_8080u;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int Hex(byte c)
            {
                if (c is >= (byte) '0' and <= (byte) '9') return c - '0';
                if (c is >= (byte) 'a' and <= (byte) 'f') return c - 'a' + 10;
                if (c is >= (byte) 'A' and <= (byte) 'F') return c - 'A' + 10;
                return -1;
            }
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

        /// <summary>读转义字符, 支持UTF8和UTF16</summary>
        // ReSharper disable once CognitiveComplexity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadEscapedCharUtf8(byte* src, out int advance)
        {
            // Expect starting backslash
            if (src[0] != '\\') throw SerdeException.ThrowInvalidEscapedChar(src[0]);

            var esc = (char) src[1];
            switch (esc)
            {
                case '"':
                    advance = 2; return '"';
                case '\\':
                    advance = 2; return '\\';
                case '/':
                    advance = 2; return '/';
                case 'b':
                    advance = 2; return '\b';
                case 't':
                    advance = 2; return '\t';
                case 'n':
                    advance = 2; return '\n';
                case 'f':
                    advance = 2; return '\f';
                case 'r':
                    advance = 2; return '\r';
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
            static uint ZeroByte(uint v) => (v - 0x0101_0101u) & ~v & 0x8080_8080u;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int Hex(byte c)
            {
                if (c is >= (byte) '0' and <= (byte) '9') return c - '0';
                if (c is >= (byte) 'A' and <= (byte) 'F') return c - 'A' + 10;
                if (c is >= (byte) 'a' and <= (byte) 'f') return c - 'a' + 10;
                return -1;
            }
        }

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
