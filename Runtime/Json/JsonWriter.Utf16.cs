/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/05
 */

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    [PublicAPI]
    public static unsafe partial class JsonWriter
    {
        #region Write Primitive

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBoolUtf16(char* dest, bool value)
        {
            const ulong le_eurt = 0x0065007500720074;
            const ulong le_slaf = 0x0073006C00610066;
            const ulong be_eurt = 0x6500750072007400;
            const ulong be_slaf = 0x73006C0061006600;
            if (value)
            {
                *(ulong*) dest = BitConverter.IsLittleEndian ? le_eurt : be_eurt;
                return 4;
            }
            *(ulong*) dest = BitConverter.IsLittleEndian ? le_slaf : be_slaf;
            dest[4] = 'e';
            return 5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteLongUtf16(char* dest, long value)
            => NumberHelper.ToUtf16StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUlongUtf16(char* dest, ulong value)
            => NumberHelper.ToUtf16StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteFloatUtf16(char* dest, float value)
            => NumberHelper.ToUtf16StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDoubleUtf16(char* dest, double value)
            => NumberHelper.ToUtf16StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDecimalUtf16(char* dest, decimal value)
        {
            var destSpan = new Span<char>(dest, DecimalFormatter.max_size);
            value.TryFormat(destSpan, out var charsWritten, default, CultureInfo.InvariantCulture);
            return charsWritten;
        }

        #endregion

        #region Write Char
        public static int WriteCharUtf16(char* dest, char value)
        {
            var start = dest;
            dest += WriteDoubleQuoteUtf16(dest);
            if (value < 0x20 || value is '"' or '\\') // 转义控制字符和特殊字符
            {
                dest += WriteEscapedCharUtf16(value, dest);
            }
            else
            {
                *dest = value;
                dest += 1;
            }
            dest += WriteDoubleQuoteUtf16(dest);
            return (int) (dest - start);
        }

        /// <summary>写入转义字符, 支持UTF8和UTF16</summary>
        // ReSharper disable once CognitiveComplexity
        public static int WriteEscapedCharUtf16(char value, char* dest)
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
            static int WriteSingleEscapedChar(char* dest, char toEscape)
            {
                dest[0] = '\\';
                dest[1] = toEscape;
                return 2;
            }

            // 写入\u00xx类型的转义字符.这种字符在文本中占6个位置
            static int WriteDoubleEscapedChar(char* dest, char firstToEscape, char secondToEscape)
            {
                // 需要转移的控制字符范围是\u0000到\u001F, 所以可以固定前四位一定是\u00
                dest[0] = '\\';
                dest[1] = 'u';
                dest[2] = '0';
                dest[3] = '0';
                dest[4] = firstToEscape;
                dest[5] = secondToEscape;
                return 6;
            }
        }

        #endregion

        #region Write String

        public static int WriteStringUtf16(char* dest, ReadOnlySpan<char> value)
        {
            if (value == null)
                return WriteNullUtf16(dest);
            var start = dest;
            dest += WriteDoubleQuoteUtf16(dest);
            dest += WriteStringValueUtf16(value, dest);
            dest += WriteDoubleQuoteUtf16(dest);
            return (int) (dest - start);
        }

        /// <summary>在不考虑转义字符的情况下写入字符串</summary>
        /// <return>charsWritten</return>
        /// <remarks>需要自己保证不会出现内存越界</remarks>
        public static int WriteStringNoEscapeUtf16(char* dest, ReadOnlySpan<char> str)
        {
            if (str == null)
                return WriteNullUtf16(dest);
            var start = dest;
            dest += WriteDoubleQuoteUtf16(dest);
            dest += WriteRawUtf16(dest, str);
            dest += WriteDoubleQuoteUtf16(dest);
            return (int) (dest - start);
        }

        /// <summary>将<see cref="string"/>写入UTF16 Buffer.</summary>
        /// <returns>charsWritten</returns>
        /// <remarks>会考虑到转义字符.</remarks>
        public static int WriteStringValueUtf16(ReadOnlySpan<char> str, char* dest)
        {
            // 需要考虑到转义字符, 所以必须遍历来找到转义字符，并且单独写入.
            var originalPtr = dest;
            var from = 0;
            for (var i = 0; i < str.Length; i++)
            {
                ref readonly var c = ref str[i];
                if (c < 0x20 || c is '"' or '\\')
                {
                    if (i - from > 0)
                    {
                        // 字符串本身就是UTF16编码, 在不考虑大端序小端序的情况下可以直接写入
                        dest += WriteRawUtf16(dest, str.Slice(from, i - from));
                    }
                    dest += WriteEscapedCharUtf16(c, dest);
                    from = i + 1;
                }
            }
            if (from < str.Length)
            {
                dest += WriteRawUtf16(dest, str[from..]);
            }
            return (int) (dest - originalPtr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteRawUtf16(char* dest, ReadOnlySpan<char> src)
        {
            fixed (char* srcPtr = src)
            {
                Unsafe.CopyBlockUnaligned(dest, srcPtr, (uint) src.Length * sizeof(char));
            }
            return src.Length;
        }

        #endregion

        #region Write Literal

        /// <summary>写入一个英文冒号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteKeyEndingUtf16(char* dest)
        {
            *dest = ':';
            return 1;
        }

        /// <summary>写入一个英文逗号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteValueEndingUtf16(char* dest)
        {
            *dest = ',';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDoubleQuoteUtf16(char* dest)
        {
            *dest = '"';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteSpaceUtf16(char* dest)
        {
            *dest = ' ';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBeginObjectUtf16(char* dest)
        {
            *dest = '{';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteEndObjectUtf16(char* dest)
        {
            *dest = '}';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBeginArrayUtf16(char* dest)
        {
            *dest = '[';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteEndArrayUtf16(char* dest)
        {
            *dest = ']';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteNullUtf16(char* dest)
        {
            // 小端
            const ulong le_llun = 0x006C006C0075006E;
            // 大端
            const ulong be_llun = 0x6C006C0075006E00;
            *(ulong*) dest = BitConverter.IsLittleEndian ? le_llun : be_llun;
            return 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteChar(char* dest, char character)
        {
            *dest = character;
            return 1;
        }

        #endregion

        #region Write Indent

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteIndent(int count, char* dest)
        {
            for (var i = 0; i < count; i++)
            {
                dest[i] = ' ';
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteNewLine(char* dest)
        {
            *dest = '\n';
            return 1;
        }

        #endregion
    }
}