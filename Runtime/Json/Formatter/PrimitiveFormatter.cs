/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/08
 * Description: PrimitiveFormatter
 */

using System.Runtime.CompilerServices;

namespace PeroTools2.Serde.Json
{
    static partial class SerdeJsonFormatterProvider
    {
        private static void RegisterPrimitiveFormatters()
        {
            Register(new BoolFormatter());
            Register(new NullableBoolFormatter());
            Register(new ByteFormatter());
            Register(new NullableByteFormatter());
            Register(new SbyteFormatter());
            Register(new NullableSbyteFormatter());
            Register(new UshortFormatter());
            Register(new NullableUshortFormatter());
            Register(new ShortFormatter());
            Register(new NullableShortFormatter());
            Register(new UintFormatter());
            Register(new NullableUintFormatter());
            Register(new IntFormatter());
            Register(new NullableIntFormatter());
            Register(new UlongFormatter());
            Register(new NullableUlongFormatter());
            Register(new LongFormatter());
            Register(new NullableLongFormatter());
            Register(new FloatFormatter());
            Register(new NullableFloatFormatter());
            Register(new DoubleFormatter());
            Register(new NullableDoubleFormatter());
            Register(new DecimalFormatter());
            Register(new NullableDecimalFormatter());
            Register(new CharFormatter());
            Register(new NullableCharFormatter());
        }
    }

    public sealed class BoolFormatter : IFixedSerdeJsonFormatter<bool>
    {
        public const int max_size = 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(bool value, byte* dest) => JsonWriter.WriteBoolUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(bool value, char* dest) => JsonWriter.WriteBoolUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadBoolUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadBoolUtf16(src, out advance);
    }

