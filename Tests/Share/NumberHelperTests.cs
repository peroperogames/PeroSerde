/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/14-18:59:57
 */

using System.Globalization;
using PeroTools2.Serde.Json;

namespace PeroTools2.Serde.Share.Tests;

public sealed class NumberHelperTests
{
    [TestCase(Encoding.UTF8, 0.15f)]
    [TestCase(Encoding.UTF8, 0.05f)]
    [TestCase(Encoding.UTF8, -0.15f)]
    [TestCase(Encoding.UTF8, 10.15f)]
    [TestCase(Encoding.UTF8, -10.15f)]
    [TestCase(Encoding.UTF16, 0.15f)]
    [TestCase(Encoding.UTF16, 0.05f)]
    [TestCase(Encoding.UTF16, -0.15f)]
    [TestCase(Encoding.UTF16, 10.15f)]
    [TestCase(Encoding.UTF16, -10.15f)]
    public unsafe void FloatToStringTests(Encoding encoding, float value)
    {
        if (encoding == Encoding.UTF8)
        {
            var dest = stackalloc byte[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf8StringUnsafe(value, dest);
            var span = new Span<byte>(dest, length);
            var result = MathF.Abs(float.Parse(System.Text.Encoding.UTF8.GetString(span)) - value);
            Assert.LessOrEqual(result, 0.001f);
        }
        else
        {
            var dest = stackalloc char[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf16StringUnsafe(value, dest);
            var span = new Span<char>(dest, length);
            Assert.AreEqual(value.ToString(CultureInfo.InvariantCulture), new string(span));
            var result = MathF.Abs(float.Parse(span) - value);
            Assert.LessOrEqual(result, 0.001f);
        }
    }

    [TestCase(Encoding.UTF8, 0.15d)]
    [TestCase(Encoding.UTF8, 0.05d)]
    [TestCase(Encoding.UTF8, -0.15d)]
    [TestCase(Encoding.UTF8, 10.15d)]
    [TestCase(Encoding.UTF8, -10.15d)]
    [TestCase(Encoding.UTF16, 0.15d)]
    [TestCase(Encoding.UTF16, 0.05d)]
    [TestCase(Encoding.UTF16, -0.15d)]
    [TestCase(Encoding.UTF16, 10.15d)]
    [TestCase(Encoding.UTF16, -10.15d)]
    public unsafe void DoubleToStringTests(Encoding encoding, double value)
    {
        if (encoding == Encoding.UTF8)
        {
            var dest = stackalloc byte[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf8StringUnsafe(value, dest);
            var span = new Span<byte>(dest, length);
            var result = MathF.Abs((float) (double.Parse(System.Text.Encoding.UTF8.GetString(span)) - value));
            Assert.LessOrEqual(result, 0.001d);
        }
        else
        {
            var dest = stackalloc char[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf16StringUnsafe(value, dest);
            var span = new Span<char>(dest, length);
            var result = MathF.Abs((float) (double.Parse(span) - value));
            Assert.LessOrEqual(result, 0.001d);
        }
    }

    [TestCase(Encoding.UTF8, 102)]
    [TestCase(Encoding.UTF8, -102)]
    [TestCase(Encoding.UTF16, 102)]
    [TestCase(Encoding.UTF16, -102)]
    public unsafe void LongToStringTests(Encoding encoding, long value)
    {
        if (encoding == Encoding.UTF8)
        {
            var dest = stackalloc byte[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf8StringUnsafe(value, dest);
            var span = new Span<byte>(dest, length);
            var result = MathF.Abs(long.Parse(System.Text.Encoding.UTF8.GetString(span)) - value);
            Assert.Less(result, 0.05f);
        }
        else
        {
            var dest = stackalloc char[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf16StringUnsafe(value, dest);
            var span = new Span<char>(dest, length);
            var result = MathF.Abs(long.Parse(span) - value);
            Assert.Less(result, 0.05f);
        }
    }

    [TestCase(Encoding.UTF8, (ulong) 102)]
    [TestCase(Encoding.UTF16, (ulong) 102)]
    public unsafe void UlongToStringTests(Encoding encoding, ulong value)
    {
        if (encoding == Encoding.UTF8)
        {
            var dest = stackalloc byte[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf8StringUnsafe(value, dest);
            var span = new Span<byte>(dest, length);
            var result = MathF.Abs(ulong.Parse(System.Text.Encoding.UTF8.GetString(span)) - value);
            Assert.Less(result, 0.05f);
        }
        else
        {
            var dest = stackalloc char[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf16StringUnsafe(value, dest);
            var span = new Span<char>(dest, length);
            var result = MathF.Abs(ulong.Parse(span) - value);
            Assert.Less(result, 0.05f);
        }
    }

    [TestCase(Encoding.UTF8, 2.596443771167918E-18)]
    [TestCase(Encoding.UTF16, 2.596443771167918E-18)]
    [TestCase(Encoding.UTF8, 1E-300)]
    [TestCase(Encoding.UTF16, 1E-300)]
    [TestCase(Encoding.UTF8, double.Epsilon)]
    [TestCase(Encoding.UTF16, double.Epsilon)]
    public unsafe void DoubleToStringIsWellFormedJsonTests(Encoding encoding, double value)
    {
        string text;
        if (encoding == Encoding.UTF8)
        {
            var dest = stackalloc byte[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf8StringUnsafe(value, dest);
            text = System.Text.Encoding.UTF8.GetString(new ReadOnlySpan<byte>(dest, length));
        }
        else
        {
            var dest = stackalloc char[FloatFormatter.max_size];
            var length = NumberHelper.ToUtf16StringUnsafe(value, dest);
            text = new String(new ReadOnlySpan<char>(dest, length));
        }

        Assert.That(text, Does.Match(@"^-?(0|[1-9]\d*)(\.\d+)?([eE][+-]?\d+)?$"));
    }
}