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
        public void ListNumberSerializeTest(Encoding encoding)
        {
            var obj = new ListNumberType(new List<int> { 1, 2, 3, 4, 5 });
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ListNumberType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ListNumberDeserializeTest(Encoding encoding)
        {
            var obj = new ListNumberType(new List<int> { 1, 2, 3, 4, 5 });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<ListNumberType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<ListNumberType>(json);
            Assert.AreEqual(obj, newObj);
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
            try
            {
                var obj = new ListObjectType(new List<BoolType> { new BoolType(true), new BoolType(false) });
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

                var newObj = encoding == Encoding.UTF8
                    ? JsonConverter.UTF8.Deserialize<ListObjectType>(System.Text.Encoding.UTF8.GetBytes(json))
                    : JsonConverter.UTF16.Deserialize<ListObjectType>(json);
                Assert.AreEqual(obj, newObj);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(e.InnerException);
                throw;
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
    }
}