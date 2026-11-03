/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/25
 */

using UnityEngine;
using NUnit.Framework;
using PeroTools2.Serde.Share.Editor.Tests;

namespace PeroTools2.Serde.Json.Editor.Tests
{
    public sealed class BuiltinFormattersTests
    {
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2TypeSerializeTest(Encoding encoding)
        {
            var obj = new Vector2Type(new Vector2(0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector2Type>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2TypeDeserializeTest(Encoding encoding)
        {
            var obj = new Vector2Type(new Vector2(0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<Vector2Type>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<Vector2Type>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2TypeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector2Type(new Vector2(0.5f, 0.5f));
            // Warm up once so pooled buffers are already rented.
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // Serializing produces a string, so an allocation is expected.
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, Vector2Type serializeObj)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Serialize(serializeObj);
                }
                else
                {
                    JsonConverter.UTF16.Serialize(serializeObj);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2TypeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector2Type(new Vector2(0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // Warm up once to cover pool allocation.
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<Vector2Type>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<Vector2Type>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3TypeSerializeTest(Encoding encoding)
        {
            var obj = new Vector3Type(new Vector3(0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector3Type>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3TypeDeserializeTest(Encoding encoding)
        {
            var obj = new Vector3Type(new Vector3(0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<Vector3Type>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<Vector3Type>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3TypeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector3Type(new Vector3(0.5f, 0.5f, 0.5f));
            // Warm up once so pooled buffers are already rented.
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // Serializing produces a string, so an allocation is expected.
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, Vector3Type serializeObj)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Serialize(serializeObj);
                }
                else
                {
                    JsonConverter.UTF16.Serialize(serializeObj);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3TypeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector3Type(new Vector3(0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // Warm up once to cover pool allocation.
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<Vector3Type>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<Vector3Type>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector4TypeSerializeTest(Encoding encoding)
        {
            var obj = new Vector4Type(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector4Type>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector4TypeDeserializeTest(Encoding encoding)
        {
            var obj = new Vector4Type(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<Vector4Type>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<Vector4Type>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector4TypeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector4Type(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            // Warm up once so pooled buffers are already rented.
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // Serializing produces a string, so an allocation is expected.
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, Vector4Type serializeObj)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Serialize(serializeObj);
                }
                else
                {
                    JsonConverter.UTF16.Serialize(serializeObj);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector4TypeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector4Type(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // Warm up once to cover pool allocation.
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<Vector4Type>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<Vector4Type>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2IntTypeSerializeTest(Encoding encoding)
        {
            var obj = new Vector2IntType(new Vector2Int(12, 34));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector2IntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2IntTypeDeserializeTest(Encoding encoding)
        {
            var obj = new Vector2IntType(new Vector2Int(12, 34));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<Vector2IntType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<Vector2IntType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2IntTypeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector2IntType(new Vector2Int(12, 34));
            // Warm up once so pooled buffers are already rented.
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // Serializing produces a string, so an allocation is expected.
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, Vector2IntType serializeObj)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Serialize(serializeObj);
                }
                else
                {
                    JsonConverter.UTF16.Serialize(serializeObj);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2IntTypeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector2IntType(new Vector2Int(12, 34));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // Warm up once to cover pool allocation.
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<Vector2IntType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<Vector2IntType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3IntTypeSerializeTest(Encoding encoding)
        {
            var obj = new Vector3IntType(new Vector3Int(12, 34, 56));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector3IntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3IntTypeDeserializeTest(Encoding encoding)
        {
            var obj = new Vector3IntType(new Vector3Int(12, 34, 56));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<Vector3IntType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<Vector3IntType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3IntTypeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector3IntType(new Vector3Int(12, 34, 56));
            // Warm up once so pooled buffers are already rented.
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // Serializing produces a string, so an allocation is expected.
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, Vector3IntType serializeObj)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Serialize(serializeObj);
                }
                else
                {
                    JsonConverter.UTF16.Serialize(serializeObj);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3IntTypeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector3IntType(new Vector3Int(12, 34, 56));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // Warm up once to cover pool allocation.
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<Vector3IntType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<Vector3IntType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void QuaternionTypeSerializeTest(Encoding encoding)
        {
            var obj = new QuaternionType(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<QuaternionType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void QuaternionTypeDeserializeTest(Encoding encoding)
        {
            var obj = new QuaternionType(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<QuaternionType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<QuaternionType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void QuaternionTypeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new QuaternionType(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f));
            // Warm up once so pooled buffers are already rented.
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // Serializing produces a string, so an allocation is expected.
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, QuaternionType serializeObj)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Serialize(serializeObj);
                }
                else
                {
                    JsonConverter.UTF16.Serialize(serializeObj);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void QuaternionTypeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new QuaternionType(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // Warm up once to cover pool allocation.
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<QuaternionType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<QuaternionType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ColorTypeSerializeTest(Encoding encoding)
        {
            var obj = new ColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ColorType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ColorTypeDeserializeTest(Encoding encoding)
        {
            var obj = new ColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<ColorType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<ColorType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ColorTypeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new ColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            // Warm up once so pooled buffers are already rented.
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // Serializing produces a string, so an allocation is expected.
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, ColorType serializeObj)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Serialize(serializeObj);
                }
                else
                {
                    JsonConverter.UTF16.Serialize(serializeObj);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ColorTypeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new ColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // Warm up once to cover pool allocation.
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<ColorType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<ColorType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void HexColorTypeSerializeTest(Encoding encoding)
        {
            var obj = new HexColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<HexColorType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void HexColorTypeDeserializeTest(Encoding encoding)
        {
            var obj = new HexColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<HexColorType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<HexColorType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void HexColorTypeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new HexColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            // Warm up once so pooled buffers are already rented.
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // Serializing produces a string, so an allocation is expected.
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, HexColorType serializeObj)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Serialize(serializeObj);
                }
                else
                {
                    JsonConverter.UTF16.Serialize(serializeObj);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void HexColorTypeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new HexColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // Warm up once to cover pool allocation.
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<HexColorType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<HexColorType>(jsonText);
                }
            }
        }
    }
}