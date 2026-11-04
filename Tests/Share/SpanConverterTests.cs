/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2025/01/15-16:49:18
 */

using System.Globalization;

namespace PeroTools2.Serde.Share.Tests;

public sealed class SpanConverterTests
{
    [TestCase(Encoding.UTF8, short.MaxValue)]
    [TestCase(Encoding.UTF8, short.MinValue)]
    [TestCase(Encoding.UTF16, short.MaxValue)]
    [TestCase(Encoding.UTF16, short.MinValue)]
    public void ConvertToShortTest(Encoding encoding, short value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString()).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToShort(result));
        }
        else
        {
            var result = value.ToString().AsSpan();
            Assert.AreEqual(value, SpanConverter.ToShort(result));
        }
    }

    [TestCase(Encoding.UTF8, int.MaxValue)]
    [TestCase(Encoding.UTF8, int.MinValue)]
    [TestCase(Encoding.UTF16, int.MaxValue)]
    [TestCase(Encoding.UTF16, int.MinValue)]
    public void ConvertToIntTest(Encoding encoding, int value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString()).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToInt(result));
        }
        else
        {
            var result = value.ToString().AsSpan();
            Assert.AreEqual(value, SpanConverter.ToInt(result));
        }
    }

    [TestCase(Encoding.UTF8, long.MaxValue)]
    [TestCase(Encoding.UTF8, long.MinValue)]
    [TestCase(Encoding.UTF16, long.MaxValue)]
    [TestCase(Encoding.UTF16, long.MinValue)]
    public void ConvertToLongTest(Encoding encoding, long value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString()).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToLong(result));
        }
        else
        {
            var result = value.ToString().AsSpan();
            Assert.AreEqual(value, SpanConverter.ToLong(result));
        }
    }

    [TestCase(Encoding.UTF8, byte.MaxValue)]
    [TestCase(Encoding.UTF8, byte.MinValue)]
    [TestCase(Encoding.UTF16, byte.MaxValue)]
    [TestCase(Encoding.UTF16, byte.MinValue)]
    public void ConvertToByteTest(Encoding encoding, byte value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString()).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToByte(result));
        }
        else
        {
            var result = value.ToString().AsSpan();
            Assert.AreEqual(value, SpanConverter.ToByte(result));
        }
    }

    [TestCase(Encoding.UTF8, ushort.MaxValue)]
    [TestCase(Encoding.UTF8, ushort.MinValue)]
    [TestCase(Encoding.UTF16, ushort.MaxValue)]
    [TestCase(Encoding.UTF16, ushort.MinValue)]
    public void ConvertToUshortTest(Encoding encoding, ushort value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString()).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToUshort(result));
        }
        else
        {
            var result = value.ToString().AsSpan();
            Assert.AreEqual(value, SpanConverter.ToUshort(result));
        }
    }

    [TestCase(Encoding.UTF8, uint.MaxValue)]
    [TestCase(Encoding.UTF8, uint.MinValue)]
    [TestCase(Encoding.UTF16, uint.MaxValue)]
    [TestCase(Encoding.UTF16, uint.MinValue)]
    public void ConvertToUintTest(Encoding encoding, uint value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString()).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToUint(result));
        }
        else
        {
            var result = value.ToString().AsSpan();
            Assert.AreEqual(value, SpanConverter.ToUint(result));
        }
    }

    [TestCase(Encoding.UTF8, ulong.MaxValue)]
    [TestCase(Encoding.UTF8, ulong.MinValue)]
    [TestCase(Encoding.UTF16, ulong.MaxValue)]
    [TestCase(Encoding.UTF16, ulong.MinValue)]
    public void ConvertToUlongTest(Encoding encoding, ulong value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString()).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToUlong(result));
        }
        else
        {
            var result = value.ToString().AsSpan();
            Assert.AreEqual(value, SpanConverter.ToUlong(result));
        }
    }

    [TestCase(Encoding.UTF8, sbyte.MaxValue)]
    [TestCase(Encoding.UTF8, sbyte.MinValue)]
    [TestCase(Encoding.UTF16, sbyte.MaxValue)]
    [TestCase(Encoding.UTF16, sbyte.MinValue)]
    public void ConvertToSbyteTest(Encoding encoding, sbyte value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString()).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToSbyte(result));
        }
        else
        {
            var result = value.ToString().AsSpan();
            Assert.AreEqual(value, SpanConverter.ToSbyte(result));
        }
    }

    [TestCase(Encoding.UTF8, 0.0523d)]
    [TestCase(Encoding.UTF8, 12.3E+3)]
    [TestCase(Encoding.UTF16, 0.0523d)]
    [TestCase(Encoding.UTF16, 12.3E+3)]
    public void ConvertToDoubleTest(Encoding encoding, double value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString(CultureInfo.InvariantCulture)).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToDouble(result));
        }
        else
        {
            var result = value.ToString(CultureInfo.InvariantCulture).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToDouble(result));
        }
    }

    [TestCase(Encoding.UTF8, 0.0523f)]
    [TestCase(Encoding.UTF8, 12f)]
    [TestCase(Encoding.UTF16, 0.0523f)]
    [TestCase(Encoding.UTF16, 12f)]
    public void ConvertToFloatTest(Encoding encoding, float value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString(CultureInfo.InvariantCulture)).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToFloat(result));
        }
        else
        {
            var result = value.ToString(CultureInfo.InvariantCulture).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToFloat(result));
        }
    }

    [TestCase(Encoding.UTF8, 1 / 3)]
    [TestCase(Encoding.UTF8, -1 / 3)]
    [TestCase(Encoding.UTF16, 1 / 3)]
    [TestCase(Encoding.UTF16, -1 / 3)]
    public void ConvertToDecimalTest(Encoding encoding, decimal value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString(CultureInfo.InvariantCulture)).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToDecimal(result));
        }
        else
        {
            var result = value.ToString(CultureInfo.InvariantCulture).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToDecimal(result));
        }
    }

    [TestCase(Encoding.UTF8, 'O')]
    [TestCase(Encoding.UTF8, '啊')]
    [TestCase(Encoding.UTF16, 'O')]
    [TestCase(Encoding.UTF16, '啊')]
    public void ConvertToCharTest(Encoding encoding, char value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value.ToString()).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToChar(result));
        }
        else
        {
            var result = value.ToString().AsSpan();
            Assert.AreEqual(value, SpanConverter.ToChar(result));
        }
    }

    [TestCase(Encoding.UTF8, true)]
    [TestCase(Encoding.UTF8, false)]
    [TestCase(Encoding.UTF16, true)]
    [TestCase(Encoding.UTF16, false)]
    public void ConvertToBoolTest(Encoding encoding, bool value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value ? "true" : "false").AsSpan();
            Assert.AreEqual(value, SpanConverter.ToBool(result));
        }
        else
        {
            var result = value ? "true" : "false".AsSpan();
            Assert.AreEqual(value, SpanConverter.ToBool(result));
        }
    }

    [TestCase(Encoding.UTF8, "This is a string")]
    [TestCase(Encoding.UTF8, "")]
    [TestCase(Encoding.UTF16, "This is a string")]
    [TestCase(Encoding.UTF16, "")]
    public void ConvertToStringTest(Encoding encoding, string value)
    {
        if (encoding == Encoding.UTF8)
        {
            var result = System.Text.Encoding.UTF8.GetBytes(value).AsSpan();
            Assert.AreEqual(value, SpanConverter.ToString(result));
        }
        else
        {
            var result = value.AsSpan();
            Assert.AreEqual(value, SpanConverter.ToString(result));
        }
    }
}