/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Dundun
 * Created On: 2025/07/29
 * Description: JSON data formatting test
 */

using NUnit.Framework;

namespace PeroTools2.Serde.Json.Editor.Tests
{
    public class IndentFormatHelperTest
    {
        /// <summary>
        /// 基础键值对测试
        /// </summary>
        private readonly string m_SimpleKeyValue = "{\"name\":\"John\",\"age\":30,\"active\":true}";

        private readonly string m_ExpectedSimpleKeyValue = "{\n    \"name\": \"John\",\n    \"age\": 30,\n    \"active\": true\n}";

        /// <summary>
        /// 测试特殊键名
        /// </summary>
        private readonly string m_SpecialKeys = "{\"\":\"empty key\",\"   \":\"three spaces\",\"key.with.dot\":\"dot in key\",\"key:colon\":\"colon in key\"}";

        private readonly string m_ExpectedspecialKeys = "{\n    \"\": \"empty key\",\n    \"   \": \"three spaces\",\n    \"key.with.dot\": \"dot in key\",\n    \"key:colon\": \"colon in key\"\n}";

        /// <summary>
        /// 嵌套结构测试
        /// </summary>
        private readonly string m_DeepNestedStructure =
            "{\"person\":{\"name\":\"Alice\",\"address\":{\"city\":\"Paris\"}},\"hobbies\":[\"reading\",\"swimming\",{\"sports\":[\"tennis\",\"soccer\"]}]}";

        private readonly string m_ExpecteddeepNestedStructure =
            "{\n    \"person\": {\n        \"name\": \"Alice\",\n        \"address\": {\n            \"city\": \"Paris\"\n        }\n    },\n    \"hobbies\": [\n        \"reading\",\n        \"swimming\",\n        {\n            \"sports\": [\n                \"tennis\",\n                \"soccer\"\n            ]\n        }\n    ]\n}";

        /// <summary>
        /// 转义字符测试
        /// </summary>
        private readonly string m_EscapeCharacters =
            "{\"message\":\"Hello\\\\nWorld\",\"path\":\"C:\\\\\\\\Users\\\\\\\\\",\"quotes\":\"Single ' and double \\\" quotes\",\"unicode\":\"\\u03B1\\u03B2\\u03B3\"}";

        private readonly string m_ExpectedescapeCharacters =
            "{\n    \"message\": \"Hello\\\\nWorld\",\n    \"path\": \"C:\\\\\\\\Users\\\\\\\\\",\n    \"quotes\": \"Single ' and double \\\" quotes\",\n    \"unicode\": \"\\u03B1\\u03B2\\u03B3\"\n}";

        /// <summary>
        /// 空结构测试
        /// </summary>
        private readonly string m_EmptyStructures = "{\"emptyObject\":{},\"emptyArray\":[],\"data\":{\"nestedEmpty\":{}}}";

        private readonly string m_ExpectedEmptyStructures = "{\n    \"emptyObject\": {},\n    \"emptyArray\": [],\n    \"data\": {\n        \"nestedEmpty\": {}\n    }\n}";

        /// <summary>
        /// 混合结构测试
        /// </summary>
        private readonly string m_MixedStructure =
            "{\"name\":\"John\",\"details\":{\"age\":30,\"address\":{\"city\":\"New York\",\"zip\":10001}},\"tags\":[\"tag1\",\"tag2\",{\"special\":\"value\"}]}";

        private readonly string m_ExpectedMixedStructure =
            "{\n    \"name\": \"John\",\n    \"details\": {\n        \"age\": 30,\n        \"address\": {\n            \"city\": \"New York\",\n            \"zip\": 10001\n        }\n    },\n    \"tags\": [\n        \"tag1\",\n        \"tag2\",\n        {\n            \"special\": \"value\"\n        }\n    ]\n}";

        /// <summary>
        /// 数组结构测试
        /// </summary>
        private readonly string m_ArrayStructure = "[\"apple\",\"banana\",\"orange\",{\"fruit\":\"mango\"},[\"nested\",\"array\"]]";

        private readonly string m_ExpectedArrayStructure =
            "[\n    \"apple\",\n    \"banana\",\n    \"orange\",\n    {\n        \"fruit\": \"mango\"\n    },\n    [\n        \"nested\",\n        \"array\"\n    ]\n]";

        [Test]
        public void TestFormatBasicStructure()
        {
            var resultsimpleKeyValue = IndentFormatHelper.FormatJson(m_SimpleKeyValue);
            Assert.AreEqual(m_ExpectedSimpleKeyValue, resultsimpleKeyValue, "Result should be expectedSimpleKeyValue");
        }

        [Test]
        public void TestFormatSpecialKeyName()
        {
            var resultspecialKeys = IndentFormatHelper.FormatJson(m_SpecialKeys);
            Assert.AreEqual(m_ExpectedspecialKeys, resultspecialKeys, "Result should be AfterspecialKeys");
        }

        [Test]
        public void TestFormatNestedStructure()
        {
            var resultdeepNestedStructure = IndentFormatHelper.FormatJson(m_DeepNestedStructure);
            Assert.AreEqual(m_ExpecteddeepNestedStructure, resultdeepNestedStructure, "Result should be AfterdeepNestedStructure");
        }

        [Test]
        public void TestFormatEscapeCharacter()
        {
            var resultescapeCharacters = IndentFormatHelper.FormatJson(m_EscapeCharacters);
            Assert.AreEqual(m_ExpectedescapeCharacters, resultescapeCharacters, "Result should be AfterescapeCharacters");
        }

        [Test]
        public void TestemptyStructures()
        {
            var resultemptyStructures = IndentFormatHelper.FormatJson(m_EmptyStructures);
            Assert.AreEqual(m_ExpectedEmptyStructures, resultemptyStructures, "Result should be expectedEmptyStructures");
        }

        [Test]
        public void TestmixedStructure()
        {
            var resultmixedStructure = IndentFormatHelper.FormatJson(m_MixedStructure);
            Assert.AreEqual(m_ExpectedMixedStructure, resultmixedStructure, "Result should be expectedMixedStructure");
        }

        [Test]
        public void TestarrayStructure()
        {
            var resultarrayStructure = IndentFormatHelper.FormatJson(m_ArrayStructure);
            Assert.AreEqual(m_ExpectedArrayStructure, resultarrayStructure, "Result should be expectedArrayStructure");
        }
    }
}