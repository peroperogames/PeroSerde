/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 字符串 Json 格式化器
    /// </summary>
    public sealed class StringFormatter : ISerdeJsonFormatter<string>
    {
        public JsonType jsonType => JsonType.Value;
        public bool isFixedFormatter => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(string value, ref JsonWriterUtf8 writer) => writer.WriteString(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(string value, ref JsonWriterUtf16 writer) => writer.WriteString(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string DeserializeUtf8(ref ReadOnlySpan<byte> span, ref JsonReaderUtf8 reader)
        {
            var result = reader.ReadUntilEnd();
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.IsNull(result) ? null : Encoding.UTF8.GetString(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string DeserializeUtf16(ref ReadOnlySpan<char> span, ref JsonReaderUtf16 reader)
        {
            var result = reader.ReadUntilEnd();
            return SpanConverter.IsNull(result) ? null : result.ToString();
        }
    }
}