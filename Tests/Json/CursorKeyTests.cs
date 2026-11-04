/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/03
 */

namespace PeroTools2.Serde.Json.Tests;

[SerdeJsonSerializable]
public partial class CursorKeySimpleObject
{
    public long nameabcd;
}

[SerdeJsonSerializable]
public partial class CursorKeyCollisionObject
{
    public long nameabcd1;
    public long nameabcd2;
}

/// <summary>
/// Equivalence and fallback-path tests for key prefix matching.
/// If the generation-time prefix constant and the runtime packing differ in a single bit, members silently keep their default value, so this behavior must be pinned down by tests.
/// </summary>
public sealed unsafe class CursorKeyTests
{
    private static ulong NaivePrefix(ReadOnlySpan<byte> key)
    {
        ulong value = 0;
        for (var i = 0; i < key.Length && i < 8; i++)
        {
            value |= (ulong) key[i] << (i * 8);
        }

        return value;
    }

    private static ulong RuntimePrefix(ReadOnlySpan<byte> key)
    {
        fixed (byte* p = key)
        {
            return StringHelper.GetKeyPrefix(p, key.Length);
        }
    }

    [Test]
    public void KeyPrefixMatchesNaiveTest()
    {
        foreach (var length in new[] { 0, 1, 2, 7, 8, 9, 15, 16, 17 })
        {
            var buffer = new byte[length];
            for (var i = 0; i < length; i++)
            {
                buffer[i] = (byte) (0x20 + (i * 7) % 0x5F);
            }

            Assert.AreEqual(NaivePrefix(buffer), RuntimePrefix(buffer), $"length={length}");
        }

        var special = new byte[] { 0x00, 0x80, 0xFF, 0x7F, 0x00, 0x41, 0x42, 0x43, 0x44 };
        Assert.AreEqual(NaivePrefix(special), RuntimePrefix(special));
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
        var value = JsonConverter.UTF8.Deserialize<CursorKeySimpleObject>(json);
        Assert.AreEqual(123456789L, value.nameabcd);
    }

    [Test]
    public void SharedPrefixMembersTest()
    {
        // The first 8 characters of two members are identical -> prefix collision -> fall back to the old path entirely, but deserialization must still be correct
        var json = "{\"nameabcd1\":11,\"nameabcd2\":22}"u8.ToArray();
        var value = JsonConverter.UTF8.Deserialize<CursorKeyCollisionObject>(json);
        Assert.AreEqual(11L, value.nameabcd1);
        Assert.AreEqual(22L, value.nameabcd2);
    }
}