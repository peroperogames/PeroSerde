/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/27-18:49:38
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PeroTools2.Serde.Json;

/// <summary>
/// Json formatter for arrays
/// </summary>
/// <typeparam name="T">Element type</typeparam>
[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public sealed class ArrayFormatter<T> : ISerdeJsonFormatter<T[]>
{
    private static IFixedSerdeJsonFormatter<T> s_FixedFormatter;
    private static ISerdeJsonFormatter<T> s_UnfixedFormatter;
    private static bool s_IsFixed;
    private static bool s_Resolved;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureResolved()
    {
        // Resolved on first use: the element formatter can be registered after this formatter, which happens when a
        // generated registrar runs before the registrations of the element types
        if (s_Resolved)
        {
            return;
        }
        Resolve();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Resolve()
    {
        var formatter = SerdeJsonFormatterProvider.GetFormatterInternal<T>();
        if (formatter is IFixedSerdeJsonFormatter<T> fixedFormatter)
        {
            s_IsFixed = true;
            s_FixedFormatter = fixedFormatter;
        }
        else if (formatter is ISerdeJsonFormatter<T> unfixedFormatter)
        {
            s_UnfixedFormatter = unfixedFormatter;
        }
        Volatile.Write(ref s_Resolved, true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeUtf8(T[] value, ref JsonWriterUtf8 writer)
    {
        EnsureResolved();
        if (s_IsFixed)
        {
            writer.WriteCollection(value, s_FixedFormatter);
        }
        else
        {
            writer.WriteCollection(value, s_UnfixedFormatter);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeUtf16(T[] value, ref JsonWriterUtf16 writer)
    {
        EnsureResolved();
        if (s_IsFixed)
        {
            writer.WriteCollection(value, s_FixedFormatter);
        }
        else
        {
            writer.WriteCollection(value, s_UnfixedFormatter);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
    {
        EnsureResolved();
        if (reader.TryReadNull())
        {
            return null;
        }
        var list = ListPool<T>.shared.Rent();
        try
        {
            if (s_IsFixed)
            {
                reader.ReadCollection(list, s_FixedFormatter, false);
            }
            else
            {
                reader.ReadCollection(list, s_UnfixedFormatter, false);
            }

            return list?.ToArray();
        }
        finally
        {
            ListPool<T>.shared.Return(list);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
    {
        EnsureResolved();
        if (reader.TryReadNull())
        {
            return null;
        }
        var list = ListPool<T>.shared.Rent();
        try
        {
            if (s_IsFixed)
            {
                reader.ReadCollection(list, s_FixedFormatter, false);
            }
            else
            {
                reader.ReadCollection(list, s_UnfixedFormatter, false);
            }

            return list?.ToArray();
        }
        finally
        {
            ListPool<T>.shared.Return(list);
        }
    }
}