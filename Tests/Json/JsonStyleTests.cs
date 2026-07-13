/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/08-19:46:33
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using PeroTools2.Serde.Editor.Tests;
using UnityEngine;

namespace PeroTools2.Serde.Json.Editor.Tests
{
    public sealed class JsonStyleTests
    {
        public static readonly string s_LargeJsonTestFile = "Packages/com.prpr.serde/Tests/Json/very_large_test.json";

        public static IEnumerable<TestCaseData> HandlesVariousCharactersSerializeTestCases()
        {
            var testCases = new[]
            {
                ("Mayor", "ASCII字符"),
                ("市长", "CJK字符"),
                ("😀", "Emoji"),
                ("\uD842\uDFB7", "代理对"),
                ("e\u0301", "组合字符"),
                ("市长Mayor😀", "混合字符")
            };

            foreach (var (value, description) in testCases)
            {
                yield return new TestCaseData(Encoding.UTF8, value, description)
                    .SetName($"Serialize_UTF8_{description}");

                yield return new TestCaseData(Encoding.UTF16, value, description)
                    .SetName($"Serialize_UTF16_{description}");
            }
        }

        [TestCaseSource(nameof(HandlesVariousCharactersSerializeTestCases))]
        public void HandlesVariousCharactersSerialize(Encoding kind, string value, string _)
        {
            var str = kind switch
            {
                Encoding.UTF8 => System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(value)),
                Encoding.UTF16 => JsonConverter.UTF16.Serialize(value),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
            Assert.AreEqual($"\"{value}\"", str);
            Assert.IsFalse(str.Contains("\uFFFD"));
        }

        public static IEnumerable<TestCaseData> HandlesVariousCharactersDeserializeTestCases()
        {
            var testCases = new[]
            {
                ("{ \"b\": \"Mayor\" }", "Mayor", "ASCII字符"),
                ("{ \"b\": \"市长\" }", "市长", "CJK字符"),
                ("{ \"b\": \"😀\" }", "😀", "Emoji"),
                ("{ \"b\": \"\uD842\uDFB7\" }", "\uD842\uDFB7", "代理对"),
                ("{ \"b\": \"e\u0301\" }", "e\u0301", "组合字符"),
                ("{ \"b\": \"市长Mayor😀\" }", "市长Mayor😀", "混合字符"),
                ("{ \"b\": \"\" }", "", "空字符串"),
                ("{ \"b\": null }", null, "null值")
            };

            foreach (var (json, expectedValue, description) in testCases)
            {
                yield return new TestCaseData(Encoding.UTF8, json, expectedValue, description)
                    .SetName($"Deserialize_UTF8_{description}");

                yield return new TestCaseData(Encoding.UTF16, json, expectedValue, description)
                    .SetName($"Deserialize_UTF16_{description}");
            }
        }

