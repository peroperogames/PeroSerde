/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/03
 */

namespace PeroTools2.Serde.Json.Tests;

[SerdeJsonSerializable]
public partial class KeyDispatchSimpleObject
{
    public long nameabcd;
}

[SerdeJsonSerializable]
public partial class KeyDispatchCollisionObject
{
    public long nameabcd1;
    public long nameabcd2;
}

[SerdeJsonSerializable]
public partial class ScalarHolder
{
    public int value;
}

[SerdeJsonSerializable]
public partial class FlagHolder
{
    public bool flag;
}

[SerdeJsonSerializable]
public partial class NonAsciiMemberHolder
{
    public int 值;
}

/// <summary>
/// Equivalence and fallback-path tests for the generated key dispatch.
/// If the generation-time prefix constant and the runtime packing differ in a single bit, members silently keep their
/// default value, so this behavior must be pinned down by tests.
/// </summary>
public sealed unsafe class KeyDispatchTests
{
    private static (ulong low, ulong high) NaivePrefix(string key)
    {
        ulong low = 0;
        ulong high = 0;
        for (var i = 0; i < key.Length; i++)
        {
            if (i < 4)
            {
                low |= (ulong) key[i] << (i * 16);
            }
            else if (i < 8)
            {
                high |= (ulong) key[i] << ((i - 4) * 16);
            }
        }

        return (low, high);
    }

    private static (ulong low, ulong high) RuntimePrefix(string key)
    {
        StringHelper.GetKeyPrefix(key.AsSpan(), out var low, out var high);
        return (low, high);
    }

    [Test]
    public void KeyPrefixMatchesNaiveTest()
    {
        var keys = new[]
        {
            string.Empty, "a", "ab", "abcd", "abcde", "abcdefgh", "abcdefghi", "abcdefghijklmnop",
            "stringValue", "intValue", "floatValue", "boolValue", "listValue", "objValue"
        };

        foreach (var key in keys)
        {
            Assert.AreEqual(NaivePrefix(key), RuntimePrefix(key), $"key={key}");
        }

        // Non-ASCII characters are packed the same way, one UTF-16 code unit per slot
        foreach (var key in new[] { "值", "值a", "对象值", "字符串值测试用例" })
        {
            Assert.AreEqual(NaivePrefix(key), RuntimePrefix(key), $"key={key}");
        }
    }

    [Test]
    public void EscapedKeyWithWhitespaceTest()
    {
        // An escaped key falls back to the general string reader; the whitespace in front of it must not break the read
        var utf16 = JsonConverter.UTF16.Deserialize<ScalarHolder>("{ \"a\\u0062\": 1, \"value\": 2 }");
        Assert.AreEqual(2, utf16.value);

        var utf8 = JsonConverter.UTF8.Deserialize<ScalarHolder>("{ \"a\\u0062\": 1, \"value\": 2 }"u8.ToArray());
        Assert.AreEqual(2, utf8.value);
    }

    [Test]
    public void EmptyAndTruncatedArrayTest()
    {
        Assert.AreEqual(0, JsonConverter.UTF8.Deserialize<int[]>("[]"u8.ToArray()).Length);
        Assert.AreEqual(0, JsonConverter.UTF8.Deserialize<int[]>("[  ]"u8.ToArray()).Length);
        Assert.AreEqual(0, JsonConverter.UTF16.Deserialize<int[]>("[  ]").Length);

        // A truncated array has to report a SerdeException, not an IndexOutOfRangeException
        Assert.Throws<SerdeException>(() => JsonConverter.UTF8.Deserialize<int[]>("["u8.ToArray()));
        Assert.Throws<SerdeException>(() => JsonConverter.UTF16.Deserialize<int[]>("["));
        Assert.Throws<SerdeException>(() => JsonConverter.UTF8.Deserialize<int[]>("[  "u8.ToArray()));
    }

    [Test]
    public void ArrayElementWhitespaceTest()
    {
        var utf8 = JsonConverter.UTF8.Deserialize<int[]>("[ 1 , 2 ]"u8.ToArray());
        CollectionAssert.AreEqual(new[] { 1, 2 }, utf8);

        var utf16 = JsonConverter.UTF16.Deserialize<int[]>("[ 1 , 2 ]");
        CollectionAssert.AreEqual(new[] { 1, 2 }, utf16);
    }

    [Test]
    public void TopLevelScalarAtBufferEndTest()
    {
        // The fastest paths read a delimiter after the token, so a top level value that ends the buffer is the case
        // where a missing bound shows up as an out of range read
        Assert.AreEqual(123, JsonConverter.UTF16.Deserialize<int>("123"));
        Assert.AreEqual(123, JsonConverter.UTF8.Deserialize<int>("123"u8.ToArray()));
        Assert.AreEqual(1234567890123L, JsonConverter.UTF16.Deserialize<long>("1234567890123"));
        Assert.AreEqual(1234567890123L, JsonConverter.UTF8.Deserialize<long>("1234567890123"u8.ToArray()));
        Assert.AreEqual(3.5f, JsonConverter.UTF16.Deserialize<float>("3.5"));
        Assert.AreEqual(3.5f, JsonConverter.UTF8.Deserialize<float>("3.5"u8.ToArray()));
        Assert.AreEqual("abc", JsonConverter.UTF16.Deserialize<string>("\"abc\""));
        Assert.AreEqual(true, JsonConverter.UTF16.Deserialize<bool>("true"));
    }

