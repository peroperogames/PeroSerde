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
    public sealed class BuiltinFormattersDeserializeTests
    {
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void BoolTest(Encoding encoding)
        {
            var obj = new BoolType(true);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<BoolType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<BoolType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void CharTest(Encoding encoding)
        {
            var obj = new CharType('\ufff0');
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<CharType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<CharType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ByteTest(Encoding encoding)
        {
            var obj = new ByteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<ByteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<ByteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void SbyteTest(Encoding encoding)
        {
            var obj = new SbyteType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<SbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<SbyteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ShortTest(Encoding encoding)
        {
            var obj = new ShortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<ShortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<ShortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UshortTest(Encoding encoding)
        {
            var obj = new UshortType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<UshortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<UshortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void IntTest(Encoding encoding)
        {
            var obj = new IntType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<IntType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<IntType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UintTest(Encoding encoding)
        {
            var obj = new UintType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<UintType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<UintType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void LongTest(Encoding encoding)
        {
            var obj = new LongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<LongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<LongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UlongTest(Encoding encoding)
        {
            var obj = new UlongType(10);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<UlongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<UlongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void FloatTest(Encoding encoding)
        {
            var obj = new FloatType(10.4f);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<FloatType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<FloatType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DoubleTest(Encoding encoding)
        {
            var obj = new DoubleType(10.4d);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<DoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<DoubleType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DecimalTest(Encoding encoding)
        {
            var obj = new DecimalType(13 / 2);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<DecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<DecimalType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullableBoolTest(Encoding encoding)
        {
            var obj = new NullableBoolType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NullableBoolType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NullableBoolType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableCharTest(Encoding encoding)
        {
            var obj = new NuallableCharType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableCharType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableCharType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableByteTest(Encoding encoding)
        {
            var obj = new NuallableByteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableByteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableByteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableSbyteTest(Encoding encoding)
        {
            var obj = new NuallableSbyteType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableSbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableSbyteType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableShortTest(Encoding encoding)
        {
            var obj = new NuallableShortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableShortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableShortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUshortTest(Encoding encoding)
        {
            var obj = new NuallableUshortType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableUshortType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableUshortType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableIntTest(Encoding encoding)
        {
            var obj = new NuallableIntType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableIntType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableIntType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUintTest(Encoding encoding)
        {
            var obj = new NuallableUintType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableUintType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableUintType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableLongTest(Encoding encoding)
        {
            var obj = new NuallableLongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableLongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableLongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableUlongTest(Encoding encoding)
        {
            var obj = new NuallableUlongType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableUlongType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableUlongType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableFloatTest(Encoding encoding)
        {
            var obj = new NuallableFloatType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableFloatType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableFloatType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDoubleTest(Encoding encoding)
        {
            var obj = new NuallableDoubleType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableDoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableDoubleType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NuallableDecimalTest(Encoding encoding)
        {
            var obj = new NuallableDecimalType(null);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<NuallableDecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<NuallableDecimalType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void StringTest(Encoding encoding)
        {
            var obj = new StringType("String");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<StringType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<StringType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void DateTimeTest(Encoding encoding)
        {
            var obj = new DateTimeType(DateTime.Now);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<DateTimeType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<DateTimeType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixMillisecondDateTimeTest(Encoding encoding)
        {
            var obj = new UnixMillisecondDateTimeType(DateTime.Now);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<UnixMillisecondDateTimeType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<UnixMillisecondDateTimeType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void UnixSecondDateTimeTest(Encoding encoding)
        {
            var obj = new UnixSecondDateTimeType(DateTime.Now);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<UnixSecondDateTimeType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<UnixSecondDateTimeType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector2Test(Encoding encoding)
        {
            var obj = new Vector2Type(new Vector2(0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<Vector2Type>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<Vector2Type>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector3Test(Encoding encoding)
        {
            var obj = new Vector3Type(new Vector3(0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<Vector3Type>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<Vector3Type>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void Vector4Test(Encoding encoding)
        {
            var obj = new Vector4Type(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<Vector4Type>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<Vector4Type>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ColorTest(Encoding encoding)
        {
            var obj = new ColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<ColorType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<ColorType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void HexColorTest(Encoding encoding)
        {
            var obj = new HexColorType(new Color(0.5f, 0.5f, 0.5f, 0.5f));
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<HexColorType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<HexColorType>(json);
            Assert.AreEqual(obj, newObj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void GuidTest(Encoding encoding)
        {
            var obj = new GuidType(Guid.NewGuid());
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var newObj = encoding == Encoding.UTF8
                ? JsonConverter.DeserializeUtf8<GuidType>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.DeserializeUtf16<GuidType>(json);
            Assert.AreEqual(obj, newObj);
        }
    }
}