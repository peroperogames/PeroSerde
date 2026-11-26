/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/26
 */

using System;
using System.Collections.Generic;

namespace PeroTools2.Serde.Json.Tests;

/// <summary>
/// Element type without a generated formatter, registered by hand after the container formatter
/// </summary>
public sealed class LateElement
{
    public int value;
}

public sealed class LateElementFormatter : ISerdeJsonFormatter<LateElement>
{
    public void SerializeUtf8(LateElement value, ref JsonWriterUtf8 writer) => writer.WriteValue(value.value);

    public void SerializeUtf16(LateElement value, ref JsonWriterUtf16 writer) => writer.WriteValue(value.value);

    public LateElement DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader) =>
        new LateElement { value = reader.ReadValue<int>() };

    public LateElement DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader) =>
        new LateElement { value = reader.ReadValue<int>() };
}

/// <summary>
/// The container formatters must not look the element formatter up in their static constructor. A generated registrar
/// runs the static constructor of the containing type, which can happen before the element type got registered - in
/// Unity that is the normal case for UnityEngine types, whose formatters are registered in a RuntimeInitializeOnLoadMethod
/// </summary>
public struct LateKey
{
    public int value;
}

public sealed class LateKeyFormatter : ISerdeJsonFormatter<LateKey>
{
    public void SerializeUtf8(LateKey value, ref JsonWriterUtf8 writer) => writer.WriteValue(value.value);

    public void SerializeUtf16(LateKey value, ref JsonWriterUtf16 writer) => writer.WriteValue(value.value);

    public LateKey DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader) =>
        new LateKey { value = reader.ReadValue<int>() };

    public LateKey DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader) =>
        new LateKey { value = reader.ReadValue<int>() };
}

public sealed class FormatterResolutionTests
{
    [Test]
    public void CollectionFormatterResolvesElementFormatterLateTest()
    {
        // the element formatter is missing at this point; this used to throw from the static constructor
        SerdeJsonFormatterProvider.Register(new CollectionFormatter<List<LateElement>, LateElement>());
        RegisterElementFormatter();

        var list = new List<LateElement> { new LateElement { value = 3 }, new LateElement { value = 4 } };

        Assert.AreEqual("[3,4]", JsonConverter.UTF16.Serialize(list));
        Assert.AreEqual(2, JsonConverter.UTF16.Deserialize<List<LateElement>>("[3,4]").Count);
        Assert.AreEqual("[3,4]", System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(list)));
    }

    [Test]
    public void ArrayFormatterResolvesElementFormatterLateTest()
    {
        SerdeJsonFormatterProvider.Register(new ArrayFormatter<LateElement>());
        RegisterElementFormatter();

        var array = new[] { new LateElement { value = 5 } };

        Assert.AreEqual("[5]", JsonConverter.UTF16.Serialize(array));
        Assert.AreEqual(5, JsonConverter.UTF16.Deserialize<LateElement[]>("[5]")[0].value);
    }

    [Test]
    public void DictionaryFormatterResolvesKeyFormatterLateTest()
    {
        SerdeJsonFormatterProvider.Register(new DictionaryFormatter<Dictionary<LateKey, int>, LateKey, int>());
        RegisterKeyFormatter();

        var dictionary = new Dictionary<LateKey, int> { { new LateKey { value = 3 }, 7 } };

        // the key is written through ToString because the key formatter is not a fixed one, the point here is that
        // registering this formatter before the key type used to throw from the static field initializer
        Assert.AreEqual("{\"PeroTools2.Serde.Json.Tests.LateKey\":7}", JsonConverter.UTF16.Serialize(dictionary));
    }

    [Test]
    public void KeyValuePairFormatterResolvesKeyFormatterLateTest()
    {
        SerdeJsonFormatterProvider.Register(new KeyValuePairFormatter<LateKey, int>());
        RegisterKeyFormatter();

        var pair = new KeyValuePair<LateKey, int>(new LateKey { value = 4 }, 9);

        Assert.AreEqual("{\"PeroTools2.Serde.Json.Tests.LateKey\":9}", JsonConverter.UTF16.Serialize(pair));
    }

    private static void RegisterKeyFormatter()
    {
        if (!SerdeJsonFormatterProvider.IsRegistered<LateKey>())
        {
            SerdeJsonFormatterProvider.Register(new LateKeyFormatter());
        }
    }

    private static void RegisterElementFormatter()
    {
        // the tests share the global registry, so only register once and keep the ordering point of each test
        if (!SerdeJsonFormatterProvider.IsRegistered<LateElement>())
        {
            SerdeJsonFormatterProvider.Register(new LateElementFormatter());
        }
    }
}
