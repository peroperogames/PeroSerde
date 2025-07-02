/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/07/02-18:14:05
 */

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// Quaternion Json 格式化器
    /// </summary>
    /// <example>
    /// <code>
    /// "(12.1,32.45)"// 不带空格
    /// </code>
    /// </example>
    [PublicAPI]
    public sealed class QuaternionFormatter : ISerdeJsonFormatter<Quaternion>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(Quaternion value, ref JsonWriterUtf8 writer)
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
        public void SerializeUtf16(Quaternion value, ref JsonWriterUtf16 writer)
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

        public unsafe Quaternion DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
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
            return new Quaternion(x, y, z, w);
        }

        public unsafe Quaternion DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
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
            return new Quaternion(x, y, z, w);
        }
    }
}