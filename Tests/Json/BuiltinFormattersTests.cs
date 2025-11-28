/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/25
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using PeroTools2.Serde.Editor.Tests;

namespace PeroTools2.Serde.Json.Editor.Tests
{
    public sealed class BuiltinFormattersTests
    {
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void BoolSerializeTest(Encoding encoding)
        {
            var obj = new BoolType(true);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<BoolType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void BoolDeserializeTest(Encoding encoding)
        {
            var obj = new BoolType(true);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<BoolType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<BoolType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void BoolSerializeNoGCTest(Encoding encoding)
        {
            var obj = new BoolType(true);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, BoolType serializeObj)
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
        public void BoolDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new BoolType(true);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<BoolType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<BoolType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CharSerializeTest(Encoding encoding)
        {
            var obj = new CharType('\ufff0');
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<CharType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CharDeserializeTest(Encoding encoding)
        {
            var obj = new CharType('\ufff0');
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<CharType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<CharType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CharSerializeNoGCTest(Encoding encoding)
        {
            var obj = new CharType('\ufff0');
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, CharType serializeObj)
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
        public void CharDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new CharType('\ufff0');
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<CharType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<CharType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ByteSerializeTest(Encoding encoding)
        {
            var obj = new ByteType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ByteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ByteDeserializeTest(Encoding encoding)
        {
            var obj = new ByteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<ByteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<ByteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ByteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new ByteType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, ByteType serializeObj)
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
        public void ByteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new ByteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<ByteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<ByteType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void SbyteSerializeTest(Encoding encoding)
        {
            var obj = new SbyteType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<SbyteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void SbyteDeserializeTest(Encoding encoding)
        {
            var obj = new SbyteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<SbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<SbyteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void SbyteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new SbyteType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, SbyteType serializeObj)
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
        public void SbyteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new SbyteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<SbyteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<SbyteType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ShortSerializeTest(Encoding encoding)
        {
            var obj = new ShortType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ShortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ShortDeserializeTest(Encoding encoding)
        {
            var obj = new ShortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<ShortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<ShortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ShortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new ShortType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, ShortType serializeObj)
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
        public void ShortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new ShortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<ShortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<ShortType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UshortSerializeTest(Encoding encoding)
        {
            var obj = new UshortType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UshortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UshortDeserializeTest(Encoding encoding)
        {
            var obj = new UshortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<UshortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<UshortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UshortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new UshortType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, UshortType serializeObj)
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
        public void UshortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new UshortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<UshortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<UshortType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void IntSerializeTest(Encoding encoding)
        {
            var obj = new IntType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<IntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void IntDeserializeTest(Encoding encoding)
        {
            var obj = new IntType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<IntType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<IntType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void IntSerializeNoGCTest(Encoding encoding)
        {
            var obj = new IntType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, IntType serializeObj)
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
        public void IntDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new IntType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<IntType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<IntType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UintSerializeTest(Encoding encoding)
        {
            var obj = new UintType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UintType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UintDeserializeTest(Encoding encoding)
        {
            var obj = new UintType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<UintType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<UintType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UintSerializeNoGCTest(Encoding encoding)
        {
            var obj = new UintType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, UintType serializeObj)
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
        public void UintDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new UintType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<UintType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<UintType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void LongSerializeTest(Encoding encoding)
        {
            var obj = new LongType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<LongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void LongDeserializeTest(Encoding encoding)
        {
            var obj = new LongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<LongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<LongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void LongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new LongType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, LongType serializeObj)
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
        public void LongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new LongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<LongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<LongType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UlongSerializeTest(Encoding encoding)
        {
            var obj = new UlongType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UlongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UlongDeserializeTest(Encoding encoding)
        {
            var obj = new UlongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<UlongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<UlongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UlongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new UlongType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, UlongType serializeObj)
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
        public void UlongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new UlongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<UlongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<UlongType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void FloatSerializeTest(Encoding encoding)
        {
            var obj = new FloatType(10.4f);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<FloatType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void FloatDeserializeTest(Encoding encoding)
        {
            var obj = new FloatType(10.4f);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<FloatType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<FloatType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void FloatSerializeNoGCTest(Encoding encoding)
        {
            var obj = new FloatType(10.4f);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, FloatType serializeObj)
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
        public void FloatDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new FloatType(10.4f);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<FloatType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<FloatType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DoubleSerializeTest(Encoding encoding)
        {
            var obj = new DoubleType(10.4d);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DoubleType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DoubleDeserializeTest(Encoding encoding)
        {
            var obj = new DoubleType(10.4d);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<DoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<DoubleType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DoubleSerializeNoGCTest(Encoding encoding)
        {
            var obj = new DoubleType(10.4d);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, DoubleType serializeObj)
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
        public void DoubleDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new DoubleType(10.4d);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<DoubleType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<DoubleType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DecimalSerializeTest(Encoding encoding)
        {
            var obj = new DecimalType(13 / 2);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DecimalType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DecimalDeserializeTest(Encoding encoding)
        {
            var obj = new DecimalType(13 / 2);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<DecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<DecimalType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DecimalSerializeNoGCTest(Encoding encoding)
        {
            var obj = new DecimalType(13 / 2);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, DecimalType serializeObj)
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
        public void DecimalDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new DecimalType(13 / 2);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<DecimalType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<DecimalType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableBoolSerializeTest(Encoding encoding)
        {
            var obj = new NullableBoolType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableBoolType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableBoolDeserializeTest(Encoding encoding)
        {
            var obj = new NullableBoolType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableBoolType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableBoolType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableBoolSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableBoolType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableBoolType serializeObj)
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
        public void NullableBoolDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableBoolType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableBoolType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableBoolType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableCharSerializeTest(Encoding encoding)
        {
            var obj = new NullableCharType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableCharType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableCharDeserializeTest(Encoding encoding)
        {
            var obj = new NullableCharType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableCharType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableCharType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableCharSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableCharType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableCharType serializeObj)
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
        public void NullableCharDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableCharType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableCharType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableCharType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableByteSerializeTest(Encoding encoding)
        {
            var obj = new NullableByteType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableByteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableByteDeserializeTest(Encoding encoding)
        {
            var obj = new NullableByteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableByteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableByteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableByteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableByteType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableByteType serializeObj)
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
        public void NullableByteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableByteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableByteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableByteType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableSbyteSerializeTest(Encoding encoding)
        {
            var obj = new NullableSbyteType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableSbyteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableSbyteDeserializeTest(Encoding encoding)
        {
            var obj = new NullableSbyteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableSbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableSbyteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableSbyteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableSbyteType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableSbyteType serializeObj)
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
        public void NullableSbyteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableSbyteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableSbyteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableSbyteType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableShortSerializeTest(Encoding encoding)
        {
            var obj = new NullableShortType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableShortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableShortDeserializeTest(Encoding encoding)
        {
            var obj = new NullableShortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableShortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableShortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableShortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableShortType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableShortType serializeObj)
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
        public void NullableShortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableShortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableShortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableShortType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableUshortSerializeTest(Encoding encoding)
        {
            var obj = new NullableUshortType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableUshortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableUshortDeserializeTest(Encoding encoding)
        {
            var obj = new NullableUshortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableUshortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableUshortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableUshortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableUshortType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableUshortType serializeObj)
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
        public void NullableUshortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableUshortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableUshortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableUshortType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableIntSerializeTest(Encoding encoding)
        {
            var obj = new NullableIntType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableIntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableIntDeserializeTest(Encoding encoding)
        {
            var obj = new NullableIntType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableIntType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableIntType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableIntSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableIntType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableIntType serializeObj)
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
        public void NullableIntDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableIntType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableIntType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableIntType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableUintSerializeTest(Encoding encoding)
        {
            var obj = new NullableUintType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableUintType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableUintDeserializeTest(Encoding encoding)
        {
            var obj = new NullableUintType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableUintType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableUintType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableUintSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableUintType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableUintType serializeObj)
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
        public void NullableUintDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableUintType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableUintType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableUintType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableLongSerializeTest(Encoding encoding)
        {
            var obj = new NullableLongType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableLongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableLongDeserializeTest(Encoding encoding)
        {
            var obj = new NullableLongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableLongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableLongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableLongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableLongType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableLongType serializeObj)
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
        public void NullableLongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableLongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableLongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableLongType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableUlongSerializeTest(Encoding encoding)
        {
            var obj = new NullableUlongType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableUlongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableUlongDeserializeTest(Encoding encoding)
        {
            var obj = new NullableUlongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableUlongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableUlongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableUlongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableUlongType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableUlongType serializeObj)
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
        public void NullableUlongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableUlongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableUlongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableUlongType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableFloatSerializeTest(Encoding encoding)
        {
            var obj = new NullableFloatType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableFloatType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableFloatDeserializeTest(Encoding encoding)
        {
            var obj = new NullableFloatType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableFloatType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableFloatType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableFloatSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableFloatType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableFloatType serializeObj)
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
        public void NullableFloatDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableFloatType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableFloatType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableFloatType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableDoubleSerializeTest(Encoding encoding)
        {
            var obj = new NullableDoubleType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableDoubleType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableDoubleDeserializeTest(Encoding encoding)
        {
            var obj = new NullableDoubleType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableDoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableDoubleType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableDoubleSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableDoubleType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableDoubleType serializeObj)
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
        public void NullableDoubleDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableDoubleType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableDoubleType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableDoubleType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableDecimalSerializeTest(Encoding encoding)
        {
            var obj = new NullableDecimalType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableDecimalType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableDecimalDeserializeTest(Encoding encoding)
        {
            var obj = new NullableDecimalType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableDecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableDecimalType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableDecimalSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableDecimalType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableDecimalType serializeObj)
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
        public void NullableDecimalDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableDecimalType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableDecimalType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableDecimalType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringBoolSerializeTest(Encoding encoding)
        {
            var obj = new StringBoolType(true);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringBoolType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringBoolDeserializeTest(Encoding encoding)
        {
            var obj = new StringBoolType(true);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringBoolType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringBoolType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringBoolSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringBoolType(true);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringBoolType serializeObj)
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
        public void StringBoolDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringBoolType(true);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringBoolType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringBoolType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringByteSerializeTest(Encoding encoding)
        {
            var obj = new StringByteType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            try
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<StringByteType>(json);
                Assert.AreEqual(obj, result);
            }
            catch
            {
                Debug.LogError(json);
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringByteDeserializeTest(Encoding encoding)
        {
            var obj = new StringByteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringByteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringByteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringByteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringByteType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringByteType serializeObj)
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
        public void StringByteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringByteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringByteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringByteType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringSbyteSerializeTest(Encoding encoding)
        {
            var obj = new StringSbyteType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringSbyteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringSbyteDeserializeTest(Encoding encoding)
        {
            var obj = new StringSbyteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringSbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringSbyteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringSbyteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringSbyteType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringSbyteType serializeObj)
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
        public void StringSbyteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringSbyteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringSbyteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringSbyteType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringShortSerializeTest(Encoding encoding)
        {
            var obj = new StringShortType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringShortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringShortDeserializeTest(Encoding encoding)
        {
            var obj = new StringShortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringShortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringShortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringShortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringShortType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringShortType serializeObj)
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
        public void StringShortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringShortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringShortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringShortType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringUshortSerializeTest(Encoding encoding)
        {
            var obj = new StringUshortType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringUshortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringUshortDeserializeTest(Encoding encoding)
        {
            var obj = new StringUshortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringUshortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringUshortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringUshortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringUshortType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringUshortType serializeObj)
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
        public void StringUshortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringUshortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringUshortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringUshortType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringIntSerializeTest(Encoding encoding)
        {
            var obj = new StringIntType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringIntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringIntDeserializeTest(Encoding encoding)
        {
            var obj = new StringIntType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringIntType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringIntType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringIntSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringIntType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringIntType serializeObj)
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
        public void StringIntDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringIntType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringIntType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringIntType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringUintSerializeTest(Encoding encoding)
        {
            var obj = new StringUintType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringUintType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringUintDeserializeTest(Encoding encoding)
        {
            var obj = new StringUintType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringUintType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringUintType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringUintSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringUintType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringUintType serializeObj)
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
        public void StringUintDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringUintType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringUintType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringUintType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringLongSerializeTest(Encoding encoding)
        {
            var obj = new StringLongType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringLongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringLongDeserializeTest(Encoding encoding)
        {
            var obj = new StringLongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringLongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringLongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringLongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringLongType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringLongType serializeObj)
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
        public void StringLongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringLongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringLongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringLongType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringUlongSerializeTest(Encoding encoding)
        {
            var obj = new StringUlongType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringUlongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringUlongDeserializeTest(Encoding encoding)
        {
            var obj = new StringUlongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringUlongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringUlongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringUlongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringUlongType(10);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringUlongType serializeObj)
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
        public void StringUlongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringUlongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringUlongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringUlongType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringFloatSerializeTest(Encoding encoding)
        {
            var obj = new StringFloatType(10.4f);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringFloatType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringFloatDeserializeTest(Encoding encoding)
        {
            var obj = new StringFloatType(10.4f);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringFloatType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringFloatType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringFloatSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringFloatType(10.4f);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringFloatType serializeObj)
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
        public void StringFloatDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringFloatType(10.4f);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringFloatType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringFloatType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringDoubleSerializeTest(Encoding encoding)
        {
            var obj = new StringDoubleType(10.4d);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringDoubleType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringDoubleDeserializeTest(Encoding encoding)
        {
            var obj = new StringDoubleType(10.4d);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringDoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringDoubleType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringDoubleSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringDoubleType(10.4d);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringDoubleType serializeObj)
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
        public void StringDoubleDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringDoubleType(10.4d);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringDoubleType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringDoubleType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringDecimalSerializeTest(Encoding encoding)
        {
            var obj = new StringDecimalType(13 / 2);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringDecimalType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringDecimalDeserializeTest(Encoding encoding)
        {
            var obj = new StringDecimalType(13 / 2);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringDecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringDecimalType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringDecimalSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringDecimalType(13 / 2);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringDecimalType serializeObj)
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
        public void StringDecimalDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringDecimalType(13 / 2);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringDecimalType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringDecimalType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableBoolSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableBoolType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableBoolType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableBoolDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableBoolType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableBoolType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableBoolType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableBoolSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableBoolType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableBoolType serializeObj)
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
        public void StringNullableBoolDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableBoolType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableBoolType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableBoolType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableByteSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableByteType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableByteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableByteDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableByteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableByteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableByteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableByteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableByteType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableByteType serializeObj)
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
        public void StringNullableByteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableByteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableByteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableByteType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableSbyteSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableSbyteType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableSbyteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableSbyteDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableSbyteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableSbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableSbyteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableSbyteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableSbyteType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableSbyteType serializeObj)
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
        public void StringNullableSbyteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableSbyteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableSbyteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableSbyteType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableShortSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableShortType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableShortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableShortDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableShortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableShortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableShortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableShortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableShortType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableShortType serializeObj)
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
        public void StringNullableShortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableShortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableShortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableShortType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableUshortSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableUshortType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableUshortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableUshortDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableUshortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableUshortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableUshortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableUshortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableUshortType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableUshortType serializeObj)
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
        public void StringNullableUshortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableUshortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableUshortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableUshortType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableIntSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableIntType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableIntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableIntDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableIntType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableIntType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableIntType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableIntSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableIntType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableIntType serializeObj)
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
        public void StringNullableIntDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableIntType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableIntType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableIntType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableUintSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableUintType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableUintType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableUintDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableUintType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableUintType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableUintType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableUintSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableUintType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableUintType serializeObj)
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
        public void StringNullableUintDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableUintType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableUintType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableUintType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableLongSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableLongType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableLongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableLongDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableLongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableLongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableLongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableLongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableLongType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableLongType serializeObj)
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
        public void StringNullableLongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableLongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableLongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableLongType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableUlongSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableUlongType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableUlongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableUlongDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableUlongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableUlongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableUlongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableUlongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableUlongType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableUlongType serializeObj)
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
        public void StringNullableUlongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableUlongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableUlongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableUlongType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableFloatSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableFloatType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableFloatType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableFloatDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableFloatType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableFloatType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableFloatType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableFloatSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableFloatType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableFloatType serializeObj)
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
        public void StringNullableFloatDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableFloatType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableFloatType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableFloatType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableDoubleSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableDoubleType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableDoubleType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableDoubleDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableDoubleType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableDoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableDoubleType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableDoubleSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableDoubleType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableDoubleType serializeObj)
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
        public void StringNullableDoubleDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableDoubleType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableDoubleType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableDoubleType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableDecimalSerializeTest(Encoding encoding)
        {
            var obj = new StringNullableDecimalType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableDecimalType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableDecimalDeserializeTest(Encoding encoding)
        {
            var obj = new StringNullableDecimalType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNullableDecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNullableDecimalType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNullableDecimalSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableDecimalType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNullableDecimalType serializeObj)
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
        public void StringNullableDecimalDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNullableDecimalType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNullableDecimalType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNullableDecimalType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringSerializeTest(Encoding encoding)
        {
            var obj = new StringType("String");
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringDeserializeTest(Encoding encoding)
        {
            var obj = new StringType("String");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringType("String");
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringType serializeObj)
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
        public void StringDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringType("String");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DateTimeSerializeTest(Encoding encoding)
        {
            var obj = new DateTimeType(DateTime.Now);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DateTimeType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DateTimeDeserializeTest(Encoding encoding)
        {
            var obj = new DateTimeType(DateTime.Now);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<DateTimeType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<DateTimeType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DateTimeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new DateTimeType(DateTime.Now);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, DateTimeType serializeObj)
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
        public void DateTimeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new DateTimeType(DateTime.Now);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<DateTimeType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<DateTimeType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixMillisecondDateTimeSerializeTest(Encoding encoding)
        {
            var obj = new UnixMillisecondDateTimeType(DateTime.Now);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UnixMillisecondDateTimeType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixMillisecondDateTimeDeserializeTest(Encoding encoding)
        {
            var obj = new UnixMillisecondDateTimeType(DateTime.Now);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<UnixMillisecondDateTimeType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<UnixMillisecondDateTimeType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixMillisecondDateTimeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new UnixMillisecondDateTimeType(DateTime.Now);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, UnixMillisecondDateTimeType serializeObj)
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
        public void UnixMillisecondDateTimeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new UnixMillisecondDateTimeType(DateTime.Now);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<UnixMillisecondDateTimeType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<UnixMillisecondDateTimeType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixSecondDateTimeSerializeTest(Encoding encoding)
        {
            var obj = new UnixSecondDateTimeType(DateTime.Now);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UnixSecondDateTimeType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixSecondDateTimeDeserializeTest(Encoding encoding)
        {
            var obj = new UnixSecondDateTimeType(DateTime.Now);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<UnixSecondDateTimeType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<UnixSecondDateTimeType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixSecondDateTimeSerializeNoGCTest(Encoding encoding)
        {
            var obj = new UnixSecondDateTimeType(DateTime.Now);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, UnixSecondDateTimeType serializeObj)
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
        public void UnixSecondDateTimeDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new UnixSecondDateTimeType(DateTime.Now);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<UnixSecondDateTimeType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<UnixSecondDateTimeType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2SerializeTest(Encoding encoding)
        {
            var obj = new Vector2Type(new Vector2(0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector2Type>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2DeserializeTest(Encoding encoding)
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
        public void Vector2SerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector2Type(new Vector2(0.5f, 0.5f));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
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
        public void Vector2DeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector2Type(new Vector2(0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
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
        public void Vector3SerializeTest(Encoding encoding)
        {
            var obj = new Vector3Type(new Vector3(0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector3Type>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3DeserializeTest(Encoding encoding)
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
        public void Vector3SerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector3Type(new Vector3(0.5f, 0.5f, 0.5f));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
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
        public void Vector3DeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector3Type(new Vector3(0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
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
        public void Vector4SerializeTest(Encoding encoding)
        {
            var obj = new Vector4Type(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector4Type>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector4DeserializeTest(Encoding encoding)
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
        public void Vector4SerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector4Type(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
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
        public void Vector4DeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector4Type(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
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
        public void Vector2IntSerializeTest(Encoding encoding)
        {
            var obj = new Vector2IntType(new Vector2Int(12, 34));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector2IntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2IntDeserializeTest(Encoding encoding)
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
        public void Vector2IntSerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector2IntType(new Vector2Int(12, 34));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
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
        public void Vector2IntDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector2IntType(new Vector2Int(12, 34));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
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
        public void Vector3IntSerializeTest(Encoding encoding)
        {
            var obj = new Vector3IntType(new Vector3Int(12, 34, 56));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector3IntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3IntDeserializeTest(Encoding encoding)
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
        public void Vector3IntSerializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector3IntType(new Vector3Int(12, 34, 56));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
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
        public void Vector3IntDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new Vector3IntType(new Vector3Int(12, 34, 56));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
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
        public void QuaternionSerializeTest(Encoding encoding)
        {
            var obj = new QuaternionType(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<QuaternionType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void QuaternionDeserializeTest(Encoding encoding)
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
        public void QuaternionSerializeNoGCTest(Encoding encoding)
        {
            var obj = new QuaternionType(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
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
        public void QuaternionDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new QuaternionType(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
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
        public void ColorSerializeTest(Encoding encoding)
        {
            var obj = new ColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ColorType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ColorDeserializeTest(Encoding encoding)
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
        public void ColorSerializeNoGCTest(Encoding encoding)
        {
            var obj = new ColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
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
        public void ColorDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new ColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
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
        public void HexColorSerializeTest(Encoding encoding)
        {
            var obj = new HexColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<HexColorType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void HexColorDeserializeTest(Encoding encoding)
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
        public void HexColorSerializeNoGCTest(Encoding encoding)
        {
            var obj = new HexColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
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
        public void HexColorDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new HexColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
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

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void GuidSerializeTest(Encoding encoding)
        {
            var obj = new GuidType(Guid.NewGuid());
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<GuidType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void GuidDeserializeTest(Encoding encoding)
        {
            var obj = new GuidType(Guid.NewGuid());
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<GuidType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<GuidType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void GuidSerializeNoGCTest(Encoding encoding)
        {
            var obj = new GuidType(Guid.NewGuid());
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, GuidType serializeObj)
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
        public void GuidDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new GuidType(Guid.NewGuid());
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<GuidType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<GuidType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ListNumberSerializeTest(Encoding encoding)
        {
            var obj = new ListNumberType(new List<int> { 1, 2, 3, 4 });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ListNumberType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ListNumberDeserializeTest(Encoding encoding)
        {
            var obj = new ListNumberType(new List<int> { 1, 2, 3, 4 });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<ListNumberType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<ListNumberType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ListNumberSerializeNoGCTest(Encoding encoding)
        {
            var obj = new ListNumberType(new List<int> { 1, 2, 3, 4 });
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, ListNumberType serializeObj)
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
        public void ListNumberDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new ListNumberType(new List<int> { 1, 2, 3, 4 });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(2));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<ListNumberType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<ListNumberType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ListObjectSerializeTest(Encoding encoding)
        {
            var obj = new ListObjectType(new List<BoolType> { new BoolType(true), new BoolType(false) });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ListObjectType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ListObjectDeserializeTest(Encoding encoding)
        {
            var obj = new ListObjectType(new List<BoolType> { new BoolType(true), new BoolType(false) });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<ListObjectType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<ListObjectType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ListObjectSerializeNoGCTest(Encoding encoding)
        {
            var obj = new ListObjectType(new List<BoolType> { new BoolType(true), new BoolType(false) });
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, ListObjectType serializeObj)
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
        public void ListObjectDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new ListObjectType(new List<BoolType> { new BoolType(true), new BoolType(false) });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(2));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<ListObjectType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<ListObjectType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DictionaryNumberSerializeTest(Encoding encoding)
        {
            var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { { 1, new BoolType(true) }, { 2, new BoolType(false) } });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DictionaryNumberType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DictionaryNumberDeserializeTest(Encoding encoding)
        {
            var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { { 1, new BoolType(true) }, { 2, new BoolType(false) } });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<DictionaryNumberType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<DictionaryNumberType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DictionaryNumberSerializeNoGCTest(Encoding encoding)
        {
            var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { { 1, new BoolType(true) }, { 2, new BoolType(false) } });
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, DictionaryNumberType serializeObj)
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
        public void DictionaryNumberDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { { 1, new BoolType(true) }, { 2, new BoolType(false) } });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(3));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<DictionaryNumberType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<DictionaryNumberType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DictionaryStringSerializeTest(Encoding encoding)
        {
            var obj = new DictionaryStringType(new Dictionary<string, BoolType> { { "True", new BoolType(true) }, { "False", new BoolType(false) } });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DictionaryStringType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DictionaryStringDeserializeTest(Encoding encoding)
        {
            var obj = new DictionaryStringType(new Dictionary<string, BoolType> { { "True", new BoolType(true) }, { "False", new BoolType(false) } });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<DictionaryStringType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<DictionaryStringType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DictionaryStringSerializeNoGCTest(Encoding encoding)
        {
            var obj = new DictionaryStringType(new Dictionary<string, BoolType> { { "True", new BoolType(true) }, { "False", new BoolType(false) } });
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, DictionaryStringType serializeObj)
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
        public void DictionaryStringDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new DictionaryStringType(new Dictionary<string, BoolType> { { "True", new BoolType(true) }, { "False", new BoolType(false) } });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(5));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<DictionaryStringType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<DictionaryStringType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NumberEnumSerializeTest(Encoding encoding)
        {
            var obj = new NumberEnumType(NumberEnum.BEnum);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NumberEnumType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NumberEnumDeserializeTest(Encoding encoding)
        {
            var obj = new NumberEnumType(NumberEnum.BEnum);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NumberEnumType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NumberEnumType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NumberEnumSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NumberEnumType(NumberEnum.BEnum);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NumberEnumType serializeObj)
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
        public void NumberEnumDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NumberEnumType(NumberEnum.BEnum);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NumberEnumType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NumberEnumType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringEnumSerializeTest(Encoding encoding)
        {
            var obj = new StringEnumType(StringEnum.BEnum);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringEnumType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringEnumDeserializeTest(Encoding encoding)
        {
            var obj = new StringEnumType(StringEnum.BEnum);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringEnumType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringEnumType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringEnumSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringEnumType(StringEnum.BEnum);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringEnumType serializeObj)
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
        public void StringEnumDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringEnumType(StringEnum.BEnum);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringEnumType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringEnumType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNumberEnumSerializeTest(Encoding encoding)
        {
            var obj = new StringNumberEnumType(StringNumberEnum.BEnum);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNumberEnumType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNumberEnumDeserializeTest(Encoding encoding)
        {
            var obj = new StringNumberEnumType(StringNumberEnum.BEnum);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringNumberEnumType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringNumberEnumType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringNumberEnumSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNumberEnumType(StringNumberEnum.BEnum);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringNumberEnumType serializeObj)
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
        public void StringNumberEnumDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringNumberEnumType(StringNumberEnum.BEnum);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringNumberEnumType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringNumberEnumType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NumberStringEnumSerializeTest(Encoding encoding)
        {
            var obj = new NumberStringEnumType(NumberStringEnum.BEnum);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NumberStringEnumType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NumberStringEnumDeserializeTest(Encoding encoding)
        {
            var obj = new NumberStringEnumType(NumberStringEnum.BEnum);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NumberStringEnumType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NumberStringEnumType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NumberStringEnumSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NumberStringEnumType(NumberStringEnum.BEnum);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NumberStringEnumType serializeObj)
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
        public void NumberStringEnumDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NumberStringEnumType(NumberStringEnum.BEnum);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NumberStringEnumType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NumberStringEnumType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NumberFlagsSerializeTest(Encoding encoding)
        {
            var obj = new NumberFlagsType(NumberFlags.BFlag | NumberFlags.CFlag);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NumberFlagsType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NumberFlagsDeserializeTest(Encoding encoding)
        {
            var obj = new NumberFlagsType(NumberFlags.BFlag | NumberFlags.CFlag);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NumberFlagsType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NumberFlagsType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NumberFlagsSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NumberFlagsType(NumberFlags.BFlag | NumberFlags.CFlag);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NumberFlagsType serializeObj)
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
        public void NumberFlagsDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NumberFlagsType(NumberFlags.BFlag | NumberFlags.CFlag);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NumberFlagsType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NumberFlagsType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringFlagsSerializeTest(Encoding encoding)
        {
            var obj = new StringFlagsType(StringFlags.BFlag | StringFlags.CFlag);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringFlagsType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringFlagsDeserializeTest(Encoding encoding)
        {
            var obj = new StringFlagsType(StringFlags.BFlag | StringFlags.CFlag);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<StringFlagsType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<StringFlagsType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringFlagsSerializeNoGCTest(Encoding encoding)
        {
            var obj = new StringFlagsType(StringFlags.BFlag | StringFlags.CFlag);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, StringFlagsType serializeObj)
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
        public void StringFlagsDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new StringFlagsType(StringFlags.BFlag | StringFlags.CFlag);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<StringFlagsType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<StringFlagsType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void LazySerializeTest(Encoding encoding)
        {
            var obj = new LazyType(new Lazy<string>("LazyString"));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<LazyType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void LazyDeserializeTest(Encoding encoding)
        {
            var obj = new LazyType(new Lazy<string>("LazyString"));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<LazyType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<LazyType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void LazySerializeNoGCTest(Encoding encoding)
        {
            var obj = new LazyType(new Lazy<string>("LazyString"));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, LazyType serializeObj)
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
        public void LazyDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new LazyType(new Lazy<string>("LazyString"));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(2));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<LazyType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<LazyType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableSerializeTest(Encoding encoding)
        {
            var obj = new NullableType(new BoolType(true));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableDeserializeTest(Encoding encoding)
        {
            var obj = new NullableType(new BoolType(true));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NullableType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NullableType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableType(new BoolType(true));
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NullableType serializeObj)
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
        public void NullableDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NullableType(new BoolType(true));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(0));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<NullableType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NullableType>(jsonText);
                }
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void GenericCollectionSerializeTest(Encoding encoding)
        {
            var obj = new GenericTypeCollection(new List<GenericType<int>> { new GenericType<int>(1), new GenericType<int>(2) });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<GenericTypeCollection>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void GenericCollectionDeserializeTest(Encoding encoding)
        {
            var obj = new GenericTypeCollection(new List<GenericType<int>> { new GenericType<int>(1), new GenericType<int>(2) });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<GenericTypeCollection>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<GenericTypeCollection>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void GenericCollectionSerializeNoGCTest(Encoding encoding)
        {
            var obj = new GenericTypeCollection(new List<GenericType<int>> { new GenericType<int>(1), new GenericType<int>(2) });
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, GenericTypeCollection serializeObj)
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
        public void GenericCollectionDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new GenericTypeCollection(new List<GenericType<int>> { new GenericType<int>(1), new GenericType<int>(2) });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var utf8Bytes = encoding == Encoding.UTF8 ? System.Text.Encoding.UTF8.GetBytes(json) : null;
            // 先调用一次，解决对象池的分配
            Deserialize(encoding, json, utf8Bytes);

            var action = new TestDelegate(() => { Deserialize(encoding, json, utf8Bytes); });
            Assert.That(action, Is.Not.AllocatingGCMemory(2));

            static void Deserialize(Encoding encodingType, string jsonText, byte[] utf8)
            {
                if (encodingType == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<GenericTypeCollection>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<GenericTypeCollection>(jsonText);
                }
            }
        }
    }
}