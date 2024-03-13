/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/07
 * Description: ArrayFormatter
 */

using System;

namespace PeroTools2.Serde.Json.Formatter
{
    public sealed class ArrayFormatter<T> : SerdeJsonFormatter<T[]>
    {
        public override bool isUnsafeFormatter => false;
        public override void SerializeUtf8<TBufferWriter>(T[] value, ref JsonWriterUtf8<TBufferWriter> writer) => writer.WriteArray(value);
        public override void SerializeUtf16<TBufferWriter>(T[] value, ref JsonWriterUtf16<TBufferWriter> writer) => writer.WriteArray(value);
    }

    /// <summary>以Base64字符串的方式读写byte[]</summary>
    public sealed class ByteArrayFormatter : SerdeJsonFormatter<byte[]>
    {
        public override bool isUnsafeFormatter => false;

        public override void SerializeUtf8<TBufferWriter>(byte[] value, ref JsonWriterUtf8<TBufferWriter> writer)
        {
            var base64Str = Convert.ToBase64String(value, Base64FormattingOptions.None);
            writer.WriteValue(base64Str);
        }

        public override void SerializeUtf16<TBufferWriter>(byte[] value, ref JsonWriterUtf16<TBufferWriter> writer)
        {
            var base64Str = Convert.ToBase64String(value, Base64FormattingOptions.None);
            writer.WriteValue(base64Str);
        }
    }
}