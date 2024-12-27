/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/25
 * Description: SerdeSerializer
 */

using System;
using Newtonsoft.Json;
using UnityEngine;
using PeroTools2.Serde.Json;
using JsonReader = Newtonsoft.Json.JsonReader;
using JsonWriter = Newtonsoft.Json.JsonWriter;

namespace PeroTools2.Serde.Editor.Tests
{
    public enum Encoding
    {
        UTF8,
        UTF16
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
    public partial struct DateTimeType
    {
        public DateTime value;

        public DateTimeType(DateTime value) => this.value = value;
    }

    [SerdeJsonSerializable]
    public partial struct UnixSecondDateTimeType : IEquatable<UnixSecondDateTimeType>
    {
        [SerdeJsonFormatter(typeof(UnixSecondsFormatter))]
        [JsonConverter(typeof(UnixSecondsConverter))]
        public DateTime value;

        public UnixSecondDateTimeType(DateTime value) => this.value = value;

        private sealed class UnixSecondsConverter : JsonConverter<DateTime>
        {
            public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
                => writer.WriteValue(DateTimeHelper.DateTimeToUnixTimeSeconds(value));

            public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
                => DateTimeHelper.UnixTimeSecondsToDateTime((long) reader.Value);
        }

        public bool Equals(UnixSecondDateTimeType other) => other.value.ToString() == value.ToString();

        public override bool Equals(object obj) => obj is UnixMillisecondDateTimeType other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();
    }

    [SerdeJsonSerializable]
    public partial struct UnixMillisecondDateTimeType : IEquatable<UnixMillisecondDateTimeType>
    {
        [SerdeJsonFormatter(typeof(UnixMillisecondFormatter))]
        [JsonConverter(typeof(UnixMillisecondConverter))]
        public DateTime value;

        public UnixMillisecondDateTimeType(DateTime value) => this.value = value;

        private sealed class UnixMillisecondConverter : JsonConverter<DateTime>
        {
            public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
                => writer.WriteValue(DateTimeHelper.DateTimeToUnixTimeMilliseconds(value));

            public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
                => DateTimeHelper.UnixTimeMillisecondsToDateTime((long) reader.Value);
        }

        public bool Equals(UnixMillisecondDateTimeType other) => other.value.ToString() == value.ToString();

        public override bool Equals(object obj) => obj is UnixMillisecondDateTimeType other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();
    }

    [SerdeJsonSerializable]
    public partial struct Vector2Type
    {
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 value;

        public Vector2Type(Vector2 value) => this.value = value;

        private sealed class Vector2Converter : JsonConverter<Vector2>
        {
            public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.x},{value.y})");
            }

            public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
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
            public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.x},{value.y},{value.z})");
            }

            public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
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
            public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.x},{value.y},{value.z},{value.w})");
            }

            public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
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
            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
            {
                writer.WriteValue($"({value.r},{value.g},{value.b},{value.a})");
            }

            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
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
            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
                => writer.WriteValue($"#{ColorUtility.ToHtmlStringRGBA(value)}");

            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
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
}