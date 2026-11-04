/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: Gatongone
 * Created On: 2024/12/23-17:41:00
 */

using System;

namespace PeroTools2.Serde.Json;

/// <summary>
/// Automatically adds a Formatter for this type
/// </summary>
[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Enum,
    Inherited = false
)]
public sealed class SerdeJsonSerializableAttribute : Attribute
{
    /// <param name="ignoreMissingMembers">If true, missing members are ignored during deserialization</param>
    /// <param name="ignoreNullMembers">If true, members whose value is null are ignored during serialization</param>
    public SerdeJsonSerializableAttribute(bool ignoreMissingMembers = true, bool ignoreNullMembers = false) { }
}

/// <summary>
/// Tells Serde which name to use when serializing and deserializing JSON
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SerdeJsonMemberAttribute : Attribute
{
    /// <summary>
    /// Tells Serde which name to use when serializing and deserializing JSON
    /// </summary>
    /// <param name="formatName">The name to use</param>
    public SerdeJsonMemberAttribute(string formatName) { }
}

/// <summary>
/// Tells Serde to ignore this property or field when serializing and deserializing JSON
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SerdeJsonIgnoreMemberAttribute : Attribute;

/// <summary>
/// Tells Serde which JSON formatter to use when serializing and deserializing JSON
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class SerdeJsonFormatterAttribute : Attribute
{
    /// <summary>
    /// Tells Serde which JSON formatter to use when serializing and deserializing JSON
    /// </summary>
    /// <param name="formatterType">The formatter type</param>
    public SerdeJsonFormatterAttribute(Type formatterType) { }
}

/// <summary>
/// Tells Serde how to provide the default serializer and deserializer for an enum type
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public class SerdeJsonEnumFormatterAttribute : Attribute
{
    /// <summary>
    /// Tells Serde how to provide the default serializer and deserializer for an enum type
    /// </summary>
    /// <param name="serializationFormat">The format to use when serializing</param>
    /// <param name="deserializationFormat">The format to use when deserializing</param>
    public SerdeJsonEnumFormatterAttribute(SerdeEnumFormat serializationFormat = SerdeEnumFormat.Number, SerdeEnumFormat deserializationFormat = SerdeEnumFormat.Number) { }
}

public enum SerdeEnumFormat
{
    /// <summary>
    /// Tells Serde to provide the enum serializer or deserializer using the string form
    /// </summary>
    String = 0,

    /// <summary>
    /// Tells Serde to provide the enum serializer or deserializer using the numeric form
    /// </summary>
    Number = 1
}