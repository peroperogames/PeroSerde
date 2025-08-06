/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// Vector2 Json 格式化器
    /// </summary>
    /// <example>
    /// <code>
    /// "(12.1,32.45)"// 不带空格
    /// </code>
    /// </example>
    [PublicAPI]
    public sealed class Vector2Formatter : ISerdeJsonFormatter<Vector2>
    {
        public void SerializeUtf8(Vector2 value, ref JsonWriterUtf8 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteAscii('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteAscii(')');
            writer.WriteDoubleQuote();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(Vector2 value, ref JsonWriterUtf16 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteChar('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteChar(')');
            writer.WriteDoubleQuote();
        }

        public unsafe Vector2 DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0f;
            var y = 0f;
            var xStart = 0;
            var yStart = 0;
            var component = 1;
            if (span[0] != (byte) '(' || span[span.Length - 1] != (byte) ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = (char) span[i];
                if (character is '(' or '"' || StringHelper.IsWhiteSpace(character)) continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                fixed (byte* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, (byte) ',', (byte) ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToFloat(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToFloat(span.Slice(yStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Vector2(x, y);
        }

        public unsafe Vector2 DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0f;
            var y = 0f;
            var xStart = 0;
            var yStart = 0;
            var component = 1;
            if (span[0] != '(' || span[span.Length - 1] != ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = span[i];
                if (character is '"') continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                fixed (char* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, ',', ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToFloat(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToFloat(span.Slice(yStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Vector2(x, y);
        }
    }

    /// <summary>
    /// Vector3 Json 格式化器
    /// </summary>
    /// <example>
    /// <code>
    /// "(12.1,32.45,0.5)"// 不带空格
    /// </code>
    /// </example>
    [PublicAPI]
    public sealed class Vector3Formatter : ISerdeJsonFormatter<Vector3>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(Vector3 value, ref JsonWriterUtf8 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteAscii('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteValueEnding();
            writer.WriteValue(value.z);
            writer.WriteAscii(')');
            writer.WriteDoubleQuote();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(Vector3 value, ref JsonWriterUtf16 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteChar('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteValueEnding();
            writer.WriteValue(value.z);
            writer.WriteChar(')');
            writer.WriteDoubleQuote();
        }

        public unsafe Vector3 DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0f;
            var y = 0f;
            var z = 0f;
            var xStart = 0;
            var yStart = 0;
            var zStart = 0;
            var component = 1;
            if (span[0] != (byte) '(' || span[span.Length - 1] != (byte) ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = (char) span[i];
                if (character is '(' or '"' || StringHelper.IsWhiteSpace(character)) continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                else if (component == 3 && zStart == 0)
                {
                    zStart = i;
                }
                fixed (byte* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, (byte) ',', (byte) ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToFloat(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToFloat(span.Slice(yStart, length));
                            break;
                        case 3:
                            z = SpanConverter.ToFloat(span.Slice(zStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Vector3(x, y, z);
        }

        public unsafe Vector3 DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0f;
            var y = 0f;
            var z = 0f;
            var xStart = 0;
            var yStart = 0;
            var zStart = 0;
            var component = 1;
            if (span[0] != '(' || span[span.Length - 1] != ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = span[i];
                if (character is '"') continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                else if (component == 3 && zStart == 0)
                {
                    zStart = i;
                }
                fixed (char* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, ',', ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToFloat(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToFloat(span.Slice(yStart, length));
                            break;
                        case 3:
                            z = SpanConverter.ToFloat(span.Slice(zStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Vector3(x, y, z);
        }
    }

    /// <summary>
    /// Vector4 Json 格式化器
    /// </summary>
    /// <example>
    /// <code>
    /// "(12.1,32.45,0.5,1.23)"// 不带空格
    /// </code>
    /// </example>
    [PublicAPI]
    public sealed class Vector4Formatter : ISerdeJsonFormatter<Vector4>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(Vector4 value, ref JsonWriterUtf8 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteAscii('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteValueEnding();
            writer.WriteValue(value.z);
            writer.WriteValueEnding();
            writer.WriteValue(value.w);
            writer.WriteAscii(')');
            writer.WriteDoubleQuote();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(Vector4 value, ref JsonWriterUtf16 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteChar('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteValueEnding();
            writer.WriteValue(value.z);
            writer.WriteValueEnding();
            writer.WriteValue(value.w);
            writer.WriteChar(')');
            writer.WriteDoubleQuote();
        }

        public unsafe Vector4 DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0f;
            var y = 0f;
            var z = 0f;
            var w = 0f;
            var xStart = 0;
            var yStart = 0;
            var zStart = 0;
            var wStart = 0;
            var component = 1;
            if (span[0] != (byte) '(' || span[span.Length - 1] != (byte) ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = (char) span[i];
                if (character is '(' or '"' || StringHelper.IsWhiteSpace(character)) continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                else if (component == 3 && zStart == 0)
                {
                    zStart = i;
                }
                else if (component == 4 && wStart == 0)
                {
                    wStart = i;
                }
                fixed (byte* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, (byte) ',', (byte) ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToFloat(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToFloat(span.Slice(yStart, length));
                            break;
                        case 3:
                            z = SpanConverter.ToFloat(span.Slice(zStart, length));
                            break;
                        case 4:
                            w = SpanConverter.ToFloat(span.Slice(wStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }
                    i += length;
                    component++;
                }
            }
            return new Vector4(x, y, z, w);
        }

        public unsafe Vector4 DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0f;
            var y = 0f;
            var z = 0f;
            var w = 0f;
            var xStart = 0;
            var yStart = 0;
            var zStart = 0;
            var wStart = 0;
            var component = 1;
            if (span[0] != '(' || span[span.Length - 1] != ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = span[i];
                if (character is '"') continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                else if (component == 3 && zStart == 0)
                {
                    zStart = i;
                }
                else if (component == 4 && wStart == 0)
                {
                    wStart = i;
                }
                fixed (char* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, ',', ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToFloat(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToFloat(span.Slice(yStart, length));
                            break;
                        case 3:
                            z = SpanConverter.ToFloat(span.Slice(zStart, length));
                            break;
                        case 4:
                            w = SpanConverter.ToFloat(span.Slice(wStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Vector4(x, y, z, w);
        }
    }

    /// <summary>
    /// Vector2Int Json 格式化器
    /// </summary>
    /// <example>
    /// <code>
    /// "(12,34)"// 不带空格
    /// </code>
    /// </example>
    [PublicAPI]
    public sealed class Vector2IntFormatter : ISerdeJsonFormatter<Vector2Int>
    {
        public void SerializeUtf8(Vector2Int value, ref JsonWriterUtf8 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteAscii('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteAscii(')');
            writer.WriteDoubleQuote();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(Vector2Int value, ref JsonWriterUtf16 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteChar('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteChar(')');
            writer.WriteDoubleQuote();
        }

        public unsafe Vector2Int DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0;
            var y = 0;
            var xStart = 0;
            var yStart = 0;
            var component = 1;
            if (span[0] != (byte) '(' || span[span.Length - 1] != (byte) ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = (char) span[i];
                if (character is '(' or '"' || StringHelper.IsWhiteSpace(character)) continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                fixed (byte* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, (byte) ',', (byte) ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToInt(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToInt(span.Slice(yStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Vector2Int(x, y);
        }

        public unsafe Vector2Int DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0;
            var y = 0;
            var xStart = 0;
            var yStart = 0;
            var component = 1;
            if (span[0] != '(' || span[span.Length - 1] != ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = span[i];
                if (character is '"') continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                fixed (char* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, ',', ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToInt(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToInt(span.Slice(yStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Vector2Int(x, y);
        }
    }

    /// <summary>
    /// Vector3Int Json 格式化器
    /// </summary>
    /// <example>
    /// <code>
    /// "(12,34,56)"// 不带空格
    /// </code>
    /// </example>
    [PublicAPI]
    public sealed class Vector3IntFormatter : ISerdeJsonFormatter<Vector3Int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(Vector3Int value, ref JsonWriterUtf8 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteAscii('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteValueEnding();
            writer.WriteValue(value.z);
            writer.WriteAscii(')');
            writer.WriteDoubleQuote();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(Vector3Int value, ref JsonWriterUtf16 writer)
        {
            writer.WriteDoubleQuote();
            writer.WriteChar('(');
            writer.WriteValue(value.x);
            writer.WriteValueEnding();
            writer.WriteValue(value.y);
            writer.WriteValueEnding();
            writer.WriteValue(value.z);
            writer.WriteChar(')');
            writer.WriteDoubleQuote();
        }

        public unsafe Vector3Int DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0;
            var y = 0;
            var z = 0;
            var xStart = 0;
            var yStart = 0;
            var zStart = 0;
            var component = 1;
            if (span[0] != (byte) '(' || span[span.Length - 1] != (byte) ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = (char) span[i];
                if (character is '(' or '"' || StringHelper.IsWhiteSpace(character)) continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                else if (component == 3 && zStart == 0)
                {
                    zStart = i;
                }
                fixed (byte* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, (byte) ',', (byte) ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToInt(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToInt(span.Slice(yStart, length));
                            break;
                        case 3:
                            z = SpanConverter.ToInt(span.Slice(zStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Vector3Int(x, y, z);
        }

        public unsafe Vector3Int DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            var span = reader.ReadUntilEnd();
            var x = 0;
            var y = 0;
            var z = 0;
            var xStart = 0;
            var yStart = 0;
            var zStart = 0;
            var component = 1;
            if (span[0] != '(' || span[span.Length - 1] != ')')
            {
                throw new Exception("Parse error");
            }
            for (var i = 1; i < span.Length; i++)
            {
                var character = span[i];
                if (character is '"') continue;

                if (component == 1 && xStart == 0)
                {
                    xStart = i;
                }
                else if (component == 2 && yStart == 0)
                {
                    yStart = i;
                }
                else if (component == 3 && zStart == 0)
                {
                    zStart = i;
                }
                fixed (char* ptr = span.Slice(i, span.Length - 1 - i))
                {
                    var length = StringHelper.IndexOfAny(ptr, span.Length, ',', ')');
                    switch (component)
                    {
                        case 1:
                            x = SpanConverter.ToInt(span.Slice(xStart, length));
                            break;
                        case 2:
                            y = SpanConverter.ToInt(span.Slice(yStart, length));
                            break;
                        case 3:
                            z = SpanConverter.ToInt(span.Slice(zStart, length));
                            break;
                        default:
                            throw SerdeException.ThrowInvalidParsing();
                    }

                    i += length;
                    component++;
                }
            }
            return new Vector3Int(x, y, z);
        }
    }
}