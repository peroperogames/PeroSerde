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
    public sealed class CustomFormatterTests
    {
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void EncodingFormatterTests(Encoding encoding)
        {
            const string testValue = "Hello, World!";
            var json = $"{{\"value\":\"{testValue}\"}}";
            var obj = new EncodinFormatterTestClass { value = testValue };
            if (encoding == Encoding.UTF8)
            {
                var utf8 = JsonConverter.UTF8.Serialize(obj);
                Assert.IsTrue(utf8.SequenceEqual(System.Text.Encoding.UTF8.GetBytes(json)));
                var utf8Value = JsonConverter.UTF8.Deserialize<EncodinFormatterTestClass>(utf8);
                Assert.AreEqual(testValue, utf8Value.value);
            }
            else
            {
                var utf16 = JsonConverter.UTF16.Serialize(obj);
                Assert.AreEqual(utf16, json);
                var utf16Value = JsonConverter.UTF16.Deserialize<EncodinFormatterTestClass>(utf16);
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
    }

    [SerdeJsonSerializable]
    internal sealed partial class PrimitiveFormatterTestClass
    {
        [SerdeJsonFormatter(typeof(TestPrimitiveFormatter))]
        public string value;
    }

    [SerdeJsonSerializable]
    internal sealed partial class EncodinFormatterTestClass
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
}