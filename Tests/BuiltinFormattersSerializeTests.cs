/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/25
 */

using System;
using UnityEngine;
using NUnit.Framework;

namespace PeroTools2.Serde.Editor.Tests
{
    public sealed class BuiltinFormattersSerializeTests
    {
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void BoolTest(Encoding encoding)
        {
            var obj = new BoolType(true);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<BoolType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CharTest(Encoding encoding)
        {
            var obj = new CharType('\ufff0');
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<CharType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ByteTest(Encoding encoding)
        {
            var obj = new ByteType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ByteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void SbyteTest(Encoding encoding)
        {
            var obj = new SbyteType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<SbyteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ShortTest(Encoding encoding)
        {
            var obj = new ShortType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ShortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UshortTest(Encoding encoding)
        {
            var obj = new UshortType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UshortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void IntTest(Encoding encoding)
        {
            var obj = new IntType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<IntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UintTest(Encoding encoding)
        {
            var obj = new UintType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UintType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void LongTest(Encoding encoding)
        {
            var obj = new LongType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<LongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UlongTest(Encoding encoding)
        {
            var obj = new UlongType(10);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UlongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void FloatTest(Encoding encoding)
        {
            var obj = new FloatType(10.4f);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<FloatType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DoubleTest(Encoding encoding)
        {
            var obj = new DoubleType(10.4d);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DoubleType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DecimalTest(Encoding encoding)
        {
            var obj = new DecimalType(13 / 2);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DecimalType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableBoolTest(Encoding encoding)
        {
            var obj = new NullableBoolType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableBoolType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableCharTest(Encoding encoding)
        {
            var obj = new NuallableCharType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableCharType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableByteTest(Encoding encoding)
        {
            var obj = new NuallableByteType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableByteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableSbyteTest(Encoding encoding)
        {
            var obj = new NuallableSbyteType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableSbyteType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableShortTest(Encoding encoding)
        {
            var obj = new NuallableShortType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableShortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUshortTest(Encoding encoding)
        {
            var obj = new NuallableUshortType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableUshortType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableIntTest(Encoding encoding)
        {
            var obj = new NuallableIntType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableIntType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUintTest(Encoding encoding)
        {
            var obj = new NuallableUintType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableUintType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableLongTest(Encoding encoding)
        {
            var obj = new NuallableLongType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableLongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUlongTest(Encoding encoding)
        {
            var obj = new NuallableUlongType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableUlongType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableFloatTest(Encoding encoding)
        {
            var obj = new NuallableFloatType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableFloatType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDoubleTest(Encoding encoding)
        {
            var obj = new NuallableDoubleType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableDoubleType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDecimalTest(Encoding encoding)
        {
            var obj = new NuallableDecimalType(null);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NuallableDecimalType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringTest(Encoding encoding)
        {
            var obj = new StringType("String");
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DateTimeTest(Encoding encoding)
        {
            var obj = new DateTimeType(DateTime.Now);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj.value.ToString(), Newtonsoft.Json.JsonConvert.DeserializeObject<DateTimeType>(json).value.ToString());
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixMillisecondDateTimeTest(Encoding encoding)
        {
            var obj = new UnixMillisecondDateTimeType(DateTime.Now);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Debug.Log($"{obj.value.ToString()}, {Newtonsoft.Json.JsonConvert.DeserializeObject<UnixMillisecondDateTimeType>(json).value.ToString()}");
            Assert.AreEqual(obj.value.ToString(), Newtonsoft.Json.JsonConvert.DeserializeObject<UnixMillisecondDateTimeType>(json).value.ToString());
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixSecondDateTimeTest(Encoding encoding)
        {
            var obj = new UnixSecondDateTimeType(DateTime.Now);
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj.value.ToString(), Newtonsoft.Json.JsonConvert.DeserializeObject<UnixSecondDateTimeType>(json).value.ToString());
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2Test(Encoding encoding)
        {
            var obj = new Vector2Type(new Vector2(0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector2Type>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3Test(Encoding encoding)
        {
            var obj = new Vector3Type(new Vector3(0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector3Type>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector4Test(Encoding encoding)
        {
            var obj = new Vector4Type(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<Vector4Type>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ColorTest(Encoding encoding)
        {
            var obj = new ColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ColorType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void HexColorTest(Encoding encoding)
        {
            var obj = new HexColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<HexColorType>(json));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void GuidTest(Encoding encoding)
        {
            var obj = new GuidType(Guid.NewGuid());
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.SerializeUtf8(obj))
                : JsonConverter.SerializeUtf16(obj);
            Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<GuidType>(json));
        }
    }
}