using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json
{
    public static unsafe partial class JsonWriter
    {
        #region Unsafe Write String

        public static int WriteStringUnsafeUtf8(byte* dest, string value)
        {
            if (value == null)
                return WriteNullUnsafeUtf8(dest);
            var start = dest;
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            dest += WriteStringValueUnsafeUtf8(value.AsSpan(), dest);
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            return (int) (dest - start);
        }

        public static int WriteStringNoEscapeUnsafeUtf8(byte* dest, string value)
        {
            if (value == null)
                return WriteNullUnsafeUtf8(dest);
            var start = dest;
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            dest += WriteStringValueNoEscapeUtf8(value.AsSpan(), dest);
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            return (int) (dest - start);
        }

        public static int WriteStringValueUnsafeUtf8(ReadOnlySpan<char> str, byte* dest)
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
                    dest += WriteCharUnsafeUtf8(dest, c);
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

        #region Unsafe Write Primitive

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBoolUnsafeUtf8(byte* dest, bool value)
        {
            if (value)
            {
                dest[0] = (byte) 't';
                dest[1] = (byte) 'r';
                dest[2] = (byte) 'u';
                dest[3] = (byte) 'e';
                return 4;
            }
            else
            {
                dest[0] = (byte) 'f';
                dest[1] = (byte) 'a';
                dest[2] = (byte) 'l';
                dest[3] = (byte) 's';
                dest[4] = (byte) 'e';
                return 5;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteInt64UnsafeUtf8(byte* dest, long value) =>
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteUInt64UnsafeUtf8(byte* dest, ulong value) =>
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteFloatUnsafeUtf8(byte* dest, float value) =>
            // TODO 格式值按照字符串或者默认值写入？
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        public static int WriteDoubleUnsafeUtf8(byte* dest, double value) =>
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDecimalUnsafeUtf8(byte* dest, decimal value)
        {
            Utf8Formatter.TryFormat(
                value,
                new Span<byte>(dest, NumberHelper.max_decimal_string_length),
                out var bytesWritten
            );
            return bytesWritten;
        }

        public static int WriteCharUnsafeUtf8(byte* dest, char value)
        {
            var start = dest;
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            if (value < 0x20 || value is '"' or '\\') // 转义控制字符和特殊字符
            {
                dest += WriteEscapedCharUnsafeUtf8(value, dest);
            }
            else
            {
                *dest = (byte) value;
                dest += 1;
            }
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            return (int) (dest - start);
        }

        /// <summary>写入转义字符, 支持UTF8和UTF16</summary>
        // ReSharper disable once CognitiveComplexity
        public static int WriteEscapedCharUnsafeUtf8(char value, byte* dest)
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

        #region Unsafe Write Literal

        /// <summary>写入一个英文冒号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteKeyEndingUnsafeUtf8(byte* dest)
        {
            *dest = (byte) ':';
            return 1;
        }

        /// <summary>写入一个英文逗号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteValueEndingUnsafeUtf8(byte* dest)
        {
            *dest = (byte) ',';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteDoubleQuoteUnsafeUtf8(byte* dest)
        {
            *dest = (byte) '"';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteSpaceUnsafeUtf8(byte* dest)
        {
            *dest = (byte) ' ';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBeginObjectUnsafeUtf8(byte* dest)
        {
            *dest = (byte) '{';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteEndObjectUnsafeUtf8(byte* dest)
        {
            *dest = (byte) '}';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteBeginArrayUnsafeUtf8(byte* dest)
        {
            *dest = (byte) '[';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteEndArrayUnsafeUtf8(byte* dest)
        {
            *dest = (byte) ']';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteNullUnsafeUtf8(byte* dest)
        {
            dest[0] = (byte) 'n';
            dest[1] = (byte) 'u';
            dest[2] = (byte) 'l';
            dest[3] = (byte) 'l';
            return 4;
        }

        #endregion

        #region Unsafe Write Indent

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteIndentUnsafe(int count, byte* dest)
        {
            for (var i = 0; i < count; i++)
            {
                dest[i] = (byte) ' ';
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WriteNewLineUnsafe(byte* dest)
        {
            *dest = (byte) '\n';
            return 1;
        }

        #endregion
    }
}