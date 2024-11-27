/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/12
 * Description: NullableStructFormatter
 */

using System;
using System.Collections.Generic;

namespace PeroTools2.Serde.Json.Formatter
{
    static partial class SerdeJsonFormatterProvider
    {
        private static Dictionary<Type, Type> s_BuiltInGenericTypeFormatters = new()
        {
            // TODO 对于IL2CPP, 自动生成注册泛型Formatter的代码来防止被裁剪
            [typeof(Nullable<>)] = typeof(NullableStructFormatter<>),
            [typeof(KeyValuePair<,>)] = typeof(KeyValuePairFormatter<,>),
            [typeof(Lazy<>)] = typeof(LazyFormatter<>),
        };
    }

    public sealed class NullableStructFormatter<T> : SerdeJsonFormatter<T?> where T : struct
    {
        public override bool isUnsafeFormatter => false;

        public override void SerializeUtf8<TBufferWriter>(T? value, ref JsonWriterUtf8<TBufferWriter> writer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.Value);
            }
        }

        public override void SerializeUtf16<TBufferWriter>(T? value, ref JsonWriterUtf16<TBufferWriter> writer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.Value);
            }
        }
    }

    public sealed class KeyValuePairFormatter<TK, TV> : SerdeJsonFormatter<KeyValuePair<TK, TV>>
    {
        public override bool isUnsafeFormatter => false;

        public override void SerializeUtf8<TBufferWriter>(KeyValuePair<TK, TV> value, ref JsonWriterUtf8<TBufferWriter> writer)
        {
            writer.WriteBeginObject();
            writer.WriteNamedValue("key", value.Key);
            writer.WriteNamedValue("value", value.Value, false);
            writer.WriteEndObject();
        }

        public override void SerializeUtf16<TBufferWriter>(KeyValuePair<TK, TV> value, ref JsonWriterUtf16<TBufferWriter> writer)
        {
            writer.WriteBeginObject();
            writer.WriteNamedValue("key", value.Key);
            writer.WriteNamedValue("value", value.Value, false);
            writer.WriteEndObject();
        }
    }

    public sealed class LazyFormatter<T> : SerdeJsonFormatter<Lazy<T>>
    {
        public override bool isUnsafeFormatter => false;

        public override void SerializeUtf8<TBufferWriter>(Lazy<T> value, ref JsonWriterUtf8<TBufferWriter> writer)
        {
            if (value.IsValueCreated)
            {
                writer.WriteValue(value.Value);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override void SerializeUtf16<TBufferWriter>(Lazy<T> value, ref JsonWriterUtf16<TBufferWriter> writer)
        {
            if (value.IsValueCreated)
            {
                writer.WriteValue(value.Value);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}