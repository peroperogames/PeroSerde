/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/25
 * Description: SerdeSerializer
 */

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PeroTools2.Serde.Json.Tests;

[SerdeJsonSerializable]
public enum NumberEnum
{
    AEnum,
    BEnum,
    CEnum
}

[SerdeJsonSerializable, SerdeJsonEnumFormatter(SerdeEnumFormat.String, SerdeEnumFormat.String)]
public enum StringEnum
{
    AEnum,
    BEnum,
    CEnum
}

[SerdeJsonSerializable, SerdeJsonEnumFormatter(SerdeEnumFormat.String)]
public enum StringNumberEnum
{
    AEnum,
    BEnum,
    CEnum
}

[SerdeJsonSerializable, SerdeJsonEnumFormatter(SerdeEnumFormat.Number, SerdeEnumFormat.String)]
public enum NumberStringEnum
{
    AEnum,
    BEnum,
    CEnum
}

[SerdeJsonSerializable, Flags]
public enum NumberFlags
{
    AFlag = 1,
    BFlag = 2,
    CFlag = 4
}

[SerdeJsonSerializable, Flags, SerdeJsonEnumFormatter(SerdeEnumFormat.String, SerdeEnumFormat.String)]
public enum StringFlags
{
    AFlag = 1,
    BFlag = 2,
    CFlag = 4
}

[SerdeJsonSerializable]
public partial struct BoolType(bool value) : IEquatable<BoolType>
{
    public bool value = value;

    public bool Equals(BoolType other) => value == other.value;

    public override bool Equals(object obj) => obj is BoolType other && Equals(other);

    public readonly override int GetHashCode() => value.GetHashCode();

    public static bool operator ==(BoolType left, BoolType right) => left.Equals(right);

    public static bool operator !=(BoolType left, BoolType right) => !(left == right);
}

[SerdeJsonSerializable]
public partial struct CharType(char value)
{
    public char value = value;
}

[SerdeJsonSerializable]
public partial struct ByteType(byte value)
{
    public byte value = value;
}

[SerdeJsonSerializable]
public partial struct SbyteType(sbyte value)
{
    public sbyte value = value;
}

[SerdeJsonSerializable]
public partial struct ShortType(short value)
{
    public short value = value;
}

[SerdeJsonSerializable]
public partial struct UshortType(ushort value)
{
    public ushort value = value;
}

[SerdeJsonSerializable]
public partial struct IntType(int value)
{
    public int value = value;
}

[SerdeJsonSerializable]
public partial struct UintType(uint value)
{
    public uint value = value;
}

[SerdeJsonSerializable]
public partial struct LongType(long value)
{
    public long value = value;
}

[SerdeJsonSerializable]
public partial struct UlongType(ulong value)
{
    public ulong value = value;
}

[SerdeJsonSerializable]
public partial struct FloatType(float value)
{
    public float value = value;
}

[SerdeJsonSerializable]
public partial struct DoubleType(double value)
{
    public double value = value;
}

[SerdeJsonSerializable]
public partial struct DecimalType(decimal value)
{
    public decimal value = value;
}

