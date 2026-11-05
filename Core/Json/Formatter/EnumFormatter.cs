/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/05
 * Description: EnumFormatter
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PeroTools2.Serde.Json;

/// <summary>
/// Static enum information
/// </summary>
internal static class EnumInfo<T> where T : unmanaged, Enum
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Dictionary<T, (ulong value, string name)> s_Items;
    private static readonly Dictionary<string, T> s_Names;

    /// <summary>TypeCode of the Enum underlying type</summary>
    internal static readonly TypeCode typeCode = Type.GetTypeCode(typeof(T));

    internal static readonly int maxUTF16StringLength;
    internal static readonly int maxUTF8StringLength;

    static EnumInfo()
    {
        var values = (T[]) Enum.GetValues(typeof(T));
        s_Items = new Dictionary<T, (ulong, string)>(values.Length);
        s_Names = new Dictionary<string, T>(values.Length);
        var utf8MaxLength = 0;
        var utf16MaxLength = 0;
        for (var i = 0; i < values.Length; i++)
        {
            var item = values[i];
            var value = EnumHelper.AsUInt64(item);
            var name = Enum.GetName(typeof(T), item)!;
            s_Items[item] = (value, name);
            s_Names[name] = item;
            utf16MaxLength = Math.Max(name.Length, utf16MaxLength);
            utf8MaxLength = Math.Max(Encoding.UTF8.GetMaxByteCount(name.Length), utf8MaxLength);
        }
        maxUTF8StringLength = utf8MaxLength;
        maxUTF16StringLength = utf16MaxLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T GetItem(string name) => s_Names[name];

    internal static T GetItem(ReadOnlySpan<byte> name)
    {
        foreach (var kv in s_Names)
        {
            if (StringHelper.EqualsUnaligned(name, kv.Key))
                return kv.Value;
        }
        return default;
    }

    internal static T GetItem(ReadOnlySpan<char> name)
    {
        foreach (var kv in s_Names)
        {
            if (StringHelper.Equals(name, kv.Key))
                return kv.Value;
        }
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string GetName(T item) => s_Items[item].name;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong GetValue(T item) => s_Items[item].value;
}

/// <summary>
/// Enum helper class
/// </summary>
public static class EnumHelper
{
    /// <summary>Converts an enum value to ulong.</summary>
    /// <remarks>Both signed and unsigned integer types are supported.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong AsUInt64<T>(T value) where T : unmanaged, Enum =>
        Unsafe.SizeOf<T>() switch
        {
            1 => Unsafe.As<T, byte>(ref value),
            2 => Unsafe.As<T, ushort>(ref value),
            4 => Unsafe.As<T, uint>(ref value),
            _ => Unsafe.As<T, ulong>(ref value)
        };

    /// <summary>Converts a ulong value to an enum of the specified type</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AsEnum<T>(ulong value) where T : unmanaged, Enum
    {
        return Unsafe.SizeOf<T>() switch
        {
            1 => UnsafeAsT((byte) value),
            2 => UnsafeAsT((ushort) value),
            4 => UnsafeAsT((uint) value),
            _ => UnsafeAsT(value)
        };

        // Works around the requirement that value be passed by ref
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T UnsafeAsT<TFrom>(TFrom value) => Unsafe.As<TFrom, T>(ref value);
    }

    /// <summary>Converts a long value to an enum of the specified type</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AsEnum<T>(long value) where T : unmanaged, Enum
    {
        return Unsafe.SizeOf<T>() switch
        {
            1 => UnsafeAsT((sbyte) value),
            2 => UnsafeAsT((short) value),
            4 => UnsafeAsT((int) value),
            _ => UnsafeAsT(value)
        };

        // Works around the requirement that value be passed by ref
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T UnsafeAsT<TFrom>(TFrom value) => Unsafe.As<TFrom, T>(ref value);
    }
}

/// <summary>
/// Enum formatter for Number values
/// </summary>
/// <typeparam name="T">Enum type</typeparam>
public class EnumNumberFormatter<T> : IFixedSerdeJsonFormatter<T> where T : unmanaged, Enum
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf8() =>
        Unsafe.SizeOf<T>() switch
        {
            1 => 4,
            2 => 6,
            4 => 11,
            _ => UlongFormatter.max_size
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf16() => Unsafe.SizeOf<T>() switch
    {
        1 => 4,
        2 => 6,
        4 => 11,
        _ => UlongFormatter.max_size
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf8Unsafe(T value, byte* dest) => EnumInfo<T>.typeCode switch
    {
        TypeCode.SByte => JsonWriter.WriteLongUtf8(dest, Unsafe.As<T, sbyte>(ref value)),
        TypeCode.Byte => JsonWriter.WriteUlongUtf8(dest, Unsafe.As<T, byte>(ref value)),
        TypeCode.Int16 => JsonWriter.WriteLongUtf8(dest, Unsafe.As<T, short>(ref value)),
        TypeCode.UInt16 => JsonWriter.WriteUlongUtf8(dest, Unsafe.As<T, ushort>(ref value)),
        TypeCode.Int32 => JsonWriter.WriteLongUtf8(dest, Unsafe.As<T, int>(ref value)),
        TypeCode.UInt32 => JsonWriter.WriteUlongUtf8(dest, Unsafe.As<T, uint>(ref value)),
        TypeCode.Int64 => JsonWriter.WriteLongUtf8(dest, Unsafe.As<T, long>(ref value)),
        TypeCode.UInt64 => JsonWriter.WriteUlongUtf8(dest, Unsafe.As<T, ulong>(ref value)),
        _ => throw new ArgumentException($"Enum {typeof(T).Name}'s UnderlyingType {Enum.GetUnderlyingType(typeof(T)).Name} is not support.")
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf16Unsafe(T value, char* dest) => EnumInfo<T>.typeCode switch
    {
        TypeCode.SByte => JsonWriter.WriteLongUtf16(dest, Unsafe.As<T, sbyte>(ref value)),
        TypeCode.Byte => JsonWriter.WriteUlongUtf16(dest, Unsafe.As<T, byte>(ref value)),
        TypeCode.Int16 => JsonWriter.WriteLongUtf16(dest, Unsafe.As<T, short>(ref value)),
        TypeCode.UInt16 => JsonWriter.WriteUlongUtf16(dest, Unsafe.As<T, ushort>(ref value)),
        TypeCode.Int32 => JsonWriter.WriteLongUtf16(dest, Unsafe.As<T, int>(ref value)),
        TypeCode.UInt32 => JsonWriter.WriteUlongUtf16(dest, Unsafe.As<T, uint>(ref value)),
        TypeCode.Int64 => JsonWriter.WriteLongUtf16(dest, Unsafe.As<T, long>(ref value)),
        TypeCode.UInt64 => JsonWriter.WriteUlongUtf16(dest, Unsafe.As<T, ulong>(ref value)),
        _ => throw new ArgumentException($"Enum {typeof(T).Name}'s UnderlyingType {Enum.GetUnderlyingType(typeof(T)).Name} is not support.")
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe T DeserializeUtf8Unsafe(byte* src, out int advance) => EnumInfo<T>.typeCode switch
    {
        TypeCode.SByte => EnumHelper.AsEnum<T>(JsonReader.ReadLongFastUtf8(src, out advance)),
        TypeCode.Byte => EnumHelper.AsEnum<T>(JsonReader.ReadUlongUtf8(src, out advance)),
        TypeCode.Int16 => EnumHelper.AsEnum<T>(JsonReader.ReadLongFastUtf8(src, out advance)),
        TypeCode.UInt16 => EnumHelper.AsEnum<T>(JsonReader.ReadUlongUtf8(src, out advance)),
        TypeCode.Int32 => EnumHelper.AsEnum<T>(JsonReader.ReadLongFastUtf8(src, out advance)),
        TypeCode.UInt32 => EnumHelper.AsEnum<T>(JsonReader.ReadUlongUtf8(src, out advance)),
        TypeCode.Int64 => EnumHelper.AsEnum<T>(JsonReader.ReadLongFastUtf8(src, out advance)),
        TypeCode.UInt64 => EnumHelper.AsEnum<T>(JsonReader.ReadUlongUtf8(src, out advance)),
        _ => throw new ArgumentException($"Enum {typeof(T).Name}'s UnderlyingType {Enum.GetUnderlyingType(typeof(T)).Name} is not support.")
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe T DeserializeUtf16Unsafe(char* src, out int advance) => EnumInfo<T>.typeCode switch
    {
        TypeCode.SByte => EnumHelper.AsEnum<T>(JsonReader.ReadLongFastUtf16(src, out advance)),
        TypeCode.Byte => EnumHelper.AsEnum<T>(JsonReader.ReadUlongUtf16(src, out advance)),
        TypeCode.Int16 => EnumHelper.AsEnum<T>(JsonReader.ReadLongFastUtf16(src, out advance)),
        TypeCode.UInt16 => EnumHelper.AsEnum<T>(JsonReader.ReadUlongUtf16(src, out advance)),
        TypeCode.Int32 => EnumHelper.AsEnum<T>(JsonReader.ReadLongFastUtf16(src, out advance)),
        TypeCode.UInt32 => EnumHelper.AsEnum<T>(JsonReader.ReadUlongUtf16(src, out advance)),
        TypeCode.Int64 => EnumHelper.AsEnum<T>(JsonReader.ReadLongFastUtf16(src, out advance)),
        TypeCode.UInt64 => EnumHelper.AsEnum<T>(JsonReader.ReadUlongUtf16(src, out advance)),
        _ => throw new ArgumentException($"Enum {typeof(T).Name}'s UnderlyingType {Enum.GetUnderlyingType(typeof(T)).Name} is not support.")
    };
}

/// <summary>
/// Enum formatter for String values
/// </summary>
/// <typeparam name="T">Enum type</typeparam>
public class EnumStringFormatter<T> : IFixedSerdeJsonFormatter<T> where T : unmanaged, Enum
{
    private static readonly int s_MaxValueUtf8 = EnumInfo<T>.maxUTF8StringLength + 2;
    private static readonly int s_MaxValueUtf16 = EnumInfo<T>.maxUTF16StringLength + 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf8() => s_MaxValueUtf8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetMaxRequiredSizeUtf16() => s_MaxValueUtf16;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf8Unsafe(T value, byte* dest)
    {
        var name = EnumInfo<T>.GetName(value);
        return JsonWriter.WriteStringNoEscapeUtf8(dest, name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int SerializeUtf16Unsafe(T value, char* dest)
    {
        var name = EnumInfo<T>.GetName(value);
        return JsonWriter.WriteStringNoEscapeUtf16(dest, name);
    }

    public unsafe T DeserializeUtf8Unsafe(byte* src, out int advance)
    {
        var value = JsonReader.ReadValueUntilEndUtf8(src, s_MaxValueUtf8, out advance);
        return EnumInfo<T>.GetItem(value);
    }

    public unsafe T DeserializeUtf16Unsafe(char* src, out int advance)
    {
        var value = JsonReader.ReadValueUntilEndUtf16(src, s_MaxValueUtf16, out advance);
        return EnumInfo<T>.GetItem(value);
    }
}