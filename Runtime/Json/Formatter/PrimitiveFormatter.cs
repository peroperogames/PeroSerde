/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/03/08
 * Description: PrimitiveFormatter
 */

namespace PeroTools2.Serde.Json.Formatter
{
    static partial class SerdeJsonFormatterProvider
    {
        static partial void RegisterPrimitiveFormatters()
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
            Register(new StringFormatter());
        }
    }

    public sealed class BoolFormatter : SerdeJsonFormatter<bool>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(bool value) => 5;
        public override int GetMaxRequiredSizeUtf16(bool value) => 5;
        public override unsafe int SerializeUtf8Unsafe(bool value, byte* dest) => JsonWriter.WriteBoolUnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(bool value, char* dest) => JsonWriter.WriteBoolUnsafeUtf16(dest, value);
    }

    public sealed class NullableBoolFormatter : SerdeJsonFormatter<bool?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(bool? value) => 5;
        public override int GetMaxRequiredSizeUtf16(bool? value) => 5;

        public override unsafe int SerializeUtf8Unsafe(bool? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteBoolUnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(bool? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteBoolUnsafeUtf16(dest, value.Value);
    }

    public sealed class ByteFormatter : SerdeJsonFormatter<byte>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(byte value) => 3;
        public override int GetMaxRequiredSizeUtf16(byte value) => 3;
        public override unsafe int SerializeUtf8Unsafe(byte value, byte* dest) => JsonWriter.WriteUInt64UnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(byte value, char* dest) => JsonWriter.WriteUInt64UnsafeUtf16(dest, value);
    }

    public sealed class NullableByteFormatter : SerdeJsonFormatter<byte?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(byte? value) => 4;
        public override int GetMaxRequiredSizeUtf16(byte? value) => 4;

        public override unsafe int SerializeUtf8Unsafe(byte? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteUInt64UnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(byte? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteUInt64UnsafeUtf16(dest, value.Value);
    }

    public sealed class SbyteFormatter : SerdeJsonFormatter<sbyte>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(sbyte value) => 4;
        public override int GetMaxRequiredSizeUtf16(sbyte value) => 4;
        public override unsafe int SerializeUtf8Unsafe(sbyte value, byte* dest) => JsonWriter.WriteInt64UnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(sbyte value, char* dest) => JsonWriter.WriteInt64UnsafeUtf16(dest, value);
    }

    public sealed class NullableSbyteFormatter : SerdeJsonFormatter<sbyte?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(sbyte? value) => 4;
        public override int GetMaxRequiredSizeUtf16(sbyte? value) => 4;

        public override unsafe int SerializeUtf8Unsafe(sbyte? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteInt64UnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(sbyte? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteInt64UnsafeUtf16(dest, value.Value);
    }

    public sealed class UshortFormatter : SerdeJsonFormatter<ushort>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(ushort value) => 5;
        public override int GetMaxRequiredSizeUtf16(ushort value) => 5;
        public override unsafe int SerializeUtf8Unsafe(ushort value, byte* dest) => JsonWriter.WriteUInt64UnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(ushort value, char* dest) => JsonWriter.WriteUInt64UnsafeUtf16(dest, value);
    }

    public sealed class NullableUshortFormatter : SerdeJsonFormatter<ushort?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(ushort? value) => 5;
        public override int GetMaxRequiredSizeUtf16(ushort? value) => 5;

        public override unsafe int SerializeUtf8Unsafe(ushort? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteUInt64UnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(ushort? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteUInt64UnsafeUtf16(dest, value.Value);
    }

    public sealed class ShortFormatter : SerdeJsonFormatter<short>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(short value) => 6;
        public override int GetMaxRequiredSizeUtf16(short value) => 6;
        public override unsafe int SerializeUtf8Unsafe(short value, byte* dest) => JsonWriter.WriteInt64UnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(short value, char* dest) => JsonWriter.WriteInt64UnsafeUtf16(dest, value);
    }

    public sealed class NullableShortFormatter : SerdeJsonFormatter<short?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(short? value) => 6;
        public override int GetMaxRequiredSizeUtf16(short? value) => 6;

        public override unsafe int SerializeUtf8Unsafe(short? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteInt64UnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(short? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteInt64UnsafeUtf16(dest, value.Value);
    }

    public sealed class UintFormatter : SerdeJsonFormatter<uint>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(uint value) => 10;
        public override int GetMaxRequiredSizeUtf16(uint value) => 10;
        public override unsafe int SerializeUtf8Unsafe(uint value, byte* dest) => JsonWriter.WriteUInt64UnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(uint value, char* dest) => JsonWriter.WriteUInt64UnsafeUtf16(dest, value);
    }

    public sealed class NullableUintFormatter : SerdeJsonFormatter<uint?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(uint? value) => 10;
        public override int GetMaxRequiredSizeUtf16(uint? value) => 10;

        public override unsafe int SerializeUtf8Unsafe(uint? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteUInt64UnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(uint? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteUInt64UnsafeUtf16(dest, value.Value);
    }

    public sealed class IntFormatter : SerdeJsonFormatter<int>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(int value) => 11;
        public override int GetMaxRequiredSizeUtf16(int value) => 11;
        public override unsafe int SerializeUtf8Unsafe(int value, byte* dest) => JsonWriter.WriteInt64UnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(int value, char* dest) => JsonWriter.WriteInt64UnsafeUtf16(dest, value);
    }

    public sealed class NullableIntFormatter : SerdeJsonFormatter<int?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(int? value) => 11;
        public override int GetMaxRequiredSizeUtf16(int? value) => 11;

        public override unsafe int SerializeUtf8Unsafe(int? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteInt64UnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(int? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteInt64UnsafeUtf16(dest, value.Value);
    }

    public sealed class UlongFormatter : SerdeJsonFormatter<ulong>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(ulong value) => NumberHelper.max_uint64_numbers_length;
        public override int GetMaxRequiredSizeUtf16(ulong value) => NumberHelper.max_uint64_numbers_length;
        public override unsafe int SerializeUtf8Unsafe(ulong value, byte* dest) => JsonWriter.WriteUInt64UnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(ulong value, char* dest) => JsonWriter.WriteUInt64UnsafeUtf16(dest, value);
    }

    public sealed class NullableUlongFormatter : SerdeJsonFormatter<ulong?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(ulong? value) => NumberHelper.max_uint64_numbers_length;
        public override int GetMaxRequiredSizeUtf16(ulong? value) => NumberHelper.max_uint64_numbers_length;

        public override unsafe int SerializeUtf8Unsafe(ulong? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteUInt64UnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(ulong? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteUInt64UnsafeUtf16(dest, value.Value);
    }

    public sealed class LongFormatter : SerdeJsonFormatter<long>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(long value) => NumberHelper.max_int64_numbers_length;
        public override int GetMaxRequiredSizeUtf16(long value) => NumberHelper.max_int64_numbers_length;
        public override unsafe int SerializeUtf8Unsafe(long value, byte* dest) => JsonWriter.WriteInt64UnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(long value, char* dest) => JsonWriter.WriteInt64UnsafeUtf16(dest, value);
    }

    public sealed class NullableLongFormatter : SerdeJsonFormatter<long?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(long? value) => NumberHelper.max_int64_numbers_length;
        public override int GetMaxRequiredSizeUtf16(long? value) => NumberHelper.max_int64_numbers_length;

        public override unsafe int SerializeUtf8Unsafe(long? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteInt64UnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(long? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteInt64UnsafeUtf16(dest, value.Value);
    }

    public sealed class FloatFormatter : SerdeJsonFormatter<float>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(float value) => NumberHelper.max_float_string_length;
        public override int GetMaxRequiredSizeUtf16(float value) => NumberHelper.max_float_string_length;
        public override unsafe int SerializeUtf8Unsafe(float value, byte* dest) => JsonWriter.WriteFloatUnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(float value, char* dest) => JsonWriter.WriteFloatUnsafeUtf16(dest, value);
    }

    public sealed class NullableFloatFormatter : SerdeJsonFormatter<float?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(float? value) => NumberHelper.max_float_string_length;
        public override int GetMaxRequiredSizeUtf16(float? value) => NumberHelper.max_float_string_length;

        public override unsafe int SerializeUtf8Unsafe(float? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteFloatUnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(float? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteFloatUnsafeUtf16(dest, value.Value);
    }

    public sealed class DoubleFormatter : SerdeJsonFormatter<double>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(double value) => NumberHelper.max_double_string_length;
        public override int GetMaxRequiredSizeUtf16(double value) => NumberHelper.max_double_string_length;
        public override unsafe int SerializeUtf8Unsafe(double value, byte* dest) => JsonWriter.WriteDoubleUnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(double value, char* dest) => JsonWriter.WriteDoubleUnsafeUtf16(dest, value);
    }

    public sealed class NullableDoubleFormatter : SerdeJsonFormatter<double?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(double? value) => NumberHelper.max_double_string_length;
        public override int GetMaxRequiredSizeUtf16(double? value) => NumberHelper.max_double_string_length;

        public override unsafe int SerializeUtf8Unsafe(double? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteDoubleUnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(double? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteDoubleUnsafeUtf16(dest, value.Value);
    }

    public sealed class DecimalFormatter : SerdeJsonFormatter<decimal>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(decimal value) => NumberHelper.max_decimal_string_length;
        public override int GetMaxRequiredSizeUtf16(decimal value) => NumberHelper.max_decimal_string_length;
        public override unsafe int SerializeUtf8Unsafe(decimal value, byte* dest) => JsonWriter.WriteDecimalUnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(decimal value, char* dest) => JsonWriter.WriteDecimalUnsafeUtf16(dest, value);
    }

    public sealed class NullableDecimalFormatter : SerdeJsonFormatter<decimal?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(decimal? value) => NumberHelper.max_decimal_string_length;
        public override int GetMaxRequiredSizeUtf16(decimal? value) => NumberHelper.max_decimal_string_length;

        public override unsafe int SerializeUtf8Unsafe(decimal? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteDecimalUnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(decimal? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteDecimalUnsafeUtf16(dest, value.Value);
    }

    public sealed class CharFormatter : SerdeJsonFormatter<char>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(char value) => 8; // 8 是 Unicode 转义 + 引号
        public override int GetMaxRequiredSizeUtf16(char value) => 8; // 8 是 Unicode 转义 + 引号
        public override unsafe int SerializeUtf8Unsafe(char value, byte* dest) => JsonWriter.WriteCharUnsafeUtf8(dest, value);
        public override unsafe int SerializeUtf16Unsafe(char value, char* dest) => JsonWriter.WriteCharUnsafeUtf16(dest, value);
    }

    public sealed class NullableCharFormatter : SerdeJsonFormatter<char?>
    {
        public override bool isUnsafeFormatter => true;
        public override int GetMaxRequiredSizeUtf8(char? value) => 8; // 8 是 Unicode 转义 + 引号
        public override int GetMaxRequiredSizeUtf16(char? value) => 8; // 8 是 Unicode 转义 + 引号

        public override unsafe int SerializeUtf8Unsafe(char? value, byte* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf8(dest)
                : JsonWriter.WriteCharUnsafeUtf8(dest, value.Value);

        public override unsafe int SerializeUtf16Unsafe(char? value, char* dest) =>
            value == null
                ? JsonWriter.WriteNullUnsafeUtf16(dest)
                : JsonWriter.WriteCharUnsafeUtf16(dest, value.Value);
    }

    public sealed class StringFormatter : SerdeJsonFormatter<string>
    {
        public override bool isUnsafeFormatter => false;

        // public override void SerializeUtf8<TBufferWriter>(string value, ref JsonWriterUtf8<TBufferWriter> writer) => writer.WriteString(value);

        public override void SerializeUtf16<TBufferWriter>(string value, ref JsonWriterUtf16<TBufferWriter> writer) => writer.WriteString(value);
    }

}