        [TestCaseSource(nameof(HandlesVariousCharactersDeserializeTestCases))]
        public void HandlesVariousCharactersDeserialize(Encoding kind, string json, string expectedValue, string _)
        {
            var result = kind switch
            {
                Encoding.UTF8 => JsonConverter.UTF8.Deserialize<DeserializeJsonObject>(System.Text.Encoding.UTF8.GetBytes(json)),
                Encoding.UTF16 => JsonConverter.UTF16.Deserialize<DeserializeJsonObject>(json),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.b);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void EscapeCharacterSerializeTest(Encoding encoding)
        {
            var obj = new EscapeCharacterJsonObject { value = "\b\f\n\r\t\"\\" };
            var json = "{\"value\":\"\\b\\f\\n\\r\\t\\\"\\\\\"}";
            var result = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual(json, result);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void EscapeCharacterDeserializeTest(Encoding encoding)
        {
            var json = "{\"value\":\"\\b\\f\\n\\r\\t\\\"\\\\\"}";
            var value = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<EscapeCharacterJsonObject>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<EscapeCharacterJsonObject>(json);
            Assert.AreEqual("\b\f\n\r\t\"\\", value.value);
        }

        public static IEnumerable<TestCaseData> EmptyDeserializeTestCases()
        {
            var testCases = new[]
            {
                ("", "空"),
                (" ", "空格"),
            };

            var testTypeCases = new[]
            {
                typeof(List<int>),
                typeof(List<string>),
                typeof(string),
                typeof(EmptyJsonObject),
                typeof(Dictionary<int, string>)
            };

            foreach (var type in testTypeCases)
            {
                foreach (var (json, description) in testCases)
                {
                    yield return new TestCaseData(type, Encoding.UTF8, json)
                        .SetName($"Deserialize_UTF8_{type.Name}_{description}");
                    yield return new TestCaseData(type, Encoding.UTF16, json)
                        .SetName($"Deserialize_UTF16_{type.Name}_{description}");
                }
            }
        }

        [TestCaseSource(nameof(EmptyDeserializeTestCases))]
        public void EmptyDeserializeTest(Type type, Encoding encoding, string json)
        {
            var method = typeof(JsonStyleTests).GetMethod(nameof(GenericEmptyDeserializeTest), BindingFlags.NonPublic | BindingFlags.Instance);
            var genericMethod = method!.MakeGenericMethod(type);
            genericMethod.Invoke(this, new object[] { encoding, json });
        }

        private void GenericEmptyDeserializeTest<T>(Encoding encoding, string json)
        {
            var result = encoding switch
            {
                Encoding.UTF8 => JsonConverter.UTF8.Deserialize<T>(System.Text.Encoding.UTF8.GetBytes(json)),
                Encoding.UTF16 => JsonConverter.UTF16.Deserialize<T>(json),
                _ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null)
            };
            Assert.AreEqual(default(T), result);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void EmptyBodyDeserializeTest(Encoding encoding)
        {
            const string json = "{ \"a\":\"A1\", \"not_exists_array\":[], \"not_exists_object\":{}, \"not_exists_number\": 123, \"not_exists_string\": \"123\"} ";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<EmptyJsonObject>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<EmptyJsonObject>(json);
            Assert.IsNotNull(obj);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void MissingMemberDeserializeTest(Encoding encoding)
        {
            const string json = "{ \"a\":\"A1\", \"not_exists_array\":[], \"not_exists_object\":{}, \"not_exists_number\": 123, \"not_exists_string\": \"123\"} ";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<JsonObject>(json);
            Assert.IsNull(obj.b);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void IgnoreNullMemberSerializeTest(Encoding encoding)
        {
            var obj = new NullMemberJsonObject { x = null, y = null, z = 1, w = null };
            var json = encoding == Encoding.UTF8
                ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
                : JsonConverter.UTF16.Serialize(obj);
            Assert.AreEqual("{\"z\":1}", json);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void RedundantMemberDeserializeTest(Encoding encoding)
        {
            const string json = "{ \"z\":\"xxx\", \"a\":\"A1\", \"b\":\"B2\", \"c\":1.0}";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<JsonObject>(json);
            Assert.AreEqual("A1", obj.a);
            Assert.AreEqual("B2", obj.b);
            Assert.AreEqual(1f, obj.c);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void MissMatchTypeMemberDeserializeTest(Encoding encoding)
        {
            const string json1 = "{ \"a\" : {\"d\": \"{{{\" } , \"b\" : \" B1\" , \"c\" : 1.1 }";
            const string json2 = "{ \"a\" : \"Test\" , \"b\" : \" B1\" , \"c\" : \"Test\" }";
            const string json3 = "{ \"a\" : \"Test\" , \"b\" : \" B1\" , \"c\" : {} }";
            const string json4 = "{ \"a\" : \"Test\" , \"b\" : \" B1\" , \"c\" : [] }";
            const string json5 = "{ \"a\" : \"Test\" , \"b\" : \" B1\" , \"c\" : null }";
            Assert.Throws<SerdeException>(() =>
            {
                if (encoding == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json1));
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<JsonObject>(json1);
                }
            });
            Assert.Throws<SerdeException>(() =>
            {
                if (encoding == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json2));
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<JsonObject>(json2);
                }
            });
            Assert.Throws<SerdeException>(() =>
            {
                if (encoding == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json3));
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<JsonObject>(json3);
                }
            });
            Assert.Throws<SerdeException>(() =>
            {
                if (encoding == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json4));
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<JsonObject>(json4);
                }
            });
            Assert.Throws<SerdeException>(() =>
            {
                if (encoding == Encoding.UTF8)
                {
                    JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json5));
                }
                else
                {
                    JsonConverter.UTF16.Deserialize<JsonObject>(json5);
                }
            });
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
            const string json = " { \"a\" : \"A \" , \"b\" : \" B\" , \"c\" : 1.1, \"d\" : [  \" 1 \" , \"2\" ] , \"e\" : { \"a\" : 1 , \"b\" : 2 } } ";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<JsonObject>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<JsonObject>(json);
            Assert.IsNotNull(obj);
            Assert.AreEqual("A ", obj.a);
            Assert.AreEqual(" B", obj.b);
            Assert.AreEqual(1.1f, obj.c);
            Assert.AreEqual(2, obj.d.Length);
            Assert.AreEqual(" 1 ", obj.d[0]);
            Assert.AreEqual("2", obj.d[1]);
            Assert.AreEqual(2, obj.e.Count);
            Assert.AreEqual(1, obj.e["a"]);
            Assert.AreEqual(2, obj.e["b"]);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void NullMemberSerializeTest(Encoding encoding)
        {
            var obj = new SampleTest { a = null, b = null, c = null, d = null, e = new List<int?> { null, null, null, null } };
            const string expected = "{\"a\":null,\"b\":null,\"c\":null,\"d\":null,\"e\":[null,null,null,null],\"f\":{\"obj\":null}}";
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
            const string json = "{\"a\":  null,\"b\": null ,\"d\":null,\"c\":\"abc\", \"e\" :  [ null,null , null, null], \"f\" : null }";
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<SampleTest>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<SampleTest>(json);
            Assert.IsNotNull(obj);
            Assert.AreEqual(obj.a, null);
            Assert.AreEqual(obj.b, null);
            Assert.AreEqual(obj.c, "abc");
            Assert.AreEqual(obj.d, null);
            Assert.AreEqual(obj.f, default(StructObject));
            Assert.AreEqual(obj.f.obj, null);
        }

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void ComplexObjectDeserializeTest(Encoding encoding)
        {
            const string json = "{\"a\":  null,\"b\": [ null, null, null ] ,\"c\": {\"1\": {\"1\": \"a\",\"2\": null,\"3\": \"b\"},\"2\": null,\"3\": {\"1\": \"c\",\"2\": null,\"3\": \"d\"}}}";
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
        public void EmptyCollectionDeserializeTest(Encoding encoding)
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

        [TestCase(Encoding.UTF8)]
        [TestCase(Encoding.UTF16)]
        public void VeryLargeJsonDeserializeTest(Encoding encoding)
        {
            var json = File.ReadAllText(s_LargeJsonTestFile);
            var obj = encoding == Encoding.UTF8
                ? JsonConverter.UTF8.Deserialize<List<LargeJsonElement>>(System.Text.Encoding.UTF8.GetBytes(json))
                : JsonConverter.UTF16.Deserialize<List<LargeJsonElement>>(json);
            Assert.Greater(obj.Count, 0);
            Assert.AreEqual(1, obj[0].id);
            Assert.AreEqual("n1", obj[0].name);
            Assert.AreEqual(1.5f, obj[0].value);
            Assert.AreEqual(false, obj[0].active);
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
        public StructObject f;
    }

    [SerdeJsonSerializable]
    public partial class JsonObject
    {
        public string a { get; private set; }
        public string b { get; private set; }
        public float c { get; private set; }
        public string[] d;
        public Dictionary<string, int> e;
    }

    [SerdeJsonSerializable]
    public partial struct StructObject
    {
        public string obj;
    }

    [SerdeJsonSerializable]
    public partial class EmptyJsonObject
    {
    }

    [SerdeJsonSerializable]
    public partial class DeserializeJsonObject
    {
        public string a { get; private set; }
        public string b { get; private set; }
    }

    [SerdeJsonSerializable]
    public partial class EscapeCharacterJsonObject
    {
        public string value;
    }

    [SerdeJsonSerializable]
    public partial class LargeJsonElement
    {
        public int id;
        public string name;
        public float value;
        public bool active;
    }

    [SerdeJsonSerializable(ignoreNullMembers: true)]
    internal sealed partial class NullMemberJsonObject
    {
        public int? x;
        public string y;
        public int z;
        public Dictionary<string, int> w;
    }
}