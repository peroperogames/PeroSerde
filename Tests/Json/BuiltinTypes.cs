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
    public partial struct NuallableCharType
    {
        public char? value;
        public NuallableCharType(char? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableByteType
    {
        public byte? value;
        public NuallableByteType(byte? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableSbyteType
    {
        public sbyte? value;
        public NuallableSbyteType(sbyte? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableShortType
    {
        public short? value;
        public NuallableShortType(short? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableUshortType
    {
        public ushort? value;
        public NuallableUshortType(ushort? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableIntType
    {
        public int? value;
        public NuallableIntType(int? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableUintType
    {
        public uint? value;
        public NuallableUintType(uint? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableLongType
    {
        public long? value;
        public NuallableLongType(long? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableUlongType
    {
        public ulong? value;
        public NuallableUlongType(ulong? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableFloatType
    {
        public float? value;
        public NuallableFloatType(float? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableDoubleType
    {
        public double? value;
        public NuallableDoubleType(double? value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct NuallableDecimalType
    {
        public decimal? value;
        public NuallableDecimalType(decimal? value) => this.value = value;
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
        [SerdeJsonFormatter(typeof(DictionaryFormatter<string, BoolType>))]
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

    /// <summary>
    /// 用来测试非NewtonSoftJsonStyle的字典序列化的结构体
    /// </summary>
    [SerdeJsonSerializable]
    public partial struct NonNewtonSoftJsonStyleDictionaryNumberType : IEquatable<NonNewtonSoftJsonStyleDictionaryNumberType>
    {
        public Dictionary<int, BoolType> value;

        public NonNewtonSoftJsonStyleDictionaryNumberType(Dictionary<int, BoolType> value) => this.value = value;

        public bool Equals(NonNewtonSoftJsonStyleDictionaryNumberType other) =>
            value != null && other.value != null &&
            value.Count == other.value.Count &&
            // ReSharper disable once UsageOfDefaultStructEquality
            !value.Except(other.value).Any();

        public override bool Equals(object obj) => obj is NonNewtonSoftJsonStyleDictionaryNumberType other && Equals(other);

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
}