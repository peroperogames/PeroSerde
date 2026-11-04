/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23
 */

using System;
using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json;

/// <summary>
/// String Json formatter
/// </summary>
public sealed class StringFormatter : ISerdeJsonFormatter<string>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeUtf8(string value, ref JsonWriterUtf8 writer) => writer.WriteString(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeUtf16(string value, ref JsonWriterUtf16 writer) => writer.WriteString(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe string DeserializeUtf8(ref ReadOnlySpan<byte> span, ref JsonReaderUtf8 reader)
    {
        var remains = reader.remains;
        fixed (byte* src = remains)
        {
            // Shares the same string parsing entry point as the cursor path
            var value = JsonReader.ReadStringValueUtf8(src, remains.Length, out var advance);
            reader.Advance(advance);
            return value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe string DeserializeUtf16(ref ReadOnlySpan<char> span, ref JsonReaderUtf16 reader)
    {
        var remains = reader.remains;
        fixed (char* src = remains)
        {
            // Shares the same string parsing entry point as the cursor path
            var value = JsonReader.ReadStringValueUtf16(src, remains.Length, out var advance);
            reader.Advance(advance);
            return value;
        }
    }
}