/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/08-19:46:33
 */

using NUnit.Framework;
using PeroTools2.Serde.Editor.Tests;

namespace PeroTools2.Serde.Json.Editor.Tests
{
    public sealed class JsonStyleTests
    {
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void WriteSpaceStyleDeserializeArrayTest(Encoding encoding)
        {
            const string json = " [ { \"a\" : \"A1 \" , \"b\" : \" B1\" } , { \"a\" : \"A2 \" , \"b\" : \" B2\" } ] ";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<JsonObject[]>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<JsonObject[]>(json);
            Assert.IsNotNull(obj);
            Assert.AreEqual(obj.Length, 2);
            Assert.AreEqual(obj[0].a, "A1 ");
            Assert.AreEqual(obj[0].b, " B1");
            Assert.AreEqual(obj[1].a, "A2 ");
            Assert.AreEqual(obj[1].b, " B2");
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void WriteSpaceStyleDeserializeObjectTest(Encoding encoding)
        {
            const string json = " { \"a\" : \"A \" , \"b\" : \" B\" } ";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<JsonObject>(json);
            Assert.IsNotNull(obj);
            Assert.AreEqual(obj.a, "A ");
            Assert.AreEqual(obj.b, " B");
        }
    }

    [SerdeJsonSerializable]
    public partial class JsonObject
    {
        public string a { get; set; }
        public string b { get; set; }
    }
}