    [Test]
    public void BoolWithTrailingWhitespaceTest()
    {
        Assert.AreEqual(true, JsonConverter.UTF16.Deserialize<FlagHolder>("{\"flag\": true }").flag);
        Assert.AreEqual(true, JsonConverter.UTF8.Deserialize<FlagHolder>("{\"flag\":true  }"u8.ToArray()).flag);
        Assert.AreEqual(false, JsonConverter.UTF16.Deserialize<FlagHolder>("{\"flag\": false\t}").flag);
    }

    [Test]
    public void NonAsciiMemberNameTest()
    {
        // The UTF8 key length is a byte count, which is not the character count of the member name
        var utf8 = JsonConverter.UTF8.Deserialize<NonAsciiMemberHolder>("{\"值\":7}"u8.ToArray());
        Assert.AreEqual(7, utf8.值);

        var utf16 = JsonConverter.UTF16.Deserialize<NonAsciiMemberHolder>("{\"值\":7}");
        Assert.AreEqual(7, utf16.值);
    }

    [Test]
    public void FloatFastPathParityTest()
    {
        var values = new[]
        {
            "3.1415927", "0.1", "1.5", "-2.25", "16777217", "33554433", "0.30000001", "123.456789",
            "1e5", "-0e0", "1.4e-45", "3.4028235e38", "-0.0", "9999999", "16777216"
        };

        foreach (var text in values)
        {
            var expected = float.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, JsonConverter.UTF16.Deserialize<float>(text), $"utf16 {text}");
            Assert.AreEqual(expected, JsonConverter.UTF8.Deserialize<float>(System.Text.Encoding.UTF8.GetBytes(text)), $"utf8 {text}");

            var expectedDouble = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(expectedDouble, JsonConverter.UTF16.Deserialize<double>(text), $"double utf16 {text}");
        }

        Assert.AreEqual(
            BitConverter.SingleToInt32Bits(-0.0f),
            BitConverter.SingleToInt32Bits(JsonConverter.UTF16.Deserialize<float>("-0.0")));
    }

    [Test]
    public void NonAsciiUnicodeEscapeTest()
    {
        // \u00E9 must be encoded as 2 UTF-8 bytes and must not be truncated to a single byte
        var json = "\"\\u00E9\""u8.ToArray();
        Assert.AreEqual("é", JsonConverter.UTF8.Deserialize<string>(json));
    }

    [Test]
    public void SurrogatePairUnicodeEscapeTest()
    {
        // A surrogate pair must be merged into one 4-byte UTF-8 code unit
        var json = "\"\\uD83D\\uDE00\""u8.ToArray();
        Assert.AreEqual("😀", JsonConverter.UTF8.Deserialize<string>(json));
    }

    [Test]
    public void EscapedBackslashAtEndTest()
    {
        // The value ends with an escaped backslash
        var json = "{\"value\":\"a\\\\\"}"u8.ToArray();
        var value = JsonConverter.UTF8.Deserialize<EscapeCharacterJsonObject>(json);
        Assert.AreEqual("a\\", value.value);
    }

    [Test]
    public void EightByteMemberNameTest()
    {
        var json = "{\"nameabcd\":123456789}"u8.ToArray();
        var value = JsonConverter.UTF8.Deserialize<KeyDispatchSimpleObject>(json);
        Assert.AreEqual(123456789L, value.nameabcd);

        var utf16 = JsonConverter.UTF16.Deserialize<KeyDispatchSimpleObject>("{\"nameabcd\":123456789}");
        Assert.AreEqual(123456789L, utf16.nameabcd);
    }

    [Test]
    public void SharedPrefixMembersTest()
    {
        // The first 8 characters of two members are identical -> prefix collision -> both still have to be read
        var json = "{\"nameabcd1\":11,\"nameabcd2\":22}"u8.ToArray();
        var value = JsonConverter.UTF8.Deserialize<KeyDispatchCollisionObject>(json);
        Assert.AreEqual(11L, value.nameabcd1);
        Assert.AreEqual(22L, value.nameabcd2);

        var utf16 = JsonConverter.UTF16.Deserialize<KeyDispatchCollisionObject>("{\"nameabcd1\":11,\"nameabcd2\":22}");
        Assert.AreEqual(11L, utf16.nameabcd1);
        Assert.AreEqual(22L, utf16.nameabcd2);
    }

    [Test]
    public void UnknownMemberTest()
    {
        // An unknown key falls through the dispatch switch and must be skipped without disturbing the rest
        var json = "{\"unknownMember\":{\"nested\":[1,2,3]},\"nameabcd\":7}"u8.ToArray();
        var value = JsonConverter.UTF8.Deserialize<KeyDispatchSimpleObject>(json);
        Assert.AreEqual(7L, value.nameabcd);

        var utf16 = JsonConverter.UTF16.Deserialize<KeyDispatchSimpleObject>("{\"nameabcd\":7,\"unknown\":\"skipped\"}");
        Assert.AreEqual(7L, utf16.nameabcd);
    }
}
