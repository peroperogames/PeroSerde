using System;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json
{
    public unsafe ref partial struct JsonWriter<TSymbol, TBufferWriter>
    {
        public void WriteValueUtf8<T>(string key, T value, bool writeComma)
        {
            // TODO 写入Format时长度会加一个空格
            var requiredSize = key.Length + GetValueRequiredSizeUtf8(value) + 2; // 2 是 : 和 , 的大小
            var destPtr = GetSpanPtr<byte>(requiredSize);
            var startPtr = destPtr;
            destPtr += WriteStringNoEscapeUnsafeUtf8(destPtr, key);
            destPtr += WriteKeyEndingUnsafeUtf8(destPtr);
            destPtr += WriteValueUnsafeUtf8(destPtr, value);
            if (writeComma)
                destPtr += WriteValueEndingUnsafeUtf8(destPtr);
            Advance((int) (destPtr - startPtr));
        }

        private static int WriteValueUnsafeUtf8<T>(byte* dest, T value)
        {
            return value switch
            {
                bool v => WriteBoolUnsafeUtf8(dest, v),
                sbyte v => WriteInt64UnsafeUtf8(dest, v),
                short v => WriteInt64UnsafeUtf8(dest, v),
                int v => WriteInt64UnsafeUtf8(dest, v),
                long v => WriteInt64UnsafeUtf8(dest, v),
                byte v => WriteUInt64UnsafeUtf8(dest, v),
                ushort v => WriteUInt64UnsafeUtf8(dest, v),
                uint v => WriteUInt64UnsafeUtf8(dest, v),
                ulong v => WriteUInt64UnsafeUtf8(dest, v),
                float v => WriteFloatUnsafeUtf8(dest, v),
                double v => WriteDoubleUnsafeUtf8(dest, v),
                decimal v => WriteDecimalUnsafeUtf8(dest, v),
                char v => WriteCharUnsafeUtf8(dest, v),
                string v => WriteStringUnsafeUtf8(dest, v),
                _ => throw new NotSupportedException($"{value.GetType()} Is Not Supported.")
            };
        }

        private static int GetValueRequiredSizeUtf8<T>(T value)
        {
            return value switch
            {
                bool v => v ? 4 : 5,
                byte => 3, // 0 ~ 255
                sbyte => 4, // -128 ~ 127
                ushort => 5, // 0 到 65535
                short => 6, // -32768 到 32767
                uint => 10, // 0 到 4294967295
                int => 11, // -2147483648 到 2147483647
                ulong => NumberHelper.max_uint64_numbers_length, // 0 到 18446744073709551615
                long => NumberHelper.max_int64_numbers_length, // -9223372036854775808 到 9223372036854775807
                float => NumberHelper.max_float_string_length,
                double => NumberHelper.max_double_string_length,
                decimal => NumberHelper.max_decimal_string_length,
                char v => 2 + (v < 0x20 || v is '"' or '\\' ? 6 : 1),
                string v => Encoding.UTF8.GetMaxByteCount(v.Length) + 2,
                _ => throw new NotSupportedException($"{value.GetType()} Is Not Supported.")
            };
        }

        #region Unsafe Write String

        private static int WriteStringUnsafeUtf8(byte* dest, string value)
        {
            if (value == null)
                return WriteNullUnsafeUtf8(dest);
            var start = dest;
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            dest += WriteStringValueUnsafeUtf8(value.AsSpan(), dest);
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            return (int) (dest - start);
        }

        private static int WriteStringNoEscapeUnsafeUtf8(byte* dest, string value)
        {
            if (value == null)
                return WriteNullUnsafeUtf8(dest);
            var start = dest;
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            dest += WriteStringValueNoEscapeUtf8(value.AsSpan(), dest);
            dest += WriteDoubleQuoteUnsafeUtf8(dest);
            return (int) (dest - start);
        }

        private static int WriteStringValueUnsafeUtf8(ReadOnlySpan<char> str, byte* dest)
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
        private static int WriteStringValueNoEscapeUtf8(ReadOnlySpan<char> str, byte* dest)
        {
            var maxByteCount = Encoding.UTF8.GetMaxByteCount(str.Length);
            var destSpan = new Span<byte>(dest, maxByteCount);
            // .Net5开始可以使用Utf8.FromUtf16()来加速
            return Encoding.UTF8.GetBytes(str, destSpan);
        }

        #endregion

        #region Unsafe Write Primitive

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WriteBoolUnsafeUtf8(byte* dest, bool value)
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
        private static int WriteInt64UnsafeUtf8(byte* dest, long value) =>
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WriteUInt64UnsafeUtf8(byte* dest, ulong value) =>
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WriteFloatUnsafeUtf8(byte* dest, float value) =>
            // TODO 格式值按照字符串或者默认值写入？
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        private static int WriteDoubleUnsafeUtf8(byte* dest, double value) =>
            NumberHelper.ToUtf8StringUnsafe(value, dest);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WriteDecimalUnsafeUtf8(byte* dest, decimal value)
        {
            Utf8Formatter.TryFormat(
                value,
                new Span<byte>(dest, NumberHelper.max_decimal_string_length),
                out var bytesWritten
            );
            return bytesWritten;
        }

        private static int WriteCharUnsafeUtf8(byte* dest, char value)
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
        private static int WriteEscapedCharUnsafeUtf8(char value, byte* dest)
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
        private static int WriteKeyEndingUnsafeUtf8(byte* dest)
        {
            *dest = (byte) ':';
            return 1;
        }

        /// <summary>写入一个英文逗号</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WriteValueEndingUnsafeUtf8(byte* dest)
        {
            *dest = (byte) ',';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WriteDoubleQuoteUnsafeUtf8(byte* dest)
        {
            *dest = (byte) '"';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WriteSpaceUnsafeUtf8(byte* dest)
        {
            *dest = (byte) ' ';
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WriteNullUnsafeUtf8(byte* dest)
        {
            dest[0] = (byte) 'n';
            dest[1] = (byte) 'u';
            dest[2] = (byte) 'l';
            dest[3] = (byte) 'l';
            return 4;
        }

        #endregion
    }
}