/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/06
 */

namespace PeroTools2.Serde.Json.Tests;

/// <summary>
/// A member whose type is a nullable reference type must not be treated as <c>Nullable&lt;T&gt;</c>; the generator used
/// to index <c>TypeArguments[0]</c> on <c>string</c> and crash with SERDE000
/// </summary>
[SerdeJsonSerializable]
public partial class NullableReferenceHolder
{
    public string? Name;
    public int Age;
}

[SerdeJsonSerializable]
public partial class NullableValueHolder
{
    public int? Score;
    public string Label;
}

public sealed class NullableReferenceTests
{
    [Test]
    public void NullableReferenceMemberTest()
    {
        var holder = new NullableReferenceHolder { Name = "Aiden", Age = 3 };
        var json = JsonConverter.UTF16.Serialize(holder);
        Assert.AreEqual("{\"Name\":\"Aiden\",\"Age\":3}", json);
        Assert.AreEqual("Aiden", JsonConverter.UTF16.Deserialize<NullableReferenceHolder>(json).Name);

        var nested = new NullableReferenceHolder { Name = null, Age = 4 };
        Assert.AreEqual("{\"Name\":null,\"Age\":4}", JsonConverter.UTF16.Serialize(nested));
        Assert.IsNull(JsonConverter.UTF16.Deserialize<NullableReferenceHolder>("{\"Name\":null,\"Age\":4}").Name);
    }

    [Test]
    public void NullableValueMemberTest()
    {
        var holder = new NullableValueHolder { Score = 7, Label = "x" };
        var json = JsonConverter.UTF16.Serialize(holder);
        Assert.AreEqual("{\"Score\":7,\"Label\":\"x\"}", json);

        var roundTrip = JsonConverter.UTF16.Deserialize<NullableValueHolder>(json);
        Assert.AreEqual(7, roundTrip.Score);
        Assert.AreEqual("x", roundTrip.Label);

        Assert.IsNull(JsonConverter.UTF16.Deserialize<NullableValueHolder>("{\"Score\":null,\"Label\":\"x\"}").Score);
    }
}
