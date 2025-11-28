/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/25
 * Description: SerdeSerializer
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace PeroTools2.Serde.Json.Editor.Tests
{
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
    public partial struct BoolType
    {
        public bool value;
        public BoolType(bool value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct CharType
    {
        public char value;
        public CharType(char value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct ByteType
    {
        public byte value;
        public ByteType(byte value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct SbyteType
    {
        public sbyte value;
        public SbyteType(sbyte value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct ShortType
    {
        public short value;
        public ShortType(short value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct UshortType
    {
        public ushort value;
        public UshortType(ushort value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct IntType
    {
        public int value;
        public IntType(int value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct UintType
    {
        public uint value;
        public UintType(uint value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct LongType
    {
        public long value;
        public LongType(long value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct UlongType
    {
        public ulong value;
        public UlongType(ulong value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct FloatType
    {
        public float value;
        public FloatType(float value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct DoubleType
    {
        public double value;
        public DoubleType(double value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct DecimalType
    {
        public decimal value;
        public DecimalType(decimal value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableBoolType
    {
        public bool? value;
        public NullableBoolType(bool? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableCharType
    {
        public char? value;
        public NullableCharType(char? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableByteType
    {
        public byte? value;
        public NullableByteType(byte? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableSbyteType
    {
        public sbyte? value;
        public NullableSbyteType(sbyte? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableShortType
    {
        public short? value;
        public NullableShortType(short? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableUshortType
    {
        public ushort? value;
        public NullableUshortType(ushort? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableIntType
    {
        public int? value;
        public NullableIntType(int? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableUintType
    {
        public uint? value;
        public NullableUintType(uint? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableLongType
    {
        public long? value;
        public NullableLongType(long? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableUlongType
    {
        public ulong? value;
        public NullableUlongType(ulong? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableFloatType
    {
        public float? value;
        public NullableFloatType(float? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableDoubleType
    {
        public double? value;
        public NullableDoubleType(double? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NullableDecimalType
    {
        public decimal? value;
        public NullableDecimalType(decimal? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringType
    {
        public string value;
        public StringType(string value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct DateTimeType : IEquatable<DateTimeType>
    {
        public DateTime value;

        public DateTimeType(DateTime value) => this.value = value;

        public bool Equals(DateTimeType other) => value == other.value || value.ToString(CultureInfo.CurrentCulture) == other.value.ToString(CultureInfo.CurrentCulture);

        public override bool Equals(object obj) => obj is DateTimeType other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();
    }

    [SerdeJsonSerializable]
    public partial struct UnixSecondDateTimeType : IEquatable<UnixSecondDateTimeType>
    {
        [SerdeJsonFormatter(typeof(UnixSecondsFormatter))]
        [JsonConverter(typeof(UnixSecondsConverter))]
        public DateTime value;

        public UnixSecondDateTimeType(DateTime value) => this.value = value;

        public bool Equals(UnixSecondDateTimeType other) => other.value.ToString(CultureInfo.CurrentCulture) == value.ToString(CultureInfo.CurrentCulture);

        public override bool Equals(object obj) => obj is UnixMillisecondDateTimeType other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();

        private sealed class UnixSecondsConverter : JsonConverter<DateTime>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, DateTime value, JsonSerializer serializer)
                => writer.WriteValue(DateTimeHelper.DateTimeToUnixTimeSeconds(value));

            public override DateTime ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
                => DateTimeHelper.UnixTimeSecondsToDateTime((long) reader.Value);
        }
    }

    [SerdeJsonSerializable]
    public partial struct UnixMillisecondDateTimeType : IEquatable<UnixMillisecondDateTimeType>
    {
        [SerdeJsonFormatter(typeof(UnixMillisecondsFormatter))]
        [JsonConverter(typeof(UnixMillisecondConverter))]
        public DateTime value;

        public UnixMillisecondDateTimeType(DateTime value) => this.value = value;

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
    public partial struct Vector2Type
    {
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 value;

        public Vector2Type(Vector2 value) => this.value = value;

        private sealed class Vector2Converter : JsonConverter<Vector2>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Vector2 value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.x},{value.y})");
            }

            public override Vector2 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var rgba = (reader.Value as string)?.Replace("(", "").Replace(")", "").Split(',');
                if (rgba == null) return new Vector2(0, 0);
                return new Vector2(float.Parse(rgba[0]), float.Parse(rgba[1]));
            }
        }
    }

    [SerdeJsonSerializable]
    public partial struct Vector3Type
    {
        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 value;

        public Vector3Type(Vector3 value) => this.value = value;

        private sealed class Vector3Converter : JsonConverter<Vector3>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Vector3 value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.x},{value.y},{value.z})");
            }

            public override Vector3 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var rgba = (reader.Value as string)?.Replace("(", "").Replace(")", "").Split(',');
                if (rgba == null) return new Vector3(0, 0, 0);
                return new Vector3(float.Parse(rgba[0]), float.Parse(rgba[1]), float.Parse(rgba[2]));
            }
        }
    }

    [SerdeJsonSerializable]
    public partial struct Vector2IntType
    {
        [JsonConverter(typeof(Vector2IntConverter))]
        public Vector2Int value;

        public Vector2IntType(Vector2Int value) => this.value = value;

        private sealed class Vector2IntConverter : JsonConverter<Vector2Int>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Vector2Int value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.x},{value.y})");
            }

            public override Vector2Int ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var rgba = (reader.Value as string)?.Replace("(", "").Replace(")", "").Split(',');
                if (rgba == null) return new Vector2Int(0, 0);
                return new Vector2Int(int.Parse(rgba[0]), int.Parse(rgba[1]));
            }
        }
    }

    [SerdeJsonSerializable]
    public partial struct Vector3IntType
    {
        [JsonConverter(typeof(Vector3IntConverter))]
        public Vector3Int value;

        public Vector3IntType(Vector3Int value) => this.value = value;

        private sealed class Vector3IntConverter : JsonConverter<Vector3Int>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Vector3Int value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.x},{value.y},{value.z})");
            }

            public override Vector3Int ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Vector3Int existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var rgba = (reader.Value as string)?.Replace("(", "").Replace(")", "").Split(',');
                if (rgba == null) return new Vector3Int(0, 0, 0);
                return new Vector3Int(int.Parse(rgba[0]), int.Parse(rgba[1]), int.Parse(rgba[2]));
            }
        }
    }

    [SerdeJsonSerializable]
    public partial struct Vector4Type
    {
        [JsonConverter(typeof(Vector4Converter))]
        public Vector4 value;

        public Vector4Type(Vector4 value) => this.value = value;

        private sealed class Vector4Converter : JsonConverter<Vector4>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Vector4 value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.x},{value.y},{value.z},{value.w})");
            }

            public override Vector4 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var rgba = (reader.Value as string)?.Replace("(", "").Replace(")", "").Split(',');
                if (rgba == null) return new Vector4(0, 0, 0, 0);
                return new Vector4(float.Parse(rgba[0]), float.Parse(rgba[1]), float.Parse(rgba[2]), float.Parse(rgba[3]));
            }
        }
    }

    [SerdeJsonSerializable]
    public partial struct QuaternionType
    {
        [JsonConverter(typeof(QuaternionConverter))]
        public Quaternion value;

        public QuaternionType(Quaternion value) => this.value = value;

        private sealed class QuaternionConverter : JsonConverter<Quaternion>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Quaternion value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.x},{value.y},{value.z},{value.w})");
            }

            public override Quaternion ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var rgba = (reader.Value as string)?.Replace("(", "").Replace(")", "").Split(',');
                if (rgba == null) return new Quaternion(0, 0, 0, 0);
                return new Quaternion(float.Parse(rgba[0]), float.Parse(rgba[1]), float.Parse(rgba[2]), float.Parse(rgba[3]));
            }
        }
    }

    [SerdeJsonSerializable]
    public partial struct ColorType
    {
        [JsonConverter(typeof(ColorConverter))]
        public Color value;

        public ColorType(Color value) => this.value = value;

        private sealed class ColorConverter : JsonConverter<Color>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Color value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.r},{value.g},{value.b},{value.a})");
            }

            public override Color ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var rgba = (reader.Value as string)?.Replace("(", "").Replace(")", "").Split(',');
                if (rgba == null) return Color.clear;
                return new Color(float.Parse(rgba[0]), float.Parse(rgba[1]), float.Parse(rgba[2]), float.Parse(rgba[3]));
            }
        }
    }

    [SerdeJsonSerializable]
    public partial struct HexColorType : IEquatable<HexColorType>
    {
        [SerdeJsonFormatter(typeof(HexColorFormatter))]
        [JsonConverter(typeof(HexColorConverter))]
        public Color value;

        public HexColorType(Color value) => this.value = value;

        public override bool Equals(object obj) => obj is HexColorType other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();

        public bool Equals(HexColorType other)
            => Mathf.Abs(other.value.r - value.r) < 0.01f &&
               Mathf.Abs(other.value.r - value.g) < 0.01f &&
               Mathf.Abs(other.value.r - value.b) < 0.01f &&
               Mathf.Abs(other.value.r - value.a) < 0.01f;

        private sealed class HexColorConverter : JsonConverter<Color>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Color value, JsonSerializer serializer)
                => writer.WriteValue($"#{ColorUtility.ToHtmlStringRGBA(value)}");

            public override Color ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
                => reader.Value is not string color || !ColorUtility.TryParseHtmlString(color, out var result)
                    ? Color.clear
                    : result;
        }
    }

    [SerdeJsonSerializable]
    public partial struct GuidType
    {
        public Guid value;
        public GuidType(Guid value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct ListNumberType : IEquatable<ListNumberType>
    {
        public List<int> value;
        public ListNumberType(List<int> value) => this.value = value;

        public bool Equals(ListNumberType other) => value.SequenceEqual(other.value);

        public override bool Equals(object obj) => obj is ListNumberType other && Equals(other);

        public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
    }

    [SerdeJsonSerializable]
    public partial struct ListObjectType : IEquatable<ListObjectType>
    {
        public List<BoolType> value;
        public ListObjectType(List<BoolType> value) => this.value = value;

        // ReSharper disable once UsageOfDefaultStructEquality
        public bool Equals(ListObjectType other) => value.SequenceEqual(other.value);

        public override bool Equals(object obj) => obj is ListObjectType other && Equals(other);

        public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
    }

    [SerdeJsonSerializable]
    public partial struct DictionaryStringType : IEquatable<DictionaryStringType>
    {
        public Dictionary<string, BoolType> value;

        public DictionaryStringType(Dictionary<string, BoolType> value) => this.value = value;

        public bool Equals(DictionaryStringType other) =>
            value != null && other.value != null &&
            value.Count == other.value.Count &&
            // ReSharper disable once UsageOfDefaultStructEquality
            !value.Except(other.value).Any();

        public override bool Equals(object obj) => obj is DictionaryStringType other && Equals(other);

        public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
    }

    [SerdeJsonSerializable]
    public partial struct DictionaryNumberType : IEquatable<DictionaryNumberType>
    {
        [SerdeJsonFormatter(typeof(DictionaryFormatter<int, BoolType>))]
        public Dictionary<int, BoolType> value;

        public DictionaryNumberType(Dictionary<int, BoolType> value) => this.value = value;

        public bool Equals(DictionaryNumberType other) =>
            value != null && other.value != null &&
            value.Count == other.value.Count &&
            // ReSharper disable once UsageOfDefaultStructEquality
            !value.Except(other.value).Any();

        public override bool Equals(object obj) => obj is DictionaryNumberType other && Equals(other);

        public override int GetHashCode() => value != null ? value.GetHashCode() : 0;
    }

    [SerdeJsonSerializable]
    public partial struct NumberEnumType
    {
        public NumberEnum value;
        public NumberEnumType(NumberEnum value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringEnumType
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public StringEnum value;

        public StringEnumType(StringEnum value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNumberEnumType
    {
        [JsonConverter(typeof(StringNumberEnumConverter))]
        public StringNumberEnum value;

        public StringNumberEnumType(StringNumberEnum value) => this.value = value;

        private sealed class StringNumberEnumConverter : JsonConverter<StringNumberEnum>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, StringNumberEnum value, JsonSerializer serializer)
                => writer.WriteValue((long) value);

            public override StringNumberEnum ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, StringNumberEnum existingValue, bool hasExistingValue, JsonSerializer serializer)
                => Enum.Parse<StringNumberEnum>(reader.Value!.ToString());
        }
    }

    [SerdeJsonSerializable]
    public partial struct NumberStringEnumType
    {
        [JsonConverter(typeof(NumberStringEnumConverter))]
        public NumberStringEnum value;

        public NumberStringEnumType(NumberStringEnum value) => this.value = value;

        private sealed class NumberStringEnumConverter : JsonConverter<NumberStringEnum>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, NumberStringEnum value, JsonSerializer serializer)
                => writer.WriteValue(value.ToString());

            public override NumberStringEnum ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, NumberStringEnum existingValue, bool hasExistingValue, JsonSerializer serializer)
                => (NumberStringEnum) (long) reader.Value!;
        }
    }

    [SerdeJsonSerializable]
    public partial struct NumberFlagsType
    {
        public NumberFlags value;
        public NumberFlagsType(NumberFlags value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringFlagsType
    {
        [JsonConverter(typeof(StringFlagsConverter))]
        public StringFlags value;

        public StringFlagsType(StringFlags value) => this.value = value;

        private sealed class StringFlagsConverter : JsonConverter<StringFlags>
        {
            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, StringFlags value, JsonSerializer serializer)
                => writer.WriteValue(value.ToString().Replace(", ", ","));

            public override StringFlags ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, StringFlags existingValue, bool hasExistingValue, JsonSerializer serializer)
                => Enum.Parse<StringFlags>(reader.Value!.ToString());
        }
    }

    [SerdeJsonSerializable]
    public partial struct LazyType : IEquatable<LazyType>
    {
        [JsonConverter(typeof(LazyConverter))]
        public Lazy<string> value;

        public LazyType(Lazy<string> value) => this.value = value;

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
    public partial struct NullableType
    {
        public Nullable<BoolType> value;
        public NullableType(BoolType? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct GenericType<T>
    {
        public T data;
        public GenericType(T data) => this.data = data;
    }

    [SerdeJsonSerializable]
    public partial struct StringBoolType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<bool>))]
        public bool value;

        public StringBoolType(bool value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringByteType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<byte>))]
        public byte value;

        public StringByteType(byte value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringSbyteType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<sbyte>))]
        public sbyte value;

        public StringSbyteType(sbyte value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringShortType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<short>))]
        public short value;

        public StringShortType(short value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringUshortType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<ushort>))]
        public ushort value;

        public StringUshortType(ushort value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringIntType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<int>))]
        public int value;

        public StringIntType(int value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringUintType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<uint>))]
        public uint value;

        public StringUintType(uint value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringLongType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<long>))]
        public long value;

        public StringLongType(long value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringUlongType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<ulong>))]
        public ulong value;

        public StringUlongType(ulong value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringFloatType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<float>))]
        public float value;

        public StringFloatType(float value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringDoubleType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<double>))]
        public double value;

        public StringDoubleType(double value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringDecimalType
    {
        [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<decimal>))]
        public decimal value;

        public StringDecimalType(decimal value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableBoolType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<bool?>))]
        public bool? value;

        public StringNullableBoolType(bool? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableByteType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<byte?>))]
        public byte? value;

        public StringNullableByteType(byte? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableSbyteType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<sbyte?>))]
        public sbyte? value;

        public StringNullableSbyteType(sbyte? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableShortType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<short?>))]
        public short? value;

        public StringNullableShortType(short? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableUshortType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<ushort?>))]
        public ushort? value;

        public StringNullableUshortType(ushort? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableIntType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<int?>))]
        public int? value;

        public StringNullableIntType(int? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableUintType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<uint?>))]
        public uint? value;

        public StringNullableUintType(uint? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableLongType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<long?>))]
        public long? value;

        public StringNullableLongType(long? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableUlongType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<ulong?>))]
        public ulong? value;

        public StringNullableUlongType(ulong? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableFloatType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<float?>))]
        public float? value;

        public StringNullableFloatType(float? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableDoubleType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<double?>))]
        public double? value;

        public StringNullableDoubleType(double? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct StringNullableDecimalType
    {
        [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
        [JsonConverter(typeof(StringPrimitiveConverter<decimal?>))]
        public decimal? value;

        public StringNullableDecimalType(decimal? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct GenericTypeCollection : IEquatable<GenericTypeCollection>
    {
        public List<GenericType<int>> data;
        public GenericTypeCollection(List<GenericType<int>> data) => this.data = data;

        // ReSharper disable once UsageOfDefaultStructEquality
        public bool Equals(GenericTypeCollection other) => data.SequenceEqual(other.data);

        public override bool Equals(object obj) => obj is GenericTypeCollection other && Equals(other);

        public readonly override int GetHashCode() => data != null ? data.GetHashCode() : 0;
    }

    internal sealed class StringPrimitiveConverter<T> : JsonConverter<T>
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, T value, JsonSerializer serializer) => writer.WriteValue(value.ToString().ToLower());

        public override T ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer) => reader.Value == null ? default : (T) Convert.ChangeType(reader.Value, typeof(T));
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
}