/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2026/11/04
 */

using System;
using System.Collections.Generic;
using PeroTools2.Serde.Json;

namespace PeroTools2.Serde.Benchmarks.Unity
{
    /// <summary>
    /// The payload used by the .NET Core benchmarks, kept identical so the numbers are comparable.
    /// Both BenchmarkDotNet and the Unity performance tests serialize these types.
    /// </summary>
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

    /// <summary>A 1000 element payload, used for the throughput oriented measurements.</summary>
    [Serializable]
    [SerdeJsonSerializable]
    public partial class BigObject
    {
        public List<BigItem> items;

        public static BigObject Create()
        {
            var big = new BigObject { items = new List<BigItem>() };
            for (var i = 0; i < 1000; i++)
            {
                big.items.Add(new BigItem { id = i, name = "item-" + i, weight = i * 1.5f, price = i * 3.25, enabled = (i & 1) == 0 });
            }
            return big;
        }
    }

    [Serializable]
    [SerdeJsonSerializable]
    public partial class BigItem
    {
        public int id;
        public string name;
        public float weight;
        public double price;
        public bool enabled;
    }
}
