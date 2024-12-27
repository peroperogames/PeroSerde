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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 5;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 5;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 3;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 4;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 4;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 4;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 5;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 5;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 6;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 6;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 10;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 10;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 11;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 11;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 11;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 11;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_uint64_numbers_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_uint64_numbers_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_uint64_numbers_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_uint64_numbers_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_int64_numbers_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_int64_numbers_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_int64_numbers_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_int64_numbers_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_float_string_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_float_string_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_float_string_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_float_string_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_double_string_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_double_string_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_double_string_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_double_string_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_decimal_string_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_decimal_string_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => NumberHelper.max_decimal_string_length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => NumberHelper.max_decimal_string_length;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 8;

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
        public JsonType jsonType => JsonType.Value;

        public bool isFixedFormatter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf8() => 8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxRequiredSizeUtf16() => 8;

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
}