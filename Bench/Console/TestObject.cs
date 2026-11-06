/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/04
 */

using System;
using System.Collections.Generic;
using PeroTools2.Serde.Json;

namespace PeroTools2.Serde.Benchmarks;

/// <summary>The payload every benchmark in this project serializes, taken from the readme.</summary>
[Serializable]
[SerdeJsonSerializable]
public partial class TestObject
{
    public string stringValue;
    public int intValue;
    public float floatValue;
    public bool boolValue;
    public List<string> listValue;
    public OtherObject objValue;

    public static readonly TestObject instance = new()
    {
        intValue = 12310,
        stringValue = "Hello, World! That was great!",
        boolValue = true,
        floatValue = 3.14159265358979323846f,
        listValue = new List<string>
        {
            "1st Hello, World!", "2nd Hello, World!", "3rd Hello, World!", "4th Hello, World!", "5th Hello, World!"
        },
        objValue = new OtherObject
        {
            charValue = 'O',
            arrayValue = new[] {"1st Hello, World!", "2nd Hello, World!", "3rd Hello, World!", "4th Hello, World!", "5th Hello, World!"},
            enumValue = TestEnum.BEnum
        }
    };
}

[Serializable]
[SerdeJsonSerializable]
public partial class OtherObject
{
    public char charValue;
    public string[] arrayValue;
    public TestEnum enumValue;
}

[Serializable]
[SerdeJsonSerializable]
public enum TestEnum
{
    AEnum,
    BEnum,
    CEnum
}
