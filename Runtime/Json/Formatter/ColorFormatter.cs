/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// Color Json 格式化器
    /// </summary>
    /// <example>
    /// <code>
    /// "(0.5,0.25,0.75,1.0)"// 不带空格
    /// </code>
    /// </example>
    [PublicAPI]
    public sealed class ColorFormatter : ISerdeJsonFormatter<Color>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(Color value, ref JsonWriterUtf8 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteAscii('(');
            writer.WriteValue(value.r);
            writer.WriteValueEnding();
            writer.WriteValue(value.g);
            writer.WriteValueEnding();
            writer.WriteValue(value.b);
            writer.WriteValueEnding();
            writer.WriteValue(value.a);
            writer.WriteAscii(')');
            writer.WriteDoubleQuote();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(Color value, ref JsonWriterUtf16 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteChar('(');
            writer.WriteValue(value.r);
            writer.WriteValueEnding();
            writer.WriteValue(value.g);
            writer.WriteValueEnding();
            writer.WriteValue(value.b);
            writer.WriteValueEnding();
            writer.WriteValue(value.a);
            writer.WriteChar(')');
            writer.WriteDoubleQuote();
        }

        public unsafe Color DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            var span = reader.ReadUntilEnd();
            var r = 0f;
            var g = 0f;
            var b = 0f;
            var a = 1f;
            var rStart = 0;
            var gStart = 0;
            var bStart = 0;
            var aStart = 0;
            var component = 1;
            if (span[0] != (byte) '(' || span[span.Length - 1] != (byte) ')')
            {
                throw SerdeException.ThrowInvalidParsing();
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = (char) span[i];
                if (character is '(' or '"' || StringHelper.IsWhiteSpace(character)) continue;

                if (component == 1 && rStart == 0)
                {
                    rStart = i;
                }
                else if (component == 2 && gStart == 0)
                {
                    gStart = i;
                }
                else if (component == 3 && bStart == 0)
                {
                    bStart = i;
                }
                else if (component == 4 && aStart == 0)
                {
                    aStart = i;
                }
                fixed (byte* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, (byte) ',', (byte) ')');
                    if (length == -1) throw new Exception("Parse error");
                    if (component == 1)
                    {
                        r = SpanConverter.ToFloat(span.Slice(rStart, length));
                    }
                    else if (component == 2)
                    {
                        g = SpanConverter.ToFloat(span.Slice(gStart, length));
                    }
                    else if (component == 3)
                    {
                        b = SpanConverter.ToFloat(span.Slice(bStart, length));
                    }
                    else if (component == 4)
                    {
                        a = SpanConverter.ToFloat(span.Slice(aStart, length));
                    }
                    i += length;
                    component++;
                }
            }
            return new Color(r, g, b, a == 0f ? 1 : a);
        }

        public unsafe Color DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            var span = reader.ReadUntilEnd();
            var r = 0f;
            var g = 0f;
            var b = 0f;
            var a = 0f;
            var rStart = 0;
            var gStart = 0;
            var bStart = 0;
            var aStart = 0;
            var component = 1;
            if (span[0] != '(' || span[span.Length - 1] != ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = span[i];
                if (character is '"') continue;

                if (component == 1 && rStart == 0)
                {
                    rStart = i;
                }
                else if (component == 2 && gStart == 0)
                {
                    gStart = i;
                }
                else if (component == 3 && bStart == 0)
                {
                    bStart = i;
                }
                else if (component == 4 && aStart == 0)
                {
                    aStart = i;
                }
                fixed (char* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, ',', ')');
                    if (component == 1)
                    {
                        r = SpanConverter.ToFloat(span.Slice(rStart, length));
                    }
                    else if (component == 2)
                    {
                        g = SpanConverter.ToFloat(span.Slice(gStart, length));
                    }
                    else if (component == 3)
                    {
                        b = SpanConverter.ToFloat(span.Slice(bStart, length));
                    }
                    else if (component == 4)
                    {
                        a = SpanConverter.ToFloat(span.Slice(aStart, length));
                    }
                    else
                    {
                        throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Color(r, g, b, a);
        }
    }

    /// <summary>
    /// 十六进制形式的 Color Json 格式化器
    /// </summary>
    /// <example>
    /// <code>
    /// "#7F7F7F7F"
    /// </code>
    /// </example>
    public sealed unsafe class HexColorFormatter : IFixedSerdeJsonFormatter<Color>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 11; // 9 + 2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 11; // 9 + 2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SerializeUtf8Unsafe(Color value, byte* dest)
        {
            dest[0] = (byte) '"';
            dest[1] = (byte) '#';
            dest[10] = (byte) '"';
            var hexChars = new Span<byte>(dest + 1, 9);

            var r = (byte) (value.r * 255);
            var g = (byte) (value.g * 255);
            var b = (byte) (value.b * 255);
            var a = (byte) (value.a * 255);

            ByteToHexChars(r, hexChars.Slice(1, 2));
            ByteToHexChars(g, hexChars.Slice(3, 2));
            ByteToHexChars(b, hexChars.Slice(5, 2));
            ByteToHexChars(a, hexChars.Slice(7, 2));
            return 11;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void ByteToHexChars(byte b, Span<byte> chars)
            {
                chars[0] = HexChar(b >> 4);
                chars[1] = HexChar(b & 0xF);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static byte HexChar(int value) => (byte) (value < 10 ? '0' + value : 'A' + (value - 10));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SerializeUtf16Unsafe(Color value, char* dest)
        {
            dest[0] = '"';
            dest[1] = '#';
            dest[10] = '"';
            var hexChars = new Span<char>(dest + 1, 9);

            var r = (byte) (value.r * 255);
            var g = (byte) (value.g * 255);
            var b = (byte) (value.b * 255);
            var a = (byte) (value.a * 255);

            ByteToHexChars(r, hexChars.Slice(1, 2));
            ByteToHexChars(g, hexChars.Slice(3, 2));
            ByteToHexChars(b, hexChars.Slice(5, 2));
            ByteToHexChars(a, hexChars.Slice(7, 2));
            return 11;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void ByteToHexChars(byte b, Span<char> chars)
            {
                chars[0] = HexChar(b >> 4);
                chars[1] = HexChar(b & 0xF);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static char HexChar(int value) => (char) (value < 10 ? '0' + value : 'A' + (value - 10));
        }

        public Color DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var span = new ReadOnlySpan<byte>(src + 1, 9);
            if (span[0] != '#' || src[0] != (byte) '"' || src[10] != (byte) '"')
            {
                throw SerdeException.ThrowInvalidParsing();
            }
            span = span.Slice(1, span.Length - 1);
#if NET8_0_OR_GREATER
            return HexToColor(int.Parse(span, NumberStyles.HexNumber));
#else
            // Utf8 Hex 转 int
            var hex = 0;
            for (var i = span.Length - 1; i >= 0; i--)
            {
                hex += (int) (Hex2Int(span[i]) * Mathf.Pow(16, span.Length - 1 - i));
            }
            advance = 11;
            return HexToColor(hex);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int Hex2Int(byte ch)
            {
                if (ch >= (byte) '0' && ch <= (byte) '9')
                    return ch - '0';
                if (ch >= (byte) 'A' && ch <= (byte) 'F')
                    return ch - (byte) 'A' + 10;
                if (ch >= (byte) 'a' && ch <= (byte) 'f')
                    return ch - (byte) 'a' + 10;
                throw SerdeException.ThrowInvalidParsing();
            }
#endif
        }

        public Color DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var span = new ReadOnlySpan<char>(src + 1, 9);
            if (span[0] != '#' || src[0] != '"' || src[10] != '"')
            {
                throw SerdeException.ThrowInvalidParsing();
            }
            span = span.Slice(1, span.Length - 1);
            advance = 11;
            return HexToColor(int.Parse(span, NumberStyles.HexNumber));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Color HexToColor(int hex)
        {
            var r = ((hex >> 24) & 0xFF) / 255f;
            var g = ((hex >> 16) & 0xFF) / 255f;
            var b = ((hex >> 8) & 0xFF) / 255f;
            var a = (hex & 0xFF) / 255f;
            return new Color(r, g, b, a);
        }
    }
}