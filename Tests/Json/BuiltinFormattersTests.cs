/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/25
 */

using System.Collections.Generic;
using PeroTools2.Serde.Share.Tests;

namespace PeroTools2.Serde.Json.Tests;

public sealed class BuiltinFormattersTests
{
    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void BoolTypeSerializeTest(Encoding encoding)
    {
        var obj = new BoolType(true);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<BoolType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void BoolTypeDeserializeTest(Encoding encoding)
    {
        var obj = new BoolType(true);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<BoolType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<BoolType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void CharTypeSerializeTest(Encoding encoding)
    {
        var obj = new CharType('\ufff0');
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<CharType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void CharTypeDeserializeTest(Encoding encoding)
    {
        var obj = new CharType('\ufff0');
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<CharType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<CharType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ByteTypeSerializeTest(Encoding encoding)
    {
        var obj = new ByteType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ByteType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ByteTypeDeserializeTest(Encoding encoding)
    {
        var obj = new ByteType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<ByteType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<ByteType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void SbyteTypeSerializeTest(Encoding encoding)
    {
        var obj = new SbyteType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<SbyteType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void SbyteTypeDeserializeTest(Encoding encoding)
    {
        var obj = new SbyteType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<SbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<SbyteType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ShortTypeSerializeTest(Encoding encoding)
    {
        var obj = new ShortType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ShortType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ShortTypeDeserializeTest(Encoding encoding)
    {
        var obj = new ShortType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<ShortType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<ShortType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UshortTypeSerializeTest(Encoding encoding)
    {
        var obj = new UshortType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UshortType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UshortTypeDeserializeTest(Encoding encoding)
    {
        var obj = new UshortType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<UshortType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<UshortType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void IntTypeSerializeTest(Encoding encoding)
    {
        var obj = new IntType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<IntType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void IntTypeDeserializeTest(Encoding encoding)
    {
        var obj = new IntType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<IntType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<IntType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UintTypeSerializeTest(Encoding encoding)
    {
        var obj = new UintType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UintType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UintTypeDeserializeTest(Encoding encoding)
    {
        var obj = new UintType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<UintType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<UintType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void LongTypeSerializeTest(Encoding encoding)
    {
        var obj = new LongType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<LongType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void LongTypeDeserializeTest(Encoding encoding)
    {
        var obj = new LongType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<LongType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<LongType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UlongTypeSerializeTest(Encoding encoding)
    {
        var obj = new UlongType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UlongType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UlongTypeDeserializeTest(Encoding encoding)
    {
        var obj = new UlongType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<UlongType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<UlongType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void FloatTypeSerializeTest(Encoding encoding)
    {
        var obj = new FloatType(10.4f);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<FloatType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void FloatTypeDeserializeTest(Encoding encoding)
    {
        var obj = new FloatType(10.4f);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<FloatType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<FloatType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DoubleTypeSerializeTest(Encoding encoding)
    {
        var obj = new DoubleType(10.4d);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DoubleType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DoubleTypeDeserializeTest(Encoding encoding)
    {
        var obj = new DoubleType(10.4d);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<DoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<DoubleType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DecimalTypeSerializeTest(Encoding encoding)
    {
        var obj = new DecimalType(13/2);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DecimalType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DecimalTypeDeserializeTest(Encoding encoding)
    {
        var obj = new DecimalType(13/2);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<DecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<DecimalType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableBoolTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableBoolType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableBoolType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableBoolTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableBoolType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableBoolType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableBoolType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableCharTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableCharType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableCharType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableCharTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableCharType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableCharType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableCharType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableByteTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableByteType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableByteType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableByteTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableByteType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableByteType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableByteType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableSbyteTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableSbyteType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableSbyteType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableSbyteTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableSbyteType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableSbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableSbyteType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableShortTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableShortType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableShortType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableShortTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableShortType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableShortType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableShortType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableUshortTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableUshortType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableUshortType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableUshortTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableUshortType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableUshortType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableUshortType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableIntTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableIntType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableIntType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableIntTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableIntType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableIntType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableIntType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableUintTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableUintType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableUintType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableUintTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableUintType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableUintType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableUintType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableLongTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableLongType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableLongType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableLongTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableLongType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableLongType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableLongType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableUlongTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableUlongType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableUlongType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableUlongTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableUlongType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableUlongType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableUlongType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableFloatTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableFloatType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableFloatType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableFloatTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableFloatType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableFloatType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableFloatType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableDoubleTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableDoubleType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableDoubleType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableDoubleTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableDoubleType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableDoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableDoubleType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableDecimalTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableDecimalType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableDecimalType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableDecimalTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableDecimalType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableDecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableDecimalType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringBoolTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringBoolType(true);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringBoolType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringBoolTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringBoolType(true);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringBoolType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringBoolType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringByteTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringByteType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringByteType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringByteTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringByteType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringByteType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringByteType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringSbyteTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringSbyteType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringSbyteType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringSbyteTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringSbyteType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringSbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringSbyteType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringShortTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringShortType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringShortType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringShortTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringShortType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringShortType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringShortType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringUshortTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringUshortType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringUshortType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringUshortTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringUshortType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringUshortType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringUshortType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringIntTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringIntType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringIntType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringIntTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringIntType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringIntType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringIntType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringUintTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringUintType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringUintType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringUintTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringUintType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringUintType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringUintType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringLongTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringLongType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringLongType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringLongTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringLongType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringLongType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringLongType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringUlongTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringUlongType(10);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringUlongType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringUlongTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringUlongType(10);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringUlongType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringUlongType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringFloatTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringFloatType(10.4f);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringFloatType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringFloatTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringFloatType(10.4f);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringFloatType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringFloatType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringDoubleTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringDoubleType(10.4d);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringDoubleType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringDoubleTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringDoubleType(10.4d);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringDoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringDoubleType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringDecimalTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringDecimalType(13/2);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringDecimalType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringDecimalTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringDecimalType(13/2);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringDecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringDecimalType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableBoolTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableBoolType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableBoolType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableBoolTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableBoolType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableBoolType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableBoolType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableByteTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableByteType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableByteType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableByteTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableByteType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableByteType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableByteType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableSbyteTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableSbyteType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableSbyteType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableSbyteTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableSbyteType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableSbyteType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableSbyteType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableShortTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableShortType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableShortType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableShortTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableShortType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableShortType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableShortType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableUshortTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableUshortType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableUshortType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableUshortTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableUshortType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableUshortType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableUshortType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableIntTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableIntType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableIntType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableIntTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableIntType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableIntType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableIntType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableUintTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableUintType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableUintType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableUintTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableUintType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableUintType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableUintType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableLongTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableLongType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableLongType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableLongTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableLongType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableLongType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableLongType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableUlongTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableUlongType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableUlongType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableUlongTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableUlongType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableUlongType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableUlongType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableFloatTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableFloatType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableFloatType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableFloatTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableFloatType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableFloatType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableFloatType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableDoubleTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableDoubleType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableDoubleType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableDoubleTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableDoubleType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableDoubleType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableDoubleType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableDecimalTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNullableDecimalType(null);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNullableDecimalType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNullableDecimalTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNullableDecimalType(null);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNullableDecimalType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNullableDecimalType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringType("String");
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringType("String");
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DateTimeTypeSerializeTest(Encoding encoding)
    {
        var obj = new DateTimeType(DateTime.Now);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DateTimeType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DateTimeTypeDeserializeTest(Encoding encoding)
    {
        var obj = new DateTimeType(DateTime.Now);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<DateTimeType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<DateTimeType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UnixMillisecondDateTimeTypeSerializeTest(Encoding encoding)
    {
        var obj = new UnixMillisecondDateTimeType(DateTime.Now);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UnixMillisecondDateTimeType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UnixMillisecondDateTimeTypeDeserializeTest(Encoding encoding)
    {
        var obj = new UnixMillisecondDateTimeType(DateTime.Now);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<UnixMillisecondDateTimeType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<UnixMillisecondDateTimeType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UnixSecondDateTimeTypeSerializeTest(Encoding encoding)
    {
        var obj = new UnixSecondDateTimeType(DateTime.Now);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<UnixSecondDateTimeType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void UnixSecondDateTimeTypeDeserializeTest(Encoding encoding)
    {
        var obj = new UnixSecondDateTimeType(DateTime.Now);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<UnixSecondDateTimeType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<UnixSecondDateTimeType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void GuidTypeSerializeTest(Encoding encoding)
    {
        var obj = new GuidType(Guid.NewGuid());
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<GuidType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void GuidTypeDeserializeTest(Encoding encoding)
    {
        var obj = new GuidType(Guid.NewGuid());
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<GuidType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<GuidType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void BoolTypeArraySerializeTest(Encoding encoding)
    {
        var obj = new BoolType[] { new(true), new(false) };
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<BoolType[]>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void BoolTypeArrayDeserializeTest(Encoding encoding)
    {
        var obj = new BoolType[] { new(true), new(false) };
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<BoolType[]>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<BoolType[]>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ListBoolTypeSerializeTest(Encoding encoding)
    {
        var obj = new List<BoolType> { new(true), new(false) };
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<List<BoolType>>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ListBoolTypeDeserializeTest(Encoding encoding)
    {
        var obj = new List<BoolType> { new(true), new(false) };
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<List<BoolType>>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<List<BoolType>>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ListNumberTypeSerializeTest(Encoding encoding)
    {
        var obj = new ListNumberType(new List<int> { 1, 2, 3, 4 });
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ListNumberType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ListNumberTypeDeserializeTest(Encoding encoding)
    {
        var obj = new ListNumberType(new List<int> { 1, 2, 3, 4 });
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<ListNumberType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<ListNumberType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ListObjectTypeSerializeTest(Encoding encoding)
    {
        var obj = new ListObjectType(new List<BoolType> { new BoolType(true), new BoolType(false) });
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<ListObjectType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void ListObjectTypeDeserializeTest(Encoding encoding)
    {
        var obj = new ListObjectType(new List<BoolType> { new BoolType(true), new BoolType(false) });
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<ListObjectType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<ListObjectType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DictionaryNumberTypeSerializeTest(Encoding encoding)
    {
        var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DictionaryNumberType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DictionaryNumberTypeDeserializeTest(Encoding encoding)
    {
        var obj = new DictionaryNumberType(new Dictionary<int, BoolType> { {1, new BoolType(true)}, {2, new BoolType(false)} });
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<DictionaryNumberType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<DictionaryNumberType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DictionaryStringTypeSerializeTest(Encoding encoding)
    {
        var obj = new DictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<DictionaryStringType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void DictionaryStringTypeDeserializeTest(Encoding encoding)
    {
        var obj = new DictionaryStringType(new Dictionary<string, BoolType> { {"True", new BoolType(true)}, {"False", new BoolType(false)} });
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<DictionaryStringType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<DictionaryStringType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NumberEnumTypeSerializeTest(Encoding encoding)
    {
        var obj = new NumberEnumType(NumberEnum.BEnum);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NumberEnumType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NumberEnumTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NumberEnumType(NumberEnum.BEnum);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NumberEnumType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NumberEnumType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringEnumTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringEnumType(StringEnum.BEnum);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringEnumType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringEnumTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringEnumType(StringEnum.BEnum);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringEnumType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringEnumType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNumberEnumTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringNumberEnumType(StringNumberEnum.BEnum);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringNumberEnumType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringNumberEnumTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringNumberEnumType(StringNumberEnum.BEnum);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringNumberEnumType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringNumberEnumType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NumberStringEnumTypeSerializeTest(Encoding encoding)
    {
        var obj = new NumberStringEnumType(NumberStringEnum.BEnum);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NumberStringEnumType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NumberStringEnumTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NumberStringEnumType(NumberStringEnum.BEnum);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NumberStringEnumType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NumberStringEnumType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NumberFlagsTypeSerializeTest(Encoding encoding)
    {
        var obj = new NumberFlagsType(NumberFlags.BFlag | NumberFlags.CFlag);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NumberFlagsType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NumberFlagsTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NumberFlagsType(NumberFlags.BFlag | NumberFlags.CFlag);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NumberFlagsType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NumberFlagsType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringFlagsTypeSerializeTest(Encoding encoding)
    {
        var obj = new StringFlagsType(StringFlags.BFlag | StringFlags.CFlag);
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<StringFlagsType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void StringFlagsTypeDeserializeTest(Encoding encoding)
    {
        var obj = new StringFlagsType(StringFlags.BFlag | StringFlags.CFlag);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<StringFlagsType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<StringFlagsType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void LazyTypeSerializeTest(Encoding encoding)
    {
        var obj = new LazyType(new Lazy<string>("LazyString"));
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<LazyType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void LazyTypeDeserializeTest(Encoding encoding)
    {
        var obj = new LazyType(new Lazy<string>("LazyString"));
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<LazyType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<LazyType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableTypeSerializeTest(Encoding encoding)
    {
        var obj = new NullableType(new BoolType(true));
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<NullableType>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void NullableTypeDeserializeTest(Encoding encoding)
    {
        var obj = new NullableType(new BoolType(true));
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<NullableType>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<NullableType>(json);
        Assert.AreEqual(obj, newObj);
    }


    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void GenericTypeCollectionSerializeTest(Encoding encoding)
    {
        var obj = new GenericTypeCollection(new List<GenericType<int>> { new GenericType<int>(1), new GenericType<int>(2) });
        var json = encoding == Encoding.UTF8
            ? System.Text.Encoding.UTF8.GetString(JsonConverter.UTF8.Serialize(obj))
            : JsonConverter.UTF16.Serialize(obj);
        Assert.AreEqual(obj, Newtonsoft.Json.JsonConvert.DeserializeObject<GenericTypeCollection>(json));
    }

    [TestCase(Encoding.UTF8)]
    [TestCase(Encoding.UTF16)]
    public void GenericTypeCollectionDeserializeTest(Encoding encoding)
    {
        var obj = new GenericTypeCollection(new List<GenericType<int>> { new GenericType<int>(1), new GenericType<int>(2) });
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var newObj = encoding == Encoding.UTF8
            ? JsonConverter.UTF8.Deserialize<GenericTypeCollection>(System.Text.Encoding.UTF8.GetBytes(json))
            : JsonConverter.UTF16.Deserialize<GenericTypeCollection>(json);
        Assert.AreEqual(obj, newObj);
    }


}