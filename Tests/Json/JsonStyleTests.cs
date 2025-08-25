/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/08-19:46:33
 */

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PeroTools2.Serde.Editor.Tests;

namespace PeroTools2.Serde.Json.Editor.Tests
{
    public sealed class JsonStyleTests
    {
        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void MissingMemberDeserializeTest(Encoding encoding)
        {
            const string json = "{ \"a\":\"A1\"} ";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<JsonObject>(json);
            Assert.IsNull(obj.b);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void WriteSpaceStyleDeserializeArrayTest(Encoding encoding)
        {
            const string json = " [ { \"a\" : \"A1 \" , \"b\" : \" B1\" , \"c\" : 1.1 } , { \"a\" : \"A2 \" , \"b\" : \" B2\" , \"c\" : 1.2 } ] ";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<JsonObject[]>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<JsonObject[]>(json);
            Assert.IsNotNull(obj);
            Assert.AreEqual(obj.Length, 2);
            Assert.AreEqual(obj[0].a, "A1 ");
            Assert.AreEqual(obj[0].b, " B1");
            Assert.AreEqual(obj[0].c, 1.1f);
            Assert.AreEqual(obj[1].a, "A2 ");
            Assert.AreEqual(obj[1].b, " B2");
            Assert.AreEqual(obj[1].c, 1.2f);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void WriteSpaceStyleDeserializeObjectTest(Encoding encoding)
        {
            const string json = " { \"a\" : \"A \" , \"b\" : \" B\" , \"c\" : 1.1 } ";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<JsonObject>(json);
            Assert.IsNotNull(obj);
            Assert.AreEqual(obj.a, "A ");
            Assert.AreEqual(obj.b, " B");
            Assert.AreEqual(obj.c, 1.1f);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullMemberSerializeTest(Encoding encoding)
        {
            var obj = new SampleTest { a = null, b = null, c = null, d = null, e = new List<int?> { null, null, null, null } };
            const string expected = "{\"a\":null,\"b\":null,\"c\":null,\"d\":null,\"e\":[null,null,null,null]}";
            if (encoding == Encoding.UTF8)
            {
                var json = JsonConverter.UTF8.Serialize(obj);
                Assert.AreEqual(json, expected);
            }
            else
            {
                var json = System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj));
                Assert.AreEqual(json, expected);
            }
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullMemberDeserializeTest(Encoding encoding)
        {
            const string json = "{\"a\":  null,\"b\": null ,\"d\":null,\"c\":\"abc\", \"e\" :  [ null,null , null, null] }";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<SampleTest>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<SampleTest>(json);
            Assert.IsNotNull(obj);
            Assert.AreEqual(obj.a, null);
            Assert.AreEqual(obj.b, null);
            Assert.AreEqual(obj.c, "abc");
            Assert.AreEqual(obj.d, null);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ComplexObjectDeserializeTest(Encoding encoding)
        {
            const string json =
                "{\"a\":  null,\"b\": [ null, null, null ] ,\"c\": {\"1\": {\"1\": \"a\",\"2\": null,\"3\": \"b\"},\"2\": null,\"3\": {\"1\": \"c\",\"2\": null,\"3\": \"d\"}}}";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<Complex>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<Complex>(json);
            Assert.IsNotNull(obj);
            Assert.AreEqual(obj.a, null);
            Assert.IsTrue(obj.b.All(static item => item == null));
            Assert.IsTrue(obj.c[1][1] == "a");
            Assert.IsTrue(obj.c[1][2] == null);
            Assert.IsTrue(obj.c[1][3] == "b");
            Assert.IsTrue(obj.c[2] == null);
            Assert.IsTrue(obj.c[3][1] == "c");
            Assert.IsTrue(obj.c[3][2] == null);
            Assert.IsTrue(obj.c[3][3] == "d");
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void EmptyCollectionSeriializeTest(Encoding encoding)
        {
            const string json =
                "{\"a\":  [], \"b\": [], \"c\": {}}";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<Collection>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<Collection>(json);
            Assert.IsNotNull(obj);
            Assert.AreEqual(0, obj.a.Length);
            Assert.AreEqual(0, obj.b.Count);
            Assert.AreEqual(0, obj.c.Count);
        }
    }

    [SerdeJsonSerializable]
    public partial class Collection
    {
        public string[] a;
        public List<string> b;
        public Dictionary<int, string> c;
    }

    [SerdeJsonSerializable]
    public partial class Complex
    {
        public string a;
        public List<string> b;
        public Dictionary<int, Dictionary<int, string>> c;
    }

    [SerdeJsonSerializable]
    public partial class SampleTest
    {
        public JsonObject a;
        public int? b;
        public string c;
        public List<int> d;
        public List<int?> e;
    }

    [SerdeJsonSerializable]
    public partial class JsonObject
    {
        public string a { get; private set; }
        public string b { get; private set; }
        public float c { get; private set; }
    }
}