[SerdeJsonSerializable]
public partial struct NullableBoolType(bool? value)
{
    public bool? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableCharType(char? value)
{
    public char? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableByteType(byte? value)
{
    public byte? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableSbyteType(sbyte? value)
{
    public sbyte? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableShortType(short? value)
{
    public short? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableUshortType(ushort? value)
{
    public ushort? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableIntType(int? value)
{
    public int? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableUintType(uint? value)
{
    public uint? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableLongType(long? value)
{
    public long? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableUlongType(ulong? value)
{
    public ulong? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableFloatType(float? value)
{
    public float? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableDoubleType(double? value)
{
    public double? value = value;
}

[SerdeJsonSerializable]
public partial struct NullableDecimalType(decimal? value)
{
    public decimal? value = value;
}

[SerdeJsonSerializable]
public partial struct StringType(string value)
{
    public string value = value;
}

[SerdeJsonSerializable]
public partial struct DateTimeType(DateTime value) : IEquatable<DateTimeType>
{
    public DateTime value = value;

    public bool Equals(DateTimeType other) => value == other.value || value.ToString(CultureInfo.CurrentCulture) == other.value.ToString(CultureInfo.CurrentCulture);

    public override bool Equals(object obj) => obj is DateTimeType other && Equals(other);

    public override int GetHashCode() => value.GetHashCode();
}

[SerdeJsonSerializable]
public partial struct UnixSecondDateTimeType(DateTime value) : IEquatable<UnixSecondDateTimeType>
{
    [SerdeJsonFormatter(typeof(UnixSecondsFormatter))]
    [JsonConverter(typeof(UnixSecondsConverter))]
    public DateTime value = value;

    public bool Equals(UnixSecondDateTimeType other) => other.value.ToString(CultureInfo.CurrentCulture) == value.ToString(CultureInfo.CurrentCulture);

    public override bool Equals(object obj) => obj is UnixSecondDateTimeType other && Equals(other);

    public readonly override int GetHashCode() => value.GetHashCode();

    private sealed class UnixSecondsConverter : JsonConverter<DateTime>
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, DateTime value, JsonSerializer serializer)
            => writer.WriteValue(DateTimeHelper.DateTimeToUnixTimeSeconds(value));

        public override DateTime ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
            => DateTimeHelper.UnixTimeSecondsToDateTime((long) reader.Value);
    }
}

[SerdeJsonSerializable]
public partial struct UnixMillisecondDateTimeType(DateTime value) : IEquatable<UnixMillisecondDateTimeType>
{
    [SerdeJsonFormatter(typeof(UnixMillisecondsFormatter))]
    [JsonConverter(typeof(UnixMillisecondConverter))]
    public DateTime value = value;

    public bool Equals(UnixMillisecondDateTimeType other) => other.value.ToString(CultureInfo.CurrentCulture) == value.ToString(CultureInfo.CurrentCulture);

    public override bool Equals(object obj) => obj is UnixMillisecondDateTimeType other && Equals(other);

    public override int GetHashCode() => value.GetHashCode();

    private sealed class UnixMillisecondConverter : JsonConverter<DateTime>
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, DateTime value, JsonSerializer serializer)
            => writer.WriteValue(DateTimeHelper.DateTimeToUnixTimeMilliseconds(value));

        public override DateTime ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
            => DateTimeHelper.UnixTimeMillisecondsToDateTime((long) reader.Value);
    }
}

[SerdeJsonSerializable]
public partial struct GuidType(Guid value)
{
    public Guid value = value;
}

[SerdeJsonSerializable]
public partial struct ListNumberType(List<int> value) : IEquatable<ListNumberType>
{
    public List<int> value = value;

    public bool Equals(ListNumberType other) => value.SequenceEqual(other.value);

    public override bool Equals(object obj) => obj is ListNumberType other && Equals(other);

    public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
}

[SerdeJsonSerializable]
public partial struct ListObjectType(List<BoolType> value) : IEquatable<ListObjectType>
{
    public List<BoolType> value = value;

    // ReSharper disable once UsageOfDefaultStructEquality
    public bool Equals(ListObjectType other) => value.SequenceEqual(other.value);

    public override bool Equals(object obj) => obj is ListObjectType other && Equals(other);

    public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
}

[SerdeJsonSerializable]
public partial struct DictionaryStringType(Dictionary<string, BoolType> value) : IEquatable<DictionaryStringType>
{
    public Dictionary<string, BoolType> value = value;

    public bool Equals(DictionaryStringType other) =>
        value != null && other.value != null &&
        value.Count == other.value.Count &&
        // ReSharper disable once UsageOfDefaultStructEquality
        !value.Except(other.value).Any();

    public override bool Equals(object obj) => obj is DictionaryStringType other && Equals(other);

    public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
}

[SerdeJsonSerializable]
public partial struct DictionaryNumberType(Dictionary<int, BoolType> value) : IEquatable<DictionaryNumberType>
{
    [SerdeJsonFormatter(typeof(DictionaryFormatter<int, BoolType>))]
    public Dictionary<int, BoolType> value = value;

    public bool Equals(DictionaryNumberType other) =>
        value != null && other.value != null &&
        value.Count == other.value.Count &&
        // ReSharper disable once UsageOfDefaultStructEquality
        !value.Except(other.value).Any();

    public override bool Equals(object obj) => obj is DictionaryNumberType other && Equals(other);

    public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
}

[SerdeJsonSerializable]
public partial struct NumberEnumType(NumberEnum value)
{
    public NumberEnum value = value;
}

[SerdeJsonSerializable]
public partial struct StringEnumType(StringEnum value)
{
    [JsonConverter(typeof(StringEnumConverter))]
    public StringEnum value = value;
}

[SerdeJsonSerializable]
public partial struct StringNumberEnumType(StringNumberEnum value)
{
    [JsonConverter(typeof(StringNumberEnumConverter))]
    public StringNumberEnum value = value;

    private sealed class StringNumberEnumConverter : JsonConverter<StringNumberEnum>
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, StringNumberEnum value, JsonSerializer serializer)
            => writer.WriteValue((long) value);

        public override StringNumberEnum ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, StringNumberEnum existingValue, bool hasExistingValue, JsonSerializer serializer)
            => Enum.Parse<StringNumberEnum>(reader.Value!.ToString());
    }
}

[SerdeJsonSerializable]
public partial struct NumberStringEnumType(NumberStringEnum value)
{
    [JsonConverter(typeof(NumberStringEnumConverter))]
    public NumberStringEnum value = value;

    private sealed class NumberStringEnumConverter : JsonConverter<NumberStringEnum>
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, NumberStringEnum value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString());

        public override NumberStringEnum ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, NumberStringEnum existingValue, bool hasExistingValue, JsonSerializer serializer)
            => (NumberStringEnum) (long) reader.Value!;
    }
}

[SerdeJsonSerializable]
public partial struct NumberFlagsType(NumberFlags value)
{
    public NumberFlags value = value;
}

[SerdeJsonSerializable]
public partial struct StringFlagsType(StringFlags value)
{
    [JsonConverter(typeof(StringFlagsConverter))]
    public StringFlags value = value;

