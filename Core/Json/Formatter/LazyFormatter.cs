/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/07-14:54:53
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

/// <summary>
/// Formatter for lazy types
/// </summary>
public sealed class LazyFormatter<T> : ISerdeJsonFormatter<Lazy<T>>
{
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Lazy<T> DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader) => new(reader.ReadValue<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Lazy<T> DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader) => new(reader.ReadValue<T>());
}