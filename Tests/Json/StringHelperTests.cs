/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/03
 */

using System.Text;

namespace PeroTools2.Serde.Json.Tests;

/// <summary>
/// Case-insensitive comparison must only fold ASCII letter case, never turn one character into another one.
/// </summary>
public sealed class StringHelperTests
{
    // Every pair differs at the first character: the two characters are not case variants of each other.
    [TestCase('[', '{')]
    [TestCase('@', '`')]
    [TestCase(']', '}')]
    [TestCase('^', '~')]
    [TestCase('_', '\u007F')]
    [TestCase('\u0000', '\u0020')]
    public void EqualsWithIgnoreCaseRejectsUnrelatedCharactersTests(char x, char y)
    {
        Assert.IsFalse(StringHelper.EqualsWithIgnoreCase(Chars(x, 1), Chars(y, 1)));
        Assert.IsFalse(StringHelper.EqualsWithIgnoreCase(Bytes(x, 1), Bytes(y, 1)));
        Assert.IsFalse(StringHelper.EqualsWithIgnoreCase(Chars(x, 1), new string(y, 1)));
        Assert.IsTrue(StringHelper.EqualsWithIgnoreCase(Chars(x, 1), Chars(x, 1)));
        Assert.IsTrue(StringHelper.EqualsWithIgnoreCase(Bytes(x, 1), Bytes(x, 1)));
        Assert.IsTrue(StringHelper.EqualsWithIgnoreCase(Chars(x, 1), new string(x, 1)));
    }

    // Lengths staying below, exactly at and above the former vector stride, plus a difference in the tail.
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(9)]
    [TestCase(16)]
    [TestCase(17)]
    public void EqualsWithIgnoreCaseLengthTests(int length)
    {
        var lower = new string('a', length);
        var upper = new string('A', length);

        Assert.IsTrue(StringHelper.EqualsWithIgnoreCase(Chars(lower), Chars(upper)));
        Assert.IsTrue(StringHelper.EqualsWithIgnoreCase(Bytes(lower), Bytes(upper)));
        Assert.IsTrue(StringHelper.EqualsWithIgnoreCase(Chars(lower), upper));

        var mismatched = new StringBuilder(upper);
        if (length > 0)
        {
            mismatched[length - 1] = 'b';
        }
        else
        {
            mismatched.Append('b');
        }
        var other = mismatched.ToString();

        Assert.IsFalse(StringHelper.EqualsWithIgnoreCase(Chars(lower), Chars(other)));
        Assert.IsFalse(StringHelper.EqualsWithIgnoreCase(Bytes(lower), Bytes(other)));
        Assert.IsFalse(StringHelper.EqualsWithIgnoreCase(Chars(lower), other));
    }

    [TestCase("abc")]
    [TestCase("aBc")]
    [TestCase("ABC")]
    [TestCase("[abc]")]
    [TestCase("{abc}")]
    public void EqualsWithIgnoreCaseStringTests(string value)
    {
        Assert.IsTrue(StringHelper.EqualsWithIgnoreCase(Chars(value), value));
        Assert.IsTrue(StringHelper.EqualsWithIgnoreCase(Bytes(value), Bytes(value)));
    }

    // Shorter buffers of the same prefix are never equal, regardless of case folding
    [TestCase("abc", "abcd")]
    [TestCase("abcd", "abc")]
    public void EqualsWithIgnoreCaseDifferentLengthTests(string x, string y)
    {
        Assert.IsFalse(StringHelper.EqualsWithIgnoreCase(Chars(x), Chars(y)));
        Assert.IsFalse(StringHelper.EqualsWithIgnoreCase(Bytes(x), Bytes(y)));
        Assert.IsFalse(StringHelper.EqualsWithIgnoreCase(Chars(x), y));
    }

    private static Span<char> Chars(string value) => value.ToCharArray();

    private static Span<char> Chars(char value, int length) => new string(value, length).ToCharArray();

    private static Span<byte> Bytes(string value) => Encoding.UTF8.GetBytes(value);

    private static Span<byte> Bytes(char value, int length) => Encoding.UTF8.GetBytes(new string(value, length));
}