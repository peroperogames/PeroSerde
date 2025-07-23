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
            var obj = new DecimalType(13/2);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DecimalType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DecimalDeserializeTest(Encoding encoding)
        {
            var obj = new DecimalType(13/2);
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
            var obj = new DecimalType(13/2);
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
            var obj = new DecimalType(13/2);
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
        public void NuallableCharSerializeTest(Encoding encoding)
        {
            var obj = new NuallableCharType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableCharType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableCharDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableCharType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableCharType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableCharType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableCharSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableCharType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableCharType serializeObj)
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
        public void NuallableCharDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableCharType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableCharType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableCharType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableByteSerializeTest(Encoding encoding)
        {
            var obj = new NuallableByteType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableByteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableByteDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableByteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableByteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableByteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableByteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableByteType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableByteType serializeObj)
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
        public void NuallableByteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableByteType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableByteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableByteType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableSbyteSerializeTest(Encoding encoding)
        {
            var obj = new NuallableSbyteType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableSbyteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableSbyteDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableSbyteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableSbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableSbyteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableSbyteSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableSbyteType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableSbyteType serializeObj)
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
        public void NuallableSbyteDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableSbyteType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableSbyteType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableSbyteType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableShortSerializeTest(Encoding encoding)
        {
            var obj = new NuallableShortType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableShortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableShortDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableShortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableShortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableShortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableShortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableShortType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableShortType serializeObj)
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
        public void NuallableShortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableShortType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableShortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableShortType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUshortSerializeTest(Encoding encoding)
        {
            var obj = new NuallableUshortType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableUshortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUshortDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableUshortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableUshortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableUshortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUshortSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableUshortType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableUshortType serializeObj)
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
        public void NuallableUshortDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableUshortType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableUshortType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableUshortType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableIntSerializeTest(Encoding encoding)
        {
            var obj = new NuallableIntType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableIntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableIntDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableIntType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableIntType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableIntType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableIntSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableIntType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableIntType serializeObj)
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
        public void NuallableIntDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableIntType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableIntType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableIntType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUintSerializeTest(Encoding encoding)
        {
            var obj = new NuallableUintType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableUintType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUintDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableUintType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableUintType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableUintType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUintSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableUintType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableUintType serializeObj)
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
        public void NuallableUintDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableUintType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableUintType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableUintType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableLongSerializeTest(Encoding encoding)
        {
            var obj = new NuallableLongType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableLongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableLongDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableLongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableLongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableLongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableLongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableLongType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableLongType serializeObj)
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
        public void NuallableLongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableLongType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableLongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableLongType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUlongSerializeTest(Encoding encoding)
        {
            var obj = new NuallableUlongType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableUlongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUlongDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableUlongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableUlongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableUlongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUlongSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableUlongType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableUlongType serializeObj)
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
        public void NuallableUlongDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableUlongType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableUlongType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableUlongType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableFloatSerializeTest(Encoding encoding)
        {
            var obj = new NuallableFloatType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableFloatType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableFloatDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableFloatType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableFloatType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableFloatType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableFloatSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableFloatType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableFloatType serializeObj)
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
        public void NuallableFloatDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableFloatType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableFloatType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableFloatType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDoubleSerializeTest(Encoding encoding)
        {
            var obj = new NuallableDoubleType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableDoubleType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDoubleDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableDoubleType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableDoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableDoubleType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDoubleSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableDoubleType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableDoubleType serializeObj)
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
        public void NuallableDoubleDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableDoubleType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableDoubleType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableDoubleType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDecimalSerializeTest(Encoding encoding)
        {
            var obj = new NuallableDecimalType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableDecimalType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDecimalDeserializeTest(Encoding encoding)
        {
            var obj = new NuallableDecimalType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<NuallableDecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<NuallableDecimalType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDecimalSerializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableDecimalType(null);
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(1));

            static void Serialize(Encoding encodingType, NuallableDecimalType serializeObj)
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
        public void NuallableDecimalDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new NuallableDecimalType(null);
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
                    JsonConverter.UTF8.Deserialize<NuallableDecimalType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<NuallableDecimalType>(jsonText);
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
            var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DictionaryNumberType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DictionaryNumberDeserializeTest(Encoding encoding)
        {
            var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
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
            var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
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
            var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
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
            var obj = new DictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DictionaryStringType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DictionaryStringDeserializeTest(Encoding encoding)
        {
            var obj = new DictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
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
            var obj = new DictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
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
            var obj = new DictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
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
        public void CollectionDictionaryNumberSerializeTest(Encoding encoding)
        {
            var obj = new CollectionDictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<CollectionDictionaryNumberType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CollectionDictionaryNumberDeserializeTest(Encoding encoding)
        {
            var obj = new CollectionDictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<CollectionDictionaryNumberType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<CollectionDictionaryNumberType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CollectionDictionaryNumberSerializeNoGCTest(Encoding encoding)
        {
            var obj = new CollectionDictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(2));

            static void Serialize(Encoding encodingType, CollectionDictionaryNumberType serializeObj)
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
        public void CollectionDictionaryNumberDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new CollectionDictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
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
                    JsonConverter.UTF8.Deserialize<CollectionDictionaryNumberType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<CollectionDictionaryNumberType>(jsonText);
                }
            }
        }
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CollectionDictionaryStringSerializeTest(Encoding encoding)
        {
            var obj = new CollectionDictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<CollectionDictionaryStringType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CollectionDictionaryStringDeserializeTest(Encoding encoding)
        {
            var obj = new CollectionDictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<CollectionDictionaryStringType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<CollectionDictionaryStringType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CollectionDictionaryStringSerializeNoGCTest(Encoding encoding)
        {
            var obj = new CollectionDictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
            // 先进行一次调用，解决对象池的问题
            Serialize(encoding, obj);

            var action = new TestDelegate(() => { Serialize(encoding, obj); });
            // 因为会序列化出来字符串，所以必然会有分配
            Assert.That(action, Is.Not.AllocatingGCMemory(2));

            static void Serialize(Encoding encodingType, CollectionDictionaryStringType serializeObj)
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
        public void CollectionDictionaryStringDeserializeNoGCTest(Encoding encoding)
        {
            var obj = new CollectionDictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
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
                    JsonConverter.UTF8.Deserialize<CollectionDictionaryStringType>(utf8);
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<CollectionDictionaryStringType>(jsonText);
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