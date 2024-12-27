/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/12
 * Description: NullableStructFormatter
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    public sealed class NullableStructFormatter<T> : ISerdeJsonFormatter<T?> where T : struct
    {
        public JsonType jsonType => SerdeJsonFormatterProvider.GetFormatter<T>().jsonType;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(T? value, ref JsonWriterUtf8 writer)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(T? value, ref JsonWriterUtf16 writer)
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

    public sealed class KeyValuePairFormatter<TK, TV> : ISerdeJsonFormatter<KeyValuePair<TK, TV>>
    {
        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

        public JsonType jsonType => JsonType.Object;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(KeyValuePair<TK, TV> value, ref JsonWriterUtf8 writer)
        {
            writer.WriteBeginObject();
            writer.WriteNamedValue(value.Key.ToString(), value.Value, false);
            writer.WriteEndObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(KeyValuePair<TK, TV> value, ref JsonWriterUtf16 writer)
        {
            writer.WriteBeginObject();
            writer.WriteNamedValue(value.Key.ToString(), value.Value, false);
            writer.WriteEndObject();
        }
    }

    public sealed class LazyFormatter<T> : ISerdeJsonFormatter<Lazy<T>>
    {
        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false;
        }

        public JsonType jsonType => SerdeJsonFormatterProvider.GetFormatter<T>().jsonType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf8(Lazy<T> value, ref JsonWriterUtf8 writer)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUtf16(Lazy<T> value, ref JsonWriterUtf16 writer)
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