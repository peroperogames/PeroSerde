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
