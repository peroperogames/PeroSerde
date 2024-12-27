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

namespace PeroTools2.Serde.Json
{
    public static class EnumInfo<T> where T : unmanaged, Enum
    {
        // ReSharper disable once StaticMemberInGenericType
        public static readonly Dictionary<T, (ulong value, string name)> items;

        /// <summary>Enum的UnderlyingType的TypeCode</summary>
        public static readonly TypeCode typeCode = Type.GetTypeCode(typeof(T));

        public static readonly bool isFlags = typeof(T).IsDefined(typeof(FlagsAttribute), false);

        public static readonly int maxUTF16StringLength;
        public static readonly int maxUTF8StringLength;

        static EnumInfo()
        {
            var values = (T[]) Enum.GetValues(typeof(T));
            items = new Dictionary<T, (ulong, string)>(values.Length);
            var utf8MaxLength = 0;
            var utf16MaxLength = 0;
            for (var i = 0; i < values.Length; i++)
            {
                var value = EnumHelper.AsUInt64(values[i]);
                var name = Enum.GetName(typeof(T), values[i])!;
                items[values[i]] = (value, name);
                utf16MaxLength = Math.Max(name.Length, utf16MaxLength);
                utf8MaxLength = Math.Max(Encoding.UTF8.GetMaxByteCount(name.Length), utf8MaxLength);
            }
            maxUTF8StringLength = utf8MaxLength;
            maxUTF16StringLength = utf16MaxLength;
        }
    }

    public static class EnumHelper
    {
        /// <summary>将枚举值转换为ulong.</summary>
        /// <remarks>有符号整型和无符号整型都支持.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong AsUInt64<T>(T value) where T : unmanaged, Enum =>
            Unsafe.SizeOf<T>() switch
            {
                1 => Unsafe.As<T, byte>(ref value),
                2 => Unsafe.As<T, ushort>(ref value),
                4 => Unsafe.As<T, uint>(ref value),
                _ => Unsafe.As<T, ulong>(ref value)
            };

        /// <summary>将ulong值转为指定类型的枚举</summary>
        /// <remarks>有符号整型和无符号整型都支持.</remarks>
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

            // 消除掉必须以ref传递value的限制
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static T UnsafeAsT<TFrom>(TFrom value) => Unsafe.As<TFrom, T>(ref value);
        }
    }

    public sealed class EnumIntFormatter<T> : IFixedSerdeJsonFormatter<T> where T : unmanaged, Enum
    {
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() =>
            Unsafe.SizeOf<T>() switch
            {
                1 => 4,
                2 => 6,
                4 => 11,
                _ => NumberHelper.max_uint64_numbers_length
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() =>
            Unsafe.SizeOf<T>() switch
            {
                1 => 4,
                2 => 6,
                4 => 11,
                _ => NumberHelper.max_uint64_numbers_length
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(T value, byte* dest) =>
            EnumInfo<T>.typeCode switch
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

        public unsafe T DeserializeUtf8Unsafe(byte* src, out int advance) => throw new NotImplementedException();

        public unsafe T DeserializeUtf16Unsafe(char* span, out int advance) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(T value, char* dest)
        {
            return EnumInfo<T>.typeCode switch
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
        }
    }

    public sealed class EnumStringFormatter<T> : IFixedSerdeJsonFormatter<T> where T : unmanaged, Enum
    {
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => EnumInfo<T>.maxUTF8StringLength + 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => EnumInfo<T>.maxUTF16StringLength + 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(T value, byte* dest)
        {
            var name = EnumInfo<T>.items[value].name;
            return JsonWriter.WriteStringNoEscapeUtf8(dest, name);
        }

        public unsafe T DeserializeUtf8Unsafe(byte* src, out int advance) => throw new NotImplementedException();

        public unsafe T DeserializeUtf16Unsafe(char* span, out int advance) => throw new NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(T value, char* dest)
        {
            var name = EnumInfo<T>.items[value].name;
            return JsonWriter.WriteStringNoEscapeUtf16(dest, name);
        }
    }
}