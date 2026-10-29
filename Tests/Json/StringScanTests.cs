/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/03
 */

namespace PeroTools2.Serde.Json.Tests;

/// <summary>
/// Equivalence tests between the vectorized scan (4x uint) and the naive scalar implementation.
/// Zero-byte detection gives false positives when a borrow crosses byte boundaries, so these traps must be covered by tests.
/// </summary>
public sealed unsafe class StringScanTests
{
    private const int capacity = 72;

    private static int NaiveIndexOf(byte* bytes, int length, byte c)
    {
        for (var i = 0; i < length; i++)
        {
            if (bytes[i] == c)
            {
                return i;
            }
        }
        return -1;
    }

    private static int NaiveIndexOfAny(byte* bytes, int length, byte c1, byte c2)
    {
        for (var i = 0; i < length; i++)
        {
            var b = bytes[i];
            if (b == c1 || b == c2)
            {
                return i;
            }
        }
        return -1;
    }

    private static int NaiveIndexOf(char* chars, int length, char c)
    {
        for (var i = 0; i < length; i++)
        {
            if (chars[i] == c)
            {
                return i;
            }
        }
        return -1;
    }

    private static int NaiveIndexOfAny(char* chars, int length, char c1, char c2)
    {
        for (var i = 0; i < length; i++)
        {
            var c = chars[i];
            if (c == c1 || c == c2)
            {
                return i;
            }
        }
        return -1;
    }

    [Test]
    public void ByteIndexOfMatchesNaive()
    {
        var buffer = stackalloc byte[capacity];
        byte[] needles = { 0x00, 0x01, 0x22, 0x5c, 0x7f, 0x80, 0xff };
        foreach (var needle in needles)
        {
            for (var length = 0; length <= capacity; length++)
            {
                for (var hit = 0; hit <= length; hit++)
                {
                    // Fill with 0x00/0x01/0x02 to create cross-byte borrow traps for zero-byte detection
                    for (var i = 0; i < capacity; i++)
                    {
                        buffer[i] = (byte) (i % 3);
                    }
                    if (hit < length)
                    {
                        buffer[hit] = needle;
                    }

                    var expected = NaiveIndexOf(buffer, length, needle);
                    var actual = StringHelper.IndexOf(buffer, length, needle);
                    Assert.AreEqual(expected, actual, $"needle=0x{needle:X2} length={length} hit={hit}");
                    Assert.AreEqual(expected >= 0, StringHelper.Contains(buffer, length, needle), $"Contains needle=0x{needle:X2} length={length} hit={hit}");
                }
            }
        }
    }

    [Test]
    public void ByteIndexOfAnyMatchesNaive()
    {
        var buffer = stackalloc byte[capacity];
        (byte, byte)[] pairs = { (0x00, 0x01), (0x22, 0x5c), (0x80, 0x7f), (0xff, 0x00) };
        foreach (var (c1, c2) in pairs)
        {
            for (var length = 0; length <= capacity; length++)
            {
                for (var hit = 0; hit <= length; hit++)
                {
                    for (var i = 0; i < capacity; i++)
                    {
                        buffer[i] = (byte) (i % 3);
                    }
                    if (hit < length)
                    {
                        buffer[hit] = c2;
                    }

                    var expected = NaiveIndexOfAny(buffer, length, c1, c2);
                    var actual = StringHelper.IndexOfAny(buffer, length, c1, c2);
                    Assert.AreEqual(expected, actual, $"c1=0x{c1:X2} c2=0x{c2:X2} length={length} hit={hit}");
                }
            }
        }
    }

    [Test]
    public void CharIndexOfMatchesNaive()
    {
        var buffer = stackalloc char[capacity];
        char[] needles = { '\0', '\u0001', '"', '\\', '\u007f', '\u0080', '\uffff' };
        foreach (var needle in needles)
        {
            for (var length = 0; length <= capacity; length++)
            {
                for (var hit = 0; hit <= length; hit++)
                {
                    for (var i = 0; i < capacity; i++)
                    {
                        buffer[i] = (char) (i % 3);
                    }
                    if (hit < length)
                    {
                        buffer[hit] = needle;
                    }

                    var expected = NaiveIndexOf(buffer, length, needle);
                    var actual = StringHelper.IndexOf(buffer, length, needle);
                    Assert.AreEqual(expected, actual, $"needle=U+{(int) needle:X4} length={length} hit={hit}");
                    Assert.AreEqual(expected >= 0, StringHelper.Contains(buffer, length, needle));
                }
            }
        }
    }

    [Test]
    public void CharIndexOfAnyMatchesNaive()
    {
        var buffer = stackalloc char[capacity];
        (char, char)[] pairs = { ('\0', '\u0001'), ('"', '\\'), ('\u0080', '\u007f'), ('\uffff', '\0') };
        foreach (var (c1, c2) in pairs)
        {
            for (var length = 0; length <= capacity; length++)
            {
                for (var hit = 0; hit <= length; hit++)
                {
                    for (var i = 0; i < capacity; i++)
                    {
                        buffer[i] = (char) (i % 3);
                    }
                    if (hit < length)
                    {
                        buffer[hit] = c2;
                    }

                    var expected = NaiveIndexOfAny(buffer, length, c1, c2);
                    var actual = StringHelper.IndexOfAny(buffer, length, c1, c2);
                    Assert.AreEqual(expected, actual, $"c1=U+{(int) c1:X4} c2=U+{(int) c2:X4} length={length} hit={hit}");
                }
            }
        }
    }
}