/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/04
 */

using System.Text;
using BenchmarkDotNet.Attributes;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using JsonSerializer = System.Text.Json.JsonSerializer;
using SwifterFormatter = Swifter.Json.JsonFormatter;

namespace PeroTools2.Serde.Benchmarks;

/// <summary>The payload and options shared by every serializer under test.</summary>
public static class Payload
{
    /// <summary>Json.net keeps the default contract: public fields, enums as numbers, chars as strings.</summary>
    public static readonly Newtonsoft.Json.JsonSerializerSettings newtonsoftSettings = new();

    /// <summary>System.Text.Json needs fields to be opted in.</summary>
    public static readonly System.Text.Json.JsonSerializerOptions systemTextJsonOptions = new()
    {
        IncludeFields = true
    };

    /// <summary>Canonical UTF16 json, produced by PeroSerde and accepted by every serializer here.</summary>
    public static readonly string utf16 = Json.JsonConverter.UTF16.Serialize(TestObject.instance);

    /// <summary>Canonical UTF8 json, produced by PeroSerde and accepted by every serializer here.</summary>
    public static readonly byte[] utf8 = Json.JsonConverter.UTF8.Serialize(TestObject.instance);

    /// <summary>Keeps the serialized result alive so the writes are not eliminated.</summary>
    public static string stringSink;

    /// <summary>Keeps the serialized result alive so the writes are not eliminated.</summary>
    public static byte[] bytesSink;

    /// <summary>Keeps the deserialized result alive so the reads are not eliminated.</summary>
    public static TestObject objectSink;
}

[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90)]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class SerializeBenchmarks
{
    [Benchmark]
    public void PeroSerde_Utf16()
    {
        Payload.stringSink = Json.JsonConverter.UTF16.Serialize(TestObject.instance);
    }

    [Benchmark]
    public void PeroSerde_Utf8()
    {
        Payload.bytesSink = Json.JsonConverter.UTF8.Serialize(TestObject.instance);
    }

    [Benchmark]
    public void NewtonsoftJson()
    {
        Payload.stringSink = JsonConvert.SerializeObject(TestObject.instance, Payload.newtonsoftSettings);
    }

    [Benchmark]
    public void SystemTextJson_String()
    {
        Payload.stringSink = JsonSerializer.Serialize(TestObject.instance, Payload.systemTextJsonOptions);
    }

    [Benchmark]
    public void SystemTextJson_Utf8()
    {
        Payload.bytesSink = JsonSerializer.SerializeToUtf8Bytes(TestObject.instance, Payload.systemTextJsonOptions);
    }

    [Benchmark(Baseline = true)]
    public void SwifterJson_Utf16()
    {
        Payload.stringSink = SwifterFormatter.SerializeObject(TestObject.instance);
    }

    [Benchmark]
    public void SwifterJson_Utf8()
    {
        Payload.bytesSink = SwifterFormatter.SerializeObject(TestObject.instance, Encoding.UTF8);
    }
}
[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90)]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class DeserializeBenchmarks
{
    [Benchmark]
    public void PeroSerde_Utf16()
    {
        Payload.objectSink = Json.JsonConverter.UTF16.Deserialize<TestObject>(Payload.utf16);
    }

    [Benchmark]
    public void PeroSerde_Utf8()
    {
        Payload.objectSink = Json.JsonConverter.UTF8.Deserialize<TestObject>(Payload.utf8);
    }

    [Benchmark]
    public void NewtonsoftJson()
    {
        Payload.objectSink = JsonConvert.DeserializeObject<TestObject>(Payload.utf16, Payload.newtonsoftSettings);
    }

    [Benchmark]
    public void SystemTextJson_String()
    {
        Payload.objectSink = JsonSerializer.Deserialize<TestObject>(Payload.utf16, Payload.systemTextJsonOptions);
    }

    [Benchmark]
    public void SystemTextJson_Utf8()
    {
        Payload.objectSink = JsonSerializer.Deserialize<TestObject>(Payload.utf8, Payload.systemTextJsonOptions);
    }

    [Benchmark(Baseline = true)]
    public void SwifterJson()
    {
        Payload.objectSink = SwifterFormatter.DeserializeObject<TestObject>(Payload.utf16);
    }

    [Benchmark]
    public void SwifterJson_Utf8()
    {
        Payload.objectSink = SwifterFormatter.DeserializeObject<TestObject>(Payload.utf8, Encoding.UTF8);
    }
}
