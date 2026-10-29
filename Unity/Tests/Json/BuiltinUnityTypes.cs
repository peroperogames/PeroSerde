/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Unity-only test types: they wrap UnityEngine value types and therefore cannot compile outside Unity.
 */

using System;
using Newtonsoft.Json;
using UnityEngine;

namespace PeroTools2.Serde.Json.Editor.Tests
{
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
}