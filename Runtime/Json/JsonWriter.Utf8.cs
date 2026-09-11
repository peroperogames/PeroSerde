/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/05
 */

using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json
{
    public static unsafe partial class JsonWriter
    {
        #region Write String

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteRawUtf8(byte* dest, ReadOnlySpan<byte> src)
        {
            fixed (byte* srcPtr = src)
            {
                Unsafe.CopyBlockUnaligned(dest, srcPtr, (uint) src.Length);
            }
            return src.Length;
        }

        public static int WriteStringUtf8(byte* dest, ReadOnlySpan<char> value)
        {
            if (value == null)
                return WriteNullUtf8(dest);
            var start = dest;
            dest += WriteDoubleQuoteUtf8(dest);
            dest += WriteStringValueUtf8(value, dest);
            dest += WriteDoubleQuoteUtf8(dest);
            return (int) (dest - start);
        }

        public static int WriteStringNoEscapeUtf8(byte* dest, ReadOnlySpan<char> value)
        {
            if (value == null)
                return WriteNullUtf8(dest);
            var start = dest;
            dest += WriteDoubleQuoteUtf8(dest);
            dest += WriteStringValueNoEscapeUtf8(value, dest);
            dest += WriteDoubleQuoteUtf8(dest);
            return (int) (dest - start);
        }

        public static int WriteStringValueUtf8(ReadOnlySpan<char> str, byte* dest)
        {
            // 需要考虑到转义字符, 所以必须遍历来找到转义字符，并且单独写入.
            var from = 0;
            var start = dest;
            for (var i = 0; i < str.Length; i++)
            {
                ref readonly var c = ref str[i];
                if (c < 0x20 || c is '"' or '\\')
                {
                    if (i - from > 0)
                    {
                        dest += WriteStringValueNoEscapeUtf8(str[from..(i - from)], dest);
                    }
                    dest += WriteEscapedCharUtf8(c, dest);
                    from = i + 1;
                }
            }
            if (from < str.Length)
            {
                dest += WriteStringValueNoEscapeUtf8(str[from..], dest);
            }
            return (int) (dest - start);
        }

        /// <summary>
        /// 在不考虑转义字符的情况下写入UTF8序列.
        /// 
        /// </summary>
        /// <remarks>
        /// 1. 如果字符串内有需要转义的字符，则结果会异常.
        /// 2. dest的可用长度必须大于<c>Encoding.UTF8.GetMaxByteCount(str.Length)</c>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteStringValueNoEscapeUtf8(ReadOnlySpan<char> str, byte* dest)
        {
            var maxByteCount = Encoding.UTF8.GetMaxByteCount(str.Length);
            var destSpan = new Span<byte>(dest, maxByteCount);
            // .Net5开始可以使用Utf8.FromUtf16()来加速
            return Encoding.UTF8.GetBytes(str, destSpan);
        }

        #endregion

        #region Write Primitive

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBoolUtf8(byte* dest, bool value)
        {
            // UTF8 不存在字节序问题
            const uint eurt = 0x65757274U;
            const uint slaf = 0x736C6166U;

            if (value)
            {
                Unsafe.WriteUnaligned(dest, eurt);
                return 4;
            }
            Unsafe.WriteUnaligned(dest, slaf);
            dest[4] = (byte) 'e';
            return 5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteLongUtf8(byte* dest, long value) =>
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUlongUtf8(byte* dest, ulong value) =>
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteFloatUtf8(byte* dest, float value) =>
            // TODO 格式值按照字符串或者默认值写入？
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        public static int WriteDoubleUtf8(byte* dest, double value) =>
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDecimalUtf8(byte* dest, decimal value)
        {
            Utf8Formatter.TryFormat(
                value,
                new Span<byte>(dest, DecimalFormatter.max_size),
                out var bytesWritten
            );
            return bytesWritten;
        }

        public static int WriteCharUtf8(byte* dest, char value)
        {
            var start = dest;
            dest += WriteDoubleQuoteUtf8(dest);
            if (value < 0x20 || value is '"' or '\\') // 转义控制字符和特殊字符
            {
                dest += WriteEscapedCharUtf8(value, dest);
            }
            else if (value < 0x80)
            {
                *dest = (byte) value;
                dest += 1;
            }
            // 暂不支持代理对
            else if (char.IsSurrogate(value))
            {
                throw SerdeException.ThrowInvalidSurrogateChar(value);
            }
            // 2-byte UTF-8 sequence
            else if (value <= 0x7FF)
            {
                dest[0] = (byte) (0xC0 | (value >> 6));
                dest[1] = (byte) (0x80 | (value & 0x3F));
                dest += 2;
            }
            // 3-byte UTF-8 sequence
            else
            {
                dest[0] = (byte) (0xE0 | (value >> 12));
                dest[1] = (byte) (0x80 | ((value >> 6) & 0x3F));
                dest[2] = (byte) (0x80 | (value & 0x3F));
                dest += 3;
            }
            dest += WriteDoubleQuoteUtf8(dest);
            return (int) (dest - start);
        }

        /// <summary>写入转义字符, 支持UTF8和UTF16</summary>
        // ReSharper disable once CognitiveComplexity
        public static int WriteEscapedCharUtf8(char value, byte* dest)
        {
            return value switch
            {
                '"' => WriteSingleEscapedChar(dest, '"'),
                '\\' => WriteSingleEscapedChar(dest, '\\'),
                '\u0000' => WriteDoubleEscapedChar(dest, '0', '0'),
                '\u0001' => WriteDoubleEscapedChar(dest, '0', '1'),
                '\u0002' => WriteDoubleEscapedChar(dest, '0', '2'),
                '\u0003' => WriteDoubleEscapedChar(dest, '0', '3'),
                '\u0004' => WriteDoubleEscapedChar(dest, '0', '4'),
                '\u0005' => WriteDoubleEscapedChar(dest, '0', '5'),
                '\u0006' => WriteDoubleEscapedChar(dest, '0', '6'),
                '\u0007' => WriteDoubleEscapedChar(dest, '0', '7'),
                '\b' => WriteSingleEscapedChar(dest, 'b'),
                '\t' => WriteSingleEscapedChar(dest, 't'),
                '\n' => WriteSingleEscapedChar(dest, 'n'),
                '\u000B' => WriteDoubleEscapedChar(dest, '0', 'B'),
                '\f' => WriteSingleEscapedChar(dest, 'f'),
                '\r' => WriteSingleEscapedChar(dest, 'r'),
                '\u000E' => WriteDoubleEscapedChar(dest, '0', 'E'),
                '\u000F' => WriteDoubleEscapedChar(dest, '0', 'F'),
                '\u0010' => WriteDoubleEscapedChar(dest, '1', '0'),
                '\u0011' => WriteDoubleEscapedChar(dest, '1', '1'),
                '\u0012' => WriteDoubleEscapedChar(dest, '1', '2'),
                '\u0013' => WriteDoubleEscapedChar(dest, '1', '3'),
                '\u0014' => WriteDoubleEscapedChar(dest, '1', '4'),
                '\u0015' => WriteDoubleEscapedChar(dest, '1', '5'),
                '\u0016' => WriteDoubleEscapedChar(dest, '1', '6'),
                '\u0017' => WriteDoubleEscapedChar(dest, '1', '7'),
                '\u0018' => WriteDoubleEscapedChar(dest, '1', '8'),
                '\u0019' => WriteDoubleEscapedChar(dest, '1', '9'),
                '\u001A' => WriteDoubleEscapedChar(dest, '1', 'A'),
                '\u001B' => WriteDoubleEscapedChar(dest, '1', 'B'),
                '\u001C' => WriteDoubleEscapedChar(dest, '1', 'C'),
                '\u001D' => WriteDoubleEscapedChar(dest, '1', 'D'),
                '\u001E' => WriteDoubleEscapedChar(dest, '1', 'E'),
                '\u001F' => WriteDoubleEscapedChar(dest, '1', 'F'),
                _ => throw SerdeException.ThrowInvalidEscapedChar(value)
            };

            // 写入\x类型的转义字符.这种字符在文本中占2个位置
            static int WriteSingleEscapedChar(byte* dest, char toEscape)
            {
                dest[0] = (byte) '\\';
                dest[1] = (byte) toEscape;
                return 2;
            }

            // 写入\u00xx类型的转义字符.这种字符在文本中占6个位置
            static int WriteDoubleEscapedChar(byte* dest, char firstToEscape, char secondToEscape)
            {
                // 需要转移的控制字符范围是\u0000到\u001F, 所以可以固定前四位一定是\u00
                dest[0] = (byte) '\\';
                dest[1] = (byte) 'u';
                dest[2] = (byte) '0';
                dest[3] = (byte) '0';
                dest[4] = (byte) firstToEscape;
                dest[5] = (byte) secondToEscape;
                return 6;
            }
        }

        #endregion

        #region Write Literal

        /// <summary>写入一个英文冒号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteKeyEndingUtf8(byte* dest)
        {
            *dest = (byte) ':';
            return 1;
        }

        /// <summary>写入一个英文逗号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteValueEndingUtf8(byte* dest)
        {
            *dest = (byte) ',';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDoubleQuoteUtf8(byte* dest)
        {
            *dest = (byte) '"';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteSpaceUtf8(byte* dest)
        {
            *dest = (byte) ' ';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBeginObjectUtf8(byte* dest)
        {
            *dest = (byte) '{';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteEndObjectUtf8(byte* dest)
        {
            *dest = (byte) '}';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBeginArrayUtf8(byte* dest)
        {
            *dest = (byte) '[';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteEndArrayUtf8(byte* dest)
        {
            *dest = (byte) ']';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteAscii(byte* dest, char ascii)
        {
            *dest = (byte) ascii;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteNullUtf8(byte* dest)
        {
            // Utf8 不需要考虑字节序问题
            const uint llun = 0x6C6C756EU;
            Unsafe.WriteUnaligned(dest, llun);
            return 4;
        }

        #endregion

        #region Write Indent

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteIndent(int count, byte* dest)
        {
            for (var i = 0; i < count; i++)
            {
                dest[i] = (byte) ' ';
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteNewLine(byte* dest)
        {
            *dest = (byte) '\n';
            return 1;
        }

        #endregion
    }
}