    private sealed class StringFlagsConverter : JsonConverter<StringFlags>
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, StringFlags value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString().Replace(", ", ","));

        public override StringFlags ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, StringFlags existingValue, bool hasExistingValue, JsonSerializer serializer)
            => Enum.Parse<StringFlags>(reader.Value!.ToString());
    }
}

[SerdeJsonSerializable]
public partial struct LazyType(Lazy<string> value) : IEquatable<LazyType>
{
    [JsonConverter(typeof(LazyConverter))]
    public Lazy<string> value = value;

    public class LazyConverter : JsonConverter<Lazy<string>>
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Lazy<string> value, JsonSerializer serializer) => serializer.Serialize(writer, value.Value);

        public override Lazy<string> ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Lazy<string> existingValue, bool hasExistingValue, JsonSerializer serializer)
            => new((string) reader.Value);
    }

    public bool Equals(LazyType other) => value != null && other.value != null && Equals(value.Value, other.value.Value);

    public override bool Equals(object obj) => obj is LazyType other && Equals(other);

    public override int GetHashCode() => value != null ? value.Value.GetHashCode() : 0;
}

[SerdeJsonSerializable]
public partial struct NullableType(BoolType? value)
{
    public Nullable<BoolType> value = value;
}

[SerdeJsonSerializable]
public partial struct GenericType<T>(T data)
{
    public T data = data;
}

[SerdeJsonSerializable]
public partial struct StringBoolType(bool value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<bool>))]
    public bool value = value;
}

