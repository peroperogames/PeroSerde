/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/12
 * Description: NullableStructFormatter
 */

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PeroTools2.Serde.Json
{
    /// <summary>
    /// 可空类型格式化器
    /// </summary>
    [PublicAPI]
    public sealed class NullableFormatter<T> : ISerdeJsonFormatter<T?> where T : struct
    {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
        {
            if (remains.Length >= 4 && SpanConverter.IsNull(remains.Slice(0, 4)))
            {
                reader.ReadValue();
                return null;
            }
            return reader.ReadValue<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
        {
            if (remains.Length >= 4 && SpanConverter.IsNull(remains.Slice(0, 4)))
            {
                reader.ReadValue();
                return null;
            }
            return reader.ReadValue<T>();
        }
    }
}