    public sealed class NullableBoolFormatter : IFixedSerdeJsonFormatter<bool?>
    {
        public const int max_size = 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(bool? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteBoolUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(bool? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteBoolUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableBoolUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableBoolUtf16(src, out advance);
    }

    public sealed class ByteFormatter : IFixedSerdeJsonFormatter<byte>
    {
        public const int max_size = 3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(byte value, byte* dest) => JsonWriter.WriteUlongUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(byte value, char* dest) => JsonWriter.WriteUlongUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe byte DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadByteUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe byte DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadByteUtf16(src, out advance);
    }

    public sealed class NullableByteFormatter : IFixedSerdeJsonFormatter<byte?>
    {
        public const int max_size = 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(byte? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteUlongUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(byte? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteUlongUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe byte? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableByteUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe byte? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableByteUtf16(src, out advance);
    }

    public sealed class SbyteFormatter : IFixedSerdeJsonFormatter<sbyte>
    {
        public const int max_size = 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(sbyte value, byte* dest) => JsonWriter.WriteLongUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(sbyte value, char* dest) => JsonWriter.WriteLongUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe sbyte DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadSbyteUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe sbyte DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadSbyteUtf16(src, out advance);
    }

    public sealed class NullableSbyteFormatter : IFixedSerdeJsonFormatter<sbyte?>
    {
        public const int max_size = 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(sbyte? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteLongUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(sbyte? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteLongUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe sbyte? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableSbyteUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe sbyte? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableSbyteUtf16(src, out advance);
    }

    public sealed class UshortFormatter : IFixedSerdeJsonFormatter<ushort>
    {
        public const int max_size = 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(ushort value, byte* dest) => JsonWriter.WriteUlongUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(ushort value, char* dest) => JsonWriter.WriteUlongUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ushort DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadUshortUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ushort DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadUshortUtf16(src, out advance);
    }

    public sealed class NullableUshortFormatter : IFixedSerdeJsonFormatter<ushort?>
    {
        public const int max_size = 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(ushort? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteUlongUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(ushort? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteUlongUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ushort? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableUshortUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ushort? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableUshortUtf16(src, out advance);
    }

    public sealed class ShortFormatter : IFixedSerdeJsonFormatter<short>
    {
        public const int max_size = 6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(short value, byte* dest) => JsonWriter.WriteLongUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(short value, char* dest) => JsonWriter.WriteLongUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe short DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadShortUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe short DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadShortUtf16(src, out advance);
    }

    public sealed class NullableShortFormatter : IFixedSerdeJsonFormatter<short?>
    {
        public const int max_size = 6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(short? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteLongUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(short? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteLongUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe short? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableShortUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe short? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableShortUtf16(src, out advance);
    }

    public sealed class UintFormatter : IFixedSerdeJsonFormatter<uint>
    {
        public const int max_size = 10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(uint value, byte* dest) => JsonWriter.WriteUlongUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(uint value, char* dest) => JsonWriter.WriteUlongUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe uint DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadUintUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe uint DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadUintUtf16(src, out advance);
    }

    public sealed class NullableUintFormatter : IFixedSerdeJsonFormatter<uint?>
    {
        public const int max_size = 10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(uint? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteUlongUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(uint? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteUlongUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe uint? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableUintUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe uint? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableUintUtf16(src, out advance);
    }

    public sealed class IntFormatter : IFixedSerdeJsonFormatter<int>
    {
        public const int max_size = 11;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(int value, byte* dest) => JsonWriter.WriteLongUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(int value, char* dest) => JsonWriter.WriteLongUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadIntUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadIntUtf16(src, out advance);
    }

    public sealed class NullableIntFormatter : IFixedSerdeJsonFormatter<int?>
    {
        public const int max_size = 11;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(int? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteLongUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(int? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteLongUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableIntUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableIntUtf16(src, out advance);
    }

    public sealed class UlongFormatter : IFixedSerdeJsonFormatter<ulong>
    {
        public const int max_size = 20;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(ulong value, byte* dest) => JsonWriter.WriteUlongUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(ulong value, char* dest) => JsonWriter.WriteUlongUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ulong DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadUlongUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ulong DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadUlongUtf16(src, out advance);
    }

    public sealed class NullableUlongFormatter : IFixedSerdeJsonFormatter<ulong?>
    {
        public const int max_size = 20;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(ulong? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteUlongUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(ulong? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteUlongUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ulong? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableUlongUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ulong? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableUlongUtf16(src, out advance);
    }

    public sealed class LongFormatter : IFixedSerdeJsonFormatter<long>
    {
        public const int max_size = 20;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(long value, byte* dest) => JsonWriter.WriteLongUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(long value, char* dest) => JsonWriter.WriteLongUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadLongUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadLongUtf16(src, out advance);
    }

    public sealed class NullableLongFormatter : IFixedSerdeJsonFormatter<long?>
    {
        public const int max_size = 20;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(long? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteLongUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(long? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteLongUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableLongUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe long? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableLongUtf16(src, out advance);
    }

    public sealed class FloatFormatter : IFixedSerdeJsonFormatter<float>
    {
        public const int max_size = 24;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(float value, byte* dest) => JsonWriter.WriteFloatUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(float value, char* dest) => JsonWriter.WriteFloatUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadFloatUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadFloatUtf16(src, out advance);
    }

    public sealed class NullableFloatFormatter : IFixedSerdeJsonFormatter<float?>
    {
        public const int max_size = 24;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(float? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteFloatUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(float? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteFloatUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableFloatUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableFloatUtf16(src, out advance);
    }

    public sealed class DoubleFormatter : IFixedSerdeJsonFormatter<double>
    {
        public const int max_size = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(double value, byte* dest) => JsonWriter.WriteDoubleUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(double value, char* dest) => JsonWriter.WriteDoubleUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadDoubleUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadDoubleUtf16(src, out advance);
    }

    public sealed class NullableDoubleFormatter : IFixedSerdeJsonFormatter<double?>
    {
        public const int max_size = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(double? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteDoubleUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(double? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteDoubleUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableDoubleUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableDoubleUtf16(src, out advance);
    }

    public sealed class DecimalFormatter : IFixedSerdeJsonFormatter<decimal>
    {
        public const int max_size = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(decimal value, byte* dest) => JsonWriter.WriteDecimalUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(decimal value, char* dest) => JsonWriter.WriteDecimalUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe decimal DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadDecimalUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe decimal DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadDecimalUtf16(src, out advance);
    }

    public sealed class NullableDecimalFormatter : IFixedSerdeJsonFormatter<decimal?>
    {
        public const int max_size = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(decimal? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteDecimalUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(decimal? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteDecimalUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe decimal? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableDecimalUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe decimal? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableDecimalUtf16(src, out advance);
    }

    public sealed class CharFormatter : IFixedSerdeJsonFormatter<char>
    {
        public const int max_size = 8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(char value, byte* dest) => JsonWriter.WriteCharUtf8(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(char value, char* dest) => JsonWriter.WriteCharUtf16(dest, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe char DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadCharUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe char DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadCharUtf16(src, out advance);
    }

    public sealed class NullableCharFormatter : IFixedSerdeJsonFormatter<char?>
    {
        public const int max_size = 8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => max_size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf8Unsafe(char? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf8(dest)
                : JsonWriter.WriteCharUtf8(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int SerializeUtf16Unsafe(char? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUtf16(dest)
                : JsonWriter.WriteCharUtf16(dest, value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe char? DeserializeUtf8Unsafe(byte* src, out int advance) => JsonReader.ReadNullableCharUtf8(src, out advance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe char? DeserializeUtf16Unsafe(char* src, out int advance) => JsonReader.ReadNullableCharUtf16(src, out advance);
    }

    public class StringPrimitiveFormatter : IFixedSerdeJsonFormatter<bool>, IFixedSerdeJsonFormatter<byte>, IFixedSerdeJsonFormatter<sbyte>, IFixedSerdeJsonFormatter<ushort>,
        IFixedSerdeJsonFormatter<short>, IFixedSerdeJsonFormatter<uint>, IFixedSerdeJsonFormatter<int>, IFixedSerdeJsonFormatter<ulong>, IFixedSerdeJsonFormatter<long>,
        IFixedSerdeJsonFormatter<float>, IFixedSerdeJsonFormatter<double>, IFixedSerdeJsonFormatter<decimal>
    {
        int IFixedSerdeJsonFormatter<bool>.GetMaxRequiredSizeUtf8() => BoolFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<bool>.GetMaxRequiredSizeUtf16() => BoolFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<bool>.SerializeUtf8Unsafe(bool value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteBoolUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<bool>.SerializeUtf16Unsafe(bool value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteBoolUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe bool IFixedSerdeJsonFormatter<bool>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, BoolFormatter.max_size + 2, out advance);
            return SpanConverter.ToBool(result);
        }

        unsafe bool IFixedSerdeJsonFormatter<bool>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, BoolFormatter.max_size + 2, out advance);
            return SpanConverter.ToBool(result);
        }

        int IFixedSerdeJsonFormatter<byte>.GetMaxRequiredSizeUtf8() => ByteFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<byte>.GetMaxRequiredSizeUtf16() => ByteFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<byte>.SerializeUtf8Unsafe(byte value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteUlongUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<byte>.SerializeUtf16Unsafe(byte value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteUlongUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe byte IFixedSerdeJsonFormatter<byte>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, ByteFormatter.max_size + 2, out advance);
            return SpanConverter.ToByte(result);
        }

        unsafe byte IFixedSerdeJsonFormatter<byte>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, ByteFormatter.max_size + 2, out advance);
            return SpanConverter.ToByte(result);
        }

        int IFixedSerdeJsonFormatter<sbyte>.GetMaxRequiredSizeUtf8() => SbyteFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<sbyte>.GetMaxRequiredSizeUtf16() => SbyteFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<sbyte>.SerializeUtf8Unsafe(sbyte value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteLongUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<sbyte>.SerializeUtf16Unsafe(sbyte value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteLongUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe sbyte IFixedSerdeJsonFormatter<sbyte>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, SbyteFormatter.max_size + 2, out advance);
            return SpanConverter.ToSbyte(result);
        }

        unsafe sbyte IFixedSerdeJsonFormatter<sbyte>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, SbyteFormatter.max_size + 2, out advance);
            return SpanConverter.ToSbyte(result);
        }

        int IFixedSerdeJsonFormatter<ushort>.GetMaxRequiredSizeUtf8() => UshortFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<ushort>.GetMaxRequiredSizeUtf16() => UshortFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<ushort>.SerializeUtf8Unsafe(ushort value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteUlongUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<ushort>.SerializeUtf16Unsafe(ushort value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteUlongUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe ushort IFixedSerdeJsonFormatter<ushort>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, UshortFormatter.max_size + 2, out advance);
            return SpanConverter.ToUshort(result);
        }

        unsafe ushort IFixedSerdeJsonFormatter<ushort>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, UshortFormatter.max_size + 2, out advance);
            return SpanConverter.ToUshort(result);
        }

        int IFixedSerdeJsonFormatter<short>.GetMaxRequiredSizeUtf8() => ShortFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<short>.GetMaxRequiredSizeUtf16() => ShortFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<short>.SerializeUtf8Unsafe(short value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteLongUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<short>.SerializeUtf16Unsafe(short value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteLongUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe short IFixedSerdeJsonFormatter<short>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, ShortFormatter.max_size + 2, out advance);
            return SpanConverter.ToShort(result);
        }

        unsafe short IFixedSerdeJsonFormatter<short>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, ShortFormatter.max_size + 2, out advance);
            return SpanConverter.ToShort(result);
        }

        int IFixedSerdeJsonFormatter<uint>.GetMaxRequiredSizeUtf8() => UintFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<uint>.GetMaxRequiredSizeUtf16() => UintFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<uint>.SerializeUtf8Unsafe(uint value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteUlongUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<uint>.SerializeUtf16Unsafe(uint value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteUlongUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe uint IFixedSerdeJsonFormatter<uint>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, UintFormatter.max_size + 2, out advance);
            return SpanConverter.ToUint(result);
        }

        unsafe uint IFixedSerdeJsonFormatter<uint>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, UintFormatter.max_size + 2, out advance);
            return SpanConverter.ToUint(result);
        }

        int IFixedSerdeJsonFormatter<int>.GetMaxRequiredSizeUtf8() => IntFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<int>.GetMaxRequiredSizeUtf16() => IntFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<int>.SerializeUtf8Unsafe(int value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteLongUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<int>.SerializeUtf16Unsafe(int value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteLongUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<int>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, IntFormatter.max_size + 2, out advance);
            return SpanConverter.ToInt(result);
        }

        unsafe int IFixedSerdeJsonFormatter<int>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, IntFormatter.max_size + 2, out advance);
            return SpanConverter.ToInt(result);
        }

        int IFixedSerdeJsonFormatter<ulong>.GetMaxRequiredSizeUtf8() => UlongFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<ulong>.GetMaxRequiredSizeUtf16() => UlongFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<ulong>.SerializeUtf8Unsafe(ulong value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteUlongUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<ulong>.SerializeUtf16Unsafe(ulong value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteUlongUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe ulong IFixedSerdeJsonFormatter<ulong>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, UlongFormatter.max_size + 2, out advance);
            return SpanConverter.ToUlong(result);
        }

        unsafe ulong IFixedSerdeJsonFormatter<ulong>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, UlongFormatter.max_size + 2, out advance);
            return SpanConverter.ToUlong(result);
        }

        int IFixedSerdeJsonFormatter<long>.GetMaxRequiredSizeUtf8() => LongFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<long>.GetMaxRequiredSizeUtf16() => LongFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<long>.SerializeUtf8Unsafe(long value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteLongUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<long>.SerializeUtf16Unsafe(long value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteLongUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe long IFixedSerdeJsonFormatter<long>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, LongFormatter.max_size + 2, out advance);
            return SpanConverter.ToLong(result);
        }

        unsafe long IFixedSerdeJsonFormatter<long>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, LongFormatter.max_size + 2, out advance);
            return SpanConverter.ToLong(result);
        }

        int IFixedSerdeJsonFormatter<float>.GetMaxRequiredSizeUtf8() => FloatFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<float>.GetMaxRequiredSizeUtf16() => FloatFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<float>.SerializeUtf8Unsafe(float value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteFloatUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<float>.SerializeUtf16Unsafe(float value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteFloatUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe float IFixedSerdeJsonFormatter<float>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, FloatFormatter.max_size + 2, out advance);
            return SpanConverter.ToFloat(result);
        }

        unsafe float IFixedSerdeJsonFormatter<float>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, FloatFormatter.max_size + 2, out advance);
            return SpanConverter.ToFloat(result);
        }

        int IFixedSerdeJsonFormatter<double>.GetMaxRequiredSizeUtf8() => DoubleFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<double>.GetMaxRequiredSizeUtf16() => DoubleFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<double>.SerializeUtf8Unsafe(double value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteDoubleUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<double>.SerializeUtf16Unsafe(double value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteDoubleUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe double IFixedSerdeJsonFormatter<double>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, DoubleFormatter.max_size + 2, out advance);
            return SpanConverter.ToDouble(result);
        }

        unsafe double IFixedSerdeJsonFormatter<double>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, DoubleFormatter.max_size + 2, out advance);
            return SpanConverter.ToDouble(result);
        }

        int IFixedSerdeJsonFormatter<decimal>.GetMaxRequiredSizeUtf8() => DecimalFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<decimal>.GetMaxRequiredSizeUtf16() => DecimalFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<decimal>.SerializeUtf8Unsafe(decimal value, byte* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteDecimalUtf8(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<decimal>.SerializeUtf16Unsafe(decimal value, char* dest)
        {
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteDecimalUtf16(dest, value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe decimal IFixedSerdeJsonFormatter<decimal>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, DecimalFormatter.max_size + 2, out advance);
            return SpanConverter.ToDecimal(result);
        }

        unsafe decimal IFixedSerdeJsonFormatter<decimal>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, DecimalFormatter.max_size + 2, out advance);
            return SpanConverter.ToDecimal(result);
        }
    }

    public class NullableStringPrimitiveFormatter : IFixedSerdeJsonFormatter<bool?>, IFixedSerdeJsonFormatter<byte?>, IFixedSerdeJsonFormatter<sbyte?>, IFixedSerdeJsonFormatter<ushort?>,
        IFixedSerdeJsonFormatter<short?>, IFixedSerdeJsonFormatter<uint?>, IFixedSerdeJsonFormatter<int?>, IFixedSerdeJsonFormatter<ulong?>, IFixedSerdeJsonFormatter<long?>,
        IFixedSerdeJsonFormatter<float?>, IFixedSerdeJsonFormatter<double?>, IFixedSerdeJsonFormatter<decimal?>
    {
        int IFixedSerdeJsonFormatter<bool?>.GetMaxRequiredSizeUtf8() => NullableBoolFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<bool?>.GetMaxRequiredSizeUtf16() => NullableBoolFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<bool?>.SerializeUtf8Unsafe(bool? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteBoolUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<bool?>.SerializeUtf16Unsafe(bool? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteBoolUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe bool? IFixedSerdeJsonFormatter<bool?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableBoolFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToBool(result);
        }

        unsafe bool? IFixedSerdeJsonFormatter<bool?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableBoolFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToBool(result);
        }

        int IFixedSerdeJsonFormatter<byte?>.GetMaxRequiredSizeUtf8() => NullableByteFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<byte?>.GetMaxRequiredSizeUtf16() => NullableByteFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<byte?>.SerializeUtf8Unsafe(byte? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteUlongUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<byte?>.SerializeUtf16Unsafe(byte? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteUlongUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe byte? IFixedSerdeJsonFormatter<byte?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableByteFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToByte(result);
        }

        unsafe byte? IFixedSerdeJsonFormatter<byte?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableByteFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToByte(result);
        }

        int IFixedSerdeJsonFormatter<sbyte?>.GetMaxRequiredSizeUtf8() => NullableSbyteFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<sbyte?>.GetMaxRequiredSizeUtf16() => NullableSbyteFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<sbyte?>.SerializeUtf8Unsafe(sbyte? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteLongUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<sbyte?>.SerializeUtf16Unsafe(sbyte? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteLongUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe sbyte? IFixedSerdeJsonFormatter<sbyte?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableSbyteFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToSbyte(result);
        }

        unsafe sbyte? IFixedSerdeJsonFormatter<sbyte?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableSbyteFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToSbyte(result);
        }

        int IFixedSerdeJsonFormatter<ushort?>.GetMaxRequiredSizeUtf8() => NullableUshortFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<ushort?>.GetMaxRequiredSizeUtf16() => NullableUshortFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<ushort?>.SerializeUtf8Unsafe(ushort? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteUlongUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<ushort?>.SerializeUtf16Unsafe(ushort? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteUlongUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe ushort? IFixedSerdeJsonFormatter<ushort?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableUshortFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToUshort(result);
        }

        unsafe ushort? IFixedSerdeJsonFormatter<ushort?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableUshortFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToUshort(result);
        }

        int IFixedSerdeJsonFormatter<short?>.GetMaxRequiredSizeUtf8() => NullableShortFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<short?>.GetMaxRequiredSizeUtf16() => NullableShortFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<short?>.SerializeUtf8Unsafe(short? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteLongUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<short?>.SerializeUtf16Unsafe(short? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteLongUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe short? IFixedSerdeJsonFormatter<short?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableShortFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToShort(result);
        }

        unsafe short? IFixedSerdeJsonFormatter<short?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableShortFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToShort(result);
        }

        int IFixedSerdeJsonFormatter<uint?>.GetMaxRequiredSizeUtf8() => NullableUintFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<uint?>.GetMaxRequiredSizeUtf16() => NullableUintFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<uint?>.SerializeUtf8Unsafe(uint? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteUlongUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<uint?>.SerializeUtf16Unsafe(uint? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteUlongUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe uint? IFixedSerdeJsonFormatter<uint?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableUintFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToUint(result);
        }

        unsafe uint? IFixedSerdeJsonFormatter<uint?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableUintFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToUint(result);
        }

        int IFixedSerdeJsonFormatter<int?>.GetMaxRequiredSizeUtf8() => NullableIntFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<int?>.GetMaxRequiredSizeUtf16() => NullableIntFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<int?>.SerializeUtf8Unsafe(int? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteLongUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<int?>.SerializeUtf16Unsafe(int? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteLongUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe int? IFixedSerdeJsonFormatter<int?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableIntFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToInt(result);
        }

        unsafe int? IFixedSerdeJsonFormatter<int?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableIntFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToInt(result);
        }

        int IFixedSerdeJsonFormatter<ulong?>.GetMaxRequiredSizeUtf8() => NullableUlongFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<ulong?>.GetMaxRequiredSizeUtf16() => NullableUlongFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<ulong?>.SerializeUtf8Unsafe(ulong? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteUlongUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<ulong?>.SerializeUtf16Unsafe(ulong? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteUlongUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe ulong? IFixedSerdeJsonFormatter<ulong?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableUlongFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToUlong(result);
        }

        unsafe ulong? IFixedSerdeJsonFormatter<ulong?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableUlongFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToUlong(result);
        }

        int IFixedSerdeJsonFormatter<long?>.GetMaxRequiredSizeUtf8() => NullableLongFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<long?>.GetMaxRequiredSizeUtf16() => NullableLongFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<long?>.SerializeUtf8Unsafe(long? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteLongUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<long?>.SerializeUtf16Unsafe(long? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteLongUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe long? IFixedSerdeJsonFormatter<long?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableLongFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToLong(result);
        }

        unsafe long? IFixedSerdeJsonFormatter<long?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableLongFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToLong(result);
        }

        int IFixedSerdeJsonFormatter<float?>.GetMaxRequiredSizeUtf8() => NullableFloatFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<float?>.GetMaxRequiredSizeUtf16() => NullableFloatFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<float?>.SerializeUtf8Unsafe(float? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteFloatUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<float?>.SerializeUtf16Unsafe(float? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteFloatUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe float? IFixedSerdeJsonFormatter<float?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableFloatFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToFloat(result);
        }

        unsafe float? IFixedSerdeJsonFormatter<float?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableFloatFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToFloat(result);
        }

        int IFixedSerdeJsonFormatter<double?>.GetMaxRequiredSizeUtf8() => NullableDoubleFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<double?>.GetMaxRequiredSizeUtf16() => NullableDoubleFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<double?>.SerializeUtf8Unsafe(double? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteDoubleUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<double?>.SerializeUtf16Unsafe(double? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteDoubleUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe double? IFixedSerdeJsonFormatter<double?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableDoubleFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToDouble(result);
        }

        unsafe double? IFixedSerdeJsonFormatter<double?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableDoubleFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToDouble(result);
        }

        int IFixedSerdeJsonFormatter<decimal?>.GetMaxRequiredSizeUtf8() => NullableDecimalFormatter.max_size + 2;

        int IFixedSerdeJsonFormatter<decimal?>.GetMaxRequiredSizeUtf16() => NullableDecimalFormatter.max_size + 2;

        unsafe int IFixedSerdeJsonFormatter<decimal?>.SerializeUtf8Unsafe(decimal? value, byte* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf8(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            dest += JsonWriter.WriteDecimalUtf8(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf8(dest);
            return (int) (dest - origin);
        }

        unsafe int IFixedSerdeJsonFormatter<decimal?>.SerializeUtf16Unsafe(decimal? value, char* dest)
        {
            if (value == null) return JsonWriter.WriteNullUtf16(dest);
            var origin = dest;
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            dest += JsonWriter.WriteDecimalUtf16(dest, value.Value);
            dest += JsonWriter.WriteDoubleQuoteUtf16(dest);
            return (int) (dest - origin);
        }

        unsafe decimal? IFixedSerdeJsonFormatter<decimal?>.DeserializeUtf8Unsafe(byte* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf8(src, NullableDecimalFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToDecimal(result);
        }

        unsafe decimal? IFixedSerdeJsonFormatter<decimal?>.DeserializeUtf16Unsafe(char* src, out int advance)
        {
            var result = JsonReader.ReadValueUntilEndUtf16(src, NullableDecimalFormatter.max_size + 2, out advance);
            if (SpanConverter.IsNull(result)) return null;
            return SpanConverter.ToDecimal(result);
        }
    }
}