[SerdeJsonSerializable]
public partial struct StringByteType(byte value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<byte>))]
    public byte value = value;
}

[SerdeJsonSerializable]
public partial struct StringSbyteType(sbyte value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<sbyte>))]
    public sbyte value = value;
}

[SerdeJsonSerializable]
public partial struct StringShortType(short value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<short>))]
    public short value = value;
}

[SerdeJsonSerializable]
public partial struct StringUshortType(ushort value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<ushort>))]
    public ushort value = value;
}

[SerdeJsonSerializable]
public partial struct StringIntType(int value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<int>))]
    public int value = value;
}

[SerdeJsonSerializable]
public partial struct StringUintType(uint value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<uint>))]
    public uint value = value;
}

[SerdeJsonSerializable]
public partial struct StringLongType(long value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<long>))]
    public long value = value;
}

[SerdeJsonSerializable]
public partial struct StringUlongType(ulong value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<ulong>))]
    public ulong value = value;
}

[SerdeJsonSerializable]
public partial struct StringFloatType(float value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<float>))]
    public float value = value;
}

[SerdeJsonSerializable]
public partial struct StringDoubleType(double value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<double>))]
    public double value = value;
}

[SerdeJsonSerializable]
public partial struct StringDecimalType(decimal value)
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<decimal>))]
    public decimal value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableBoolType(bool? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<bool?>))]
    public bool? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableByteType(byte? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<byte?>))]
    public byte? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableSbyteType(sbyte? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<sbyte?>))]
    public sbyte? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableShortType(short? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<short?>))]
    public short? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableUshortType(ushort? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<ushort?>))]
    public ushort? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableIntType(int? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<int?>))]
    public int? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableUintType(uint? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<uint?>))]
    public uint? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableLongType(long? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<long?>))]
    public long? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableUlongType(ulong? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<ulong?>))]
    public ulong? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableFloatType(float? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<float?>))]
    public float? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableDoubleType(double? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<double?>))]
    public double? value = value;
}

[SerdeJsonSerializable]
public partial struct StringNullableDecimalType(decimal? value)
{
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    [JsonConverter(typeof(StringPrimitiveConverter<decimal?>))]
    public decimal? value = value;
}

[SerdeJsonSerializable]
public partial struct GenericTypeCollection(List<GenericType<int>> data) : IEquatable<GenericTypeCollection>
{
    public List<GenericType<int>> data = data;

    // ReSharper disable once UsageOfDefaultStructEquality
    public bool Equals(GenericTypeCollection other) => data.SequenceEqual(other.data);

    public override bool Equals(object obj) => obj is GenericTypeCollection other && Equals(other);

    public readonly override int GetHashCode() => data != null ? data.GetHashCode() : 0;
}

internal sealed class StringPrimitiveConverter<T> : JsonConverter<T>
{
    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, T value, JsonSerializer serializer) => writer.WriteValue(value.ToString().ToLower());

    public override T ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        reader.Value == null ? default : (T) Convert.ChangeType(reader.Value, typeof(T));
}

internal sealed class DictionaryConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>>
{
    /// <inheritdoc />
    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Dictionary<TKey, TValue> dict, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        foreach (var item in dict)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(item.Key.ToString());
            serializer.Serialize(writer, item.Value);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }

    /// <inheritdoc />
    public override Dictionary<TKey, TValue> ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Dictionary<TKey, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var dict = new Dictionary<TKey, TValue>();
        if (reader.TokenType != JsonToken.StartArray)
        {
            return dict;
        }
        reader.Read();
        while (reader.TokenType != JsonToken.EndArray)
        {
            reader.Read();
            var key = (TKey) Convert.ChangeType(reader.Value, typeof(TKey));
            reader.Read();
            var value = serializer.Deserialize<TValue>(reader);
            dict.Add(key!, value);
            reader.Read();
            reader.Read();
        }
        return dict;
    }
}