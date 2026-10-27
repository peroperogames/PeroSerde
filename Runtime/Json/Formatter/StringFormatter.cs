/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 字符串 Json 格式化器
    /// </summary>
    [PublicAPI]
    public sealed class StringFormatter : ISerdeJsonFormatter<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(string value, ref JsonWriterUtf8 writer) => writer.WriteString(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(string value, ref JsonWriterUtf16 writer) => writer.WriteString(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string DeserializeUtf8(ref ReadOnlySpan<byte> span, ref JsonReaderUtf8 reader)
        {
            var remains = reader.remains;
            // 纯 ASCII 时直接一次分配成 string（含转义），避免中间 byte[] 分配
            if (remains.Length > 0 && remains[0] == (byte) '"')
            {
                unsafe
                {
                    fixed (byte* src = remains)
                    {
                        if (JsonReader.TryReadAsciiStringUtf8(src, remains.Length, out var asciiValue, out var advance))
                        {
                            reader.Advance(advance);
                            return asciiValue;
                        }
                    }
                }
            }

            var result = reader.ReadUntilEnd();
            if (SpanConverter.IsNull(result)) return null;
            if(remains[0] != (byte) '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
            return SpanConverter.IsNull(result) ? null : Encoding.UTF8.GetString(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string DeserializeUtf16(ref ReadOnlySpan<char> span, ref JsonReaderUtf16 reader)
        {
            var remains = reader.remains;
            // 直接一次分配成 string，同时支持字符串结束在缓冲区末尾的情况
            if (remains.Length > 0 && remains[0] == '"')
            {
                unsafe
                {
                    fixed (char* src = remains)
                    {
                        if (JsonReader.TryReadStringUtf16(src, remains.Length, out var value, out var advance))
                        {
                            reader.Advance(advance);
                            return value;
                        }
                    }
                }
            }

            var result = reader.ReadUntilEnd();
            if (SpanConverter.IsNull(result)) return null;
            if(remains[0] != '"') throw new SerdeException("Invalid json format. Expect '\"' at the beginning of string.");
            return result.ToString();
        }
    }
}