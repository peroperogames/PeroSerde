/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Dundun
 * Created On: 2025/07/29
 */

namespace PeroTools2.Serde.Json.Tests;

public sealed class IndentFormatHelperTests
{
    [Test]
    public void TestFormatBasicStructure()
    {
        const string simpleKeyValue = "{\"name\":\"John\",\"age\":30,\"active\":true}";
        const string expectedSimpleKeyValue = "{\n    \"name\": \"John\",\n    \"age\": 30,\n    \"active\": true\n}";

        var result = IndentFormatHelper.FormatJson(simpleKeyValue);
        Assert.AreEqual(expectedSimpleKeyValue, result, "Result should be expectedSimpleKeyValue");
    }

    [Test]
    public void TestFormatSpecialKeyName()
    {
        const string specialKeys = "{\"\":\"empty key\",\"   \":\"three spaces\",\"key.with.dot\":\"dot in key\",\"key:colon\":\"colon in key\"}";
        const string expectedSpecialKeys = "{\n    \"\": \"empty key\",\n    \"   \": \"three spaces\",\n    \"key.with.dot\": \"dot in key\",\n    \"key:colon\": \"colon in key\"\n}";
        var result = IndentFormatHelper.FormatJson(specialKeys);
        Assert.AreEqual(expectedSpecialKeys, result, "Result should be AfterspecialKeys");
    }

    [Test]
    public void TestFormatNestedStructure()
    {
        const string deepNestedStructure = "{\"person\":{\"name\":\"Alice\",\"address\":{\"city\":\"Paris\"}},\"hobbies\":[\"reading\",\"swimming\",{\"sports\":[\"tennis\",\"soccer\"]}]}";
        const string expectedDeepNestedStructure = "{\n    \"person\": {\n        \"name\": \"Alice\",\n        \"address\": {\n            \"city\": \"Paris\"\n        }\n    },\n    \"hobbies\": [\n        \"reading\",\n        \"swimming\",\n        {\n            \"sports\": [\n                \"tennis\",\n                \"soccer\"\n            ]\n        }\n    ]\n}";
        var result = IndentFormatHelper.FormatJson(deepNestedStructure);
        Assert.AreEqual(expectedDeepNestedStructure, result, "Result should be AfterdeepNestedStructure");
    }

    [Test]
    public void TestFormatEscapeCharacter()
    {
        const string escapeCharacters = "{\"message\":\"Hello\\\\nWorld\",\"path\":\"C:\\\\\\\\Users\\\\\\\\\",\"quotes\":\"Single ' and double \\\" quotes\",\"unicode\":\"\\u03B1\\u03B2\\u03B3\"}";
        const string expectedEscapeCharacters = "{\n    \"message\": \"Hello\\\\nWorld\",\n    \"path\": \"C:\\\\\\\\Users\\\\\\\\\",\n    \"quotes\": \"Single ' and double \\\" quotes\",\n    \"unicode\": \"\\u03B1\\u03B2\\u03B3\"\n}";
        var result = IndentFormatHelper.FormatJson(escapeCharacters);
        Assert.AreEqual(expectedEscapeCharacters, result, "Result should be AfterescapeCharacters");
    }

    [Test]
    public void TestemptyStructures()
    {
        const string emptyStructures = "{\"emptyObject\":{},\"emptyArray\":[],\"data\":{\"nestedEmpty\":{}}}";
        const string expectedEmptyStructures = "{\n    \"emptyObject\": {},\n    \"emptyArray\": [],\n    \"data\": {\n        \"nestedEmpty\": {}\n    }\n}";
        var result = IndentFormatHelper.FormatJson(emptyStructures);
        Assert.AreEqual(expectedEmptyStructures, result, "Result should be expectedEmptyStructures");
    }

    [Test]
    public void TestMixedStructure()
    {
        const string mixedStructure = "{\"name\":\"John\",\"details\":{\"age\":30,\"address\":{\"city\":\"New York\",\"zip\":10001}},\"tags\":[\"tag1\",\"tag2\",{\"special\":\"value\"}]}";
        const string expectedMixedStructure = "{\n    \"name\": \"John\",\n    \"details\": {\n        \"age\": 30,\n        \"address\": {\n            \"city\": \"New York\",\n            \"zip\": 10001\n        }\n    },\n    \"tags\": [\n        \"tag1\",\n        \"tag2\",\n        {\n            \"special\": \"value\"\n        }\n    ]\n}";
        var result = IndentFormatHelper.FormatJson(mixedStructure);
        Assert.AreEqual(expectedMixedStructure, result, "Result should be expectedMixedStructure");
    }

    [Test]
    public void TestArrayStructure()
    {
        const string arrayStructure = "[\"apple\",\"banana\",\"orange\",{\"fruit\":\"mango\"},[\"nested\",\"array\"]]";
        const string expectedArrayStructure = "[\n    \"apple\",\n    \"banana\",\n    \"orange\",\n    {\n        \"fruit\": \"mango\"\n    },\n    [\n        \"nested\",\n        \"array\"\n    ]\n]";
        var result = IndentFormatHelper.FormatJson(arrayStructure);
        Assert.AreEqual(expectedArrayStructure, result, "Result should be expectedArrayStructure");
    }
}