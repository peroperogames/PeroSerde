/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/04
 */

using System;
using BenchmarkDotNet.Running;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using JsonSerializer = System.Text.Json.JsonSerializer;
using SwifterJson = Swifter.Json.JsonFormatter;

namespace PeroTools2.Serde.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        Verify();
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }

    /// <summary>
    /// Every serializer in this project must produce an equivalent document and round-trip the same object, otherwise
    /// the numbers compare different work.
    /// </summary>
    private static void Verify()
    {
        var json = Payload.utf16;
        var bytes = Payload.utf8;

        Console.WriteLine($"[bench] payload: {bytes.Length} bytes (utf8), {json.Length} chars (utf16)");
        Console.WriteLine($"[bench] json: {json}");

        Check("PeroSerde UTF16", () => Json.JsonConverter.UTF16.Deserialize<TestObject>(json));
        Check("PeroSerde UTF8", () => Json.JsonConverter.UTF8.Deserialize<TestObject>(bytes));
        Check("Newtonsoft.Json", () => JsonConvert.DeserializeObject<TestObject>(json, Payload.newtonsoftSettings));
        Check("System.Text.Json string", () => JsonSerializer.Deserialize<TestObject>(json, Payload.systemTextJsonOptions));
        Check("System.Text.Json utf8", () => JsonSerializer.Deserialize<TestObject>(bytes, Payload.systemTextJsonOptions));
        Check("Swifter.Json", () => SwifterJson.DeserializeObject<TestObject>(json));

        var newtonsoftJson = JsonConvert.SerializeObject(TestObject.instance, Payload.newtonsoftSettings);
        var systemTextJson = JsonSerializer.Serialize(TestObject.instance, Payload.systemTextJsonOptions);
        var swifterJson = SwifterJson.SerializeObject(TestObject.instance);
        Console.WriteLine($"[bench] newtonsoft length: {newtonsoftJson.Length}");
        Console.WriteLine($"[bench] system.text.json length: {systemTextJson.Length}");
        Console.WriteLine($"[bench] swifter.json length: {swifterJson.Length}");
        Console.WriteLine($"[bench] swifter.json: {swifterJson}");
    }

    private static void Check(string name, Func<TestObject> deserialize)
    {
        var value = deserialize() ?? throw new InvalidOperationException($"{name}: null result");
        if (value.stringValue != TestObject.instance.stringValue
            || value.intValue != TestObject.instance.intValue
            // The default float formatter keeps six significant digits, so the comparison allows its rounding
            || Math.Abs(value.floatValue - TestObject.instance.floatValue) > 1e-4f
            || value.boolValue != TestObject.instance.boolValue
            || value.listValue.Count != TestObject.instance.listValue.Count
            || value.objValue.charValue != TestObject.instance.objValue.charValue
            || value.objValue.arrayValue.Length != TestObject.instance.objValue.arrayValue.Length
            || value.objValue.enumValue != TestObject.instance.objValue.enumValue)
        {
            throw new InvalidOperationException($"{name}: round trip mismatch");
        }
        Console.WriteLine($"[bench] {name}: round trip ok");
    }
}
