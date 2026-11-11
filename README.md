# PeroSerde

A zero-GC, reflection-free, high-performance serializer/deserializer with UTF8/UTF16 support. PeroSerde supports the following types out of the box:

1. **Primitive types:** string, int, uint, short, ushort, long, ulong, double, float, byte, sbyte, bool, char, decimal

2. **.NET built-in types:** Nullable\<TStruct\>, Array, List\<T\>, Dictionary\<T\>, Lazy\<T\>, DateTime, Guid, KeyValuePair\<T\>

3. **Unity built-in types:** Color, Vector2, Vector3, Vector4, Vector2Int, Vector3Int, Quaternion

4. **Enums:** Enum, Flags

## Requirements

- Unity 2021.3 or later.
- [UniTask](https://github.com/Cysharp/UniTask) (`com.cysharp.unitask`) is **optional**. When the package is present the asynchronous command handlers and the operation helpers are `UniTask` based; when it is absent they fall back to `Task` and the package has no external dependency. Declare it yourself in your project manifest if you want the `UniTask` flavour:

  ```json
  { "dependencies": { "org.nuget.system.runtime.compilerservices.unsafe": "6.0.0" } }
  ```

## Install

### Nuget

Append following content to project file (`.csproj`):

```xml

<ItemGroup>
    <PackageReference Include="PeroTools2.Serde" Version="0.2.0"/>
</ItemGroup>
```

Or install via CLI:

```sh
dotnet add package PeroTools2.Serde --version 0.2.0
```

### Unity

Append following content to `Packages/manifest.json`:

**From [GIT](https://github.com/peroperogames/PeroSerde):**

```json
{
  "dependencies": {
    "com.prpr.serde": "https://github.com/peroperogames/PeroSerde.git#0.2.0-base"
  }
}
```

**From [NPMJS](https://www.npmjs.com/package/com.prpr.serde/):**

```json
{
  "scopedRegistries": [
    {
      "name": "npmjs",
      "url": "https://registry.npmjs.org",
      "scopes": [
        "com.prpr"
      ]
    }
  ],
  "dependencies": {
    "com.prpr.serde": "0.2.0"
  }
}
```

**From [OpenUPM](https://openupm.com/packages/com.prpr.serde/):**

```json
{
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.prpr"
      ]
    }
  ],
  "dependencies": {
    "com.prpr.serde": "0.2.0"
  }
}
```

Or install via CLI:

```sh
openupm add com.prpr.serde
```

# Json

## Serialization/Deserialization

PeroSerde offers two ways to serialize/deserialize Json: UTF16 (for ordinary strings) and UTF8 (for streaming).

```C#
using PeroTools2.Serde.Json;

var myObj = new MyObject(){ ... };

// Serialize
string stringJson = JsonConverter.UTF16.Serialize(myObj);
byte[] byteJson   = JsonConverter.UTF8.Serialize(myObj);

// Deserialize
var myobj1 = JsonConverter.UTF16.Deserialize<MyObject>(stringJson);
var myobj2 = JsonConverter.UTF8.Deserialize<MyObject>(byteJson);

[SerdeJsonSerializable]
public partial class MyObject
{
    public int id;
    public string name;
    public List<MyObject> list;
    public Dictionary<string, MyObject> dict;
}
```

You can also serialize/deserialize through a Formatter instance.

> A `Formatter` tells Serde how a type has to be serialized and deserialized, see: [SerdeJsonFormatterAttribute](#serdejsonformatterattribute-use-a-custom-formatter-for-a-member), [Custom Formatter](#custom-formatters), [Built-in Formatter](#built-in-formatters)

```C#
var myFormatter = new MyFormatter();

// Serialize
string stringJson = JsonConverter.UTF16.Serialize(myObj, myFormatter);
byte[] byteJson   = JsonConverter.UTF8.Serialize(myObj, myFormatter);

// Deserialize
var myobj1 = JsonConverter.UTF16.Deserialize<MyObject>(stringJson, myFormatter);
var myobj2 = JsonConverter.UTF8.Deserialize<MyObject>(byteJson, myFormatter);
```

## Features

### SerdeJsonSerializableAttribute: generate the Formatter automatically

Custom Structs, Classes and Enums have to be marked with `[SerdeJsonSerializable]` so that Serde can recognize the structure and create a Formatter for it. Classes and Structs must also be `partial`:

```C#
using PeroTools2.Serde.Json;
[SerdeJsonSerializable]
public partial class MyObject
{
    public int id;
    public string name;
    public List<MyObject> list;
    public Dictionary<string, MyObject> dict;
}
```

> SerdeJsonSerializableAttribute has limitations, see [Formatter limitations](#formatter-limitations)

The attribute also accepts two parameters:

```C#
/// <param name="ignoreMissingMembers">When true, missing members are ignored while deserializing</param>
/// <param name="ignoreNullMembers">When true, members whose value is null are skipped while serializing</param>
public SerdeJsonSerializableAttribute(bool ignoreMissingMembers = true, bool ignoreNullMembers = false)
```

### SerdeJsonMemberAttribute: customize the serialized member name

Serde uses the member name as the Json key by default. Add `[SerdeJsonMember]` to a member to use a custom name:

```C#
using PeroTools2.Serde.Json;
[SerdeJsonSerializable]
public partial class MyObject
{
    [SerdeJsonMember("obj_id")]
    public int id;
    [SerdeJsonMember("obj_name")]
    public string name;
    [SerdeJsonMember("obj_list")]
    public List<MyObject> list;
    [SerdeJsonMember("obj_dict")]
    public Dictionary<string, MyObject> dict;
}
```

### SerdeJsonIgnoreMemberAttribute: skip a member

Serde serializes and deserializes every member by default. Use `[SerdeJsonIgnoreMember]` to skip members:

```C#
using PeroTools2.Serde.Json;
[SerdeJsonSerializable]
public partial class MyObject
{
    public int id;
    public List<MyObject> list;
    public Dictionary<string, MyObject> dict;

    [SerdeJsonIgnoreMember]
    public string name => id.ToString();
}
```

### SerdeJsonFormatterAttribute: use a custom Formatter for a member

`[SerdeJsonFormatter]` lets a single member use a specific Formatter, which Serde registers at the right moment:

```C#
using PeroTools2.Serde.Json;
[SerdeJsonSerializable]
public partial class MyObject
{
    public int id;
    public string name;
    public List<MyObject> list;
    [SerdeJsonFormatter(typeof(DictionaryFormatter<string, MyObject>))]
    public Dictionary<string, MyObject> dict;
}
```

### SerdeJsonEnumFormatterAttribute: customize how an enum is formatted

`[SerdeJsonEnumFormatter]` customizes how an enum is serialized. It takes two parameters:

1. serializationFormat: the format used while serializing

2. deserializationFormat: the format used while deserializing

Only strings and numbers are supported for now. The enum below is written as a string and read as a number:

```C#
using PeroTools2.Serde.Json;
[SerdeJsonSerializable]
[SerdeJsonEnumFormatter(SerdeEnumFormat.String, SerdeEnumFormat.Number)]
public enum MyEnum
{
    A = 1,
    B = 2,
    C = 3
}
[SerdeJsonSerializable]
public partial class MyClass
{
    public MyEnum value;
}

// Serialize
var obj = new MyClass() { value = MyEnum.B };
var json = JsonConverter.UTF16.Serialize(obj);
Debug.Log(json); // {"value":"B"}

// Deserialize
json = "{\"value\":3}";
obj = JsonConverter.UTF16.Deserialize<MyClass>(json);
Debug.Log(obj.value); // C
```

`[SerdeJsonEnumFormatter]` can be combined with [[SerdeJsonMember]](#serdejsonmemberattribute-customize-the-serialized-member-name): when `SerdeEnumFormat` is `SerdeEnumFormat.String`, the name given by `[SerdeJsonMember]` becomes the written value.

```C#
using PeroTools2.Serde.Json;
[SerdeJsonSerializable]
[SerdeJsonEnumFormatter(SerdeEnumFormat.String, SerdeEnumFormat.String)]
public enum MyEnum
{
    A = 1,
    B = 2,
    [SerdeJsonMember("I'm still C")]
    C = 3
}
[SerdeJsonSerializable]
public partial class MyClass
{
    public MyEnum value;
}

// Serialize
var obj = new MyClass() { value = MyEnum.C };
var json = JsonConverter.UTF16.Serialize(obj);
Debug.Log(json); // {"value":"I'm still C"}

// Deserialize
json = "{\"value\":\"I'm still C\"}";
obj = JsonConverter.UTF16.Deserialize<MyClass>(json);
Debug.Log(obj.value); // C
```

## Custom Formatters

### Using EncodingJsonFormatter\<T\>

`EncodingJsonFormatter` is the simplified Formatter. It only supports primitive values (**it cannot serialize/deserialize Json objects or arrays**). Its advantage is that it hides the encoding and the reader/writer stepping: only the encoded form matters.

```C#
using PeroTools2.Serde.Json;

public class MyObject
{
    public string name;
}
// This Formatter switches between string/byte[] and MyObject automatically
public class MyFormatter : EncodingJsonFormatter<MyObject>
{
    protected override MyObject DeserializeUtf16(ReadOnlySpan<char> value)
    {
        return new MyObject() { name = value };
    }

    protected override MyObject DeserializeUtf8(ReadOnlySpan<byte> value)
    {
        return new MyObject() { name = Encoding.UTF8.GetString(value) };
    }

    protected override ReadOnlySpan<char> SerializeUtf16(MyObject value)
    {
        return value.name;
    }

    protected override ReadOnlySpan<byte> SerializeUtf8(MyObject value)
    {
        return Encoding.UTF8.GetBytes(value.name);
    }
}
```

### Using PrimitiveJsonFormatter\<T\>

`PrimitiveJsonFormatter` is a further simplified [EncodingJsonFormatter](#using-encodingjsonformattert) and the least extensible Formatter (**it cannot serialize/deserialize Json objects or arrays**). You do not deal with UTF8 at all, Serde switches between UTF16 and UTF8 for you.

```C#
using PeroTools2.Serde.Json;
// This Formatter switches between string/byte[] and MyObject automatically
public class MyFormatter : PrimitiveJsonFormatter<MyObject>
{
    protected override MyObject Deserialize(ReadOnlySpan<char> value)
    {
        return new MyObject() { name = value };
    }
    protected override ReadOnlySpan<char> Serialize(MyObject value)
    {
        return value.name;
    }
}
```

### Using ISerdeJsonFormatter\<T\>

`ISerdeJsonFormatter` can serialize any type. You then have to drive the reader/writer yourself so Serde knows how far to step. A simple example:

```C#
using PeroTools2.Serde.Json;
public class MyClass
{
    public int number;
    public string name;
    public int[] list;
}

public class MyClassFormatter : ISerdeJsonFormatter<MyClass>
{
    public void SerializeUtf8(MyClass value, ref JsonWriterUtf8 writer)
    {
        // Same as UTF16
    }

    // Serialize to a UTF16 string
    // value:  the value to write
    // writer: the writer
    public void SerializeUtf16(MyClass value, ref JsonWriterUtf16 writer)
    {
        writer.WriteBeginObject();                             // {
        writer.WriteNamedValue("number", value.number, true);  // "number":xxx,
        writer.WriteNamedValue("name", value.name, true);      // "name":xxx,
        writer.WriteBeginArray();                              // [
        for (var i = 0; i < value.list.Length; i++)
        {
            // No comma after the last element
            var writeComma = i < value.list.Length - 1;
            writer.WriteValue(value.list[i], writeComma);      // 1,2,3....
        }
        writer.WriteEndArray();                                // ]
        writer.WriteEndObject();                               // }
    }

    public MyClass DeserializeUtf8(ref ReadOnlySpan<byte> remains, ref JsonReaderUtf8 reader)
    {
        // Same as UTF16
        return default;
    }

    // Deserialize into MyClass
    // remains: the remaining json
    // reader:  the reader
    public MyClass DeserializeUtf16(ref ReadOnlySpan<char> remains, ref JsonReaderUtf16 reader)
    {
        var obj = new MyClass();
        reader.ReadBeginObject();
        const int memberLength = 3;
        for (var i = 0; i < memberLength; i++)
        {
            var key = reader.ReadKey();
            // No comma after the last member
            OnReadKey(ref key, ref obj, ref reader, i < memberLength - 1);
        }
        reader.ReadEndObject();
        return obj;
    }

    private void OnReadKey(
        ref ReadOnlySpan<char> key,
        ref MyClass obj,
        ref JsonReaderUtf16 reader,
        bool isEnd)
    {
        if (key is "number")
            obj.number = reader.ReadValue<int>(isEnd);
        else if (key is "name")
            obj.name = reader.ReadValue<string>(isEnd);
        else if (key is "list")
            obj.list = reader.ReadValue<int[]>(isEnd);
    }
}
```

### Using IFixedSerdeJsonFormatter\<T\>

`IFixedSerdeJsonFormatter` fits every type with a known maximum length. Such a type can safely use unsafe code to speed up serialization/deserialization. As a rule, `IFixedSerdeJsonFormatter` is not meant for Json objects or arrays (their length is hard to bound) but for Json values such as String and Number. Take a Hero whose HP never exceeds 999 and whose Name is never longer than 10 bytes, represented as `{hp}-{name}`:

```C#
using PeroTools2.Serde.Json;
public struct Hero
{
    public int    hp;
    public string name;

    public Hero(int hp, string name)
    {
        if (hp > 999 || name.Length > 10)
        {
            throw new Exception("Invalid HP or Name");
        }

        (this.hp, this.name) = (hp, name);
    }
}

public class HeroFormatter : IFixedSerdeJsonFormatter<Hero>
{
    // "999-xxxxxxxxxx" is never longer than 16 bytes
    public int GetMaxRequiredSizeUtf8() => 16;
    public int GetMaxRequiredSizeUtf16() => 16;

    public unsafe int SerializeUtf8Unsafe(Hero value, byte* dest)
    {
        // Same as UTF16
        return 0;
    }

    public unsafe int SerializeUtf16Unsafe(Hero value, char* dest)
    {
        var begin = dest;
        var name = value.name;
        dest += JsonWriter.WriteDoubleQuoteUtf16(dest);    // "
        dest += JsonWriter.WriteLongUtf16(dest, value.hp); // 100
        dest += JsonWriter.WriteChar(dest, '-');           // -
        for (var i = 0; i < name.Length; i++)
        {
            dest += JsonWriter.WriteChar(dest, name[i]);   // name
        }

        dest += JsonWriter.WriteDoubleQuoteUtf16(dest);    // "
        return (int) (dest - begin);
    }

    public unsafe Hero DeserializeUtf8Unsafe(byte* src, out int advance)
    {
        // Same as UTF16
        advance = 0;
        return default;
    }

    public unsafe Hero DeserializeUtf16Unsafe(char* src, out int advance)
    {
        var raw = JsonReader.ReadValueUntilEndUtf16(src, 16, out advance);
        if (raw[3] == '-') // three digit HP
        {
            var hp = SpanConverter.ToInt(raw.Slice(0, 3));
            var name = SpanConverter.ToString(raw.Slice(4));
            return new Hero(hp, name);
        }
        if (raw[2] == '-') // two digit HP
        {
            var hp = SpanConverter.ToInt(raw.Slice(0, 2));
            var name = SpanConverter.ToString(raw.Slice(3));
            return new Hero(hp, name);
        }
        if (raw[1] == '-') // one digit HP
        {
            var hp = SpanConverter.ToInt(raw.Slice(0, 1));
            var name = SpanConverter.ToString(raw.Slice(2));
            return new Hero(hp, name);
        }
        throw new Exception("Invalid format: " + raw.ToString());
    }
}
```

## Built-in Formatters

### StringPrimitiveFormatter/NullableStringPrimitiveFormatter

By default a PrimitiveType other than String and Char is not deserialized from a String JsonValue. Use `StringPrimitiveFormatter`/`NullableStringPrimitiveFormatter` when it has to be:

```C#
[SerdeJsonSerializable]
public partial class MyType
{
    [SerdeJsonFormatter(typeof(StringPrimitiveFormatter))]
    public long a;
    [SerdeJsonFormatter(typeof(NullableStringPrimitiveFormatter))]
    public long? b;
}
// Without it: {"a":12,"b":13}
// With it:    {"a":"12","b":"13"}
```

### HexColorFormatter

The default ColorFormatter writes `({r:float},{g:float},{b:float},{a:float})`. Use `HexColorFormatter` to serialize/deserialize the HTML hex format `"#ffffffff"` instead:

```C#
[SerdeJsonSerializable]
public partial class MyType
{
    [SerdeJsonFormatter(typeof(HexColorFormatter))]
    public Color color;
}
// Without it: {"color":"(0.34,0.25,0.12,0.5)"}
// With it:    {"color":"#57401F80"}
```

### UnixMillisecondsFormatter

The default DateTime is written in UTC format. `UnixMillisecondsFormatter` writes a millisecond timestamp instead:

```C#
[SerdeJsonSerializable]
public partial class MyType
{
    [SerdeJsonFormatter(typeof(UnixMillisecondsFormatter))]
    public DateTime time;
}
// Without it: {"time":"2025/1/14 5:13:39"}
// With it:    {"time":1736831619562}
```

`UnixMillisecondsFormatter` only covers 2001-09-09 09:46:40 to 2286-11-21 01:46:39.

### UnixSecondsFormatter

The default DateTime is written in UTC format. `UnixSecondsFormatter` writes a second timestamp instead:

```C#
[SerdeJsonSerializable]
public partial class MyType
{
    [SerdeJsonFormatter(typeof(UnixSecondsFormatter))]
    public DateTime time;
}
// Without it: {"time":"2025/1/14 5:15:47"}
// With it:    {"time":1736831747}
```

`UnixSecondsFormatter` only covers 2001-09-09 09:46:40 to 2286-11-21 01:46:39.

### RoundTripFloatFormatter/RoundTripDoubleFormatter

The default floating point Formatter favors speed and does not guarantee round-trip precision. Use `RoundTripFloatFormatter`/`RoundTripDoubleFormatter` when ["R" style](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#standard-format-specifiers) round-trip output is required (they are slower):

```C#
[SerdeJsonSerializable]
public partial class MyType
{
    [SerdeJsonFormatter(typeof(RoundTripDoubleFormatter))]
    public double speed;
}
```

## Json formatting

### FormatJson (string json): format the given Json data

Formats the input Json string into the standard indented form and returns the formatted string.

```C#
// Raw JSON string
string simpleJson = "{\"name\":\"John\",\"age\":30,\"isStudent\":false}";
// Formatted JSON
string formattedJson = IndentFormatHelper.FormatJson(simpleJson);
Debug.Log(formattedJson);
```

## Notes

### Allocations

PeroSerde does not allocate extra memory in the vast majority of cases, except for:

1. Deserializing a type that is `IReadOnlyCollection<T>`, is not `System.Collections.ObjectModel.ReadOnlyCollection<T>`, and does not implement `ICollection` (Stack, Queue, concurrent containers, ...) allocates a temporary container. Serde has no way to add elements to such a collection, so it fills a temporary `List<T>`.

2. Serializing `KeyValuePair<TK, TV>` or `Dictionary<TK, TV>` allocates a string when TK is not a primitive and its Formatter is neither `IFixedSerdeJsonFormatter<TKey>` nor `EncodingJsonFormatter<TKey>`, because an unknown key type falls back to `ToString()`.

3. Enumerating a `System.Collections.Generic.Dictionary` boxes the enumerator unless `DictionaryFormatter` is used.

4. A string that needs escaping may allocate once.

The convenience `Serialize` methods always return a new `string`/`byte[]`, which is the allocation reported in the benchmarks below; write through `JsonWriter` directly to avoid it.

### Formatter limitations

When you use [SerdeJsonSerializableAttribute](#serdejsonserializableattribute-generate-the-formatter-automatically) to generate a Formatter, the type must have a public parameterless constructor. The following is not allowed:

```C#
[SerdeJsonSerializable]
public partial class MyType
{
    public int number;
    public MyType(int num) => number = num;
}
```

A member that is a property must have a setter of any accessibility, a member that is a field must not be readonly, unless you use a custom Formatter:

```C#
using PeroTools2.Serde.Json;
[SerdeJsonSerializable]
public partial class MyObject
{
    public readonly int id;                // not serialized/deserialized
    public string name { get; }            // not serialized/deserialized
    public List<MyObject> list;
    public Dictionary<string, MyObject> dict;
}
```

If you implement a custom Formatter and want it to be the global default, register it yourself:

```C#
public class MyType
{
    // Lazy registration
    static MyType()
    {
        SerdeJsonFormatterProvider.Register(new MyFormatter());
    }

    // Eager registration
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void RegisterMyFormatter()
    {
        SerdeJsonFormatterProvider.Register(new MyFormatter());
    }
}

public class MyFormatter : ISerdeJsonFormatter<MyType>
{
    // ...
}
```

For members whose type implements `ICollection<T>` or `IReadOnlyCollection<T>`: if the element type is not a [built-in type](#built-in-formatters), is not marked with [SerdeJsonSerializableAttribute](#serdejsonserializableattribute-generate-the-formatter-automatically) and was never [registered](#formatter-limitations), serialization/deserialization fails:

```C#
[SerdeJsonSerializable]
public partial class MyClass
{
    public List<NeverRegisterType> value;
}
public class NeverRegisterType { } // never registered

public void Test(MyClass obj)
{
    JsonConverter.UTF16.Serialize(obj); // throws
}
```

## Benchmarks

| Runtime              | Tool                                                                                                                                       | Sources                                |
|----------------------|--------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------|
| .NET 9               | [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) 0.14.0                                                                        | `Bench/Console`                        |
| Unity Editor (Mono)  | [com.unity.test-framework.performance](https://docs.unity3d.com/Packages/com.unity.test-framework.performance@3.0/manual/index.html) 3.0.3 | `Bench/Unity/SerdePerformanceTests.cs` |
| Mono / IL2CPP player | stopwatch harness running inside a real player build                                                                                       | `Bench/Unity/SerdeBench.cs`            |

Hardware: Intel Core i5-12490F, Windows 11, 64 GB RAM. System.Text.Json is not available on the Unity Mono/IL2CPP profiles, and Swifter.Json cannot be compiled by IL2CPP, so each table lists the serializers that the runtime supports.

### Reproducing

1. `.NET Core`:
   ```bash
   dotnet run -c Release --project Bench/Console/PeroTools2.Serde.Bench.csproj -- --filter '*'
   ```
   The tables are written to `BenchmarkDotNet.Artifacts/results/*.md`.

2. `Unity Editor (Mono)`: copy `Bench/Unity/*.cs` into a Unity project that references the package, add `com.unity.test-framework.performance` and `org.nuget.system.collections.immutable` to `Packages/manifest.json`, drop `Swifter.Json.dll` and `Swifter.Core.dll` (netstandard2.1) into `Assets/Plugins`, then
   ```bash
   Unity.exe -batchmode -nographics -projectPath <project> -runTests -testPlatform EditMode \
             -testFilter PeroTools2.Serde.Benchmarks.Unity.JsonPerformanceTests -testResults results.xml
   ```
   The `Time` sample group of each test holds the numbers.

3. `Mono / IL2CPP player`:
   ```bash
   Unity.exe -batchmode -nographics -projectPath <project> -executeMethod PeroTools2.Serde.Benchmarks.Unity.BenchPlayerBuilder.BuildIl2Cpp
   PeroSerdeBench.exe -serde-bench -batchmode -nographics
   ```
   `BenchPlayerBuilder.BuildMono` builds the same player with the Mono backend, and the measurement is written to `serde-bench.csv` next to the executable.

### .NET 9

Serialization:

| Method                          |      Mean | Ratio | Allocated |
|---------------------------------|----------:|------:|----------:|
| Swifter.Json (UTF16 string)     |  455.8 ns |  1.00 |     968 B |
| Swifter.Json (UTF8 bytes)       |  473.4 ns |  1.04 |     592 B |
| PeroSerde (UTF16 string)        |  486.1 ns |  1.07 |     776 B |
| PeroSerde (UTF8 bytes)          |  499.8 ns |  1.10 |     400 B |
| System.Text.Json (UTF8 bytes)   |  696.0 ns |  1.53 |     712 B |
| System.Text.Json (UTF16 string) |  740.7 ns |  1.62 |    1088 B |
| Newtonsoft.Json (UTF16 string)  | 1315.9 ns |  2.89 |    3384 B |

Deserialization:

| Method                          |      Mean | Ratio | Allocated |
|---------------------------------|----------:|------:|----------:|
| Swifter.Json (UTF16 string)     |  519.3 ns |  1.00 |    1072 B |
| Swifter.Json (UTF8 bytes)       |  591.0 ns |  1.14 |    1072 B |
| PeroSerde (UTF16 string)        |  565.9 ns |  1.09 |     968 B |
| PeroSerde (UTF8 bytes)          |  642.4 ns |  1.24 |     968 B |
| System.Text.Json (UTF16 string) | 1151.8 ns |  2.22 |    1616 B |
| System.Text.Json (UTF8 bytes)   | 1030.4 ns |  1.98 |    1616 B |
| Newtonsoft.Json (UTF16 string)  | 2190.4 ns |  4.22 |    4088 B |

### Mono (Unity Editor)

| Case                            | PeroSerde UTF16 | PeroSerde UTF8 | Swifter.Json UTF16 | Swifter.Json UTF8 | Newtonsoft.Json | UnityEngine.JsonUtility |
|---------------------------------|----------------:|---------------:|-------------------:|------------------:|----------------:|------------------------:|
| Serialize (375 B)               |          7.8 µs |         8.3 µs |             7.0 µs |            8.4 µs |         25.4 µs |                  6.0 µs |
| Deserialize (375 B)             |         11.7 µs |        13.5 µs |             8.0 µs |            9.2 µs |         35.2 µs |                 10.0 µs |
| Serialize (1000 items, 73 KB)   |         1.90 ms |        1.72 ms |            1.42 ms |           1.58 ms |         9.53 ms |                 0.62 ms |
| Deserialize (1000 items, 73 KB) |         2.37 ms |        2.94 ms |            1.71 ms |           1.76 ms |        11.02 ms |                 1.38 ms |

### Mono / IL2CPP player

Mono backend:

| Case                            | PeroSerde UTF16 | PeroSerde UTF8 | Swifter.Json UTF16 | Swifter.Json UTF8 | Newtonsoft.Json | UnityEngine.JsonUtility |
|---------------------------------|----------------:|---------------:|-------------------:|------------------:|----------------:|------------------------:|
| Serialize (375 B)               |         3.08 µs |        2.62 µs |            1.91 µs |           1.80 µs |        12.78 µs |                 4.71 µs |
| Deserialize (375 B)             |         4.10 µs |        4.20 µs |            3.39 µs |           3.59 µs |        19.05 µs |                 8.38 µs |
| Serialize (1000 items, 73 KB)   |        0.572 ms |       0.499 ms |           0.479 ms |          0.531 ms |         3.56 ms |                0.626 ms |
| Deserialize (1000 items, 73 KB) |        0.575 ms |       0.569 ms |           0.460 ms |          0.735 ms |         4.96 ms |                 1.26 ms |

IL2CPP backend:

| Case                            | PeroSerde UTF16 | PeroSerde UTF8 | Newtonsoft.Json | UnityEngine.JsonUtility |
|---------------------------------|----------------:|---------------:|----------------:|------------------------:|
| Serialize (375 B)               |         2.63 µs |        2.74 µs |        13.32 µs |                 6.42 µs |
| Deserialize (375 B)             |         5.99 µs |        5.61 µs |        23.02 µs |                 8.64 µs |
| Serialize (1000 items, 73 KB)   |        0.522 ms |       0.524 ms |         3.99 ms |                 0.85 ms |
| Deserialize (1000 items, 73 KB) |         0.87 ms |        0.76 ms |         4.03 ms |                 0.98 ms |
