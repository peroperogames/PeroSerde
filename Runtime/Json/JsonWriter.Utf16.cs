using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    public static unsafe partial class JsonWriter
    {
        #region Unsafe Write Primitive

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBoolUnsafeUtf16(char* dest, bool value)
        {
            if (value)
            {
                dest[0] = 't';
                dest[1] = 'r';
                dest[2] = 'u';
                dest[3] = 'e';
                return 4;
            }
            else
            {
                dest[0] = 'f';
                dest[1] = 'a';
                dest[2] = 'l';
                dest[3] = 's';
                dest[4] = 'e';
                return 5;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteInt64UnsafeUtf16(char* dest, long value) =>
            NumberHelper.ToUtf16StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt64UnsafeUtf16(char* dest, ulong value) =>
            NumberHelper.ToUtf16StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteFloatUnsafeUtf16(char* dest, float value) =>
            NumberHelper.ToUtf16StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDoubleUnsafeUtf16(char* dest, double value)
            => NumberHelper.ToUtf16StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDecimalUnsafeUtf16(char* dest, decimal value)
        {
            // return NumberHelper.ToUtf16StringUnsafe(value, dest);
            var destSpan = new Span<char>(dest, NumberHelper.max_decimal_string_length);
            value.TryFormat(destSpan, out var charsWritten, default, CultureInfo.InvariantCulture);
            return charsWritten;
        }

        #endregion

        #region Unsafe Write Char

        public static int WriteCharUnsafeUtf16(char* dest, char value)
        {
            var start = dest;
            dest += WriteDoubleQuoteUnsafeUtf16(dest);
            if (value < 0x20 || value is '"' or '\\') // 转义控制字符和特殊字符
            {
                dest += WriteEscapedCharUnsafeUtf16(value, dest);
            }
            else
            {
                *dest = value;
                dest += 1;
            }
            dest += WriteDoubleQuoteUnsafeUtf16(dest);
            return (int) (dest - start);
        }

        /// <summary>写入转义字符, 支持UTF8和UTF16</summary>
        // ReSharper disable once CognitiveComplexity
        public static int WriteEscapedCharUnsafeUtf16(char value, char* dest)
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

        #region Unsafe Write String

        public static int WriteStringUnsafeUtf16(char* dest, string value)
        {
            if (value == null)
                return WriteNullUnsafeUtf16(dest);
            var start = dest;
            dest += WriteDoubleQuoteUnsafeUtf16(dest);
            dest += WriteStringValueUnsafeUtf16(value.AsSpan(), dest);
            dest += WriteDoubleQuoteUnsafeUtf16(dest);
            return (int) (dest - start);
        }

        /// <summary>在不考虑转义字符的情况下写入字符串</summary>
        /// <return>charsWritten</return>
        /// <remarks>需要自己保证不会出现内存越界</remarks>
        public static int WriteStringNoEscapeUnsafeUtf16(char* dest, string str)
        {
            if (str == null)
                return WriteNullUnsafeUtf16(dest);
            var start = dest;
            dest += WriteDoubleQuoteUnsafeUtf16(dest);
            dest += WriteRawUnsafeUtf16(str.AsSpan(), dest);
            dest += WriteDoubleQuoteUnsafeUtf16(dest);
            return (int) (dest - start);
        }

        /// <summary>将<see cref="string"/>写入UTF16 Buffer.</summary>
        /// <returns>charsWritten</returns>
        /// <remarks>会考虑到转义字符.</remarks>
        public static int WriteStringValueUnsafeUtf16(ReadOnlySpan<char> str, char* dest)
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
                        dest += WriteRawUnsafeUtf16(str.Slice(from, i - from), dest);
                    }
                    dest += WriteEscapedCharUnsafeUtf16(c, dest);
                    from = i + 1;
                }
            }
            if (from < str.Length)
            {
                dest += WriteRawUnsafeUtf16(str[from..], dest);
            }
            return (int) (dest - originalPtr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteRawUnsafeUtf16(ReadOnlySpan<char> src, char* dest)
        {
            fixed (char* srcPtr = src)
            {
                Unsafe.CopyBlockUnaligned(dest, srcPtr, (uint) src.Length * sizeof(char));
            }
            return src.Length;
        }

        #endregion

        #region Unsafe Write Literal

        /// <summary>写入一个英文冒号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteKeyEndingUnsafeUtf16(char* dest)
        {
            *dest = ':';
            return 1;
        }

        /// <summary>写入一个英文逗号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteValueEndingUnsafeUtf16(char* dest)
        {
            *dest = ',';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDoubleQuoteUnsafeUtf16(char* dest)
        {
            *dest = '"';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteSpaceUnsafeUtf16(char* dest)
        {
            *dest = ' ';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBeginArrayUnsafeUtf16(char* dest)
        {
            *dest = '[';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteEndArrayUnsafeUtf16(char* dest)
        {
            *dest = ']';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteNullUnsafeUtf16(char* dest)
        {
            dest[0] = 'n';
            dest[1] = 'u';
            dest[2] = 'l';
            dest[3] = 'l';
            return 4;
        }

        #endregion

        #region Unsafe Write Indent

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteIndentUnsafe(int count, char* dest)
        {
            for (var i = 0; i < count; i++)
            {
                dest[i] = ' ';
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteNewLineUnsafe(char* dest)
        {
            *dest = '\n';
            return 1;
        }

        #endregion
    }
}