/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/14-16:49:53
 */

using System;
using System.Linq;
using NUnit.Framework;
using PeroTools2.Serde.Editor.Tests;

namespace PeroTools2.Serde.Json.Editor.Tests
{
    public sealed class FormatterTests
    {
        [Test]
        public void GenericRegisterTests()
        {
            Assert.IsTrue(SerdeJsonFormatterProvider.IsRegistered<TestGenericType<GenericType<int>>>());
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void EncodingFormatterTests(Encoding encoding)
        {
            const string testValue = "Hello, World!";
            var json = $"{{\"value\":\"{testValue}\"}}";
            var obj = new EncodingFormatterTestClass { value = testValue };
            if (encoding == Encoding.UTF8)
            {
                var utf8 = JsonConverter.UTF8.Serialize(obj);
                Assert.IsTrue(utf8.SequenceEqual(System.Text.Encoding.UTF8.GetBytes(json)));
                var utf8Value = JsonConverter.UTF8.Deserialize<EncodingFormatterTestClass>(utf8);
                Assert.AreEqual(testValue, utf8Value.value);
            }
            else
            {
                var utf16 = JsonConverter.UTF16.Serialize(obj);
                Assert.AreEqual(utf16, json);
                var utf16Value = JsonConverter.UTF16.Deserialize<EncodingFormatterTestClass>(utf16);
                Assert.AreEqual(testValue, utf16Value.value);
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void PrimitiveFormatterTests(Encoding encoding)
        {
            const string testValue = "Hello, World!";
            var json = $"{{\"value\":\"{testValue}\"}}";
            var obj = new PrimitiveFormatterTestClass { value = testValue };
            if (encoding == Encoding.UTF8)
            {
                var utf8 = JsonConverter.UTF8.Serialize(obj);
                Assert.IsTrue(utf8.SequenceEqual(System.Text.Encoding.UTF8.GetBytes(json)));
                var utf8Value = JsonConverter.UTF8.Deserialize<PrimitiveFormatterTestClass>(utf8);
                Assert.AreEqual(testValue, utf8Value.value);
            }
            else
            {
                var utf16 = JsonConverter.UTF16.Serialize(obj);
                Assert.AreEqual(utf16, json);
                var utf16Value = JsonConverter.UTF16.Deserialize<PrimitiveFormatterTestClass>(utf16);
                Assert.AreEqual(testValue, utf16Value.value);
            }
        }

        [TestCase(8 * 1024)]
        [TestCase(1024 * 1024)]
        public void PrimitiveFormatterLargeValueTests(int length)
        {
            // 超过栈分配阈值时改为从缓冲池借出，需保证往返正确且不会栈溢出
            var testValue = new string('x', length);
            var obj = new PrimitiveFormatterTestClass { value = testValue };

            var utf8 = JsonConverter.UTF8.Serialize(obj);
            Assert.AreEqual(testValue, JsonConverter.UTF8.Deserialize<PrimitiveFormatterTestClass>(utf8).value);

            var utf16 = JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(testValue, JsonConverter.UTF16.Deserialize<PrimitiveFormatterTestClass>(utf16).value);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void RoundTripNumberFormatterTests(Encoding encoding)
        {
            var obj = new RoundTripFormatterTestClass
            {
                doubleValue = 1.0000000000000002,
                floatValue = 1f / 3f,
                nullableDoubleValue = 123456789.123456789,
                nullableFloatValue = 1.2345678f,
            };

            var result = RoundTrip(encoding, obj);

            Assert.AreEqual(BitConverter.DoubleToInt64Bits(obj.doubleValue), BitConverter.DoubleToInt64Bits(result.doubleValue));
            Assert.AreEqual(BitConverter.DoubleToInt64Bits(obj.floatValue), BitConverter.DoubleToInt64Bits(result.floatValue));
            Assert.AreEqual(BitConverter.DoubleToInt64Bits(obj.nullableDoubleValue.Value), BitConverter.DoubleToInt64Bits(result.nullableDoubleValue.Value));
            Assert.AreEqual(BitConverter.DoubleToInt64Bits(obj.nullableFloatValue.Value), BitConverter.DoubleToInt64Bits(result.nullableFloatValue.Value));
        }

        [TestCase(Encoding.UTF8, 0.0)]
        [TestCase(Encoding.UTF16, 0.0)]
        [TestCase(Encoding.UTF8, double.Epsilon)]
        [TestCase(Encoding.UTF16, double.Epsilon)]
        [TestCase(Encoding.UTF8, double.MaxValue)]
        [TestCase(Encoding.UTF16, double.MaxValue)]
        [TestCase(Encoding.UTF8, double.MinValue)]
        [TestCase(Encoding.UTF16, double.MinValue)]
        [TestCase(Encoding.UTF8, -1.0 / 3.0)]
        [TestCase(Encoding.UTF16, -1.0 / 3.0)]
        public void RoundTripDoubleValueTests(Encoding encoding, double value)
        {
            var result = RoundTrip(encoding, new RoundTripFormatterTestClass { doubleValue = value });
            Assert.AreEqual(BitConverter.DoubleToInt64Bits(value), BitConverter.DoubleToInt64Bits(result.doubleValue));
        }

        [TestCase(Encoding.UTF8, 0f)]
        [TestCase(Encoding.UTF16, 0f)]
        [TestCase(Encoding.UTF8, float.Epsilon)]
        [TestCase(Encoding.UTF16, float.Epsilon)]
        [TestCase(Encoding.UTF8, float.MaxValue)]
        [TestCase(Encoding.UTF16, float.MaxValue)]
        [TestCase(Encoding.UTF8, float.MinValue)]
        [TestCase(Encoding.UTF16, float.MinValue)]
        public void RoundTripFloatValueTests(Encoding encoding, float value)
        {
            var result = RoundTrip(encoding, new RoundTripFormatterTestClass { floatValue = value });
            Assert.AreEqual(BitConverter.DoubleToInt64Bits(value), BitConverter.DoubleToInt64Bits(result.floatValue));
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void RoundTripNullableNullTests(Encoding encoding)
        {
            var obj = new RoundTripFormatterTestClass { nullableDoubleValue = null, nullableFloatValue = null };

            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);

            StringAssert.Contains("\"nullableDoubleValue\":null", json);
            StringAssert.Contains("\"nullableFloatValue\":null", json);

            var result = RoundTrip(encoding, obj);

            Assert.IsNull(result.nullableDoubleValue);
            Assert.IsNull(result.nullableFloatValue);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void RoundTripNegativeZeroTests(Encoding encoding)
        {
            var obj = new RoundTripFormatterTestClass { doubleValue = -0.0, floatValue = -0.0f };

            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);

            StringAssert.Contains("\"doubleValue\":-0", json);
            StringAssert.Contains("\"floatValue\":-0", json);

            var result = RoundTrip(encoding, obj);

            Assert.AreEqual(BitConverter.DoubleToInt64Bits(-0.0), BitConverter.DoubleToInt64Bits(result.doubleValue));
            Assert.AreEqual(BitConverter.DoubleToInt64Bits(-0.0f), BitConverter.DoubleToInt64Bits(result.floatValue));
        }

        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void SerializeNonFiniteDoubleThrowsTests(double value)
        {
            Assert.Throws<SerdeException>(() => JsonConverter.UTF16.Serialize(new RoundTripFormatterTestClass { doubleValue = value }));
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void SerializeNonFiniteFloatThrowsTests(float value)
        {
            Assert.Throws<SerdeException>(() => JsonConverter.UTF16.Serialize(new RoundTripFormatterTestClass { floatValue = value }));
        }

        private static RoundTripFormatterTestClass RoundTrip(Encoding encoding, RoundTripFormatterTestClass obj) =>
            encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<RoundTripFormatterTestClass>(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Deserialize<RoundTripFormatterTestClass>(JsonConverter.UTF16.Serialize(obj));
    }

    [SerdeJsonSerializable]
    internal sealed partial class PrimitiveFormatterTestClass
    {
        [SerdeJsonFormatter(typeof(TestPrimitiveFormatter))]
        public string value;
    }

    [SerdeJsonSerializable]
    internal sealed partial class EncodingFormatterTestClass
    {
        [SerdeJsonFormatter(typeof(TestEncodingFormatter))]
        public string value;
    }

    internal sealed class TestPrimitiveFormatter : PrimitiveJsonFormatter<string>
    {
        protected override string Deserialize(ReadOnlySpan<char> value) => value.ToString();

        protected override ReadOnlySpan<char> Serialize(string value) => $"\"{value}\"".AsSpan();
    }

    internal sealed class TestEncodingFormatter : EncodingJsonFormatter<string>
    {
        public override string DeserializeUtf16(ReadOnlySpan<char> value) => value.ToString();

        public override string DeserializeUtf8(ReadOnlySpan<byte> value) => System.Text.Encoding.UTF8.GetString(value);

        public override ReadOnlySpan<char> SerializeUtf16(string value) => $"\"{value}\"".AsSpan();

        public override ReadOnlySpan<byte> SerializeUtf8(string value) => System.Text.Encoding.UTF8.GetBytes($"\"{value}\"");
    }

    [SerdeJsonSerializable]
    internal sealed partial class RoundTripFormatterTestClass
    {
        [SerdeJsonFormatter(typeof(RoundTripDoubleFormatter))]
        public double doubleValue;

        [SerdeJsonFormatter(typeof(RoundTripFloatFormatter))]
        public float floatValue;

        [SerdeJsonFormatter(typeof(NullableRoundTripDoubleFormatter))]
        public double? nullableDoubleValue;

        [SerdeJsonFormatter(typeof(NullableRoundTripFloatFormatter))]
        public float? nullableFloatValue;
    }

    [SerdeJsonSerializable]
    internal partial class TestGenericType<T>
    {
        public T data;
    }
}